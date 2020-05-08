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
        public MainPage()
		{
			InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;
		}

        public MainPage(MainViewModel vm) : this()
        {
            BindingContext = vm;
        }
	}
}