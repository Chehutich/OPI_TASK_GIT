using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq; // Потрібно для копіювання списків
using System.Threading.Tasks;
using System.Windows.Forms;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Forms
{
    public partial class QuizForm : Form
    {
        // --- Змінні стану ---
        private Quiz currentQuiz;
        private List<Question> shuffledQuestions; // Список перемішаних питань
        private int currentQuestionIndex = 0;
        private int currentScore = 0;

        // --- Змінні для Таймера ---
        private Timer questionTimer;
        // -----------------------------------------------------
        // ТУТ ЗМІНИЛИ ЧАС:
        private int timePerQuestion = 30; // Секунд на питання
        // -----------------------------------------------------
        private int timeLeft;

        // --- Елементи інтерфейсу ---
        private Label lblProgress;
        private Label lblQuestionText;
        private Button[] optionButtons;
        private ProgressBar timeProgressBar; // Смужка часу

        public QuizForm(Quiz quiz)
        {
            // 1. Зберігаємо тест
            this.currentQuiz = quiz;

            // 2. Перемішуємо питання (створюємо випадковий список)
            this.shuffledQuestions = ShuffleList(quiz.Questions);

            SetupUI();

            // 3. Налаштовуємо таймер
            questionTimer = new Timer();
            questionTimer.Interval = 1000; // Тікає кожну 1 секунду
            questionTimer.Tick += QuestionTimer_Tick;

            ShowQuestion();
        }

        // Порожній конструктор
        public QuizForm() { SetupUI(); }

        private void SetupUI()
        {
            // Налаштування вікна
            this.Text = currentQuiz != null ? currentQuiz.Title : "Тестування";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 1. Прогрес бар (Таймер) - ЗВЕРХУ
            timeProgressBar = new ProgressBar();
            timeProgressBar.Location = new Point(0, 0);
            timeProgressBar.Size = new Size(800, 10); // Тонка смужка на всю ширину
            timeProgressBar.Style = ProgressBarStyle.Continuous;
            timeProgressBar.ForeColor = Color.Orange;
            this.Controls.Add(timeProgressBar);

            // 2. Текстовий прогрес "Питання 1/5"
            lblProgress = new Label();
            lblProgress.Text = "Питання 1 / 5";
            lblProgress.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblProgress.ForeColor = Color.Gray;
            lblProgress.Location = new Point(20, 25);
            lblProgress.AutoSize = true;
            this.Controls.Add(lblProgress);

            // 3. Текст питання
            lblQuestionText = new Label();
            lblQuestionText.Text = "Тут буде питання...";
            lblQuestionText.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblQuestionText.ForeColor = Color.Black;
            lblQuestionText.TextAlign = ContentAlignment.MiddleCenter;
            lblQuestionText.AutoSize = false;
            lblQuestionText.Size = new Size(760, 150);
            lblQuestionText.Location = new Point(20, 60);
            this.Controls.Add(lblQuestionText);

            // 4. Кнопки (2x2)
            optionButtons = new Button[4];
            int startY = 260;
            int btnWidth = 360;
            int btnHeight = 120;
            int gap = 20;

            for (int i = 0; i < 4; i++)
            {
                optionButtons[i] = new Button();
                optionButtons[i].Font = new Font("Segoe UI", 12);
                optionButtons[i].FlatStyle = FlatStyle.Flat;
                optionButtons[i].BackColor = Color.WhiteSmoke;
                optionButtons[i].Cursor = Cursors.Hand;
                optionButtons[i].Tag = i;

                int x = (i % 2 == 0) ? 20 : 20 + btnWidth + gap;
                int y = (i < 2) ? startY : startY + btnHeight + gap;

                optionButtons[i].Location = new Point(x, y);
                optionButtons[i].Size = new Size(btnWidth, btnHeight);
                optionButtons[i].Click += OptionButton_Click;

                this.Controls.Add(optionButtons[i]);
            }
        }

        private void ShowQuestion()
        {
            if (currentQuestionIndex >= shuffledQuestions.Count) return;

            var q = shuffledQuestions[currentQuestionIndex];

            // Оновлюємо UI
            lblProgress.Text = $"Питання {currentQuestionIndex + 1} / {shuffledQuestions.Count}";
            lblQuestionText.Text = q.Text;

            for (int i = 0; i < 4; i++)
            {
                optionButtons[i].Text = q.Options[i];
                optionButtons[i].BackColor = Color.LightSkyBlue;
                optionButtons[i].Enabled = true;
                optionButtons[i].ForeColor = Color.Black;
            }

            // ЗАПУСК ТАЙМЕРА
            timeLeft = timePerQuestion;
            timeProgressBar.Maximum = timePerQuestion;
            timeProgressBar.Value = timeLeft;
            questionTimer.Start();
        }

        // Логіка таймера: що відбувається кожну секунду
        private async void QuestionTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            timeProgressBar.Value = timeLeft;

            if (timeLeft <= 0)
            {
                // ЧАС ВИЙШОВ!
                questionTimer.Stop();

                // Показуємо правильну відповідь, але бали не даємо
                var question = shuffledQuestions[currentQuestionIndex];

                // Блокуємо кнопки
                foreach (var btn in optionButtons) btn.Enabled = false;

                // Підсвічуємо правильну (щоб знали на майбутнє)
                optionButtons[question.CorrectOptionIndex].BackColor = Color.LightGreen;

                MessageBox.Show("Час вийшов!", "Упс", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Чекаємо трохи і йдемо далі
                await Task.Delay(1500);
                NextQuestion();
            }
        }

        private async void OptionButton_Click(object sender, EventArgs e)
        {
            // 1. ОДРАЗУ ЗУПИНЯЄМО ТАЙМЕР
            questionTimer.Stop();

            Button clickedButton = (Button)sender;
            int selectedIndex = (int)clickedButton.Tag;

            var question = shuffledQuestions[currentQuestionIndex];

            foreach (var btn in optionButtons) btn.Enabled = false;

            // 2. Перевірка
            if (selectedIndex == question.CorrectOptionIndex)
            {
                clickedButton.BackColor = Color.LightGreen;
                currentScore += question.Points;
            }
            else
            {
                clickedButton.BackColor = Color.IndianRed;
                clickedButton.ForeColor = Color.White;
                optionButtons[question.CorrectOptionIndex].BackColor = Color.LightGreen;
            }

            await Task.Delay(1500);
            NextQuestion();
        }

        private void NextQuestion()
        {
            currentQuestionIndex++;

            if (currentQuestionIndex < shuffledQuestions.Count)
            {
                ShowQuestion();
            }
            else
            {
                FinishQuiz();
            }
        }

        private void FinishQuiz()
        {
            questionTimer.Stop(); // На всяк випадок

            QuizResult result = new QuizResult
            {
                QuizTitle = currentQuiz.Title,
                Score = currentScore,
                DateTaken = DateTime.Now
            };

            if (DataManager.CurrentUser != null)
            {
                DataManager.CurrentUser.History.Add(result);
                DataManager.SaveUsers();
            }

            // Відкриваємо результати
            ResultForm resultForm = new ResultForm(currentQuiz, currentScore);
            this.Hide();
            resultForm.ShowDialog();
            this.Close();
        }

        // --- МЕТОД ДЛЯ ПЕРЕМІШУВАННЯ СПИСКУ (Fisher-Yates Shuffle) ---
        private List<T> ShuffleList<T>(List<T> inputList)
        {
            // Створюємо копію списку, щоб не псувати оригінал
            List<T> randomList = new List<T>(inputList);

            Random r = new Random();
            int n = randomList.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = randomList[k];
                randomList[k] = randomList[n];
                randomList[n] = value;
            }
            return randomList;
        }
    }
}