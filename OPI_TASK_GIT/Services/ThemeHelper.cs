using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuizApp.Services
{
    public static class ThemeHelper
    {
        public static Color PrimaryColor = Color.FromArgb(50, 20, 100);    // Темний
        public static Color SecondaryColor = Color.FromArgb(30, 30, 70);   // Ще темніший
        public static Color AccentColor = Color.FromArgb(255, 80, 80);     // Червоний

        public static void ApplyGradient(Form form)
        {
            form.Resize += (s, e) => form.Invalidate();
            form.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    form.ClientRectangle, PrimaryColor, SecondaryColor, 45F))
                {
                    e.Graphics.FillRectangle(brush, form.ClientRectangle);
                }
            };
        }

        public static void StyleButton(Button btn, Color backColor)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(backColor);
            btn.MouseLeave += (s, e) => btn.BackColor = backColor;
        }

        // НОВИЙ МЕТОД: Стилізація таблиць
        public static void StyleGridView(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Стиль заголовків
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;

            // Стиль рядків
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 200, 255); // Ніжний фіолетовий при виборі
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            grid.RowHeadersVisible = false;
        }
    }
}