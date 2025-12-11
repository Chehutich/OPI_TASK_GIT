using System;
using System.Collections.Generic;

namespace QuizApp.Models
{
    // --- Клас Користувача (Студента) ---
    public class User
    {
        public string FirstName { get; set; } // Ім'я
        public string LastName { get; set; }  // Прізвище
        // Повне ім'я для зручності (щоб показувати в таблицях)
        public string FullName => $"{LastName} {FirstName}";

        // Історія проходження тестів цим студентом
        public List<QuizResult> History { get; set; } = new List<QuizResult>();

        public User() { }

        public User(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }

    // --- Клас Результату ---
    public class QuizResult
    {
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public DateTime DateTaken { get; set; }
    }

    // --- Клас Тесту ---
    public class Quiz
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    // --- Клас Питання ---
    public class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
        public int Points { get; set; } = 100;

        public Question() { }

        public Question(string text, List<string> options, int correctIndex)
        {
            Text = text;
            Options = options;
            CorrectOptionIndex = correctIndex;
        }
    }
}