using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Util {
    public class BitpackedDataBuffer {
        List<byte> byteBuffer = new List<byte>();
        int bufSize = 0;
        int bitSize = 0;

        public class BitpackedDataBufferReader {
            int bitIndex = 0;
            BitpackedDataBuffer buffer;
            public BitpackedDataBufferReader(BitpackedDataBuffer buffer) {
                this.buffer = buffer;
            }

            public byte[] Read(int bitCount) {
                byte[] data = buffer.GetData(bitIndex, bitCount);
                bitIndex += bitCount;
                return data;
            }

            public int ReadInt(int bitCount) {
                int data = buffer.GetIntData(bitIndex, bitCount);
                bitIndex += bitCount;
                return data;
            }

            public byte ReadByte(byte bitCount) {
                byte data = buffer.GetByteData(bitIndex, bitCount);
                bitIndex += bitCount;
                return data;
            }

            public bool ReadBool() {
                byte data = buffer.GetByteData(bitIndex, 1);
                bitIndex++;
                return data == 1;
            }

            public void Reset() {
                bitIndex = 0;
            }
        }

        private interface ByteAccessor {
            void Set(int index, byte value);
            byte Get(int index);
        }

        private class ByteListAccessor : ByteAccessor {
            private List<byte> data;

            public ByteListAccessor(List<byte> data) {
                this.data = data;
            }

            public byte Get(int index) {
                return this.data[index];
            }

            public void Set(int index, byte value) {
                this.data[index] = value;
            }
        }

        private class ByteArrayAccessor : ByteAccessor {
            private byte[] data;

            public ByteArrayAccessor(byte[] data) {
                this.data = data;
            }

            public byte Get(int index) {
                return this.data[index];
            }

            public void Set(int index, byte value) {
                this.data[index] = value;
            }
        }

        private void bitCopyData(ByteAccessor src, ByteAccessor dst, int srcBitIndex, int dstBitIndex, int bitCount, bool clearTargetBytes) {
            int stored = 0;
            int srcByteIndex = (int)(srcBitIndex / 8.0f);
            int dstByteIndex = (int)(dstBitIndex / 8.0f);
            srcBitIndex = srcBitIndex % 8;
            dstBitIndex = dstBitIndex % 8;

            if (clearTargetBytes) dst.Set(dstByteIndex, 0);

            while (stored < bitCount) {
                int srcRemain = 8 - srcBitIndex;
                int dstRemain = 8 - dstBitIndex;
                byte toStore = (byte)Min<int>(bitCount - stored, Min(srcRemain, dstRemain));

                byte srcBits = GetBits(src.Get(srcByteIndex), (byte)srcBitIndex, toStore);
                byte result = SetBits(dst.Get(dstByteIndex), (byte)dstBitIndex, toStore, srcBits);
                dst.Set(dstByteIndex, result);
                srcBitIndex += toStore;
                dstBitIndex += toStore;
                stored += toStore;

                if (stored < bitCount) {
                    if (dstBitIndex == 8) {
                        dstByteIndex++;
                        if (clearTargetBytes) dst.Set(dstByteIndex, 0);
                        dstBitIndex = 0;
                    }

                    if (srcBitIndex == 8) {
                        srcByteIndex++;
                        srcBitIndex = 0;
                    }
                }
            }
        }

        public static byte Bitmask(byte nbits) {
            return (byte)((1 << (nbits)) - 1);
        } /* a mask of nbits 1's */

        public static byte GetBits(byte x, byte bit1, byte nbits) {
            return (byte)(((x) >> (bit1)) & (Bitmask(nbits)));
        }

        public static byte SetBits(byte x, byte bit1, byte nbits, byte val) {
            /* x:bit1...bit1+nbits-1 = val */
            if ((nbits) > 0 && (bit1) >= 0) { /* check input */
                x &= (byte)(~((Bitmask((nbits))) << (bit1))); /*set field=0's*/
                x |= (byte)(((val) & (Bitmask((nbits)))) << (bit1)); /*set field=val*/
            }
            return x;
        }

        public static T Max<T>(T x, T y) {
            return (Comparer<T>.Default.Compare(x, y) > 0) ? x : y;
        }

        public static T Min<T>(T x, T y) {
            return (Comparer<T>.Default.Compare(x, y) < 0) ? x : y;
        }

        public void SetCapacity(int bitCapacity) {
            bitSize = bitCapacity;
            bufSize = (int)Mathf.Ceil(bitCapacity / 8.0f);
            if (byteBuffer.Count < bufSize) {
                while (byteBuffer.Count < bufSize) {
                    byteBuffer.Add(0);
                }
            }
        }

        public byte[] Data {
            get {
                return byteBuffer.ToArray();
            }
            set {
                byteBuffer.Clear();
                byteBuffer.AddRange(value);
                bufSize = value.Length;
                bitSize = bufSize * 8;
            }
        }

        public int Size {
            get {
                return bufSize;
            }
        }

        public BitpackedDataBufferReader Reader {
            get {
                return new BitpackedDataBufferReader(this);
            }
        }

        public void Reset() {
            bitSize = 0;
        }

        public void AddData(int bitSize, byte[] data) {
            int bitIndex = this.bitSize;
            SetCapacity(bitIndex + bitSize);
            SetData(bitIndex, bitSize, data);
        }

        public void SetData(int bitIndex, int bitSize, byte[] data) {
            bitCopyData(new ByteArrayAccessor(data), new ByteListAccessor(byteBuffer), 0, bitIndex, bitSize, false);
        }

        private static int ByteSize(int bitSize) {
            return (int)Math.Ceiling(bitSize / 8.0f);
        }

        public byte[] GetData(int bitIndex, int bitSize) {
            byte[] data = new byte[ByteSize(bitSize)];
            ByteAccessor src = new ByteListAccessor(byteBuffer);
            ByteAccessor dst = new ByteArrayAccessor(data);
            bitCopyData(src, dst, bitIndex, 0, bitSize, true);
            return data;
        }

        public byte GetByteData(int bitIndex, int bitSize) {
            byte[] data = GetData(bitIndex, bitSize);
            return data[0];
        }

        private byte[] PadData(byte[] data, int length, byte padValue = 0) {
            if(data.Length < length) {
                byte[] buffer = new byte[length];
                for(int i = 0; i < buffer.Length; i++) {
                    if(i < data.Length) {
                        buffer[i] = data[i];
                    } else {
                        buffer[i] = padValue;
                    }
                }
                return buffer;
            }

            return data;
        }

        public int GetIntData(int bitIndex, int bitSize) {
            byte[] data = GetData(bitIndex, bitSize);
            return BitConverter.ToInt32(PadData(data, 4), 0);
        }
    }
}
