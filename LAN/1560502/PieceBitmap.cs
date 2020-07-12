using System;
using System.Drawing;

namespace GhepHinh
{
    [Serializable]
    public class PieceBitmap
    {
        // lưu lại ảnh khi ko có highlight và có highlight
        private Bitmap bmp, bmpHighlight;

        // khu vực vẽ của mỗi mảnh
        public Rectangle rect;

        // mảnh ghép đó bị lệch so với tâm bao nhiêu (mỗi mảnh có thể bị lồi ra, lõm vào nên bị lệch so với tâm một khoảng)
        public Point offsetCenter;
        
        // mảnh đó được highlight ko?
        public bool isHighlight;

        public PieceBitmap(Bitmap bmp, Bitmap bmpHighlight, Rectangle rect, Point offsetCenter)
        {
            this.bmp = bmp;
            this.bmpHighlight = bmpHighlight;
            this.rect = rect;
            this.offsetCenter = offsetCenter;
            this.isHighlight = false;
        }

        public Bitmap GetBmp
        {
            get
            {
                // nếu có highlight thì dùng mảnh có highlight để vẽ và ngược lại
                return isHighlight ? bmpHighlight : bmp;
            }
        }

        // khi xoay trái thì phải xoay cả 2 ảnh có highlight và ko, chiều W, H sẽ bị đảo cho nhau,
        // do cần xoay tại tâm nên phải sử dụng công thức dưới
        public void rotateLeft()
        {
            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            bmpHighlight.RotateFlip(RotateFlipType.Rotate270FlipNone);

            int w = rect.Width;
            rect.Width = rect.Height;
            rect.Height = w;

            int left = rect.Left + (rect.Height - rect.Width) / 2, top = rect.Top + (rect.Width - rect.Height) / 2;
            rect.Location = new Point(left, top);

            int x = offsetCenter.X;
            offsetCenter.X = offsetCenter.Y;
            offsetCenter.Y = -x;
        }

        public void rotateRight()
        {
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            bmpHighlight.RotateFlip(RotateFlipType.Rotate90FlipNone);
            int w = rect.Width;
            rect.Width = rect.Height;
            rect.Height = w;

            int x = offsetCenter.X;
            offsetCenter.X = -offsetCenter.Y;
            offsetCenter.Y = x;
        }

        public void rotate180()
        {
            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            bmpHighlight.RotateFlip(RotateFlipType.Rotate180FlipNone);

            offsetCenter.X = -offsetCenter.X;
            offsetCenter.Y = -offsetCenter.Y;
        }
    }
}
