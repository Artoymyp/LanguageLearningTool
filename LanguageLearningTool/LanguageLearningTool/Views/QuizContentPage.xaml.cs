using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLearningTool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LanguageLearningTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class QuizContentPage : ContentPage
	{
		QuizViewModel _viewModel;

        public QuizContentPage()
        {
            InitializeComponent();

            _viewModel = new QuizViewModel(new[]
            {
                new Question("What is 'A'?", new []{ new Answer { Text = "A" }, new Answer { Text = "B" }, new Answer { Text = "C" }, new Answer { Text = "Maybe" } }, 0),
                new Question("Is it OK?", new []{ new Answer { Text = "Yes" }, new Answer { Text = "No" } }, 1),
            });

            BindingContext = _viewModel;
        }


  //      public QuizContentPage (QuizViewModel viewModel)
		//{
		//	_viewModel = viewModel;
		//	InitializeComponent ();
		//	BindingContext = _viewModel;
		//}
    }
}