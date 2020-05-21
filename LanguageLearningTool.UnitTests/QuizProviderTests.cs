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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageLearningTool.UnitTests
{
    

	[TestClass]
    public class QuizProviderTests
    {
        [TestMethod]
        public void ShowsAllQuestionsFromTheQuizData()
        {
            // arrange
            var root = new GrammarQuizRoot();
            var grammarTheme = new GrammarTheme {Name = "theme1"};
            var questionGroup = new QuestionGroup {Name = "group1"};
            var question = new Question {Text = "question"};
            questionGroup.Questions.Add(question);
            grammarTheme.QuestionGroups.Add(questionGroup);
            root.Themes.Add(grammarTheme);

            // act
            var actualQuestions = new QuizGenerator(root).Provide();

            // assert
            var expectedQuestions = new List<Question>
            {
                question
            };
            CollectionAssert.AreEqual(expectedQuestions, actualQuestions);
        }
    }
}
