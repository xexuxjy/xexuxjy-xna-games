﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;

namespace ModelNamer
{
    static class Common
    {
        public static bool FindCharsInStream(BinaryReader binReader, char[] charsToFind, bool resetPositionIfNotFound = false)
        {
            bool found = false;
            byte b = (byte)' ';
            int lastFoundIndex = 0;
            long currentPosition = binReader.BaseStream.Position;
            try
            {
                while (true)
                {
                    b = binReader.ReadByte();
                    if (b == charsToFind[lastFoundIndex])
                    {
                        lastFoundIndex++;
                        if (lastFoundIndex == charsToFind.Length)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        lastFoundIndex = 0;
                    }
                }
            }
            catch (Exception e)
            {
            }
            if (!found && resetPositionIfNotFound)
            {
                binReader.BaseStream.Position = currentPosition;
            }


            return found;

        }

        public static bool FuzzyEquals(float x, float y, float eps = float.Epsilon)
        {
            return Math.Abs(x - y) < eps;
        }


        public static float ReadSingleBigEndian(byte[] data, int offset)
        {
            return ReadSingle(data, offset, false);
        }
        public static float ReadSingleLittleEndian(byte[] data, int offset)
        {
            return ReadSingle(data, offset, true);
        }
        private static float ReadSingle(byte[] data, int offset, bool littleEndian)
        {
            if (BitConverter.IsLittleEndian != littleEndian)
            {   // other-endian; reverse this portion of the data (4 bytes)
                byte tmp = data[offset];
                data[offset] = data[offset + 3];
                data[offset + 3] = tmp;
                tmp = data[offset + 1];
                data[offset + 1] = data[offset + 2];
                data[offset + 2] = tmp;
            }
            return BitConverter.ToSingle(data, offset);
        }

        public static int ToInt32BigEndian(byte[] buf, int i)
        {
            return (buf[i] << 24) | (buf[i + 1] << 16) | (buf[i + 2] << 8) | buf[i + 3];
        }

        public static short ToInt16BigEndian(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            return (short)(b1 << 8 | b2);
        }

        public static int ToInt16BigEndian(byte[] buf, int i)
        {
            return (short)(buf[i] << 8 | buf[i+1]);
        }


        public static Vector3 FromStreamInt32(BinaryReader reader)
        {
            Vector3 v = new Vector3();
            v.X = reader.ReadInt32();
            v.Y = reader.ReadInt32();
            v.Z = reader.ReadInt32();
            return v;
        }

        static byte[] s_buffer = new byte[4];

        public static Vector3 FromStreamInt32BE(BinaryReader reader)
        {
            Vector3 v = new Vector3();
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.X = Common.ToInt32BigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Y = Common.ToInt32BigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Z = Common.ToInt32BigEndian(s_buffer, 0);
            return v;
        }


        public static Vector3 FromStreamVector3(BinaryReader reader)
        {
            Vector3 v = new Vector3();
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
            return v;
        }

        public static Vector3 FromStreamVector3BE(BinaryReader reader)
        {
            Vector3 v = new Vector3();
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.X = Common.ReadSingleBigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Y = Common.ReadSingleBigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Z = Common.ReadSingleBigEndian(s_buffer, 0);
            return v;
        }


        public static void WriteFloat(StreamWriter sw, Vector3 v)
        {
            sw.WriteLine(String.Format("{0:0.00000000} {1:0.00000000} {2:0.00000000}", v.X, v.Y, v.Z));
        }

        public static void WriteInt(StreamWriter sw, Vector3 v)
        {
            sw.WriteLine(String.Format("{0} {1} {2}", v.X, v.Y, v.Z));
        }



    }

    public class R2V2
    {
        public static char[] tag = new char[] { 'r', '2', 'v', '2' };


        public byte[] r2v2Tag = new byte[4];
        public int sizecounter48;  // seems to be a decrementing field that is always 0x48 different to previous/next
        public int tag201;
        public int tag0;
        public int tag1;
        public int order;		// -1 (
        public int textureNum; // which of the textures mentioned this mesh block uses.
        public byte[] nonzerostuff = new byte[28];
        public int numVertices;
        public int numFaces;
        public int val3;
        public int val4; // some sort of incrementing value.
        public int val5;
        public int val6;
        public String FileName;

        public String hexString;
        public String hexStringFormatted;
        public byte[] hex = new byte[0x50];
        public int fileSize;


        public static R2V2 FromStream(BinaryReader reader)
        {
            R2V2 r2v2 = new R2V2();
            reader.Read(r2v2.r2v2Tag, 0, r2v2.r2v2Tag.Length);
            r2v2.sizecounter48 = reader.ReadInt32();
            r2v2.tag201 = reader.ReadInt32();
            r2v2.tag0 = reader.ReadInt32();
            r2v2.tag1 = reader.ReadInt32();

            if (r2v2.tag1 != 0)
            {
                int ibreak = 0;
            }

            r2v2.order = reader.ReadInt32();
            r2v2.textureNum = reader.ReadInt32();

            reader.Read(r2v2.nonzerostuff, 0, r2v2.nonzerostuff.Length);
            r2v2.numVertices = reader.ReadInt32();
            r2v2.numFaces = reader.ReadInt32();
            r2v2.val3 = reader.ReadInt32();
            r2v2.val4 = reader.ReadInt32();
            r2v2.val5 = reader.ReadInt32();
            r2v2.val6 = reader.ReadInt32();


            reader.BaseStream.Position -= 0x50;
            reader.Read(r2v2.hex, 0, r2v2.hex.Length);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < r2v2.hex.Length; ++i)
            {
                sb.AppendFormat("{0:X02}", r2v2.hex[i]);
            }

            r2v2.hexString = sb.ToString();

            sb = new StringBuilder();
            for (int i = 0; i < r2v2.hexString.Length; ++i)
            {
                int v = i % 8;
                if (i > 0 && (v == 0))
                {
                    sb.Append(" ");
                }
                sb.Append(r2v2.hexString[i]);
            }
            r2v2.hexStringFormatted = sb.ToString();

            return r2v2;

        }
    }

}
