using System;
using System.Drawing;
using System.Windows.Forms;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class MainForm : Form
    {
        private Label lblWelcome;
        private FlowLayoutPanel panelQuizzes;

        public MainForm()
        {
            SetupUI();
            ThemeHelper.ApplyGradient(this); // Градієнт
            LoadQuizzes();
        }

        private void SetupUI()
        {
            this.Text = "Головне меню";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Хедер (Прозорий)
            Panel headerPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.Transparent };
            this.Controls.Add(headerPanel);

            string userName = DataManager.CurrentUser != null ? DataManager.CurrentUser.FirstName : "Гість";
            lblWelcome = new Label
            {
                Text = $"👋 Привіт, {userName}!",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White, // Білий текст на темному фоні
                AutoSize = true,
                Location = new Point(30, 25)
            };
            headerPanel.Controls.Add(lblWelcome);

            Button btnLogout = new Button { Text = "🚪 Вихід", Size = new Size(100, 40), Location = new Point(760, 25), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            ThemeHelper.StyleButton(btnLogout, Color.FromArgb(200, 50, 50)); // Червонуватий
            btnLogout.Click += (s, e) => { new LoginForm().Show(); this.Close(); };
            headerPanel.Controls.Add(btnLogout);

            panelQuizzes = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(30), BackColor = Color.Transparent };
            this.Controls.Add(panelQuizzes);
            panelQuizzes.BringToFront();
        }

        private void LoadQuizzes()
        {
            panelQuizzes.Controls.Clear();
            foreach (var quiz in DataManager.Quizzes)
            {
                panelQuizzes.Controls.Add(CreateQuizCard(quiz));
            }
        }

        private Button CreateQuizCard(Quiz quiz)
        {
            Button btn = new Button();
            btn.Size = new Size(250, 160);
            btn.BackColor = Color.White; // Біла картка
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(15);

            // Формуємо текст з іконкою
            string icon = quiz.Category == "Історія" ? "📜" : (quiz.Category == "Географія" ? "🌍" : "📝");
            btn.Text = $"{icon}\n\n{quiz.Title}\n\n▶️";

            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btn.ForeColor = ThemeHelper.PrimaryColor; // Фіолетовий текст

            // Ефект при наведенні
            btn.MouseEnter += (s, e) => { btn.BackColor = Color.FromArgb(240, 240, 255); btn.ForeColor = ThemeHelper.SecondaryColor; };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.White; btn.ForeColor = ThemeHelper.PrimaryColor; };

            btn.Click += (s, e) => StartQuiz(quiz);
            return btn;
        }

        private void StartQuiz(Quiz quiz)
        {
            QuizForm quizForm = new QuizForm(quiz);
            this.Hide();
            quizForm.ShowDialog();
            this.Show();
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { if (Application.OpenForms.Count == 0) Application.Exit(); }
    }
}