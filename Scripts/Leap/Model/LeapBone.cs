using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class LeapBone : MonoBehaviour {
        private bool isTracking;

        public virtual bool IsTracking {
            get {
                return isTracking;
            } internal set {
                isTracking = value;
            }
        }

        public virtual void ApplyBones(BoneDecoder boneDecoder) {
            boneDecoder.ApplyTransform(transform);
        }
    }
}
