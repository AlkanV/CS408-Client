using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS408_Client
{
    public partial class FormMain : Form
    {
        TcpClient client;
        NetworkStream stream;
        Thread thrListen;
        public FormMain()
        {
            InitializeComponent();
            client = FormConnection.client;
            stream = client.GetStream();

            // Create a thread to listen for incoming messages
            thrListen = new Thread(new ThreadStart(Listen));
            thrListen.IsBackground = true;
            thrListen.Start();
        }

        private void btnGetUsers_Click(object sender, EventArgs e)
        {
            byte[] requestByte = ASCIIEncoding.ASCII.GetBytes("g|");
            stream.Write(requestByte, 0, requestByte.Length);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            txtInformation.AppendText("\nTerminating connections");
            thrListen.Abort();
            client.Close(); // disconnect from server
            this.Hide();
            var formConnection = new FormConnection();
            formConnection.Closed += (s, args) => this.Close();
            formConnection.Show();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string Message = txtMessage.Text;
            byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("m|" + Message);
            stream.Write(messageByte, 0, messageByte.Length);
        }

        private void Listen()
        {
            while (true)
            {
                byte[] buffer = new byte[2048];
                string message_flag = "", message = "";
                if (stream.DataAvailable)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    string[] message_content = Encoding.Default.GetString(buffer).Split('|');
                    message_flag = message_content[0];
                    message = message_content[1];
                    Array.Clear(buffer, 0, buffer.Length);
                }
                if (message_flag == "i")
                {
                    txtInformation.Invoke((MethodInvoker)delegate
                    {
                        txtInformation.AppendText("\n" + message);
                    });
                }
            }
        }
    }
}
