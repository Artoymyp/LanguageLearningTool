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
        public string Text { get; set; }
    }
    public class Question : BaseViewModel
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

    public class QuizViewModel : BaseViewModel
    {
        Question _currentQuestion;
        string _progress;

        public QuizViewModel(IEnumerable<Question> questions)
        {
            QuestionIndex = -1;
            CurrentQuestion = null;
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

        public ICommand PrevQuestionCommand
        {
            get { return new Command(MoveToPrevQuestion); }
        }

        public ICommand NextQuestionCommand
        {
            get { return new Command(MoveToNextQuestion); }
        }

        void UpdateProgress()
        {
            Progress = string.Format("Question #{0} of {1}", QuestionIndex + 1, Questions.Count);
        }

        public void MoveToNextQuestion()
        {
            if (QuestionIndex < Questions.Count - 1) {
                QuestionIndex++;
                CurrentQuestion = Questions[QuestionIndex];
                UpdateProgress();
            }
        }

        public void MoveToPrevQuestion()
        {
            if (QuestionIndex > 0) {
                QuestionIndex--;
                CurrentQuestion = Questions[QuestionIndex];
                UpdateProgress();
            }
        }
    }
}
