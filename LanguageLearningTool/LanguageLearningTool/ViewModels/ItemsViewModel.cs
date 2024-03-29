﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using LanguageLearningTool.Models;
using LanguageLearningTool.Views;

namespace LanguageLearningTool.ViewModels
{
	public class ItemsViewModel : NavigatableViewModelBase<Views.ItemsPage>
    {
        readonly INavigationService _navigationService;

        public ObservableCollection<Item> Items { get; set; }
		public Command LoadItemsCommand { get; set; }

        public ICommand ItemSelectedCommand
        {
            get
            {
                return new Command<Item>(async item=>
                {
                    var itemViewModel = new ItemDetailViewModel(item);
                    await _navigationService.NavigateTo(itemViewModel);
                });
            }
        }

        public ItemsViewModel(INavigationService navigationService)
		{
            _navigationService = navigationService;
            Title = "Browse";
			Items = new ObservableCollection<Item>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

			MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
			{
				var newItem = item as Item;
				Items.Add(newItem);
				await DataStore.AddItemAsync(newItem);
			});
		}

		async Task ExecuteLoadItemsCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try {
				Items.Clear();
				var items = await DataStore.GetItemsAsync(true);
				foreach (var item in items) {
					Items.Add(item);
				}
			}
			catch (Exception ex) {
				Debug.WriteLine(ex);
			}
			finally {
				IsBusy = false;
			}
		}
	}
}