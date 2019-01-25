using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public abstract class BoneDecoder {
        private Vector3 DIRECTION_FIX = new Vector3(-1, 1, 1);
        public Header header;
        protected Vector3 position;
        protected Vector3 direction;
        public readonly string name;

        public Vector3 Position {
            get {
                return position;
            }
        }

        public Vector3 Direction {
            get {
                return direction;
            }
        }

        public BoneDecoder(Header header, string name = null) {
            this.name = name;
            this.header = header;
        }

        public BoneDecoder(BoneDecoder parent, string name = null) {
            this.name = name;
            this.header = parent.header;
        }

        public virtual void Decode() {
            position = DecodeVector(header.boneCoordSize);
            direction = DecodeVector(header.boneDirectionSize);
        }

        public void ApplyTransform(Transform transform) {
            transform.localPosition = position / 100;
            //transform.rotation = Quaternion.Euler(direction);
        }

        public void ApplyGlobalRotation(Transform transform) {
            Debug.Log(direction);
            transform.rotation = Quaternion.Euler(direction);
        }

        protected float DecodeCoordinate(CoordinateBucket.Bucket coordSize) {
            bool pos = header.reader.ReadBool();
            int coord = header.reader.ReadInt(CoordinateBucket.GetBucketSize(coordSize));
            return (pos ? -1 : 1) * PrecisionBucket.ApplyPrecision(header.precision, coord);
        }

        protected Vector3 DecodeVector(CoordinateBucket.Bucket coordSize) {
            return DecodeVector(coordSize, Vector3.one);
        }

        protected Vector3 DecodeVector(CoordinateBucket.Bucket coordSize, Vector3 adjustment) {
            if (coordSize == CoordinateBucket.Bucket.Bucket_Disabled) return Vector3.zero;
            return new Vector3(
                DecodeCoordinate(coordSize) * adjustment.x,
                DecodeCoordinate(coordSize) * adjustment.y,
                DecodeCoordinate(coordSize) * adjustment.z
            );
        }
    }
}