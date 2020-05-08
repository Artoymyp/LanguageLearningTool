using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LanguageLearningTool.Views;
using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels
{
    public interface INavigationService
    {
        Task NavigateTo(BaseViewModel vm);

        void PresentAsNavigatableMainPage(BaseViewModel viewModel);
    }

    public interface IViewLocator
    {
        Page CreateAndBindPageFor<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel;
    }

    public class ReflectingViewLocator : IViewLocator
    {
        readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();

        public ReflectingViewLocator()
        {
            IEnumerable<Type> pageTypes = GetType().Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(Page).IsAssignableFrom(t));
            foreach (Type pageType in pageTypes) {
                foreach (ConstructorInfo constructor in pageType.GetConstructors()) {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    if (parameters.Length == 1) {
                        Type parameterType = parameters[0].ParameterType;
                        if (typeof(BaseViewModel).IsAssignableFrom(parameterType)) {
                            _map.Add(parameterType, pageType);
                        }
                    }
                }
            }
        }

        public Page CreateAndBindPageFor<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel
        {
            if (_map.TryGetValue(viewModel.GetType(), out Type pageType)) {
                object result = Activator.CreateInstance(pageType, BindingFlags.CreateInstance, null, new object[] {viewModel}, null);
                var page = (Page)result;
                page.BindingContext = viewModel;
                return page;
            }
            throw new NotImplementedException();
        }
    }

    public class NavigationServiceImplementation : INavigationService
    {
        INavigation Navigation
        {
            get
            {
                return _presentationRoot.Detail.Navigation;
            }
        }

        readonly IViewLocator _viewLocator;
        MasterDetailPage _presentationRoot;

        public NavigationServiceImplementation(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator;
            //_menuPages.Add(_presentationRoot.BindingContext.GetType(), _presentationRoot.MainPage);
        }

        Page CreateAndBindPageFor(BaseViewModel vm)
        {
            return _viewLocator.CreateAndBindPageFor(vm);
        }

        public async Task NavigateTo(BaseViewModel vm)
        {
            if (Navigation == null) {
                return;
            }


            if (Navigation.NavigationStack.First().BindingContext == vm) {
                await Navigation.PopToRootAsync();
            }
            else {
                if (Navigation.NavigationStack.Last().BindingContext != vm) {
                    if (CreateAndBindPageFor(vm) is Page page) {
                        await Navigation.PushAsync(page);
                    }
                }
            }

            if (Device.RuntimePlatform == Device.Android)
                await Task.Delay(100);
        }

        public void PresentAsNavigatableMainPage(BaseViewModel vm)
        {
            Page page = CreateAndBindPageFor(vm);

            var newNavigationPage = new NavigationPage(page);

            _presentationRoot.Detail = newNavigationPage;
        }

        public void SetRoot(MainPage rootPage)
        {
            _presentationRoot = rootPage;
        }
    }
}