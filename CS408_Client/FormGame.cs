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
        Thread thrListen;

        public int surrendered { get; set; }
        public Form RefToFormConnection { get; set; }
        public FormGame()
        {
            InitializeComponent();
            client = FormConnection.client;
            stream = client.GetStream();

            surrendered = 0;

            thrListen = new Thread(new ThreadStart(Listen));
            thrListen.IsBackground = true;
            thrListen.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thrListen.Abort();
            surrendered = 1;
            DialogResult = DialogResult.OK;
            this.Close();
        }
        private void Listen()
        {
            while (true)
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
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                    if (message_flag == "s" && message == "1")
                    {

                        MessageBox.Show(this, "You Won!", "Wow...", MessageBoxButtons.OK);
                        thrListen.Abort();
                        DialogResult = DialogResult.OK;
                        this.Close();

                    }
                }
                catch
                {
                    thrListen.Abort();
                    MessageBox.Show(this, "Server got disconnected during the game", "Rekt", MessageBoxButtons.OK);
                    this.Close();
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            switch (MessageBox.Show(this, "You will lose the game, are you sure you want to exit?", "Closing", MessageBoxButtons.YesNo))
            {
                case DialogResult.No:
                    e.Cancel = true;
                    break;
                default:
                    thrListen.Abort();
                    break;
            }
        }

    }
}