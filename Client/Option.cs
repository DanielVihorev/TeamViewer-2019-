using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

static class Constants
{
    public const string LOG_OUT = "201";
}

namespace client_ppp
{
    public partial class Option : Form
    {

        TcpClient _client;
        public Option(TcpClient client)
        {
            InitializeComponent();
            _client = client;
            this.FormClosing += Option_FormClosing;
        }
        /* if user choose the remote desktop option
        * input: object sender, EventArgs 
        * output: null
        */
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainScreen Main = new MainScreen();
            Main.ShowDialog();
            Main.Close();
            this.Show();
        }

        /* if user choose file transfer option
        * input: object sender, EventArgs 
        * output: null
        */
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main file_transfer = new Main();
            file_transfer.ShowDialog();
            this.Show();
        }

        /* if user choose to log out
        * input: object sender, EventArgs 
        * output: null
        */
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                NetworkStream clientStream = _client.GetStream();
                //byte[] buffer = new ASCIIEncoding().GetBytes("201");
                byte[] buffer = new ASCIIEncoding().GetBytes(Constants.LOG_OUT);
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                _client.Close();
                Application.Exit();
            }
            catch { }
        }
            
        /* if user choose exit the form then it will automatically log out
        * input: object sender, EventArgs 
        * output: null
        */
        private void Option_FormClosing(Object sender, FormClosingEventArgs e)
        {
            try
            {
                NetworkStream clientStream = _client.GetStream();
                //byte[] buffer = new ASCIIEncoding().GetBytes("201");
                byte[] buffer = new ASCIIEncoding().GetBytes(Constants.LOG_OUT);
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                _client.Close();
                //Application.Exit();
            }
            catch { }
        }
    }
}
