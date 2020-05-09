using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels {
    public abstract class NavigatableViewModelBase<TPage> : ViewModelBase where TPage: Page
    {
        protected NavigatableViewModelBase()
        {
            
        }
    }
}