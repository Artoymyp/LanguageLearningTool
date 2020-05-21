using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageLearningTool.Models
{
    public class QuizGenerator
    {
        readonly GrammarQuizRoot _grammarQuizRoot;

        public QuizGenerator(GrammarQuizRoot grammarQuizRoot)
        {
            _grammarQuizRoot = grammarQuizRoot;
        }
        public List<Question> Provide()
        {
            return _grammarQuizRoot.Themes.SelectMany(theme => theme.QuestionGroups.SelectMany(group => @group.Questions)).ToList();
        }
    }
}
