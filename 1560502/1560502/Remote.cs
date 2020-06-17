using System;
using System.Drawing;
using System.Windows.Forms;

namespace GhepHinh
{
    public partial class Remote : Form
    {
        // lưu danh sách các mảnh ở bên form Remote
        // sẽ bị xóa khi kích đúp vào bằng hàm Dispose()
        public PictureBox[] picBox = new PictureBox[100];

        // lưu form Main để tương tác qua các button điều khiển
        public Main parent;

        // bút để vẽ highlight màu đỏ, kích thước bằng 4
        public Pen pen = new Pen(Color.Red) { Width = 4 };

        public Bitmap old;

        // ảnh đang chọn trong from Remote
        public int selected = -1;

        // chỉ số trong label Selected
        public int index = 0;

        public Remote()
        {
            InitializeComponent();
        }

        // hàm vẽ highlight bằng một hình chữ nhật màu đỏ
        public void changeHighLight(int s)
        {
            if (selected != s)
            {
                if (selected != -1)
                {
                    picBox[selected].Image = old;
                }
                selected = s;

                // lưu lại ảnh để về sau nếu chọn ảnh khác thì gán lại
                old = (Bitmap)picBox[selected].Image;
                Bitmap b = new Bitmap(old);
                Graphics g = Graphics.FromImage(b);

                // vẽ hình chữ nhật lên ảnh mới
                g.DrawRectangle(pen, new Rectangle(2, 2, old.Width - 4, old.Height - 4));
                picBox[selected].Image = b;

                // đưa ảnh được chọn lên trên
                picBox[selected].BringToFront();
            }
        }

        public void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox pic = sender as PictureBox;

                // vẽ highlight
                changeHighLight((int)pic.Tag);

                parent.isDragging = true;
                parent.currentX = e.X;
                parent.currentY = e.Y;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (selected != -1)
                {
                    // sửa lại direction của mảnh
                    parent.rotate(selected);

                    // xoay ảnh old và ảnh đang chọn luôn
                    old.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    Bitmap b = (Bitmap)picBox[selected].Image;
                    b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    PictureBox pic = picBox[selected];
                    pic.Image = b;

                    // đặt lại vị trí bức ảnh để vị trí xoay tại tâm
                    int left = pic.Left + (pic.Height - pic.Width) / 2, top = pic.Top + (pic.Width - pic.Height) / 2;

                    // clamp để tránh ảnh bị tràn ra ngoài
                    parent.clamp(pic, ref left, ref top);
                    pic.Location = new Point(left, top);
                }
            }
        }

        public void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox pic = sender as PictureBox;

                // lấy chỉ số bức ảnh
                int selected = (int)pic.Tag;

                // xóa bức ảnh bên Remote
                pic.Dispose();

                Bitmap b = (Bitmap)parent.picBox[selected].Image;

                // cần xoay bức ảnh bên main cho đúng chiều bên remote
                switch (parent.direction[selected])
                {
                    case 1: b.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                    case 2: b.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                    case 3: b.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                    default: break;
                }

                pic = parent.picBox[selected];
                pic.Image = b;

                // gán parent để thêm PictureBox này vào trong grpMain
                pic.Parent = parent.grpMain;

                // thêm sự kiện
                pic.MouseDown += parent.pictureBox_MouseDown;
                pic.MouseUp += parent.pictureBox_MouseUp;
                pic.MouseMove += parent.pictureBox_MouseMove;

                parent.changeHighLight(selected);

                parent.checkPiece(selected);

                // cài đặt ánh xạ
                parent.map1[selected] = parent.indexPiece;
                parent.map2[parent.indexPiece] = selected;
                parent.indexPiece++;

                changeIndex(parent.indexPiece - 1);
            }
        }

        // hàm này để sửa chỉ số trong label Selected
        public void changeIndex(int i)
        {
            index = i;
            // khi bấm - nếu index về 0 thì quay vòng đến cuối cùng và ngược lại
            if (index == 0)
                index = parent.indexPiece - 1;
            else if (index == parent.indexPiece)
                index = 1;
            lblSelected.Text = "" + index;
        }

        // các hàm của button điều khiển
        private void btnRotate_Click(object sender, EventArgs e)
        {
            parent.rotatePicbox();
        }

        private void btnMinus_MouseClick(object sender, MouseEventArgs e)
        {
            if (parent.indexPiece > 1)
                changeIndex(index - 1);
            parent.changeHighLight(parent.map2[index]);
        }

        private void btnPlus_MouseClick(object sender, MouseEventArgs e)
        {
            if (parent.indexPiece > 1)
                changeIndex(index + 1);
            parent.changeHighLight(parent.map2[index]);
        }

        private void btnLeft_MouseDown(object sender, MouseEventArgs e)
        {
            // bật timer và đặt offset, sang trái thì x = -5, y = 0 ...
            if (parent.selected != -1)
            {
                parent.timer.Enabled = true;
                parent.offsetX = -5;
                parent.offsetY = 0;
            }
        }

        private void btnRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (parent.selected != -1)
            {
                parent.timer.Enabled = true;
                parent.offsetX = 5;
                parent.offsetY = 0;
            }
        }

        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (parent.selected != -1)
            {
                parent.timer.Enabled = true;
                parent.offsetX = 0;
                parent.offsetY = -5;
            }
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (parent.selected != -1)
            {
                parent.timer.Enabled = true;
                parent.offsetX = 0;
                parent.offsetY = 5;
            }
        }

        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            if (parent.selected != -1)
            {
                // nhả button thì cần tắt timer, tự khớp ảnh, check xem win ko
                parent.timer.Enabled = false;

                PictureBox pic = parent.picBox[selected];
                int left = pic.Left, top = pic.Top;

                // tự khớp ảnh và check biên
                parent.fit(pic, ref left, ref top);
                parent.clamp(pic, ref left, ref top);
                pic.Location = new Point(left, top);

                parent.checkPiece(parent.selected);
            }
        }

        private void Remote_LocationChanged(object sender, EventArgs e)
        {
            // form Remote di chuyển thì form Main di chuyển theo
            // tuy nhiên phải check xem vị trí nó ko đúng thì mới cần đặt lại
            if (parent.Left != Left - 10 - parent.Width || parent.Top != Top)
                parent.Location = new Point(Left - 10 - parent.Width, Top);
        }
    }
}
