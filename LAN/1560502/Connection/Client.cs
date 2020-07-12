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
        public Main frmMain;
        public Remote frmRemote;
        public Help frmHelp;

        IPEndPoint IP;
        Socket client;
        public bool isActive;

        public Client(Main main, Remote remote, Help help, string ip)
        {
            frmMain = main;
            frmRemote = remote;
            frmHelp = help;
            isActive = false;

            try
            {
                IP = new IPEndPoint(IPAddress.Parse(ip), 9999);
                Connect();
            }
            catch { }
        }

        void Connect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Không thể kết nối");
                return;
            }

            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
            isActive = true;
        }

        public void Close()
        {
            isActive = false;
            client.Close();
        }

        public void Send(SendObject obj)
        {
            if (client != null)
            {
                client.Send(Serialize(obj));
            }
        }

        void Receive()
        {
            try
            {
                while (isActive)
                {
                    byte[] data = new byte[2048000];
                    client.Receive(data);

                    SendObject o = Deserialize(data);
                    Process(o);
                }
            }
            catch
            {
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
            }
        }

    }
}
