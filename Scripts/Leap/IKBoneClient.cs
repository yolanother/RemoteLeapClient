using System.Collections;
using System.Collections.Generic;
using DoubTech.Sockets;
using UnityEngine;

namespace DoubTech.Leap {
    public class IKBoneClient : BoneClient {
        [SerializeField]
        Animator animator;

        private BaseSocket tcpSocket = new TcpSocket(2048);

        protected override BaseSocket Socket {
            get {
                return tcpSocket;
            }
        }
    }
}
