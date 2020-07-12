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
        public Main frmMain;
        public Remote frmRemote;
        public Help frmHelp;

        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;
        public bool isActive;

        public Server(Main main, Remote remote, Help help)
        {
            frmMain = main;
            frmRemote = remote;
            frmHelp = help;
            isActive = false;

            Connect();
        }

        void Connect()
        {
            try
            {
                clientList = new List<Socket>();
                IP = new IPEndPoint(IPAddress.Any, 9999);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                server.Bind(IP);
                Thread Listen = new Thread(() =>
                {
                    try
                    {
                        while (isActive)
                        {
                            server.Listen(100);
                            Socket client = server.Accept();
                            clientList.Add(client);

                            var data = new InitData(frmMain.row, frmMain.col, frmMain.WP, frmMain.HP,
                                frmMain.map, frmMain.map1, frmMain.map2, frmMain.indexPiece, frmMain.pieces,
                                frmMain.selectedPiece, frmMain.image, frmRemote.index, frmRemote.selectedPiece);
                            client.Send(Serialize(new SendObject(SendObject.INIT, data)));
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
                isActive = true;
            }
            catch
            {
                MessageBox.Show("Không thể tạo");
            }
        }

        public void Close()
        {
            isActive = false;
            server.Close();
        }

        public void Send(SendObject obj)
        {
            foreach (Socket item in clientList)
            {
                if (item != null)
                {
                    item.Send(Serialize(obj));
                }
            }
        }

        void Receive(Object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[2048000];
                    client.Receive(data);

                    SendObject o = Deserialize(data);

                    Process(o);

                    foreach (Socket item in clientList)
                    {
                        if (item != null && item != client)
                        {
                            item.Send(Serialize(o));
                        }
                    }
                }
            }
            catch
            {
                clientList.Remove(client);
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
