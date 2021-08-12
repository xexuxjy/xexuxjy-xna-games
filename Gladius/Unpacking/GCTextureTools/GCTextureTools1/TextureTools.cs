// a greatly modified parser , originaly based on the targa reader by David Polomis (paloma_sw@cox.net)
// now modified to extract image files from the ps2 version of LucasArts gladius.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GCTextureTools;
using System.Drawing.Imaging;
using System.Drawing;


namespace GCTextureTools
{

    public class GladiusImage
    {
        private GladiusHeader gladiusHeader = null;
        private string strFileName = string.Empty;
        public string ImageName;
        public DirectBitmap DirectBitmap;

        public byte[] CompressedData;

        public GladiusImage()
        {
            this.gladiusHeader = new GladiusHeader();
        }

        public GladiusHeader Header
        {
            get { return this.gladiusHeader; }
            set { this.gladiusHeader = value; }
        }


        public string FileName
        {
            get { return this.strFileName; }
        }

    }



    public class GladiusHeader
    {
        public int Width = 0;
        public int Height = 0;
        public int CompressedSize = 0;
        public bool ContainsDefinition;
        public ushort DXTType = 0;
    }



    public class ImageExtractor
    {

        public void ExtractImages(string sourceDirectory, string targetDirectory)
        {
            List<string> fileNames = new List<string>();
            fileNames.AddRange(Directory.GetFiles(sourceDirectory, "*"));
            ExtractImages(fileNames, targetDirectory);
        }

        public void ProcessImages(String sourcePath, String outputDirectory)
        {
            List<string> fileNames = new List<string>();
            fileNames.AddRange(Directory.GetFiles(sourcePath, "**"));
            ExtractImages(fileNames, outputDirectory);
        }


        public void ReadPTDTSection(BinaryReader binReader, List<GladiusImage> imageList)
        {
            if (Common.FindCharsInStream(binReader, Common.ptdtTag))
            {
                uint sectionSize = binReader.ReadUInt32();
                int skip = 8;
                int adjustedSize = (int)sectionSize - skip;
                binReader.BaseStream.Position += skip;



                foreach (GladiusImage image in imageList)
                {
                    image.CompressedData = binReader.ReadBytes(image.Header.CompressedSize);

                    int potWidth = ToNextNearest(image.Header.Width);
                    int potHeight = ToNextNearest(image.Header.Height);

                    image.DirectBitmap = new DirectBitmap(potWidth, potHeight);

                    DecompressDXT1GC(image.CompressedData, potWidth, potHeight, image.DirectBitmap.Bits);

                }
            }
        }

        int ToNextNearest(int x)
        {
            if (x < 0) { return 0; }
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
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

                    List<GladiusImage> imageList = new List<GladiusImage>();
                    try
                    {
                        FileInfo file = new FileInfo(fileName);
                        using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                        using (BinaryReader binReader = new BinaryReader(fs))
                        {
                            List<String> textureNameList = new List<string>();
                            Common.ReadNullSeparatedNames(binReader, Common.nmptTag, textureNameList);
                            {
                                long currentPos = binReader.BaseStream.Position;

                                ReadHeaderSection(binReader, imageList, textureNameList);
                                binReader.BaseStream.Position = 0;
                                ReadPTDTSection(binReader, imageList);

                                foreach (GladiusImage gi in imageList)
                                {
                                    if (gi.DirectBitmap != null)
                                    {
                                        gi.DirectBitmap.Bitmap.Save(targetDirectory + gi.ImageName + ".png", ImageFormat.Png);
                                        gi.DirectBitmap.Dispose();
                                    }
                                    gi.CompressedData = null;
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


        public void ReadHeaderSection(BinaryReader binReader, List<GladiusImage> imageList, List<string> textureNames)
        {
            if (Common.FindCharsInStream(binReader, Common.pfhdTag, true))
            {
                int sectionSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                //if (pad1 != 0 || pad1 != 1)
                if (pad1 != 0 && pad1 != 1)
                {
                    int ibreak = 0;
                }
                int numTextures = binReader.ReadInt32();

                for (int u = 0; u < numTextures; ++u)
                {
                    GladiusImage image = new GladiusImage();
                    image.ImageName = textureNames[u];

                    imageList.Add(image);

                    short compressType = binReader.ReadInt16();
                    image.Header.DXTType = (ushort)(compressType == 17152 ? 5 : 1);
                    binReader.ReadInt16();
                    binReader.ReadInt32();
                    image.Header.Width = binReader.ReadInt16();
                    image.Header.Height = binReader.ReadInt16();
                    image.Header.CompressedSize = binReader.ReadInt32();

                    binReader.BaseStream.Position += 16;

                }

            }


        }


        public static void DecompressDXT1GC(byte[] input, int width, int height, byte[] output)
        {
            int offset = 0;
            int bcw = (width + 3) / 4;
            int bch = (height + 3) / 4;
            int clen_last = (width + 3) % 4 + 1;
            //uint[] buffer = new uint[16];
            uint[] colors = new uint[4];
            int yblock = 0;
            int dstIndex = 0;
            int increment = 8;
            int blockCount = 0;
            uint[] tempData = new uint[width * height];

            int alphaOffset = input.Length / 2 ;

            for (int y = 0; y < height; y += increment)
            {
                for (int x = 0; x < width; x += increment)
                {

                    DXTBlock block = GetMergedBlocks(input, offset,alphaOffset);
                    FillDest(tempData, ref dstIndex, (y * width + x), width, block);
                    offset += increment;

                    block = GetMergedBlocks(input, offset,alphaOffset);
                    FillDest(tempData, ref dstIndex, (y * width + x + 4), width, block);
                    offset += increment;

                    block = GetMergedBlocks(input, offset, alphaOffset);
                    FillDest(tempData, ref dstIndex, ((y + 4) * width + x), width, block);
                    offset += increment;

                    block = GetMergedBlocks(input, offset, alphaOffset);
                    FillDest(tempData, ref dstIndex, ((y + 4) * width + x + 4), width, block);
                    offset += increment;
                    
                    blockCount += 4;
                }
            }

            int count = 0;
            for (int i = 0; i < tempData.Length; ++i)
            {
                byte[] bytes = BitConverter.GetBytes(tempData[i]);
                foreach (byte b in bytes)
                {
                    output[count++] = b;
                }
            }

        }

        public static DXTBlock GetMergedBlocks(byte[] input,int offset,int alphaOffset)
        {
            DXTBlock block = DXTBlock.FromCompressed(input, offset);
            DXTBlock alphaBlock = DXTBlock.FromCompressed(input, offset+alphaOffset);

            for(int i=0;i<block.DecodedColours.Length;++i)
            {
                Color c = block.DecodedColours[i];
                Color ca = alphaBlock.DecodedColours[i];

                c = Color.FromArgb(ca.G, c.R, c.G, c.B);
                block.DecodedColours[i] = c;
            }

            return block;
        }


        public static uint SwapBytes(uint x)
        {
            return ((x & 0x000000ff) << 24) +
                   ((x & 0x0000ff00) << 8) +
                   ((x & 0x00ff0000) >> 8) +
                   ((x & 0xff000000) >> 24);
        }


        public static Color[] TempColors = new Color[4];

        //public static DXTBlock DecompressBlock(byte[] input, int offset)
        //{
        //    int r0, g0, b0, r1, g1, b1;

        //    ushort q0 = (ushort)(input[offset + 0] | input[offset + 1] << 8);
        //    ushort q1 = (ushort)(input[offset + 2] | input[offset + 3] << 8);

        //    bool hasAlpha = q0 < q1;
        //    hasAlpha = false;

        //    q0 = Swap16(q0);
        //    q1 = Swap16(q1);


        //    Rgb565(q0, out r0, out g0, out b0);
        //    Rgb565(q1, out r1, out g1, out b1);

        //    TempColors[0] = Color.FromArgb(255,r0, g0, b0);
        //    TempColors[1] = Color.FromArgb(255,r1, g1, b1);

        //    if (hasAlpha)
        //    {
        //        TempColors[2] = Color.FromArgb(255, (r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2);
        //        TempColors[3] = Color.FromArgb(0, 0, 0, 0);
        //    }
        //    else
        //    {
        //        TempColors[2] = Color.FromArgb(255, (r0 * 2 + r1) / 3, (g0 * 2 + g1) / 3, (b0 * 2 + b1) / 3);
        //        TempColors[3] = Color.FromArgb(255, (r0 + r1 * 2) / 3, (g0 + g1 * 2) / 3, (b0 + b1 * 2) / 3);
        //    }

        //    DXTBlock block = new DXTBlock();
        //    for (int i = 0; i < 4; ++i)
        //    {
        //        block.DecodedColours[i] = TempColors[i];
        //    }

        //    uint d = BitConverter.ToUInt32(input, offset + 4);


        //    for (int y = 0; y < 4; y++)
        //    {
        //        // swap order here to get correct picture.
        //        for (int x = 3; x >= 0; --x)
        //        {
        //            block.LineIndices[(y * 4) + x] = (uint)(d & 3);
        //            d >>= 2;
        //        }
        //    }

        //    for (int i = 0; i < block.SourceColours.Length; i++)
        //    {
        //        if(hasAlpha && i == 3)
        //        {
        //            block.SourceColours[i] = Color.FromArgb(0, 0, 0, 0);
        //        }
        //        else
        //        {
        //            block.SourceColours[i] = block.DecodedColours[block.LineIndices[i]];
        //        }

        //    }


        //    return block;
        //}




        public static void FillDest(uint[] dst, ref int dstIndex, int offset, int pitch, DXTBlock src)
        {
            int localDstIndex = dstIndex;
            int numiter = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {

                    uint val = src.LineIndices[numiter];
                    //dst[localDstIndex + offset + x] = src.DecodedColours[(int)((val >> 6) & 3)];
                    dst[localDstIndex + offset + x] = (uint)src.DecodedColours[val].ToArgb();
                    numiter++;
                }
                localDstIndex += pitch;
            }
        }




        public static DXTBlock NextBlock(BinaryReader binReader)
        {
            DXTBlock block = new DXTBlock();
            block.color1 = binReader.ReadUInt16();
            block.color2 = binReader.ReadUInt16();

            int index = 0;
            for (int i = 0; i < 4; ++i)
            {
                byte b = binReader.ReadByte();
                for (int x = 0; x < 4; ++x)
                {
                    //block.Lines[index++] = (uint)(b >> x);
                    block.LineIndices[index++] = (uint)(b >> x);
                }
            }
            return block;
        }


        public static int DXTBlend(int v1, int v2)
        {
            // 3/8 blend, which is close to 1/3
            return ((v1 * 3 + v2 * 5) >> 3);
        }

        public static byte Convert3To8(byte v)
        {
            // Swizzle bits: 00000123 -> 12312312
            return (byte)((v << 5) | (v << 2) | (v >> 1));
        }

        public static byte Convert4To8(byte v)
        {
            // Swizzle bits: 00001234 -> 12341234
            return (byte)((v << 4) | v);
        }

        public static byte Convert5To8(byte v)
        {
            // Swizzle bits: 00012345 -> 12345123
            return (byte)((v << 3) | (v >> 2));
        }

        public static byte Convert6To8(byte v)
        {
            // Swizzle bits: 00123456 -> 12345612
            return (byte)((v << 2) | (v >> 4));
        }

        public static uint MakeRGBA(int r, int g, int b, int a)
        {
            return (uint)((a << 24) | (b << 16) | (g << 8) | r);
        }


        public static ushort Swap16(ushort x)
        {
            //return BinaryPrimitives.ReverseEndianness(val);
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rgb565(uint c, out int r, out int g, out int b)
        {
            r = ((int)c & 0xf800) >> 8;
            g = ((int)c & 0x07e0) >> 3;
            b = ((int)c & 0x001f) << 3;
            r |= r >> 5;
            g |= g >> 6;
            b |= b >> 5;
        }

        public static void Rgb565Swizzle(uint c, out int r, out int g, out int b)
        {
            b = Convert5To8((byte)(c & 0x1F));
            g = Convert6To8((byte)((c >> 5) & 0x3F));
            r = Convert5To8((byte)((c >> 11) & 0x1F));

            r |= r >> 5;
            g |= g >> 6;
            b |= b >> 5;
        }


        public static List<DXTBlock> ReadBitmap(DirectBitmap bitmap)
        {

            List<DXTBlock> blockList = new List<DXTBlock>();

            byte[] bytes = bitmap.Bits;

            Color[] pixels = new Color[bytes.Length / 4];
            int count = 0;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                pixels[count++] = Color.FromArgb(bytes[i + 0], bytes[i + 1], bytes[i + 2], bytes[i + 3]);
            }

            for (int y = 0; y < bitmap.Height; y += 4)
            {
                for (int x = 0; x < bitmap.Width; x += 4)
                {
                    int offset = (y * bitmap.Width) + x;
                    blockList.Add(DXTBlock.FromUncompressed(pixels, offset, bitmap.Width));
                }

            }
            int ibreak = 0;
            return blockList;

        }




        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ColorPacked(int r, int g, int b, int a)
        {
            return (uint)(r << 16 | g << 8 | b | a << 24);
        }

        public static byte[] ProcessBlockList(List<DXTBlock> blocks)
        {
            byte[] results = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                using (BinaryWriter binWriter = new BinaryWriter(memStream))
                {
                    foreach (DXTBlock block in blocks)
                    {
                        DXTBlock resultBlock = BlockEncoder.Bc1BlockEncoderSlow.EncodeBlock(block);
                        resultBlock.Write(binWriter);

                    }
                    results = memStream.ToArray();
                }
            }
            return results;
        }



        public static void TestReencode(string reencodePath)
        {
            String basePath = @"F:\UnityProjects\GladiusDFGui\Assets\Resources\Textures\";
            string filename = "endlesshorizons_tournament.png";

            System.IO.DirectoryInfo targetInfo = new DirectoryInfo(basePath);
            if (!targetInfo.Exists)
            {
                targetInfo.Create();
            }



            DirectBitmap directBitmap = new DirectBitmap(basePath+filename);
            int width = directBitmap.Width;
            int height = directBitmap.Height;

            List<DXTBlock> blockList = ReadBitmap(directBitmap);
            byte[] processResults = ProcessBlockList(blockList);


            DirectBitmap reencodedBitmap = new DirectBitmap(width, height);
            byte[] output = new byte[width * height * 4];
            DecompressDXT1GC(processResults, width, height, output);
            
            for(int i=0;i<output.Length;++i)
            {
                if(output[i] != 0)
                {
                    int ibreak = 0;
                }
            }


            DecompressDXT1GC(processResults, width, height, reencodedBitmap.Bits);

            reencodedBitmap.Bitmap.Save(reencodePath + filename, ImageFormat.Png);
            reencodedBitmap.Dispose();


        }

        public static void TestExtract(string sourcePath,string outputDirectory)
        {
            new ImageExtractor().ProcessImages(sourcePath, outputDirectory);
        }

        static void Main(string[] args)
        {
            string baseInput = @"M:\GladiusISOWorkingExtracted\python-gc\gc\data\texture\"; 
            string baseOutput = @"m:\tmp\gladius\";
            
            string sourcePath = baseInput+@"gui\leagues\";

            string outputDirectory = baseOutput+@"textures-gc\";
            string reencodedOutputDirectory = baseOutput + @"textures-gc-reencoded\";

            TestExtract(sourcePath, outputDirectory);
            //TestReencode(reencodedOutputDirectory);




            int ibreak = 0;
        }
    }
    public sealed class DirectBitmap : IDisposable
    {
        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new byte[width * height * 4];
            m_bitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Stride, PixelFormat.Format32bppArgb, m_bitsHandle.AddrOfPinnedObject());
        }

        public DirectBitmap(string filename)
        {
            Bitmap = new Bitmap(filename);
            Width = Bitmap.Width;
            Height = Bitmap.Height;

            BitmapData bitmapData = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Bits = new byte[Math.Abs(bitmapData.Stride * bitmapData.Height)];
            Marshal.Copy(bitmapData.Scan0, Bits, 0, Bits.Length);


            int ibreak = 0;
        }


        ~DirectBitmap()
        {
            Dispose(false);
        }

        public void SetPixel(int x, int y, System.Drawing.Color color)
        {
            int index = x + (y * Width);
            unchecked
            {
                uint value = (uint)color.ToArgb();
                Bits[index + 0] = (byte)(value >> 0);
                Bits[index + 1] = (byte)(value >> 8);
                Bits[index + 2] = (byte)(value >> 16);
                Bits[index + 3] = (byte)(value >> 24);
            }
        }

        public System.Drawing.Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            uint col = BitConverter.ToUInt32(Bits, index);
            return System.Drawing.Color.FromArgb(unchecked((int)col));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                Bitmap.Dispose();
                if (m_bitsHandle != null && m_bitsHandle.IsAllocated)
                {
                    m_bitsHandle.Free();
                }
                m_disposed = true;
            }
        }

        public int Height { get; }
        public int Width { get; }
        public int Stride => Width * 4;
        public Bitmap Bitmap { get; }

        public byte[] Bits = null;
        public IntPtr BitsPtr => m_bitsHandle.AddrOfPinnedObject();

        private readonly GCHandle m_bitsHandle;
        private bool m_disposed;
    }

    public class DXTBlock
    {
        public ushort color1;
        public ushort color2;

        public uint[] LineIndices = new uint[16];
        //public uint[] Lines = new uint[16];

        public Color[] DecodedColours = new Color[4];
        public Color[] SourceColours = new Color[16];


        public ColorRgb565 CalculatedColor0 = new ColorRgb565();
        public ColorRgb565 CalculatedColor1 = new ColorRgb565();

        public bool HasTransparentPixels()
        {
            for (var i = 0; i < DecodedColours.Length; i++)
            {
                if (DecodedColours[i].A < 255) return true;
            }
            return false;
        }



        public bool HasAlphaOrBlack
        {
            get
            {
                return false;
                //return CalculatedColor0.data <= CalculatedColor1.data;
            }
        }

        public static DXTBlock FromUncompressed(Color[] data, int offset, int stride)
        {
            DXTBlock block = new DXTBlock();
            int count = 0;
            for (int y = 0; y < 4; ++y)
            {
                for (int x = 0; x < 4; ++x)
                {
                    block.SourceColours[count++] = data[offset + x];
                }
                offset += stride;
            }
            return block;
        }

        public void Write(BinaryWriter binWriter)
        {
            binWriter.Write(CalculatedColor0.data);
            binWriter.Write(CalculatedColor1.data);

            int packedIndices = 0;
            int count = 0;
            foreach (int index in LineIndices)
            {
                packedIndices |= (index << count++);
            }

            binWriter.Write(packedIndices);
        }


        public static DXTBlock FromCompressed(byte[] input, int offset)
        {
            DXTBlock block = new DXTBlock();
            int r0, g0, b0, r1, g1, b1;

            ushort q0 = (ushort)(input[offset + 0] | input[offset + 1] << 8);
            ushort q1 = (ushort)(input[offset + 2] | input[offset + 3] << 8);

            q0 = ImageExtractor.Swap16(q0);
            q1 = ImageExtractor.Swap16(q1);


            ImageExtractor.Rgb565(q0, out r0, out g0, out b0);
            ImageExtractor.Rgb565(q1, out r1, out g1, out b1);

            ImageExtractor.TempColors[0] = Color.FromArgb(255, r0, g0, b0);
            ImageExtractor.TempColors[1] = Color.FromArgb(255, r1, g1, b1);
            if (q0 > q1)
            {
                ImageExtractor.TempColors[2] = Color.FromArgb(255, (r0 * 2 + r1) / 3, (g0 * 2 + g1) / 3, (b0 * 2 + b1) / 3);
                ImageExtractor.TempColors[3] = Color.FromArgb(255, (r0 + r1 * 2) / 3, (g0 + g1 * 2) / 3, (b0 + b1 * 2) / 3);
            }
            else
            {
                ImageExtractor.TempColors[2] = Color.FromArgb(255, (r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2);
            }

            for (int i = 0; i < 4; ++i)
            {
                block.DecodedColours[i] = ImageExtractor.TempColors[i];
            }



            uint d = BitConverter.ToUInt32(input, offset + 4);


            for (int y = 0; y < 4; y++)
            {
                // swap order here to get correct picture.
                for (int x = 3; x >= 0; --x)
                {
                    block.LineIndices[(y * 4) + x] = (uint)(d & 3);
                    d >>= 2;
                }
            }

            //    for (int i = 0; i < block.SourceColours.Length; i++)
            //    {
            //        if(hasAlpha && i == 3)
            //        {
            //            block.SourceColours[i] = Color.FromArgb(0, 0, 0, 0);
            //        }
            //        else
            //        {
            //            block.SourceColours[i] = block.DecodedColours[block.LineIndices[i]];
            //        }

            //    }

            return block;

        }

    };


}
