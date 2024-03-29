﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LanguageLearningTool.Models;
using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels
{
    public class AnswerViewModel : ViewModelBase
    {
        Color _backgroundColor;
        bool _isSelected;
        bool _isCorrect;
        bool _isEnabled = true;

        public AnswerViewModel(Answer answer)
        {
            Text = answer.Text;
            IsCorrect = answer.IsCorrect;
        }

        public AnswerViewModel(string text = "")
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
    public class QuestionViewModel : ViewModelBase
    {
        AnswerViewModel _selectedItem;

        public QuestionViewModel(Question question) : 
            this(question.Text, question.Answers.ConvertAll(a=>new AnswerViewModel(a)))
        { }

        public QuestionViewModel(string text, IEnumerable<AnswerViewModel> answers)
        {
            Text = text;
            foreach (AnswerViewModel answer in answers) {
                Answers.Add(answer);
            }
        }
        public string Text { get; set; }
        public IList<AnswerViewModel> Answers { get; private set; } = new List<AnswerViewModel>();
        public bool IsCorrect()
        {
            foreach (AnswerViewModel answer in Answers) {
                if (answer.IsCorrect != answer.IsSelected) {
                    return false;
                }
            }

            return true;
        }

        public AnswerViewModel SelectedItem
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
        QuestionViewModel _currentQuestion;
        string _progress;
        string _nextButtonCaption;
        bool _prevButtonIsEnabled;
        bool _canSelectAnswers = true;

        public QuizViewModel(GrammarQuizRoot quizRoot, INavigationService navigationService) :
            this(quizRoot?.
                    Themes.
                    SelectMany(t=>t.QuestionGroups).
                    SelectMany(g=>g.Questions).
                    Select(q=>new QuestionViewModel(q)), 
                 navigationService)
        { }

        public QuizViewModel(IEnumerable<QuestionViewModel> questions, INavigationService navigationService)
        {
            Title = "Grammar quiz";
            _navigationService = navigationService;
            QuestionIndex = -1;
            if (questions != null) {
                Questions.AddRange(questions);
            }
            MoveToNextQuestion();
        }

        public QuestionViewModel CurrentQuestion
        {
            get { return _currentQuestion; }
            set { SetProperty(ref _currentQuestion, value); }
        }

        List<QuestionViewModel> Questions { get; } = new List<QuestionViewModel>();

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
                    foreach (QuestionViewModel question in Questions) {
                        foreach (AnswerViewModel answer in question.Answers) {
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
