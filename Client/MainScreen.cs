using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO.Compression;

namespace client_ppp
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    struct DEVMODE
    {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        [System.Runtime.InteropServices.FieldOffset(0)]
        public string dmDeviceName;
        [System.Runtime.InteropServices.FieldOffset(32)]
        public Int16 dmSpecVersion;
        [System.Runtime.InteropServices.FieldOffset(34)]
        public Int16 dmDriverVersion;
        [System.Runtime.InteropServices.FieldOffset(36)]
        public Int16 dmSize;
        [System.Runtime.InteropServices.FieldOffset(38)]
        public Int16 dmDriverExtra;
        [System.Runtime.InteropServices.FieldOffset(40)]
        public DM dmFields;

        [System.Runtime.InteropServices.FieldOffset(44)]
        Int16 dmOrientation;
        [System.Runtime.InteropServices.FieldOffset(46)]
        Int16 dmPaperSize;
        [System.Runtime.InteropServices.FieldOffset(48)]
        Int16 dmPaperLength;
        [System.Runtime.InteropServices.FieldOffset(50)]
        Int16 dmPaperWidth;
        [System.Runtime.InteropServices.FieldOffset(52)]
        Int16 dmScale;
        [System.Runtime.InteropServices.FieldOffset(54)]
        Int16 dmCopies;
        [System.Runtime.InteropServices.FieldOffset(56)]
        Int16 dmDefaultSource;
        [System.Runtime.InteropServices.FieldOffset(58)]
        Int16 dmPrintQuality;

        [System.Runtime.InteropServices.FieldOffset(44)]
        public POINTL dmPosition;
        [System.Runtime.InteropServices.FieldOffset(52)]
        public Int32 dmDisplayOrientation;
        [System.Runtime.InteropServices.FieldOffset(56)]
        public Int32 dmDisplayFixedOutput;

        [System.Runtime.InteropServices.FieldOffset(60)]
        public short dmColor;
        [System.Runtime.InteropServices.FieldOffset(62)]
        public short dmDuplex;
        [System.Runtime.InteropServices.FieldOffset(64)]
        public short dmYResolution;
        [System.Runtime.InteropServices.FieldOffset(66)]
        public short dmTTOption;
        [System.Runtime.InteropServices.FieldOffset(68)]
        public short dmCollate;
        [System.Runtime.InteropServices.FieldOffset(72)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        [System.Runtime.InteropServices.FieldOffset(102)]
        public Int16 dmLogPixels;
        [System.Runtime.InteropServices.FieldOffset(104)]
        public Int32 dmBitsPerPel;
        [System.Runtime.InteropServices.FieldOffset(108)]
        public Int32 dmPelsWidth;
        [System.Runtime.InteropServices.FieldOffset(112)]
        public Int32 dmPelsHeight;
        [System.Runtime.InteropServices.FieldOffset(116)]
        public Int32 dmDisplayFlags;
        [System.Runtime.InteropServices.FieldOffset(116)]
        public Int32 dmNup;
        [System.Runtime.InteropServices.FieldOffset(120)]
        public Int32 dmDisplayFrequency;
    }

    struct POINTL
    {
        public Int32 x;
        public Int32 y;
    }

    [Flags()]
    enum DM : int
    {
        Orientation = 0x1,
        PaperSize = 0x2,
        PaperLength = 0x4,
        PaperWidth = 0x8,
        Scale = 0x10,
        Position = 0x20,
        NUP = 0x40,
        DisplayOrientation = 0x80,
        Copies = 0x100,
        DefaultSource = 0x200,
        PrintQuality = 0x400,
        Color = 0x800,
        Duplex = 0x1000,
        YResolution = 0x2000,
        TTOption = 0x4000,
        Collate = 0x8000,
        FormName = 0x10000,
        LogPixels = 0x20000,
        BitsPerPixel = 0x40000,
        PelsWidth = 0x80000,
        PelsHeight = 0x100000,
        DisplayFlags = 0x200000,
        DisplayFrequency = 0x400000,
        ICMMethod = 0x800000,
        ICMIntent = 0x1000000,
        MediaType = 0x2000000,
        DitherType = 0x4000000,
        PanningWidth = 0x8000000,
        PanningHeight = 0x10000000,
        DisplayFixedOutput = 0x20000000
    }

    [Flags]
    public enum MouseEventFlags : uint
    {
        LEFTDOWN = 0x00000002,
        LEFTUP = 0x00000004,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP = 0x00000040,
        MOVE = 0x00000001,
        ABSOLUTE = 0x00008000,
        RIGHTDOWN = 0x00000008,
        RIGHTUP = 0x00000010,
        WHEEL = 0x00000800,
        XDOWN = 0x00000080,
        XUP = 0x00000100
    }

    public partial class MainScreen : Form
    {

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
           UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData,
           UIntPtr dwExtraInfo);


        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        const int KEYEVENTF_KEYUP = 0x2;
        const int KEYEVENTF_EXTENDEDKEY = 0x1;

        Socket mouseKeyboardListening = null;
        byte[] sequence = new byte[2000];
        
        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private Button button1;
        private Button button2;

        delegate void SendImageHandler();
        string otherComputerIP = "";

        private bool getFileData = false;
        private int ind = 0;
        private string fileName = "";
        private int width = 0;
        private int height = 0;

        public MainScreen()
        {
            InitializeComponent();
        }

        /* when form is loaded change the lebel text to ip of this computer
        * input: object sender, EventArgs 
        * output: null
        */
        private void frmServerAnaform_Load(object sender, EventArgs e)
        {
            //string host = Dns.GetHostName();
            IPHostEntry host;
            string localIp = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIp = ip.ToString();
                    //lblIP.Text = ip.ToString();
                    lblIP.Text = localIp;
                }
            }
        }

        /* when connected wait for when data arrives
        * input: IAsyncResult iar
        * output: null
        */
        void whenConnected(IAsyncResult iar)
        {
            Socket soket = mouseKeyboardListening.EndAccept(iar);
            soket.BeginReceive(sequence, 0, sequence.Length, SocketFlags.None, new AsyncCallback(whenDataArrives), soket);
            mouseKeyboardListening.BeginAccept(new AsyncCallback(whenConnected), null);
        }

       /* when data arrives check what is this data if this is mouse position then
       * change the mouse position if it is keyboard key then apply it 
       * input: IAsyncResult iar
       * output: null
       */
        void whenDataArrives(IAsyncResult iar)
        {
            Socket soket = (Socket)iar.AsyncState;
            int incomingDataLength = soket.EndReceive(iar);
            byte[] data = new byte[incomingDataLength];
            Array.Copy(sequence, data, data.Length);
            string execution = Encoding.UTF8.GetString(data);
            if (execution.Contains("AC"))
            {
                otherComputerIP = execution.Substring(0, execution.IndexOf('|'));
                DEVMODE dv = new DEVMODE();
                EnumDisplaySettings(null, -1, ref dv);
                string size = dv.dmPelsWidth.ToString() + ":" + dv.dmPelsHeight.ToString() + "|";
                width = dv.dmPelsWidth;
                height = dv.dmPelsHeight;
                //string gonderilcek = "1280" + ":" + "720" + "|";
                //if (dv.dmPelsWidth.ToString() < 1280)

                displayScreening(size);
                SendImageHandler sendImage = new SendImageHandler(SendImage);
                sendImage.BeginInvoke(new AsyncCallback(processEnded), null);
            }
            else if (execution.Contains("DragNDrop"))
            {
                //Thread reciveFile = new Thread(new ThreadStart(getFile));
                //reciveFile.Start();
                string outputFolder = "Transfers";
                string index = "";
                int read = 0;
                //byte[] fileData = new byte[1500];
                byte[] fileData = new byte[Constants.FILE_DATA_SIZE];
                string del = "|DragNDrop";
                execution = execution.Replace(del, "");
                if (execution == "EndOfFile")
                {
                    getFileData = false;
                    ind = 0;
                }
                else if (getFileData)
                {
                    FileStream fs = new FileStream(Path.Combine(outputFolder, fileName), FileMode.Open);
                    index = execution.Substring(0, execution.IndexOf(':')); //getting index
                    //fs.Position = Convert.ToInt64(index);
                    read = int.Parse(execution.Substring(execution.IndexOf(':') + 1, execution.IndexOf('|') - execution.IndexOf(':') - 1));
                    del = index + ":" + read.ToString() + "|";
                    execution = execution.Replace(del, "");

                    fileData = Encoding.ASCII.GetBytes(execution);

                    lock (this)
                    {
                        fs.Position = ind;
                        fs.Write(fileData, 0, fileData.Length);
                        //fs.Write(fileData, 0, read);
                    }
                    ind = ind + fileData.Length;
                    fs.Close();
                }
                else if (execution != null)
                {

                    //string outputFolder = "Transfers";
                    fileName = execution.Substring(0, execution.IndexOf(':'));
                    int fileSize = int.Parse(execution.Substring(execution.IndexOf(':') + 1, execution.IndexOf('|') - execution.IndexOf(':') - 1));
                    del = fileName + ":" + fileSize.ToString() + "|";
                    execution = execution.Replace(del, "");

                    FileStream fs = new FileStream(Path.Combine(outputFolder, fileName), FileMode.Create);
                    fs.SetLength(fileSize);

                    fs.Close();
                    getFileData = true;
                }


            }
            else
            {
                // to set the real 
                //int aa = Screen.PrimaryScreen.Bounds.Height - 1280 + 16;
                //int bb = Screen.PrimaryScreen.Bounds.Height - 720 + 38;
                if (execution.Contains("MouseMove"))
                {
                    int x = int.Parse(execution.Substring(0, execution.IndexOf(':')));
                    int y = int.Parse(execution.Substring(execution.IndexOf(':') + 1, execution.IndexOf('|') - execution.IndexOf(':') - 1));
                    ////x = x + aa;
                    ////y = y + bb;
                    //x = x + 16;
                    //y = y + 38;
                    //Cursor.Position = new Point(x, y);
                    SetCursorPos(x, y);
                }
                else if (execution.Contains("MouseDown"))
                {
                    string a = execution.Substring(0, execution.IndexOf(':'));
                    if (a == "Left")
                        mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                    else
                        mouse_event((uint)MouseEventFlags.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                }
                else if (execution.Contains("MouseUp"))
                {
                    string a = execution.Substring(0, execution.IndexOf(':'));
                    if (a == "Left")
                        mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero);
                    else
                        mouse_event((uint)MouseEventFlags.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                }
                else if (execution.Contains("Key"))
                {

                    byte tusKodu = byte.Parse(execution.Substring(0, execution.IndexOf(':')));
                    keybd_event(tusKodu, 0x45, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                }
            }
        }

       /* this funciton send display size/resulotion
       * input: string toBeSent
       * output: null
       */
        void displayScreening(string toBeSent)
        {
            Socket connect = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //connect.Connect(IPAddress.Parse(otherComputerIP), 1453);
            connect.Connect(IPAddress.Parse(otherComputerIP), Constants.REMOTE_PORT);
            byte[] solve = Encoding.UTF8.GetBytes(toBeSent);
            connect.Send(solve, 0, solve.Length, SocketFlags.None);
            connect.Close();
        }
        /* this funciton send the screenshot image 
       * input: null
       * output: null
       */
        void SendImage()
        {
            while (true)
            {
                Socket connect = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connect.Connect(IPAddress.Parse(otherComputerIP), Constants.REMOTE_PORT);
                byte[] screen = Screenshot();
                connect.Send(screen, 0, screen.Length, SocketFlags.None);
                connect.Close();
            }
        }
        /* this funciton screenshot and save it as image in memory stream then compress it and return the bytes
       * input: null
       * output: compressed screntshot bytes
       */
        byte[] Screenshot()
        {
            // #1
            //Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            //Graphics gr = Graphics.FromImage(bmp);
            //gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            //MemoryStream ms = new MemoryStream();
            //bmp.Save(ms, ImageFormat.MemoryBmp);
            //return ms.GetBuffer();

            ////////////////////////////
            /// #2
            //Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
            //                       Screen.PrimaryScreen.Bounds.Height,
            //                       PixelFormat.Format32bppArgb);

            //// Create a graphics object from the bitmap.
            //var gfxScreenshot = Graphics.FromImage(screenshot);

            //// Take the screenshot from the upper left corner to the right bottom corner.
            //gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
            //                            Screen.PrimaryScreen.Bounds.Y,
            //                            0,
            //                            0,
            //                            Screen.PrimaryScreen.Bounds.Size,
            //                            CopyPixelOperation.SourceCopy);

            //MemoryStream ms = new MemoryStream();
            //screenshot.Save(ms, ImageFormat.Png);
            //return ms.GetBuffer();


            // compress

            

            //Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap bmp = new Bitmap(width, height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            //compress

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);


            byte[] compressed;

            using (var outStream = new MemoryStream())
            {
                using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    using (var mStream = new MemoryStream(ms.GetBuffer()))
                    {
                        mStream.CopyTo(tinyStream);


                        compressed = outStream.ToArray();
                    }
                }
            }


            return compressed;

        }

        /* this run when the process ends
       * input: IAsyncResult iar
       * output: null
       */
        void processEnded(IAsyncResult iar)
        {
        }


        /* this let the user to be the one who controlls the other computer and run the conrollers remote screen form
       * input: object sender, EventArgs e
       * output: null
       */
        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            RemoteScreen screen = new RemoteScreen(textBox1.Text);
            screen.ShowDialog();

            // here need to return after we finish what we selected
            this.Close();
        }

        //private void InitializeComponent()
        //{
        //    this.button2 = new System.Windows.Forms.Button();
        //    this.SuspendLayout();
        //    // 
        //    // button2
        //    // 
        //    this.button2.Location = new System.Drawing.Point(29, 32);
        //    this.button2.Name = "button2";
        //    this.button2.Size = new System.Drawing.Size(75, 23);
        //    this.button2.TabIndex = 0;
        //    this.button2.Text = "Give Access";
        //    this.button2.UseVisualStyleBackColor = true;
        //    this.button2.Click += new System.EventHandler(this.button2_Click);
        //    // 
        //    // MainScreen
        //    // 
        //    this.ClientSize = new System.Drawing.Size(284, 261);
        //    this.Controls.Add(this.button2);
        //    this.Name = "MainScreen";
        //    this.ResumeLayout(false);

        //}

        /* give the permission to other users to controll this computer and waits for an connection
         * input: object sender, EventArgs e
         * output: null
         */
        private void button2_Click(object sender, EventArgs e)
        {
                button1.Enabled = false;
                mouseKeyboardListening = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //mouse_klavyeDinleme.Bind(new IPEndPoint(IPAddress.Parse(ip.AddressList[0].ToString()), 1453));
                mouseKeyboardListening.Bind(new IPEndPoint(IPAddress.Parse(lblIP.Text), Constants.REMOTE_PORT));

                mouseKeyboardListening.Listen(1);


                mouseKeyboardListening.BeginAccept(new AsyncCallback(whenConnected), null);
        }
    }
}
