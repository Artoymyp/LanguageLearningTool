using System.Collections.Generic;
using System.Linq;
using LanguageLearningTool.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LanguageLearningTool.UnitTests
{
	
	[TestClass]
	public class QuizViewModelTests
	{
        Question CreateQuestion()
        {
            return new Question("Question1", new []{new Answer("A1"), new Answer("A2")}, 1);
        }

        List<Question> CreateTwoQuestions()
        {
            return new List<Question>
            {
                CreateQuestion(),
                CreateQuestion(),
            };
        }

        List<Question> CreateThreeQuestions()
        {
            return new List<Question>
            {
                CreateQuestion(),
                CreateQuestion(),
                CreateQuestion(),
            };
        }

        QuizViewModel CreateQuiz(IEnumerable<Question> questions, INavigationService navigationService = null)
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
    }
}
