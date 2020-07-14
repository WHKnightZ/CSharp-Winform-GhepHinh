using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace GhepHinh
{
    public class Server
    {
        // ở đây khi gửi dữ liệu sẽ phải gửi bằng một mảng byte, khi nhận cũng vậy
        // tuy nhiên phải khởi tạo một mảng byte trước mà ko biết kích thước bao nhiêu
        // mình cho luôn bằng 1024, vì data gửi đến server khá nhẹ
        public static int SIZE = 1024;

        public Main frmMain;
        public Remote frmRemote;
        public Help frmHelp;

        // IP của máy chủ
        IPEndPoint IP;
        // server
        Socket server;
        // và một danh sách các client kết nối đến (1 server - nhiều client)
        List<Socket> clientList;

        // biến này để check server khởi tạo thành công ko, nếu ko thì isActive = false và form Main ko làm gì hết
        public bool isActive;

        public Server(Main main, Remote remote, Help help)
        {
            frmMain = main;
            frmRemote = remote;
            frmHelp = help;
            isActive = true;

            Connect();
        }

        // hàm tạo kết nối
        void Connect()
        {
            try
            {
                // khởi tạo một danh sách client
                clientList = new List<Socket>();

                // mấy bước khởi tạo server, IPAddress.Any nghĩa là client IP nào cũng kết nối được
                // Port = 9999 thì chọn bừa cũng được, miễn là giống với Port bên client, kiểu
                // 2 thằng phải vào cùng một chỗ mới gặp nhau được
                IP = new IPEndPoint(IPAddress.Any, 9999);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                server.Bind(IP);

                // khởi tạo xong thì tạo một luồng chạy ngầm liên tục lắng nghe mấy thằng kết nối đến
                Thread Listen = new Thread(() =>
                {
                    try
                    {
                        while (isActive)
                        {
                            // ở đây, server sẽ lắng nghe, đợi client kết nối đến
                            // các lệnh bên dưới sẽ ko chạy cho đến khi nào có một client kết nối đến
                            // nếu một client kết nối đến, các lệnh khởi tạo bên dưới sẽ được chạy và
                            // vòng lặp lại quay về đây, server lại lắng nghe tiếp client khác
                            server.Listen(100);
                            
                            // client kết nối thì chấp nhận và lưu client đó lại, thêm vào danh sách các client
                            Socket client = server.Accept();
                            clientList.Add(client);

                            // đồng thời gửi sự kiện khởi tạo cho client đó để client đó sao chép dữ liệu cho giống với server
                            var data = new InitData(frmMain.row, frmMain.col, frmMain.WP, frmMain.HP,
                                frmMain.map, frmMain.map1, frmMain.map2, frmMain.indexPiece, frmMain.pieces,
                                frmMain.selectedPiece, frmMain.image, frmRemote.index, frmRemote.selectedPiece);
                            client.Send(Serialize(new SendObject(SendObject.INIT, data)));

                            // đồng thời tạo thêm một luồng để nhận dữ liệu từ client này
                            // có bao nhiêu client thì phải tạo thêm bấy nhiêu luồng
                            Thread receive = new Thread(Receive);
                            receive.IsBackground = true;
                            receive.Start(client);
                        }
                    }
                    catch
                    {
                        IP = new IPEndPoint(IPAddress.Any, 9999);
                        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    }
                });

                Listen.IsBackground = true;
                Listen.Start();
            }
            catch
            {
                isActive = false;
                MessageBox.Show("Không thể tạo");
            }
        }

        // hàm đóng kết nối
        public void Close()
        {
            isActive = false;
            server.Close();
        }

        // hàm gửi sự kiện
        public void Send(SendObject obj)
        {
            // khi gửi thì phải gửi cho tất cả các client
            foreach (Socket item in clientList)
            {
                if (item != null)
                {
                    // mã hóa xong mới được gửi
                    item.Send(Serialize(obj));
                }
            }
        }

        // hàm nhận sự kiện
        void Receive(Object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (isActive)
                {
                    // khởi tạo mảng data, đồng thời hàm Receive là lắng nghe client đó bắn dữ liệu
                    // nếu client đó bắn dữ liệu, server nhận được thì sẽ đưa dữ liệu đó vào mảng data, chương trình
                    // sẽ chạy tiếp xuống lệnh bên dưới, chạy xong lại quay lại một vòng lặp, lắng nghe tiếp
                    byte[] data = new byte[SIZE];
                    client.Receive(data);

                    // giải mã mảng data vừa nhận để đưa nó về SendObject
                    SendObject o = Deserialize(data);

                    // và xử lý nó
                    Process(o);

                    foreach (Socket item in clientList)
                    {
                        if (item != null && item != client)
                        {
                            // đồng thời server phải gửi dữ liệu cho tất cả các client mà ko phải thằng client đã gửi
                            // để tất cả các máy đồng bộ
                            item.Send(Serialize(o));
                        }
                    }
                }
            }
            catch
            {
                // client lỗi thì xóa nó luôn
                clientList.Remove(client);
                client.Close();
            }
        }

        // hàm mã hóa biến SendObject => mảng byte
        byte[] Serialize(SendObject obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        // hàm giải mã biến mảng byte => SendObject
        SendObject Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return (SendObject)formatter.Deserialize(stream);
        }

        // hàm xử lý các sự kiện nhận được, swich case luôn cho đơn giản
        void Process(SendObject obj)
        {
            // ngoài switch case có thể tối ưu hóa bằng delegate (một mảng các hàm)
            // delegate [] func, func[0] = frmMain.EventInit, func[1] = frmMain.EventSelectRemote ...
            switch (obj.type)
            {
                case SendObject.INIT: frmMain.EventInit((InitData)obj.data); break;
                case SendObject.SELECT_REMOTE: frmMain.EventSelectRemote((SelectData)obj.data); break;
                case SendObject.TRANSLATE_REMOTE: frmMain.EventTranslateRemote((TranslateData)obj.data); break;
                case SendObject.ROTATE_REMOTE: frmMain.EventRotateRemote(); break;
                case SendObject.APPEND_MAIN: frmMain.EventAppendMain(); break;
                case SendObject.SELECT_MAIN: frmMain.EventSelectMain((SelectData)obj.data); break;
                case SendObject.TRANSLATE_MAIN: frmMain.EventTranslateMain((TranslateData)obj.data); break;
                case SendObject.ROTATE_MAIN: frmMain.EventRotateMain(); break;
                case SendObject.WIN: frmMain.EventWin(); break;
                case SendObject.LOCK_REMOTE: frmMain.EventLockRemote((SelectData)obj.data); break;
                case SendObject.UNLOCK_REMOTE: frmMain.EventUnlockRemote(); break;
                case SendObject.LOCK_MAIN: frmMain.EventLockMain((SelectData)obj.data); break;
                case SendObject.UNLOCK_MAIN: frmMain.EventUnlockMain((TranslateData)obj.data); break;
            }
        }
    }
}
