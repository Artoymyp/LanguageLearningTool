using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LanguageLearningTool.Views;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels
{
    public interface INavigationService
    {
        Task NavigateTo(ViewModelBase vm);

        void PresentAsNavigatableMainPage(ViewModelBase viewModel);
    }

    public interface IViewLocator
    {
        Page CreateAndBindPageFor<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
    }

    public class ReflectingViewLocator : IViewLocator
    {
        readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();

        public ReflectingViewLocator()
        {
            IEnumerable<Type> vmTypes = 
                GetType()
                    .Assembly
                    .GetTypes()
                    .Where(t => t.BaseType != null)
                    .Where(t => t.BaseType.IsGenericType)
                    .Where(t => t.BaseType.GetGenericTypeDefinition() == typeof(NavigatableViewModelBase<>));

            foreach (Type vmType in vmTypes) {
                Type pageType = vmType.BaseType?.GetGenericArguments()[0];
                if (pageType != null) {
                    _map.Add(vmType, pageType);
                }
            }
        }

        public Page CreateAndBindPageFor<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
        {
            if (_map.TryGetValue(viewModel.GetType(), out Type pageType)) {
                object result = Activator.CreateInstance(pageType, BindingFlags.CreateInstance, null, null, null);
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

        Page CreateAndBindPageFor(ViewModelBase vm)
        {
            return _viewLocator.CreateAndBindPageFor(vm);
        }

        public async Task NavigateTo(ViewModelBase vm)
        {
            if (Navigation == null) {
                return;
            }
            
            if (vm != null) {
                Analytics.TrackEvent("Going to " + vm.GetType().Name);
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

        public void PresentAsNavigatableMainPage(ViewModelBase vm)
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