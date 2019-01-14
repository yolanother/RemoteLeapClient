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
        [SerializeField]
        private string host = "localhost";
        [SerializeField]
        private int port = 4444;

        private bool connected;
        private bool connecting;

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
        /// Returns true if the socket thread is running and is connected to the host
        /// </summary>
        /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected {
            get {
                return null != socketThread && socketThread.IsAlive && null != Socket && Socket.Connected;
            }
        }

        /// <summary>
        /// Returns true if the socket thread is running and is connected to the host
        /// </summary>
        /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
        public bool IsConnecting {
            get {
                return connecting;
            }
        }

        protected abstract BaseSocket Socket {
            get;
        }

        private void Listen() {
            try {
                byte[] buffer = new byte[0];
                do {
                    try {
                        buffer = Socket.Receive();
                    } catch (SocketException e) {
                        if (connected) {
                            OnListenerSocketException(e);
                        }
                    }
                    if (buffer.Length > 0) {
                        receiveBytes(buffer, buffer.Length);
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
        public void Connect() {
            if (IsConnected) return;
            if (null != Socket && Socket.Connected) Socket.Disconnect();
            socketThread = RunOnBackground(SocketMainThread);
        }

        private void ShutdownSocket() {
            connecting = false;
            RunOnMain(() => {
                if (null != Socket && Socket.Connected) {
                    connected = false;
                    Socket.Disconnect();
                    OnDisconected();
                }
            });
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
                connecting = true;
                OnConnecting();
                if (null == Socket) {
                    RunOnMain(OnConnectionFailed);
                    return;
                }
                Socket.Connect(Host, Port);
                RunOnMain(() => {
                    OnConnected();
                    connected = true;
                    connecting = false;
                    });
                // Wait until we have finished connection initialization and
                // allowed the main thread to update accordingly before we start
                // listening for data.
                while (!connected) Thread.Sleep(100);
                Listen();
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

        private void OnApplicationPause(bool pause) {
            if(pause) {
                Disconnect();
            }
        }

        private void OnDisable() {
            Disconnect();
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
                Socket.Send(buffer);
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
