using System;
using System.Drawing;
using System.Windows.Forms;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class LoginForm : Form
    {
        private Panel cardPanel; // Біла картка
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtAdminPass;
        private Button btnAdminEnter;
        private Label lblError;

        public LoginForm()
        {
            SetupUI();
            // Застосовуємо градієнт до всієї форми
            ThemeHelper.ApplyGradient(this);
        }

        private void SetupUI()
        {
            this.Text = "Вхід у систему";
            this.Size = new Size(500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 1. Створюємо "Картку" по центру
            cardPanel = new Panel();
            cardPanel.Size = new Size(360, 520);
            // Центруємо картку:
            cardPanel.Location = new Point((this.ClientSize.Width - cardPanel.Width) / 2, 60);
            cardPanel.BackColor = Color.White;
            this.Controls.Add(cardPanel);

            // 2. Заголовок (всередині картки)
            Label lblIcon = new Label();
            lblIcon.Text = "🎓"; // Іконка шапки
            lblIcon.Font = new Font("Segoe UI", 40);
            lblIcon.AutoSize = true;
            lblIcon.Location = new Point(135, 20);
            cardPanel.Controls.Add(lblIcon);

            Label lblTitle = new Label();
            lblTitle.Text = "QUIZ APP";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = ThemeHelper.PrimaryColor;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(115, 90);
            cardPanel.Controls.Add(lblTitle);

            // 3. Поля введення
            Label lblName = new Label { Text = "Ім'я:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(30, 150), AutoSize = true };
            cardPanel.Controls.Add(lblName);

            txtFirstName = new TextBox { Location = new Point(30, 175), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };
            cardPanel.Controls.Add(txtFirstName);

            Label lblSurname = new Label { Text = "Прізвище:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(30, 220), AutoSize = true };
            cardPanel.Controls.Add(lblName); // Помилка копіювання, треба додати lblSurname
            cardPanel.Controls.Add(lblSurname);

            txtLastName = new TextBox { Location = new Point(30, 245), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };
            cardPanel.Controls.Add(txtLastName);

            // 4. Кнопка Входу
            Button btnLogin = new Button();
            btnLogin.Text = "🚀 ПОЧАТИ";
            btnLogin.Location = new Point(30, 310);
            btnLogin.Size = new Size(300, 50);
            ThemeHelper.StyleButton(btnLogin, ThemeHelper.PrimaryColor); // Використовуємо наш стиль
            btnLogin.Click += BtnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            lblError = new Label { ForeColor = Color.Red, Location = new Point(30, 290), AutoSize = true, Font = new Font("Segoe UI", 9) };
            cardPanel.Controls.Add(lblError);

            // --- Адмін частина ---
            Label divider = new Label { BorderStyle = BorderStyle.Fixed3D, Location = new Point(30, 390), Size = new Size(300, 2) };
            cardPanel.Controls.Add(divider);

            Label lblAdminLink = new Label { Text = "🔒 Вхід для викладача", Font = new Font("Segoe UI", 9, FontStyle.Underline), ForeColor = Color.Gray, Cursor = Cursors.Hand, AutoSize = true, Location = new Point(110, 400) };
            lblAdminLink.Click += (s, e) => {
                bool show = !txtAdminPass.Visible;
                txtAdminPass.Visible = show;
                btnAdminEnter.Visible = show;
            };
            cardPanel.Controls.Add(lblAdminLink);

            txtAdminPass = new TextBox { PasswordChar = '●', Location = new Point(30, 430), Size = new Size(200, 30), Visible = false, Font = new Font("Segoe UI", 10) };
            cardPanel.Controls.Add(txtAdminPass);

            btnAdminEnter = new Button { Text = "Ввійти", Location = new Point(240, 429), Size = new Size(90, 32), Visible = false, FlatStyle = FlatStyle.Flat, BackColor = Color.Gray, ForeColor = Color.White };
            btnAdminEnter.Click += BtnAdminEnter_Click;
            cardPanel.Controls.Add(btnAdminEnter);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string name = txtFirstName.Text.Trim();
            string surname = txtLastName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname))
            {
                lblError.Text = "⚠️ Введіть ім'я та прізвище!";
                return;
            }

            User newUser = new User(name, surname);
            DataManager.CurrentUser = newUser;

            bool exists = false;
            foreach (var u in DataManager.Users)
            {
                if (u.FullName == newUser.FullName)
                {
                    DataManager.CurrentUser = u;
                    exists = true;
                    break;
                }
            }

            if (!exists) DataManager.Users.Add(newUser);
            DataManager.SaveUsers();

            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
        }

        private void BtnAdminEnter_Click(object sender, EventArgs e)
        {
            if (txtAdminPass.Text == "admin123")
            {
                AdminDashboard dashboard = new AdminDashboard();
                dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("⛔ Невірний пароль!");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { Application.Exit(); }
    }
}