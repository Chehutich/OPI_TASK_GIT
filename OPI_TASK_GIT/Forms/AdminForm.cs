using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class AdminForm : Form
    {
        // Якщо цей об'єкт не null, значить ми редагуємо старий тест
        private Quiz quizToEdit = null;

        private List<Question> tempQuestions = new List<Question>();

        // Елементи UI
        private TextBox txtQuizTitle;
        private TextBox txtQuizCategory;

        private TextBox txtQuestionText;
        private TextBox txtOption1;
        private TextBox txtOption2;
        private TextBox txtOption3;
        private TextBox txtOption4;
        private RadioButton rb1, rb2, rb3, rb4;

        private Label lblCount;
        private Button btnAddQuestion;
        private Button btnSaveQuiz;

        // Приймає тест або null
        public AdminForm(Quiz editQuiz = null)
        {
            this.quizToEdit = editQuiz;
            SetupUI();

            if (quizToEdit != null)
            {
                LoadDataForEdit();
            }
        }

        // Порожній конструктор для дизайнера (щоб не сварився)
        public AdminForm() : this(null) { }

        private void LoadDataForEdit()
        {
            txtQuizTitle.Text = quizToEdit.Title;
            txtQuizCategory.Text = quizToEdit.Category;
            this.Text = $"Редагування тесту: {quizToEdit.Title}";

            tempQuestions = new List<Question>(quizToEdit.Questions);

            lblCount.Text = $"Питань у тесті: {tempQuestions.Count}";
            btnSaveQuiz.Text = "ЗБЕРЕГТИ ЗМІНИ";
        }

        private void SetupUI()
        {
            this.Text = "Створення нового тесту";
            this.Size = new Size(600, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            int x = 30;

            // БЛОК 1
            Label lblHeader1 = new Label { Text = "1. Інформація про Тест", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.DarkSlateBlue, Location = new Point(x, 20), AutoSize = true };
            this.Controls.Add(lblHeader1);

            this.Controls.Add(new Label { Text = "Назва тесту:", Location = new Point(x, 50), AutoSize = true });
            txtQuizTitle = new TextBox { Location = new Point(x, 70), Size = new Size(520, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtQuizTitle);

            this.Controls.Add(new Label { Text = "Категорія:", Location = new Point(x, 100), AutoSize = true });
            txtQuizCategory = new TextBox { Location = new Point(x, 120), Size = new Size(520, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtQuizCategory);

            Label divider = new Label { BorderStyle = BorderStyle.Fixed3D, Location = new Point(x, 160), Size = new Size(520, 2) };
            this.Controls.Add(divider);

            // БЛОК 2
            Label lblHeader2 = new Label { Text = "2. Додавання нових питань", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.DarkSlateBlue, Location = new Point(x, 170), AutoSize = true };
            this.Controls.Add(lblHeader2);

            // Підказка для режиму редагування
            Label lblEditHint = new Label { Text = "(Тут ви додаєте нові питання до існуючих)", ForeColor = Color.Gray, Location = new Point(x + 250, 175), AutoSize = true };
            this.Controls.Add(lblEditHint);

            this.Controls.Add(new Label { Text = "Текст питання:", Location = new Point(x, 200), AutoSize = true });
            txtQuestionText = new TextBox { Location = new Point(x, 220), Size = new Size(520, 50), Multiline = true, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtQuestionText);

            int startY = 290;
            int gap = 40;

            rb1 = new RadioButton { Location = new Point(x, startY), Size = new Size(14, 13) };
            txtOption1 = new TextBox { Location = new Point(x + 25, startY - 3), Size = new Size(495, 25) };
            this.Controls.Add(rb1); this.Controls.Add(txtOption1);

            rb2 = new RadioButton { Location = new Point(x, startY + gap), Size = new Size(14, 13) };
            txtOption2 = new TextBox { Location = new Point(x + 25, startY + gap - 3), Size = new Size(495, 25) };
            this.Controls.Add(rb2); this.Controls.Add(txtOption2);

            rb3 = new RadioButton { Location = new Point(x, startY + gap * 2), Size = new Size(14, 13) };
            txtOption3 = new TextBox { Location = new Point(x + 25, startY + gap * 2 - 3), Size = new Size(495, 25) };
            this.Controls.Add(rb3); this.Controls.Add(txtOption3);

            rb4 = new RadioButton { Location = new Point(x, startY + gap * 3), Size = new Size(14, 13) };
            txtOption4 = new TextBox { Location = new Point(x + 25, startY + gap * 3 - 3), Size = new Size(495, 25) };
            this.Controls.Add(rb4); this.Controls.Add(txtOption4);

            Label lblHint = new Label { Text = "* Поставте крапку біля правильної відповіді", ForeColor = Color.Gray, Location = new Point(x, startY + gap * 4), AutoSize = true };
            this.Controls.Add(lblHint);

            btnAddQuestion = new Button();
            btnAddQuestion.Text = "+ Додати питання";
            btnAddQuestion.Location = new Point(x, 480);
            btnAddQuestion.Size = new Size(520, 40);
            btnAddQuestion.BackColor = Color.LightSteelBlue;
            btnAddQuestion.FlatStyle = FlatStyle.Flat;
            btnAddQuestion.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAddQuestion.Click += BtnAddQuestion_Click;
            this.Controls.Add(btnAddQuestion);

            // БЛОК 3
            lblCount = new Label();
            lblCount.Text = "Питань у тесті: 0";
            lblCount.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCount.Location = new Point(x, 540);
            lblCount.AutoSize = true;
            this.Controls.Add(lblCount);

            btnSaveQuiz = new Button();
            btnSaveQuiz.Text = "СТВОРИТИ ТЕСТ";
            btnSaveQuiz.Location = new Point(x, 580);
            btnSaveQuiz.Size = new Size(520, 60);
            btnSaveQuiz.BackColor = Color.DarkSlateBlue;
            btnSaveQuiz.ForeColor = Color.White;
            btnSaveQuiz.FlatStyle = FlatStyle.Flat;
            btnSaveQuiz.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnSaveQuiz.Cursor = Cursors.Hand;
            btnSaveQuiz.Click += BtnSaveQuiz_Click;
            this.Controls.Add(btnSaveQuiz);

            Button btnCancel = new Button { Text = "Скасувати", Location = new Point(x, 650), Size = new Size(520, 30), FlatStyle = FlatStyle.Flat, ForeColor = Color.IndianRed };
            btnCancel.Click += (s, e) => { this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void BtnAddQuestion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuestionText.Text) || string.IsNullOrWhiteSpace(txtOption1.Text) ||
                string.IsNullOrWhiteSpace(txtOption2.Text) || string.IsNullOrWhiteSpace(txtOption3.Text) || string.IsNullOrWhiteSpace(txtOption4.Text))
            {
                MessageBox.Show("Заповніть всі поля!", "Помилка");
                return;
            }

            int correctIndex = -1;
            if (rb1.Checked) correctIndex = 0;
            else if (rb2.Checked) correctIndex = 1;
            else if (rb3.Checked) correctIndex = 2;
            else if (rb4.Checked) correctIndex = 3;

            if (correctIndex == -1)
            {
                MessageBox.Show("Оберіть правильну відповідь!", "Увага");
                return;
            }

            List<string> options = new List<string> { txtOption1.Text, txtOption2.Text, txtOption3.Text, txtOption4.Text };
            Question newQ = new Question(txtQuestionText.Text, options, correctIndex);

            tempQuestions.Add(newQ);

            lblCount.Text = $"Питань у тесті: {tempQuestions.Count}";

            txtQuestionText.Clear();
            txtOption1.Clear(); txtOption2.Clear(); txtOption3.Clear(); txtOption4.Clear();
            rb1.Checked = false; rb2.Checked = false; rb3.Checked = false; rb4.Checked = false;
            txtQuestionText.Focus();
        }

        // ЛОГІКА ЗБЕРЕЖЕННЯ (Оновлена)
        private void BtnSaveQuiz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuizTitle.Text) || string.IsNullOrWhiteSpace(txtQuizCategory.Text))
            {
                MessageBox.Show("Введіть Назву та Категорію!", "Помилка");
                return;
            }

            if (tempQuestions.Count == 0)
            {
                MessageBox.Show("Тест повинен мати хоча б одне питання!", "Помилка");
                return;
            }

            // ВАРІАНТ А: РЕДАГУВАННЯ
            if (quizToEdit != null)
            {
                var original = DataManager.Quizzes.FirstOrDefault(q => q == quizToEdit);
                if (original != null)
                {
                    original.Title = txtQuizTitle.Text;
                    original.Category = txtQuizCategory.Text;
                    original.Questions = tempQuestions;
                    original.Description = $"Оновлено викладачем. Питань: {tempQuestions.Count}";

                    MessageBox.Show("Зміни успішно збережено!", "Редагування");
                }
            }
            // ВАРІАНТ Б: СТВОРЕННЯ НОВОГО
            else
            {
                Quiz newQuiz = new Quiz
                {
                    Title = txtQuizTitle.Text,
                    Category = txtQuizCategory.Text,
                    Description = $"Створено викладачем. Питань: {tempQuestions.Count}",
                    Questions = tempQuestions
                };
                DataManager.Quizzes.Add(newQuiz);
                MessageBox.Show("Тест успішно створено!", "Створення");
            }

            DataManager.SaveQuizzes();
            this.Close();
        }
    }
}