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
    public class Client
    {
        // ở bên server chỉ 1024 nhưng ở đây thì phải xử lý khác một chút, do ban đầu khởi tạo thì phải
        // gửi một đống hình ảnh, khá nặng nên kích thước mảng byte lớn chút, khởi tạo xong thì lại đưa
        // về 1024 cho nhẹ
        public static int SIZE_BASE = 1024 * 5000;
        public static int SIZE = 1024;

        public Main frmMain;
        public Remote frmRemote;
        public Help frmHelp;

        IPEndPoint IP;
        Socket client;
        int Size;

        public bool isActive;

        public Client(Main main, Remote remote, Help help, string ip)
        {
            frmMain = main;
            frmRemote = remote;
            frmHelp = help;
            isActive = true;
            Size = SIZE_BASE;

            try
            {
                // tạo IP từ ô nhập vào
                IP = new IPEndPoint(IPAddress.Parse(ip), 9999);
            }
            catch
            {
                MessageBox.Show("IP không hợp lệ");
                isActive = false;
                return;
            }

            // xong thì kết nối
            Connect();
        }

        // khởi tạo kết nối đến server
        void Connect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                // kết nối đến IP vừa nhập, cụ thể là IP của server
                // về sau client.Send() sẽ gửi dữ liệu đến server đã kết nối
                client.Connect(IP);
            }
            catch
            {
                isActive = false;
                MessageBox.Show("Không thể kết nối");
                return;
            }

            // lắng nghe server gửi dữ liệu đến, ở đây chỉ giao tiếp với server nên chỉ cần một luồng
            // ko như server giao tiếp với nhiều client
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        // đóng kết nối
        public void Close()
        {
            isActive = false;
            client.Close();
        }

        // gửi dữ liệu, chỉ cần gửi cho một thằng, chính là server, ở đây client
        // chính là thằng server đã kết nối đến
        public void Send(SendObject obj)
        {
            if (client != null)
            {
                client.Send(Serialize(obj));
            }
        }

        // hàm lắng nghe nhận dữ liệu, giống với bên server, cũng nhận, giải mã, xử lý, lỗi thì đóng kết nối
        void Receive()
        {
            try
            {
                while (isActive)
                {
                    byte[] data = new byte[Size];
                    Size = SIZE;
                    client.Receive(data);

                    SendObject o = Deserialize(data);
                    Process(o);
                }
            }
            catch
            {
                MessageBox.Show("Mất kết nối đến máy chủ");
                Close();
            }
        }

        byte[] Serialize(SendObject obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        SendObject Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return (SendObject)formatter.Deserialize(stream);
        }

        void Process(SendObject obj)
        {
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
