using System.Collections;
using System.Collections.Generic;
using DoubTech.Util;
using UnityEngine;

namespace DoubTech.Leap {
    public class Header {
        public BitpackedDataBuffer.BitpackedDataBufferReader reader;
        public HandPresence activeHands;
        public PrecisionBucket.Precision precision;
        public CoordinateBucket.Bucket palmCoordSize;
        public CoordinateBucket.Bucket boneCoordSize;
        public CoordinateBucket.Bucket boneDirectionSize;

        public Header(BitpackedDataBuffer.BitpackedDataBufferReader reader) {
            this.reader = reader;
            activeHands = (HandPresence)reader.ReadByte(2);
            precision = (PrecisionBucket.Precision)reader.ReadByte(2);
            palmCoordSize = (CoordinateBucket.Bucket)reader.ReadInt(3);
            boneCoordSize = (CoordinateBucket.Bucket)reader.ReadInt(3);
            boneDirectionSize = (CoordinateBucket.Bucket)reader.ReadInt(3);

            /*Debug.Log("Active Hands: " + activeHands + " [" + ((int)activeHands) + "]" +
                    ", Precision: " + precision + " [" + ((int)precision) + "]" +
                    ", palmCoordSize: " + palmCoordSize + " [" + ((int)palmCoordSize) + "]" +
                    ", boneCoordSize: " + boneCoordSize + " [" + ((int)boneCoordSize) + "]");*/
        }
    }
}
