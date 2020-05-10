using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageLearningTool.ViewModels
{
    public class QuizResultViewModel : NavigatableViewModelBase<Views.QuizResultView>
    {
        double _testRate;

        public double TestRate
        {
            get { return _testRate; }
            set
            {
                _testRate = value;
                Message = string.Format("{0:0}%", value * 100);
            }
        }

        public string Message { get; private set; }
    }
}
