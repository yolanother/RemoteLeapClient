using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap.Sample {
    public class SampleButtonControl : MonoBehaviour {
        [SerializeField]
        private BoneClient boneClient;

        [SerializeField]
        private GameObject connectButton;

        [SerializeField]
        private GameObject disconnectButton;

        void Update() {
            connectButton.SetActive(!boneClient.IsConnected);
            disconnectButton.SetActive(boneClient.IsConnected);
        }
    }
}