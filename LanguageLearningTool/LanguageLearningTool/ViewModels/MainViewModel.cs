using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageLearningTool.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        MenuViewModel _menu;
        bool _isMenuPresented;

        public MainViewModel(INavigationService navigationService)
        {
            Menu = new MenuViewModel(navigationService, this);
            Detail = Menu.SelectedItem.ViewModel;
        }

        public MenuViewModel Menu
        {
            get { return _menu; }
            set { _menu = value; }
        }

        public ViewModelBase Detail { get; set; }

        public bool IsMenuPresented
        {
            get { return _isMenuPresented; }
            set { SetProperty(ref _isMenuPresented, value); }
        }
    }
}
