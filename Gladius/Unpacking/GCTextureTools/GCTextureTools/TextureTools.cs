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
using System.Buffers.Binary;
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

        public void ProcessImages()
        {
            List<string> fileNames = new List<string>();
            String sourcePath = @"D:\gladius-extracted-archive\ps2-decompressed\ClassImages\";
            sourcePath = @"E:\gladius-extracted-archive\xbox-decompressed\PTTPFiles";
            sourcePath = @"D:\GladiusISOWorkingExtracted\python-gc\gc\data\texture\gui\leagues\";

            fileNames.AddRange(Directory.GetFiles(sourcePath, "**"));
            String outputDirectory = @"E:\gladius-extracted-archive\skygold-texture-output\";
            outputDirectory = @"d:\tmp\gladius\textures-gc\";

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

                    //potWidth *= 2;
                    //potHeight *= 2;

                    image.DirectBitmap = new DirectBitmap(potWidth, potHeight);



                    DecompressDXT1GC(image.CompressedData, potWidth, potHeight, image.DirectBitmap.Bits);
                    //Decode(potWidth, potHeight, image.CompressedData, image.DirectBitmap.Bits);

                    //Decode2Converter(potWidth, potHeight, image.CompressedData, ref image.DirectBitmap.Bits);
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

        public static void DecompressDXT1(byte[] input, int width, int height, byte[] output)
        {
            int offset = 0;
            int bcw = (width + 3) / 4;
            int bch = (height + 3) / 4;
            int clen_last = (width + 3) % 4 + 1;
            uint[] buffer = new uint[16];
            uint[] colors = new uint[4];

            int yblock = 0;

            for (int t = 0; t < bch; t++)
            {
                for (int s = 0; s < bcw; s++, offset += 8)
                {
                    int r0, g0, b0, r1, g1, b1;
                    int q0 = input[offset + 0] | input[offset + 1] << 8;
                    int q1 = input[offset + 2] | input[offset + 3] << 8;

                    //Rgb565(q0, out r0, out g0, out b0);
                    //Rgb565(q1, out r1, out g1, out b1);

                    Rgb565Swizzle(q0, out r0, out g0, out b0);
                    Rgb565Swizzle(q1, out r1, out g1, out b1);


                    colors[0] = Color(r0, g0, b0, 255);
                    colors[1] = Color(r1, g1, b1, 255);
                    if (q0 > q1)
                    {
                        colors[2] = Color((r0 * 2 + r1) / 3, (g0 * 2 + g1) / 3, (b0 * 2 + b1) / 3, 255);
                        colors[3] = Color((r0 + r1 * 2) / 3, (g0 + g1 * 2) / 3, (b0 + b1 * 2) / 3, 255);
                    }
                    else
                    {
                        colors[2] = Color((r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2, 255);
                    }

                    uint d = BitConverter.ToUInt32(input, offset + 4);
                    for (int i = 0; i < 16; i++, d >>= 2)
                    {
                        buffer[i] = unchecked((uint)colors[d & 3]);
                    }

                    int clen = (s < bcw - 1 ? 4 : clen_last) * 4;
                    for (int i = 0, y = t * 4; i < 4 && y < height; i++, y++)
                    {
                        Buffer.BlockCopy(buffer, i * 4 * 4, output, (y * width + s * 4) * 4, clen);
                    }
                }
                yblock++;
            }
            int ibreak = 0;
        }


        public static void DecompressDXT1GC(byte[] input, int width, int height, byte[] output)
        {
            int offset = 0;
            int bcw = (width + 3) / 4;
            int bch = (height + 3) / 4;
            int clen_last = (width + 3) % 4 + 1;
            uint[] buffer = new uint[16];
            uint[] colors = new uint[4];
            int yblock = 0;
            int dstIndex = 0;
            int increment = 8;

            uint[] tempData = new uint[width * height];

            for (int y = 0; y < height; y += increment)
            {
                for (int x = 0; x < width; x += increment)

                {
                    DXTBlock block = DecompressBlock(input, offset, colors, buffer);
                    FillDest(tempData, ref dstIndex, (y * width + x), width, block);
                    offset += increment;

                    block = DecompressBlock(input, offset, colors, buffer);
                    FillDest(tempData, ref dstIndex, (y * width + x + 4), width, block);
                    offset += increment;


                    block = DecompressBlock(input, offset, colors, buffer);
                    FillDest(tempData, ref dstIndex, ((y + 4) * width + x), width, block);
                    offset += increment;

                    block = DecompressBlock(input, offset, colors, buffer);
                    FillDest(tempData, ref dstIndex, ((y + 4) * width + x + 4), width, block);
                    offset += increment;

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


        public static DXTBlock DecompressBlock(byte[] input, int offset, uint[] colors, uint[] buffer)
        {
            int r0, g0, b0, r1, g1, b1;
            int q0 = input[offset + 0] | input[offset + 1] << 8;
            int q1 = input[offset + 2] | input[offset + 3] << 8;

            //Rgb565(q0, out r0, out g0, out b0);
            //Rgb565(q1, out r1, out g1, out b1);

            Rgb565Swizzle(q0, out r0, out g0, out b0);
            Rgb565Swizzle(q1, out r1, out g1, out b1);


            colors[0] = Color(r0, g0, b0, 255);
            colors[1] = Color(r1, g1, b1, 255);
            if (q0 > q1)
            {
                colors[2] = Color((r0 * 2 + r1) / 3, (g0 * 2 + g1) / 3, (b0 * 2 + b1) / 3, 255);
                colors[3] = Color((r0 + r1 * 2) / 3, (g0 + g1 * 2) / 3, (b0 + b1 * 2) / 3, 255);
            }
            else
            {
                colors[2] = Color((r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2, 255);
            }

            uint d = BitConverter.ToUInt32(input, offset + 4);
            for (int i = 0; i < 16; i++, d >>= 2)
            {
                buffer[i] = unchecked((uint)colors[d & 3]);
            }

            DXTBlock block = new DXTBlock();
            for (int i = 0; i < 4; ++i)
            {
                block.resultColours[i] = colors[i];
            }


            for (int i = 0; i < block.lines.Length; i++)
            {
                // fill in block...

            }
            //for (int i = 0; i < buffer.Length; ++i)
            //{
            //    block.lines = 
            //}

            return block;
        }


        public static void Decode(int height, int width, byte[] srcInfo, byte[] dst)
        {
            int dstIndex = 0;
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(srcInfo)))
            {
                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        DXTBlock block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest(dst, ref dstIndex, (y * width + x), width, block);

                        block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest(dst, ref dstIndex, (y * width + x + 4), width, block);

                        block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest(dst, ref dstIndex, ((y + 4) * width + x), width, block);

                        block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest(dst, ref dstIndex, ((y + 4) * width + x + 4), width, block);
                    }
                }
            }
        }

        public static void Decode2Converter(int height, int width, byte[] srcInfo, ref byte[] dst)
        {
            uint[] dc2dst = new uint[width * height];
            Decode2(height, width, srcInfo, dc2dst);
            int dstIndex = 0;
            foreach (uint val in dc2dst)
            {
                byte[] bytes = BitConverter.GetBytes(val);
                foreach (byte b in bytes)
                {
                    dst[dstIndex++] = b;
                }
            }
        }

        public static void Decode2(int height, int width, byte[] srcInfo, uint[] dst)
        {
            int dstIndex = 0;
            int yblocks = 0;
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(srcInfo)))
            {
                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        DXTBlock block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest2(dst, ref dstIndex, (y * width + x), width, block);

                        block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest2(dst, ref dstIndex, (y * width + x + 4), width, block);

                        block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest2(dst, ref dstIndex, ((y + 4) * width + x), width, block);

                        block = NextBlock(binReader);
                        DecodeDXTBlock(block);
                        FillDest2(dst, ref dstIndex, ((y + 4) * width + x + 4), width, block);
                    }
                    yblocks++;
                }
            }
            int ibreak = 0;
        }



        public static void FillDest(byte[] dst, ref int dstIndex, int offset, int pitch, DXTBlock src)
        {
            // byte to uint
            pitch *= 4;
            int numiter = 0;
            int localDstIndex = dstIndex;
            for (int y = 0; y < 4; y++)
            {
                int val = src.lines[y];
                for (int x = 0; x < 4; x++)
                {
                    byte[] bytes = BitConverter.GetBytes(src.resultColours[(int)((val >> 6) & 3)]);
                    for (int i = 0; i < bytes.Length; ++i)
                    {
                        dst[localDstIndex + i + offset + x] = bytes[i];
                        numiter++;
                    }
                    val <<= 2;
                    //dstIndex += 4;
                }
                //dst += pitch;
                localDstIndex += pitch;
            }
            int ibreak = 0;
        }


        public static void FillDest(uint[] dst, ref int dstIndex, int offset, int pitch, DXTBlock src)
        {
            int localDstIndex = dstIndex;
            for (int y = 0; y < 4; y++)
            {
                int val = src.lines[y];
                for (int x = 0; x < 4; x++)
                {
                    dst[localDstIndex + offset + x] = src.resultColours[(int)((val >> 6) & 3)];
                    val <<= 2;
                }
                localDstIndex += pitch;
            }
        }



        //public static void FillDest(byte[] dst, ref int dstIndex, int offset, int pitch, DXTBlock src)
        //{
        //    int localDstIndex = dstIndex;
        //    for (int y = 0; y < 4; y++)
        //    {
        //        int val = src.lines[y];
        //        for (int x = 0; x < 4; x++)
        //        {
        //            byte[] bytes = BitConverter.GetBytes(src.resultColours[(int)((val >> 6) & 3)]);
        //            for (int i = 0; i < bytes.Length; ++i)
        //            {
        //                dst[dstIndex++] = bytes[i];
        //            }
        //            val <<= 2;
        //        }
        //        //dst += pitch;
        //        localDstIndex += pitch;
        //    }
        //}


        public static void FillDest2(uint[] dst, ref int dstIndex, int offset, int pitch, DXTBlock src)
        {
            int localDstIndex = dstIndex;
            for (int y = 0; y < 4; y++)
            {
                int val = src.lines[y];
                for (int x = 0; x < 4; x++)
                {
                    dst[localDstIndex + offset] = src.resultColours[(int)((val >> 6) & 3)];
                    val <<= 2;
                    //dstIndex += 4;
                }
                //dst += pitch;
                //localDstIndex += pitch;
            }
        }



        public static DXTBlock NextBlock(BinaryReader binReader)
        {
            DXTBlock block = new DXTBlock();
            block.color1 = binReader.ReadUInt16();
            block.color2 = binReader.ReadUInt16();
            for (int i = 0; i < block.lines.Length; ++i)
            {
                block.lines[i] = binReader.ReadByte();
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


        public static ushort Swap16(ushort val)
        {
            return BinaryPrimitives.ReverseEndianness(val);
        }


        public static void DecodeDXTBlock(DXTBlock src)
        {
            // S3TC Decoder (Note: GCN decodes differently from PC so we can't use native support)
            // Needs more speed.
            ushort c1 = Swap16(src.color1);
            ushort c2 = Swap16(src.color2);
            int blue1 = Convert5To8((byte)(c1 & 0x1F));
            int blue2 = Convert5To8((byte)(c2 & 0x1F));
            int green1 = Convert6To8((byte)((c1 >> 5) & 0x3F));
            int green2 = Convert6To8((byte)((c2 >> 5) & 0x3F));
            int red1 = Convert5To8((byte)((c1 >> 11) & 0x1F));
            int red2 = Convert5To8((byte)((c2 >> 11) & 0x1F));
            src.resultColours[0] = MakeRGBA(red1, green1, blue1, 255);
            src.resultColours[1] = MakeRGBA(red2, green2, blue2, 255);
            if (c1 > c2)
            {
                src.resultColours[2] =
                MakeRGBA(DXTBlend(red2, red1), DXTBlend(green2, green1), DXTBlend(blue2, blue1), 255);
                src.resultColours[3] =
                MakeRGBA(DXTBlend(red1, red2), DXTBlend(green1, green2), DXTBlend(blue1, blue2), 255);
            }
            else
            {
                // color[3] is the same as color[2] (average of both colors), but transparent.
                // This differs from DXT1 where color[3] is transparent black.
                src.resultColours[2] = MakeRGBA((red1 + red2) / 2, (green1 + green2) / 2, (blue1 + blue2) / 2, 255);
                src.resultColours[3] = MakeRGBA((red1 + red2) / 2, (green1 + green2) / 2, (blue1 + blue2) / 2, 0);
            }
        }



        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Rgb565(int c, out int r, out int g, out int b)
        {
            r = (c & 0xf800) >> 8;
            g = (c & 0x07e0) >> 3;
            b = (c & 0x001f) << 3;
            r |= r >> 5;
            g |= g >> 6;
            b |= b >> 5;
        }

        private static void Rgb565Swizzle(int c, out int r, out int g, out int b)
        {
            b = Convert5To8((byte)(c & 0x1F));
            g = Convert6To8((byte)((c >> 5) & 0x3F));
            r = Convert5To8((byte)((c >> 11) & 0x1F));

            r |= r >> 5;
            g |= g >> 6;
            b |= b >> 5;
        }




        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Color(int r, int g, int b, int a)
        {
            return (uint)(r << 16 | g << 8 | b | a << 24);
        }

        static void Main(string[] args)
        {
            new ImageExtractor().ProcessImages();
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
            //Bitmap = new Bitmap(Width, Height, Stride, PixelFormat.Format32bppArgb, m_bitsHandle.AddrOfPinnedObject());
            //Bitmap = new Bitmap(Width, Height, Stride, PixelFormat.Format32bppArgb, m_bitsHandle.AddrOfPinnedObject());
            Bitmap = new Bitmap(Width, Height, Stride, PixelFormat.Format32bppArgb, m_bitsHandle.AddrOfPinnedObject());
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
                m_bitsHandle.Free();
                m_disposed = true;
            }
        }

        public int Height { get; }
        public int Width { get; }
        public int Stride => Width * 4;
        public Bitmap Bitmap { get; }
        //public byte[] Bits { get; }

        public byte[] Bits = null;
        public IntPtr BitsPtr => m_bitsHandle.AddrOfPinnedObject();

        private readonly GCHandle m_bitsHandle;
        private bool m_disposed;
    }

    public class DXTBlock
    {
        public ushort color1;
        public ushort color2;
        public byte[] lines = new byte[4];

        public uint[] resultColours = new uint[4];


    };


}
