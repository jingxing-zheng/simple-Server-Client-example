using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System.Threading;

namespace Server
{
    public partial class Server : Form
    {

        static Socket serverSocket = null;
        static Dictionary<String, Socket> clientItem = new Dictionary<String, Socket>();
        public Server()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            serverSocket.Bind(new IPEndPoint(ipaddress, 8888));
            serverSocket.Listen(10);
            Thread thread = new Thread(ListenClientConnect);
            thread.IsBackground = true;
            thread.Start();
            receiveTextBox.AppendText("服务器启动,等待连接...\n");
        }
        private void ListenClientConnect()
        {
            Socket clientSocket = null;
            while (true)
            {
                try
                {
                    clientSocket = serverSocket.Accept();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    break;
                }
                IPAddress clientIP = (clientSocket.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (clientSocket.RemoteEndPoint as IPEndPoint).Port;
                String client = "IP:" + clientIP + " Port:" + clientPort;
                String message = client + "连接服务器成功\n";
                byte[] message1 = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(message1);
                receiveTextBox.AppendText("与客户端" + client + "连接成功\n");
                clientItem.Add(client, clientSocket);
                clientListBox.Items.Add(client);
                ParameterizedThreadStart p = new ParameterizedThreadStart(receiver);
                Thread thread = new Thread(p);
                thread.IsBackground = true;
                thread.Start(clientSocket);
            }
        }

        private void receiver(Object o)
        {
            Socket socket = o as Socket;
            while (true)
            {
                byte[] buff = new byte[1024 * 8];
                try
                {
                    int length = socket.Receive(buff);
                    if (length > 0)
                    {
                        String message = Encoding.UTF8.GetString(buff, 0, length);
                        String client = socket.RemoteEndPoint + ":";
                        receiveTextBox.AppendText(client + "\n" + message+"\n");
                    }
                }
                catch (Exception e)
                {
                    receiveTextBox.AppendText(socket.RemoteEndPoint + "中断\n" );
                    IPAddress clientIP = (socket.RemoteEndPoint as IPEndPoint).Address;
                    int clientPort = (socket.RemoteEndPoint as IPEndPoint).Port;
                    String client = "IP:" + clientIP + " Port:" + clientPort;
                    for (int i = 0; i < clientListBox.Items.Count; i++)
                    {
                        if (client.Equals(clientListBox.Items[i]))
                        {
                            clientListBox.Items.RemoveAt(i);
                        }
                    }
                        clientItem.Remove(client);
                    break;
                }
            }
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            String message = sendMessageTextBox.Text+"\n";
            byte[] message1 = Encoding.UTF8.GetBytes(message);
            if (!String.IsNullOrEmpty(clientListBox.Text.Trim()))
            {
                for (int i = 0; i < clientListBox.SelectedItems.Count; i++)
                {
                    clientItem[clientListBox.SelectedItems[i].ToString()].Send(message1);
                }
            }
            sendMessageTextBox.Clear();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            receiveTextBox.Clear();
        }


    }
}
