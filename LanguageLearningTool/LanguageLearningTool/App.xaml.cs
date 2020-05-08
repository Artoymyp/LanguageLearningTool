using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LanguageLearningTool.Services;
using LanguageLearningTool.ViewModels;
using LanguageLearningTool.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LanguageLearningTool
{
	public partial class App : Application
	{
		//TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
		public static string AzureBackendUrl = "http://localhost:5000";
		public static bool UseMockDataStore = true;
        readonly INavigationService _navigationService;

        static App()
        {
        }

        public App()
		{
			InitializeComponent();

            var navigationService = new NavigationServiceImplementation(new ReflectingViewLocator());
            _navigationService = navigationService;
            var rootVm = new MainViewModel(_navigationService);
            var rootPage = new MainPage(rootVm);
            navigationService.SetRoot(rootPage);
            _navigationService.PresentAsNavigatableMainPage(rootVm.Detail);
            MainPage = rootPage;

			if (UseMockDataStore)
				DependencyService.Register<MockDataStore>();
			else
				DependencyService.Register<AzureDataStore>();
        }

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
