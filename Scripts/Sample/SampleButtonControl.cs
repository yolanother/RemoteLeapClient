using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoubTech.Leap.Sample {
    public class SampleButtonControl : MonoBehaviour {
        const string PREFS_KEY_HOST = "host";
        const string PREFS_KEY_PORT = "port";

        [SerializeField]
        private BoneClient boneClient;

        [SerializeField]
        private GameObject connectButton;

        [SerializeField]
        private GameObject disconnectButton;

        [SerializeField]
        private InputField host;

        [SerializeField]
        private InputField port;

        private void Start() {
            host.text = PlayerPrefs.GetString(PREFS_KEY_HOST, "localhost");
            port.text = PlayerPrefs.GetString(PREFS_KEY_PORT, "4444");
        }

        void Update() {
            connectButton.SetActive(!boneClient.IsConnected);
            disconnectButton.SetActive(boneClient.IsConnected);
        }

        public void Connect() {
            boneClient.Host = host.text;
            PlayerPrefs.SetString(PREFS_KEY_HOST, host.text);
            boneClient.Port = int.Parse(port.text);
            PlayerPrefs.SetString(PREFS_KEY_PORT, port.text);
            PlayerPrefs.Save();

            boneClient.Connect();
        }
    }
}