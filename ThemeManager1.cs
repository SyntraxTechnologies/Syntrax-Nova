using System.Drawing;
using System.Windows.Forms;

namespace MiniWordPro
{
    public static class ThemeManager
    {
        static bool dark = false;

        public static void Toggle(Form f, TabControl tabs)
        {
            dark = !dark;

            Color bg = dark ? Color.FromArgb(30, 30, 30) : Color.White;
            Color fg = dark ? Color.White : Color.Black;

            f.BackColor = bg;

            foreach (TabPage t in tabs.TabPages)
            {
                var box = t.Controls[0] as RichTextBox;
                box.BackColor = bg;
                box.ForeColor = fg;
            }
        }
    }
}