using System;
using System.Threading.Tasks;

namespace LanguageLearningTool.ViewModels
{
    public interface INavigationService
    {
        Task NavigateToQuizResult(QuizViewModel vm);
    }

    public class NavigationServiceImplementation : INavigationService
    {
        public async Task NavigateToQuizResult(QuizViewModel vm)
        {
            throw new NotImplementedException(); 
        }
    }
}