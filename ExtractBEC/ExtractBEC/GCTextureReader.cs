using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;

namespace ExtractBEC
{

    public class CommonModelImporter
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
        public static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
        public static char[] dtntTag = new char[] { 'D', 'T', 'N', 'T' };


        public static void ReadNullSeparatedNames(BinaryReader binReader, List<String> names)
        {
            char b;
            int count = 0;
            int maxCount = 10000;

            while (count < maxCount)
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
                    names.Add(sb.ToString());
                }
                // check for 2 null pointers and break

                if (binReader.PeekChar() == 0)
                {
                    break;
                }
            }
        }


        public static string ReadString(BinaryReader binReader)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            char b;
            while ((b = (char)binReader.ReadByte()) != 0x00)
            {
                count++;
                sb.Append(b);
            }
            return sb.ToString();
        }


        public static bool FindCharsInStream(BinaryReader binReader, char[] charsToFind, bool resetPositionIfNotFound = true)
        {
            bool found = false;
            byte b = (byte)' ';
            int lastFoundIndex = 0;
            long currentPosition = binReader.BaseStream.Position;
            try
            {
                while (binReader.BaseStream.Position < binReader.BaseStream.Length)
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


    }

    public class GCTextureReader
    {
        static void Main(string[] args)
        {
            string basePath = @"D:\GladiusISOWorkingExtracted\python-gc\gc\data\texture\";
            string inputFile = basePath + "3d_loading.tga";

            using (BinaryReader binReader = new BinaryReader(new FileStream(inputFile, FileMode.Open)))
            {
                GCTexture gcTexture = GCTexture.FromStream(binReader);


            }
        }

        public class GCTexture
        {
            public String Name;

            public PTTPChunk m_PTTPChunk;
            public NMPTChunk m_NMPTChunk;
            public PFHDChunk m_PFHDChunk;
            public PADDChunk m_PADDChunk;
            public PTDTChunk m_PTDTChunk;
            public BaseChunk m_ENDChunk;

            public static GCTexture FromStream(BinaryReader binReader)
            {
                GCTexture gcTexture = new GCTexture();
                string name = "test";
                gcTexture.m_PTTPChunk = (PTTPChunk)BaseChunk.FromStreamMaster(name, binReader);
                gcTexture.m_NMPTChunk = (NMPTChunk)BaseChunk.FromStreamMaster(name, binReader);
                gcTexture.m_PFHDChunk = (PFHDChunk)BaseChunk.FromStreamMaster(name, binReader);
                gcTexture.m_PADDChunk = (PADDChunk)BaseChunk.FromStreamMaster(name, binReader);
                gcTexture.m_PTDTChunk = (PTDTChunk)BaseChunk.FromStreamMaster(name, binReader);
                gcTexture.m_ENDChunk = BaseChunk.FromStreamMaster(name, binReader);

                return null;
            }

            public void DXTUnpack()
            {
                //Surface surfaceFromFile = Surface.LoadFromRawData(, m_PFHDChunk.Width, m_PFHDChunk.Height, 4 * m_PFHDChunk.Width, false);
                //Compressor compressor = new Compressor();
                //compressor.Compression.Format = CompressionFormat.DXT1;
                //compressor.Input.GenerateMipmaps = false;
                //compressor.Input.SetData(surfaceFromFile);

                //compressor.Output.




            }
        }

        public enum Platform
        {
            GC,
            PS2Preview,
            PS2Review,
            XBOX
        }


        public class BaseChunk
        {
            public uint Position;
            public char[] Signature;
            public uint Length;
            public uint Version;
            public uint NumElements;

            public const uint BlockSize = 16;

            public static bool CompareSignature(char[] a, char[] b)
            {
                if (a == null || b == null)
                {
                    return false;
                }
                if (a.Length != b.Length)
                {
                    return false;
                }
                for (int i = 0; i < a.Length; ++i)
                {
                    if (a[i] != b[i])
                    {
                        return false;
                    }
                }
                return true;
            }


            public static BaseChunk FromStreamMaster(string name, BinaryReader binReader,Platform platform = Platform.GC)
            {
                char[] type = binReader.ReadChars(4);
                uint size = binReader.ReadUInt32();

                binReader.BaseStream.Position -= 8;

                if (CompareSignature(CommonModelImporter.versTag, type))
                {
                    BaseChunk chunk = new BaseChunk();
                    chunk.BaseFromStream(binReader);
                    binReader.BaseStream.Position += (size - 16);
                    return chunk;
                }
                if (CompareSignature(CommonModelImporter.cprtTag, type))
                {
                    BaseChunk chunk = new BaseChunk();
                    chunk.BaseFromStream(binReader);
                    binReader.BaseStream.Position += (size - 16);
                    return chunk;
                }
                if (CompareSignature(CommonModelImporter.pttpTag, type))
                {
                    return PTTPChunk.FromStream(name, binReader);
                }

                if (CompareSignature(CommonModelImporter.paddTag, type))
                {
                    return PADDChunk.FromStream(name, binReader);
                }
                if (CompareSignature(CommonModelImporter.nmptTag, type))
                {
                    return NMPTChunk.FromStream(name, binReader);
                }
                if (CompareSignature(CommonModelImporter.pfhdTag, type))
                {
                    if(platform == Platform.PS2Preview || platform == Platform.PS2Review)
                    {
                        return null;//PS2PFHDChunk.FromStream(name, binReader,platform);
                    }
                    else 
                    {
                        return PFHDChunk.FromStream(name, binReader);
                    }
                }
                if (CompareSignature(CommonModelImporter.ptdtTag, type))
                {
                    return PTDTChunk.FromStream(name, binReader);
                }

                int ibreak = 0;
                binReader.BaseStream.Position += size;
                return null;

            }


            public void BaseFromStream(BinaryReader binReader)
            {
                Position = (uint)binReader.BaseStream.Position;
                Signature = binReader.ReadChars(4);
                Length = binReader.ReadUInt32();
                Version = binReader.ReadUInt32();
                NumElements = binReader.ReadUInt32();
            }

            public void MoveToEnd(BinaryReader binReader)
            {
                binReader.BaseStream.Position = Position + Length;
            }
        }

        public class PTTPChunk : BaseChunk
        {
            public static BaseChunk FromStream(string name, BinaryReader binReader)
            {
                PTTPChunk chunk = new PTTPChunk();
                chunk.BaseFromStream(binReader);
                return chunk;

            }

        }


        public class NMPTChunk : BaseChunk
        {
            public List<string> TextureNames = new List<string>();
            public static BaseChunk FromStream(string name, BinaryReader binReader)
            {
                NMPTChunk chunk = new NMPTChunk();
                chunk.BaseFromStream(binReader);

                for(int i=0;i<chunk.NumElements;++i)
                {
                    string textureName = CommonModelImporter.ReadString(binReader);
                    chunk.TextureNames.Add(textureName);
                }
                chunk.MoveToEnd(binReader);
                return chunk;

            }
        }
        public class PFHDChunk : BaseChunk
        {
            public int Width;
            public int Height;
            public int Stride;
            public byte Format;

            // 512 x 256
            public static BaseChunk FromStream(string name, BinaryReader binReader)
            {
                PFHDChunk chunk = new PFHDChunk();
                chunk.BaseFromStream(binReader);

                byte b1 = binReader.ReadByte();

                //PTX_GAMECUBE_S3_CMPR = 32,	// game cube DXT1

                chunk.Format = binReader.ReadByte();
                byte b3 = binReader.ReadByte();
                byte b4 = binReader.ReadByte();

                int unk2 = binReader.ReadInt32();

                chunk.Width= binReader.ReadInt16();
                chunk.Height = binReader.ReadInt16();
                //int breadth = binReader.ReadInt32();
                chunk.MoveToEnd(binReader);
                return chunk;

            }
        }

        public class PADDChunk : BaseChunk
        {
            public static BaseChunk FromStream(string name, BinaryReader binReader)
            {
                PADDChunk chunk = new PADDChunk();
                chunk.BaseFromStream(binReader);
                chunk.MoveToEnd(binReader);
                return chunk;

            }
        }


        public class PTDTChunk : BaseChunk
        {
            public byte[] Data;
            public static BaseChunk FromStream(string name, BinaryReader binReader)
            {
                PTDTChunk chunk = new PTDTChunk();
                chunk.BaseFromStream(binReader);
                chunk.Data = binReader.ReadBytes((int)(chunk.Length - BlockSize));

                chunk.MoveToEnd(binReader);
                return chunk;

            }

        }

    }
}