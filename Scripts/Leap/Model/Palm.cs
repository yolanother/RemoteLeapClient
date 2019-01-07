using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class Palm : LeapBone {
        [SerializeField]
        private Finger thumb;
        [SerializeField]
        private Finger index;
        [SerializeField]
        private Finger middle;
        [SerializeField]
        private Finger ring;
        [SerializeField]
        private Finger pinky;

        public override bool IsTracking {
            get {
                return base.IsTracking;
            }

            internal set {
                if (thumb) thumb.IsTracking = value;
                if (index) index.IsTracking = value;
                if (middle) middle.IsTracking = value;
                if (ring) ring.IsTracking = value;
                if (pinky) pinky.IsTracking = value;
                base.IsTracking = value;
            }
        }

        public override void ApplyBones(BoneDecoder boneDecoder) {
           if(boneDecoder is PalmDecoder) {
                PalmDecoder palmDecoder = boneDecoder as PalmDecoder;
                palmDecoder.ApplyTransform(transform);
                if (thumb) thumb.ApplyBones(palmDecoder.thumb);
                if (index) index.ApplyBones(palmDecoder.index);
                if (middle) middle.ApplyBones(palmDecoder.middle);
                if (ring) ring.ApplyBones(palmDecoder.ring);
                if (pinky) pinky.ApplyBones(palmDecoder.pinky);
            }
        }
    }
}
