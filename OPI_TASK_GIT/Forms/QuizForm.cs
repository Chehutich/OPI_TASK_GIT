using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class QuizForm : Form
    {
        private Quiz currentQuiz;
        private List<Question> shuffledQuestions;
        private int currentQuestionIndex = 0;
        private int currentScore = 0;

        private Timer questionTimer;
        private int timePerQuestion = 30;
        private int timeLeft;

        private Label lblProgress;
        private Label lblQuestionText;
        private Button[] optionButtons;
        private Panel questionCard; // Біла картка для питання
        private Panel progressBack; // Фон для прогрес бару
        private Panel progressFront; // Сам прогрес бар

        public QuizForm(Quiz quiz)
        {
            this.currentQuiz = quiz;
            this.shuffledQuestions = ShuffleList(quiz.Questions);
            SetupUI();
            ThemeHelper.ApplyGradient(this); // Градієнт

            questionTimer = new Timer { Interval = 1000 };
            questionTimer.Tick += QuestionTimer_Tick;
            ShowQuestion();
        }
        public QuizForm() { SetupUI(); }

        private void SetupUI()
        {
            this.Text = currentQuiz != null ? currentQuiz.Title : "Тестування";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Кастомний прогрес бар (Смужка)
            progressBack = new Panel { Location = new Point(0, 0), Size = new Size(900, 10), BackColor = Color.FromArgb(100, 255, 255, 255) };
            this.Controls.Add(progressBack);

            progressFront = new Panel { Location = new Point(0, 0), Size = new Size(900, 10), BackColor = Color.Orange };
            progressBack.Controls.Add(progressFront); // Вкладаємо всередину

            lblProgress = new Label
            {
                Text = "Питання 1 / 5",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 30),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblProgress);

            // Біла картка для питання
            questionCard = new Panel { Location = new Point(30, 70), Size = new Size(825, 180), BackColor = Color.White };
            this.Controls.Add(questionCard);

            lblQuestionText = new Label
            {
                Text = "Завантаження...",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ThemeHelper.PrimaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Dock = DockStyle.Fill // На всю картку
            };
            questionCard.Controls.Add(lblQuestionText);

            // Кнопки
            optionButtons = new Button[4];
            int startY = 280;
            int btnWidth = 400;
            int btnHeight = 100;
            int gap = 25;

            for (int i = 0; i < 4; i++)
            {
                optionButtons[i] = new Button();
                int x = (i % 2 == 0) ? 30 : 30 + btnWidth + gap;
                int y = (i < 2) ? startY : startY + btnHeight + gap;

                optionButtons[i].Location = new Point(x, y);
                optionButtons[i].Size = new Size(btnWidth, btnHeight);
                optionButtons[i].Tag = i;

                // Використовуємо наш стиль, але з білим кольором
                ThemeHelper.StyleButton(optionButtons[i], Color.White);
                optionButtons[i].ForeColor = ThemeHelper.PrimaryColor; // Текст фіолетовий
                optionButtons[i].Click += OptionButton_Click;

                this.Controls.Add(optionButtons[i]);
            }
        }

        private void ShowQuestion()
        {
            if (currentQuestionIndex >= shuffledQuestions.Count) return;
            var q = shuffledQuestions[currentQuestionIndex];

            lblProgress.Text = $"⏳ Питання {currentQuestionIndex + 1} з {shuffledQuestions.Count}";
            lblQuestionText.Text = q.Text;

            for (int i = 0; i < 4; i++)
            {
                optionButtons[i].Text = q.Options[i];
                optionButtons[i].BackColor = Color.White;
                optionButtons[i].ForeColor = ThemeHelper.PrimaryColor;
                optionButtons[i].Enabled = true;
            }

            timeLeft = timePerQuestion;
            UpdateProgress();
            questionTimer.Start();
        }

        private void UpdateProgress()
        {
            // Рахуємо ширину смужки
            float percent = (float)timeLeft / timePerQuestion;
            progressFront.Width = (int)(this.Width * percent);

            // Змінюємо колір: Зелений -> Жовтий -> Червоний
            if (percent > 0.6) progressFront.BackColor = Color.LimeGreen;
            else if (percent > 0.3) progressFront.BackColor = Color.Gold;
            else progressFront.BackColor = Color.Red;
        }

        private async void QuestionTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            UpdateProgress();

            if (timeLeft <= 0)
            {
                questionTimer.Stop();
                var question = shuffledQuestions[currentQuestionIndex];
                foreach (var btn in optionButtons) btn.Enabled = false;
                optionButtons[question.CorrectOptionIndex].BackColor = Color.LightGreen;
                optionButtons[question.CorrectOptionIndex].ForeColor = Color.White;

                MessageBox.Show("⏰ Час вийшов!");
                await Task.Delay(1000);
                NextQuestion();
            }
        }

        private async void OptionButton_Click(object sender, EventArgs e)
        {
            questionTimer.Stop();
            Button clickedButton = (Button)sender;
            int selectedIndex = (int)clickedButton.Tag;
            var question = shuffledQuestions[currentQuestionIndex];

            foreach (var btn in optionButtons) btn.Enabled = false;

            if (selectedIndex == question.CorrectOptionIndex)
            {
                clickedButton.BackColor = Color.LimeGreen;
                clickedButton.ForeColor = Color.White;
                currentScore += question.Points;
            }
            else
            {
                clickedButton.BackColor = Color.Tomato;
                clickedButton.ForeColor = Color.White;
                optionButtons[question.CorrectOptionIndex].BackColor = Color.LimeGreen;
                optionButtons[question.CorrectOptionIndex].ForeColor = Color.White;
            }

            await Task.Delay(1500);
            NextQuestion();
        }

        private void NextQuestion()
        {
            currentQuestionIndex++;
            if (currentQuestionIndex < shuffledQuestions.Count) ShowQuestion();
            else FinishQuiz();
        }

        private void FinishQuiz()
        {
            questionTimer.Stop();
            QuizResult result = new QuizResult { QuizTitle = currentQuiz.Title, Score = currentScore, DateTaken = DateTime.Now };
            if (DataManager.CurrentUser != null) { DataManager.CurrentUser.History.Add(result); DataManager.SaveUsers(); }

            ResultForm resultForm = new ResultForm(currentQuiz, currentScore);
            this.Hide();
            resultForm.ShowDialog();
            this.Close();
        }

        private List<T> ShuffleList<T>(List<T> inputList)
        {
            List<T> randomList = new List<T>(inputList);
            Random r = new Random();
            int n = randomList.Count;
            while (n > 1) { n--; int k = r.Next(n + 1); T value = randomList[k]; randomList[k] = randomList[n]; randomList[n] = value; }
            return randomList;
        }

        private void QuizForm_Load(object sender, EventArgs e)
        {

        }
    }
}