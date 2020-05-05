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
		Dictionary<MenuItemType, NavigationPage> MenuPages = new Dictionary<MenuItemType, NavigationPage>();

		public MainPage()
		{
			InitializeComponent();

			MasterBehavior = MasterBehavior.Popover;

			MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
		}

		public async Task NavigateFromMenu(MenuItemType id)
		{
			if (!MenuPages.ContainsKey(id)) {
				switch (id) {
					case MenuItemType.Browse:
						MenuPages.Add(id, new NavigationPage(new ItemsPage()));
						break;
					case MenuItemType.About:
						MenuPages.Add(id, new NavigationPage(new AboutPage()));
						break;
					case MenuItemType.Quiz:
                    {
                        MenuPages.Add(id, new NavigationPage(new QuizContentPage()));
                        break;
                    }
				}
			}

			var newPage = MenuPages[id];

			if (newPage != null && Detail != newPage) {
				Detail = newPage;

				if (Device.RuntimePlatform == Device.Android)
					await Task.Delay(100);

				IsPresented = false;
			}
		}
	}
}