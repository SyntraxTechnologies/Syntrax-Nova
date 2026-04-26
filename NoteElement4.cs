using System.Drawing;

namespace MiniOneNote
{
    public enum NoteType
    {
        Text,
        Image
    }

    public class NoteElement
    {
        public NoteType Type = NoteType.Text;

        public int X = 50;
        public int Y = 50;
        public int Width = 250;
        public int Height = 80;

        public string Text = "New Note";

        // TEXT STYLE
        public string FontName = "Calibri";
        public float FontSize = 14f;

        public bool Bold = false;
        public bool Italic = false;
        public bool Underline = false;

        public Color TextColor = Color.Black;

        // IMAGE
        public Image Image = null;
    }
}