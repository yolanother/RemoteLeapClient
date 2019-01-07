using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public abstract class BoneDecoder {
        public Header header;
        private Vector3 position;
        public readonly string name;

        public BoneDecoder(Header header, string name = null) {
            this.name = name;
            this.header = header;
        }

        public BoneDecoder(BoneDecoder parent, string name = null) {
            this.name = name;
            this.header = parent.header;
        }

        public void Decode() {
            position = OnDecode();
        }

        protected virtual Vector3 OnDecode() {
            return DecodeVector(header.boneCoordSize);
        }

        public void ApplyTransform(Transform transform) {
            transform.localPosition = position;
        }

        protected float DecodeCoordinate(CoordinateBucket.Bucket coordSize) {
            bool pos = header.reader.ReadBool();
            int coord = header.reader.ReadInt(CoordinateBucket.GetBucketSize(coordSize));
            return (pos ? 1 : -1) * PrecisionBucket.ApplyPrecision(header.precision, coord);
        }

        protected Vector3 DecodeVector(CoordinateBucket.Bucket coordSize) {
            return new Vector3(
                DecodeCoordinate(coordSize),
                DecodeCoordinate(coordSize),
                DecodeCoordinate(coordSize)
            );
        }
    }
}