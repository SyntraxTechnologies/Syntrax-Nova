using System.Drawing;
using System.Windows.Forms;

namespace MiniExcel
{
    public static class ThemeManager
    {
        public static bool DarkMode = false;

        public static Color Back
        {
            get { return DarkMode ? Color.FromArgb(30, 30, 30) : Color.White; }
        }

        public static Color Fore
        {
            get { return DarkMode ? Color.Gainsboro : Color.Black; }
        }

        public static Color GridBack
        {
            get { return DarkMode ? Color.FromArgb(45, 45, 45) : Color.White; }
        }

        public static Color HeaderBack
        {
            get { return DarkMode ? Color.FromArgb(60, 60, 60) : SystemColors.Control; }
        }

        public static void Apply(Form form, DataGridView grid, ToolStrip toolBar)
        {
            form.BackColor = Back;
            form.ForeColor = Fore;

            toolBar.BackColor = HeaderBack;
            toolBar.ForeColor = Fore;

            grid.BackgroundColor = GridBack;
            grid.GridColor = DarkMode ? Color.Gray : Color.LightGray;

            grid.DefaultCellStyle.BackColor = GridBack;
            grid.DefaultCellStyle.ForeColor = Fore;

            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Fore;

            grid.RowHeadersDefaultCellStyle.BackColor = HeaderBack;
            grid.RowHeadersDefaultCellStyle.ForeColor = Fore;

            grid.EnableHeadersVisualStyles = false;
        }
    }
}