using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LanguageLearningTool.Models;
using LanguageLearningTool.Views;
using LanguageLearningTool.ViewModels;

namespace LanguageLearningTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
        ItemsViewModel viewModel => (ItemsViewModel)BindingContext;

		public ItemsPage()
        {
            InitializeComponent();
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var item = args.SelectedItem as Item;
			if (item == null)
				return;

            viewModel.ItemSelectedCommand.Execute(args.SelectedItem);

            // Manually deselect item.
			ItemsListView.SelectedItem = null;
		}

		async void AddItem_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Items.Count == 0)
				viewModel.LoadItemsCommand.Execute(null);
		}
	}
}