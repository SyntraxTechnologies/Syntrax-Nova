using System.Drawing;
using System.Windows.Forms;

namespace MiniOneNote
{
    public static class ThemeManager
    {
        public static bool DarkMode = false;

        public static void Apply(Form form, Panel canvas, ToolStrip tool)
        {
            if (DarkMode)
            {
                form.BackColor = Color.FromArgb(30, 30, 30);
                form.ForeColor = Color.Gainsboro;

                canvas.BackColor = Color.FromArgb(45, 45, 45);

                tool.BackColor = Color.FromArgb(45, 45, 45);
                tool.ForeColor = Color.Gainsboro;

                foreach (ToolStripItem i in tool.Items)
                    i.ForeColor = Color.Gainsboro;
            }
            else
            {
                form.BackColor = SystemColors.Control;
                form.ForeColor = Color.Black;

                canvas.BackColor = Color.White;

                tool.BackColor = SystemColors.Control;
                tool.ForeColor = Color.Black;

                foreach (ToolStripItem i in tool.Items)
                    i.ForeColor = Color.Black;
            }
        }
    }
}