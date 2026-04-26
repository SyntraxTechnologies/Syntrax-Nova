using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniOneNote
{
    public class MainForm : Form
    {
        private Notebook notebook = new Notebook();

        private ListBox sections;
        private ListBox pages;
        private NoteCanvas canvas;
        private ToolStrip tool;

        private Section currentSection;
        private Page currentPage;

        private ComboBox fontBox;
        private ComboBox sizeBox;

        public MainForm()
        {
            Text = "Syntrax NovaBook (Beta 1.02)";
            Width = 1200;
            Height = 800;

            InitUI();
            CreateDemo();
        }

        private void InitUI()
        {
            sections = new ListBox();
            sections.Dock = DockStyle.Left;
            sections.Width = 150;
            sections.SelectedIndexChanged += SectionChanged;
            Controls.Add(sections);

            pages = new ListBox();
            pages.Dock = DockStyle.Left;
            pages.Width = 150;
            pages.SelectedIndexChanged += PageChanged;
            Controls.Add(pages);

            canvas = new NoteCanvas();
            canvas.Dock = DockStyle.Fill;
            Controls.Add(canvas);

            tool = new ToolStrip();

            // TEXT CONTROLS
            fontBox = new ComboBox();
            foreach (FontFamily f in FontFamily.Families)
                fontBox.Items.Add(f.Name);

            fontBox.SelectedIndexChanged += ApplyFormat;

            sizeBox = new ComboBox();
            sizeBox.Items.AddRange(new object[] { "10", "12", "14", "16", "18", "24", "32" });
            sizeBox.SelectedIndexChanged += ApplyFormat;

            ToolStripButton bold = new ToolStripButton("B");
            bold.Click += (s, e) => ToggleStyle("bold");

            ToolStripButton italic = new ToolStripButton("I");
            italic.Click += (s, e) => ToggleStyle("italic");

            ToolStripButton underline = new ToolStripButton("U");
            underline.Click += (s, e) => ToggleStyle("underline");

            ToolStripButton color = new ToolStripButton("Color");
            color.Click += PickColor;

            ToolStripButton addText = new ToolStripButton("Text");
            addText.Click += (s, e) =>
            {
                currentPage.Elements.Add(new NoteElement());
                canvas.Invalidate();
            };

            ToolStripButton save = new ToolStripButton("Save");
            save.Click += SaveFile;

            ToolStripButton load = new ToolStripButton("Load");
            load.Click += LoadFile;

            ToolStripButton dark = new ToolStripButton("Dark");
            dark.Click += (s, e) =>
            {
                ThemeManager.DarkMode = !ThemeManager.DarkMode;
                ThemeManager.Apply(this, canvas, tool);
            };

            tool.Items.Add(new ToolStripControlHost(fontBox));
            tool.Items.Add(new ToolStripControlHost(sizeBox));
            tool.Items.Add(bold);
            tool.Items.Add(italic);
            tool.Items.Add(underline);
            tool.Items.Add(color);
            tool.Items.Add(addText);
            tool.Items.Add(save);
            tool.Items.Add(load);
            tool.Items.Add(dark);

            Controls.Add(tool);
        }

        private void CreateDemo()
        {
            var sec = new Section();
            var page = new Page();

            sec.Pages.Add(page);
            notebook.Sections.Add(sec);

            RefreshUI();
        }

        private void SectionChanged(object sender, EventArgs e)
        {
            currentSection = notebook.Sections[sections.SelectedIndex];
            RefreshPages();
        }

        private void PageChanged(object sender, EventArgs e)
        {
            currentPage = currentSection.Pages[pages.SelectedIndex];
            canvas.CurrentPage = currentPage;
            canvas.Invalidate();
        }

        private void RefreshUI()
        {
            sections.Items.Clear();
            foreach (var s in notebook.Sections)
                sections.Items.Add(s.Name);

            sections.SelectedIndex = 0;
        }

        private void RefreshPages()
        {
            pages.Items.Clear();
            foreach (var p in currentSection.Pages)
                pages.Items.Add(p.Title);

            pages.SelectedIndex = 0;
        }

        private void ApplyFormat(object sender, EventArgs e)
        {
            if (canvas.Selected == null) return;

            if (fontBox.SelectedItem != null)
                canvas.Selected.FontName = fontBox.SelectedItem.ToString();

            float size;
            if (sizeBox.SelectedItem != null &&
                float.TryParse(sizeBox.SelectedItem.ToString(), out size))
            {
                canvas.Selected.FontSize = size;
            }

            canvas.Invalidate();
        }

        private void ToggleStyle(string type)
        {
            if (canvas.Selected == null) return;

            if (type == "bold") canvas.Selected.Bold = !canvas.Selected.Bold;
            if (type == "italic") canvas.Selected.Italic = !canvas.Selected.Italic;
            if (type == "underline") canvas.Selected.Underline = !canvas.Selected.Underline;

            canvas.Invalidate();
        }

        private void PickColor(object sender, EventArgs e)
        {
            if (canvas.Selected == null) return;

            ColorDialog d = new ColorDialog();
            if (d.ShowDialog() == DialogResult.OK)
            {
                canvas.Selected.TextColor = d.Color;
                canvas.Invalidate();
            }
        }

        private void SaveFile(object sender, EventArgs e)
        {
            if (currentPage == null) return;

            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "OneNote File|*.onenote";

            if (d.ShowDialog() == DialogResult.OK)
                FileManager.Save(currentPage, d.FileName);
        }

        private void LoadFile(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "OneNote File|*.onenote";

            if (d.ShowDialog() == DialogResult.OK)
            {
                currentPage = FileManager.Load(d.FileName);
                canvas.CurrentPage = currentPage;
                canvas.Invalidate();
            }
        }
    }
}