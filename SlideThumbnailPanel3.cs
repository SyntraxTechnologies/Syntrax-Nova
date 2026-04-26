using System.Drawing;
using System.Windows.Forms;

namespace MiniPowerPoint
{
    public class SlideThumbnailPanel : ListBox
    {
        public SlideManager Manager;

        public SlideThumbnailPanel()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 60;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || Manager == null) return;

            e.Graphics.FillRectangle(Brushes.WhiteSmoke, e.Bounds);
            e.Graphics.DrawRectangle(Pens.Gray, e.Bounds);

            string text = "Slide " + (e.Index + 1);

            e.Graphics.DrawString(text, Font, Brushes.Black, e.Bounds.X + 10, e.Bounds.Y + 20);
        }

        protected override void OnSelectedIndexChanged(System.EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (Manager != null)
                Manager.CurrentIndex = SelectedIndex;
        }
    }
}