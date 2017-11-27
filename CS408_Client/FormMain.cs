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

        public Form RefToFormConnection { get; set; }

        public FormMain()
        {
            InitializeComponent();
            client = FormConnection.client;
            stream = client.GetStream();

            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }

            // Create a thread to listen for incoming messages
            thrListen = new Thread(new ThreadStart(Listen));
            thrListen.IsBackground = true;
            thrListen.Start();
        }

        private void btnGetUsers_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] requestByte = ASCIIEncoding.ASCII.GetBytes("g|");
                stream.Write(requestByte, 0, requestByte.Length);
                listUsers.Items.Clear();
            }
            catch
            {
                thrListen.Abort();
                client.Close(); // disconnect from server
                this.RefToFormConnection.Show();
                MessageBox.Show(this, "Server not available", "Rekt", MessageBoxButtons.OK);
                this.Close();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            txtInformation.AppendText("\nTerminating connections");
            thrListen.Abort();
            client.Close(); // disconnect from server
            this.RefToFormConnection.Show();
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string message = txtMessage.Text;
                byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("m|" + message);
                stream.Write(messageByte, 0, messageByte.Length);
            }
            catch
            {
                thrListen.Abort();
                client.Close(); // disconnect from server
                this.RefToFormConnection.Show();
                MessageBox.Show(this, "Server not available", "Rekt", MessageBoxButtons.OK);
                this.Close();
            }
            
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
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                    if (message_flag == "i")
                    {
                        txtInformation.Invoke((MethodInvoker)delegate
                        {
                            txtInformation.AppendText("\n" + message);
                        });
                    }
                    else if (message_flag == "g")
                    {
                        listUsers.Invoke((MethodInvoker)delegate
                        {
                            listUsers.Items.Add(message);
                        });
                    }
                    else if (message_flag == "m")
                    {
                        txtInformation.Invoke((MethodInvoker)delegate
                        {
                            txtInformation.AppendText("\n" + message);
                        });
                        txtMessage.Invoke((MethodInvoker)delegate
                        {
                            txtMessage.Clear();
                        });
                    }
                    else if (message_flag == "i")
                    {
                        txtInformation.Invoke((MethodInvoker)delegate
                        {
                            txtInformation.AppendText("\n" + message + " an invitation has geldi kapiya dayandi artik mubarek.");
                        });

                        using (var form = new FormInvite(message))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                bool acceptValue= form.accepted;
                            }
                        }
                    }
                }
                catch
                {
                    thrListen.Abort();
                    client.Close(); // disconnect from server
                    this.RefToFormConnection.Show();
                    MessageBox.Show(this, "Server not available", "Closing", MessageBoxButtons.OK);
                    this.Close();
                }
            }
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
                // these last two lines will stop the beep sound
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            switch (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo))
            {
                case DialogResult.No:
                    e.Cancel = true;
                    break;
                default:
                    thrListen.Abort();
                    client.Close(); // disconnect from server
                    this.RefToFormConnection.Show();
                    break;
            }
        }

        private void btnInvite_Click(object sender, EventArgs e)
        {
            try
            {
                string invitePerson = listUsers.SelectedItem.ToString();
                byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("v|" + invitePerson);
                stream.Write(messageByte, 0, messageByte.Length);
            }
            catch
            {
                thrListen.Abort();
                client.Close(); // disconnect from server
                this.RefToFormConnection.Show();
                MessageBox.Show(this, "Server not available", "Rekt", MessageBoxButtons.OK);
                this.Close();
            }
        }
    }
}