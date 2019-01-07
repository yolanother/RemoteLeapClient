using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using DoubTech.Util;

namespace DoubTech.Sockets {
    public abstract class BaseClient : BackgroundableMonoBehavior {
        public delegate void SocketSendFailedLister(Exception e);

        private Thread socketThread;
        private Socket socket;
        [SerializeField]
        private string host = "localhost";
        [SerializeField]
        private int port = 4444;
        [SerializeField]
        private ProtocolType protocol = ProtocolType.Tcp;
        [SerializeField]
        private int bufferSize = 2048;

        private bool connected;

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// 
        /// NOTE: If connected this will take effect after the next block of data
        /// is received.
        /// </summary>
        /// <value>The size of the buffer.</value>
        public int BufferSize {
            get {
                return bufferSize;
            }
            set {
                bufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host {
            get {
                return host;
            }
            set {
                if (IsConnected) throw new ArgumentException("Cannot set host while connected.");
                host = value;
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// 
        /// </summary>
        /// <value>The port.</value>
        public int Port {
            get {
                return port;
            }
            set {
                if (IsConnected) throw new ArgumentException("Cannot set port while connected.");
                port = value;
            }
        }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        public ProtocolType Protocol {
            get {
                return protocol;
            }
            set {
                if (IsConnected) throw new ArgumentException("Cannot set protocol while connected.");
                protocol = value;
            }
        }

        /// <summary>
        /// Returns true if the socket thread is running and is connected to the host
        /// </summary>
        /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected {
            get {
                return null != socketThread && socketThread.IsAlive && socket.Connected;
            }
        }

        private static Socket ConnectSocket(string server, int port, ProtocolType protocol) {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList) {
                if (address.AddressFamily != AddressFamily.InterNetwork) continue;
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, protocol);
                tempSocket.Connect(ipe);
                if (tempSocket.Connected) {
                    s = tempSocket;
                    break;
                }
            }
            return s;
        }

        private void Listen(Socket socket) {
            try {
                byte[] buffer = new byte[BufferSize];
                int bytes = 0;
                do {
                    try {
                        bytes = socket.Receive(buffer, buffer.Length, 0);
                    } catch (SocketException e) {
                        if (connected) {
                            OnListenerSocketException(e);
                        }
                    }
                    if (bytes > 0) {
                        receiveBytes(buffer, bytes);
                        if (bufferSize != buffer.Length) {
                            buffer = new byte[BufferSize];
                        }
                    }
                }
                while (connected);
            } catch (ThreadAbortException) {
                // If thread was aborted we're shutting down. Do nothing.
            }
        }

        private void receiveBytes(byte[] buffer, int len) {
            if (!OnBytesReceivedInBackground(buffer, len)) {
                RunOnMain(() => { OnBytesReceived(buffer, len); });
            }
        }

        /// <summary>
        /// Connect to the host and port defined by <see cref="Host"/>
        /// </summary>
        public void Connnect() {
            if (IsConnected) return;
            if (null != socket && socket.Connected) socket.Disconnect(true);
            socketThread = RunOnBackground(SocketMainThread);
        }

        private void ShutdownSocket() {
            if (null != socket && socket.Connected) {
                connected = false;
                socket.Disconnect(true);
                socket.Close();
                RunOnMain(OnDisconected);
            }
            if(socketThread != null) {
                if(Thread.CurrentThread != socketThread && socketThread.IsAlive) {
                    socketThread.Abort();
                }
                socketThread = null;
            }
        }

        /// <summary>
        /// Disconnect from the server and shut down the client thread.
        /// </summary>
        public void Disconnect() {
            ShutdownSocket();
        }

        private void SocketMainThread() {
            try {
                connected = false;
                OnConnecting();
                socket = ConnectSocket(Host, Port, Protocol);
                if (null == socket) {
                    RunOnMain(OnConnectionFailed);
                    return;
                }
                RunOnMain(() => { 
                    OnConnected();
                    connected = true;
                    });
                // Wait until we have finished connection initialization and
                // allowed the main thread to update accordingly before we start
                // listening for data.
                while (!connected) Thread.Sleep(100);
                Listen(socket);
            } catch (SocketException e) {
                RunOnMain(() => {
                    if (!OnSocketException(e)) {
                        ShutdownSocket();
                    }
                });
            } catch (ThreadInterruptedException) {
                // Thread was aborted shutdown.
                ShutdownSocket();
            } catch (Exception e) {
                ShutdownSocket();
                // Since this was run with RunOnBackground the BackgroundMonoBehavior's
                // unhandled exception hanndler will be used.
                throw e;
            }
        }

        /// <summary>
        /// Send the specified buffer and background.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="background">If set to <c>true</c> this data will be sent on its own background thread.</param>
        /// <param name="failedLister">Callback to handle exceptions thrown by Send. If null exceptions will be thrown on whatever thread this is run from.</param>
        protected void Send(byte[] buffer, bool background = false, SocketSendFailedLister failedLister = null) {
            if (!IsConnected) {
                if (background) {
                    RunOnBackground(() => { DoSend(buffer, failedLister); });
                } else {
                    DoSend(buffer, failedLister);
                }
            }
        }

        private void DoSend(byte[] buffer, SocketSendFailedLister failedListener = null) {
            try {
                socket.Send(buffer);
            } catch (ThreadAbortException) {
                // Do nothing, thread was shutdown
            } catch (Exception e) {
                if (null != failedListener) {
                    RunOnMain(() => { failedListener(e); });
                } else {
                    throw e;
                }
            }
        }

        protected abstract void OnConnecting();
        protected abstract void OnConnectionFailed();
        protected abstract void OnDisconected();
        protected abstract void OnConnected();

        /// <summary>
        /// Called when there is a socket exception
        /// </summary>
        /// <returns><c>true</c>, if the exception was handled and the socket can remain connected, <c>false</c> otherwise disconnect and shutdown.</returns>
        /// <param name="e">E.</param>
        protected virtual bool OnSocketException(SocketException e) {
            return false;
        }

        /// <summary>
        /// Called when there is a socket exception
        /// </summary>
        /// <returns><c>true</c>, if the exception was handled and the socket can remain connected, <c>false</c> otherwise disconnect and shutdown.</returns>
        /// <param name="e">E.</param>
        protected virtual void OnListenerSocketException(SocketException e) {

        }

        /// <summary>
        /// Called on the background thread when bytes are received from a socket.
        /// This will block on the socket receive thread.
        /// </summary>
        /// <returns><c>true</c>, if bytes received in background was oned, <c>false</c> otherwise.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="len">Length.</param>
        protected virtual bool OnBytesReceivedInBackground(byte[] buffer, int len) {
            return false;
        }

        /// <summary>
        /// Called on the main thread when bytes are received from a socket if they
        /// were not processed and consumed by OnBytesReceivedInBackground
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="len">Length.</param>
        protected virtual void OnBytesReceived(byte[] buffer, int len) {
            // Optionally abstract. Override if foreground processing is needed
        }
    }
}
