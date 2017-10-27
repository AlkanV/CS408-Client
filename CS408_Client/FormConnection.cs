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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // 1 - Input parameters
            string IPinput = textBox2.Text;
            int PortInput = Convert.ToInt32(textBox1.Text);
            string usernameInput = textBox3.Text;

            // 2 - Create the connection
            TcpClient client = new TcpClient(IPinput, PortInput);
            NetworkStream stream = client.GetStream();
            byte[] connectionData = ASCIIEncoding.ASCII.GetBytes("u|" + usernameInput);

            // 3 - Send the text
            stream.Write(connectionData, 0, connectionData.Length);

            // 4 - Read back data
            byte[] responseBuffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(responseBuffer, 0, client.ReceiveBufferSize);
            Console.WriteLine("Read: ", Encoding.ASCII.GetString(responseBuffer, 0, bytesRead));
        }
    }
}
