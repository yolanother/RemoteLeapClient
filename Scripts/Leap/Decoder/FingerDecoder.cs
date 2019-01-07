using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class FingerDecoder : BoneDecoder {
        public readonly FingerBoneDecoder[] bones = new FingerBoneDecoder[4];

        public FingerDecoder(Header header, string name) : base(header, name) {
            for (int i = 0; i < bones.Length; i++) bones[i] = new FingerBoneDecoder(this, name + "," + i);
        }

        public FingerDecoder(BoneDecoder parent, string name) : base(parent, name) {
            for (int i = 0; i < bones.Length; i++) bones[i] = new FingerBoneDecoder(this, name + "," + i);
        }

        protected override Vector3 OnDecode() {
            foreach (FingerBoneDecoder bone in bones) {
                bone.Decode();
            }
            return Vector3.zero;
        }
    }

    public class FingerBoneDecoder : BoneDecoder {
        public FingerBoneDecoder(Header header, string name) : base(header, name) { }

        public FingerBoneDecoder(BoneDecoder parent, string name) : base(parent, name) { }
    }
}
