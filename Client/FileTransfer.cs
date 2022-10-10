using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using client_ppp;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading.Tasks;
using VirusTotalNET.Objects;
using VirusTotalNET.ResponseCodes;
using VirusTotalNET.Results;
using VirusTotalNET;

public partial class Main : Form
{
    public static byte[] key;
    public static byte[] IV;

    public static byte[] firstKey;
    public static byte[] firstIV;
    public static byte[] secondKey;
    public static byte[] secondIV;

    static string resultFromScan = "";

    /* encrypt function , will encrypt the bytes with aes encryption
     * input: byte[] bytes
     * output: byte[] encrepted bytes
     */
    public static byte[] Encrypt(byte[] bytes)
    {
        if (bytes == null || key == null || IV == null || bytes.Length <= 0 || key.Length <= 0 || IV.Length <= 0)
        {
            return null;
        }
        byte[] data;
        // create aes object with key and IV
        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            aes.IV = IV;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                data = ms.ToArray();
                
            }
        }
        return data;
    }

    /* dycrypt function , will decrypt the bytes with aes decryption
     * input: byte[] bytes
     * output: byte[] decrepted bytes
     */
    public static byte[] Decrypt(byte[] bytes)
    {
        if (bytes == null || key == null || IV == null || bytes.Length <= 0 || key.Length <= 0 || IV.Length <= 0)
        {
            return null;
        }
        byte[] data;
        // create aes object with key and IV
        using (Aes aes = new AesCryptoServiceProvider())
        {
            aes.Key = key;
            aes.IV = IV;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(bytes, 0, bytes.Length);
                //cs.FlushFinalBlock();
                data = ms.ToArray();

            }
        }
        return data;
    }


    //This will hold our listener. We will only need to create one instance of this.
    private Listener listener;
    //This will hold our transfer client.
    private TransferClient transferClient;
    //This will hold our output folder.
    private string outputFolder;
    //This will hold our overall progress timer.
    private Timer tmrOverallProg;
    //This is our variable to determine of the server is running or not to accept another connection if our client
    //Disconnects
    private bool serverRunning;

    public Main()
    {
        InitializeComponent();
        //Create the listener and register the event.
        listener = new Listener();
        listener.Accepted += listener_Accepted;

        //Create the timer and register the event.
        tmrOverallProg = new Timer();
        tmrOverallProg.Interval = 1000;
        tmrOverallProg.Tick += tmrOverallProg_Tick;

        //Set our default output folder.
        outputFolder = "Transfers";

        //If it does not exist, create it.
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        btnConnect.Click += new EventHandler(btnConnect_Click);
        btnStartServer.Click += new EventHandler(btnStartServer_Click);
        btnStopServer.Click += new EventHandler(btnStopServer_Click);
        btnSendFile.Click += new EventHandler(btnSendFile_Click);
        btnPauseTransfer.Click += new EventHandler(btnPauseTransfer_Click);
        btnStopTransfer.Click += new EventHandler(btnStopTransfer_Click);
        btnOpenDir.Click += new EventHandler(btnOpenDir_Click);
        btnClearComplete.Click += new EventHandler(btnClearComplete_Click);

        btnStopServer.Enabled = false;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        //Deregister all the events from the client if it is connected.
        deregisterEvents();
        base.OnFormClosing(e);
    }

    void tmrOverallProg_Tick(object sender, EventArgs e)
    {
        if (transferClient == null)
            return;
        //Get and display the overall progress.
        progressOverall.Value = transferClient.GetOverallProgress();
    }

    void listener_Accepted(object sender, SocketAcceptedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(new SocketAcceptedHandler(listener_Accepted), sender, e);
            return;
        }
        
        //Stop the listener
        listener.Stop();

        //Create the transfer client based on our newly connected socket.
        transferClient = new TransferClient(e.Accepted);
        //Set our output folder.
        transferClient.OutputFolder = outputFolder;
        //Register the events.
        registerEvents();
        //Run the client
        transferClient.Run();
        //Start the progress timer
        tmrOverallProg.Start();
        //And set the new connection state.
        setConnectionStatus(transferClient.EndPoint.Address.ToString());
    }

    /* this function will create new transfer client and attempts to connect 
     * then it will send the first generated encryption key before it starts to send the files
     * input: object sender, EventArgs e
     * output: null
     */
    private void btnConnect_Click(object sender, EventArgs e)
    {
        if (transferClient == null)
        {
            key = null;
            IV = null;
            firstKey = null;
            firstIV = null;
            secondKey = null;
            secondIV = null;
            //Create our new transfer client.
            //And attempt to connect
            transferClient = new TransferClient();
            transferClient.Connect(txtCntHost.Text.Trim(), int.Parse(txtCntPort.Text.Trim()), connectCallback);
            Enabled = false;
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                //key = aes.Key;
                //IV = aes.IV;
                firstKey = aes.Key;
                firstIV = aes.IV;
            }
            System.Threading.Thread.Sleep(1000);
            //transferClient.Send(key);
            //transferClient.Send(IV);
            transferClient.Send(firstKey);
            transferClient.Send(firstIV);
        }
        else
        {
            //This means we're trying to disconnect.
            transferClient.Close();
            transferClient = null;
        }
    }

    /* this function will start the connection
     * input: object sender, EventArgs e
     * output: null
     */

    private void connectCallback(object sender, string error)
    {
        if (InvokeRequired)
        {
            Invoke(new ConnectCallback(connectCallback), sender, error);
            return;
        }
        //Set the form to enabled.
        Enabled = true;
        //If the error is not equal to null, something went wrong.
        if (error != null)
        {
            transferClient.Close();
            transferClient = null;
            MessageBox.Show(error, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        //Register the events
        registerEvents();
        //Set the output folder
        transferClient.OutputFolder = outputFolder;
        //Run the client
        transferClient.Run();
        //Set the connection status
        setConnectionStatus(transferClient.EndPoint.Address.ToString());
        //Start the progress timer.
        tmrOverallProg.Start();
        //Set our connect button text to "Disconnect"
        btnConnect.Text = "Disconnect";
    }

    private void registerEvents()
    {
        transferClient.Complete += transferClient_Complete;
        transferClient.Disconnected += transferClient_Disconnected;
        transferClient.ProgressChanged += transferClient_ProgressChanged;
        transferClient.Queued += transferClient_Queued;
        transferClient.Stopped += transferClient_Stopped;
    }

    /* this function will strop the transfer
     * input: object sender, EventArgs e
     * output: null
     */
    void transferClient_Stopped(object sender, TransferQueue queue)
    {
        if (InvokeRequired)
        {
            Invoke(new TransferEventHandler(transferClient_Stopped), sender, queue);
            return;
        }
        //Remove the stopped transfer from view.
        lstTransfers.Items[queue.ID.ToString()].Remove();
    }

    /* this function will queue the file that needed to be send to another computer
     * input: object sender, EventArgs e
     * output: null
     */
    void transferClient_Queued(object sender, TransferQueue queue)
    {
        if (InvokeRequired)
        {
            Invoke(new TransferEventHandler(transferClient_Queued), sender, queue);
            return;
        }

        //Create the LVI for the new transfer.
        ListViewItem i = new ListViewItem();
         i.Text = queue.ID.ToString();
        i.SubItems.Add(queue.Filename);
        //If the type equals download, it will use the string of "Download", if not, it'll use "Upload"
        i.SubItems.Add(queue.Type == QueueType.Download ? "Download" : "Upload");
        i.SubItems.Add("0%");
        i.Tag = queue; //Set the tag to queue so we can grab is easily.
        i.Name = queue.ID.ToString(); //Set the name of the item to the ID of our transfer for easy access.
        lstTransfers.Items.Add(i); //Add the item
        i.EnsureVisible();
        
        //If the type is download, let the uploader know we're ready.
        if (queue.Type == QueueType.Download)
        {
            transferClient.StartTransfer(queue);
        }
    }

    /* this function will change the file transfer progress bar
     * input: object sender, EventArgs e
     * output: null
     */
    void transferClient_ProgressChanged(object sender, TransferQueue queue)
    {
        if (InvokeRequired)
        {
            Invoke(new TransferEventHandler(transferClient_ProgressChanged), sender, queue);
            return;
        }

        //Set the progress cell to our current progress.
        lstTransfers.Items[queue.ID.ToString()].SubItems[3].Text = queue.Progress + "%";
    }

    /* this function will disconnect the two computers and reset the encryption keys and IVs
     * input: object sender, EventArgs e
     * output: null
     */
    void transferClient_Disconnected(object sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(new EventHandler(transferClient_Disconnected), sender, e);
            return;
        }

        //Deregister the transfer client events
        deregisterEvents();

        //Close every transfer
        foreach (ListViewItem item in lstTransfers.Items)
        {
            TransferQueue queue = (TransferQueue)item.Tag;
            queue.Close();
        }
        //Clear the listview
        lstTransfers.Items.Clear();
        progressOverall.Value = 0;

        //Set the client to null
        transferClient = null;

        //Set the connection status to nothing
        setConnectionStatus("-");

        // restoring the aes encrypt keys to null
        key = null;
        IV = null;
        firstKey = null;
        firstIV = null;
        secondKey = null;
        secondIV = null;

        //If the server is still running, wait for another connection
        if (serverRunning)
        {
            listener.Start(int.Parse(txtServerPort.Text.Trim()));
            setConnectionStatus("Waiting...");
        }
        else //If we connected then disconnected, set the text back to connect.
        {
            btnConnect.Text = "Connect";
        }
    }

    /* This just plays a little sound to let us know a transfer completed.
     * input: object sender, EventArgs e
     * output: null
     */
    void transferClient_Complete(object sender, TransferQueue queue)
    {
        System.Media.SystemSounds.Asterisk.Play();
    }



    private void deregisterEvents()
    {
        if (transferClient == null)
            return;
        transferClient.Complete -= transferClient_Complete;
        transferClient.Disconnected -= transferClient_Disconnected;
        transferClient.ProgressChanged -= transferClient_ProgressChanged;
        transferClient.Queued -= transferClient_Queued;
        transferClient.Stopped -= transferClient_Stopped;
    }

    /* This updates the connection status.
     * input: object sender, EventArgs e
     * output: null
     */
    private void setConnectionStatus(string connectedTo)
    {
        lblConnected.Text = "Connection: " + connectedTo;
    }

    /* This start the server to wait for another clien tto try to connect
     * input: object sender, EventArgs e
     * output: null
     */
    private void btnStartServer_Click(object sender, EventArgs e)
    {
        //We disabled the button, but lets just do a quick check
        if (serverRunning)
            return;
        serverRunning = true;
        try
        {
            //Try to listen on the desired port
            listener.Start(int.Parse(txtServerPort.Text.Trim()));
            //Set the connection status to waiting
            setConnectionStatus("Waiting...");
            //Enable/Disable the server buttons.
            btnStartServer.Enabled = false;
            btnStopServer.Enabled = true;
        }
        catch
        {
            MessageBox.Show("Unable to listen on port " + txtServerPort.Text, "", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }

    /* This just stop the server listen or connection.
     * input: object sender, EventArgs e
     * output: null
     */
    private void btnStopServer_Click(object sender, EventArgs e)
    {

        if (!serverRunning)
            return;

        //clearing aes key
        key = null;
        IV = null;
        firstKey = null;
        firstIV = null;
        secondKey = null;
        secondIV = null;

        //Close the client if its active.
        if (transferClient != null)
        {
            transferClient.Close();
            //INSERT
            transferClient = null;
            //
        }
        //Stop the listener
        listener.Stop();
        //Stop the timer
        tmrOverallProg.Stop();
        //Reset the connection statis
        setConnectionStatus("-");
        //Set our variables and enable/disable the buttons.
        serverRunning = false;
        btnStartServer.Enabled = true;
        btnStopServer.Enabled = false;
    }

    /* Loop and clear all complete or inactive transfers
     * input: object sender, EventArgs e
     * output: null
     */
    private void btnClearComplete_Click(object sender, EventArgs e)
    {

        foreach (ListViewItem i in lstTransfers.Items)
        {
            TransferQueue queue = (TransferQueue)i.Tag;

            if (queue.Progress == 100 || !queue.Running)
            {
                i.Remove();
            }
        }
    }

    /* Get a user defined save directory
     * input: object sender, EventArgs e
     * output: null
     */
    private void btnOpenDir_Click(object sender, EventArgs e)
    {

        using (FolderBrowserDialog fb = new FolderBrowserDialog())
        {
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputFolder = fb.SelectedPath;

                if (transferClient != null)
                {
                    transferClient.OutputFolder = outputFolder;
                }

                txtSaveDir.Text = outputFolder;
            }
        }
    }

    /* Get the user desired files to send and checks if they contain a virus and then send the file if it is not infected
     * input: object sender, EventArgs e
     * output: null
     */
    private async void btnSendFile_Click(object sender, EventArgs e)
    {
        if (transferClient == null)
            return;
        
        using (OpenFileDialog o = new OpenFileDialog())
        {
            o.Filter = "All Files (*.*)|*.*";
            o.Multiselect = true;

            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in o.FileNames)
                {
                    byte[] fileBytes = File.ReadAllBytes(file);
                    if (fileBytes.Length > 16000000)
                    {
                        MessageBox.Show("this file: " + file + ". cannot be sent beacause it is larger the 16MB!\n if you want to send file that are equals and larger than 16MB Please use file transfer option instead...");
                    }
                    else
                    {
                        resultFromScan = "";
                        //check file for virus
                        bool isInfected = false;

                        VirusTotal virusTotal = new VirusTotal("4f086d0dd58ae2502b57178ce50e4f9d7815f52b693a305b3981ecb5331ff97c");

                        //Use HTTPS instead of HTTP
                        virusTotal.UseTLS = true;

                        //Create the EICAR test virus. See http://www.eicar.org/86-0-Intended-use.html
                        //byte[] eicar = Encoding.ASCII.GetBytes(@"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*");

                        //Check if the file has been scanned before.
                        FileReport fileReport;

                        fileReport = await virusTotal.GetFileReportAsync(fileBytes);

                        //catch
                        //{
                        //    // can only scan 4 files in one minute
                        //    System.Threading.Thread.Sleep(60000);
                        //    fileReport = await virusTotal.GetFileReportAsync(fileBytes);
                        //}

                        bool hasFileBeenScannedBefore = fileReport.ResponseCode == FileReportResponseCode.Present;

                        Console.WriteLine("File has been scanned before: " + (hasFileBeenScannedBefore ? "Yes" : "No"));

                        //If the file has been scanned before, the results are embedded inside the report.
                        if (hasFileBeenScannedBefore)
                        {
                            PrintScan(fileReport);
                        }
                        else
                        {
                            if (fileBytes.Length <= 32000000)
                            {
                                ScanResult fileResult = await virusTotal.ScanFileAsync(fileBytes, Path.GetFileName(file));
                                PrintScan(fileResult);
                            }
                            else
                            {
                                // it throw exception because file size is above what is permitted
                                int i = 0;
                                while (i < fileBytes.Length)
                                {
                                    byte[] bytes = new byte[32000];
                                    Array.Copy(fileBytes, i, bytes, 0, 32000);
                                    if (i == 0)
                                    {
                                        //System.Threading.Thread.Sleep(60000);
                                    }
                                    else
                                    {
                                        System.Threading.Thread.Sleep(21000);
                                    }
                                    ScanResult fileResult = await virusTotal.ScanFileAsync(bytes, Path.GetFileName(file));

                                    PrintScan(fileResult);
                                    if (resultFromScan.Contains("True"))
                                    {
                                        i = fileBytes.Length;
                                        //break;
                                    }
                                    i = i + 32000;
                                }
                            }

                        }

                        Console.WriteLine();
                        // if it founds true more than three times that means that the file is inficted
                        //if (resultFromScan.Contains("True"))
                        if (CountStringOccurrences(resultFromScan, "True") >= 3)
                        {
                            isInfected = true;
                        }
                        else
                        {
                            isInfected = false;
                        }

                        if (!isInfected)
                        {
                            transferClient.QueueTransfer(file);
                        }
                        else
                        {
                            MessageBox.Show("this file: " + file + ". cannot be sent because it is infected!!!");
                        }
                    }
                }
            }
        }
    }
    /* puase the file data transfer
     * input: object sender, EventArgs e
     * output: null
     */
    private void btnPauseTransfer_Click(object sender, EventArgs e)
    {
        if (transferClient == null)
            return;
        //Loop and pause/resume all selected downloads.
        foreach (ListViewItem i in lstTransfers.SelectedItems)
        {
            TransferQueue queue = (TransferQueue)i.Tag;
            queue.Client.PauseTransfer(queue);
        }
    }

    /* stop and end the file data transfer
     * input: object sender, EventArgs e
     * output: null
     */
    private void btnStopTransfer_Click(object sender, EventArgs e)
    {
        if (transferClient == null)
            return;

        //Loop and stop all selected downloads.
        foreach (ListViewItem i in lstTransfers.SelectedItems)
        {
            TransferQueue queue = (TransferQueue)i.Tag;
            queue.Client.StopTransfer(queue);
            i.Remove();
        }

        progressOverall.Value = 0;
    }
    /* when form is loaded change the ip to the computer corrent ip
     * input: object sender, EventArgs e
     * output: null
     */
    private void Main_Load(object sender, EventArgs e)
    {
        //txtCntHost
        IPHostEntry host;
        string localIp = "?";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily.ToString() == "InterNetwork")
            {
                localIp = ip.ToString();
                txtCntHost.Text = localIp;
            }
        }
    }

    private static void PrintScan(ScanResult scanResult)
    {
        Console.WriteLine("Scan ID: " + scanResult.ScanId);
        Console.WriteLine("Message: " + scanResult.VerboseMsg);
        Console.WriteLine();
        resultFromScan = scanResult.ScanId + scanResult.VerboseMsg;
    }

    private static void PrintScan(FileReport fileReport)
    {
        Console.WriteLine("Scan ID: " + fileReport.ScanId);
        Console.WriteLine("Message: " + fileReport.VerboseMsg);

        resultFromScan = fileReport.ScanId + fileReport.VerboseMsg;

        if (fileReport.ResponseCode == FileReportResponseCode.Present)
        {
            foreach (KeyValuePair<string, ScanEngine> scan in fileReport.Scans)
            {
                Console.WriteLine("{0,-25} Detected: {1}", scan.Key, scan.Value.Detected);
                resultFromScan = resultFromScan + "{0,-25} Detected: {1}" + scan.Key + scan.Value.Detected;
            }
        }

        Console.WriteLine();
    }

    public int CountStringOccurrences(string text, string pattern)
    {
        // Loop through all instances of the string 'text'.
        int count = 0;
        int i = 0;
        while ((i = text.IndexOf(pattern, i)) != -1)
        {
            i += pattern.Length;
            count++;
        }
        return count;
    }
}