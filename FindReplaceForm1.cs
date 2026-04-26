using System;
using System.Windows.Forms;

namespace MiniWordPro
{
    public class FindReplaceForm : Form
    {
        TextBox f, r;
        RichTextBox box;

        public FindReplaceForm(RichTextBox b)
        {
            box = b;

            Text = "Find & Replace";
            Width = 320;
            Height = 200;

            f = new TextBox { Left = 20, Top = 20, Width = 260 };
            r = new TextBox { Left = 20, Top = 60, Width = 260 };

            Button find = new Button { Text = "Find", Left = 20, Top = 100 };
            Button rep = new Button { Text = "Replace All", Left = 120, Top = 100 };

            find.Click += (s, e) =>
            {
                int i = box.Text.IndexOf(f.Text);
                if (i >= 0)
                {
                    box.Select(i, f.Text.Length);
                    box.Focus();
                }
            };

            rep.Click += (s, e) =>
            {
                box.Text = box.Text.Replace(f.Text, r.Text);
            };

            Controls.AddRange(new Control[] { f, r, find, rep });
        }
    }
}