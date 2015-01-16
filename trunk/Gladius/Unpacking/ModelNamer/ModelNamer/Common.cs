using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ModelNamer
{
    static class Common
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
        //public static char[] endTag = new char[] { (char)0x3F,'E', 'N', 'D'};


        public static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag, 
                                      dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag, vflaTag, 
                                      ramTag, msarTag, nlvlTag, meshTag, elemTag, skelTag, skinTag,
                                      vflgTag,stypTag,nameTag };

        public static char[][] xboxTags = { versTag, cprtTag, selsTag, txtrTag, xrndTag};


        public static int CountCharsInStream(BinaryReader binReader, char[] charsToFind)
        {
            int count = 0;
            try
            {
                while (binReader.BaseStream.Position < binReader.BaseStream.Length)
                {
                    if (FindCharsInStream(binReader, charsToFind,false))
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


        public static float  ToFloatUInt16BigEndian(BinaryReader reader)
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

        public static float ToHalfFloatInt16(BinaryReader reader)
        {
            ushort val = reader.ReadUInt16();
            HalfSingle hs = new HalfSingle();
            hs.PackedValue = val;
            return hs.ToSingle();
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

        public static string ByteArrayToString(byte[] ba,int spaceCount = 0)
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

        public static string ByteArrayToStringSub(byte[] ba, int start,int length)
        {
            int total = start+length;
            if (ba != null && (total < ba.Length))
            {
                StringBuilder hex = new StringBuilder(length * 2);
                int counter = 0;
                
                for(int i=start;i<total;++i)
                {
                    hex.AppendFormat("{0:X2}", ba[i]);
                }
                return hex.ToString();
            }
            return "";
        }




        public static int ToInt16BigEndian(byte[] buf, int i)
        {
            return (short)(buf[i] << 8 | buf[i+1]);
        }

        public static int ReadInt32BigEndian(BinaryReader reader)
        {
            reader.Read(s_buffer, 0, s_buffer.Length);
            return Common.ToInt32BigEndian(s_buffer, 0);
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

        public static Vector2 FromStreamVector2(BinaryReader reader)
        {
            Vector2 v = new Vector2();
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            return v;
        }

        
        public static Quaternion FromStreamQuaternion(BinaryReader reader)
        {
            Quaternion q = new Quaternion();
            q.X = reader.ReadSingle();
            q.Y = reader.ReadSingle();
            q.Z = reader.ReadSingle();
            q.W = reader.ReadSingle();
            return q;
        }

        
        public static float ByteToFloat(byte b)
        {
            int val = (int)b;
            if(val > 127)
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
            float floatpart = (((float)reader.ReadByte())/255.0f);

            return fixedpart+floatpart;

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

        public static Vector2 FromStreamVector2BE(BinaryReader reader)
        {
            Vector2 v = new Vector2();
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.X = Common.ReadSingleBigEndian(s_buffer, 0);
            reader.Read(s_buffer, 0, s_buffer.Length);
            v.Y = Common.ReadSingleBigEndian(s_buffer, 0);
            return v;
        }

        public static Matrix FromStreamMatrixBE(BinaryReader reader)
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

            return new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }

        public static void WriteFloat(StreamWriter sw, Vector3 v)
        {
            sw.WriteLine(String.Format("{0:0.00000000} {1:0.00000000} {2:0.00000000}", v.X, v.Y, v.Z));
        }

        public static void WriteInt(StreamWriter sw, Vector3 v)
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
                    sb.Clear();
                }
            }
        }

        public static void ReadNullSeparatedNames(BinaryReader binReader, List<String> selsNames)
        {
            StringBuilder sb = new StringBuilder();

            byte b;
            byte lb = 0xFF;
            int count = 0;
            while ((b = binReader.ReadByte()) != 0x00 && lb != 0x00)
            {
                if (b != 0x00)
                {
                    sb.Append((char) b);
                }
                else if (sb.Length > 0)
                {
                    selsNames.Add(sb.ToString());
                    sb.Clear();
                }
                lb = b;
            }
        }



        public static void ReadNullSeparatedNames(BinaryReader binReader, long position, int numAnims,List<String> selsNames)
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
                sb.Clear();
            }
        }



        public static void ReadTextureNames(BinaryReader binReader, char[] tagName,List<String> textureNames,bool stripExtension = false)
        {
            if (Common.FindCharsInStream(binReader, tagName,true))
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

    public class BoneNode
    {
        public String name;
        public Vector3 offset;
        public Quaternion rotation;
        public byte pad1;
        public byte id;
        public byte id2;
        public byte parentId;

        public Matrix rotationMatrix;
        public Matrix combinedMatrix;
        public Matrix finalMatrix;
        public Matrix localMatrix;

        public BoneNode parent;
        public List<BoneNode> children = new List<BoneNode>();

        public static BoneNode FromStream(BinaryReader binReader)
        {
            BoneNode node = new BoneNode();
            node.offset = Common.FromStreamVector3(binReader);
            node.rotation = Common.FromStreamQuaternion(binReader);
            Debug.Assert(Common.FuzzyEquals(node.rotation.Length(), 1.0f, 0.0001f));

            node.pad1 = binReader.ReadByte();
            node.id = binReader.ReadByte();
            node.id2 = binReader.ReadByte();
            node.parentId = binReader.ReadByte();

            node.rotationMatrix = Matrix.CreateFromQuaternion(node.rotation);
            node.localMatrix = Matrix.CreateTranslation(node.offset) * node.rotationMatrix;

            return node;
        }
    }

    public class BaseModel
    {
        public BaseModel(string name)
        {
            m_name = name;
        }

        public virtual void LoadData(BinaryReader binReader){}
        public virtual void BuildBB(){}
        public virtual void Validate(){}

        public void LoadModelTags(BinaryReader binReader,char[][] tagList)
        {
            foreach (char[] tag in tagList)
            {
                // reset for each so we don't worry about order
                binReader.BaseStream.Position = 0;
                if (Common.FindCharsInStream(binReader, tag, true))
                {
                    TagSizeAndData tsad = TagSizeAndData.Create(binReader);

                    m_tagSizes[tag] = tsad;
                }
                else
                {
                    m_tagSizes[tag] = new TagSizeAndData(-1);
                }
            }
        }


        public string FindTextureName(ShaderData sd)
        {
            int index = sd.textureId1;
            if (index == 255)
            {
                index = 0;
            }
            String textureName = m_textures[index].textureName;
            if (textureName.Contains("skygold"))
            {
                index = sd.textureId2;
            }

            if (index == 255)
            {
                index = 0;
            }

            textureName = m_textures[index].textureName;

            return textureName;
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




        public void ConstructSkeleton()
        {
            Dictionary<int, BoneNode> dictionary = new Dictionary<int, BoneNode>();
            foreach (BoneNode node in m_bones)
            {
                dictionary[node.id] = node;
            }

            foreach (BoneNode node in m_bones)
            {
                if (node.id != node.parentId)
                {
                    BoneNode parent = dictionary[node.parentId];
                    parent.children.Add(node);
                    node.parent = parent;
                }
            }


            if (m_bones.Count > 0)
            {

                m_rootBone = m_bones[0];
                CalcBindFinalMatrix(m_rootBone, Matrix.Identity);

                if (!m_builtSkelBB)
                {
                    Vector3 min = new Vector3(float.MaxValue);
                    Vector3 max = new Vector3(float.MinValue);


                    foreach (BoneNode node in m_bones)
                    {
                        // build tranform from parent chain?
                        Vector3 offset = node.finalMatrix.Translation;

                        min = Vector3.Min(min, offset);
                        max = Vector3.Max(max, offset);
                    }

                    SkelMinBB = min;
                    SkelMaxBB = max;
                    m_builtSkelBB = true;
                }


            }

        }

        void CalcBindFinalMatrix(BoneNode bone, Matrix parentMatrix)
        {
            bone.combinedMatrix = bone.localMatrix * parentMatrix;
            //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
            bone.finalMatrix = bone.combinedMatrix;

            foreach (BoneNode child in bone.children)
            {
                CalcBindFinalMatrix(child, bone.combinedMatrix);
            }
        }

        public bool Valid
        {
            get { return true; }
        }

        
        public Dictionary<char[], TagSizeAndData> m_tagSizes = new Dictionary<char[], TagSizeAndData>();
        public List<ModelSubMesh> m_modelMeshes = new List<ModelSubMesh>();
        public String m_name;
        public List<TextureData> m_textures = new List<TextureData>();
        public List<String> m_names = new List<String>();
        public List<Vector3> m_centers = new List<Vector3>();
        public List<String> m_selsInfo = new List<string>();
        public List<ShaderData> m_shaderData = new List<ShaderData>();
        //public List<Vector2> UVs = new List<Vector2>();


        public Vector3 MinBB;
        public Vector3 MaxBB;
        public Vector3 SkelMinBB;
        public Vector3 SkelMaxBB;

        public Vector3 Center;
        public List<Matrix> m_matrices = new List<Matrix>();
        public List<BoneNode> m_bones = new List<BoneNode>();
        public BoneNode m_rootBone;
        public bool m_builtBB = false;
        public bool m_builtSkelBB = false;
        public bool m_skinned = false;
        public bool m_hasSkeleton = false;
        //public int m_maxVertex;
        public int MaxVertex
        { get; set; }

        public int m_maxNormal;
        public int m_maxUv;
    }


    public abstract class ModelSubMesh
    {
        public Vector3 MinBB;
        public Vector3 MaxBB;
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

        public int MaxUV
        {
            get;
            set;
        }

        public bool Valid
        {
            get { return true; }
        }

        public abstract List<Vector3> Vertices
        {
            get;
        }

        public abstract List<Vector3> Normals
        {
            get;
        }

        public bool HasNormals
        {
            get { return Normals.Count > 0; }
        }

        public abstract List<Vector2> UVs
        {
            get;
        }

        public abstract List<ushort> Indices
        {
            get;
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



}
