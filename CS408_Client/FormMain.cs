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
        string inGameWith = "";
        DateTime inviteSentAt; // stores the timestamp of the most recent sent invite
        bool invitationSentReceived;

        public Form RefToFormConnection { get; set; }

        public FormMain()
        {
            InitializeComponent();
            client = FormConnection.client;
            stream = client.GetStream();
            invitationSentReceived = false;

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
                Thread.ResetAbort();
            }

        }

        public void DisplayInfo(string message)
        {
            txtInformation.Invoke((MethodInvoker)delegate
            {
                txtInformation.AppendText(message + "\n");
            });
        }

        private void Listen()
        {
            int acceptValue = 0;
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
                            DisplayInfo("Users have been fetched");
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
                            if (invitationSentReceived)
                            {
                                continue;
                            }
                            invitationSentReceived = true;
                            inGameWith = message;
                            DisplayInfo("Invite received from " + inGameWith);
                            FormInvite form = new FormInvite(message);
                            var result = form.ShowDialog();

                            if (result == DialogResult.OK)
                            {
                                acceptValue = form.accepted;
                                try
                                {
                                    // respond to the invite
                                    byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("r|" + acceptValue + "|" + inGameWith);
                                    Thread.Sleep(20);
                                    if (stream.CanWrite)
                                    {
                                        stream.Write(messageByte, 0, messageByte.Length);
                                    }
                                    else
                                    {
                                        DisplayInfo("Error: cannot write to the network stream!");
                                    }
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
                                DisplayInfo("Invitation from " + inGameWith + "was accepted");
                                // 1 - indicate the start of game to the server
                                try
                                {
                                    byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("a|" + 1);
                                    Thread.Sleep(20);
                                    if (stream.CanWrite)
                                    {
                                        stream.Write(messageByte, 0, messageByte.Length);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Cannot write to the stream!", "v flag error", MessageBoxButtons.OK);
                                    }
                                }
                                catch
                                {
                                    thrListen.Abort();
                                    client.Close(); // disconnect from server
                                    this.RefToFormConnection.Show();
                                    MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
                                    this.Close();
                                }

                                // 2 - Open the game and block this thread until the game is finished
                                FormGame game = new FormGame(inGameWith);
                                var gameResult = game.ShowDialog();
                                game.Close();

                                // 3 - The dialog result will be "cancel" if a communication error was encountered
                                if (gameResult == DialogResult.Cancel)
                                {
                                    thrListen.Abort();
                                    client.Close(); // disconnect from server
                                    this.RefToFormConnection.Show();
                                    MessageBox.Show("Server not available", "Rekt", MessageBoxButtons.OK);
                                    this.Close();
                                }
                                // indicate end of game to the server
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
                        else if (message_flag == "r")
                        {
                            invitationSentReceived = false;
                            if (message == "0")
                            {
                                DisplayInfo("Intivation Declined. Now you can send or receive a new invitation");
                            }
                            else if (message == "1")
                            {
                                // 1 - indicate server the initiation of game
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
                                
                                // 2 - open the game form and block this thread until the game is finished
                                FormGame game = new FormGame(inGameWith);
                                var gameResult = game.ShowDialog();
                                game.Close();

                                // 3 - if the form encountered an error, the dialog result will be
                                // set to DialogResult.Cancel
                                if (gameResult == DialogResult.Cancel)
                                {
                                    // a communication error occured in FormGame - terminate connection
                                    thrListen.Abort();
                                    client.Close(); // disconnect from server
                                    this.RefToFormConnection.Show();
                                    this.Close();
                                }

                                // 4 - indicate the end of game to the server
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
            if (!invitationSentReceived)
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
                            inGameWith = invitePerson;
                            byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("v|" + invitePerson);
                            Thread.Sleep(20);
                            if (stream.CanWrite)
                            {
                                stream.Write(messageByte, 0, messageByte.Length);
                            }
                            else
                            {
                                DisplayInfo("Error: Cannot write to the stream!");
                            }
                            invitationSentReceived = true;
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
                    MessageBox.Show(this, "Server not available - exception catched in btnInviteClick()", "Rekt", MessageBoxButtons.OK);
                    this.Close();
                }
            }
            else
            {
                DisplayInfo("You cannot send an invite at this time");
            }
        }
    }
}
