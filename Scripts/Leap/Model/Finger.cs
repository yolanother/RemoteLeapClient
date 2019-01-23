using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class Finger : LeapBone {
        [SerializeField]
        private LeapBone[] bones;
        [SerializeField]
        bool drawFingerLine = true;

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
                    bones[bones.Length - i - 1].ApplyBones(fingerDecoder.bones[fingerDecoder.bones.Length - i - 1]);

                    if (i > 0 && drawFingerLine) {
                        LineRenderer lineRenderer = bones[bones.Length - i - 1].LineRenderer;
                        if (null != lineRenderer) {
                            lineRenderer.enabled = true;
                            Vector3 start = bones[bones.Length - i - 1].transform.position;
                            Vector3 end = bones[bones.Length - i].transform.position;
                            lineRenderer.SetPosition(0, start);
                            lineRenderer.SetPosition(1, end);
                        }
                    }
                }
            }
        }
    }
}
