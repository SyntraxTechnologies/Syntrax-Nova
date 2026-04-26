using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniWordPro
{
    public class EditorTab : TabPage
    {
        public RichTextBox Box;
        Action<int, int> stats;

        float zoom = 1f;

        public EditorTab(Action<int, int> callback)
        {
            Text = "Document";
            stats = callback;

            Box = new RichTextBox();
            Box.Dock = DockStyle.Fill;
            Box.Font = new Font("Calibri", 12);
            Box.AcceptsTab = true;

            Box.TextChanged += (s, e) => Update();

            Controls.Add(Box);
        }

        void Update()
        {
            string t = Box.Text;
            int words = string.IsNullOrWhiteSpace(t) ? 0 : t.Split(' ').Length;
            int chars = t.Length;

            stats(words, chars);
        }

        // ===== FORMATTING =====
        public void Toggle(FontStyle style)
        {
            if (Box.SelectionFont == null) return;

            Font f = Box.SelectionFont;

            FontStyle ns = f.Style.HasFlag(style)
                ? f.Style & ~style
                : f.Style | style;

            Box.SelectionFont = new Font(f, ns);
        }

        public void Align(HorizontalAlignment a)
        {
            Box.SelectionAlignment = a;
        }

        public void ChangeColor()
        {
            ColorDialog d = new ColorDialog();
            if (d.ShowDialog() == DialogResult.OK)
                Box.SelectionColor = d.Color;
        }

        public void Highlight()
        {
            ColorDialog d = new ColorDialog();
            if (d.ShowDialog() == DialogResult.OK)
                Box.SelectionBackColor = d.Color;
        }

        public void ClearFormatting()
        {
            Box.SelectionFont = new Font("Calibri", 12);
            Box.SelectionColor = Color.Black;
            Box.SelectionBackColor = Color.White;
        }

        // ===== INSERT =====
        public void InsertImage()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Image|*.png;*.jpg;*.jpeg;*.bmp";

            if (d.ShowDialog() == DialogResult.OK)
            {
                Clipboard.SetImage(Image.FromFile(d.FileName));
                Box.Paste();
            }
        }

        public void InsertDate()
        {
            Box.SelectedText = DateTime.Now.ToString();
        }

        // ===== THEME =====
        public void ApplyTheme(Color bg, Color fg)
        {
            Box.SuspendLayout();

            Box.BackColor = bg;

            // This is the KEY fix:
            Box.SelectAll();
            Box.SelectionColor = fg;
            Box.DeselectAll();

            Box.ForeColor = fg;

            Box.ResumeLayout();
        }
    }
}