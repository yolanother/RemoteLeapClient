using System.Collections;
using System.Collections.Generic;
using DoubTech.Leap;
using UnityEngine;

public class AndroidStartup : MonoBehaviour {
    [SerializeField]
    private BoneClient boneClient;

    private void OnEnable() {
        boneClient.Connect();
    }

    private void OnDisable() {
        boneClient.Disconnect();
    }

    private void OnApplicationPause(bool pause) {
        if(pause) {
            boneClient.Disconnect();
        } else {
            boneClient.Connect();
        }
    }
}
