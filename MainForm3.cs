using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniPowerPoint
{
    public class MainForm : Form
    {
        private SlideManager manager = new SlideManager();
        private SlideCanvas canvas;
        private SlideThumbnailPanel thumbnails;
        private ToolStrip toolBar;

        public MainForm()
        {
            Text = "Syntrax NovaSlide (Beta 1.02)";
            Width = 1200;
            Height = 800;

            InitUI();

            manager.AddSlide();
            RefreshView();
        }

        private void InitUI()
        {
            thumbnails = new SlideThumbnailPanel();
            thumbnails.Dock = DockStyle.Left;
            thumbnails.Width = 180;
            thumbnails.Manager = manager;

            Controls.Add(thumbnails);

            canvas = new SlideCanvas();
            canvas.Dock = DockStyle.Fill;

            Controls.Add(canvas);

            toolBar = new ToolStrip();

            ToolStripButton addSlide = new ToolStripButton("Add Slide");
            addSlide.Click += (s, e) =>
            {
                manager.AddSlide();
                RefreshView();
            };

            ToolStripButton delSlide = new ToolStripButton("Delete Slide");
            delSlide.Click += (s, e) =>
            {
                manager.RemoveCurrent();
                RefreshView();
            };

            ToolStripButton addText = new ToolStripButton("Add Text");
            addText.Click += (s, e) =>
            {
                var sld = manager.Current;
                if (sld != null)
                {
                    sld.Elements.Add(new SlideElement());
                    canvas.Invalidate();
                }
            };

            ToolStripButton dark = new ToolStripButton("Dark Mode");
            dark.Click += (s, e) =>
            {
                ThemeManager.DarkMode = !ThemeManager.DarkMode;
                ThemeManager.Apply(this, canvas, toolBar);
            };

            ToolStripButton play = new ToolStripButton("Start Slideshow");
            play.Click += (s, e) => StartSlideshow();

            toolBar.Items.Add(addSlide);
            toolBar.Items.Add(delSlide);
            toolBar.Items.Add(addText);
            toolBar.Items.Add(dark);
            toolBar.Items.Add(play);

            Controls.Add(toolBar);
        }

        private void RefreshView()
        {
            thumbnails.Items.Clear();

            for (int i = 0; i < manager.Slides.Count; i++)
                thumbnails.Items.Add("Slide " + (i + 1));

            thumbnails.SelectedIndex = manager.CurrentIndex;

            canvas.CurrentSlide = manager.Current;
            canvas.Invalidate();
        }

        private void StartSlideshow()
        {
            Form fs = new Form();
            fs.WindowState = FormWindowState.Maximized;
            fs.BackColor = Color.Black;

            Panel view = new Panel();
            view.Dock = DockStyle.Fill;

            fs.Controls.Add(view);

            int index = 0;

            Label lbl = new Label();
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Arial", 28);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;

            view.Controls.Add(lbl);

            Timer t = new Timer();
            t.Interval = 2000;

            t.Tick += (s, e) =>
            {
                if (index >= manager.Slides.Count)
                {
                    t.Stop();
                    fs.Close();
                    return;
                }

                var slide = manager.Slides[index];

                string text = "";

                foreach (var el in slide.Elements)
                    text += el.Text + "\n";

                lbl.Text = text;

                index++;
            };

            t.Start();
            fs.ShowDialog();
        }
    }
}