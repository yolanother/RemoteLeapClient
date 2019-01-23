using System.Net;
using UnityEngine;

namespace DoubTech.Sockets {
    public class UdpSocket : BaseSocket {
        private System.Net.Sockets.UdpClient udpClient;
        private int port;

        public bool Connected {
            get {
                return null != udpClient;
            }
        }

        public void Connect(string server, int port) {
            udpClient = new System.Net.Sockets.UdpClient(port);
            this.port = port;
            Debug.Log("Connnencted to udp server " + server);
        }

        public void Disconnect() {
            udpClient.Close();
            udpClient = null;
        }

        public byte[] Receive() {
            //IPEndPoint object will allow us to read datagrams sent from any source.
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
            Debug.Log("Listening for incoming data on udp port " + port);
            byte[] buffer = udpClient.Receive(ref remoteEP);
            Debug.Log("Received " + buffer.Length + " bytes");
            return buffer;
        }

        public int Send(byte[] buffer) {
           return udpClient.Send(buffer, buffer.Length);
        }
    }
}
