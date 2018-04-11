using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
namespace client
{
    public partial class client : Form
    {
        Thread clientThread = null;
        Socket clientSocket = null;
        public client()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            receiveTextBox.Clear();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ip=new IPEndPoint(ipaddress,8888);
            try
            {
                clientSocket.Connect(ip);
            }
            catch (Exception)
            {
                receiveTextBox.AppendText("连接失败\n");
            }
            clientThread = new Thread(receiver);
            clientThread.IsBackground = true;
            clientThread.Start();
        }

        private void receiver()
        {
            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024 * 8];
                    int length = clientSocket.Receive(buff);
                    if (length > 0)
                    {
                        String message = Encoding.UTF8.GetString(buff);
                        receiveTextBox.AppendText("server:" + message+"\n");
                    }
                }
                catch(Exception e)
                {
                    receiveTextBox.AppendText("连接中断\n");
                    break;
                }
            }
  
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            String message = sendMessageTextBox.Text+"\n";
            byte[] message1 = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(message1);

        }


        

    }
}
