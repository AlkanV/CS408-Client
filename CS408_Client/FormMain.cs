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
        string storedName = "";
        DateTime inviteSentAt; // stores the timestamp of the most recent sent invite

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

            this.Text = "client [" + FormConnection.username_me + "]";
        }

        private void btnGetUsers_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] requestByte = ASCIIEncoding.ASCII.GetBytes("g|");
                Thread.Sleep(20);
                stream.Write(requestByte, 0, requestByte.Length);
                DisplayInfo("sending g| flag");
                listUsers.Items.Clear();
            }
            catch
            {
                thrListen.Abort();
                client.Close(); // disconnect from server
                this.RefToFormConnection.Show();
                MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
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
                Thread.Sleep(20);
                stream.Write(messageByte, 0, messageByte.Length);
            }
            catch
            {
                thrListen.Abort();
                client.Close(); // disconnect from server
                this.RefToFormConnection.Show();
                MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
                this.Close();
            }

        }

        private void DisplayInfo(string message)
        {
            txtInformation.Invoke((MethodInvoker)delegate
            {
                txtInformation.AppendText(message + "\n");
            });
        }

        private void Listen()
        {
            int acceptValue = 0, surrenderValue = 0;
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
                        if (message_flag == "v")
                        {
                            storedName = message;
                            DisplayInfo("in game with " + storedName);
                        }
                        DisplayInfo("READ: " + message_content[0] + "|" + message_content[1]);
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                    if (message_flag == "i")
                    {
                        DisplayInfo(message);
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
                        DisplayInfo(message);
                        txtMessage.Invoke((MethodInvoker)delegate
                        {
                            txtMessage.Clear();
                        });
                    }
                    else if (message_flag == "v")
                    {
                        DisplayInfo("An invitation has geldi kapiya dayandi artik mubarek from" + message);

                        FormInvite form = new FormInvite(message);
                        var result = form.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            acceptValue = form.accepted;
                            try
                            {
                                byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("r|" + acceptValue + "|" + storedName);
                                Thread.Sleep(20);
                                stream.Write(messageByte, 0, messageByte.Length);
                            }
                            catch
                            {
                                thrListen.Abort();
                                client.Close(); // disconnect from server
                                this.RefToFormConnection.Show();
                                MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
                                this.Close();
                            }
                        }
                        if (acceptValue == 1)
                        {
                            FormGame game = new FormGame();
                            var gameResult = game.ShowDialog();
                            try
                            {
                                byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("a|" + 1);
                                Thread.Sleep(20);
                                stream.Write(messageByte, 0, messageByte.Length);
                            }
                            catch
                            {
                                thrListen.Abort();
                                client.Close(); // disconnect from server
                                this.RefToFormConnection.Show();
                                MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
                                this.Close();
                            }
                            if (gameResult == DialogResult.OK)
                            {
                                game.Close();
                                surrenderValue = game.surrendered;
                                try
                                {
                                    byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("s|" + surrenderValue + "|" + storedName);
                                    Thread.Sleep(20);
                                    stream.Write(messageByte, 0, messageByte.Length);
                                }
                                catch
                                {
                                    thrListen.Abort();
                                    client.Close(); // disconnect from server
                                    this.RefToFormConnection.Show();
                                    MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
                                    this.Close();
                                }
                                if (surrenderValue == 1)
                                {
                                    MessageBox.Show("You lost!", "Rekt", MessageBoxButtons.OK);
                                    try
                                    {
                                        byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("a|" + 0);
                                        Thread.Sleep(20);
                                        stream.Write(messageByte, 0, messageByte.Length);
                                    }
                                    catch
                                    {
                                        thrListen.Abort();
                                        client.Close(); // disconnect from server
                                        this.RefToFormConnection.Show();
                                        MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
                                        this.Close();
                                    }
                                }
                            }
                        }
                    }

                    else if (message_flag == "r")
                    {
                        if (message == "0")
                        {
                            DisplayInfo("Intivation Declined. Now you can send or receive a new invitation");
                        }
                        else if (message == "1")
                        {
                            FormGame game = new FormGame();
                            var gameResult = game.ShowDialog();
                            try
                            {
                                byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("a|" + 1); // indicate start of gameplay to server
                                Thread.Sleep(20);
                                stream.Write(messageByte, 0, messageByte.Length);
                            }
                            catch
                            {
                                thrListen.Abort();
                                client.Close(); // disconnect from server
                                this.RefToFormConnection.Show();
                                MessageBox.Show("Server not available - cannot send a|1", "Rekt", MessageBoxButtons.OK);
                                this.Close();
                            }
                            if (gameResult == DialogResult.OK)
                            {
                                game.Close();
                                surrenderValue = game.surrendered;
                                try
                                {
                                    byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("s|" + surrenderValue + "|" + storedName);
                                    Thread.Sleep(20);
                                    stream.Write(messageByte, 0, messageByte.Length);
                                }
                                catch
                                {
                                    thrListen.Abort();
                                    client.Close(); // disconnect from server
                                    this.RefToFormConnection.Show();
                                    MessageBox.Show("Server not available - cannot send s|" + surrenderValue + "|" + storedName, "Rekt", MessageBoxButtons.OK);
                                    this.Close();
                                }
                                if (surrenderValue == 1)
                                {
                                    MessageBox.Show("You lost!", "Rekt", MessageBoxButtons.OK);
                                    try
                                    {
                                        byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("a|" + 0);
                                        Thread.Sleep(20);
                                        stream.Write(messageByte, 0, messageByte.Length);
                                    }
                                    catch
                                    {
                                        thrListen.Abort();
                                        client.Close(); // disconnect from server
                                        this.RefToFormConnection.Show();
                                        MessageBox.Show("Server not available - cannot send a|0", "Rekt", MessageBoxButtons.OK);
                                        this.Close();
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    thrListen.Abort();
                    client.Close(); // disconnect from server
                    this.RefToFormConnection.Show();
                    MessageBox.Show("Server not available", "Closing", MessageBoxButtons.OK);
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
                    MessageBox.Show(this, "Bastin bi kere artik carpiya...", "cok gec", MessageBoxButtons.OK);
                    thrListen.Abort();
                    client.Close(); // disconnect from server
                    this.RefToFormConnection.Show();
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
                const int timeoutSeconds = 15;
                if ((DateTime.Now - inviteSentAt).TotalSeconds < timeoutSeconds)
                {
                    DisplayInfo("You have to wait for " + (timeoutSeconds - (DateTime.Now - inviteSentAt).TotalSeconds)
                        + " send an invite again");
                }
                else
                {
                    string invitePerson = listUsers.SelectedItem.ToString();
                    if (invitePerson != FormConnection.username_me)
                    {
                        byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("v|" + invitePerson);
                        Thread.Sleep(20);
                        stream.Write(messageByte, 0, messageByte.Length);
                        inviteSentAt = DateTime.Now;
                    }
                    else
                    {
                        MessageBox.Show(this, "You can not invite yourself", "Forever alone", MessageBoxButtons.OK);
                    }
                }

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
