using AntlrParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SkeletonReader
    {
    }

    public static class CommonModelImporter
    {
        public static string s_rootPath;



        public static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
        public static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
        public static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' }; // External link information? referes to textures, other models, entities and so on? 
        public static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
        public static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
        public static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
        public static char[] dslsTag = new char[] { 'D', 'S', 'L', 'S' };   // DisplayList information - int16 pairs of position,normal,uv0 - possible that some uv's also used for weights?
        public static char[] dsliTag = new char[] { 'D', 'S', 'L', 'I' };   // display list -offsets and lengths
        public static char[] dslcTag = new char[] { 'D', 'S', 'L', 'C' };   // seems to contain the number of display lists and then bytes at 01 to say used?
        public static char[] posiTag = new char[] { 'P', 'O', 'S', 'I' };
        public static char[] normTag = new char[] { 'N', 'O', 'R', 'M' };
        public static char[] uv0Tag = new char[] { 'U', 'V', '0', ' ' };    // for non-skinned models, this seems to be 2 bigendian float32's
        public static char[] uv1Tag = new char[] { 'U', 'V', '1', ' ' };    // for non-skinned models, this seems to be 2 bigendian float32's

        public static char[] vflaTag = new char[] { 'V', 'F', 'L', 'A' };
        public static char[] ramTag = new char[] { 'R', 'A', 'M', ' ' };
        public static char[] msarTag = new char[] { 'M', 'S', 'A', 'R' };
        public static char[] nlvlTag = new char[] { 'N', 'L', 'V', 'L' };
        public static char[] meshTag = new char[] { 'M', 'E', 'S', 'H' };   // how many mesh segments exist. each block is 24 bytes
        public static char[] elemTag = new char[] { 'E', 'L', 'E', 'M' };   // how many elements , matches mesh segments, each block is 8 bytes,
        public static char[] skelTag = new char[] { 'S', 'K', 'E', 'L' };
        public static char[] skinTag = new char[] { 'S', 'K', 'I', 'N' };
        public static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' };
        public static char[] vflgTag = new char[] { 'V', 'F', 'L', 'G' };
        public static char[] stypTag = new char[] { 'S', 'T', 'Y', 'P' };
        public static char[] pak1Tag = new char[] { 'P', 'A', 'K', '1' };
        public static char[] jlodTag = new char[] { 'J', 'L', 'O', 'D' };
        public static char[] nmptTag = new char[] { 'N', 'M', 'P', 'T' };
        public static char[] pttpTag = new char[] { 'P', 'T', 'T', 'P' };
        public static char[] r2d2Tag = new char[] { 'R', '2', 'D', '2', 'p', 's', 'x', '2' };
        public static char[] pfhdTag = new char[] { 'P', 'F', 'H', 'D' };
        public static char[] ptdtTag = new char[] { 'P', 'T', 'D', 'T' };
        public static char[] tmapTag = new char[] { 't', 'm', 'a', 'p' };

        public static char[] xrndTag = new char[] { 'X', 'R', 'N', 'D' };
        public static char[] doegTag = new char[] { 'd', 'o', 'e', 'g' };
        public static char[] endTag = new char[] { 'E', 'N', 'D', (char)0x2E };
        public static char[] obbtTag = new char[] { 'O', 'B', 'B', 'T' };
        //public static char[] endTag = new char[] { (char)0x3F,'E', 'N', 'D'};


        public static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag,
                                      dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag, vflaTag,
                                      ramTag, msarTag, nlvlTag, meshTag, elemTag, skelTag, skinTag,
                                      vflgTag,stypTag,nameTag };

        public static char[][] xboxTags = { versTag, cprtTag, selsTag, txtrTag, xrndTag };


        public static int CountCharsInStream(BinaryReader binReader, char[] charsToFind)
        {
            int count = 0;
            try
            {
                while (binReader.BaseStream.Position < binReader.BaseStream.Length)
                {
                    if (FindCharsInStream(binReader, charsToFind, false))
                    {
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return count;
        }

        public static float FromStream2ByteToFloatR(BinaryReader reader)
        {
            float floatpart = (((float)reader.ReadByte()) / 255.0f);
            float fixedpart = ByteToFloat(reader.ReadByte());

            return fixedpart + floatpart;
        }

        public static float FromStream2ByteToFloat(BinaryReader reader)
        {
            float fixedpart = ByteToFloat(reader.ReadByte());
            float floatpart = (((float)reader.ReadByte()) / 255.0f);

            return fixedpart + floatpart;
        }



        public static String ToStringC(IndexedMatrix m)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0:0.00000000}, {1:0.00000000}, {2:0.00000000}, {3:0.00000000}, ", m.M11, m.M12, m.M13, m.M14));
            sb.Append(String.Format("{0:0.00000000}, {1:0.00000000}, {2:0.00000000}, {3:0.00000000}, ", m.M21, m.M22, m.M23, m.M24));
            sb.Append(String.Format("{0:0.00000000}, {1:0.00000000}, {2:0.00000000}, {3:0.00000000}, ", m.M31, m.M32, m.M33, m.M34));
            sb.Append(String.Format("{0:0.00000000}, {1:0.00000000}, {2:0.00000000}, {3:0.00000000} ", m.M41, m.M42, m.M43, m.M44));
            return sb.ToString();
        }


        public static String ToStringC(IndexedVector4 v4)
        {
            return String.Format("{0:0.00000000}, {1:0.00000000}, {2:0.00000000}, {3:0.00000000}", v4.X, v4.Y, v4.Z, v4.W);
        }


        public static String ToString(IndexedVector4 v4)
        {
            return String.Format("{0:0.00000000} {1:0.00000000} {2:0.00000000} {3:0.00000000}", v4.X, v4.Y, v4.Z, v4.W);
        }

        public static String ToStringC(IndexedVector3 v3)
        {
            return String.Format("{0:0.00000000}, {1:0.00000000}, {2:0.00000000}", v3.X, v3.Y, v3.Z);
        }


        public static String ToString(IndexedVector3 v3)
        {
            return String.Format("{0:0.00000000} {1:0.00000000} {2:0.00000000}", v3.X, v3.Y, v3.Z);
        }

        public static String ToString(IndexedVector2 v2)
        {
            return String.Format("{0:0.00000000} {1:0.00000000}", v2.X, v2.Y);
        }

        public static String ToStringFA(float[] fa)
        {
            StringBuilder sb = new StringBuilder();
            foreach (float f in fa)
            {
                sb.AppendFormat("{0:0.00000000},");
            }
            return sb.ToString();
        }

        public static bool PositionAtFloat(BinaryReader binReader, float val, bool skipBack = true)
        {
            long startPosition = binReader.BaseStream.Position;
            bool found = false;
            while (binReader.BaseStream.Position < binReader.BaseStream.Length - 4)
            {
                float sval = binReader.ReadSingle();
                if (val == sval)
                {
                    found = true;
                    if (skipBack)
                    {
                        binReader.BaseStream.Position -= 4;
                    }
                    break;
                }
            }
            if (!found)
            {
                binReader.BaseStream.Position = startPosition;
                int ibreak = 0;
            }
            return found;

        }

        public static bool PositionAtFloats(BinaryReader binReader, float[] vals, bool skipBack = true)
        {
            long startPosition = binReader.BaseStream.Position;
            bool found = false;
            int count = 0;
            while (binReader.BaseStream.Position < binReader.BaseStream.Length - 4)
            {
                float sval = binReader.ReadSingle();
                if (sval == vals[count])
                {
                    count++;
                    if (count == vals.Length)
                    {
                        found = true;
                        if (skipBack)
                        {
                            binReader.BaseStream.Position -= 4 * vals.Length;
                        }
                        break;
                    }
                }
                else
                {
                    count = 0;
                    binReader.BaseStream.Position -= ((count - 1) * 4);
                }

            }
            if (!found)
            {
                binReader.BaseStream.Position = startPosition;
                int ibreak = 0;
            }
            return found;

        }




        public static bool PositionAtInt(BinaryReader binReader, int val, bool skipBack = true)
        {
            bool found = false;
            long startPosition = binReader.BaseStream.Position;
            while (binReader.BaseStream.Position < binReader.BaseStream.Length - 4)
            {
                int sval = binReader.ReadInt32();
                if (val == sval)
                {
                    found = true;
                    if (skipBack)
                    {
                        binReader.BaseStream.Position -= 4;
                    }
                    break;
                }
            }
            if (!found)
            {
                binReader.BaseStream.Position = startPosition;
                int ibreak = 0;
            }
            return found;
        }


        public static bool FindCharsInStream(BinaryReader binReader, char[] charsToFind, bool resetPositionIfNotFound = true)
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

        public static bool FindCharsInStream(BinaryReader binReader, char[] charsToFind, long endRange, bool resetPositionIfNotFound = true)
        {
            bool found = false;
            byte b = (byte)' ';
            int lastFoundIndex = 0;
            long currentPosition = binReader.BaseStream.Position;
            try
            {
                while (true)
                {
                    if (binReader.BaseStream.Position >= endRange)
                    {
                        found = false;
                        break;
                    }
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



        public static bool FindCharsInStream(BinaryReader binReader, byte[] charsToFind, bool resetPositionIfNotFound = true)
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
                        if (lastFoundIndex > 2)
                        {
                            int ibreak = 0;
                        }
                        if (lastFoundIndex == charsToFind.Length)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        binReader.BaseStream.Position -= lastFoundIndex;
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

        public static ushort ToUInt16BigEndian(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            return (ushort)(b1 << 8 | b2);
        }

        public static ushort ToUInt24BigEndian(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            byte b3 = reader.ReadByte();
            return (ushort)(b1 << 16 | b2 << 8 | b3);
        }

        //public static ush

        public static float ToFloatUInt16BigEndian(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            ushort val = (ushort)(b1 << 8 | b2);
            float result = (val / (float)(UInt16.MaxValue));
            return result;
        }

        public static float ToFloatUInt16(BinaryReader reader)
        {
            ushort val = reader.ReadUInt16();
            float result = (val / (float)(UInt16.MaxValue));
            return result;
        }

        //public static float ToHalfFloatInt16(BinaryReader reader)
        //{
        //    ushort val = reader.ReadUInt16();
        //    HalfSingle hs = new HalfSingle();
        //    hs.PackedValue = val;
        //    return hs.ToSingle();
        //}

        public static float ToFloatInt16(BinaryReader reader)
        {

            short val = reader.ReadInt16();
            float result = (val / (float)(Int16.MaxValue));
            return result;
        }

        public static float ToFloatInt16BigEndian(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            short val = (short)(b1 << 8 | b2);

            float result = (val / (float)(Int16.MaxValue));
            return result;
        }

        public static string ByteArrayToString(byte[] ba, int spaceCount = 0)
        {
            if (ba != null)
            {
                StringBuilder hex = new StringBuilder(ba.Length * 2);
                int counter = 0;
                foreach (byte b in ba)
                {
                    hex.AppendFormat("{0:X2}", b);
                    counter++;
                    if (spaceCount > 0 && counter % spaceCount == 0)
                    {
                        hex.Append(" ");
                    }
                }
                return hex.ToString();
            }
            return "";
        }

        public static string ByteArrayToStringSub(byte[] ba, int start, int length)
        {
            int total = start + length;
            if (ba != null && (total < ba.Length))
            {
                StringBuilder hex = new StringBuilder(length * 2);
                int counter = 0;

                for (int i = start; i < total; ++i)
                {
                    hex.AppendFormat("{0:X2}", ba[i]);
                }
                return hex.ToString();
            }
            return "";
        }




        public static int ToInt16BigEndian(byte[] buf, int i)
        {
            return (short)(buf[i] << 8 | buf[i + 1]);
        }

        public static int ReadInt32BigEndian(BinaryReader reader)
        {
            reader.Read(s_buffer, 0, s_buffer.Length);
            return Common.ToInt32BigEndian(s_buffer, 0);
        }



        public static IndexedVector3 FromStreamInt32(BinaryReader reader)
        {
            IndexedVector3 v = new IndexedVector3();
            v.X = reader.ReadInt32();
            v.Y = reader.ReadInt32();
            v.Z = reader.ReadInt32();
            return v;
        }

        static byte[] s_buffer = new byte[4];

        public static IndexedVector3 FromStreamInt32BE(BinaryReader reader)
        {
            IndexedVector3 v = new IndexedVector3();
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.X = Common.ToInt32BigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Y = Common.ToInt32BigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Z = Common.ToInt32BigEndian(s_buffer, 0);
            return v;
        }


        public static IndexedVector3 FromStreamVector3(BinaryReader reader)
        {
            IndexedVector3 v = new IndexedVector3();
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
            return v;
        }

        public static IndexedVector2 FromStreamVector2(BinaryReader reader)
        {
            IndexedVector2 v = new IndexedVector2();
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            return v;
        }


        public static IndexedQuaternion FromStreamQuaternion(BinaryReader reader)
        {
            IndexedQuaternion q = new IndexedQuaternion();
            q.X = reader.ReadSingle();
            q.Y = reader.ReadSingle();
            q.Z = reader.ReadSingle();
            q.W = reader.ReadSingle();
            return q;
        }


        public static float ByteToFloat(byte b)
        {
            int val = (int)b;
            if (val > 127)
            {
                val = -256 + val;
            }
            return ((float)val);
        }


        public static float FromStream2ByteToFloatU(BinaryReader reader)
        {
            //float v = ToFloatUInt16(reader);

            float v = ToFloatUInt16(reader);
            return v;
            //float fixedpart = (float)(reader.ReadByte());
            //float floatpart = (((float)reader.ReadByte()) / 255.0f);

            //return fixedpart + floatpart;
        }


        //public static float FromStream2ByteToFloatR(BinaryReader reader)
        //{
        //    float floatpart = (((float)reader.ReadByte()) / 255.0f);
        //    float fixedpart = ByteToFloat(reader.ReadByte());

        //    return fixedpart + floatpart;
        //}

        //public static float FromStream2ByteToFloat(BinaryReader reader)
        //{
        //    float fixedpart = ByteToFloat(reader.ReadByte());
        //    float floatpart = (((float)reader.ReadByte()) / 255.0f);

        //    return fixedpart + floatpart;

        //    //short s = reader.ReadInt16();
        //    //fixed / 65536.0
        //    //return ((float)s / 65536.0f);
        //    //byte b1 = reader.ReadByte();
        //    //byte b2 = reader.ReadByte();

        //    //int val = (int)b;
        //    //if (val > 127)
        //    //{
        //    //    val = -256 + val;
        //    //}
        //    //return ((float)val);
        //}




        public static IndexedVector3 FromStreamVector3BE(BinaryReader reader)
        {
            IndexedVector3 v = new IndexedVector3();
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.X = Common.ReadSingleBigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Y = Common.ReadSingleBigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Z = Common.ReadSingleBigEndian(s_buffer, 0);
            return v;
        }

        public static IndexedVector2 FromStreamVector2BE(BinaryReader reader)
        {
            IndexedVector2 v = new IndexedVector2();
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.X = Common.ReadSingleBigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Y = Common.ReadSingleBigEndian(s_buffer, 0);
            return v;
        }


        public static void WriteFloat(StreamWriter sw, IndexedVector3 v)
        {
            sw.WriteLine(String.Format("{0:0.00000000} {1:0.00000000} {2:0.00000000}", v.X, v.Y, v.Z));
        }

        public static void WriteInt(StreamWriter sw, IndexedVector3 v)
        {
            sw.WriteLine(String.Format("{0} {1} {2}", v.X, v.Y, v.Z));
        }


        public static void ReadNullSeparatedNames(BinaryReader binReader, char[] tagName, List<String> selsNames)
        {
            if (Common.FindCharsInStream(binReader, tagName))
            {
                int selsSectionLength = binReader.ReadInt32();

                int pad = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();

                selsSectionLength -= 16;


                char b;
                int count = 0;
                while (count < selsSectionLength)
                {
                    StringBuilder sb = new StringBuilder();
                    while ((b = (char)binReader.ReadByte()) != 0x00)
                    {
                        count++;
                        sb.Append(b);
                    }
                    count++;
                    if (sb.Length > 0)
                    {
                        selsNames.Add(sb.ToString());
                    }
                }
            }
        }

        public static void ReadNullSeparatedNames(BinaryReader binReader, long position, int numAnims, List<String> selsNames)
        {
            binReader.BaseStream.Position = position;
            char b;
            int count = 0;
            while (selsNames.Count < numAnims)
            {
                StringBuilder sb = new StringBuilder();
                while ((b = (char)binReader.ReadByte()) != 0x00)
                {
                    count++;
                    sb.Append(b);
                }
                count++;
                if (sb.Length > 0)
                {
                    selsNames.Add(sb.ToString());
                }
            }
        }



        public static void ReadTextureNames(BinaryReader binReader, char[] tagName, List<String> textureNames, bool stripExtension = false)
        {
            if (Common.FindCharsInStream(binReader, tagName, true))
            {
                int dslsSectionLength = binReader.ReadInt32();
                int uk2a = binReader.ReadInt32();
                int numTextures = binReader.ReadInt32();
                int textureSlotSize = 0x98;

                for (int i = 0; i < numTextures; ++i)
                {
                    StringBuilder sb = new StringBuilder();
                    int count = 0;
                    char b;
                    while ((b = (char)binReader.ReadByte()) != 0x00)
                    {
                        count++;
                        sb.Append(b);
                    }
                    count++;

                    String textureName = sb.ToString();
                    if (stripExtension)
                    {
                        textureName = textureName.Substring(0, textureName.IndexOf('.'));
                    }
                    textureNames.Add(textureName);
                    binReader.BaseStream.Position += (textureSlotSize - count);

                }
            }
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
        public int order;       // -1 (
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

    public class BoneNode
    {
        public static float BoneOffsetScale = 1f;

        public static HashSet<byte> pad1Bytes = new HashSet<byte>();
        public static Dictionary<byte, int> pad1ByteCount = new Dictionary<byte, int>();
        public static Dictionary<byte, List<string>> pad1ByteNames = new Dictionary<byte, List<string>>();
        public String name;
        public IndexedVector3 offset;
        public IndexedQuaternion rotation;

        // seems to signify 'stuff' ;)  1 == root/parents,2== general skeleton, 3== mount points
        public byte flags;
        public byte id;
        public byte id2;
        public byte parentId;

        public String DummyName
        {
            get; set;
        }

        public string UniqueName
        {
            get
            {
                if (name.StartsWith("mp"))
                {
                    return name;
                }
                return name + "--" + id + "--" + flags;
            }
        }


        //public Matrix rotationMatrix;
        //public Matrix combinedMatrix;
        public IndexedMatrix finalMatrix;
        //public Matrix localMatrix;
        public IndexedMatrix frameMatrix;

        public IndexedMatrix bindPoseMatrix;

        public BoneNode parent;
        public BoneNode zeroBone;
        public List<BoneNode> children = new List<BoneNode>();

        public IndexedVector3 FramePosition;
        public IndexedQuaternion FrameRotation;
        public IndexedVector4 AngleAxis;


        public const int kDontTranslate = 1;
        public const int kDontRotate = 2;
        public const int kUseRootForParent = 4;

        public void ResetFrameValues()
        {
            FramePosition = new IndexedVector3();
            FrameRotation = IndexedQuaternion.Identity;
        }
    public static IndexedVector4 QuaternionToAA(IndexedQuaternion q)
    {
        IndexedVector4 result = new IndexedVector4();
        double angle = 2 * Math.Acos(q.W);
        double s = Math.Sqrt(1 - q.W * q.W);
        result.W = (float)angle;
        if (s < 0.001)
        {
            result.X = q.X;
            result.Y = q.Y;
            result.Z = q.Z;
        }
        else
        {
            result.X = q.X / (float)s;
            result.Y = q.Y / (float)s;
            result.Z = q.Z / (float)s;
        }
        return result;
    }

    public void ApplyFrameValues()
        {
            IndexedMatrix m = IndexedMatrix.CreateFromQuaternion(FrameRotation);
            m._origin = FramePosition;

            //if (zeroBone != null)
            //{
            //    m.Translation -= zeroBone.offset;
            //}


            //frameMatrix = localMatrix * m;
            //frameMatrix = m * localMatrix;
            frameMatrix = m;
        }


        public static BoneNode FromStream(BinaryReader binReader, bool swapLeftRight)
        {
            BoneNode node = new BoneNode();
            node.offset = Common.FromStreamVector3(binReader);
            node.offset *= BoneOffsetScale;

            if (swapLeftRight)
            {
                node.offset = new IndexedVector3(-node.offset.X, node.offset.Y, node.offset.Z);
            }

            node.rotation = Common.FromStreamQuaternion(binReader);
            node.AngleAxis = QuaternionToAA(node.rotation);
            AdjustQuaternion(ref node.rotation);





        //Debug.Assert(Common.FuzzyEquals(node.rotation.Length(), 1.0f, 0.0001f));


        // look at range of values here. - still need to know why nodes are 'out'
        node.flags = binReader.ReadByte();
            pad1Bytes.Add(node.flags);
            int byteCount = 0;
            if (!pad1ByteCount.TryGetValue(node.flags, out byteCount))
            {
                pad1ByteCount[node.flags] = 0;
            }
            byteCount++;
            pad1ByteCount[node.flags] = byteCount;

            node.id = binReader.ReadByte();
            node.id2 = binReader.ReadByte();
            node.parentId = binReader.ReadByte();

            IndexedMatrix m = IndexedMatrix.CreateFromQuaternion(node.rotation);
            m._origin = node.offset;
            node.bindPoseMatrix = m;

            node.frameMatrix = IndexedMatrix.Identity;


            return node;
        }

        public String ToString()
        {
        String result = String.Format("N[{0}]\t\t id1[{1}] ", name, id);
        result += String.Format("pos[{0:0.0000},{1:0.0000},{2:0.0000}] rot[{3:0.0000} ,{4:0.0000} ,{5:0.0000} ,{6:0.0000}]", offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z, rotation.W);
        //result += String.Format("AA[{0:0.0000} ,{1:0.0000} ,{2:0.0000} ,{3:0.0000}]", AngleAxis.X, AngleAxis.Y, AngleAxis.Z, AngleAxis.W);
        result += String.Format("AA[{0:0.0000} ,{1:0.0000} ,{2:0.0000} ,{3:0.0000}]", RadianToDegree(AngleAxis.X), RadianToDegree(AngleAxis.Y), RadianToDegree(AngleAxis.Z), RadianToDegree(AngleAxis.W));

        return result;
        }
        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        public static void AdjustQuaternion(ref IndexedQuaternion q)
        {
            q = new IndexedQuaternion(q.X, q.Y, q.Z, -q.W);
        }

}


public class BaseModel
    {
        public BaseModel(string name)
        {
            m_name = name;
        }

        public virtual void LoadData(BinaryReader binReader) { }
        public virtual void BuildBB() { }
        public virtual void Validate() { }


        public TextureData FindTextureData(ShaderData sd)
        {
            int index = sd.textureId1;
            if (index == 255)
            {
                index = 0;
            }
            TextureData result = m_textures[index];//.textureName;
            if (result.textureName.Contains("skygold"))
            {
                index = sd.textureId2;
            }

            if (index == 255)
            {
                index = 0;
            }

            result = m_textures[index];

            return result;
        }


        public void ReadTextureSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.txtrTag))
            {
                int blocksize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int numElements = binReader.ReadInt32();
                for (int i = 0; i < numElements; ++i)
                {
                    TextureData textureData = TextureData.FromStream(binReader);
                    //if(!textureData.textureName.Contains("skygold"))
                    {
                        m_textures.Add(textureData);
                    }
                }
            }

        }


        public void DumpSections(String fileOutputDir)
        {
            fileOutputDir += "-tag-output";
            foreach (char[] tag in m_tagSizes.Keys)
            {
                try
                {
                    TagSizeAndData tsad = m_tagSizes[tag];
                    if (tsad.length > 0)
                    {

                        String tagOutputDirname = fileOutputDir + "/" + m_name + "/";
                        try
                        {
                            Directory.CreateDirectory(tagOutputDirname);
                        }
                        catch (Exception e) { }

                        String tagOutputFilename = tagOutputDirname + "/" + new String(tag);
                        using (System.IO.BinaryWriter outStream = new BinaryWriter(File.Open(tagOutputFilename, FileMode.Create)))
                        {
                            outStream.Write(tsad.data);
                        }
                    }
                }
                catch (Exception e)
                { }
            }
        }


        private void BuildBoneList(BoneNode node, List<BoneNode> nodes)
        {
            //if (node.id == 0 || node.id != 0 && node.flags == 2)
            {
                nodes.Add(node);
                foreach (BoneNode child in node.children)
                {
                    BuildBoneList(child, nodes);
                }
            }
        }



        public void BuildSkeleton()
        {
            int ibreak = 0;
            foreach (BoneNode node in BoneList)
            {
                m_boneIdDictionary[node.id] = node;
            }

            foreach (BoneNode node in BoneList)
            {
                if (node.id != node.parentId)
                {
                    BoneNode parent = m_boneIdDictionary[node.parentId];
                    parent.children.Add(node);
                    node.parent = parent;
                }
            }


            if (BoneList.Count > 0)
            {

                m_rootBone = BoneList[0];
                //m_rootBone = m_bones[1];
                //BuildBoneList(m_rootBone, BoneList);

                // find zero bone
                m_zeroBone = null;
                foreach (BoneNode bn in BoneList)
                {
                    if (bn.name == "zero")
                    {
                        m_zeroBone = bn;
                        break;
                    }
                }

                foreach (BoneNode bn in BoneList)
                {
                    bn.zeroBone = m_zeroBone;
                }

                CalcBindFinalMatrix(IndexedMatrix.Identity);

                IndexedVector3 zeroOffset = new IndexedVector3();

                //foreach (BoneNode node in BoneList)
                //{
                //    if(node.id == 3)
                //    {
                //        node.rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up,90);
                //    //if (node.name == "zero")
                //    //{
                //    //    zeroOffset = node.offset;
                //    //    break;
                //    //}
                //    }
                //}



                if (!m_builtSkelBB)
                {
                    IndexedVector3 min = new IndexedVector3(float.MaxValue);
                    IndexedVector3 max = new IndexedVector3(float.MinValue);


                    foreach (BoneNode node in BoneList)
                    {
                        //if (node.name == "zero")
                        //{
                        //    continue;
                        //}

                        // build tranform from parent chain?
                        IndexedVector3 offset = node.finalMatrix._origin;
                        offset -= zeroOffset;

                        min = IndexedVector3.Min(min, offset);
                        max = IndexedVector3.Max(max, offset);
                    }

                    SkelMinBB = min;
                    SkelMaxBB = max;
                    m_builtSkelBB = true;
                }


            }

        }


        public void CalcBindFinalMatrix(IndexedMatrix parentMatrix)
        {
            //CalcBindFinalMatrix(m_rootBone, parentMatrix);
            try
            {
                foreach (BoneNode boneNode in BoneList)
                {
                    //boneNode.finalMatrix = boneNode.frameMatrix * boneNode.bindPoseMatrix;
                    boneNode.finalMatrix = boneNode.bindPoseMatrix;
                    try
                    {
                        if (boneNode.parent != null)
                        {
                            if ((boneNode.flags & BoneNode.kUseRootForParent) != 0)
                            {
                                boneNode.finalMatrix *= IndexedMatrix.Identity;
                            }
                            else
                            {
                                boneNode.finalMatrix *= boneNode.parent.finalMatrix;
                            }
                            //for (int i = 0; i < boneNode.timeMatrices.Count; ++i)
                            {
                                //boneNode.adjustedTimeMatrices[i] = boneNode.adjustedTimeMatrices[i] * boneNode.parent.finalMatrix;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        int ibreak = 0;
                    }
                }
            }
            catch (Exception e)
            {
                int ibreak = 0;
            }
        }

        //public void CalcBindFinalMatrix(BoneNode bone, Matrix parentMatrix)
        //{
        //    //bone.combinedMatrix = bone.localMatrix * parentMatrix;
        //    bone.combinedMatrix = bone.frameMatrix* parentMatrix;
        //    //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
        //    bone.finalMatrix = bone.combinedMatrix;

        //    foreach (BoneNode child in bone.children)
        //    {
        //        CalcBindFinalMatrix(child, bone.combinedMatrix);
        //    }
        //}

        public virtual bool Valid
        {
            get { return true; }
        }



        public List<BoneNode> BoneList
        {
            get
            {
                //return m_bones; 
                return m_bonesShrunk;
            }
        }

        public Dictionary<int, BoneNode> m_boneIdDictionary = new Dictionary<int, BoneNode>();
        public Dictionary<char[], TagSizeAndData> m_tagSizes = new Dictionary<char[], TagSizeAndData>();
        public List<ModelSubMesh> m_modelMeshes = new List<ModelSubMesh>();
        public String m_name;
        public List<TextureData> m_textures = new List<TextureData>();
        public List<MaterialData> m_materials = new List<MaterialData>();
        public List<String> m_boneNames = new List<String>();
        public Dictionary<int, String> m_boneNameDictionary = new Dictionary<int, string>();
        public List<String> m_textureNames = new List<String>();
        public List<IndexedVector3> m_centers = new List<IndexedVector3>();
        //public List<String> m_selsInfo = new List<string>();
        public List<ShaderData> m_shaderData = new List<ShaderData>();
        //public List<Vector2> UVs = new List<Vector2>();

        public IndexedVector3 MinBB;
        public IndexedVector3 MaxBB;
        public IndexedVector3 SkelMinBB;
        public IndexedVector3 SkelMaxBB;

        public IndexedVector3 Center;
        public List<IndexedMatrix> m_matrices = new List<IndexedMatrix>();
        private List<BoneNode> m_bones = new List<BoneNode>();
        private List<BoneNode> m_bonesShrunk = new List<BoneNode>();
        public BoneNode m_rootBone;
        public BoneNode m_zeroBone;

        public bool m_builtBB = false;
        public bool m_builtSkelBB = false;
        public bool m_skinned = false;
        public bool m_hasSkeleton = false;
        public bool m_animsLoaded = false;

        //public int m_maxVertex;
        public int MaxVertex
        { get; set; }

        public int m_maxNormal;
        public int m_maxUv;
    }


    /*
     * lodlevel 1
        gc : 'full' model at 'best' quality?

        lodlevel 2 
        gc : front face minus eyes, quiver


        lodlevel 4
        gc: eyes and quiver


        lodlevel 8 
        gc: hair with bar and quiver

        lodlevel 16
        gc: hair sides , ears and quiver

        lodlevel 32
        gc: looks like 'good model' simialr to lod 1?


        lodlevel 64
        gc: lower quality full model , lod 2?


        lodlevel 128
        gc: lower quality full model , lod 3?

        lodlevel 128
        gc: lowest quality full model , lod 4?
     * */

    public abstract class ModelSubMesh
    {
        public IndexedVector3 MinBB;
        public IndexedVector3 MaxBB;
        public abstract int NumIndices
        { get; }
        public abstract int NumVertices
        { get; }

        public int LodLevel
        { get; set; }

        public int MeshId
        {
            get;
            set;
        }
        public int MaxNormal
        {
            get;
            set;
        }

        public int MaxVertex
        {
            get;
            set;
        }

        public int MinUV
        {
            get;
            set;
        }


        public int MaxUV
        {
            get;
            set;
        }

        public bool Valid
        {
            get { return true; }
        }

        public abstract List<IndexedVector3> Vertices
        {
            get;
        }

        public abstract List<IndexedVector3> Normals
        {
            get;
        }

        public bool HasNormals
        {
            get { return Normals.Count > 0; }
        }

        public abstract List<IndexedVector2> UVs
        {
            get;
        }

        public abstract List<ushort> Indices
        {
            get;
        }

        public void BuildMinMax()
        {
            MinUV = int.MaxValue;
            MaxUV = int.MinValue;



        }

        //public List<Vector3> Vertices = new List<Vector3>();
        //public List<Vector3> Normals = new List<Vector3>();
        //public List<Vector2> UVs = new List<Vector2>();
        //public List<ushort> Indices = new List<ushort>();
    }

    public class BaseModelReader
    {

        public virtual BaseModel LoadSingleModel(String modelPath, bool readDisplayLists = true)
        {
            return null;
        }
    }



    public class TagSizeAndData
    {
        public static TagSizeAndData Create(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            TagSizeAndData t = new TagSizeAndData(length);
            reader.BaseStream.Position -= 8;
            t.data = reader.ReadBytes(t.length);
            return t;

        }

        public TagSizeAndData(int len)
        {
            length = len;
        }

        public int length;
        public byte[] data;
    }


    public class ShaderData
    {
        public String shaderName;
        public String textureName;
        public int emptyTag1;
        public int unk1;
        public int unk2;
        public int unk3;
        public byte textureId1;
        public byte textureId2;
        public byte[] unkba1;
        public int unk4;
        public int unk5;
        public int unk6;
        public int unk7;



        public static ShaderData FromStream(BinaryReader binReader)
        {
            ShaderData shader = new ShaderData();
            shader.emptyTag1 = binReader.ReadInt32();

            byte[] name = binReader.ReadBytes(124);
            StringBuilder sb = new StringBuilder();
            char b;
            for (int i = 0; i < name.Length; ++i)
            {
                b = (char)name[i];
                if (b == 0x00)
                {
                    break;
                }
                sb.Append(b);
            }
            shader.shaderName = sb.ToString();

            shader.unk1 = binReader.ReadInt32();
            shader.unk2 = binReader.ReadInt32();
            shader.unk3 = binReader.ReadInt32();
            shader.textureId1 = binReader.ReadByte();
            shader.textureId2 = binReader.ReadByte();
            shader.unkba1 = binReader.ReadBytes(6);
            shader.unk4 = binReader.ReadInt32();
            shader.unk5 = binReader.ReadInt32();
            shader.unk6 = binReader.ReadInt32();
            shader.unk7 = binReader.ReadInt32();
            return shader;
        }

    }

    public class CommonMaterialData
    {
        public CommonTextureData diffuseTextureData;
        public CommonTextureData specularTextureData;
        //public string fbxNodeId;
        public string name;

        public static CommonMaterialData FromDiffuseTexture(string textureName)
        {
            CommonMaterialData cmd = new CommonMaterialData();
            cmd.diffuseTextureData = new CommonTextureData();
            cmd.diffuseTextureData.textureName = textureName;
            return cmd;
        }

    }

    public class CommonTextureData
    {
        public string textureName;
        public string fullPathName;
        public int width;
        public int height;
        //public String fbxMaterialNodeId;
        //public String fbxTextureNodeId;
        //public String fbxVideoNodeId;
    }


    public class CommonMeshData
    {

        public List<int> Indices = new List<int>();
        public List<int> Vertices = new List<int>();
        public int LodLevel;
        public string Name;
        public int MaterialId;
        public int Index;
        public int VertexListIndex = 0;
        public int IndexListIndex = 0;
        public bool ZeroBasedIndex = false;
        public IndexedVector4 boundingSphere;
    }

    public class CommonTopLevelModelData // Yuck
    {
        public String Name;
        public List<VertexDataAndDesc> m_vertexDataList = new List<VertexDataAndDesc>();
        public List<List<int>> m_indexDataList = new List<List<int>>();
        public List<CommonModelData> Models = new List<CommonModelData>();
        public List<string> demandLoadVertexList = new List<string>();
        public List<string> demandLoadIndexList = new List<string>();

    }

    public class CommonModelData
    {
        public XboxModel XBoxModel;

        public int AdjustBone(int index, int adjust)
        {
            if (XBoxModel != null)
            {
                return XBoxModel.AdjustBone(index, adjust);
            }
            return index;
        }


        public List<CommonMeshData> GetFilteredSubmeshes(List<int> excludeList)
        {
            List<CommonMeshData> filteredList = new List<CommonMeshData>();
            for (int i = 0; i < CommonMeshData.Count; ++i)
            {
                if (!excludeList.Contains(i))
                {
                    filteredList.Add(CommonMeshData[i]);
                }
            }
            return filteredList;
        }


        public String Name;
        public List<VertexDataAndDesc> VertexDataLists = new List<VertexDataAndDesc>();
        public List<List<int>> IndexDataList = new List<List<int>>();
        public List<CommonMeshData> CommonMeshData = new List<CommonMeshData>();
        public List<CommonTextureData> CommonTextures = new List<CommonTextureData>();
        public List<CommonMaterialData> CommonMaterials = new List<CommonMaterialData>();
        public Dictionary<int, BoneNode> BoneIdDictionary = new Dictionary<int, BoneNode>();
        public List<BoneNode> BoneList = new List<BoneNode>();
        public BoneNode RootBone;
        public bool Skinned;
        public bool AnimsLoaded;
    }

    public class VertexDataAndDesc
    {
        public String Description;
        public int ExpectedElements = -1;
        public List<string> Properties = new List<string>();
        public List<CommonVertexInstance> VertexData = new List<CommonVertexInstance>();
    }

    public class MaterialData2
    {
        public TextureData DiffuseTexture;
        public TextureData SpecularTexture;
        public String Name;
        public CommonMaterialData ToCommon()
        {
            CommonMaterialData commonMaterial = new CommonMaterialData();
            commonMaterial.name = Name;
            if (DiffuseTexture != null)
            {
                commonMaterial.diffuseTextureData = DiffuseTexture.ToCommon();
            }
            if (SpecularTexture != null)
            {
                commonMaterial.specularTextureData = SpecularTexture.ToCommon();
            }
            return commonMaterial;
        }
    }

    public class MaterialData
    {
        public IndexedVector4 Header;
        public bool Blank;
        public int header1;
        public int header2;
        public int header3;
        public int diffuseTextureId = -1;
        public int specularTextureId = -1;
        public int header4;
        public float startVal;
        public int header5;
        public int header6;

        public float endVal;
        public int[] endBlock2 = new int[8];
        public List<MaterialSlotInfo> m_materialSlotInfoList = new List<MaterialSlotInfo>();

        public TextureData diffuseTextureData;
        public TextureData specularTextureData;


        public static void FixupMaterialSlots(MaterialData materialData, List<string> texturenames)
        {
            TextureData diffuseTexture = new TextureData();
            if (materialData.m_materialSlotInfoList.Count > 0)
            {
                int index = materialData.m_materialSlotInfoList[0].resolvedTexture;
                if (index < texturenames.Count)
                {
                    diffuseTexture.textureName = texturenames[index];
                    materialData.diffuseTextureData = diffuseTexture;
                }
                else
                {
                    int ibreak = 0;
                }
                if (materialData.m_materialSlotInfoList.Count == 2)
                {
                    TextureData specularTexture = new TextureData();
                    specularTexture.textureName = texturenames[materialData.m_materialSlotInfoList[1].resolvedTexture];
                    materialData.specularTextureData = specularTexture;
                }
            }
        }


        public void FixupMaterialData(string materialName, List<string> textureNames)
        {
            if (!materialName.Contains("skygold"))
            {
                diffuseTextureData = new TextureData();
                diffuseTextureData.textureName = materialName;

                string skygold = textureNames.Find(x => x.Contains("skygold"));
                if (skygold != null)
                {
                    specularTextureData = new TextureData();
                    specularTextureData.textureName = skygold;
                }
            }

        }


        public CommonMaterialData ToCommon()
        {
            CommonMaterialData cmd = new CommonMaterialData();
            if (diffuseTextureData != null)
            {
                cmd.diffuseTextureData = diffuseTextureData.ToCommon();
            }
            if (specularTextureData != null)
            {
                cmd.specularTextureData = specularTextureData.ToCommon();
            }

            return cmd;
        }


        public void WriteInfo(StreamWriter sw)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("T{0} {1} {2} {3} {4} {5:0.00000} {6} {7}", diffuseTextureId / XboxModel.s_textureBlockSize, header1, header2, header3, header4, startVal, header5, header6);
            //sb.AppendLine();
            //foreach (int[] db in m_data)
            //{
            //    foreach (int i in db)
            //    {
            //        sb.Append(String.Format("{0,10} ", i));
            //    }
            //    sb.AppendLine();
            //}
            //sb.AppendLine();
            //sb.AppendFormat("{0:0.000000}", endVal);
            //sb.AppendLine();
            //sb.AppendLine("*******************************************");
            //sw.WriteLine(sb.ToString());
        }

    }




    public class TextureData
    {
        public string textureName;
        public string fullPathName;
        public int minusOne;
        public int unknown;
        public int width;
        public int height;
        public int three;
        public int zero;


        public CommonTextureData ToCommon()
        {
            CommonTextureData ctd = new CommonTextureData();
            ctd.textureName = textureName;
            ctd.fullPathName = fullPathName;
            ctd.width = width;
            ctd.height = height;
            return ctd;
        }

        public static TextureData FromStream(BinaryReader binReader)
        {
            TextureData textureData = new TextureData();
            StringBuilder sb = new StringBuilder();
            int count = 0;
            char b;
            int textureNameLength = 0x80;
            for (int i = 0; i < textureNameLength; ++i)
            {
                b = (char)binReader.ReadByte();
                if (b != 0x00)
                {
                    sb.Append(b);
                }
            }

            String textureName = sb.ToString();
            textureData.textureName = textureName;
            textureData.minusOne = binReader.ReadInt32();
            textureData.unknown = binReader.ReadInt32();
            textureData.width = binReader.ReadInt32();
            textureData.height = binReader.ReadInt32();
            textureData.three = binReader.ReadInt32();
            textureData.zero = binReader.ReadInt32();

            //Debug.Assert(textureData.three == 3);
            //Debug.Assert(textureData.zero == 0);

            return textureData;
        }

        public override string ToString()
        {
            return String.Format("N {0} a {1} b {2} w {3} h {4} u1 {5} u2 {6}", textureName, minusOne, unknown, width, height, three, zero);
        }

    }


public class CommonVertexInstance
{
    public static int sFailedCount = 0;

    public IndexedVector3 Position;
    public IndexedVector3 Normal;
    public IndexedVector3 Tangent;
    public IndexedVector2 UV;
    public IndexedVector2 UV2;
    public IndexedVector2 UV3;
    public Color DiffuseColor;
    public int ExtraData;
    //public byte[] Weights;
    public short[] BoneIndices;
    public short[] TranslatedBoneIndices;
    public int BoneInfo1;
    public int BoneWeights;
    public int BoneInfo3;

    public static HashSet<short> sBoneIndices = new HashSet<short>();
    public static Dictionary<short, int> sBoneIndicesCount = new Dictionary<short, int>();

    public override string ToString()
    {
        return String.Format("P {0}\tN {1}\tUV {2}\tE {3}", CommonModelImporter.ToString(Position), CommonModelImporter.ToString(Normal), CommonModelImporter.ToString(UV), ExtraData);
    }


    public int ActiveWeights()
    {
        int result = 0;
        for (int i = 0; i < 3; ++i)
        {
            if (Weight(i) > 0.0f)
            {
                result++;
            }
        }
        return result;
    }

    public float Weight(int index)
    {
        uint[] masks = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };
        uint mask = masks[index];
        int a = (int)(BoneWeights & mask);
        if (index == 3 && a != 0)
        {
            int ibreak = 0;
        }
        a = a >> (index * 8);
        return (float)a / (float)255;
    }

    public void SanityTest()
    {

        for (int i = 0; i < 3; ++i)
        {
            float weight = Weight(i);
            int index = BoneIndices[i];
            if (weight == 0 && index != -1)
            {
                int ibreak = 0;
                sFailedCount++;
            }

            if (index != -1)
            {
                if (weight == 0)
                {
                    int ibreak = 0;
                    sFailedCount++;

                }
            }

        }
    }

}
    



    public class MaterialSlotInfo
    {
        public int always3073;
        public int slotId;
        public int textureOffset;
        public int resolvedTexture;

        public static MaterialSlotInfo FromStream(BinaryReader binReader)
        {
            MaterialSlotInfo slotInfo = new MaterialSlotInfo();
            slotInfo.always3073 = binReader.ReadInt32();
            //Debug.Assert(slotInfo.always3073 == 3073);
            slotInfo.slotId = binReader.ReadInt32();
            slotInfo.textureOffset = binReader.ReadInt32();
            slotInfo.resolvedTexture = slotInfo.textureOffset / 64;
            return slotInfo;
        }

    }

