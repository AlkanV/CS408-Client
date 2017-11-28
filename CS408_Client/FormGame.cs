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
    public partial class FormGame : Form
    {
        public int surrendered { get; set; }
        public Form RefToFormConnection { get; set; }
        public FormGame()
        {
            surrendered = 0;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            surrendered = 1;
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
