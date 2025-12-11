using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class AdminForm : Form
    {
        private Quiz quizToEdit;
        private List<Question> tempQuestions = new List<Question>();

        private TextBox txtQuizTitle, txtQuizCategory, txtQuestionText, txtOption1, txtOption2, txtOption3, txtOption4;
        private RadioButton rb1, rb2, rb3, rb4;
        private Label lblCount;
        private Button btnSaveQuiz;

        public AdminForm(Quiz editQuiz = null)
        {
            this.quizToEdit = editQuiz;
            if (quizToEdit != null) tempQuestions = new List<Question>(quizToEdit.Questions);
            SetupUI();
            ThemeHelper.ApplyGradient(this);
        }
        public AdminForm() : this(null) { }

        private void SetupUI()
        {
            this.Text = "Редактор тесту";
            this.Size = new Size(650, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Велика біла картка
            Panel card = new Panel { Size = new Size(580, 720), Location = new Point(25, 20), BackColor = Color.White };
            this.Controls.Add(card);

            Label lblTitle = new Label { Text = quizToEdit == null ? "✨ Створення тесту" : "✏️ Редагування", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor, Location = new Point(20, 20), AutoSize = true };
            card.Controls.Add(lblTitle);

            // Поля
            AddLabel(card, "Назва:", 70);
            txtQuizTitle = AddText(card, 95);
            AddLabel(card, "Категорія:", 135);
            txtQuizCategory = AddText(card, 160);

            Label div = new Label { BorderStyle = BorderStyle.Fixed3D, Location = new Point(20, 205), Size = new Size(540, 2) };
            card.Controls.Add(div);

            AddLabel(card, "➕ Додати питання:", 220);
            txtQuestionText = new TextBox { Location = new Point(20, 245), Size = new Size(540, 60), Multiline = true, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.WhiteSmoke };
            card.Controls.Add(txtQuestionText);

            int y = 320; int gap = 40;
            rb1 = AddRadio(card, y); txtOption1 = AddOption(card, y);
            rb2 = AddRadio(card, y + gap); txtOption2 = AddOption(card, y + gap);
            rb3 = AddRadio(card, y + gap * 2); txtOption3 = AddOption(card, y + gap * 2);
            rb4 = AddRadio(card, y + gap * 3); txtOption4 = AddOption(card, y + gap * 3);

            Button btnAdd = new Button { Text = "Додати в список", Location = new Point(20, 500), Size = new Size(540, 40) };
            ThemeHelper.StyleButton(btnAdd, Color.LightSteelBlue); btnAdd.ForeColor = Color.Black;
            btnAdd.Click += BtnAddQuestion_Click;
            card.Controls.Add(btnAdd);

            lblCount = new Label { Text = $"Питань: {tempQuestions.Count}", Location = new Point(20, 560), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            card.Controls.Add(lblCount);

            btnSaveQuiz = new Button { Text = "💾 ЗБЕРЕГТИ ВСЕ", Location = new Point(20, 600), Size = new Size(540, 50) };
            ThemeHelper.StyleButton(btnSaveQuiz, ThemeHelper.PrimaryColor);
            btnSaveQuiz.Click += BtnSaveQuiz_Click;
            card.Controls.Add(btnSaveQuiz);

            Button btnCancel = new Button { Text = "Скасувати", Location = new Point(20, 660), Size = new Size(540, 30), FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
            btnCancel.Click += (s, e) => this.Close();
            card.Controls.Add(btnCancel);

            if (quizToEdit != null) { txtQuizTitle.Text = quizToEdit.Title; txtQuizCategory.Text = quizToEdit.Category; }
        }

        private void AddLabel(Panel p, string t, int y) { p.Controls.Add(new Label { Text = t, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray }); }
        private TextBox AddText(Panel p, int y) { var t = new TextBox { Location = new Point(20, y), Size = new Size(540, 30), Font = new Font("Segoe UI", 11), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle }; p.Controls.Add(t); return t; }
        private TextBox AddOption(Panel p, int y) { var t = new TextBox { Location = new Point(45, y - 3), Size = new Size(515, 30), Font = new Font("Segoe UI", 10) }; p.Controls.Add(t); return t; }
        private RadioButton AddRadio(Panel p, int y) { var r = new RadioButton { Location = new Point(20, y), Size = new Size(20, 20) }; p.Controls.Add(r); return r; }

        // --- ЛОГІКА (ТА САМА) ---
        private void BtnAddQuestion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuestionText.Text)) { MessageBox.Show("Введіть питання!"); return; }
            int correct = rb1.Checked ? 0 : rb2.Checked ? 1 : rb3.Checked ? 2 : rb4.Checked ? 3 : -1;
            if (correct == -1) { MessageBox.Show("Виберіть правильну відповідь!"); return; }

            tempQuestions.Add(new Question(txtQuestionText.Text, new List<string> { txtOption1.Text, txtOption2.Text, txtOption3.Text, txtOption4.Text }, correct));
            lblCount.Text = $"Питань: {tempQuestions.Count}";
            txtQuestionText.Clear(); txtOption1.Clear(); txtOption2.Clear(); txtOption3.Clear(); txtOption4.Clear();
        }

        private void BtnSaveQuiz_Click(object sender, EventArgs e)
        {
            if (tempQuestions.Count == 0) { MessageBox.Show("Додайте хоча б одне питання!"); return; }

            if (quizToEdit != null)
            {
                var original = DataManager.Quizzes.FirstOrDefault(q => q == quizToEdit);
                if (original != null) { original.Title = txtQuizTitle.Text; original.Category = txtQuizCategory.Text; original.Questions = tempQuestions; }
            }
            else
            {
                DataManager.Quizzes.Add(new Quiz { Title = txtQuizTitle.Text, Category = txtQuizCategory.Text, Questions = tempQuestions });
            }
            DataManager.SaveQuizzes();
            this.Close();
        }
    }
}