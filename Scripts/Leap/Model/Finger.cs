using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class Finger : LeapBone {
        [SerializeField]
        private LeapBone[] bones;

        public override bool IsTracking {
            get {
                return base.IsTracking;
            }

            internal set {
                foreach (LeapBone bone in bones) {
                    bone.IsTracking = value;
                }
                base.IsTracking = value;
            }
        }

        public override void ApplyBones(BoneDecoder boneDecoder) {
            if (boneDecoder is FingerDecoder) {
                FingerDecoder fingerDecoder = boneDecoder as FingerDecoder;
                for (int i = 0; i < bones.Length && i < fingerDecoder.bones.Length; i++) {
                    bones[i].ApplyBones(fingerDecoder.bones[i]);
                }
            }
        }
    }
}
