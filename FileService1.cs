using System.Windows.Forms;

namespace MiniWordPro
{
    public static class FileService
    {
        public static void Open(RichTextBox b)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "RTF|*.rtf";

            if (d.ShowDialog() == DialogResult.OK)
                b.LoadFile(d.FileName);
        }

        public static void Save(RichTextBox b)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "RTF|*.rtf";

            if (d.ShowDialog() == DialogResult.OK)
                b.SaveFile(d.FileName);
        }
    }
}