using System;
using System.Collections.Generic;
using System.Text;
using LanguageLearningTool.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageLearningTool.UnitTests
{
    [TestClass]
    public class QuizDataReaderTests
    {
        [TestMethod]
        public void CanReadQuizDataXmlTest()
        {
            // arrange
            var reader = new QuizDataReader();

            // act
            GrammarQuizRoot quizData = reader.GetQuizData();

            // assert
            Assert.IsNotNull(quizData);
        }
    }
}
