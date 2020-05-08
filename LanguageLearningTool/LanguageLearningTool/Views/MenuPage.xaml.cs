using LanguageLearningTool.Models;
using System;
using System.Collections.Generic;
using LanguageLearningTool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LanguageLearningTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
		public MenuPage()
		{
            InitializeComponent();
		}

        public MenuPage(MenuViewModel vm) : this()
        {

        }

        void ListViewMenu_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((MenuViewModel)BindingContext).ItemSelectedCommand.Execute(e.SelectedItem);
        }
    }
}