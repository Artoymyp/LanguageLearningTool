using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LanguageLearningTool.Models;
using LanguageLearningTool.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageLearningTool.UnitTests
{
    [TestClass]
    public class QuizProviderTests
    {
        ApplicationDbContext Context;
        IDbContextTransaction Transaction;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new ApplicationDbContext("some.db");
            Context.Database.EnsureCreated();
            Transaction = Context.Database.BeginTransaction();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Transaction != null) {
                Transaction.Dispose();
                Transaction = null;
            }
            if (Context != null) {
                Context.Database.CloseConnection();
                Context.Database.EnsureDeleted();
                Context.Dispose();
                Context = null;
            }
        }

        [TestMethod]
        public void QuestionsAreLearnedInDefaultOrder()
        {
            // arrange
            Context.Questions.Add(new QuestionInfo());
            Context.Questions.Add(new QuestionInfo());
            Context.SaveChanges();

            Context.Users.Add(new User());
            Context.SaveChanges();

            // act
            var quizGenerator = new QuizGenerator(Context);
            var questions = new List<QuestionInfo>
            {
                quizGenerator.StartLearningNewQuestion(),
                quizGenerator.StartLearningNewQuestion()
            };

            // assert
            Assert.AreEqual(1, questions[0].Id);
            Assert.AreEqual(2, questions[1].Id);
        }

        [TestMethod]
        public void UserStartsLearningFromTheFirstQuestion()
        {
            // arrange
            Context.Users.Add(new User());
            Context.SaveChanges();

            // act
            var user = Context.Users.First();

            // assert
            Assert.AreEqual(1, user.NextNewQuestionId);
        }

        [TestMethod]
        public void LearningNewQuestionAdvancesNextNewQuestionForUser()
        {
            // arrange
            Context.Questions.Add(new QuestionInfo());
            Context.Users.Add(new User());
            Context.SaveChanges();

            // act
            new QuizGenerator(Context).StartLearningNewQuestion();

            // assert
            var user = Context.Users.First();
            Assert.AreEqual(2, user.NextNewQuestionId);
        }

        [TestMethod]
        public void LearningNewQuestionProvidesNextNewQuestionAvailableForUser()
        {
            // arrange
            Context.Questions.Add(new QuestionInfo());
            Context.Users.Add(new User());
            Context.SaveChanges();

            var user = Context.Users.First();
            var expectedNextNewQuestionId = user.NextNewQuestionId;

            // act
            var newNewQuestion = new QuizGenerator(Context).StartLearningNewQuestion();

            // assert
            Assert.AreEqual(expectedNextNewQuestionId, newNewQuestion.Id);
        }

        [TestMethod]
        public void IfNoNewQuestionAvailableDoesNotAdvanceNextNewQuestion()
        {
            // arrange
            Context.Users.Add(new User());
            Context.SaveChanges();
            var user = Context.Users.First();
            var expectedNextNewQuestion = user.NextNewQuestionId;

            // act
            var newQuestion = new QuizGenerator(Context).StartLearningNewQuestion();

            // assert
            user = Context.Users.First();
            Assert.IsNull(newQuestion);
            Assert.AreEqual(expectedNextNewQuestion, user.NextNewQuestionId);
        }
    }
}
