using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniWordPro
{
    public class MainForm : Form
    {
        TabControl tabs;
        StatusStrip status;
        ToolStripStatusLabel wordLbl;
        ToolStripStatusLabel charLbl;
        ToolStripStatusLabel zoomLbl;

        ToolStrip ribbon;
        bool dark = false;

        public MainForm()
        {
            Text = "Syntrax NovaWrite (Beta 1.02)";
            Width = 1300;
            Height = 850;

            BuildUI();
            NewDoc();
        }

        void BuildUI()
        {
            // ===== RIBBON =====
            ribbon = new ToolStrip();
            ribbon.GripStyle = ToolStripGripStyle.Hidden;
            ribbon.Dock = DockStyle.Top;
            Controls.Add(ribbon);

            Group("File",
                Btn("New", (s, e) => NewDoc()),
                Btn("Open", (s, e) => FileService.Open(Current().Box)),
                Btn("Save", (s, e) => FileService.Save(Current().Box))
            );

            Group("Format",
                Btn("B", (s, e) => Current().Toggle(FontStyle.Bold)),
                Btn("I", (s, e) => Current().Toggle(FontStyle.Italic)),
                Btn("U", (s, e) => Current().Toggle(FontStyle.Underline)),
                Btn("Color", (s, e) => Current().ChangeColor()),
                Btn("Highlight", (s, e) => Current().Highlight())
            );

            Group("Align",
                Btn("Left", (s, e) => Current().Align(HorizontalAlignment.Left)),
                Btn("Center", (s, e) => Current().Align(HorizontalAlignment.Center)),
                Btn("Right", (s, e) => Current().Align(HorizontalAlignment.Right))
            );

            Group("Insert",
                Btn("Image", (s, e) => Current().InsertImage()),
                Btn("Date", (s, e) => Current().InsertDate())
            );

            Group("Tools",
                Btn("Find", (s, e) => new FindReplaceForm(Current().Box).ShowDialog()),
                Btn("Dark Mode", (s, e) => ToggleTheme()),
                Btn("Clear", (s, e) => Current().ClearFormatting())
            );

            // ===== TABS =====
            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            Controls.Add(tabs);

            // ===== STATUS =====
            status = new StatusStrip();
            wordLbl = new ToolStripStatusLabel("Words: 0");
            charLbl = new ToolStripStatusLabel("Chars: 0");
            zoomLbl = new ToolStripStatusLabel("Zoom: 100%");

            status.Items.Add(wordLbl);
            status.Items.Add(charLbl);
            status.Items.Add(zoomLbl);

            Controls.Add(status);
        }

        ToolStripButton Btn(string text, EventHandler click)
        {
            var b = new ToolStripButton(text);
            b.Click += click;
            return b;
        }

        void Group(string title, params ToolStripButton[] buttons)
        {
            var lbl = new ToolStripLabel("[" + title + "]");
            ribbon.Items.Add(lbl);

            foreach (var b in buttons)
                ribbon.Items.Add(b);

            ribbon.Items.Add(new ToolStripSeparator());
        }

        void NewDoc()
        {
            var tab = new EditorTab(UpdateStats);
            tabs.TabPages.Add(tab);
            tabs.SelectedTab = tab;
        }

        EditorTab Current()
        {
            return tabs.SelectedTab as EditorTab;
        }

        void UpdateStats(int words, int chars)
        {
            wordLbl.Text = "Words: " + words;
            charLbl.Text = "Chars: " + chars;
        }

        void ToggleTheme()
        {
            dark = !dark;

            Color bg = dark ? Color.FromArgb(30, 30, 30) : Color.White;
            Color fg = dark ? Color.Gainsboro : Color.Black;

            this.BackColor = bg;
            this.ForeColor = fg;

            ribbon.BackColor = bg;
            ribbon.ForeColor = fg;

            status.BackColor = bg;
            status.ForeColor = fg;

            for (int i = 0; i < tabs.TabPages.Count; i++)
            {
                EditorTab tab = tabs.TabPages[i] as EditorTab;
                if (tab != null)
                {
                    tab.ApplyTheme(bg, fg);
                }
            }
        }
    }
}