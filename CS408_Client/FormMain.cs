using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS408_Client
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnGetUsers_Click(object sender, EventArgs e)
        {

        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            txtInformation.AppendText("\nTerminating connections");

            this.Hide();
            var formConnection = new FormConnection();
            formConnection.Closed += (s, args) => this.Close();
            formConnection.Show();

        }
    }
}
