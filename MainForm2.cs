using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniExcel
{
    public class MainForm : Form
    {
        private DataGridView grid;
        private ToolStrip toolBar;

        private ToolStripComboBox fontBox;
        private ToolStripComboBox sizeBox;

        private ToolStripButton darkModeBtn;

        private CellStorage storage = new CellStorage();

        public MainForm()
        {
            Text = "Syntrax NovaSheet (Beta 1.02)";
            Width = 1100;
            Height = 750;

            InitGrid();
            InitToolbar();

            ThemeManager.Apply(this, grid, toolBar);
        }

        private void InitGrid()
        {
            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;

            grid.AllowUserToAddRows = false;
            grid.RowHeadersWidth = 60;
            grid.EditMode = DataGridViewEditMode.EditOnEnter;

            grid.CellEndEdit += new DataGridViewCellEventHandler(OnCellEdit);
            grid.CellFormatting += new DataGridViewCellFormattingEventHandler(OnCellFormat);

            // Columns A-Z
            for (int i = 0; i < 26; i++)
            {
                char col = (char)('A' + i);
                grid.Columns.Add(col.ToString(), col.ToString());
                grid.Columns[i].Width = 90;
            }

            // Rows
            for (int r = 0; r < 50; r++)
            {
                grid.Rows.Add();
                grid.Rows[r].HeaderCell.Value = (r + 1).ToString();
            }

            Controls.Add(grid);
        }

        private void InitToolbar()
        {
            toolBar = new ToolStrip();

            fontBox = new ToolStripComboBox();
            foreach (FontFamily f in FontFamily.Families)
                fontBox.Items.Add(f.Name);

            fontBox.Width = 160;
            fontBox.SelectedIndexChanged += new EventHandler(ApplyFormatting);

            sizeBox = new ToolStripComboBox();
            sizeBox.Items.Add("8");
            sizeBox.Items.Add("10");
            sizeBox.Items.Add("11");
            sizeBox.Items.Add("12");
            sizeBox.Items.Add("14");
            sizeBox.Items.Add("16");
            sizeBox.Items.Add("18");
            sizeBox.Items.Add("20");
            sizeBox.Items.Add("24");

            sizeBox.Width = 60;
            sizeBox.SelectedIndexChanged += new EventHandler(ApplyFormatting);

            ToolStripButton boldBtn = new ToolStripButton("B");
            boldBtn.Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
            boldBtn.Click += new EventHandler(ToggleBold);

            ToolStripButton italicBtn = new ToolStripButton("I");
            italicBtn.Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Italic);
            italicBtn.Click += new EventHandler(ToggleItalic);

            ToolStripButton underlineBtn = new ToolStripButton("U");
            underlineBtn.Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Underline);
            underlineBtn.Click += new EventHandler(ToggleUnderline);

            ToolStripButton fontColorBtn = new ToolStripButton("Font Color");
            fontColorBtn.Click += new EventHandler(SetFontColor);

            ToolStripButton backColorBtn = new ToolStripButton("Cell Color");
            backColorBtn.Click += new EventHandler(SetBackColor);

            darkModeBtn = new ToolStripButton("Dark Mode");
            darkModeBtn.Click += new EventHandler(ToggleDarkMode);

            toolBar.Items.Add(new ToolStripLabel("Font:"));
            toolBar.Items.Add(fontBox);

            toolBar.Items.Add(new ToolStripLabel("Size:"));
            toolBar.Items.Add(sizeBox);

            toolBar.Items.Add(boldBtn);
            toolBar.Items.Add(italicBtn);
            toolBar.Items.Add(underlineBtn);

            toolBar.Items.Add(fontColorBtn);
            toolBar.Items.Add(backColorBtn);

            toolBar.Items.Add(new ToolStripSeparator());
            toolBar.Items.Add(darkModeBtn);

            Controls.Add(toolBar);
        }

        private void OnCellEdit(object sender, DataGridViewCellEventArgs e)
        {
            CellModel model = storage.Get(e.RowIndex, e.ColumnIndex);
            model.Text = Convert.ToString(grid[e.ColumnIndex, e.RowIndex].Value);
        }

        private void OnCellFormat(object sender, DataGridViewCellFormattingEventArgs e)
        {
            CellModel model = storage.Get(e.RowIndex, e.ColumnIndex);

            e.CellStyle.Font = new Font(
                model.FontName,
                model.FontSize,
                model.Style
            );

            e.CellStyle.ForeColor = model.ForeColor;
            e.CellStyle.BackColor = model.BackColor;
        }

        private void ApplyFormatting(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                CellModel model = storage.Get(cell.RowIndex, cell.ColumnIndex);

                if (fontBox.SelectedItem != null)
                    model.FontName = fontBox.SelectedItem.ToString();

                float size;
                if (sizeBox.SelectedItem != null &&
                    float.TryParse(sizeBox.SelectedItem.ToString(), out size))
                {
                    model.FontSize = size;
                }
            }

            grid.Refresh();
        }

        private void ToggleBold(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Bold);
        }

        private void ToggleItalic(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Italic);
        }

        private void ToggleUnderline(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Underline);
        }

        private void ToggleStyle(FontStyle style)
        {
            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                CellModel model = storage.Get(cell.RowIndex, cell.ColumnIndex);

                if ((model.Style & style) == style)
                    model.Style = model.Style & ~style;
                else
                    model.Style = model.Style | style;
            }

            grid.Refresh();
        }

        private void SetFontColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (DataGridViewCell cell in grid.SelectedCells)
                {
                    storage.Get(cell.RowIndex, cell.ColumnIndex).ForeColor = dlg.Color;
                }

                grid.Refresh();
            }
        }

        private void SetBackColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (DataGridViewCell cell in grid.SelectedCells)
                {
                    storage.Get(cell.RowIndex, cell.ColumnIndex).BackColor = dlg.Color;
                }

                grid.Refresh();
            }
        }

        private void ToggleDarkMode(object sender, EventArgs e)
        {
            ThemeManager.DarkMode = !ThemeManager.DarkMode;
            ThemeManager.Apply(this, grid, toolBar);
        }
    }
}