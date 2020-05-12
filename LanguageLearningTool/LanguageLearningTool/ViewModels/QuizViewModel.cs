using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels
{
    public class Answer : ViewModelBase
    {
        Color _backgroundColor;
        bool _isSelected;
        bool _isCorrect;
        bool _isEnabled = true;

        public Answer(string text = "")
        {
            Text = text;
        }
        public string Text { get; set; }

        public bool IsCorrect
        {
            get { return _isCorrect; }
            set
            {
                _isCorrect = value;
                UpdateBackgroundColor();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                UpdateBackgroundColor();
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                UpdateBackgroundColor();
            }
        }

        public Xamarin.Forms.Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetProperty(ref _backgroundColor, value); }
        }

        Xamarin.Forms.Color EvaluateBackgroundColor()
        {
            if (IsEnabled) {
                if (IsSelected) {
                    return Color.BurlyWood;
                }
            }
            else {
                if (IsCorrect) {
                    return Color.Green;
                }
                if (IsSelected) {
                    return Color.DarkRed;
                }
            }
            return Color.Transparent;
        }

        public void UpdateBackgroundColor()
        {
            BackgroundColor = EvaluateBackgroundColor();
        }
    }
    public class Question : ViewModelBase
    {
        Answer _selectedItem;

        public Question(string text, IEnumerable<Answer> answers)
        {
            Text = text;
            foreach (Answer answer in answers) {
                Answers.Add(answer);
            }
        }
        public string Text { get; set; }
        public IList<Answer> Answers { get; private set; } = new List<Answer>();
        public bool IsCorrect()
        {
            foreach (Answer answer in Answers) {
                if (answer.IsCorrect != answer.IsSelected) {
                    return false;
                }
            }

            return true;
        }

        public Answer SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != null && value == null) {
                    // ListView.SelectedItem's binding is overriding ViewModel's property on initialization with null.
                    // Because of that the initial selection is lost. This is a simplest solution to preserve the original value of ViewModel's property.
                    OnPropertyChanged();
                    return;
                }
                if (_selectedItem != value) {
                    var oldSelectedItem = _selectedItem;
                    if (SetProperty(ref _selectedItem, value)) {
                        if (oldSelectedItem != null) {
                            oldSelectedItem.IsSelected = false;
                        }


                        if (_selectedItem != null) {
                            _selectedItem.IsSelected = true;
                        }
                    }
                }
            }
        }
    }

    public class QuizViewModel : NavigatableViewModelBase<Views.QuizContentPage>
    {
        readonly INavigationService _navigationService;
        Question _currentQuestion;
        string _progress;
        string _nextButtonCaption;
        bool _prevButtonIsEnabled;
        bool _canSelectAnswers = true;

        public QuizViewModel(IEnumerable<Question> questions, INavigationService navigationService)
        {
            Title = "Grammar quiz";
            _navigationService = navigationService;
            QuestionIndex = -1;
            Questions.AddRange(questions);
            MoveToNextQuestion();
        }

        public Question CurrentQuestion
        {
            get { return _currentQuestion; }
            set { SetProperty(ref _currentQuestion, value); }
        }

        List<Question> Questions { get; } = new List<Question>();

        public int QuestionIndex { get; set; }

        public string Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        public ICommand PrevButtonCommand
        {
            get { return new Command(MoveToPrevQuestion); }
        }

        public ICommand NextButtonCommand
        {
            get { return new Command(MoveToNextQuestion); }
        }

        public string NextButtonCaption
        {
            get { return _nextButtonCaption; }
            set { SetProperty(ref _nextButtonCaption, value); }
        }

        public bool PrevButtonIsEnabled
        {
            get { return _prevButtonIsEnabled; }
            set { SetProperty(ref _prevButtonIsEnabled, value); }
        }

        public bool CanSelectAnswers
        {
            get { return _canSelectAnswers; }
            set
            {
                if (SetProperty(ref _canSelectAnswers, value)) {
                    foreach (Question question in Questions) {
                        foreach (Answer answer in question.Answers) {
                            answer.IsEnabled = value;
                        }
                    }
                }
            }
        }

        public const string NextQuizQuestionButtonText = "Next question";
        public const string ShowQuizResultButtonText = "Finish";

        void UpdateProgress()
        {
            Progress = string.Format("Question #{0} of {1}", QuestionIndex + 1, Questions.Count);
        }

        void MoveToNextQuestion()
        {
            if (QuestionIndex < Questions.Count - 1) {
                QuestionIndex++;
                MoveToQuestion(QuestionIndex);
            }
            else {
                var resultVm = new QuizResultViewModel
                {
                    TestRate = Questions.Average(q=>q.IsCorrect() ? 1.0 : 0.0)
                };
                _navigationService.NavigateTo(resultVm);
                CanSelectAnswers = false;
            }
        }

        void MoveToPrevQuestion()
        {
            if (QuestionIndex > 0) {
                QuestionIndex--;
                MoveToQuestion(QuestionIndex);
            }
        }

        void MoveToQuestion(int questionIndex)
        {
            NextButtonCaption = questionIndex < Questions.Count - 1 ? NextQuizQuestionButtonText : ShowQuizResultButtonText;
            PrevButtonIsEnabled = questionIndex != 0;
            CurrentQuestion = Questions[QuestionIndex];
            UpdateProgress();
        }
    }
}
