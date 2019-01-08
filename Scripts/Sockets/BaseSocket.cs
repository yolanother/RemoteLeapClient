using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Sockets {
    public interface BaseSocket {
        bool Connected {
            get;
        }

        void Disconnect();
        void Connect(string server, int port);
        byte[] Receive();
        int Send(byte[] buffer);
    }
}
