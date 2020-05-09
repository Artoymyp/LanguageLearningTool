using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels
{
    public class Answer
    {
        public Answer(string text = "")
        {
            Text = text;
        }
        public string Text { get; set; }
    }
    public class Question : ViewModelBase
    {
        public Question(string text, IEnumerable<Answer> answers, int correctAnswerIndex)
        {
            Text = text;
            foreach (Answer answer in answers) {
                Answers.Add(answer);
            }
            CorrectAnswerIndex = correctAnswerIndex;
        }
        public string Text { get; set; }
        public IList<Answer> Answers { get; private set; } = new List<Answer>();
        public int SelectedAnswerIndex { get; set; }
        public int CorrectAnswerIndex { get; private set; }
    }
    public class QuizViewModel1 : QuizViewModel
    {
        QuizViewModel1():base(null, null) { }
    }
    public class QuizViewModel : NavigatableViewModelBase<Views.QuizContentPage>
    {
        readonly INavigationService _navigationService;
        Question _currentQuestion;
        string _progress;
        string _nextButtonCaption;
        bool _prevButtonIsEnabled;

        public QuizViewModel(IEnumerable<Question> questions, INavigationService navigationService)
        {
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
                var resultVm = new QuizResultViewModel();
                _navigationService.NavigateTo(resultVm);
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
