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
    public partial class FormGame : Form
    {
        TcpClient client;
        NetworkStream stream;
        Thread thrListen1;
        private bool gameTerminating;

        public int surrendered { get; set; }
        public Form RefToFormConnection { get; set; }
        public FormGame()
        {
            InitializeComponent();
            client = FormConnection.client;
            stream = client.GetStream();

            surrendered = 0;

            thrListen1 = new Thread(new ThreadStart(Listen));
            thrListen1.IsBackground = true;
            gameTerminating = false;
            thrListen1.Start();

            this.Text = "client [" + FormConnection.username_me + "]";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            surrendered = 1;
            this.Close();
        }
        private void Listen()
        {
            while (!gameTerminating)
            {
                try
                {
                    byte[] buffer = new byte[2048];
                    string message_flag = "", message = "";
                    if (stream.DataAvailable)
                    {
                        stream.Read(buffer, 0, buffer.Length);
                        string[] message_content = Encoding.Default.GetString(buffer).Split('|');
                        message_flag = message_content[0];
                        message = message_content[1];
                        message = message.Substring(0, message.IndexOf('\0'));

                        if (message_flag == "s" && message == "1")
                        {
                            MessageBox.Show("You Won!", "Wow...", MessageBoxButtons.OK);
                            byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("a|" + 0);
                            Thread.Sleep(20);
                            if (stream.CanWrite)
                            {
                                stream.Write(messageByte, 0, messageByte.Length);
                            }
                            else
                            {
                                MessageBox.Show("Cannot write to the stream!", "FormGame Error", MessageBoxButtons.OK);
                            }
                            gameTerminating = true;
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
                catch
                {
                    MessageBox.Show("Server got disconnected during the game", "Rekt", MessageBoxButtons.OK);
                    this.Close();
                    Thread.ResetAbort();
                }
            }
            this.Invoke((MethodInvoker)delegate
            {
                // close the form on the forms thread
                this.Close();
            });
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            DialogResult = DialogResult.OK; // indicate that the game form was terminated
        }

    }
}