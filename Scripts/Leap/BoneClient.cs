using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DoubTech.Sockets;
using DoubTech.Util;
using UnityEngine;

namespace DoubTech.Leap {
    public class BoneClient : BaseClient {
        [SerializeField]
        private Hand leftHand;

        [SerializeField]
        private Hand rightHand;

        public enum Protocol {
            Tcp,
            Udp
        }

        [SerializeField]
        private Protocol protocol = Protocol.Udp;

        private BitpackedDataBuffer dataBuffer = new BitpackedDataBuffer();
        private BaseSocket tcpSocket = new TcpSocket(2048);
        private BaseSocket udpSocket = new UdpSocket();
        
        protected override BaseSocket Socket {
            get {
                return protocol == Protocol.Tcp ? tcpSocket : udpSocket;
            }
        }

        protected override void OnConnecting() {
            Debug.Log("Connecting...");
        }

        protected override void OnConnected() {
            Debug.Log("Connnected.");
        }

        protected override void OnConnectionFailed() {
            Debug.Log("Connnection failed.");
        }

        protected override void OnDisconected() {
            Debug.Log("Disconnected");
        }

        protected override bool OnBytesReceivedInBackground(byte[] buffer, int len) {
            dataBuffer.Data = buffer;
            //Debug.Log("Received " + len + " bytes.");

            DecodeHands();

            return base.OnBytesReceivedInBackground(buffer, len);
        }

        protected override bool OnSocketException(SocketException e) {
            Debug.LogError(e);
            return true;
        }

        private void DecodeHands() {
            lock (dataBuffer) {
                Header header = new Header(dataBuffer.Reader);
                PalmDecoder leftPalm = null;
                PalmDecoder rightPalm = null;

                if ((header.activeHands & HandPresence.Left) > 0) {
                    leftPalm = new PalmDecoder(header);
                    leftPalm.Decode();
                }
                if ((header.activeHands & HandPresence.Right) > 0) {
                    rightPalm = new PalmDecoder(header);
                    rightPalm.Decode();
                }

                RunOnMain(() => {
                    if (null != leftPalm) {
                        if (null != leftHand && null != leftHand.Palm) {
                            leftHand.Palm.ApplyBones(leftPalm);
                        }
                    }
                    if (null != rightPalm) {
                        if (null != rightHand && null != rightHand.Palm) {
                            rightHand.Palm.ApplyBones(rightPalm);
                        }
                    }
                });
            }
        }
    }
}