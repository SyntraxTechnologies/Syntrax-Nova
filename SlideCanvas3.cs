using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniPowerPoint
{
    public class SlideCanvas : Panel
    {
        public Slide CurrentSlide;
        public SlideElement Selected;

        private bool dragging = false;
        private int offsetX;
        private int offsetY;

        public SlideCanvas()
        {
            DoubleBuffered = true;
            BackColor = Color.White;

            MouseDown += Canvas_MouseDown;
            MouseMove += Canvas_MouseMove;
            MouseUp += Canvas_MouseUp;
            MouseDoubleClick += Canvas_DoubleClick;
        }


        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentSlide == null) return;

            // hit test (top-most first)
            for (int i = CurrentSlide.Elements.Count - 1; i >= 0; i--)
            {
                SlideElement el = CurrentSlide.Elements[i];

                Rectangle rect = new Rectangle(el.X, el.Y, el.Width, el.Height);

                if (rect.Contains(e.Location))
                {
                    Selected = el;

                    dragging = true;
                    offsetX = e.X - el.X;
                    offsetY = e.Y - el.Y;

                    Invalidate();
                    return;
                }
            }

            Selected = null;
            Invalidate();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging && Selected != null)
            {
                Selected.X = e.X - offsetX;
                Selected.Y = e.Y - offsetY;

                Invalidate();
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Canvas_DoubleClick(object sender, MouseEventArgs e)
        {
            if (Selected == null) return;

            if (Selected.Type == ElementType.Text)
            {
                string result = ShowTextEditor(Selected.Text);

                if (!string.IsNullOrEmpty(result))
                {
                    Selected.Text = result;
                    Invalidate();
                }
            }
        }


        private string ShowTextEditor(string text)
        {
            Form f = new Form();
            f.Text = "Edit Text";
            f.Width = 400;
            f.Height = 200;
            f.StartPosition = FormStartPosition.CenterScreen;

            TextBox box = new TextBox();
            box.Multiline = true;
            box.Dock = DockStyle.Fill;
            box.Text = text;

            Button ok = new Button();
            ok.Text = "OK";
            ok.Dock = DockStyle.Bottom;

            string result = text;

            ok.Click += (s, e) =>
            {
                result = box.Text;
                f.Close();
            };

            f.Controls.Add(box);
            f.Controls.Add(ok);

            f.ShowDialog();

            return result;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (CurrentSlide == null) return;

            foreach (SlideElement el in CurrentSlide.Elements)
            {
                Rectangle r = new Rectangle(el.X, el.Y, el.Width, el.Height);

                switch (el.Type)
                {
                    case ElementType.Text:
                        DrawText(e.Graphics, el, r);
                        break;

                    case ElementType.Rectangle:
                        DrawRectangle(e.Graphics, el, r);
                        break;

                    case ElementType.Ellipse:
                        DrawEllipse(e.Graphics, el, r);
                        break;

                    case ElementType.Image:
                        DrawImage(e.Graphics, el, r);
                        break;
                }

                // selection border
                if (el == Selected)
                {
                    e.Graphics.DrawRectangle(Pens.Blue, r);
                }
            }
        }


        private void DrawText(Graphics g, SlideElement el, Rectangle r)
        {
            Font font = new Font(el.FontName, el.FontSize, el.Style);
            Brush brush = new SolidBrush(el.TextColor);

            g.DrawString(el.Text, font, brush, r);
        }

        private void DrawRectangle(Graphics g, SlideElement el, Rectangle r)
        {
            using (Brush b = new SolidBrush(el.FillColor))
                g.FillRectangle(b, r);

            g.DrawRectangle(new Pen(el.BorderColor), r);
        }

        private void DrawEllipse(Graphics g, SlideElement el, Rectangle r)
        {
            using (Brush b = new SolidBrush(el.FillColor))
                g.FillEllipse(b, r);

            g.DrawEllipse(new Pen(el.BorderColor), r);
        }

        private void DrawImage(Graphics g, SlideElement el, Rectangle r)
        {
            if (el.Image != null)
            {
                g.DrawImage(el.Image, r);
            }
        }
    }
}