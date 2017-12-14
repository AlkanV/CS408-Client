﻿using System;
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
    public delegate void CloseDelegate();
    public partial class FormGame : Form
    {
        TcpClient client;
        NetworkStream stream;
        Thread thrListen1;
        private bool gameTerminating;
        string inGameWith;

        public Form RefToFormConnection { get; set; }
        public FormGame(string opponentUsername)
        {
            InitializeComponent();
            client = FormConnection.client;
            stream = client.GetStream();
            inGameWith = opponentUsername;

            thrListen1 = new Thread(new ThreadStart(Listen));
            thrListen1.IsBackground = true;
            gameTerminating = false;
            thrListen1.Start();

            txtGuessedNumber.ReadOnly = true; // wait for the server to send "x" flag

            this.Text = "client [" + FormConnection.username_me + "]";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("s|" + 1 + "|" + inGameWith);
                Thread.Sleep(20);
                if (stream.CanWrite)
                {
                    stream.Write(messageByte, 0, messageByte.Length);
                }
            }
            catch
            {
                MessageBox.Show("Error occured - couldn't sent s|1|" + inGameWith);
                DialogResult = DialogResult.Cancel;
                this.Close();
            }

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
                        else if (message_flag == "x")
                        {
                            // game can start
                            txtGuessedNumber.Invoke((MethodInvoker)delegate
                            {
                                txtGuessedNumber.ReadOnly = false;
                            });
                        }
                        else if (message_flag == "f")
                        {
                            // two parties have sent their guesses - game ended
                            if (message == "0")
                            {
                                MessageBox.Show("You Won!", "Wow...", MessageBoxButtons.OK);
                            }
                            else if (message == "1")
                            {
                                MessageBox.Show("You Lost", ":(", MessageBoxButtons.OK);
                            }
                            else
                            {
                                MessageBox.Show("Tie", "Wow...", MessageBoxButtons.OK);
                            }
                            gameTerminating = true;
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
                catch
                {
                    MessageBox.Show("Server got disconnected during the game", "Rekt", MessageBoxButtons.OK);

                    this.Invoke(new CloseDelegate(this.Close));
                }
            }
            this.Invoke(new CloseDelegate(this.Close));
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            DialogResult = DialogResult.OK; // indicate that the game form was terminated
        }

        private void btnGuess_Click(object sender, EventArgs e)
        {
            int guessedNumber;
            string guessedNumber_str = txtGuessedNumber.Text;
            if (!Int32.TryParse(guessedNumber_str, out guessedNumber))
            {
                MessageBox.Show("Please enter an integer between 1 and 100");
                return;
            }
           

            byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("e|" + guessedNumber);
            Thread.Sleep(20);
            if (stream.CanWrite)
            {
                stream.Write(messageByte, 0, messageByte.Length);
            }
            else
            {
                MessageBox.Show("Cannot write to the stream!", "FormGame Error", MessageBoxButtons.OK);
            }
            txtGuessedNumber.ReadOnly = true;
        }
    }
}