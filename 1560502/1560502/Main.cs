using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// khi chọn hình nên ưu tiên hình có kích thước >= 480x384, tỉ lệ 5:4 (kích thước ảnh sau resize là 480x384)

namespace GhepHinh
{
    public partial class Main : Form
    {
        // form Điều khiển
        private Remote frmRemote;

        // form Giúp đỡ
        private Help frmHelp;

        // số cột, hàng của bức ảnh
        private int col, row;

        // để random giá trị cần
        private Random random = new Random();

        // lưu danh sách các mảnh ở bên form Main, lúc đầu sẽ chưa được cho vào groupBox Main
        // sẽ được thêm vào groupBox khi kích đúp vào mảnh ở form Remote
        public PictureBox[] picBox = new PictureBox[100];

        // lưu hướng xoay của các mảnh
        public int[] direction = new int[100];

        // hướng xoay 0 => next là 1, 1 => 2, 2 => 3, 3 => 0
        public int[] nextDirection = { 1, 2, 3, 0 };

        // kích thước ảnh đầy đủ ở form Main
        private const int WM = 480, HM = 384;

        // kích thước ảnh đầy đủ ở form Remote
        private const int WR = 250, HR = 200;

        // lưu kích thước của mỗi mảnh, dùng trong tự động khớp vị trí
        // nếu lấy tâm của bức ảnh / kích thước mỗi mảnh => tọa độ theo hàng và cột
        public int WP, HP;

        // check xem chuột có đang bị giữ ko, có giữ thì mới di chuyển được mảnh tranh
        public bool isDragging = false;

        // lưu vị trí mảnh khi kéo lê
        public int currentX, currentY;

        // một mảng để lưu trạng thái đúng/ sai vị trí các mảnh: đúng->map[chỉ số ảnh]=true
        // khi nào tất cả bằng true thì win
        public bool[] map;

        // cần một biến để đếm tổng số mảnh
        public int countPieces = 0;

        // một mảng để lưu tọa độ theo hàng và cột của các mảnh
        public Point[] pos = new Point[100];

        // mảnh được chọn hiện tại, = -1 nghĩa là chưa chọn
        public int selected = -1;

        // bút để vẽ highlight màu đỏ, kích thước bằng 4
        public Pen pen = new Pen(Color.Red) { Width = 4 };
        public Bitmap old;

        // cần một mảng để ánh xạ xem ảnh được chọn là ảnh thứ bao nhiêu cho vào form Main
        // ảnh đầu cho vào form Main sẽ là 1, tiếp đến là 2, 3 ...
        public int[] map1 = new int[100];

        // và một mảng để biết xem ảnh đang đánh số bên Remote là ảnh thứ bao nhiêu trong danh sách
        public int[] map2 = new int[101]; // 101 do bắt đầu từ 1->100 (101 ptu, ko dùng số 0)

        // và cần một biến để đếm thứ tự mảnh, bắt đầu từ 1
        public int indexPiece = 1;

        public Main()
        {
            InitializeComponent();

            frmRemote = new Remote();

            // lưu lại parent của form Remote để về sau có thể điều khiển từ form Remote
            frmRemote.parent = this;

            // đặt tọa độ của form Remote ở bên phải form Main
            frmRemote.Location = new Point(Width + Left + 10, Top);

            frmHelp = new Help();
            // lúc đầu tắt checkbox Help
            cbHelp.Enabled = false;
        }

        // reset các trạng thái khi chọn ảnh mới
        public void reset()
        {
            // còn ảnh nào thì xóa hết ảnh đấy đi
            for (int i = 0; i < countPieces; i++)
            {
                if (picBox[i] != null)
                    picBox[i].Dispose();
                if (frmRemote.picBox[i] != null)
                    frmRemote.picBox[i].Dispose();
            }

            // gán lại các trạng thái lúc đầu của các form
            selected = -1;
            frmRemote.selected = -1;
            indexPiece = 1;
            frmRemote.index = 0;
            frmRemote.lblSelected.Text = "0";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            Image image;

            // phòng trường hợp người dùng ko chọn tập tin hình ảnh, sử dụng một try catch để bắt lỗi
            try
            {
                image = Image.FromFile(openFileDialog.FileName);
            }
            catch
            {
                MessageBox.Show("Cần chọn một ảnh!", "Thông báo");
                return;
            }

            // chọn xong ảnh thì reset mọi thứ trước
            reset();

            // gán bức ảnh ở form Help bằng ảnh đã chọn
            frmHelp.pictureBox.Image = image;
            col = (int)numCol.Value;
            row = (int)numRow.Value;

            countPieces = col * row;
            map = new bool[countPieces];
            // khởi tạo cho chưa có mảnh nào đúng vị trí
            for (int i = 0; i < countPieces; i++)
                map[i] = false;

            // Cần resize bức ảnh được chọn về kích thước phù hợp với form
            Bitmap srcMain = new Bitmap(WM, HM); // resize form Main
            Graphics gMain = Graphics.FromImage(srcMain);
            gMain.DrawImage(image, new Rectangle(0, 0, WM, HM));

            Bitmap srcRemote = new Bitmap(WR, HR); // resize form Remote
            Graphics gRemote = Graphics.FromImage(srcRemote);
            gRemote.DrawImage(image, new Rectangle(0, 0, WR, HR));

            Rectangle cropRect;
            PictureBox pic;
            Bitmap target;
            Graphics g;

            // kích thước 1 piece ở các form Main và Remote
            WP = WM / col;
            HP = HM / row;
            int wr = WR / col, hr = HR / row, wr1, hr1;
            int k;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    k = i * col + j;

                    // cắt ảnh dựa theo tọa độ i, j
                    cropRect = new Rectangle(j * wr, i * hr, wr, hr);
                    target = new Bitmap(wr, hr);

                    // Graphics.FromImage để khởi tạo việc vẽ lên ảnh target
                    g = Graphics.FromImage(target);
                    // cắt từ ảnh srcRemote tại vị trí cropRect và vẽ lên ảnh target
                    g.DrawImage(srcRemote, new Rectangle(0, 0, wr, hr), cropRect, GraphicsUnit.Pixel);

                    // ngẫu nhiên từ 0->3 tương đương xoay trái 0->3 lần
                    // hướng xoay sẽ được lưu lại để về sau truyền sang form Main
                    direction[k] = random.Next(4);
                    switch (direction[k])
                    {
                        // 1->xoay trái 90 hay chính là xoay phải 270
                        case 1: target.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                        // 2->xoay 2 lần hay chính là 180
                        case 2: target.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                        case 3: target.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                        default: break;
                    }
                    // sau khi xoay xong có thể kích thước ảnh bị thay đổi (đổi w cho h) nên phải tính lại w, h, lưu vào wr1, hr1
                    wr1 = target.Width;
                    hr1 = target.Height;
                    pic = new PictureBox() { SizeMode = PictureBoxSizeMode.AutoSize };
                    pic.Image = target;

                    // vị trí của bức ảnh phải nằm trong khoảng 0->kích thước cha-con để tránh ảnh bị bay ra ngoài groupBox
                    pic.Location = new Point(random.Next(frmRemote.grpPieces.Width - wr1 - 20) + 10, random.Next(frmRemote.grpPieces.Height - hr1 - 28) + 20);
                    pic.Parent = frmRemote.grpPieces;

                    // sử dụng tag để lưu chỉ số của pictureBox, về sau sẽ dùng trong bắt sự kiện
                    pic.Tag = k;

                    // các sự kiện down, up, move để kéo dê pictureBox, sự kiện nháy đúp
                    pic.MouseDown += frmRemote.pictureBox_MouseDown;
                    pic.MouseUp += pictureBox_MouseUp;
                    pic.MouseMove += pictureBox_MouseMove;
                    pic.MouseDoubleClick += frmRemote.pictureBox_MouseDoubleClick;

                    frmRemote.picBox[k] = pic;

                    int posX = j * WP, posY = i * HP;
                    cropRect = new Rectangle(posX, posY, WP, HP);

                    // tọa độ theo cột, hàng của mảnh k đương nhiên là j, i
                    pos[k] = new Point(j, i);

                    target = new Bitmap(WP, HP);
                    g = Graphics.FromImage(target);
                    g.DrawImage(srcMain, new Rectangle(0, 0, WP, HP), cropRect, GraphicsUnit.Pixel);

                    pic = new PictureBox() { SizeMode = PictureBoxSizeMode.AutoSize };
                    pic.Image = target;

                    // vị trí của các bức ảnh đều nằm ở góc trái
                    pic.Location = new Point(10, 20);

                    // sử dụng tag để lưu chỉ số của pictureBox, về sau sẽ dùng trong bắt sự kiện
                    pic.Tag = k;

                    picBox[k] = pic;
                }
            }

            // chọn xong ảnh thì show form Remote và cho phép dùng Help
            frmRemote.Show();
            cbHelp.Enabled = true;
        }

        private void cbHelp_CheckedChanged(object sender, EventArgs e)
        {
            // khi thay đổi giá trị checkbox Help => show hoặc hide form Help
            if (cbHelp.Checked)
            {
                // đặt vị trí của form Help ở góc phải dưới
                frmHelp.Location = new Point(Left + Width - frmHelp.Width, Top + Height - frmHelp.Height);
                frmHelp.Show();
            }
            else
            {
                frmHelp.Hide();
            }
        }

        // hàm khớp vị trí cho các mảnh ghép, được gọi mỗi khi nhả chuột
        public void fit(PictureBox p, ref int left, ref int top)
        {
            // lấy tâm của piece / kích thước piece => tọa độ theo hàng cột
            // để ý ở đây là lấy phần nguyên (int / int => int chứ ko phải float)
            // -10 và -20 ở đây là xóa đi margin left và top
            int col = (left + (p.Width / 2) - 10) / WP;
            int row = (top + (p.Height / 2) - 20) / HP;

            left = col * WP + WP / 2 - p.Width / 2 + 10; // col * WP => tọa độ thật của ô, cộng thêm WP / 2 => tọa độ tâm
            top = row * HP + HP / 2 - p.Height / 2 + 20; // đương nhiên phải bù lại margin left và top
        }

        // hàm này để giới hạn biên cho bức ảnh, ko thể ra ngoài groupBox
        public void clamp(PictureBox p, ref int left, ref int top)
        {
            if (left < 10) left = 10; // biên trái là 10 (marginLeft 10 :D)
            if (top < 20) top = 20; // biên trên 20

            // sau khi clamp xong left, top thì phải clamp cả right và bottom tuy nhiên right và bottom ko sửa trực tiếp
            // được nên cần thông qua left và top, để ý thì left + p.Width chính là right ...
            if (left + p.Width > p.Parent.Width - 10) left = p.Parent.Width - p.Width - 10; // biên phải 10
            if (top + p.Height > p.Parent.Height - 8) top = p.Parent.Height - p.Height - 8; // biên dưới 8
        }

        public void changeHighLight(int s)
        {
            // ảnh được chọn khác với ảnh hiện tại thì mới cần sửa
            if (selected != s)
            {
                if (selected != -1)
                {
                    // nếu đã có hình được chọn thì phải xóa highlight đi bằng cách gán lại hình ảnh cũ đã lưu
                    picBox[selected].Image = old;
                }
                selected = s;

                // old để lưu lại ảnh khi chưa bị vẽ highlight lên
                old = (Bitmap)picBox[selected].Image;
                Bitmap b = new Bitmap(old);
                Graphics g = Graphics.FromImage(b);

                // dùng graphics để vẽ highlight bằng một hình chữ nhật màu đỏ
                g.DrawRectangle(pen, new Rectangle(2, 2, old.Width - 4, old.Height - 4));
                picBox[selected].Image = b;

                // đưa ảnh được chọn lên trên
                picBox[selected].BringToFront();
            }
        }

        public void rotatePicbox()
        {
            if (selected != -1)
            {
                // sửa lại direction của ảnh
                rotate(selected);

                // xoay ảnh old và ảnh đang chọn luôn
                old.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Bitmap b = (Bitmap)picBox[selected].Image;
                b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                PictureBox pic = picBox[selected];
                pic.Image = b;

                // đặt lại vị trí bức ảnh để vị trí xoay tại tâm
                int left = pic.Left + (pic.Height - pic.Width) / 2, top = pic.Top + (pic.Width - pic.Height) / 2;

                // tự khớp ảnh và check biên
                fit(pic, ref left, ref top);
                clamp(pic, ref left, ref top);
                pic.Location = new Point(left, top);

                // check xem mảnh ghép đúng chưa
                checkPiece(selected);
            }
        }

        public void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // lấy ảnh được chọn
                PictureBox pic = sender as PictureBox;

                // vẽ highlight lên mảnh được chọn đồng thời lưu lại tọa độ chuột
                // để về sau kéo dê chuột bao nhiêu thì ảnh bị kéo theo bấy nhiêu
                changeHighLight((int)pic.Tag);

                // khi click vào ảnh thì phải đổi chỉ số bên form Remote
                frmRemote.changeIndex(map1[selected]);
                isDragging = true;
                currentX = e.X;
                currentY = e.Y;
            }
            else if (e.Button == MouseButtons.Right)
            {
                // khi bấm chuột phải, xoay ảnh
                rotatePicbox();
            }
        }

        public void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int top, left;
                PictureBox picBox = sender as PictureBox;
                left = picBox.Left + (e.X - currentX);
                top = picBox.Top + (e.Y - currentY);

                // sau khi di chuyển cần phải giới hạn lại vị trí, ko cho nó ra ngoài biên
                clamp(picBox, ref left, ref top);

                // và đặt lại vị trí
                picBox.Location = new Point(left, top);
            }
        }

        public void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;

                // nhả chuột ra thì phải check xem mảnh đấy đúng vị trí chưa

                if (selected != -1)
                {
                    PictureBox pic = picBox[selected];
                    int left = pic.Left, top = pic.Top;

                    // tự khớp ảnh và check biên
                    fit(pic, ref left, ref top);
                    clamp(pic, ref left, ref top);
                    pic.Location = new Point(left, top);

                    checkPiece(selected);
                }
            }
        }

        public void rotate(int s)
        {
            direction[s] = nextDirection[direction[s]];
        }

        public bool checkWin()
        {
            // tất cả các phần tử của map = true thì win
            for (int i = 0; i < countPieces; i++)
                if (!map[i])
                    return false;
            return true;
        }

        public void checkPiece(int s)
        {
            PictureBox p = picBox[s];
            int left = p.Left, top = p.Top;
            int col = (left + (p.Width / 2) - 10) / WP;
            int row = (top + (p.Height / 2) - 20) / HP;

            // nếu hướng của mảnh là 0 đồng thời mảnh đó nằm đúng hàng cột thì mảnh đó Ok
            if (direction[s] == 0 && col == pos[s].X && row == pos[s].Y)
            {
                map[s] = true;

                // check xong mảnh đó rồi thì check win luôn
                if (checkWin())
                {
                    picBox[selected].Image = old;
                    for (int i = 0; i < countPieces; i++)
                    {
                        // ko cho click vào ảnh nữa
                        picBox[i].Enabled = false;
                    }
                    MessageBox.Show("Bạn đã thắng!", "Thông báo");
                }
            }
            else
                map[s] = false;
        }

        private void Main_LocationChanged(object sender, EventArgs e)
        {
            // form Main di chuyển thì form Remote di chuyển theo
            // tuy nhiên phải check xem vị trí nó ko đúng thì mới cần đặt lại
            if (frmRemote.Left != Width + Left + 10 || frmRemote.Top != Top)
                frmRemote.Location = new Point(Width + Left + 10, Top);
        }

    }
}
