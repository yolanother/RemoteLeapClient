using System.Net;
using UnityEngine;

namespace DoubTech.Sockets {
    public class UdpSocket : BaseSocket {
        private System.Net.Sockets.UdpClient udpClient;

        public bool Connected {
            get {
                return null != udpClient;
            }
        }

        public void Connect(string server, int port) {
            udpClient = new System.Net.Sockets.UdpClient(port);
            udpClient.Connect(server, port);
            Debug.Log("Connnencted to udp server " + server);
        }

        public void Disconnect() {
            udpClient.Close();
            udpClient = null;
        }

        public byte[] Receive() {
            //IPEndPoint object will allow us to read datagrams sent from any source.
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = udpClient.Receive(ref remoteEP);
            Debug.Log("Received " + buffer.Length + " bytes");
            return buffer;
        }

        public int Send(byte[] buffer) {
           return udpClient.Send(buffer, buffer.Length);
        }
    }
}
