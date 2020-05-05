using LanguageLearningTool.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageLearningTool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LanguageLearningTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
	{
		Dictionary<MenuItemType, ContentPage> MenuPages = new Dictionary<MenuItemType, ContentPage>();
        NavigationPage RootNavigation;
		public MainPage()
		{
			InitializeComponent();

			MasterBehavior = MasterBehavior.Popover;

            RootNavigation = (NavigationPage)Detail;

            MenuPages.Add((int)MenuItemType.Browse, (ContentPage)RootNavigation.CurrentPage);
		}

		public async Task NavigateFromMenu(MenuItemType id)
		{
			if (!MenuPages.ContainsKey(id)) {
				switch (id) {
					case MenuItemType.Browse:
						MenuPages.Add(id, new ItemsPage());
						break;
					case MenuItemType.About:
						MenuPages.Add(id, new AboutPage());
						break;
					case MenuItemType.Quiz:
                    {
                        MenuPages.Add(id, new QuizContentPage());
                        break;
                    }
				}
			}

			var newPage = MenuPages[id];

            if (RootNavigation.RootPage == newPage) {
                await RootNavigation.PopToRootAsync();

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
			else if (newPage != null && RootNavigation.CurrentPage != newPage) {
				//Detail = newPage;
                await RootNavigation.PushAsync(newPage);

				if (Device.RuntimePlatform == Device.Android)
					await Task.Delay(100);

				IsPresented = false;
			}
            else if (RootNavigation.CurrentPage == newPage) {
                IsPresented = false;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);
            }
        }
	}
}