using System.Drawing;

namespace MiniPowerPoint
{
    public enum ElementType
    {
        Text,
        Image,
        Rectangle,
        Ellipse
    }

    public class SlideElement
    {
        public ElementType Type = ElementType.Text;

        public int X = 50;
        public int Y = 50;
        public int Width = 200;
        public int Height = 80;

        public string Text = "Text";

        public string FontName = "Calibri";
        public float FontSize = 18f;
        public FontStyle Style = FontStyle.Regular;

        public Color TextColor = Color.Black;
        public Color FillColor = Color.LightGray;
        public Color BorderColor = Color.Black;

        public Image Image = null;
    }
}