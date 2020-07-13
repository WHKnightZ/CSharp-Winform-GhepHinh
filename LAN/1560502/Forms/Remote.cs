using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GhepHinh
{
    public partial class Remote : Form
    {
        // lưu form Main để tương tác qua các button điều khiển
        public Main parent;

        // chỉ số trong label Selected
        public int index = 0;

        public bool isDragging = false;
        public int currentX, currentY;

        public List<Piece> pieces;
        public Piece selectedPiece = null;

        public Remote()
        {
            InitializeComponent();
        }

        // hàm này để giới hạn biên cho bức ảnh, ko thể ra ngoài remotePic
        public void clamp(ref Rectangle r)
        {
            int left = r.X;
            int top = r.Y;
            if (left < 0) left = 0; // biên trái là 0
            if (top < 0) top = 0; // biên trên 0

            // sau khi clamp xong left, top thì phải clamp cả right và bottom tuy nhiên right và bottom ko sửa trực tiếp
            // được nên cần thông qua left và top, để ý thì left + p.Width chính là right ...
            if (left + r.Width > remotePic.Width) left = remotePic.Width - r.Width;
            if (top + r.Height > remotePic.Height) top = remotePic.Height - r.Height;
            r.Location = new Point(left, top);
        }

        public void rotate()
        {
            if (selectedPiece != null)
            {
                selectedPiece.remotePiece.rotateLeft();
                selectedPiece.direction++;

                clamp(ref selectedPiece.remotePiece.rect);

                if (selectedPiece.direction == 4)
                    selectedPiece.direction = 0;
                remotePic.Invalidate();
            }
        }

        // đưa mảnh được chọn sang form Main, được gọi khi nháy đúp chuột vào 1 mảnh
        public void append()
        {
            //cần xoay bức ảnh bên main cho đúng chiều bên remote
            switch (selectedPiece.direction)
            {
                case 1: selectedPiece.mainPiece.rotateLeft(); break;
                case 2: selectedPiece.mainPiece.rotate180(); break;
                case 3: selectedPiece.mainPiece.rotateRight(); break;
                default: break;
            }

            // xóa highlight cũ đi và tạo highlight mới
            if (parent.selectedPiece != null)
                parent.selectedPiece.mainPiece.isHighlight = false;

            parent.selectedPiece = selectedPiece;
            selectedPiece.isActive = true;
            parent.selectedPiece.mainPiece.isHighlight = true;
            parent.clamp();

            parent.checkPiece();

            //cài đặt ánh xạ
            parent.map1[selectedPiece.index] = parent.indexPiece;
            parent.map2[parent.indexPiece] = selectedPiece.index;
            parent.indexPiece++;

            changeIndex(parent.indexPiece - 1);

            selectedPiece = null;

            // đưa ảnh sang thì ảnh bên form remote và main đều thay đổi nên cần cập nhật lại
            parent.mainPic.Invalidate();
            remotePic.Invalidate();
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

        // tương tự hàm vẽ bên form main, nhưng bên đây piece phải ko active mới đc vẽ
        private void remotePic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (Piece piece in parent.pieces)
            {
                if (!piece.isActive)
                    g.DrawImage(piece.remotePiece.GetBmp, piece.remotePiece.rect);
            }
        }

        private Piece getPieceByMouse(int x, int y)
        {
            for (int i = parent.countPieces - 1; i >= 0; i--)
            {
                if (!pieces[i].isActive && pieces[i].remotePiece.rect.Contains(x, y))
                {
                    return pieces[i];
                }
            }
            return null;
        }

        private void remotePic_MouseDown(object sender, MouseEventArgs e)
        {
            if (!parent.isLocked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // lấy ảnh được chọn theo tọa độ chuột
                    Piece piece = getPieceByMouse(e.X, e.Y);
                    if (piece == null)
                        return;

                    // xóa highlight ảnh cũ
                    if (selectedPiece != null)
                        selectedPiece.remotePiece.isHighlight = false;

                    // được phép kéo dê ảnh
                    isDragging = true;

                    // lưu lại tọa độ chuột để về sau kéo dê chuột bao nhiêu thì ảnh bị kéo theo bấy nhiêu
                    currentX = e.X;
                    currentY = e.Y;

                    // lưu lại ảnh đã chọn, đồng thời đưa ảnh lên trên bằng cách xóa nó khỏi danh sách
                    // và đưa nó xuống cuối (do ảnh cuối danh sách được vẽ cuối cùng, sẽ ở trên cùng)
                    selectedPiece = piece;
                    pieces.Remove(selectedPiece);
                    pieces.Add(selectedPiece);

                    // tạo highlight ảnh mới
                    selectedPiece.remotePiece.isHighlight = true;

                    remotePic.Invalidate();

                    // gửi sự kiện khóa form Remote và sự kiện chuyển mảnh được chọn
                    var data = new SelectData(selectedPiece.index);
                    parent.Send(new SendObject(SendObject.LOCK_REMOTE, data));
                }
                else if (e.Button == MouseButtons.Right)
                {
                    // gửi sự kiện xoay mảnh được chọn
                    parent.Send(new SendObject(SendObject.ROTATE_REMOTE, null));
                    rotate();
                }
            }
        }

        private void remotePic_MouseUp(object sender, MouseEventArgs e)
        {
            if (!parent.isLocked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // gửi sự kiện mở khóa
                    if (selectedPiece != null)
                    {
                        var data = new TranslateData(selectedPiece.remotePiece.rect.Left,
                                selectedPiece.remotePiece.rect.Top);
                        parent.Send(new SendObject(SendObject.UNLOCK_REMOTE, data));
                    }
                    isDragging = false;
                }
            }
        }

        private void remotePic_MouseMove(object sender, MouseEventArgs e)
        {
            if (!parent.isLocked)
            {
                if (isDragging)
                {
                    int top, left;

                    left = selectedPiece.remotePiece.rect.Left + (e.X - currentX);
                    top = selectedPiece.remotePiece.rect.Top + (e.Y - currentY);
                    selectedPiece.remotePiece.rect.Location = new Point(left, top);

                    currentX = e.X;
                    currentY = e.Y;

                    // sau khi di chuyển cần phải giới hạn lại vị trí, ko cho nó ra ngoài biên
                    clamp(ref selectedPiece.remotePiece.rect);

                    // Invalidate để vẽ lại pictureBox
                    remotePic.Invalidate();
                }
            }
        }

        // double click thì đưa mảnh ghép sang form main
        private void remotePic_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!parent.isLocked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (selectedPiece == null)
                        return;

                    parent.Send(new SendObject(SendObject.APPEND_MAIN, null));
                    append();
                }
            }
        }

        // các hàm của button điều khiển
        private void btnMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if (!parent.isLocked)
            {
                if (parent.indexPiece > 1)
                    changeIndex(index - 1);

                int i = parent.map2[index];
                var data = new SelectData(i);
                parent.Send(new SendObject(SendObject.SELECT_MAIN, data));
                parent.changePiece(i);
            }
        }

        private void btnPlus_MouseDown(object sender, MouseEventArgs e)
        {
            if (!parent.isLocked)
            {
                if (parent.indexPiece > 1)
                    changeIndex(index + 1);
                int i = parent.map2[index];
                var data = new SelectData(i);
                parent.Send(new SendObject(SendObject.SELECT_MAIN, data));
                parent.changePiece(i);
            }
        }

        private void btnRotate_MouseDown(object sender, MouseEventArgs e)
        {
            if (!parent.isLocked)
            {
                parent.Send(new SendObject(SendObject.ROTATE_MAIN, null));
                parent.rotate();
            }
        }

        // dịch ảnh theo từng ô, công thêm một lượng bằng kích thước 1 ô WP và HP
        private void translate(int x, int y)
        {
            if (!parent.isLocked)
            {
                parent.selectedPiece.x += x;
                parent.selectedPiece.y += y;

                int left = parent.selectedPiece.mainPiece.rect.Left, top = parent.selectedPiece.mainPiece.rect.Top;
                left += x * parent.WP;
                top += y * parent.HP;

                parent.selectedPiece.mainPiece.rect.Location = new Point(left, top);

                parent.clamp();

                var data = new TranslateData(parent.selectedPiece.mainPiece.rect.Left,
                    parent.selectedPiece.mainPiece.rect.Top, parent.selectedPiece.x, parent.selectedPiece.y);
                parent.Send(new SendObject(SendObject.TRANSLATE_MAIN, data));

                parent.checkPiece();

                parent.mainPic.Invalidate();
            }
        }

        private void btnLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (parent.selectedPiece != null && parent.selectedPiece.x > 0)
            {
                translate(-1, 0);
            }
        }

        private void btnRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (parent.selectedPiece != null && parent.selectedPiece.x < parent.col - 1)
            {
                translate(1, 0);
            }
        }

        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (parent.selectedPiece != null && parent.selectedPiece.y > 0)
            {
                translate(0, -1);
            }
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (parent.selectedPiece != null && parent.selectedPiece.y < parent.row - 1)
            {
                translate(0, 1);
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
