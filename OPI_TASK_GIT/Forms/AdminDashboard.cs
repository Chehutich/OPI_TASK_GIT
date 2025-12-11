using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class AdminDashboard : Form
    {
        private DataGridView gridTests;
        private Button btnCreate;
        private Button btnEdit;
        private Button btnViewResults;
        private Button btnResetStats;
        private Button btnDeleteTest;
        private Button btnLogout;

        public AdminDashboard()
        {
            SetupUI();
            LoadTests();
        }

        private void SetupUI()
        {
            this.Text = "Адмін-панель: Керування тестами";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Label lblTitle = new Label();
            lblTitle.Text = "Панель керування";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.DarkSlateBlue;
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            gridTests = new DataGridView();
            gridTests.Location = new Point(20, 70);
            gridTests.Size = new Size(600, 500);
            gridTests.BackgroundColor = Color.White;
            gridTests.AllowUserToAddRows = false;
            gridTests.ReadOnly = true;
            gridTests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridTests.MultiSelect = false;
            gridTests.RowHeadersVisible = false;

            gridTests.Columns.Add("Title", "Назва тесту");
            gridTests.Columns[0].Width = 250;
            gridTests.Columns.Add("Category", "Категорія");
            gridTests.Columns[1].Width = 150;
            gridTests.Columns.Add("QCount", "Питань");
            gridTests.Columns[2].Width = 80;
            gridTests.Columns.Add("Attempts", "Проходжень");
            gridTests.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.Controls.Add(gridTests);

            // ПРАВА ПАНЕЛЬ
            int btnX = 640;
            int btnW = 220;
            int btnH = 50;
            int gap = 15;
            int startY = 70;

            // 1. Створити
            btnCreate = CreateButton("➕ Створити новий", Color.SeaGreen, btnX, startY);
            btnCreate.Click += BtnCreate_Click;
            this.Controls.Add(btnCreate);

            // 2. Редагувати
            btnEdit = CreateButton("✏️ Редагувати тест", Color.DarkGoldenrod, btnX, startY + btnH + gap);
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            // 3. Результати
            btnViewResults = CreateButton("📊 Результати", Color.RoyalBlue, btnX, startY + (btnH + gap) * 2);
            btnViewResults.Click += BtnViewResults_Click;
            this.Controls.Add(btnViewResults);

            // 4. Скинути статистику
            btnResetStats = CreateButton("🧹 Очистити дані", Color.Orange, btnX, startY + (btnH + gap) * 3);
            btnResetStats.Click += BtnResetStats_Click;
            this.Controls.Add(btnResetStats);

            // 5. Видалити
            btnDeleteTest = CreateButton("🗑 Видалити тест", Color.IndianRed, btnX, startY + (btnH + gap) * 4);
            btnDeleteTest.Click += BtnDeleteTest_Click;
            this.Controls.Add(btnDeleteTest);

            btnLogout = new Button();
            btnLogout.Text = "Вийти";
            btnLogout.Location = new Point(btnX, 540);
            btnLogout.Size = new Size(btnW, 30);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.ForeColor = Color.Gray;
            btnLogout.Click += (s, e) => { new LoginForm().Show(); this.Close(); };
            this.Controls.Add(btnLogout);
        }

        private Button CreateButton(string text, Color color, int x, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.Size = new Size(220, 50);
            btn.Location = new Point(x, y);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadTests()
        {
            gridTests.Rows.Clear();
            if (DataManager.Quizzes.Count == 0) return;

            foreach (var quiz in DataManager.Quizzes)
            {
                int attempts = 0;
                foreach (var u in DataManager.Users)
                {
                    attempts += u.History.Where(h => h.QuizTitle == quiz.Title).Count();
                }
                gridTests.Rows.Add(quiz.Title, quiz.Category, quiz.Questions.Count, attempts);
            }
        }

        // ЛОГІКА 

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            AdminForm createForm = new AdminForm(null);
            this.Hide();
            createForm.ShowDialog();
            this.Show();
            LoadTests();
        }

        // РЕДАГУВАННЯ
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (gridTests.SelectedRows.Count == 0)
            {
                MessageBox.Show("Оберіть тест для редагування!", "Увага");
                return;
            }

            string title = gridTests.SelectedRows[0].Cells[0].Value.ToString();
            var quizToEdit = DataManager.Quizzes.FirstOrDefault(q => q.Title == title);

            if (quizToEdit != null)
            {
                AdminForm editForm = new AdminForm(quizToEdit);
                this.Hide();
                editForm.ShowDialog();
                this.Show();
                LoadTests();
            }
        }

        private void BtnViewResults_Click(object sender, EventArgs e)
        {
            if (gridTests.SelectedRows.Count == 0) return;
            string title = gridTests.SelectedRows[0].Cells[0].Value.ToString();
            var quiz = DataManager.Quizzes.FirstOrDefault(q => q.Title == title);
            if (quiz != null)
            {
                ResultForm resForm = new ResultForm(quiz, 0);
                this.Hide();
                resForm.ShowDialog();
                this.Show();
            }
        }

        private void BtnResetStats_Click(object sender, EventArgs e)
        {
            if (gridTests.SelectedRows.Count == 0) return;
            string title = gridTests.SelectedRows[0].Cells[0].Value.ToString();

            if (MessageBox.Show("Видалити всі результати проходжень цього тесту?", "Очищення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (var user in DataManager.Users) user.History.RemoveAll(h => h.QuizTitle == title);
                DataManager.SaveUsers();
                LoadTests();
                MessageBox.Show("Історію очищено.");
            }
        }

        private void BtnDeleteTest_Click(object sender, EventArgs e)
        {
            if (gridTests.SelectedRows.Count == 0) return;
            string title = gridTests.SelectedRows[0].Cells[0].Value.ToString();
            var quiz = DataManager.Quizzes.FirstOrDefault(q => q.Title == title);

            if (MessageBox.Show($"Видалити тест '{title}'?", "Видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                DataManager.Quizzes.Remove(quiz);
                DataManager.SaveQuizzes();
                LoadTests();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (Application.OpenForms.Count == 0) Application.Exit();
        }
    }
}