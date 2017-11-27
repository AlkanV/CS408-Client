using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS408_Client
{
    public partial class FormInvite : Form
    {
        string userName;
        public bool accepted { get; set; }
        public Form RefToFormConnection { get; set; }
        public FormInvite(string username)
        {
            InitializeComponent();
            userName = username;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            accepted = true;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            accepted = false;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lblInvitation_Click(object sender, EventArgs e)
        {
            lblInvitation.Text = userName + "has sent you an invite! It seems that you are not that much alone!";
        }
    }
}
