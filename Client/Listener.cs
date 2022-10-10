using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
    internal delegate void SocketAcceptedHandler(object sender, SocketAcceptedEventArgs e);

    internal class SocketAcceptedEventArgs : EventArgs
    {
        // accept the connection
        public Socket Accepted
        {
            get;
            private set;
        }
        
        // address
        public IPAddress Address
        {
            get;
            private set;
        }
        // ip
        public IPEndPoint EndPoint
        {
            get;
            private set;
        }

        // this function accept the socket connection
        public SocketAcceptedEventArgs(Socket sck)
        {
            Accepted = sck;
            Address = ((IPEndPoint)sck.RemoteEndPoint).Address;
            EndPoint = (IPEndPoint)sck.RemoteEndPoint;
        }
    }

    internal class Listener
    {
        #region Variables
        private Socket _socket = null;
        private bool _running = false;
        private int _port = -1;
        #endregion

        #region Properties
        public Socket BaseSocket
        {
            get { return _socket; }
        }

        public bool Running
        {
            get { return _running; }
        }

        public int Port
        {
            get { return _port; }
        }
        #endregion

        public event SocketAcceptedHandler Accepted;

        // build the listner class
        public Listener()
        {

        }

    /* start the listing on the chosen port
     * input: int port
     * output: null
     */
    public void Start(int port)
        {
            if (_running)
                return;

            _port = port;
            _running = true;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(100);
            _socket.BeginAccept(acceptCallback, null);
        }

        /* stop the connection
         * input: null
         * output: null
         */
        public void Stop()
        {
            if (!_running)
                return;

            _running = false;
            _socket.Close();
        }

    /* accept the connection
     * input: IAsyncResult ar
     * output: null
     */
    private void acceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket sck = _socket.EndAccept(ar);

                if (Accepted != null)
                {
                    Accepted(this, new SocketAcceptedEventArgs(sck));
                }
            }
            catch
            {
            }

            if (_running)
                _socket.BeginAccept(acceptCallback, null);
        }
    }
