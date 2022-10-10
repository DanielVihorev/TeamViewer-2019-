using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace client_ppp
{
    public partial class RemoteScreen : Form
    {
        Socket socket = null;
        //Byte[] bytes = new Byte[9999999];
        Byte[] bytes = new Byte[Constants.MAX_BYTE_SIZE];
        string ipp = "";

        int otherComputerWidth = 0;
        int otherComputerHeight = 0;


        public RemoteScreen(string ip)
        {
            ipp = ip;
            InitializeComponent();
        }

        /* when trhe screen load get the corrent computer ip and try to connect to server
        * input: object sender, EventArgs e
        * output: null
        */
        private void Screen_Load(object sender, EventArgs e)
        {
            string hostt = Dns.GetHostName();
            IPHostEntry ippp = Dns.GetHostByName(hostt);

            IPHostEntry host;
            string localIp = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIp = ip.ToString();
                }
            }


            //
            //function3("10.0.0.5");
            //
            // change ip here
            firstConnectSend(localIp);
            //IPEndPoint iipp = new IPEndPoint(IPAddress.Parse(localIp),1453);
            IPEndPoint iipp = new IPEndPoint(IPAddress.Parse(localIp), Constants.REMOTE_PORT);
            //

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            socket.Bind(iipp);

            socket.Listen(1);
            socket.BeginAccept(new AsyncCallback(beenConnectedTo), null);
            System.Threading.Thread.Sleep(1000);
            
            //function3(localIp);
            //function3(localIp);
        }

        /* when it connected to another computer it will wait to get data from another computer
        * input: IAsyncResult var
        * output: null
        */
        void beenConnectedTo(IAsyncResult var)
        {
            Socket sock = socket.EndAccept(var);
            sock.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(dataReceived), sock);
            socket.BeginAccept(new AsyncCallback(beenConnectedTo), null);
        }
        bool otherComputerInfo = false;


        /* when data recived it will check if it is bigger than 50 to check if is the screent shot or another computer screen details
        * input: IAsyncResult var
        * output: null
        */
        void dataReceived(IAsyncResult var)
        {
            Socket sock = (Socket)var.AsyncState;
            int num = sock.EndReceive(var);
            Byte[] bytes2 = new Byte[num];
            Array.Copy(bytes, bytes2, bytes2.Length);
            // if size of byte[] (num) is bigger than 50 it means that it is a screenshot
            if(num < 50)
            //if(!otherComputerInfo)
            {
                string st = Encoding.UTF8.GetString(bytes2);
                int width = int.Parse(st.Substring(0, st.IndexOf(':')));
                int height = int.Parse(st.Substring(st.IndexOf(':') + 1, st.IndexOf('|') - st.IndexOf(':') - 1));


                otherComputerWidth = width;
                otherComputerHeight = height;
                //this.Size = new Size(width+16, height+38);
                //try
                //{
                //    this.Size = new Size(1024 + 17, 600 + 40);
                //}
                //catch
                //{ }
                //this.Size = new Size(1024 + 17, 600 + 40);

                //this.Size = new Size(1024 + 17 - 6, 600 + 40 - 6);
                // minus 6 because change of the form border style to single

                otherComputerInfo = true;

            }
            else
            {
                //MemoryStream ms = new MemoryStream(bytes2);
                //Image img = Bitmap.FromStream(ms);
                //pictureBox1.Image = img;

                //decomopress

                MemoryStream ms = new MemoryStream(bytes2);
                Image img = null;
                using (var inStream = new MemoryStream(bytes2))
                {
                    using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
                    {
                        using (var bigStreamOut = new MemoryStream())
                        {
                            try
                            {


                                bigStream.CopyTo(bigStreamOut);
                                img = Bitmap.FromStream(bigStreamOut);
                            }
                            catch { }
                        }
                    }
                }


                //Image img = Bitmap.FromStream(ms);


                //added on 08/03.2019
                //if (pictureBox1.Size != this.Size)
                //{
                //    pictureBox1.Size = this.Size;
                //}
                //
                pictureBox1.Image = img;
            }

        }

        /* try to connect to server this function is the fiorst try to connect and it sends the ip as an string to other computer
        * input: string ip
        * output: null
        */
        void firstConnectSend(string ip)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
            Byte[] bytes3 = Encoding.UTF8.GetBytes(ip +"|AC");
            sock.Send(bytes3);
            //sock.Send(bytes3, 0, bytes3.Length, SocketFlags.None);
            sock.Close();
        }

        /* handle the mouse function and send it to the controlled computer
        * input: object sender, MouseEventArgs e
        * output: null
        */
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Byte[] bytes3 = null;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bytes3 = Encoding.UTF8.GetBytes("Left:MouseDown");
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                bytes3 = Encoding.UTF8.GetBytes("Right:MouseDown");
            }
            else
            {
                return;
            }
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
            sock.Send(bytes3);
            sock.Close();
        }

        /* handle the mouse movement and position and send the position to the controlled computer
        * input: object sender, MouseEventArgs e
        * output: null
        */
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X * otherComputerWidth / (this.Size.Width - 17);
            int y = e.Y * otherComputerHeight / (this.Size.Height - 40);

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
            Byte[] bytes3 = Encoding.UTF8.GetBytes(x.ToString() + ":" + y.ToString() + "|MouseMove");
            sock.Send(bytes3);
            sock.Close();
        }
        /* handle the mouse function and send it to the controlled computer
        * input: object sender, MouseEventArgs e
        * output: null
        */
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Byte[] bytes3 = null;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bytes3 = Encoding.UTF8.GetBytes("Left:MouseUp");
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                bytes3 = Encoding.UTF8.GetBytes("Right:MouseUp");
            }
            else
            {
                return;
            }
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
            sock.Send(bytes3);
            sock.Close();
        }

        /* handle the keyboard key entered and send it to the controlled computer
        * input: object sender, MouseEventArgs e
        * output: null
        */
        private void Screen_KeyUp(object sender, KeyEventArgs e)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
            Byte[] bytes3 = Encoding.UTF8.GetBytes(e.KeyValue.ToString()+ ":Key");
            sock.Send(bytes3);
            sock.Close();
        }

        /* handle the file that has dragged to the form
        * input: object sender, MouseEventArgs e
        * output: null
        */
        private void RemoteScreen_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        /* when dropping the file that has been dragged to form send it to the controlled computer
        * input: object sender, MouseEventArgs e
        * output: null
        */
        private void RemoteScreen_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (string file in files)
            {
                //MessageBox.Show(file);
                //sendFile(file);

                //#working
                FileStream fs = new FileStream(file, FileMode.Open);
                //fs.Read(fileData, 0, fileData.Length);
                //if (fs.Length < 16000000)
                //{
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
                    // PacketWriter ppw = new PacketWriter();
                    //ppw.Write(Path.GetFileName(file));
                    //ppw.Write(fs.Length);
                    //Byte[] bytes3 = Encoding.UTF8.GetBytes(Path.GetFileName(file) + ":" + fs.Length.ToString() + "|" + System.Text.Encoding.UTF8.GetString(fileData) + "|DragNDrop");
                    Byte[] bytes3 = Encoding.UTF8.GetBytes(Path.GetFileName(file) + ":" + fs.Length.ToString() + "|" + "|DragNDrop");
                    //Byte[] bytes3 = Encoding.UTF8.GetBytes(System.Text.Encoding.UTF8.GetString(ppw.GetBytes()) + "|DragNDrop");
                    sock.Send(bytes3);
                    sock.Close();

                    System.Threading.Thread.Sleep(1000);
                    //sending file data by 2048 at one time

                    // BinaryReader br = new BinaryReader(fs);
                    long index = 0;
                    int read = 0;
                    index = fs.Position;
                    while (index < fs.Length)
                    {

                        ///PacketWriter pw = new PacketWriter();
                        byte[] fileData = new byte[1500];
                        //fs.Position = index;

                        read = fs.Read(fileData, 0, 1000);
                        //pw.Write(fileData, 0, read);
                        byte[] b = new byte[read];
                        Array.Copy(fileData, b, b.Length);
                        string ssssttt;
                        lock (this)
                        {
                            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
                            //bytes3 = Encoding.UTF8.GetBytes("|DragNDrop" + System.Text.Encoding.UTF8.GetString(fileData));
                            //bytes3 = Encoding.UTF8.GetBytes("|DragNDrop"+index.ToString()+":"+read.ToString()+"|" + System.Text.Encoding.UTF8.GetString(pw.GetBytes()));
                            ssssttt = System.Text.Encoding.UTF8.GetString(b);
                            bytes3 = Combine(Encoding.UTF8.GetBytes("|DragNDrop" + index.ToString() + ":" + read.ToString() + "|"), b);
                            sock.Send(bytes3);
                            sock.Close();
                        }
                        index = index + read;
                        Thread.Sleep(500);

                    }
                    fs.Close();
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(IPAddress.Parse(ipp), Constants.REMOTE_PORT);
                    bytes3 = Encoding.UTF8.GetBytes("EndOfFile" + "|DragNDrop");
                    sock.Send(bytes3);
                    sock.Close();
                //}


            }
            

        }

        /* combine the first byte array with the second byte array and return the combine
        * input: byte[] first, byte[] second
        * output: byte[] combined
        */
        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
