using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        // cần một mảng để ánh xạ xem ảnh được chọn là ảnh thứ bao nhiêu cho vào form Main
        // ảnh đầu cho vào form Main sẽ là 1, tiếp đến là 2, 3 ...
        public int[] map1 = new int[100];

        // và một mảng để biết xem ảnh đang đánh số bên Remote là ảnh thứ bao nhiêu trong danh sách
        public int[] map2 = new int[101]; // 101 do bắt đầu từ 1->100 (101 ptu, ko dùng số 0)

        // và cần một biến để đếm thứ tự mảnh, bắt đầu từ 1
        public int indexPiece = 1;

        //
        public List<Piece> pieces = new List<Piece>();
        public Piece selectedPiece = null;

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
            //// còn ảnh nào thì xóa hết ảnh đấy đi
            //for (int i = 0; i < countPieces; i++)
            //{
            //    if (picBox[i] != null)
            //        picBox[i].Dispose();
            //    if (frmRemote.picBox[i] != null)
            //        frmRemote.picBox[i].Dispose();
            //}

            //// gán lại các trạng thái lúc đầu của các form
            //selected = -1;
            //frmRemote.selected = -1;
            //indexPiece = 1;
            //frmRemote.index = 0;
            //frmRemote.lblSelected.Text = "0";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();

        }

        private List<PieceBitmap> splitImage(Bitmap img)
        {
            List<PieceBitmap> pieceBitmaps = new List<PieceBitmap>();

            int w = img.Width / col;
            int h = img.Height / row;
            int wf, hf, xo, yo, xo2, yo2;

            int[] horizontalsX = { 0, (int)(w * 0.4f), (int)(w * 0.38f), (int)(w * 0.5f), (int)(w * 0.62f), (int)(w * 0.6f), w };
            int[] horizontalsY = { 0, 0, (int)(h * 0.2f), (int)(h * 0.25f), (int)(h * 0.2f), 0, 0 };

            int[] verticalsX = { 0, 0, (int)(w * 0.2f), (int)(w * 0.25f), (int)(w * 0.2f), 0, 0 };
            int[] verticalsY = { 0, (int)(h * 0.4f), (int)(h * 0.38f), (int)(h * 0.5f), (int)(h * 0.62f), (int)(h * 0.6f), h };

            Pen p = new Pen(Color.Black) { Width = 2 };
            Pen pHighlight = new Pen(Color.Red) { Width = 2 };

            Brush brush = new TextureBrush(img);
            Point[] points;

            int countPoints = horizontalsX.Length;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    GraphicsPath path = new GraphicsPath();

                    int left = j * w, top = i * h;
                    int right = left + w, bottom = top + h;
                    int k = (i + j) % 2 == 0 ? 1 : -1;

                    // Vẽ đường bên trên
                    yo = 5;
                    if (i == 0) path.AddLine(left, top, right, top);
                    else
                    {
                        if (k == -1) yo = (int)(h * 0.3f);
                        points = new Point[countPoints];
                        for (int n = 0; n < countPoints; n++)
                            points[n] = new Point(left + horizontalsX[n], top + k * horizontalsY[n]);
                        path.AddCurve(points);
                    }

                    // Vẽ đường bên phải
                    xo2 = 5;
                    if (j == col - 1) path.AddLine(right, top, right, bottom);
                    else
                    {
                        if (k == 1) xo2 = (int)(w * 0.3f);
                        points = new Point[countPoints];
                        for (int n = 0; n < countPoints; n++)
                            points[n] = new Point(right + k * verticalsX[n], top + verticalsY[n]);
                        path.AddCurve(points);
                    }

                    // Vẽ đường bên dưới
                    yo2 = 5;
                    if (i == row - 1) path.AddLine(right, bottom, left, bottom);
                    else
                    {
                        if (k == -1) yo2 = (int)(h * 0.3f);
                        points = new Point[countPoints];
                        for (int n = 0; n < countPoints; n++)
                            points[n] = new Point(right - horizontalsX[n], bottom - k * horizontalsY[n]);
                        path.AddCurve(points);
                    }

                    // Vẽ đường bên trái
                    xo = 5;
                    if (j == 0) path.AddLine(left, bottom, left, top);
                    else
                    {
                        if (k == 1) xo = (int)(w * 0.3f);
                        points = new Point[countPoints];
                        for (int n = 0; n < countPoints; n++)
                            points[n] = new Point(left - k * verticalsX[n], bottom - verticalsY[n]);
                        path.AddCurve(points);
                    }

                    wf = xo + w + xo2;
                    hf = yo + h + yo2;
                    Bitmap bmp = new Bitmap(wf, hf);
                    Graphics g = Graphics.FromImage(bmp);

                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    g.TranslateTransform(xo - w * j, yo - h * i);
                    g.FillPath(brush, path);
                    g.DrawPath(p, path);

                    Bitmap bmp2 = new Bitmap(wf, hf);
                    Graphics g2 = Graphics.FromImage(bmp2);

                    g2.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g2.CompositingQuality = CompositingQuality.HighQuality;
                    g2.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g2.SmoothingMode = SmoothingMode.AntiAlias;

                    g2.TranslateTransform(xo - w * j, yo - h * i);
                    g2.FillPath(brush, path);
                    g2.DrawPath(pHighlight, path);

                    pieceBitmaps.Add(new PieceBitmap(bmp, bmp2, new Rectangle(0, 0, wf, hf)));
                }
            }
            return pieceBitmaps;
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

            pieces = new List<Piece>();
            List<PieceBitmap> p1 = splitImage(srcMain);
            List<PieceBitmap> p2 = splitImage(srcRemote);

            int direction;

            for (int i = 0; i < countPieces; i++)
            {
                p2[i].rect.Location = new Point(random.Next(frmRemote.remotePic.Width), random.Next(frmRemote.remotePic.Height));
                direction = random.Next(4);
                switch (direction)
                {
                    case 1: p2[i].rotateLeft(); break;
                    case 2: p2[i].rotate180(); break;
                    case 3: p2[i].rotateRight(); break;
                    default: break;
                }
                frmRemote.clamp(ref p2[i].rect);
                pieces.Add(new Piece(p1[i], p2[i], i, direction));
            }

            // chọn xong ảnh thì show form Remote và cho phép dùng Help
            frmRemote.Show();
            frmRemote.pieces = pieces;
            frmRemote.remotePic.Invalidate();
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

        // hàm này để giới hạn biên cho bức ảnh, ko thể ra ngoài mainPic
        public void clamp(ref Rectangle r)
        {
            int left = r.X;
            int top = r.Y;
            if (left < 0) left = 0; // biên trái là 0
            if (top < 0) top = 0; // biên trên 0

            // sau khi clamp xong left, top thì phải clamp cả right và bottom tuy nhiên right và bottom ko sửa trực tiếp
            // được nên cần thông qua left và top, để ý thì left + p.Width chính là right ...
            if (left + r.Width > mainPic.Width) left = mainPic.Width - r.Width;
            if (top + r.Height > mainPic.Height) top = mainPic.Height - r.Height;
            r.Location = new Point(left, top);
        }

        public void rotate()
        {
            if (selectedPiece != null)
            {
                selectedPiece.mainPiece.rotateLeft();
                selectedPiece.direction++;

                clamp(ref selectedPiece.mainPiece.rect);

                if (selectedPiece.direction == 4)
                    selectedPiece.direction = 0;
                mainPic.Invalidate();
            }
        }

        private void mainPic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (Piece piece in pieces)
            {
                if (piece.isActive)
                    g.DrawImage(piece.mainPiece.GetBmp, piece.mainPiece.rect);
            }
        }

        private Piece getPieceByMouse(int x, int y)
        {
            for (int i = countPieces - 1; i >= 0; i--)
            {
                if (pieces[i].isActive && pieces[i].mainPiece.rect.Contains(x, y))
                {
                    return pieces[i];
                }
            }
            return null;
        }

        private void mainPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // lấy ảnh được chọn theo tọa độ chuột
                Piece piece = getPieceByMouse(e.X, e.Y);
                if (piece == null)
                    return;

                // xóa highlight ảnh cũ
                if (selectedPiece != null)
                    selectedPiece.mainPiece.isHighlight = false;

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
                selectedPiece.mainPiece.isHighlight = true;

                // khi click vào ảnh thì phải đổi chỉ số bên form Remote
                //frmRemote.changeIndex(map1[selected]);
                mainPic.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                // khi bấm chuột phải, xoay ảnh
                rotate();
            }
        }

        private void mainPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;

                // nhả chuột ra thì phải check xem mảnh đấy đúng vị trí chưa

                //if (selected != -1)
                //{
                //    PictureBox pic = picBox[selected];
                //    int left = pic.Left, top = pic.Top;

                //    // tự khớp ảnh và check biên
                //    fit(pic, ref left, ref top);
                //    clamp(pic, ref left, ref top);
                //    pic.Location = new Point(left, top);

                //    checkPiece(selected);
                //}
            }
        }

        private void mainPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int top, left;

                left = selectedPiece.mainPiece.rect.Left + (e.X - currentX);
                top = selectedPiece.mainPiece.rect.Top + (e.Y - currentY);
                selectedPiece.mainPiece.rect.Location = new Point(left, top);

                currentX = e.X;
                currentY = e.Y;

                // sau khi di chuyển cần phải giới hạn lại vị trí, ko cho nó ra ngoài biên
                clamp(ref selectedPiece.mainPiece.rect);

                // Invalidate để vẽ lại pictureBox
                mainPic.Invalidate();
            }
        }

        //public void pictureBox_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        isDragging = false;

        //        // nhả chuột ra thì phải check xem mảnh đấy đúng vị trí chưa

        //        if (selected != -1)
        //        {
        //            PictureBox pic = picBox[selected];
        //            int left = pic.Left, top = pic.Top;

        //            // tự khớp ảnh và check biên
        //            fit(pic, ref left, ref top);
        //            clamp(pic, ref left, ref top);
        //            pic.Location = new Point(left, top);

        //            checkPiece(selected);
        //        }
        //    }
        //}

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
            //PictureBox p = picBox[s];
            //int left = p.Left, top = p.Top;
            //int col = (left + (p.Width / 2) - 10) / WP;
            //int row = (top + (p.Height / 2) - 20) / HP;

            //// nếu hướng của mảnh là 0 đồng thời mảnh đó nằm đúng hàng cột thì mảnh đó Ok
            //if (direction[s] == 0 && col == pos[s].X && row == pos[s].Y)
            //{
            //    map[s] = true;

            //    // check xong mảnh đó rồi thì check win luôn
            //    if (checkWin())
            //    {
            //        picBox[selected].Image = old;
            //        for (int i = 0; i < countPieces; i++)
            //        {
            //            // ko cho click vào ảnh nữa
            //            picBox[i].Enabled = false;
            //        }
            //        MessageBox.Show("Bạn đã thắng!", "Thông báo");
            //    }
            //}
            //else
            //    map[s] = false;
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
