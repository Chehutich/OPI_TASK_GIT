using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class ResultForm : Form
    {
        private Quiz currentQuiz;
        private int myScore;
        private Panel podiumPanel;
        private DataGridView gridOthers;

        public ResultForm(Quiz quiz, int score)
        {
            this.currentQuiz = quiz;
            this.myScore = score;
            SetupUI();
            LoadLeaderboard();
            ThemeHelper.ApplyGradient(this);
        }

        private void SetupUI()
        {
            this.Text = "Результати";
            this.Size = new Size(800, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitle = new Label
            {
                Text = $"🏆 Турнірна таблиця: {currentQuiz.Title}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitle);

            Label lblMyResult = new Label
            {
                Text = $"Твій результат: {myScore} балів",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 60)
            };
            this.Controls.Add(lblMyResult);

            podiumPanel = new Panel { Location = new Point(20, 100), Size = new Size(740, 240), BackColor = Color.Transparent };
            this.Controls.Add(podiumPanel);

            gridOthers = new DataGridView { Location = new Point(50, 360), Size = new Size(680, 250) };
            ThemeHelper.StyleGridView(gridOthers); // Застосовуємо стиль таблиці

            gridOthers.Columns.Add("Rank", "#");
            gridOthers.Columns.Add("Name", "Учасник");
            gridOthers.Columns.Add("Score", "Бали");
            gridOthers.Columns.Add("Date", "Дата");
            gridOthers.Columns[0].Width = 50; gridOthers.Columns[1].Width = 300;
            this.Controls.Add(gridOthers);

            Button btnHome = new Button { Text = "🏠 В меню", Size = new Size(200, 45), Location = new Point(290, 640) };
            ThemeHelper.StyleButton(btnHome, ThemeHelper.PrimaryColor);
            // Щоб кнопка була видна на темному фоні, зробимо її світлішою або білою рамкою
            btnHome.BackColor = Color.White; btnHome.ForeColor = ThemeHelper.PrimaryColor;
            btnHome.Click += (s, e) => this.Close();
            this.Controls.Add(btnHome);
        }

        private void LoadLeaderboard()
        {
            var allResults = new List<LeaderboardItem>();
            foreach (var user in DataManager.Users)
                foreach (var record in user.History)
                    if (record.QuizTitle == currentQuiz.Title)
                        allResults.Add(new LeaderboardItem { UserName = user.FullName, Score = record.Score, Date = record.DateTaken });

            allResults = allResults.OrderByDescending(x => x.Score).ToList();

            if (allResults.Count > 0) CreatePodiumBox(1, allResults[0], Color.Gold, 280, 20, "🥇");
            if (allResults.Count > 1) CreatePodiumBox(2, allResults[1], Color.Silver, 50, 45, "🥈");
            if (allResults.Count > 2) CreatePodiumBox(3, allResults[2], Color.SandyBrown, 510, 45, "🥉");

            if (allResults.Count > 3)
            {
                gridOthers.Visible = true;
                for (int i = 3; i < allResults.Count; i++)
                {
                    var item = allResults[i];
                    gridOthers.Rows.Add(i + 1, item.UserName, item.Score, item.Date.ToShortDateString());
                }
            }
            else gridOthers.Visible = false;
        }

        private void CreatePodiumBox(int place, LeaderboardItem item, Color color, int xPos, int yPos, string icon)
        {
            Panel box = new Panel { Size = new Size(180, 160), Location = new Point(xPos, yPos), BackColor = Color.White };

            Label lblPlace = new Label { Text = icon, Font = new Font("Segoe UI", 30), Dock = DockStyle.Top, Height = 60, TextAlign = ContentAlignment.MiddleCenter };
            box.Controls.Add(lblPlace);

            Label lblName = new Label { Text = item.UserName, Font = new Font("Segoe UI", 11, FontStyle.Bold), Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, ForeColor = ThemeHelper.PrimaryColor };
            box.Controls.Add(lblName);

            Label lblScore = new Label { Text = $"{item.Score}", Font = new Font("Segoe UI", 12), Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray };
            box.Controls.Add(lblScore);

            // Кольорова смужка знизу
            Panel strip = new Panel { Dock = DockStyle.Bottom, Height = 10, BackColor = color };
            box.Controls.Add(strip);

            podiumPanel.Controls.Add(box);
        }
        private class LeaderboardItem { public string UserName { get; set; } public int Score { get; set; } public DateTime Date { get; set; } }
    }
}