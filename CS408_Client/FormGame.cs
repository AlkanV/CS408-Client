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
    public delegate void CloseDelegate();
    public partial class FormGame : Form
    {
        int score;
        TcpClient client;
        NetworkStream stream;
        Thread thrListen1;
        private bool gameTerminating, inRound = true;
        string inGameWith;

        public Form RefToFormConnection { get; set; }
        public FormGame(string opponentUsername)
        {
            score = 0;
            InitializeComponent();
            client = FormConnection.client;
            stream = client.GetStream();
            inGameWith = opponentUsername;

            gameTerminating = false;
            //txtGuessedNumber.ReadOnly = true; // wait for the server to send "x" flag

            this.Text = "client [" + FormConnection.username_me + "]";
            thrListen1 = new Thread(new ThreadStart(Listen));
            thrListen1.IsBackground = true;
            thrListen1.Start();
        }

        /*
        private void eventLoop()
        {

            thrListen1 = new Thread(new ThreadStart(Listen));
            thrListen1.IsBackground = true;
            thrListen1.Start();
            bool isalive = thrListen.IsAlive;
            while (isalive)
            {
                isalive = thrListen.IsAlive;
            }
            this.Close();
        }
        */

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
                            gameTerminating = true;
                            DialogResult = DialogResult.OK;
                        }
                        else if (message_flag == "x")
                        {
                            // game can start
                            /*
                            txtGuessedNumber.Invoke((MethodInvoker)delegate
                            {
                                txtGuessedNumber.ReadOnly = false;
                            });*/
                        }
                        else if (message_flag == "f")
                        {
                            // two parties have sent their guesses - game ended
                            if (message == "0")
                            {
                                MessageBox.Show("You Won the round!", "Wow...", MessageBoxButtons.OK);
                                score++;
                                lblScore.Text = "Score: " + score;
                            }
                            else if (message == "1")
                            {
                                MessageBox.Show("You Lost the round", ":(", MessageBoxButtons.OK);
                            }
                            else
                            {
                                MessageBox.Show("Tie", "Wow...", MessageBoxButtons.OK);
                            }
                            inRound = true;
                        }
                        else if (message_flag == "w") //w diye yeni bi flag yarat, w0 = oyun sonlanmadi, w1 = oyun sonlandi
                        {
                            if (message[0] == '1' && score != 2)
                            {
                                MessageBox.Show("You Lost the game", ":(", MessageBoxButtons.OK);
                                DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            if (message[0] == '1' && score == 2)
                            {
                                MessageBox.Show("You Won the game!", "Wow...", MessageBoxButtons.OK);
                                DialogResult = DialogResult.OK;
                                gameTerminating = true;
                            }
                        }
                        else if (message_flag == "j") //j for disconnected opponent
                        {
                            MessageBox.Show("You Won the game!", "Wow...", MessageBoxButtons.OK);
                            DialogResult = DialogResult.OK;
                            gameTerminating = true;
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
                catch
                {
                    //MessageBox.Show("Exception encountered in FormGame::Listen", "Rekt", MessageBoxButtons.OK);
                    gameTerminating = true;
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
        }

        private void btnGuess_Click(object sender, EventArgs e)
        {
            int guessedNumber;
            string guessedNumber_str = txtGuessedNumber.Text;
            if (!Int32.TryParse(guessedNumber_str, out guessedNumber))
            {
                MessageBox.Show("Please enter an integer between 1 and 100", "Are you dumb?");
                return;
            }

            if (inRound)
            {
                byte[] messageByte = ASCIIEncoding.ASCII.GetBytes("e|" + guessedNumber);
                Thread.Sleep(20);
                if (stream.CanWrite)
                {
                    stream.Write(messageByte, 0, messageByte.Length);
                }
                else
                {
                    MessageBox.Show("Cannot write to the stream!", "FormGame Error", MessageBoxButtons.OK);
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            inRound = false;
        }
    }
}