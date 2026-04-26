using System.Drawing;

namespace MiniExcel
{
    public class CellModel
    {
        public string Text = "";

        public FontStyle Style = FontStyle.Regular;
        public string FontName = "Calibri";
        public float FontSize = 11f;

        public Color ForeColor = Color.Black;
        public Color BackColor = Color.White;
    }
}