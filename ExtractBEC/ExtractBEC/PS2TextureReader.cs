using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static ExtractBEC.GCTextureReader;

namespace ExtractBEC
{
    // a greatly modified parser , originaly based on the targa reader by David Polomis (paloma_sw@cox.net)
    // now modified to extract image files from the ps2 version of LucasArts gladius.

    public class PS2TextureReader
    {

        static void Main(string[] args)
        {
            List<string> fileNames = new List<string>();
            String sourcePath = @"F:\Gladius\PS2-texture-extraction\Preview\pttp\";
            //fileNames.AddRange(Directory.GetFiles(sourcePath, "File129"));
            fileNames.AddRange(Directory.GetFiles(sourcePath, "**"));
            //fileNames.AddRange(Directory.GetFiles(sourcePath, "File96"));
            //fileNames.AddRange(Directory.GetFiles(sourcePath, "File24"));


            String outputDirectory = @"F:\Gladius\PS2-texture-extraction\Preview\pttp-converted\";

            new PS2TextureReader().ExtractImages(fileNames, outputDirectory);
        }


        public void ExtractImages(List<string> fileNames, string targetDirectory)
        {
            System.IO.DirectoryInfo targetInfo = new DirectoryInfo(targetDirectory);
            if (!targetInfo.Exists)
            {
                targetInfo.Create();
            }

            bool doDelete = true;
            if (doDelete)
            {
                foreach (FileInfo file in targetInfo.GetFiles())
                {
                    file.Delete();
                }
            }

            using (StreamWriter errorStream = new StreamWriter(new FileStream(targetDirectory + "\\errors.txt", FileMode.OpenOrCreate)))
            {
                foreach (string fileName in fileNames)
                {
                    try
                    {
                        FileInfo file = new FileInfo(fileName);
                        using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                        using (BinaryReader binReader = new BinaryReader(fs))
                        {

                            PS2Texture texture = PS2Texture.FromStream(binReader);

                            int ibreak = 0;

                            if (texture.m_PS2PFHDChunk != null)
                            {
                                foreach (PS2PS2PFHDCEntry entry in texture.m_PS2PFHDChunk.TextureEntries)
                                {
                                    if (entry.ContainsDefinition)
                                    {
                                        try
                                        {
                                            entry.CreateBitMap();
                                            String filename = entry.Name + "-" + entry.Width+"-" +entry.Height + ".png";
                                            entry.BitmapImage.Save(targetDirectory + filename, ImageFormat.Png);
                                        }
                                        finally
                                        {
                                            entry.DisposeBitmap();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        int ibreak = 0;
                    }
                }
            }

        }



    }



    public class PS2Texture
    {

        public PTTPChunk m_PTTPChunk;
        public BaseChunk m_NAMEChunk;
        public BaseChunk m_DTNTChunk;
        public NMPTChunk m_NMPTChunk;
        public PS2PFHDChunk m_PS2PFHDChunk;
        public BaseChunk m_ENDChunk;


        public static PS2Texture FromStream(BinaryReader binReader)
        {
            PS2Texture ps2Texture = new PS2Texture();
            string name = "test";
            ps2Texture.m_PTTPChunk = (PTTPChunk)BaseChunk.FromStreamMaster(name, binReader);

            ps2Texture.m_NAMEChunk = BaseChunk.FromStreamMaster(name, binReader);

            if (CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.nmptTag))
            {
                binReader.BaseStream.Position -= 4;
                ps2Texture.m_NMPTChunk = (NMPTChunk)BaseChunk.FromStreamMaster(name, binReader);
            }

            if (CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.dtntTag))
            {
                binReader.BaseStream.Position -= 4;
                ps2Texture.m_DTNTChunk = BaseChunk.FromStreamMaster(name, binReader);
            }

            Platform platform = ps2Texture.m_NMPTChunk != null ? Platform.PS2Review : Platform.PS2Preview;

            if (platform == Platform.PS2Preview || platform == Platform.PS2Review)
            {
                ps2Texture.m_PS2PFHDChunk = (PS2PFHDChunk)BaseChunk.FromStreamMaster(name, binReader, platform);
            }
            else
            {
                //ps2Texture.m_PFHDChunk = (PFHDChunk)BaseChunk.FromStreamMaster(name, binReader, platform);
            }

            ps2Texture.m_ENDChunk = BaseChunk.FromStreamMaster(name, binReader);

            // read the rest of the file =
            //ps2Texture.m_R2D2psx2Data = binReader.ReadBytes((int)(binReader.BaseStream.Length - binReader.BaseStream.Position));

            if (ps2Texture.m_NMPTChunk != null && ps2Texture.m_PS2PFHDChunk != null)
            {
                Debug.Assert(ps2Texture.m_PS2PFHDChunk.TextureEntries.Count == ps2Texture.m_NMPTChunk.TextureNames.Count);
                for (int i = 0; i < ps2Texture.m_NMPTChunk.TextureNames.Count; ++i)
                {
                    ps2Texture.m_PS2PFHDChunk.TextureEntries[i].Name = ps2Texture.m_NMPTChunk.TextureNames[i];
                }
            }


            if (ps2Texture.m_PS2PFHDChunk != null)
            {
                // now get the texture map blocks 
                foreach (PS2PS2PFHDCEntry entry in ps2Texture.m_PS2PFHDChunk.TextureEntries)
                {

                    if (entry.ContainsDefinition)
                    {
                        CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.tmapTag);
                        entry.FillTextureData(binReader);
                        //break;
                    }

                }
            }
            return ps2Texture;
        }

    }



    public class PS2PS2PFHDCEntry
    {
        public int Width;
        public int Height;
        public int Stride;
        public int BytesPerPixel;
        public byte Format;
        public string Name;
        public bool ContainsDefinition;

        GCHandle ImageByteHandle;
        public Image BitmapImage;

        public byte[] m_rawColourMap;
        public byte[] m_swizzledColourMap;
        public byte[] m_imageData;


        public void FillTextureData(BinaryReader binReader)
        {
            int imagePadding = 0x13;
            int headerPadding = 0;

            // find tmap location


            binReader.BaseStream.Position += 24;
            //binReader.BaseStream.Position += 20;

            if (BytesPerPixel == 1)
            {
                headerPadding = 0x78 - 20;
            }
            else if (BytesPerPixel == 4)
            {
                headerPadding = 101;
            }

            binReader.BaseStream.Position += headerPadding;
            if (BytesPerPixel == 1)
            {
                LoadColourMapInfo(binReader);
            }

            binReader.BaseStream.Position += imagePadding;

            LoadImageData(binReader);



        }
        public void LoadColourMapInfo(BinaryReader binReader)
        {
            int paletteSize = 1024;

            m_rawColourMap = new byte[paletteSize];
            m_swizzledColourMap = new byte[paletteSize];

            int rawCounter = 0;
            while (rawCounter < paletteSize)
            {
                int a = 0;
                int r = 0;
                int g = 0;
                int b = 0;

                byte rbyte = binReader.ReadByte();
                byte gbyte = binReader.ReadByte();
                byte bbyte = binReader.ReadByte();
                byte abyte = binReader.ReadByte();

                m_rawColourMap[rawCounter++] = rbyte;
                m_rawColourMap[rawCounter++] = gbyte;
                m_rawColourMap[rawCounter++] = bbyte;
                m_rawColourMap[rawCounter++] = abyte;

            }

            byte[] src = m_rawColourMap;
            byte[] tgt = m_swizzledColourMap;

            int blockSize = 8;
            int blockIncrement = blockSize * 4;
            int offset = 0;
            while (offset < src.Length)
            {

                for (int j = 0; j < blockSize; ++j)
                {
                    tgt[offset + 0] = src[offset + 2];
                    tgt[offset + 1] = src[offset + 1];
                    tgt[offset + 2] = src[offset + 0];
                    tgt[offset + 3] = src[offset + 3];
                    offset += 4;
                }

                for (int j = 0; j < blockSize; ++j)
                {
                    tgt[offset + 0] = src[blockIncrement + offset + 2];
                    tgt[offset + 1] = src[blockIncrement + offset + 1];
                    tgt[offset + 2] = src[blockIncrement + offset + 0];
                    tgt[offset + 3] = src[blockIncrement + offset + 3];
                    offset += 4;
                }

                for (int j = 0; j < blockSize; ++j)
                {
                    tgt[offset + 0] = src[offset + 2 - (blockIncrement * 1)];
                    tgt[offset + 1] = src[offset + 1 - (blockIncrement * 1)];
                    tgt[offset + 2] = src[offset + 0 - (blockIncrement * 1)];
                    tgt[offset + 3] = src[offset + 3 - (blockIncrement * 1)];
                    offset += 4;
                }

                for (int j = 0; j < blockSize; ++j)
                {
                    tgt[offset + 0] = src[offset + 2];
                    tgt[offset + 1] = src[offset + 1];
                    tgt[offset + 2] = src[offset + 0];
                    tgt[offset + 3] = src[offset + 3];
                    offset += 4;
                }

            }

            bool allAlpha = true;
            for (int i = 0; i < tgt.Length; i += 4)
            {
                if (tgt[i + 3] != 0)
                {
                    allAlpha = false;
                    break;
                }
            }


            //if (allAlpha)
            if (true)
            {
                for (int i = 0; i < tgt.Length; i += 4)
                {
                    tgt[i + 3] = 255;
                }
            }


            // skip 12?
            int a1 = binReader.ReadInt32();
            int a2 = binReader.ReadInt32();


            int widthCopy = binReader.ReadInt16();
            int heightCopy = binReader.ReadInt16();

        }


        public void LoadImageData(BinaryReader binReader)
        {
            Stride = Width * BytesPerPixel;

            m_imageData = this.LoadImageBytes(binReader);

        }

        public void CreateBitMap()
        {
            ImageByteHandle = GCHandle.Alloc(m_imageData, GCHandleType.Pinned);

            PixelFormat pf = BytesPerPixel == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format32bppArgb;

            BitmapImage = new Bitmap(Width, Height, Stride, pf, ImageByteHandle.AddrOfPinnedObject());

            if (pf == PixelFormat.Format8bppIndexed)
            {
                int numColourEntries = m_rawColourMap.Length / 4;
                if (numColourEntries > 0)
                {
                    ColorPalette pal = BitmapImage.Palette;

                    byte[] map = m_rawColourMap;
                    map = m_swizzledColourMap;
                    for (int i = 0; i < numColourEntries; i++)
                    {
                        //Color c = Color.FromArgb(255, map[i * 4], map[(i * 4) + 1], map[(i * 4) + 2]);
                        Color c = Color.FromArgb(map[(i * 4) + 3], map[i * 4], map[(i * 4) + 1], map[(i * 4) + 2]);
                        byte A = c.A;
                        Color c2 = Color.FromArgb(A, c.B, c.G, c.R);
                        pal.Entries[i] = c2;
                    }
                    BitmapImage.Palette = pal;
                }
            }

        }

        public void DisposeBitmap()
        {
            // Dispose managed resources.
            if (BitmapImage != null)
            {
                BitmapImage.Dispose();
            }

            if (ImageByteHandle != null)
            {
                if (ImageByteHandle.IsAllocated)
                {
                    ImageByteHandle.Free();
                }
            }
        }

        private byte[] LoadImageBytes(BinaryReader binReader)
        {
            byte[] data = null;
            int intImageRowByteSize = (int)this.Width * ((int)this.BytesPerPixel);

            // get the size in bytes of the whole image
            int intImageByteSize = intImageRowByteSize * (int)this.Height;
            data = new byte[intImageByteSize];

            try
            {
                if (BytesPerPixel == 1)
                {
                    data = binReader.ReadBytes(intImageByteSize);
                    //for (int i = 0; i < data.Length; ++i)
                    //{
                    //    data[i] = binReader.ReadByte();
                    //}

                }
                else if (BytesPerPixel == 4)
                {
                    byte[] subBytes = new byte[4];
                    int adjustedLength = data.Length / 4;
                    int counter = 0;
                    for (int i = 0; i < adjustedLength; ++i)
                    {
                        subBytes[0] = binReader.ReadByte();
                        subBytes[1] = binReader.ReadByte();
                        subBytes[2] = binReader.ReadByte();
                        subBytes[3] = binReader.ReadByte();

                        //bgr
                        //Array.Reverse(subBytes);
                        //subBytes[3] = 0xff;
                        data[counter++] = subBytes[2];
                        data[counter++] = subBytes[1];
                        data[counter++] = subBytes[0];
                        data[counter++] = subBytes[3];
                    }

                }
            }
            catch (Exception e)
            {
                int ibreak = 0;
                // if we read past the end, somethings wrong, but try and return as much data as we could get..
            }
            // return the image byte array
            return data;

        }


        public static PS2PS2PFHDCEntry FromStream(PS2PFHDChunk owner, string name, BinaryReader binReader, Platform platform)
        {
            PS2PS2PFHDCEntry entry = new PS2PS2PFHDCEntry();

            if (platform == Platform.PS2Preview)
            {
                int currentPosition = (int)binReader.BaseStream.Position;
                binReader.ReadInt32();
                //binReader.ReadByte();

                entry.BytesPerPixel = 1;
                entry.Width = binReader.ReadInt16();
                entry.Height = binReader.ReadInt16();

                binReader.ReadInt32();


                int cd = binReader.ReadInt32();
                entry.ContainsDefinition = (cd != -1);
                if (owner.Version == 1)
                {
                    entry.ContainsDefinition = false;
                }

                entry.Name = CommonModelImporter.ReadString(binReader);
                int alignment = 4;

                int rem = ((int)(binReader.BaseStream.Position) % alignment);
                int mod = alignment - rem;
                if (rem == 0)
                {
                    mod = 0;
                }

                binReader.BaseStream.Position += mod;
            }
            else if (platform == Platform.PS2Review)
            {
                int sectionLength = 32;
                int currentPosition = (int)binReader.BaseStream.Position;

                byte b1 = binReader.ReadByte();

                //PTX_GAMECUBE_S3_CMPR = 32,	// game cube DXT1

                entry.Format = binReader.ReadByte();
                entry.ContainsDefinition = true;
                entry.BytesPerPixel = 1;


                byte b3 = binReader.ReadByte();
                byte b4 = binReader.ReadByte();

                int unk2 = binReader.ReadInt32();

                entry.Width = binReader.ReadInt16();
                entry.Height = binReader.ReadInt16();
                binReader.BaseStream.Position = currentPosition + sectionLength;

            }
            return entry;
        }
    }

    public class PS2PFHDChunk : BaseChunk
    {
        public List<PS2PS2PFHDCEntry> TextureEntries = new List<PS2PS2PFHDCEntry>();


        // 512 x 256
        public static BaseChunk FromStream(string name, BinaryReader binReader, Platform platform)
        {
            PS2PFHDChunk chunk = new PS2PFHDChunk();
            chunk.BaseFromStream(binReader);
            for (int i = 0; i < chunk.NumElements; ++i)
            {
                chunk.TextureEntries.Add(PS2PS2PFHDCEntry.FromStream(chunk, name, binReader, platform));
            }
            chunk.MoveToEnd(binReader);
            return chunk;
        }
    }


}

