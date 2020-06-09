using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LanguageLearningTool.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int NextNewQuestion { get; set; }
    }

    public class QuestionInfo
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        readonly string _databasePath;

        public DbSet<QuestionInfo> Questions { get; set; }

        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
        }
    }

    public interface IDbService
    {
        Question PeekNewQuestion();

        void PromoteToNewQuestion();
    }

    public class DbServiceImplementation : IDbService
    {
        public Question PeekNewQuestion()
        {
            throw new NotImplementedException();
        }

        public void PromoteToNewQuestion()
        {
            throw new NotImplementedException();
        }
    }

    public class PersonalQuizRepositoryItem
    {
        public Question Question { get; set; }
        public int AskedCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class QuizGenerator
    {
        readonly ApplicationDbContext _context;
        readonly GrammarQuizRoot _grammarQuizRoot;

        public QuizGenerator(GrammarQuizRoot grammarQuizRoot)
        {
            _grammarQuizRoot = grammarQuizRoot;
        }

        public QuizGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Question> Provide()
        {
            return _grammarQuizRoot.Themes.SelectMany(theme => theme.QuestionGroups.SelectMany(group => @group.Questions)).ToList();
        }

        public QuestionInfo StartLearningNewQuestion()
        {
            throw new NotImplementedException();
        }
    }
}
