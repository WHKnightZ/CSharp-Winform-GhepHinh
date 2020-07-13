using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

// ở v1, sử dụng 1 list các pictureBox, tuy nhiên ở v2 đã thay hết list pictureBox thành một pictureBox
// kèm theo sự kiện paint (vẽ lên pictureBox), chúng ta sẽ sử dụng lớp Graphics kèm theo 1 list các Bitmap
// để vẽ lên pictureBox đó

// với kết nối mạng LAN:
// Connection/Server.cs: tạo server đợi các client kết nối, khi nhận được sự kiện từ client sẽ bắn sang các client còn lại
// Connection/Client.cs: tạo client kết nối đến server
// Connection/SendObject.cs: lưu object được gửi đi, object có thông tin event và data kèm theo
// còn lại là các lớp để lưu thông tin data truyền đi, như là SelectData lưu mảnh được chọn, chỉ cần mỗi chỉ số
// InitData thì lưu toàn bộ thông tin khởi tạo, khá nhiều thứ, TranslateData thì phải lưu tọa độ của hình chữ nhật

// Có bổ sung thêm form InputIP để nhập IP, form Choose để chọn Chủ, Khách

// khi chọn hình nên ưu tiên hình có kích thước >= 480x384, tỉ lệ 5:4 (kích thước ảnh sau resize là 480x384)

namespace GhepHinh
{
    public partial class Main : Form
    {
        // mỗi mảnh ghép phải vẽ highlight ở biên, mảnh ghép đó sẽ to lên 
        // một chút nên phải có thêm một cái offset 
        private const int OFFSET = 5;

        // form Điều khiển
        public Remote frmRemote;

        // form Giúp đỡ
        public Help frmHelp;

        // số cột, hàng của bức ảnh
        public int col, row;

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

        // cần một mảng để ánh xạ xem ảnh được chọn là ảnh thứ bao nhiêu cho vào form Main
        // ảnh đầu cho vào form Main sẽ là 1, tiếp đến là 2, 3 ...
        public int[] map1 = new int[100];

        // và một mảng để biết xem ảnh đang đánh số bên Remote là ảnh thứ bao nhiêu trong danh sách
        public int[] map2 = new int[101]; // 101 do bắt đầu từ 1->100 (101 ptu, ko dùng số 0)

        // và cần một biến để đếm thứ tự mảnh, bắt đầu từ 1
        public int indexPiece = 1;

        // Lưu thông tin các mảnh ghép
        public List<Piece> pieces = new List<Piece>();

        // Mảnh ghép đang được chọn
        public Piece selectedPiece = null;

        // Lưu Image lúc đầu để về sau khi win sẽ vẽ lại picBox bằng ảnh này
        public Bitmap image;

        // IP
        public string IP;

        // lưu server, client để tương tác giữa các máy, mỗi form chỉ có
        // một trong 2 server hoặc client hoạt động, phụ thuộc vào isServer
        public bool isServer; // cũng có thể tạo thêm một biến mới: sender, khi khởi tạo, sender
        public Server server; // sẽ được gán = server hoặc client, về sau dùng luôn sender để gửi
        public Client client; // chứ ko cần check isServer nữa

        public Queue<SendObject> sendObjects = new Queue<SendObject>();
        // mỗi khi có người đang kéo lê một mảnh ở form Main thì người khác
        // ko thể điều khiển, cho đến khi người đó nhả mảnh, isLocked để khóa kéo lê
        public bool isLocked;

        public Main()
        {
            InitializeComponent();

            // lấy IP của máy mình để cho vào cái label ở góc, máy khác nhập cái
            // IP này vào form để kết nối
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = address.ToString();
                    lblIP.Text = "IP: " + IP;
                    break;
                }
            }

            // nghe nói là để tránh xung đột khi nhiều thread cùng truy cập một tài nguyên
            CheckForIllegalCrossThreadCalls = false;

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
            // nếu server, client đã có thì phải giải phóng trước, chẳng hạn lúc game đang chơ mà chơi lại
            if (server != null)
                server.Close();
            if (client != null)
                client.Close();

            // cho phép form hoạt động
            isLocked = false;

            // khởi tạo vài biến cơ bản
            pieces.Clear();
            countPieces = 0;
            selectedPiece = null;
            frmRemote.selectedPiece = null;
            indexPiece = 1;
            frmRemote.index = 0;
            frmRemote.lblSelected.Text = "0";
            mainPic.Image = null;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // bật form chọn Chủ, Khách
            Choose choose = new Choose();
            var result = choose.ShowDialog();
            if (result == DialogResult.OK)
            {
                // nếu là chủ thì cho chọn ảnh, khách thì yêu cầu nhập IP
                if (choose.isServer)
                    openFileDialog.ShowDialog();
                else
                {
                    InputIP input = new InputIP(IP);
                    var result2 = input.ShowDialog();
                    if (result2 == DialogResult.OK)
                    {
                        reset();
                        IP = input.IP;
                        isServer = false;

                        // khởi tạo client, xem trong Connection/Client.cs để rõ hơn
                        client = new Client(this, frmRemote, frmHelp, IP);
                        if (!client.isActive)
                            return;

                        this.Text = "Ghép hình - Khách";
                    }
                }
            }
        }

        // chia bức ảnh được chọn thành các miếng nhỏ
        private List<PieceBitmap> splitImage(Bitmap img)
        {
            List<PieceBitmap> pieceBitmaps = new List<PieceBitmap>();

            int w = img.Width / col;
            int h = img.Height / row;

            // w là chiều rộng, cao của mảnh ghép khi chưa tính cái phần lồi ra, lõm vào, khi tính cả
            // phần lồi ra lõm vào thì sẽ có kích thước full chính là wf, hf, còn xo có thể coi là kích thước
            // của phần lồi ra đấy, = OFFSET khi ko lồi, bằng w*0.3 khi lồi
            int wf, hf, xo, yo, xo2, yo2;

            // để tạo ra các cái phần lồi, lõm thì cần cắt ảnh ban đầu theo các đường cong, dựa theo các điểm bên dưới
            // hãy thử vẽ ra giấy, giả sử chọn một hình chữ nhật với kích thước w = 100, h = 100, xong lấy tọa độ các
            // đỉnh như bên dưới (bao gồm horizontalsX và horizontalsY), nối lại sẽ được mảnh jigsaw puzzle
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
                    // lớp này để tạo đường cong cắt ảnh
                    GraphicsPath path = new GraphicsPath();

                    int left = j * w, top = i * h;
                    int right = left + w, bottom = top + h;
                    int k = (i + j) % 2 == 0 ? 1 : -1;

                    // Vẽ đường bên trên
                    yo = OFFSET;
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
                    xo2 = OFFSET;
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
                    yo2 = OFFSET;
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
                    xo = OFFSET;
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

                    // tăng chất lượng graphics
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    g.TranslateTransform(xo - w * j, yo - h * i);

                    // cắt ảnh
                    g.FillPath(brush, path);
                    // vẽ viền
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

                    pieceBitmaps.Add(new PieceBitmap(bmp, bmp2, new Rectangle(0, 0, wf, hf), new Point((xo2 - xo) / 2, (yo2 - yo) / 2)));
                }
            }
            return pieceBitmaps;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            // phòng trường hợp người dùng ko chọn tập tin hình ảnh, sử dụng một try catch để bắt lỗi
            try
            {
                image = (Bitmap)Image.FromFile(openFileDialog.FileName);
            }
            catch
            {
                MessageBox.Show("Cần chọn một ảnh!", "Thông báo");
                return;
            }

            // chọn xong ảnh thì reset mọi thứ trước
            reset();

            isServer = true;

            // khởi tạo server, xem Connection/Server.cs
            server = new Server(this, frmRemote, frmHelp);
            if (!server.isActive)
                return;

            this.Text = "Ghép hình - Chủ";

            // gán bức ảnh ở form Help bằng ảnh đã chọn
            frmHelp.pictureBox.Image = image;
            col = (int)numCol.Value;
            row = (int)numRow.Value;

            WP = WM / col;
            HP = HM / row;

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

            // Danh sách các mảnh ở form Main và Remote
            List<PieceBitmap> p1 = splitImage(srcMain);
            List<PieceBitmap> p2 = splitImage(srcRemote);

            int direction;

            // tổng hợp 2 danh sách thành một danh sách mới, có các thông số chung như direction, index
            // ở đây cũng sẽ đảo ngẫu nhiên vị trí và hướng xoay của các mảnh bên form Remote
            // đồng thời tính tâm của các mảnh trong form Main
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

            mainPic.Enabled = true;
            frmRemote.remotePic.Enabled = true;

            // chọn xong ảnh thì show form Remote và cho phép dùng Help
            frmRemote.Show();

            // thêm một cái pieces bên Remote tham chiếu đến pieces bên này để đỡ phải viết nhiều,
            // ko có thì ở bên kia muốn dùng sẽ phải gọi parent.pieces
            frmRemote.pieces = pieces;

            // hàm invalidate để vẽ lại pictureBox
            mainPic.Invalidate();
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
        public void fit()
        {
            Rectangle r = selectedPiece.mainPiece.rect;

            // lấy tâm của piece / kích thước piece => tọa độ theo hàng cột
            // để ý ở đây là lấy phần nguyên (int / int => int chứ ko phải float)
            int left = r.X, top = r.Y;
            int col = (left + (r.Width / 2) - OFFSET) / WP;
            int row = (top + (r.Height / 2) - OFFSET) / HP;

            // col * WP + WP / 2 sẽ lấy được tâm ô đang xét, - r.Width / 2 sẽ đưa về biên trái của mảnh
            // sau đó bù thêm lượng offset đã tính lúc đầu, mảnh sẽ đưa về chính giữa
            left = col * WP + WP / 2 - r.Width / 2 + selectedPiece.mainPiece.offsetCenter.X + OFFSET;
            top = row * HP + HP / 2 - r.Height / 2 + selectedPiece.mainPiece.offsetCenter.Y + OFFSET;

            selectedPiece.mainPiece.rect.Location = new Point(left, top);

            // đương nhiên cũng phải tính lại tọa độ theo hàng cột để check win
            selectedPiece.x = col;
            selectedPiece.y = row;
        }

        // hàm này để giới hạn biên cho bức ảnh, ko thể ra ngoài mainPic
        public void clamp()
        {
            Rectangle r = selectedPiece.mainPiece.rect;

            int left = r.X;
            int top = r.Y;
            if (left < 0) left = 0; // biên trái là 0
            if (top < 0) top = 0; // biên trên 0

            // sau khi clamp xong left, top thì phải clamp cả right và bottom tuy nhiên right và bottom ko sửa trực tiếp
            // được nên cần thông qua left và top, để ý thì left + p.Width chính là right ...
            if (left + r.Width > mainPic.Width) left = mainPic.Width - r.Width;
            if (top + r.Height > mainPic.Height) top = mainPic.Height - r.Height;

            selectedPiece.mainPiece.rect.Location = new Point(left, top);
        }

        public void rotate()
        {
            if (selectedPiece != null)
            {
                selectedPiece.mainPiece.rotateLeft();
                selectedPiece.direction++;

                fit();
                clamp();

                if (selectedPiece.direction == 4)
                    selectedPiece.direction = 0;
                mainPic.Invalidate();

                checkPiece();
            }
        }

        // hàm vẽ các mảnh ghép lên pictureBox picMain
        private void mainPic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // vẽ các mảnh ghép lên pictureBox, ở đây isActive == true nghĩa là nó có ở bên form main
            // false nghĩa là chỉ có bên form remote, còn nữa, các mảnh được vẽ lần lượt, mảnh ở cuối danh
            // sách sẽ vẽ cuối cùng nên đương nhiên nó nằm trên cùng
            foreach (Piece piece in pieces)
            {
                if (piece.isActive)
                    g.DrawImage(piece.mainPiece.GetBmp, piece.mainPiece.rect);
            }
        }

        // hàm này để thay đổi mảnh được chọn bởi button +, - bên form Remote
        public void changePiece(int index)
        {
            // nếu hiện có mảnh đang được chọn thì phải xóa highlight mảnh đó đi
            if (selectedPiece != null)
                selectedPiece.mainPiece.isHighlight = false;

            // tìm ra mảnh có index giống với index được chỉ định
            Piece piece = null;
            foreach (Piece p in pieces)
            {
                if (p.index == index)
                {
                    piece = p;
                    break;
                }
            }

            if (piece == null)
                return;

            // lưu lại ảnh đã chọn, đồng thời đưa ảnh lên trên bằng cách xóa nó khỏi danh sách
            // và đưa nó xuống cuối (do ảnh cuối danh sách được vẽ cuối cùng, sẽ ở trên cùng)
            selectedPiece = piece;
            selectedPiece.mainPiece.isHighlight = true;

            pieces.Remove(selectedPiece);
            pieces.Add(selectedPiece);

            mainPic.Invalidate();
        }

        // ở đây, muốn lấy được mảnh tại tọa độ chuột, đơn giản chỉ cần check trong toàn
        // bộ danh sách các mảnh, mảnh nào chứa tọa độ đang xét thì mảnh đó là mảnh được chọn
        // rect là hình chữ nhật chứa khu vực vẽ của mảnh ghép, và ở đây phải xét từ cuối danh
        // sách do những mảnh ở cuối là mảnh nằm bên trên, phải được xét trước
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
            // các sự kiện mouse, control bên form remote từ giờ sẽ bổ sung thêm check isLocked
            // để xem có người khác đang điều khiển thì khóa ko cho form này dùng chuột được
            if (!isLocked)
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
                    frmRemote.changeIndex(map1[selectedPiece.index]);

                    mainPic.Invalidate();

                    // bấm chuột thì gửi sự kiện lock cho các form khác
                    var data = new SelectData(selectedPiece.index);
                    Send(new SendObject(SendObject.LOCK_MAIN, data));

                }
                else if (e.Button == MouseButtons.Right)
                {
                    // khi bấm chuột phải, xoay ảnh, gửi sự kiện xoay, xem Connection/SendObject.cs để rõ hơn
                    Send(new SendObject(SendObject.ROTATE_MAIN, null));

                    rotate();
                }
            }
        }

        private void mainPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isLocked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    isDragging = false;

                    // nhả chuột ra thì phải check xem mảnh đấy đúng vị trí chưa

                    if (selectedPiece != null)
                    {
                        // tự khớp ảnh và check biên
                        fit();
                        clamp();

                        mainPic.Invalidate();

                        // gửi sự kiện mở khóa, vị trí mảnh bị thay đổi
                        var data = new TranslateData(selectedPiece.mainPiece.rect.Left,
                            selectedPiece.mainPiece.rect.Top, selectedPiece.x, selectedPiece.y);
                        Send(new SendObject(SendObject.UNLOCK_MAIN, data));

                        checkPiece();
                    }
                }
            }
        }

        private void mainPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isLocked)
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
                    clamp();

                    // Invalidate để vẽ lại pictureBox
                    mainPic.Invalidate();
                }
            }
        }

        public bool checkWin()
        {
            // tất cả các phần tử của map = true thì win
            for (int i = 0; i < countPieces; i++)
                if (!map[i])
                    return false;
            return true;
        }

        public void win()
        {
            // check cái countPieces = 0 là bởi thằng client win, nó đã gọi hàm này, tuy nhiên, server
            // win lại gửi lại yêu cầu chạy hàm win => gọi 2 lần dẫn đến show MessageBox 2 lần
            // thêm cái check này để tránh gọi 2 lần, dù sao thì nếu win rồi thì countPieces sẽ bằng 0 mà
            if (countPieces != 0)
            {
                countPieces = 0;
                mainPic.Image = image;
                pieces.Clear();
                selectedPiece = null;
                mainPic.Enabled = false;
                frmRemote.remotePic.Enabled = false;

                mainPic.Invalidate();

                MessageBox.Show("Bạn đã thắng!", "Thông báo");
            }
        }

        public void checkPiece()
        {
            int index = selectedPiece.index;
            int x = index % col;
            int y = index / col;

            // nếu hướng của mảnh là 0 đồng thời mảnh đó nằm đúng hàng cột thì mảnh đó Ok
            if (selectedPiece.direction == 0 && selectedPiece.x == x && selectedPiece.y == y)
            {
                map[index] = true;

                // check xong mảnh đó rồi thì check win luôn
                if (checkWin())
                {
                    // gửi sự kiện win cho các máy khác trước
                    Send(new SendObject(SendObject.WIN, null));
                    win();
                }
            }
            else
                map[index] = false;
        }

        private void Main_LocationChanged(object sender, EventArgs e)
        {
            // form Main di chuyển thì form Remote di chuyển theo
            // tuy nhiên phải check xem vị trí nó ko đúng thì mới cần đặt lại
            if (frmRemote.Left != Width + Left + 10 || frmRemote.Top != Top)
                frmRemote.Location = new Point(Width + Left + 10, Top);
        }

        // tắt chương trình thì đóng hết các kết nối
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (server != null)
                server.Close();
            if (client != null)
                client.Close();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (isDragging)
            {
                var data = new TranslateData(selectedPiece.mainPiece.rect.Left,
                            selectedPiece.mainPiece.rect.Top, selectedPiece.x, selectedPiece.y);
                Send(new SendObject(SendObject.TRANSLATE_MAIN, data));
            }
            else
            if (frmRemote.isDragging)
            {
                var data = new TranslateData(frmRemote.selectedPiece.remotePiece.rect.Left,
                            frmRemote.selectedPiece.remotePiece.rect.Top);
                Send(new SendObject(SendObject.TRANSLATE_REMOTE, data));
            }

            if (sendObjects.Count > 0)
            {
                SendObject obj = sendObjects.Dequeue();
                if (isServer)
                    server.Send(obj);
                else
                    client.Send(obj);
            }
        }

        /* Mạng LAN */

        // mỗi form có thể là server hoặc client, chức năng của mình là gì thì làm việc đấy
        public void Send(SendObject obj)
        {
            sendObjects.Enqueue(obj);
        }

        // khi client kết nối đến server, server sẽ gửi lại toàn bộ thông tin các thuộc tính
        // của mình, client sẽ khởi tạo dựa theo các thuộc tính đó, gán lại hết luôn
        public void EventInit(InitData data)
        {
            col = data.col;
            row = data.row;
            WP = data.WP;
            HP = data.HP;
            map = data.map;
            map1 = data.map1;
            map2 = data.map2;
            indexPiece = data.indexPiece;
            pieces = data.pieces;
            selectedPiece = data.selectedPiece;
            image = data.image;
            frmRemote.index = data.remoteIndex;
            frmRemote.pieces = data.pieces;
            frmRemote.selectedPiece = data.selectedPiece;
            countPieces = data.pieces.Count;

            frmHelp.pictureBox.Image = image;

            mainPic.Enabled = true;
            frmRemote.remotePic.Enabled = true;

            Invoke((MethodInvoker)delegate ()
            {
                frmRemote.Show();
            });

            mainPic.Invalidate();
            frmRemote.remotePic.Invalidate();

            cbHelp.Enabled = true;

            numCol.Value = data.col;
            numRow.Value = data.row;

            frmRemote.lblSelected.Text = data.remoteIndex.ToString();
        }

        // khi có ai đó chọn một mảnh ở form remote thì bắn một sự kiện
        // form này nhận được sự kiện cũng thay đổi mảnh được chọn theo
        public void EventSelectRemote(SelectData data)
        {
            if (frmRemote.selectedPiece != null)
                frmRemote.selectedPiece.remotePiece.isHighlight = false;
            foreach (Piece piece in pieces)
            {
                if (piece.index == data.index)
                {
                    frmRemote.selectedPiece = piece;
                    pieces.Remove(piece);
                    pieces.Add(piece);
                    piece.remotePiece.isHighlight = true;
                    frmRemote.remotePic.Invalidate();
                    return;
                }
            }
        }

        // tương tự, khi có mảnh nào đó dịch chuyển thì top, left sẽ thay đổi
        // form này chỉ cần cập nhật top, left của mảnh đang chọn, dù gì thì mảnh
        // đang chọn ở 2 máy luôn giống nhau
        public void EventTranslateRemote(TranslateData data)
        {
            frmRemote.selectedPiece.remotePiece.rect.Location = new Point(data.left, data.top);
            frmRemote.remotePic.Invalidate();
        }

        // xoay mảnh đang chọn
        public void EventRotateRemote()
        {
            frmRemote.rotate();
        }

        // khi có người nháy đúp vào một mảnh thì sẽ bắn sự kiện này
        // máy kia cũng được đưa mảnh được chọn sang form main
        public void EventAppendMain()
        {
            frmRemote.append();
        }

        public void EventSelectMain(SelectData data)
        {
            foreach (Piece piece in pieces)
            {
                if (piece.index == data.index)
                {
                    changePiece(data.index);
                    frmRemote.changeIndex(map1[data.index]);
                    return;
                }
            }
        }

        public void EventTranslateMain(TranslateData data)
        {
            selectedPiece.x = data.x;
            selectedPiece.y = data.y;
            selectedPiece.mainPiece.rect.Location = new Point(data.left, data.top);
            mainPic.Invalidate();
        }

        public void EventRotateMain()
        {
            rotate();
        }

        public void EventWin()
        {
            win();
        }

        // khi có người kéo một mảnh bên form remote, form sẽ bị khóa, ko thể
        // tác động được cho đến khi người đó nhả ra => Unlock
        public void EventLockRemote(SelectData data)
        {
            if (frmRemote.selectedPiece != null)
                frmRemote.selectedPiece.remotePiece.isHighlight = false;
            foreach (Piece piece in pieces)
            {
                if (piece.index == data.index)
                {
                    frmRemote.selectedPiece = piece;
                    pieces.Remove(piece);
                    pieces.Add(piece);
                    piece.remotePiece.isHighlight = true;
                    frmRemote.remotePic.Invalidate();
                    return;
                }
            }

            isLocked = true;
        }

        public void EventUnlockRemote()
        {
            isLocked = false;
        }

        // tương tự, khi form main bị lock thì ko kéo thả đc, đồng thời
        // các control bên remote cũng ko dùng được
        public void EventLockMain(SelectData data)
        {
            foreach (Piece piece in pieces)
            {
                if (piece.index == data.index)
                {
                    changePiece(data.index);
                    frmRemote.changeIndex(map1[data.index]);
                    return;
                }
            }

            isLocked = true;
        }

        public void EventUnlockMain(TranslateData data)
        {
            selectedPiece.x = data.x;
            selectedPiece.y = data.y;
            selectedPiece.mainPiece.rect.Location = new Point(data.left, data.top);
            mainPic.Invalidate();

            isLocked = false;
        }
    }
}
