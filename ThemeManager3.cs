using System.Drawing;
using System.Windows.Forms;

namespace MiniPowerPoint
{
    public static class ThemeManager
    {
        public static bool DarkMode = false;

        public static void Apply(Form form, Panel canvas, ToolStrip toolBar)
        {
            if (DarkMode)
                Dark(form, canvas, toolBar);
            else
                Light(form, canvas, toolBar);
        }

        private static void Dark(Form form, Panel canvas, ToolStrip toolBar)
        {
            Color bg = Color.FromArgb(30, 30, 30);
            Color panel = Color.FromArgb(45, 45, 45);
            Color fg = Color.Gainsboro;

            form.BackColor = bg;
            form.ForeColor = fg;

            canvas.BackColor = panel;

            toolBar.BackColor = panel;
            toolBar.ForeColor = fg;

            foreach (ToolStripItem i in toolBar.Items)
            {
                i.BackColor = panel;
                i.ForeColor = fg;
            }
        }

        private static void Light(Form form, Panel canvas, ToolStrip toolBar)
        {
            Color bg = SystemColors.Control;
            Color panel = Color.White;
            Color fg = Color.Black;

            form.BackColor = bg;
            form.ForeColor = fg;

            canvas.BackColor = panel;

            toolBar.BackColor = bg;
            toolBar.ForeColor = fg;

            foreach (ToolStripItem i in toolBar.Items)
            {
                i.BackColor = bg;
                i.ForeColor = fg;
            }
        }
    }
}