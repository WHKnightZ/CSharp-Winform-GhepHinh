using System.Drawing;

namespace GhepHinh
{
    public class PieceBitmap
    {
        private Bitmap bmp, bmpHighlight;

        public Rectangle rect;
        public bool isHighlight;

        public PieceBitmap(Bitmap bmp, Bitmap bmpHighlight, Rectangle rect)
        {
            this.bmp = bmp;
            this.bmpHighlight = bmpHighlight;
            this.rect = rect;
            this.isHighlight = false;
        }

        public Bitmap GetBmp
        {
            get
            {
                return isHighlight ? bmpHighlight : bmp;
            }
        }

        public void rotateLeft()
        {
            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            bmpHighlight.RotateFlip(RotateFlipType.Rotate270FlipNone);

            int w = rect.Width;
            rect.Width = rect.Height;
            rect.Height = w;

            int left = rect.Left + (rect.Height - rect.Width) / 2, top = rect.Top + (rect.Width - rect.Height) / 2;
            rect.Location = new Point(left, top);
        }

        public void rotateRight()
        {
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            bmpHighlight.RotateFlip(RotateFlipType.Rotate90FlipNone);
            int w = rect.Width;
            rect.Width = rect.Height;
            rect.Height = w;
        }

        public void rotate180()
        {
            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            bmpHighlight.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }
    }
}
