using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace client_ppp
{
    public partial class LogInScreen : Form
    {
        public static string MSG { get; set; }
        public static string RecivedMSG { get; set; }
        public static TcpClient client { get; set; }
        public static NetworkStream clientStream { get; set; }
        public static IPEndPoint serverEndPoint { get; set; }
        public static NetworkStream clientStream2 { get; set; }
        public static TcpClient client3 { get; set; }
        public static byte[] key { get; set; }
        
        /* run the log in form
        * input: null
        * output: null
        */
        public LogInScreen()
        {
            InitializeComponent();
            //Init_Data();
            Thread c = new Thread(serverConnection);
            c.Start();

        }
        /* try to connect to server and when connected send the first message
        * input: null
        * output: null
        */
        private void serverConnection()
        {
            try
            {
                client = new TcpClient();
                while (!client.Connected)
                {
                    //starts the connection
                    client = new TcpClient();
                    // change ip here
                    // ip for computer that runs the server on it
                    serverEndPoint = new IPEndPoint(IPAddress.Parse("192.168.20.226"), Constants.PORT);
                    client.Connect(serverEndPoint);
                    clientStream = client.GetStream();
                }
                MSG = "";
                sendMSG();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                serverConnection();
                //Application.Exit();
            }

        }
        /* send message to server
        * input: null
        * output: null
        */
        public static void sendMSG()
        {
            try
            {
                clientStream = client.GetStream();
                byte[] buffer = new ASCIIEncoding().GetBytes(MSG);
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        /* recive message from server
        * input: null
        * output: null
        */
        public static void reciveMSG()
        {
            try
            {
                byte[] bufferIn = new byte[100];
                int bytesRead = clientStream.Read(bufferIn, 0, 100);
                RecivedMSG = new ASCIIEncoding().GetString(bufferIn);
                RecivedMSG = RecivedMSG.Replace("\0","");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        /* send message and then recive a message from server
        * input: null
        * output: null
        */
        public static void msgHandler()
        {
            sendMSG();
            reciveMSG();
        }

        /* try to log in to the project
        * input: object sender, EventArgs 
        * output: null
        */
        private void LogInButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(usernameBox.Text != string.Empty || passwordBox.Text != string.Empty)
                {
                    string pass = passwordBox.Text;

                    errorLabel.Invoke(new Action(() => errorLabel.Text = string.Empty));

                    string uLen = (usernameBox.Text.Length < 10) ? Constants.ZERO + usernameBox.Text.Length.ToString() : usernameBox.Text.Length.ToString();
                    string pLen = (pass.Length < 10) ? Constants.ZERO + pass.Length.ToString() : pass.Length.ToString();

                    MSG = Constants.LOG_IN + uLen + usernameBox.Text + pLen + pass;
                    msgHandler();
                    if (RecivedMSG == Constants.SUCCESS)
                    {
                        save_data();
                        this.Hide();

                        Option opt = new Option(client);
                        opt.ShowDialog();
                        //this.Show();
                        ///////////////////////////////////////////////////////////////////////////
                        this.Close();

                        //this.Show();
                    }
                    else if(RecivedMSG == Constants.NOT_REGISTERED)
                    {
                        //save_data();
                        //this.Hide();
                        MessageBox.Show("Username or Password is is incorrect!!!\nPlease Try Again...");
                    }
                }
                else if(usernameBox.Text == "" || passwordBox.Text == "")
                {
                    MessageBox.Show("Username or Password is Empty", "Error", MessageBoxButtons.OK);
                }
            }

            catch (Exception)
            {
                MessageBox.Show("Error", "Something Happened", MessageBoxButtons.OK);
            }
        }

        /* run the sign up form
        * input: object sender, EventArgs 
        * output: null
        */
        private void SignUpButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form signUp = new SignUpForm();
            signUp.ShowDialog();
            this.Show();
        }

        /* checks if there is user data saved from remember me feature
        * input: null 
        * output: null
        */
        private void Init_Data()
        {
            if (Properties.Settings.Default.Username != string.Empty)
            {
                if (Properties.Settings.Default.Remember == "yes")
                {
                    usernameBox.Text = Properties.Settings.Default.Username;
                    passwordBox.Text = Properties.Settings.Default.Password;
                }
                else
                {
                    usernameBox.Text = Properties.Settings.Default.Username;
                }
            }

        }

        /* if user choose remember me feature this function will save his data
        * input: null
        * output: null
        */
        private void save_data()
        {
            if (remeberBox.Checked)
            {
                Properties.Settings.Default.Username = usernameBox.Text;
                Properties.Settings.Default.Password = passwordBox.Text;
                Properties.Settings.Default.Remember = "yes";
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Username = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Remember = "no";
                Properties.Settings.Default.Save();
            }
        }

        /* this function run when the log in form load
        * input: object sender, EventArgs 
        * output: null
        */
        private void LogInScreen_Load(object sender, EventArgs e)
        {
            Init_Data();
        }
    }
}
