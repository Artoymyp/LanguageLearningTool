﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using LanguageLearningTool.Models;
using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        readonly INavigationService _navigationService;
        readonly MainViewModel _ownerVm;
        HomeMenuItem _selectedItem;
        readonly List<HomeMenuItem> _menuItems;

        public MenuViewModel(INavigationService navigationService, MainViewModel ownerVm)
        {
            _navigationService = navigationService;
            _ownerVm = ownerVm;
            _menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {ViewModel = new ItemsViewModel(navigationService), Title = "Browse"},
                new HomeMenuItem {ViewModel = new AboutViewModel(), Title = "About"},
                new HomeMenuItem
                {
                    ViewModel = new QuizViewModel(new[]
                        {
                            new QuestionViewModel("What is 'A'?", new[] {new AnswerViewModel {Text = "A", IsCorrect = true}, new AnswerViewModel {Text = "B"}, new AnswerViewModel {Text = "C"}, new AnswerViewModel {Text = "Maybe"}}),
                            new QuestionViewModel("Is it OK?", new[] {new AnswerViewModel {Text = "Yes"}, new AnswerViewModel {Text = "No", IsCorrect = true}}),
                        },
                        _navigationService),
                    Title = "Quiz"
                },
            };
            _selectedItem = MenuItems[0];
        }

        public List<HomeMenuItem> MenuItems
        {
            get { return _menuItems; }
        }

        public HomeMenuItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }

        public ICommand ItemSelectedCommand
        {
            get
            {
                return new Command<HomeMenuItem>(selectedItem=>
                {
                    _navigationService.NavigateTo(selectedItem.ViewModel);
                    if (_ownerVm != null) {
                        _ownerVm.IsMenuPresented = false;
                    }
                });
            }
        }
    }
}
