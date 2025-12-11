using System;
using System.Drawing;
using System.Windows.Forms;
using QuizApp.Services;
using QuizApp.Models;

namespace QuizApp.Forms
{
    public partial class MainForm : Form
    {
        private Label lblWelcome;
        private Button btnLogout;
        private FlowLayoutPanel panelQuizzes; // Контейнер для кнопок тестів

        public MainForm()
        {
            SetupUI();
            LoadQuizzes(); // Завантажуємо список тестів при старті
        }

        private void SetupUI()
        {
            // 1. Налаштування вікна
            this.Text = "Головне меню";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke; // Трохи сірий фон, щоб білі кнопки виділялися

            // 2. Верхня панель (Header)
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 60;
            headerPanel.BackColor = Color.White;
            this.Controls.Add(headerPanel);

            // 3. Привітання
            lblWelcome = new Label();
            // Беремо ім'я з DataManager, або "Гість", якщо раптом null
            string userName = DataManager.CurrentUser != null ? DataManager.CurrentUser.FirstName : "Гість";
            lblWelcome.Text = $"Привіт, {userName}!";
            lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblWelcome.ForeColor = Color.DarkSlateBlue;
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(20, 15);
            headerPanel.Controls.Add(lblWelcome);

            // 4. Кнопка Вихід
            btnLogout = new Button();
            btnLogout.Text = "Вихід";
            btnLogout.Size = new Size(80, 30);
            btnLogout.Location = new Point(this.ClientSize.Width - 100, 15);
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Приклеїти до правого краю
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.BackColor = Color.IndianRed;
            btnLogout.ForeColor = Color.White;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) =>
            {
                // Повертаємось на логін
                LoginForm login = new LoginForm();
                login.Show();
                this.Close(); // Закриваємо меню
            };
            headerPanel.Controls.Add(btnLogout);

            // 5. Контейнер для тестів (FlowLayoutPanel)
            // Це "розумна" панель, яка сама розставляє кнопки в ряд
            panelQuizzes = new FlowLayoutPanel();
            panelQuizzes.Dock = DockStyle.Fill; // Займає весь простір під хедером
            panelQuizzes.AutoScroll = true; // Якщо тестів багато - з'явиться прокрутка
            panelQuizzes.Padding = new Padding(20); // Відступи від країв
            this.Controls.Add(panelQuizzes);
            // Переносимо панель на передній план (хоча вона і так під Top)
            panelQuizzes.BringToFront();
        }

        // Метод, який створює кнопку для кожного тесту
        private void LoadQuizzes()
        {
            panelQuizzes.Controls.Clear(); // Чистимо, щоб не дублювати

            // Перевірка: якщо тестів немає
            if (DataManager.Quizzes.Count == 0)
            {
                Label lblEmpty = new Label();
                lblEmpty.Text = "На жаль, доступних тестів поки немає.";
                lblEmpty.AutoSize = true;
                lblEmpty.Font = new Font("Segoe UI", 12);
                panelQuizzes.Controls.Add(lblEmpty);
                return;
            }

            // Проходимось по кожному тесту в базі
            foreach (var quiz in DataManager.Quizzes)
            {
                Button btnQuiz = CreateQuizButton(quiz);
                panelQuizzes.Controls.Add(btnQuiz);
            }
        }

        // Дизайн картки тесту
        private Button CreateQuizButton(Quiz quiz)
        {
            Button btn = new Button();
            btn.Size = new Size(220, 150); // Розмір картки
            btn.BackColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            // Робимо гарний текст на кнопці: Назва + Категорія
            btn.Text = $"{quiz.Title}\n\n({quiz.Category})";
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.ForeColor = Color.DarkSlateBlue;
            btn.Margin = new Padding(10); // Відступ між кнопками

            // Подія натискання: Почати цей тест
            btn.Click += (sender, e) => StartQuiz(quiz);

            return btn;
        }

        private void StartQuiz(Quiz quiz)
        {
            // Створюємо форму тестування, передаючи обраний тест
            QuizForm quizForm = new QuizForm(quiz);

            // Ховаємо меню, поки йде тест
            this.Hide();

            // Показуємо форму тесту як діалог (програма чекає, поки він не закриється)
            quizForm.ShowDialog();

            // Коли тест закрився - знову показуємо меню
            this.Show();
        }

        // Закриття всієї програми при закритті меню
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // Якщо ми просто закриваємо хрестиком, а не через Вихід, то закриваємо все
            if (Application.OpenForms.Count == 0) Application.Exit();
        }
    }
}