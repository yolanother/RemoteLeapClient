using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class PalmDecoder : BoneDecoder {
        public readonly FingerDecoder thumb;
        public readonly FingerDecoder index;
        public readonly FingerDecoder middle;
        public readonly FingerDecoder ring;
        public readonly FingerDecoder pinky;

        public PalmDecoder(Header header) : base(header) {
            thumb = new FingerDecoder(this, "thumb");
            index = new FingerDecoder(this, "index");
            middle = new FingerDecoder(this, "middle");
            ring = new FingerDecoder(this, "ring");
            pinky = new FingerDecoder(this, "pinky");
        }

        public PalmDecoder(BoneDecoder parent) : base(parent) {
            thumb = new FingerDecoder(this, "thumb");
            index = new FingerDecoder(this, "index");
            middle = new FingerDecoder(this, "middle");
            ring = new FingerDecoder(this, "ring");
            pinky = new FingerDecoder(this, "pinky");
        }

        public override void Decode() {
            position = DecodeVector(header.palmCoordSize);
            direction = Quaternion.Euler(-90, 0, 0) * DecodeVector(header.boneDirectionSize);
            thumb.Decode();
            index.Decode();
            middle.Decode();
            ring.Decode();
            pinky.Decode();
        }
    }
}
