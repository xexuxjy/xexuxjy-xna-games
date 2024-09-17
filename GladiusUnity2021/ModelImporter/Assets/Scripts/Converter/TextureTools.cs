using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using Color = System.Drawing.Color;

// lots of help from https://github.com/Nominom/BCnEncoder.NET
// and https://github.com/mafaca/Dxt

namespace GCTextureTools
{
    public class GCGladiusImage
    {
        //private TextureHeaderInfo m_header = null;
        private string strFileName = string.Empty;
        public string ImageName;

        public byte HeaderSize;
        public byte Type;
        public byte MipCount;
        public byte BitDepth;

        public ushort UStart;
        public ushort VStart;
        
        public string Name;
        public ushort Width;
        public ushort Height;
    
        public int Size;
        public int Offset;
        public int NameOffset;
        
        
        public bool ContainsDefinition;
        public ushort DXTType = 0;
        public ushort CompressType;

        
        public DirectBitmap DirectBitmap;

        public byte[] CompressedData;


        public string FileName
        {
            get { return this.strFileName; }
        }


        public string DebugInfo
        {
        
        get {
            return
                $" HdrSz : {HeaderSize} DXT : {DXTType} MipCnt : {MipCount} BitDepth : {BitDepth}  Us : {UStart} Vs : {VStart}  W : {Width} H : {Height}  Size: {Size} Offset : {Offset} NOffset : {NameOffset}";
        }
    }

        public string DebugInfoCSV
        {

            get
            {
                return
                    $"{HeaderSize},{DXTType} , {MipCount}, {BitDepth} , {UStart}, {VStart} , {Width}, {Height}  , {Size} , {Offset}, {NameOffset}";
            }
        }

        public static GCGladiusImage FromStream(BinaryReader reader)
        {
            GCGladiusImage gcGladiusImage = new GCGladiusImage();

            gcGladiusImage.HeaderSize = reader.ReadByte();
            gcGladiusImage.DXTType = reader.ReadByte();
            gcGladiusImage.MipCount = reader.ReadByte();
            gcGladiusImage.BitDepth = reader.ReadByte();

            gcGladiusImage.UStart = reader.ReadUInt16();
            gcGladiusImage.VStart = reader.ReadUInt16();
            
            gcGladiusImage.Width = reader.ReadUInt16();
            gcGladiusImage.Height = reader.ReadUInt16();
            gcGladiusImage.Size = reader.ReadInt32();
            gcGladiusImage.Offset = reader.ReadInt32();
            gcGladiusImage.NameOffset = reader.ReadInt32();

            // padding
            reader.ReadInt32();
            reader.ReadInt32();
            
            // need to store these.
            
            return gcGladiusImage;
        }
    }


    // public class GCGladiusImageHeader
    // {
    //     public int Width = 0;
    //     public int Height = 0;
    //     public int CompressedSize = 0;
    //     public bool ContainsDefinition;
    //     public ushort DXTType = 0;
    // }


    public class GCImageExtractor
    {
        public static Color[] TempColors = new Color[4];

        public static void ClearTempColours()
        {
            for (int i = 0; i < TempColors.Length; ++i)
            {
                TempColors[i] = Color.FromArgb(0, 0, 0, 0);
            }
        }

        public void ExtractImages(string sourceDirectory, string targetDirectory)
        {
            List<string> fileNames = new List<string>();
            fileNames.AddRange(Directory.GetFiles(sourceDirectory, "*"));
            ExtractImages(fileNames, targetDirectory);
        }

        public void ProcessImages(String sourcePath, String outputDirectory)
        {
            List<string> fileNames = new List<string>();
            FileAttributes attr = File.GetAttributes(sourcePath);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                FileInfo fileInfo = new FileInfo(sourcePath);
                fileNames.AddRange(Directory.GetFiles(sourcePath, "**"));
            }
            else
            {
                fileNames.Add(sourcePath);
            }
            ExtractImages(fileNames, outputDirectory);
        }


        public void ReadPTDTSection(BinaryReader binReader, List<GCGladiusImage> imageList, StringBuilder debugInfo)
        {
            if (Common.FindCharsInStream(binReader, Common.ptdtTag))
            {
                uint sectionSize = binReader.ReadUInt32();
                int skip = 8;
                int adjustedSize = (int)sectionSize - skip;
                binReader.BaseStream.Position += skip;


                foreach (GCGladiusImage image in imageList)
                {
                    image.CompressedData = binReader.ReadBytes(image.Size);

                    int potWidth = image.Width; // ToNextNearest(image.Header.Width);
                    int potHeight = image.Height; // ToNextNearest(image.Header.Height);

                    //image.DirectBitmap = new DirectBitmap(potWidth, potHeight);

                    DecompressDXT1GC(image, debugInfo);
                }
            }
        }

        int ToNextNearest(int x)
        {
            if (x < 0)
            {
                return 0;
            }

            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        public void ExtractImagesChunk(List<string> fileNames, string targetDirectory)
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

            StringBuilder debugInfo = new StringBuilder();

            foreach (string fileName in fileNames)
            {
                FileInfo file = new FileInfo(fileName);
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                {
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        List<GCGladiusImage> imageList = LoadDataChunk(binReader, debugInfo);
                        foreach (GCGladiusImage gi in imageList)
                        {
                            if (gi.DirectBitmap != null)
                            {
                                gi.DirectBitmap.Bitmap.Save(targetDirectory + gi.ImageName + ".png",
                                    ImageFormat.Png);
                                gi.DirectBitmap.Dispose();
                            }

                            gi.CompressedData = null;
                        }
                    }
                }
            }
        }

        public static List<GCGladiusImage> LoadDataChunk(BinaryReader binReader, StringBuilder debugInfo)
        {
            string name = "";
            List<BaseChunk> chunkList = new List<BaseChunk>();

            binReader.BaseStream.Position = 0;
            int count = 0;

            do
            {
                int position = (int)binReader.BaseStream.Position;
                BaseChunk chunk = BaseChunk.FromStreamMaster(name, binReader, debugInfo);
                if (chunk != null)
                {
                    chunkList.Add(chunk);

                    if (chunk is EndChunk)
                    {
                        break;
                    }

                    binReader.BaseStream.Position = position + chunk.Length;
                }
            } while (count++ < 100);


            PTTPChunk pttpChunk = (PTTPChunk)chunkList.Find(x => x is PTTPChunk);
            NAMEChunk nameChunk = (NAMEChunk)chunkList.Find(x => x is NAMEChunk);
            NMTPChunk nmptChunk = (NMTPChunk)chunkList.Find(x => x is NMTPChunk);
            PFHDChunk pfhdChunk = (PFHDChunk)chunkList.Find(x => x is PFHDChunk);
            PTDTChunk ptdtChunk = (PTDTChunk)chunkList.Find(x => x is PTDTChunk);

            List<GCGladiusImage> imageList = pfhdChunk.ProcessData(nmptChunk.Data, debugInfo);
            ptdtChunk.ProcessData(imageList, debugInfo);

            return imageList;
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

            StringBuilder debugInfo = new StringBuilder();

            foreach (string fileName in fileNames)
            {
                List<GCGladiusImage> imageList = new List<GCGladiusImage>();
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
                            ReadPTDTSection(binReader, imageList, debugInfo);

                            foreach (GCGladiusImage gi in imageList)
                            {
                                if (gi.DirectBitmap != null)
                                {
                                    gi.DirectBitmap.Bitmap.Save(targetDirectory + gi.ImageName + ".png",
                                        ImageFormat.Png);
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

            using (StreamWriter errorStream =
                   new StreamWriter(new FileStream(targetDirectory + "\\errors.txt", FileMode.OpenOrCreate)))
            {
                errorStream.WriteLine(debugInfo.ToString());
            }

            int stophere = 0;
        }


        public void ReadHeaderSection(BinaryReader binReader, List<GCGladiusImage> imageList, List<string> textureNames)
        {
            if (Common.FindCharsInStream(binReader, Common.pfhdTag, true))
            {
                int sectionSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();

                int numTextures = binReader.ReadInt32();

                for (int u = 0; u < numTextures; ++u)
                {
                    GCGladiusImage image = GCGladiusImage.FromStream(binReader);
                    image.ImageName = textureNames[u];
                    imageList.Add(image);
                }
            }
        }


        public static void DecompressDXT1GC(GCGladiusImage image, StringBuilder debugInfo)
        {
            int offset = 0;

            byte[] input = image.CompressedData;

            int width = image.Width;
            int height = image.Height;

            int blockCountX = (width + 3) / 4;
            int blockCountY = (height + 3) / 4;


            int alphaOffset = input.Length / 2;

            int blockStorage = 0;

            int lastBlockStorage = 0;

            bool oddXBlocks = blockCountX % 2 == 1;
            bool oddYBlocks = blockCountY % 2 == 1;

            if (oddXBlocks)
            {
                blockCountX += 1;
            }

            if (oddXBlocks)
            {
                blockCountY += 1;
            }

            width = blockCountX * 4;
            height = blockCountY * 4;


            DXTBlock block = null;
            DXTBlock alphaBlock = null;


            uint[] tempData = new uint[width * height];
            uint[] tempData2 = new uint[tempData.Length];

            GetBlocks(input, blockStorage, alphaOffset, out block, out alphaBlock, debugInfo);
            DXTBlock padBlock = block;
            DXTBlock padAlphaBlock = alphaBlock;

            image.DirectBitmap = new DirectBitmap(width, height);
            byte[] output = image.DirectBitmap.Bits;

            for (int y = 0; y < blockCountY; y++)
            {
                lastBlockStorage = blockStorage;

                for (int x = 0; x < blockCountX; x++)
                {
                    try
                    {
                        try
                        {
                            GetBlocks(input, blockStorage, alphaOffset, out block, out alphaBlock, debugInfo);
                        }
                        catch (Exception e)
                        {
                            int ibreak = 0;
                        }

                        try
                        {
                            FillDest2(tempData2, x, y, width, block, alphaBlock, debugInfo);
                        }
                        catch (Exception e)
                        {
                            int ibreak = 0;
                        }

                        if (x < blockCountX - 1)
                        {
                            if (x % 2 == 0)
                            {
                                blockStorage += 8;
                            }
                            else
                            {
                                blockStorage += 24;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        int ibreak = 0;
                    }
                }

                if (oddXBlocks)
                {
                    FillDest2(tempData2, blockCountX, y, width, padBlock, padAlphaBlock, debugInfo);
                }


                if (y % 2 == 0)
                {
                    blockStorage = lastBlockStorage + 16;
                }
                else
                {
                    blockStorage += 8;
                }
            }

            if (oddYBlocks)
            {
                for (int i = 0; i < blockCountX; ++i)
                {
                    FillDest2(tempData2, i, blockCountY, width, padBlock, padAlphaBlock, debugInfo);
                }
            }

            //File.WriteAllText(@"d:\tmp\gladius-textures\new-order.txt", debugOut1.ToString());
            //File.WriteAllText(@"d:\tmp\gladius-textures\old-order.txt", debugOut2.ToString());

            try
            {
                int count = 0;
                for (int i = 0; i < tempData.Length; ++i)
                {
                    byte[] bytes = BitConverter.GetBytes(tempData2[i]);
                    foreach (byte b in bytes)
                    {
                        output[count++] = b;
                    }
                }
            }
            catch (Exception e)
            {
                int ibreak = 0;
            }
        }

        public static void GetBlocks(byte[] input, int offset, int alphaOffset, out DXTBlock block,
            out DXTBlock alphaBlock,
            StringBuilder debugOut = null)
        {
            if (debugOut != null)
            {
                debugOut.AppendLine("" + offset);
            }

            block = DXTBlock.FromCompressed(input, offset);
            alphaBlock = null;
            //alphaBlock = DXTBlock.FromCompressed(input, offset+alphaOffset);
        }


        public static uint SwapBytes(uint x)
        {
            return ((x & 0x000000ff) << 24) +
                   ((x & 0x0000ff00) << 8) +
                   ((x & 0x00ff0000) >> 8) +
                   ((x & 0xff000000) >> 24);
        }


        public static void FillDest(uint[] dst, int offset, int pitch, DXTBlock block, DXTBlock alphaBlock)
        {
            int localDstIndex = 0;
            int numiter = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int index = (y * 4) + x;

                    int val = block[index];
                    Color blockColour = block.DecodedColours[val];

                    Color alphaColour = Color.White;
                    if (alphaBlock != null)
                    {
                        int alphaVal = alphaBlock[index];
                        alphaColour = alphaBlock.DecodedColours[alphaVal];
                    }

                    //Color resultColor = alphaColour;
                    Color resultColor = Color.FromArgb(alphaColour.G, blockColour.R, blockColour.G, blockColour.B);

                    dst[localDstIndex + offset + x] = (uint)resultColor.ToArgb();
                    numiter++;
                }

                localDstIndex += pitch;
            }

            int ibreak = 0;
        }

        public static void FillDest2(uint[] dst, int xblock, int yblock, int pitch, DXTBlock block, DXTBlock alphaBlock,
            StringBuilder debugInfo)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int index = (y * 4) + x;

                    int val = block[index];
                    Color blockColour = block.DecodedColours[val];

                    Color alphaColour = Color.White;
                    if (alphaBlock != null)
                    {
                        int alphaVal = alphaBlock[index];
                        alphaColour = alphaBlock.DecodedColours[alphaVal];
                    }

                    Color resultColor = Color.FromArgb(alphaColour.G, blockColour.R, blockColour.G, blockColour.B);

                    int yindex = (yblock * 4) + y;
                    int xindex = (xblock * 4) + x;

                    int destIndex = (yindex * pitch) + xindex;
                    dst[destIndex] = (uint)resultColor.ToArgb();
                    debugInfo.AppendLine($"Filling (di,i,x,y) : {destIndex},{index},{x}, {y} with {resultColor}");
                }
            }

            int ibreak = 0;
        }


        public static void FillDestSource(uint[] dst, ref int dstIndex, int offset, int pitch, DXTBlock src)
        {
            int localDstIndex = dstIndex;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    dst[localDstIndex + offset + x] = (uint)src.SourceColours[(y * 4) + x].ToArgb();
                }

                localDstIndex += pitch;
            }
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

        public static ushort Swap16(ushort x)
        {
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


        public static void ReadColours(UnityEngine.Color[] colours, int width, int height,
            List<DXTBlock> colorBlockList,
            List<DXTBlock> alphaBlockList)
        {
            bool hasAlpha = true;
            foreach (UnityEngine.Color c in colours)
            {
                if (c.a != 1.0f)
                {
                    hasAlpha = true;
                    break;
                }
            }
            
            UnityEngine.Color[] unpackedColourPixels = new UnityEngine.Color[colours.Length];
            UnityEngine.Color[] unpackedAlphaPixels = new UnityEngine.Color[colours.Length];

            int count = 0;
            foreach (UnityEngine.Color c in colours)
            {
                unpackedColourPixels[count] = new UnityEngine.Color(c.r, c.g, c.b, 1.0f);
                //unpackedAlphaPixels[count] = new UnityEngine.Color(0.0f, c.g, 0.0f, 1.0f);
                unpackedAlphaPixels[count] = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 1.0f);
                count++;
            }

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    int index = (y * width + x);
                    colorBlockList.Add(DXTBlock.FromUncompressed(unpackedColourPixels, index, width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(unpackedAlphaPixels, index, width));
                    }

                    index = (y * width + x + 4);
                    colorBlockList.Add(DXTBlock.FromUncompressed(unpackedColourPixels, index, width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(unpackedAlphaPixels, index, width));
                    }

                    index = ((y + 4) * width + x);
                    colorBlockList.Add(DXTBlock.FromUncompressed(unpackedColourPixels, index, width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(unpackedAlphaPixels, index, width));
                    }

                    index = ((y + 4) * width + x + 4);
                    colorBlockList.Add(DXTBlock.FromUncompressed(unpackedColourPixels, index, width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(unpackedAlphaPixels, index, width));
                    }
                }
            }
        }

        public static void ReadBitmap(DirectBitmap bitmap, List<DXTBlock> colorBlockList, List<DXTBlock> alphaBlockList)
        {
            byte[] bytes = bitmap.Bits;
            int width = bitmap.Width;
            int height = bitmap.Height;


            Color[] colorPixels = new Color[bytes.Length / 4];
            int count = 0;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                //colorPixels[count++] = Color.FromArgb(255, bytes[i + 1], bytes[i + 2], bytes[i + 3]);
                Color c = Color.FromArgb(255, bytes[i + 2], bytes[i + 1], bytes[i + 0]);
                colorPixels[count++] = c;
            }

            bool hasAlpha = true;
            Color[] alphaPixels = new Color[bytes.Length / 4];
            count = 0;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                // set alpha into green
                //alphaPixels[count++] = Color.FromArgb(255, 0, bytes[i + 0], 0);
                alphaPixels[count++] = Color.FromArgb(255, 0, bytes[i + 3], 0);
                if (bytes[i + 3] != 255)
                {
                    hasAlpha = true;
                }
            }

                        
            
            
            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    int index = (y * width + x);
                    colorBlockList.Add(DXTBlock.FromUncompressed(colorPixels, index, bitmap.Width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(alphaPixels, index, bitmap.Width));
                    }

                    index = (y * width + x + 4);
                    colorBlockList.Add(DXTBlock.FromUncompressed(colorPixels, index, bitmap.Width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(alphaPixels, index, bitmap.Width));
                    }

                    index = ((y + 4) * width + x);
                    colorBlockList.Add(DXTBlock.FromUncompressed(colorPixels, index, bitmap.Width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(alphaPixels, index, bitmap.Width));
                    }

                    index = ((y + 4) * width + x + 4);
                    colorBlockList.Add(DXTBlock.FromUncompressed(colorPixels, index, bitmap.Width));
                    if (hasAlpha)
                    {
                        alphaBlockList.Add(DXTBlock.FromUncompressed(alphaPixels, index, bitmap.Width));
                    }
                }
            }

            int ibreak = 0;
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ColorPacked(int r, int g, int b, int a)
        {
            return (uint)(r << 16 | g << 8 | b | a << 24);
        }

        public static byte[] ProcessBlockList(List<DXTBlock> colorBlocks, List<DXTBlock> alphaBlocks)
        {
            byte[] results = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                using (BinaryWriter binWriter = new BinaryWriter(memStream))
                {
                    foreach (DXTBlock block in colorBlocks)
                    {
                        DXTBlock resultBlock = BlockEncoder.Bc1BlockEncoderSlow.EncodeBlock(block);
                        resultBlock.Write(binWriter);
                    }

                    // go through and write alpha info
                    foreach (DXTBlock block in alphaBlocks)
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
            //String basePath = @"F:\UnityProjects\GladiusDFGui\Assets\Resources\Textures\";
            //string filename = "endlesshorizons_tournament.png";
            string basePath = @"d:\tmp\";
            string filename = @"test256.png";

            System.IO.DirectoryInfo targetInfo = new DirectoryInfo(reencodePath);
            if (!targetInfo.Exists)
            {
                targetInfo.Create();
            }

            DirectBitmap directBitmap = new DirectBitmap(basePath + filename);
            int width = directBitmap.Width;
            int height = directBitmap.Height;

            List<DXTBlock> colorBlockList = new List<DXTBlock>();
            List<DXTBlock> alphaBlockList = new List<DXTBlock>();
            ReadBitmap(directBitmap, colorBlockList, alphaBlockList);

            byte[] processResults = ProcessBlockList(colorBlockList, alphaBlockList);

            DirectBitmap reencodedBitmap = new DirectBitmap(width, height);

            //DecompressDXT1GC(processResults, width, height, reencodedBitmap.Bits);

            //reencodedBitmap.Bitmap.Save(reencodePath + filename, ImageFormat.Png);
            //reencodedBitmap.Dispose();
        }

        public static void TestExtract(string sourcePath, string outputDirectory)
        {
            new GCImageExtractor().ProcessImages(sourcePath, outputDirectory);
        }


        public static void DecodeFile(string fileName, string outputDirectory)
        {
            new GCImageExtractor().ProcessImages(fileName, outputDirectory);
        }


        public static void EncodeFile(List<Texture2D> textureList, string imageName, string destinationFile)
        {
            textureList = textureList.OrderBy(x => x.name).ToList();
            
            List<TextureHeaderInfo> headerList = new List<TextureHeaderInfo>();
            List<byte[]> processeedList = new List<byte[]>();
            foreach (Texture2D texture in textureList)
            {
                int width = texture.width;
                int height = texture.height;

                
                List<DXTBlock> colorBlockList = new List<DXTBlock>();
                List<DXTBlock> alphaBlockList = new List<DXTBlock>();
                ReadColours(texture.GetPixels(), width, height, colorBlockList, alphaBlockList);

                byte[] processedResults = ProcessBlockList(colorBlockList, alphaBlockList);

                TextureHeaderInfo headerInfo = new TextureHeaderInfo();
                
                headerInfo.Name = texture.name+".tga";
                
                headerInfo.Width = width;
                headerInfo.Height = height;
                headerInfo.CompressedSize = processedResults.Length;

                headerList.Add(headerInfo);
                processeedList.Add(processedResults);
            }

            WriteMultiImageFile(imageName, headerList, processeedList);
        }


        public static void EncodeFile(Texture2D t, string imageName, string destinationFile)
        {
            //DirectBitmap directBitmap = new DirectBitmap(t);
            //int width = directBitmap.Width;
            //int height = directBitmap.Height;

            int width = t.width;
            int height = t.height;


            List<DXTBlock> colorBlockList = new List<DXTBlock>();
            List<DXTBlock> alphaBlockList = new List<DXTBlock>();
            //ReadBitmap(directBitmap, colorBlockList, alphaBlockList);
            ReadColours(t.GetPixels(), width, height, colorBlockList, alphaBlockList);

            byte[] processResults = ProcessBlockList(colorBlockList, alphaBlockList);
            WriteImageFile(destinationFile, imageName, width, height, processResults);
        }

        public static void EncodeFile(string originalFile, string destinationFile)
        {
            string imageName = Path.GetFileName(destinationFile);
            DirectBitmap directBitmap = new DirectBitmap(originalFile);
            int width = directBitmap.Width;
            int height = directBitmap.Height;

            List<DXTBlock> colorBlockList = new List<DXTBlock>();
            List<DXTBlock> alphaBlockList = new List<DXTBlock>();
            ReadBitmap(directBitmap, colorBlockList, alphaBlockList);

            byte[] processResults = ProcessBlockList(colorBlockList, alphaBlockList);
            WriteImageFile(destinationFile, imageName, width, height, processResults);
        }


        /*
         *  GC Texture File made up of :
         *  PTTP
         *  NAME
         *  NMPT
         *  PFHD
         *  PTDT
         *  END
         *
         *  with possible PADD's
         *
         *
         *
         * 
        // wheel info - 9 textures
        NMTP     size               num textures
        50464844 40010000 01000000 09000000 
    
                           W    H    
        0020 0800 00000000 8000 8000 002B0000 50010000 00000000 00000000 00000000
        0020 0900 00000000 8000 0001 C0550000 302C0000 10000000 00000000 00000000
        0020 0900 00000000 8000 0001 C0550000 D0810000 20000000 00000000 00000000
        0020 0900 00000000 8000 0001 C0550000 70D70000 32000000 00000000 00000000
        0020 0900 00000000 8000 0001 C0550000 102D0100 44000000 00000000 00000000
        0020 0900 00000000 8000 0001 C0550000 B0820100 54000000 00000000 00000000
        0020 0900 00000000 8000 0001 C0550000 50D80100 65000000 00000000 00000000
        0022 0800 00000000 8000 8000 00560000 F02D0200 77000000 00000000 00000000
        0022 0800 00000000 8000 8000 00560000 D0830200 8F000000 00000000 00000000
        00000000000000000000000000000000
         */

        public static void WriteMultiImageFile(string outputName, List<TextureHeaderInfo> textureInfoList,
            List<byte[]> dataList)
        {
            using (FileStream fs = new FileStream(outputName, FileMode.Create))
            {
                using (BinaryWriter binWriter = new BinaryWriter(fs))
                {
                    List<string> textureNames = new List<string>();

                    int count = 0;
                    foreach (TextureHeaderInfo textureInfo in textureInfoList)
                    {
                        textureInfo.Name = textureInfo.Name.Replace(".png", ".tga");
                        textureInfo.Name = textureInfo.Name.Replace(".jpg", ".tga");
                        textureInfo.Name = textureInfo.Name.Replace(".gif", ".tga");

                        textureNames.Add(textureInfo.Name);
                    }

                    binWriter.Write(Common.pttpTag);
                    binWriter.Write(BaseChunk.ChunkHeaderSize);
                    binWriter.Write(3);
                    binWriter.Write(1);

                    GladiusFileWriter.WriteNAME(binWriter, textureNames);
                    GladiusFileWriter.WriteNMTP(binWriter, textureNames);

                    GladiusFileWriter.WritePFHD(binWriter, textureInfoList);

                    GladiusFileWriter.PadIfNeeded(binWriter);

                    GladiusFileWriter.WritePTDT(binWriter, textureInfoList, dataList);

                    GladiusFileWriter.WriteEND(binWriter);
                }
            }
        }


        public static void WriteImageFile(string outputName, string imageName, int width, int height, byte[] data)
        {
            imageName = imageName.Replace(".png", ".tga");
            imageName = imageName.Replace(".jpg", ".tga");
            imageName = imageName.Replace(".gif", ".tga");

            using (FileStream fs = new FileStream(outputName, FileMode.Create))
            {
                using (BinaryWriter binWriter = new BinaryWriter(fs))
                {
                    binWriter.Write(Common.pttpTag);
                    binWriter.Write(16);
                    binWriter.Write(3);
                    binWriter.Write(1);

                    binWriter.Write(Common.nmptTag);
                    binWriter.Write(48);
                    binWriter.Write(0);
                    binWriter.Write(0);
                    binWriter.Write(Encoding.ASCII.GetBytes(imageName));
                    for (int i = 0; i < 32 - imageName.Length; ++i)
                    {
                        binWriter.Write((byte)0);
                    }

                    binWriter.Write(Common.pfhdTag);
                    // section size
                    binWriter.Write(64);
                    // pad 1
                    binWriter.Write(1);
                    // num textures
                    binWriter.Write(1);
                    // compress type
                    binWriter.Write((ushort)0x2200);
                    // unknown
                    binWriter.Write((ushort)0);

                    binWriter.Write(0x80);

                    binWriter.Write((ushort)width);
                    binWriter.Write((ushort)height);
                    // compressed size
                    binWriter.Write(data.Length);


                    binWriter.Write(0x50);
                    binWriter.Write(0);

                    for (int i = 0; i < 24; ++i)
                    {
                        binWriter.Write((byte)0);
                    }


                    binWriter.Write(Common.paddTag);
                    binWriter.Write(16);
                    binWriter.Write(1);
                    binWriter.Write(1);

                    binWriter.Write(Common.ptdtTag);
                    binWriter.Write(16 + data.Length);
                    binWriter.Write(1);
                    binWriter.Write(1);
                    binWriter.Write(data);

                    binWriter.Write(Common.endTag);
                    binWriter.Write(16);
                    binWriter.Write(1);
                    binWriter.Write(1);
                }
            }
        }


        static void Main(string[] args)
        {
            string baseInput = @"f:\tmp\image\input\";
            string baseOutput = @"f:\tmp\image\output\";

            string sourcePath = baseInput; //baseInput+@"gui\leagues\";

            string outputDirectory = baseOutput;
            string reencodedOutputDirectory = baseOutput + @"textures-gc-reencoded\";

            //TestExtract(sourcePath, outputDirectory);
            //TestReencode(reencodedOutputDirectory);


            if (args.Length != 2)
            {
                System.Console.WriteLine("texturetools <original file>  <destination file>");
            }
            else
            {
                string originalFilename = args[0];
                string destinatioFilename = args[1];

                EncodeFile(originalFilename, destinatioFilename);
            }

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

        public DirectBitmap(Texture2D t)
        {
            Width = t.width;
            Height = t.height;
            Bits = new byte[Width * Height * 4];
            //Bits = t.GetPixels();//t.GetRawTextureData();
            m_bitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Stride, PixelFormat.Format32bppArgb, m_bitsHandle.AddrOfPinnedObject());
            UnityEngine.Color[] colourData = t.GetPixels();
            SetPixels(colourData);
        }


        public DirectBitmap(string filename)
        {
            Bitmap = new Bitmap(filename);
            Width = Bitmap.Width;
            Height = Bitmap.Height;

            BitmapData bitmapData = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            Bits = new byte[Math.Abs(bitmapData.Stride * bitmapData.Height)];
            Marshal.Copy(bitmapData.Scan0, Bits, 0, Bits.Length);


            int ibreak = 0;
        }


        ~DirectBitmap()
        {
            Dispose(false);
        }

        public void SetPixels(UnityEngine.Color[] colourData)
        {
            int colourCounter = 0;
            unchecked
            {
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        int index = x + (y * Width);
                        // Bits[index + 0] = (byte)(colourData[colourCounter][0] * 255);
                        // Bits[index + 1] = (byte)(colourData[colourCounter][1] * 255);
                        // Bits[index + 2] = (byte)(colourData[colourCounter][2] * 255);
                        // Bits[index + 3] = (byte)(colourData[colourCounter][3] * 255);
                        Bits[index + 0] = (byte)(colourData[colourCounter][3] * 255);
                        Bits[index + 1] = (byte)(colourData[colourCounter][0] * 255);
                        Bits[index + 2] = (byte)(colourData[colourCounter][1] * 255);
                        Bits[index + 3] = (byte)(colourData[colourCounter][2] * 255);

                        colourCounter++;
                    }
                }
            }
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

        public void SetPixel(int x, int y, UnityEngine.Color color)
        {
            int index = x + (y * Width);
            unchecked
            {
                // Bits[index + 0] = (byte)(color[0] * 255);
                // Bits[index + 1] = (byte)(color[1] * 255);
                // Bits[index + 2] = (byte)(color[2] * 255);
                // Bits[index + 3] = (byte)(color[3] * 255);
                Bits[index + 0] = (byte)(color[3] * 255);
                Bits[index + 1] = (byte)(color[0] * 255);
                Bits[index + 2] = (byte)(color[1] * 255);
                Bits[index + 3] = (byte)(color[2] * 255);
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

        //public uint[] LineIndices = new uint[16];
        //public uint[] Lines = new uint[16];

        public Color[] DecodedColours = new Color[4];
        public Color[] SourceColours = new Color[16];


        public ColorRgb565 CalculatedColor0 = new ColorRgb565();
        public ColorRgb565 CalculatedColor1 = new ColorRgb565();

        public uint colorIndices = 0;

        public int this[int index]
        {
            get => (int)(colorIndices >> (index * 2)) & 0b11;
            set
            {
                colorIndices = (uint)(colorIndices & ~(0b11 << (index * 2)));
                var val = value & 0b11;
                colorIndices = colorIndices | ((uint)val << (index * 2));
            }
        }


        public bool HasAlphaOrBlack
        {
            get
            {
                //return false;
                return CalculatedColor0.data <= CalculatedColor1.data;
            }
        }


        public static DXTBlock FromUncompressed(UnityEngine.Color[] data, int offset, int stride)
        {
            DXTBlock block = new DXTBlock();
            int count = 0;
            for (int y = 0; y < 4; ++y)
            {
                for (int x = 0; x < 4; ++x)
                {
                    Color convertedColour = Color.FromArgb((int)(data[offset + x].a * 255),
                        (int)(data[offset + x].r * 255),
                        (int)(data[offset + x].g * 255), (int)(data[offset + x].b * 255));

                    block.SourceColours[count++] = convertedColour;
                }

                offset += stride;
            }


            return block;
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
            ushort swappedQ0 = GCImageExtractor.Swap16(CalculatedColor0.data);
            ushort swappedQ1 = GCImageExtractor.Swap16(CalculatedColor1.data);

            binWriter.Write(swappedQ0);
            binWriter.Write(swappedQ1);

            uint convertedColorIndices = 0;
            uint d = colorIndices;

            if (colorIndices != 0)
            {
                int ibreak = 0;
            }

            int count = 30;
            for (int y = 3; y >= 0; y--)
            {
                //for (int x = 3; x >= 0; x--)
                for (int x = 0; x < 4; x++)
                {
                    int val = this[(y * 4) + x];
                    convertedColorIndices |= (uint)(val << count);
                    count -= 2;
                }
            }

            //binWriter.Write(colorIndices);
            binWriter.Write(convertedColorIndices);
        }


        public static DXTBlock FromCompressed(byte[] input, int offset)
        {
            if (offset > input.Length - 3)
            {
                int ibreak = 0;
            }
            
            DXTBlock block = new DXTBlock();
            int r0, g0, b0, r1, g1, b1;

            ushort q0 = (ushort)(input[offset + 0] | input[offset + 1] << 8);
            ushort q1 = (ushort)(input[offset + 2] | input[offset + 3] << 8);

            q0 = GCImageExtractor.Swap16(q0);
            q1 = GCImageExtractor.Swap16(q1);


            GCImageExtractor.Rgb565(q0, out r0, out g0, out b0);
            GCImageExtractor.Rgb565(q1, out r1, out g1, out b1);

            GCImageExtractor.TempColors[0] = Color.FromArgb(255, r0, g0, b0);
            GCImageExtractor.TempColors[1] = Color.FromArgb(255, r1, g1, b1);
            if (q0 > q1)
            {
                GCImageExtractor.TempColors[2] =
                    Color.FromArgb(255, (r0 * 2 + r1) / 3, (g0 * 2 + g1) / 3, (b0 * 2 + b1) / 3);
                GCImageExtractor.TempColors[3] =
                    Color.FromArgb(255, (r0 + r1 * 2) / 3, (g0 + g1 * 2) / 3, (b0 + b1 * 2) / 3);
            }
            else
            {
                GCImageExtractor.TempColors[2] = Color.FromArgb(255, (r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2);
                GCImageExtractor.TempColors[3] = Color.FromArgb(0, 0, 0, 0);
            }

            for (int i = 0; i < 4; ++i)
            {
                block.DecodedColours[i] = GCImageExtractor.TempColors[i];
            }


            uint d = BitConverter.ToUInt32(input, offset + 4);
            block.colorIndices = 0;

            for (int y = 0; y < 4; ++y)
            {
                for (int x = 3; x >= 0; x--)
                {
                    block[(y * 4) + x] = (int)(d & 3);
                    d >>= 2;
                }
            }

            GCImageExtractor.ClearTempColours();

            return block;
        }
    };
}