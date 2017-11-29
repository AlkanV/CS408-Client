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

namespace CS408_Client
{
    public partial class FormConnection : Form
    {
        public static TcpClient client;
        public static string username_me;
        public FormConnection()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // Checks the entered username in the username textbox
            if (txtUserName.Text.Length < 8 || txtUserName.Text.IndexOf('~') >= 0 || txtUserName.Text.IndexOf('$') >= 0) 
            {
                MessageBox.Show("Username should be at least 8 characters long. And should not contain \"~ $\"", "Invalid Username", MessageBoxButtons.OK);
                txtIpAddress.Clear();
                txtPort.Clear();
                txtUserName.Clear();
            }
            else if (txtIpAddress.Text.Length == 0 || txtPort.Text.Length == 0 || txtUserName.Text.Length == 0)
            {
                MessageBox.Show("Please do not leave the fields empty", "Invalid Username", MessageBoxButtons.OK);
                txtIpAddress.Clear();
                txtPort.Clear();
                txtUserName.Clear();
            }
            else
            {
                string IPinput = txtIpAddress.Text;
                int PortInput = Convert.ToInt32(txtPort.Text);
                string usernameInput = txtUserName.Text;

                // 2 - Create the connection
                try
                {
                    client = new TcpClient(IPinput, (int)PortInput);
                    NetworkStream stream = client.GetStream();
                    byte[] connectionData = ASCIIEncoding.ASCII.GetBytes("u|" + usernameInput);

                    // 3 - Send the text
                    stream.Write(connectionData, 0, connectionData.Length);

                    // 4 - Listen for response
                    byte[] response = new byte[2048];
                    int numBytesRead = stream.Read(response, 0, response.Length);
                    string message_flag = "";
                    string response_str = Encoding.Default.GetString(response);
                    response_str = response_str.Substring(0, response_str.IndexOf('\0'));
                    message_flag = response_str.Substring(0, response_str.IndexOf('|'));

                    if (message_flag == "e")
                    {
                        MessageBox.Show("Username already taken", "Invalid Username", MessageBoxButtons.OK);
                        client.Close();
                        txtIpAddress.Clear();
                        txtPort.Clear();
                        txtUserName.Clear();
                    }
                    else
                    {
                        username_me = usernameInput;
                        FormMain fm = new FormMain();
                        fm.RefToFormConnection = this;
                        this.Visible = false;
                        fm.Show();

                    }
                }
                catch (SocketException exc)
                {
                    MessageBox.Show("Cannot connect to specified server", "Invalid IP/Port", MessageBoxButtons.OK);
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            //System.Windows.Forms.Application.Exit();
        }

        private void txtIpAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect.PerformClick();
                // these last two lines will stop the beep sound
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect.PerformClick();
                // these last two lines will stop the beep sound
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }
}
