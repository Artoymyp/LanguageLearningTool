using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LanguageLearningTool.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LanguageLearningTool.UnitTests
{
	
	[TestClass]
	public class QuizViewModelTests
    {
        QuestionViewModel CreateQuestion()
        {
            return new QuestionViewModel("Question1", new[] {new AnswerViewModel("A1"), new AnswerViewModel("A2") {IsCorrect = true}});
        }

        List<QuestionViewModel> CreateTwoQuestions()
        {
            return new List<QuestionViewModel>
            {
                CreateQuestion(),
                CreateQuestion(),
            };
        }

        List<QuestionViewModel> CreateThreeQuestions()
        {
            return new List<QuestionViewModel>
            {
                CreateQuestion(),
                CreateQuestion(),
                CreateQuestion(),
            };
        }

        QuizViewModel CreateQuiz(IEnumerable<QuestionViewModel> questions, INavigationService navigationService = null)
        {
            return new QuizViewModel(questions, navigationService);
        }

        [TestMethod]
        public void ShowsFirstQuestion_WhenStarts()
        {
            // arrange
            var questions = CreateTwoQuestions();
            var quizVm = CreateQuiz(questions);

            // assert
            Assert.AreEqual(questions.First(), quizVm.CurrentQuestion);
        }

        [TestMethod]
        public void ShowsNextQuestion_WhenMovedToNextQuestion()
        {
            // arrange
            var questions = CreateTwoQuestions();
            var quizVm = CreateQuiz(questions);

            // act
            quizVm.NextButtonCommand.Execute(null);

            // assert
            Assert.AreEqual(questions.Skip(1).First(), quizVm.CurrentQuestion);
        }

        [TestMethod]
        public void ShowsPrevQuestion_WhenMovedToPrevQuestion()
        {
            // arrange
            var questions = CreateTwoQuestions();
            var quizVm = CreateQuiz(questions);
            quizVm.NextButtonCommand.Execute(null);

            // act
            quizVm.PrevButtonCommand.Execute(null);

            // assert
            Assert.AreEqual(questions.First(), quizVm.CurrentQuestion);
        }

        [DataTestMethod]
        [DataRow(0, QuizViewModel.NextQuizQuestionButtonText)]
        [DataRow(1, QuizViewModel.NextQuizQuestionButtonText)]
        [DataRow(2, QuizViewModel.ShowQuizResultButtonText)]
        public void ShowsFinishButton_OnlyForFinalQuestion(int nextCommandCallCount, string expectedCommandName)
        {
            // arrange
            var questions = CreateThreeQuestions();
            var quizVm = CreateQuiz(questions);

            // act
            for(int questionIndex = 0; questionIndex < nextCommandCallCount; ++questionIndex) {
                quizVm.NextButtonCommand.Execute(null);
            }

            // assert
            Assert.AreEqual(expectedCommandName, quizVm.NextButtonCaption);
        }

        [TestMethod]
        public void ShowsNextButton_WhenMovedBackFromFinalQuestion()
        {
            // arrange
            var questions = CreateTwoQuestions();
            var quizVm = CreateQuiz(questions);
            quizVm.NextButtonCommand.Execute(null);

            // act
            quizVm.PrevButtonCommand.Execute(null);

            // assert
            Assert.AreEqual(QuizViewModel.NextQuizQuestionButtonText, quizVm.NextButtonCaption);
        }

        [TestMethod]
        public void DoesNotNavigateToQuizResults_UntilLastQuestion()
        {
            // arrange
            var questions = CreateTwoQuestions();
            var navigationServiceMock = new Mock<INavigationService>();
            var quizVm = CreateQuiz(questions, navigationServiceMock.Object);

            // act
            quizVm.NextButtonCommand.Execute(null);

            // assert
            navigationServiceMock.Verify(service => service.NavigateTo(It.IsAny<QuizResultViewModel>()), Times.Never);
        }

        [TestMethod]
        public void NavigatesToQuizResults_AfterLastQuestion()
        {
            // arrange
            var questions = CreateTwoQuestions();
            var navigationServiceMock = new Mock<INavigationService>();
            var quizVm = CreateQuiz(questions, navigationServiceMock.Object);

            // act
            quizVm.NextButtonCommand.Execute(null);
            quizVm.NextButtonCommand.Execute(null);

            // assert
            navigationServiceMock.Verify(service => service.NavigateTo(It.IsAny<QuizResultViewModel>()), Times.Once);
        }

        [TestMethod]
        public void ShowCorrectResult_AfterLastQuestion()
        {
            // arrange
            var questions = new List<QuestionViewModel>
            {
                new QuestionViewModel("Question1", new[] {new AnswerViewModel("A1"), new AnswerViewModel("A2") {IsCorrect = true}}),
            };
            var navigationServiceMock = new Mock<INavigationService>();
            var quizVm = CreateQuiz(questions, navigationServiceMock.Object);

            // act
            quizVm.CurrentQuestion.Answers[1].IsSelected = true;
            quizVm.NextButtonCommand.Execute(null);

            // assert
            navigationServiceMock.Verify(service => service.NavigateTo(It.Is<QuizResultViewModel>(vm=>Math.Abs(vm.TestRate - 1) < 0.01)), Times.Once);
        }

        [DataTestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        public void PrevButtonIsDisabled_OnlyForFirstQuestion(int nextCommandCallCount, bool expectedIsEnabled)
        {
            // arrange
            var questions = CreateTwoQuestions();
            var quizVm = CreateQuiz(questions);

            // act
            for (int questionIndex = 0; questionIndex < nextCommandCallCount; ++questionIndex) {
                quizVm.NextButtonCommand.Execute(null);
            }

            // assert
            Assert.AreEqual(expectedIsEnabled, quizVm.PrevButtonIsEnabled);
        }

        [TestMethod]
        public void CanSelectAnswers_Initially()
        {
            // arrange
            var questions = CreateQuestion();

            // act
            var quizVm = CreateQuiz(new[] {questions}, new Mock<INavigationService>().Object);

            // assert
            Assert.IsTrue(quizVm.CanSelectAnswers);
        }

        [TestMethod]
        public void CannotSelectAnswers_AfterResultsShown()
        {
            // arrange
            var questions = CreateQuestion();
            var quizVm = CreateQuiz(new[] { questions }, new Mock<INavigationService>().Object);

            // act
            quizVm.NextButtonCommand.Execute(null);

            // assert
            Assert.IsFalse(quizVm.CanSelectAnswers);
        }

        public static class MemberInfoGetting
        {
            public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
            {
                MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
                return expressionBody.Member.Name;
            }
        }

        Xamarin.Forms.Color GetColor(string name)
        {
            var memberInfos = typeof(Xamarin.Forms.Color).GetField(name);
            return (Xamarin.Forms.Color)memberInfos.GetValue(null);
        }

        [DataTestMethod]
        [DataRow("Transparent", false, false, false)]
        [DataRow("Transparent", false, false, true)]
        [DataRow("BurlyWood", false, true, false)]
        [DataRow("BurlyWood", false, true, true)]
        [DataRow("Transparent", true, false, false)]
        [DataRow("Green", true, false, true)]
        [DataRow("DarkRed", true, true, false)]
        [DataRow("Green", true, true, true)]
        public void AnswerIsGreen_WhenIsCorrectAndIsSelectedAndQuizIsFinished(
            string expectedColorString,
            bool quizIsComplete,
            bool isSelected,
            bool isCorrect
            )
        {
            // arrange
            var answer = new AnswerViewModel("A2");
            var questions = new[] {new QuestionViewModel("Question1", new[] {answer})};
            var quizVm = CreateQuiz(questions, new Mock<INavigationService>().Object);
            Xamarin.Forms.Color expectedColor = GetColor(expectedColorString);

            // act
            answer.IsCorrect = isCorrect;

            if (isSelected) {
                quizVm.CurrentQuestion.SelectedItem = answer;
            }

            quizVm.CanSelectAnswers = !quizIsComplete;

            // assert
            Assert.AreEqual(
                expectedColor,
                answer.BackgroundColor,
                string.Format("QuizIsComplete:{0}, AnswerIsSelected:{1}, AnswerIsCorrect:{2}", quizIsComplete, isSelected, isCorrect));
        }
    }
}
