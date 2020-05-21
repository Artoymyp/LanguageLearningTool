using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageLearningTool.Models
{
    public class GrammarTheme
    {
        public string Name { get; set; }
        public List<QuestionGroup> QuestionGroups { get; } = new List<QuestionGroup>();
    }
}
