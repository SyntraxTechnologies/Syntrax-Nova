using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniOneNote
{
    public class NoteCanvas : Panel
    {
        public Page CurrentPage;
        public NoteElement Selected;

        private bool dragging;
        private int dx, dy;

        public NoteCanvas()
        {
            DoubleBuffered = true;
            BackColor = Color.White;

            MouseDown += Down;
            MouseMove += Move;
            MouseUp += Up;
            MouseDoubleClick += Edit;
        }

        private void Down(object sender, MouseEventArgs e)
        {
            if (CurrentPage == null) return;

            for (int i = CurrentPage.Elements.Count - 1; i >= 0; i--)
            {
                var el = CurrentPage.Elements[i];

                Rectangle r = new Rectangle(el.X, el.Y, el.Width, el.Height);

                if (r.Contains(e.Location))
                {
                    Selected = el;
                    dragging = true;
                    dx = e.X - el.X;
                    dy = e.Y - el.Y;
                    Invalidate();
                    return;
                }
            }

            Selected = null;
            Invalidate();
        }

        private void Move(object sender, MouseEventArgs e)
        {
            if (dragging && Selected != null)
            {
                Selected.X = e.X - dx;
                Selected.Y = e.Y - dy;
                Invalidate();
            }
        }

        private void Up(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Edit(object sender, MouseEventArgs e)
        {
            if (Selected == null || Selected.Type != NoteType.Text) return;

            Form f = new Form();
            f.Width = 400;
            f.Height = 200;

            TextBox box = new TextBox();
            box.Multiline = true;
            box.Dock = DockStyle.Fill;
            box.Text = Selected.Text;

            Button ok = new Button();
            ok.Text = "OK";
            ok.Dock = DockStyle.Bottom;

            ok.Click += (s, ev) =>
            {
                Selected.Text = box.Text;
                f.Close();
                Invalidate();
            };

            f.Controls.Add(box);
            f.Controls.Add(ok);
            f.ShowDialog();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (CurrentPage == null) return;

            foreach (var el in CurrentPage.Elements)
            {
                Rectangle r = new Rectangle(el.X, el.Y, el.Width, el.Height);

                if (el.Type == NoteType.Text)
                {
                    DrawText(e.Graphics, el, r);
                }
                else if (el.Type == NoteType.Image && el.Image != null)
                {
                    e.Graphics.DrawImage(el.Image, r);
                }

                if (el == Selected)
                    e.Graphics.DrawRectangle(Pens.Blue, r);
            }
        }

        private void DrawText(Graphics g, NoteElement el, Rectangle r)
        {
            FontStyle style = FontStyle.Regular;

            if (el.Bold) style |= FontStyle.Bold;
            if (el.Italic) style |= FontStyle.Italic;
            if (el.Underline) style |= FontStyle.Underline;

            Font font = new Font(el.FontName, el.FontSize, style);
            Brush brush = new SolidBrush(el.TextColor);

            g.DrawString(el.Text, font, brush, r);
        }
    }
}