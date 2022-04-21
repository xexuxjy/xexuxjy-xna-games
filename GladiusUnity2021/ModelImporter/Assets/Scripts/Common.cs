using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

public static class Common
{
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

    public static void ReadNullSeparatedNamesInSectionLength(BinaryReader binReader, List<String> selsNames, long sectionLength)
    {
        StringBuilder sb = new StringBuilder();
        long currentPosition = binReader.BaseStream.Position;
        long endPosition = currentPosition + sectionLength;

        char b;
        while (currentPosition < endPosition)
        {
            while ((b = (char)binReader.ReadByte()) != 0x00)
            {
                sb.Append(b);
            }
            if (sb.Length > 0)
            {
                selsNames.Add(sb.ToString());
            }
            sb = new StringBuilder();
            // if next character is null as well, we're done.
            if (binReader.PeekChar() == 0x00)
            {
                break;
            }
        }
        binReader.BaseStream.Position = endPosition + 1;
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

        //short s = reader.ReadInt16();
        //fixed / 65536.0
        //return ((float)s / 65536.0f);
        //byte b1 = reader.ReadByte();
        //byte b2 = reader.ReadByte();

        //int val = (int)b;
        //if (val > 127)
        //{
        //    val = -256 + val;
        //}
        //return ((float)val);
    }




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


    public static IndexedMatrix FromStreamMatrixBE(BinaryReader reader)
    {

        reader.Read(s_buffer, 0, s_buffer.Length);
        float m11 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m12 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m13 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m14 = Common.ReadSingleBigEndian(s_buffer, 0);

        float m21 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m22 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m23 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m24 = Common.ReadSingleBigEndian(s_buffer, 0);

        float m31 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m32 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m33 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m34 = Common.ReadSingleBigEndian(s_buffer, 0);

        float m41 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m42 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m43 = Common.ReadSingleBigEndian(s_buffer, 0);
        reader.Read(s_buffer, 0, s_buffer.Length);
        float m44 = Common.ReadSingleBigEndian(s_buffer, 0);

        return new IndexedMatrix(m11, m12, m13, m21, m22, m23, m31, m32, m33, m41, m42, m43);
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

            StringBuilder sb = new StringBuilder();

            char b;
            int count = 0;
            while (count < selsSectionLength)
            {
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
                //sb.Clear();
                sb = new StringBuilder();
            }
        }
    }

    public static void ReadNullSeparatedNames(BinaryReader binReader, long position, int numAnims, List<String> selsNames)
    {
        binReader.BaseStream.Position = position;
        StringBuilder sb = new StringBuilder();
        char b;
        int count = 0;
        while (selsNames.Count < numAnims)
        {
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
            //sb.Clear();
            sb = new StringBuilder();
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

    public static void DecomposeMatrix(ref UnityEngine.Matrix4x4 mat, ref UnityEngine.Vector3 pos, ref UnityEngine.Quaternion rot)
    {
        // Extract new local position
        pos = mat.GetColumn(3);

        // Extract new local rotation
        rot = UnityEngine.Quaternion.LookRotation(
            mat.GetColumn(2),
            mat.GetColumn(1)
        );

        //// Extract new local scale
        //Vector3 scale = new Vector3(
        //    m.GetColumn(0).magnitude,
        //    m.GetColumn(1).magnitude,
        //    m.GetColumn(2).magnitude
        //);
    }

    public static UnityEngine.Quaternion Normalize(UnityEngine.Quaternion q)
    {
        UnityEngine.Vector4 v = new UnityEngine.Vector4(q.x, q.y, q.z, q.w);
        v.Normalize();
        return new UnityEngine.Quaternion(v.x, v.y, v.z, v.w);
    }

    public static UnityEngine.Vector3 ExtractEulerZYX(UnityEngine.Matrix4x4 m)
    {
        //m.Translation = Vector3.Zero;
        m = UnityEngine.Matrix4x4.Transpose(m);

        UnityEngine.Vector3 r = new UnityEngine.Vector3();
        if (m.m20 < 1.0f)
        {
            if (m.m20> -1.0f)
            {
                r.y = (float)Math.Asin(-m.m20);
                r.z = (float)Math.Atan2(m.m10, m.m00);
                r.x = (float)Math.Atan2(m.m21, m.m22);
            }
            else
            {
                // Not a unique solution.
                r.y = (float)Math.PI / 2.0f;
                r.z = -(float)Math.Atan2(-m.m12, m.m11);
                r.x = 0;
            }
        }
        else
        {
            r.y = (float)-Math.PI / 2.0f;
            r.z = -(float)Math.Atan2(-m.m12, m.m11);
            r.x = 0;
        }
        return r;
    }


}
