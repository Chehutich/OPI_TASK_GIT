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
}