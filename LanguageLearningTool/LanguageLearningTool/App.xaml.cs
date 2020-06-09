using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LanguageLearningTool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LanguageLearningTool.Services;
using LanguageLearningTool.ViewModels;
using LanguageLearningTool.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LanguageLearningTool
{
	public partial class App : Application
	{
		//TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
		public static string AzureBackendUrl = "http://localhost:5000";
		public static bool UseMockDataStore = true;
        readonly INavigationService _navigationService;
        static string LocalDbFilename = "Local.db";
        public static string UserName;
        static App()
        {
        }

        public App()
		{
			InitializeComponent();

            var navigationService = new NavigationServiceImplementation(new ReflectingViewLocator());
            _navigationService = navigationService;

            var dbPath = GetDbPath();
            bool exists = File.Exists(dbPath);
            if (exists) {
                File.Delete(dbPath);
            }
            exists = File.Exists(dbPath);
            List<User> users;
            using (var applicationContext = new ApplicationDbContext(dbPath)) {
                applicationContext.Database.EnsureCreated();
                applicationContext.Users.Add(new User() { Email = "a@b.com" });
                users = applicationContext.Users.ToList();
                applicationContext.SaveChanges();
            }
            using (var applicationContext = new ApplicationDbContext(dbPath)) {
                users = applicationContext.Users.ToList();
            }

            UserName = users[0].Email;
            exists = File.Exists(dbPath);
            if (exists) {
                File.Delete(dbPath);
            }

            var rootVm = new MainViewModel(_navigationService);
            var rootPage = new MainPage(rootVm);
            navigationService.SetRoot(rootPage);
            _navigationService.PresentAsNavigatableMainPage(rootVm.Detail);
            MainPage = rootPage;

			if (UseMockDataStore)
				DependencyService.Register<MockDataStore>();
			else
				DependencyService.Register<AzureDataStore>();

            //using (var applicationContext = new ApplicationDbContext(dbPath)) {
            //    users = applicationContext.Users.ToList();
            //}

            //using (var applicationDbContext = new ApplicationDbContext(dbPath)) {
            //    using (var tr = applicationDbContext.Database.BeginTransaction()) {
            //        users = applicationDbContext.Users.ToList();
            //        applicationDbContext.Users.Add(new User() { Email = "c@d.com" });
            //        users = applicationDbContext.Users.ToList();
            //        applicationDbContext.SaveChanges();
            //        users = applicationDbContext.Users.ToList();
            //    }
            //}
            //using (var applicationContext = new ApplicationDbContext(dbPath)) {
            //    users = applicationContext.Users.ToList();
            //}
            //using (var applicationDbContext = new ApplicationDbContext(dbPath)) {
            //    using (var tr = applicationDbContext.Database.BeginTransaction()) {
            //        users = applicationDbContext.Users.ToList();
            //        applicationDbContext.Users.Add(new User() { Email = "e@f.com" });
            //        users = applicationDbContext.Users.ToList();
            //        applicationDbContext.SaveChanges();
            //        users = applicationDbContext.Users.ToList();
            //        tr.Commit();
            //    }
            //}
            //int i = 1;
        }
        public static string GetDbPath()
        {
            var libFolder = FileSystem.AppDataDirectory;
            return Path.Combine(libFolder, LocalDbFilename);
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
