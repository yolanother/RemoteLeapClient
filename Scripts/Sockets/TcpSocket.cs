using System.Net;
using System.Net.Sockets;

namespace DoubTech.Sockets {
    public class TcpSocket : BaseSocket {
        private Socket socket;
        byte[] buffer;

        public TcpSocket(int bufferSize) {
            this.buffer = new byte[bufferSize];
        }

        public bool Connected {
            get {
                return null != socket && socket.Connected;
            }
        }


        public void Connect(string server, int port) {
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
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                tempSocket.Connect(ipe);
                if (tempSocket.Connected) {
                    socket = tempSocket;
                    break;
                }
            }
        }

        public void Disconnect() {
            if(null != socket) {
                socket.Disconnect(false);
                socket.Close();
                socket = null;
            }
        }

        public byte[] Receive() {
            int len = socket.Receive(buffer, buffer.Length, 0);
            byte[] data = new byte[len];
            System.Array.Copy(buffer, data, len);
            return data;
        }

        public int Send(byte[] buffer) {
            return socket.Send(buffer);
        }
    }
}
