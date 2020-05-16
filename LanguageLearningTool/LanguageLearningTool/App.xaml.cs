using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LanguageLearningTool.Services;
using LanguageLearningTool.ViewModels;
using LanguageLearningTool.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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
            const string appSecret = "ios=01cc33e2-1153-47f1-b372-57769388548f;" +
                                     "android=f25b5768-482f-413e-a1d5-1f36c5a157bd;" +
                                     "uwp=35055e63-2a8b-46f7-b4c3-5d0456408e81";
            AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("App started");
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
