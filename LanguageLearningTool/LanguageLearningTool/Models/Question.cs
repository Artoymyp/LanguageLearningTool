using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageLearningTool.Models
{
    public class Question
    {
        public string Text { get; set; }
        public List<Answer> Answers { get; } = new List<Answer>();
    }
}
