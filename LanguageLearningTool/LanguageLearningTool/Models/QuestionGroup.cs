using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageLearningTool.Models
{
    public class QuestionGroup
    {
        public string Name { get; set; }
        public string Grammar { get; set; }
        public List<Question> Questions { get; } = new List<Question>();
    }
}
