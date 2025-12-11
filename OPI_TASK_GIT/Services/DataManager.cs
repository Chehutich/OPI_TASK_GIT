using Newtonsoft.Json;
using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;


namespace QuizApp.Services
{
    public static class DataManager
    {
        // Шляхи до файлів
        private static string usersFile = "users.json";
        private static string quizzesFile = "quizzes.json";

        // Списки даних
        public static List<User> Users { get; set; } = new List<User>();
        public static List<Quiz> Quizzes { get; set; } = new List<Quiz>();

        // Поточний користувач
        public static User CurrentUser { get; set; }

        // Завантаження даних 
        public static void LoadData()
        {
            if (File.Exists(usersFile))
            {
                string json = File.ReadAllText(usersFile);
                Users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }

            if (File.Exists(quizzesFile))
            {
                string json = File.ReadAllText(quizzesFile);
                Quizzes = JsonConvert.DeserializeObject<List<Quiz>>(json) ?? new List<Quiz>();
            }
            else
            {

                CreateDemoData();
            }
        }

        //  Збереження даних 
        public static void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(Users, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(usersFile, json);
        }
    }
    public static void SaveQuizzes()
        {
            string json = JsonConvert.SerializeObject(Quizzes, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(quizzesFile, json);
        }


        private static void CreateDemoData()
        {
            //  Тест 1: Географія
            Quiz geo = new Quiz
            {
                Title = "Географія світу",
                Category = "Географія",
                Description = "Перевір свої знання країн та столиць!"
            };

            geo.Questions.Add(new Question("Столиця Франції?", new List<string> { "Берлін", "Мадрид", "Париж", "Рим" }, 2));
            geo.Questions.Add(new Question("Яка річка протікає через Київ?", new List<string> { "Дніпро", "Дунай", "Темза", "Ніл" }, 0));
            geo.Questions.Add(new Question("Найбільший океан на Землі?", new List<string> { "Атлантичний", "Індійський", "Тихий", "Північний Льодовитий" }, 2));
            geo.Questions.Add(new Question("Столиця Японії?", new List<string> { "Пекін", "Сеул", "Токіо", "Бангкок" }, 2));
            geo.Questions.Add(new Question("На якому материку знаходиться Єгипет?", new List<string> { "Євразія", "Африка", "Австралія", "Південна Америка" }, 1));
            geo.Questions.Add(new Question("Столиця США?", new List<string> { "Нью-Йорк", "Вашингтон", "Лос-Анджелес", "Чикаго" }, 1));
            geo.Questions.Add(new Question("Яка країна має форму чобота?", new List<string> { "Іспанія", "Греція", "Італія", "Португалія" }, 2));
            geo.Questions.Add(new Question("Найвища гора у світі?", new List<string> { "Говерла", "Кіліманджаро", "Еверест", "Альпи" }, 2));
            geo.Questions.Add(new Question("Столиця Великої Британії?", new List<string> { "Лондон", "Дублін", "Единбург", "Манчестер" }, 0));
            geo.Questions.Add(new Question("Де знаходяться піраміди?", new List<string> { "Мексика", "Індія", "Єгипет", "Китай" }, 2));

            // Тест 2: Історія України
            Quiz hist = new Quiz
            {
                Title = "Історія України",
                Category = "Історія",
                Description = "Основні дати та видатні постаті."
            };

            hist.Questions.Add(new Question("Коли була проголошена незалежність України?", new List<string> { "1990", "1991", "1996", "2004" }, 1));
            hist.Questions.Add(new Question("В якому році хрестили Русь?", new List<string> { "988", "1054", "1240", "882" }, 0));
            hist.Questions.Add(new Question("Хто був першим президентом України?", new List<string> { "Кучма", "Ющенко", "Кравчук", "Порошенко" }, 2));
            hist.Questions.Add(new Question("Коли прийняли Конституцію України?", new List<string> { "1991", "1996", "2000", "2014" }, 1));
            hist.Questions.Add(new Question("Гетьман, зображений на купюрі 5 гривень?", new List<string> { "Мазепа", "Хмельницький", "Сагайдачний", "Виговський" }, 1));
            hist.Questions.Add(new Question("Столиця Київської Русі?", new List<string> { "Чернігів", "Галич", "Київ", "Львів" }, 2));
            hist.Questions.Add(new Question("В якому році сталася аварія на ЧАЕС?", new List<string> { "1980", "1986", "1991", "2000" }, 1));
            hist.Questions.Add(new Question("Автор 'Кобзаря'?", new List<string> { "Іван Франко", "Леся Українка", "Тарас Шевченко", "Григорій Сковорода" }, 2));
            hist.Questions.Add(new Question("Як називалась козацька держава?", new List<string> { "Гетьманщина", "Запорізька Січ", "УНР", "Русь" }, 1));
            hist.Questions.Add(new Question("Грошова одиниця України?", new List<string> { "Рубль", "Долар", "Гривня", "Злотий" }, 2));

            Quizzes.Add(geo);
            Quizzes.Add(hist);
            SaveQuizzes();

        }
    }
}