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
		public QuizContentPage() : this(null)
        {
            
        }

        public QuizContentPage (QuizViewModel viewModel)
		{
			InitializeComponent ();
			BindingContext = viewModel;
		}
    }
}