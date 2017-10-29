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
        public FormConnection()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {


            // 1 - Input parameters
            string IPinput = txtIpAddress.Text;
            int PortInput = Convert.ToInt32(txtPort.Text);
            string usernameInput = txtUserName.Text;

            // kChecks the entered username in the username textbox
            if (txtUserName.Text.Length < 8 || txtUserName.Text.IndexOf('~') >= 0 || txtUserName.Text.IndexOf('$') >= 0) 
            {
                MessageBox.Show("Username should be at least 8 characters long. And should not contain \"~ $\"", "Invalid Username", MessageBoxButtons.OK);
            }

            // 2 - Create the connection
            TcpClient client = new TcpClient(IPinput, PortInput);
            NetworkStream stream = client.GetStream();
            byte[] connectionData = ASCIIEncoding.ASCII.GetBytes("u|" + usernameInput);

            // 3 - Send the text
            stream.Write(connectionData, 0, connectionData.Length);

            FormMain fm = new FormMain();
            fm.Show();
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
