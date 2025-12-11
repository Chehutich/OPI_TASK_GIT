using System;
using System.Windows.Forms;
using QuizApp.Services;
using QuizApp.Forms;

namespace QuizApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DataManager.LoadData();

            Application.Run(new LoginForm());
        }
    }
}