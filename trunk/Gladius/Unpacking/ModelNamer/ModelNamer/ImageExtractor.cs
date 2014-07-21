// a greatly modified parser , originaly based on the targa reader by David Polomis (paloma_sw@cox.net)
// now modified to extract image files from the ps2 version of LucasArts gladius.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ModelNamer
{


    public class GladiusImage : IDisposable
    {
        private GladiusHeader gladiusHeader = null;
        private Bitmap bitmapImage = null;
        private string strFileName = string.Empty;
        private int intStride = 0;
        private GCHandle ImageByteHandle;
        private System.Collections.Generic.List<System.Collections.Generic.List<byte>> rows = new System.Collections.Generic.List<System.Collections.Generic.List<byte>>();
        private System.Collections.Generic.List<byte> row = new System.Collections.Generic.List<byte>();

            String targetDirectory = @"D:/gladius-extracted/test-extract/";
            //String filepath = @"D:\gladius-extracted\ps2-decompressed\converted1\";
            String filepath = @"D:\gladius-extracted\ps2-decompressed\PTTP\";
            String errorFile = @"D:\gladius-extracted\ps2-decompressed-errors";
            String infoFile = @"D:\gladius-extracted\ps2-texturelist";



        /// <summary>
        /// Creates a new instance of the TargaImage object.
        /// </summary>
        public GladiusImage()
        {
            this.gladiusHeader = new GladiusHeader();
            this.bitmapImage = null;
        }


        /// <summary>
        /// Gets a TargaHeader object that holds the Targa Header information of the loaded file.
        /// </summary>
        public GladiusHeader Header
        {
            get { return this.gladiusHeader; }
            set { this.gladiusHeader = value; }
        }

        /// <summary>
        /// Gets a Bitmap representation of the loaded file.
        /// </summary>
        public Bitmap Image
        {
            get { return this.bitmapImage; }
        }

        /// <summary>
        /// Gets the full path and filename of the loaded file.
        /// </summary>
        public string FileName
        {
            get { return this.strFileName; }
        }


        /// <summary>
        /// Gets the byte offset between the beginning of one scan line and the next. Used when loading the image into the Image Bitmap.
        /// </summary>
        /// <remarks>
        /// The memory allocated for Microsoft Bitmaps must be aligned on a 32bit boundary.
        /// The stride refers to the number of bytes allocated for one scanline of the bitmap.
        /// </remarks>
        public int Stride
        {
            get { return this.intStride; }
        }

        ~GladiusImage()
        {
            Dispose(false);
        }

        public void LoadColourMapInfo(BinaryReader binReader)
        {
            int paletteSize = 1024;

            this.gladiusHeader.RawColourMap = new byte[paletteSize];
            this.gladiusHeader.SwizzledColourMap = new byte[paletteSize];

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

                this.gladiusHeader.RawColourMap[rawCounter++] = rbyte;
                this.gladiusHeader.RawColourMap[rawCounter++] = gbyte;
                this.gladiusHeader.RawColourMap[rawCounter++] = bbyte;
                this.gladiusHeader.RawColourMap[rawCounter++] = abyte;
            }

            byte[] src = this.gladiusHeader.RawColourMap;
            byte[] tgt = this.gladiusHeader.SwizzledColourMap;

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

            // skip 12?
            int a1 = binReader.ReadInt32();
            int a2 = binReader.ReadInt32();
            
            this.gladiusHeader.Width = binReader.ReadInt16();
            this.gladiusHeader.Height = binReader.ReadInt16();
        }

        // assumes we've position the stream.
        private byte[] LoadImageBytes(BinaryReader binReader)
        {
            byte[] data = null;
            int intImageRowByteSize = (int)this.gladiusHeader.Width * ((int)this.gladiusHeader.BytesPerPixel);
            
            // get the size in bytes of the whole image
            int intImageByteSize = intImageRowByteSize * (int)this.gladiusHeader.Height;
            data = new byte[intImageByteSize];

            try
            {
                if(Header.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    for (int i = 0; i < data.Length; ++i)
                    {

                        data[i] = binReader.ReadByte();
                    }
                }
                else if (Header.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    byte[] subBytes = new byte[4];
                    int adjustedLength = data.Length / 4;
                    int counter = 0;
                    for (int i = 0; i < adjustedLength; ++i)
                    {
                        subBytes[0] = binReader.ReadByte ();
                        subBytes[1] = binReader.ReadByte ();
                        subBytes[2] = binReader.ReadByte ();
                        subBytes[3] = binReader.ReadByte ();

                        //bgr
                        //Array.Reverse(subBytes);
                        subBytes[3] = 0xff;
                        data[counter++] = subBytes[2];
                        data[counter++] = subBytes[1];
                        data[counter++] = subBytes[0];
                        data[counter++] = subBytes[3];
                    }

                }




            }
            catch (Exception e)
            {
                // if we read past the end, somethings wrong, but try and return as much data as we could get..
            }
            // return the image byte array
            return data;

        }

        public void LoadGladiusImage(BinaryReader binReader)
        {
            this.intStride = (((int)this.gladiusHeader.Width * (int)this.gladiusHeader.PixelDepth + 31) & ~31) >> 3; // width in bytes

            byte[] bimagedata = this.LoadImageBytes(binReader);

            this.ImageByteHandle = GCHandle.Alloc(bimagedata, GCHandleType.Pinned);

            PixelFormat pf = this.Header.PixelFormat;


            this.bitmapImage = new Bitmap((int)this.gladiusHeader.Width,
                                            (int)this.gladiusHeader.Height,
                                            this.intStride,
                                            pf,
                                            this.ImageByteHandle.AddrOfPinnedObject());

            if (pf == PixelFormat.Format8bppIndexed)
            {
                int numColourEntries = gladiusHeader.RawColourMap.Length / 4;
                if (numColourEntries > 0)
                {
                    ColorPalette pal = this.bitmapImage.Palette;

                    byte[] map = gladiusHeader.RawColourMap;
                    map = gladiusHeader.SwizzledColourMap;
                    for (int i = 0; i < numColourEntries; i++)
                    {
                        Color c = Color.FromArgb(255, map[i * 4], map[(i * 4) + 1], map[(i * 4) + 2]);
                        Color c2 = Color.FromArgb(c.A, c.B, c.G, c.R);
                        pal.Entries[i] = c2;
                    }
                    this.bitmapImage.Palette = pal;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            //GC.SuppressFinalize(this);

        }


        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            // If disposing equals true, dispose all managed 
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                if (this.bitmapImage != null)
                {
                    this.bitmapImage.Dispose();
                }

                if (this.ImageByteHandle != null)
                {
                    if (this.ImageByteHandle.IsAllocated)
                    {
                        this.ImageByteHandle.Free();
                    }

                }
            }
        }


        #endregion
    }



    public class GladiusHeader
    {
        private short sColorMapFirstEntryIndex = 0;
        private short sColorMapLength = 0;
        private byte bColorMapEntrySize = 0;
        public int Width = 0;
        public int Height = 0;
        private byte bPixelDepth = 0;
        private byte bImageDescriptor = 0;
        public byte[] RawColourMap;
        public byte[] SwizzledColourMap;
        public PixelFormat PixelFormat;

        public static char[] PTTPHeader = new char[] { 'P', 'T', 'T', 'P' };
        public static char[] NAMEHeader = new char[] { 'N', 'A', 'M', 'E' };

        public static char[] NMPTHeader = new char[] { 'N', 'M', 'P', 'T' };
        public static char[] PFHDHeader = new char[] { 'P', 'F', 'H', 'D' };

        public static char[] r2d2Header = new char[] { 'R', '2', 'D', '2','p','s','x','2' };
        public static char[] tmapHeader = new char[] { 't', 'm', 'a', 'p' };

        static char[][] allTags = { PTTPHeader, NAMEHeader, NMPTHeader, PFHDHeader, r2d2Header };


        //readpfhd!


        /// <summary>
        /// Gets the index of the first color map entry. ColorMapFirstEntryIndex refers to the starting entry in loading the color map.
        /// </summary>
        public short ColorMapFirstEntryIndex
        {
            get { return this.sColorMapFirstEntryIndex; }
        }

        /// <summary>
        /// Sets the ColorMapFirstEntryIndex property, available only to objects in the same assembly as TargaHeader.
        /// </summary>
        /// <param name="sColorMapFirstEntryIndex">The First Entry Index value read from the file.</param>
        internal protected void SetColorMapFirstEntryIndex(short sColorMapFirstEntryIndex)
        {
            this.sColorMapFirstEntryIndex = sColorMapFirstEntryIndex;
        }

        /// <summary>
        /// Gets total number of color map entries included.
        /// </summary>
        public short ColorMapLength
        {
            get { return this.sColorMapLength; }
        }

        /// <summary>
        /// Sets the ColorMapLength property, available only to objects in the same assembly as TargaHeader.
        /// </summary>
        /// <param name="sColorMapLength">The Color Map Length value read from the file.</param>
        internal protected void SetColorMapLength(short sColorMapLength)
        {
            this.sColorMapLength = sColorMapLength;
        }

        /// <summary>
        /// Gets the number of bits per entry in the Color Map. Typically 15, 16, 24 or 32-bit values are used.
        /// </summary>
        public byte ColorMapEntrySize
        {
            get { return this.bColorMapEntrySize; }
        }

        /// <summary>
        /// Sets the ColorMapEntrySize property, available only to objects in the same assembly as TargaHeader.
        /// </summary>
        /// <param name="bColorMapEntrySize">The Color Map Entry Size value read from the file.</param>
        internal protected void SetColorMapEntrySize(byte bColorMapEntrySize)
        {
            this.bColorMapEntrySize = bColorMapEntrySize;
        }


        /// <summary>
        /// Gets the number of bits per pixel. This number includes
        /// the Attribute or Alpha channel bits. Common values are 8, 16, 24 and 32.
        /// </summary>
        public byte PixelDepth
        {
            get { return this.bPixelDepth; }
        }

        /// <summary>
        /// Sets the PixelDepth property, available only to objects in the same assembly as TargaHeader.
        /// </summary>
        /// <param name="bPixelDepth">The Pixel Depth value read from the file.</param>
        internal protected void SetPixelDepth(byte bPixelDepth)
        {
            this.bPixelDepth = bPixelDepth;
        }

        /// <summary>
        /// Gets the number of bytes per pixel.
        /// </summary>
        public int BytesPerPixel
        {
            get
            {
                return (int)this.bPixelDepth / 8;
            }
        }

        public static bool ReadToNextTMapBlock(BinaryReader binReader,ref int imageCount)
        {
            bool foundR2D2 = Common.FindCharsInStream(binReader, GladiusHeader.r2d2Header);
            bool foundTmap = false;
            byte[] extra = new byte[8];
            
            while(foundR2D2)
            {
                imageCount++;

                binReader.Read(extra,0,extra.Length);
                bool shouldHaveTmap = (extra[5] != 0x00);
                if(shouldHaveTmap)
                {
                    foundTmap = Common.FindCharsInStream(binReader, GladiusHeader.tmapHeader);
                    break;
                }
                    foundR2D2 = Common.FindCharsInStream(binReader, GladiusHeader.r2d2Header);
            }

            return foundTmap;
        }


        public static List<String> BuildImageList(BinaryReader binReader)
        {
            List<String> textureNameList = new List<string>();

            // load header and map of image names.
            if (Common.FindCharsInStream(binReader, GladiusHeader.PTTPHeader))
            {
                if (Common.FindCharsInStream(binReader, GladiusHeader.NMPTHeader))
                {
                    int unknown1 = binReader.ReadInt32();
                    int unknown2 = binReader.ReadInt32();
                    int numTextures = binReader.ReadInt32();
                    //byte pad1 = binReader.ReadByte();
                    //byte pad2 = binReader.ReadByte();
                    //byte pad3 = binReader.ReadByte();

                    for (int i = 0; i < numTextures; ++i)
                    {
                        StringBuilder sb = new StringBuilder();
                        char b = binReader.ReadChar();
                        while (b != 0x00)
                        {
                            if (b != 0x00)
                            {
                                sb.Append(b);
                            }
                            b = binReader.ReadChar();
                        }
                        textureNameList.Add(sb.ToString());
                    }
                }

            }
            return textureNameList;
        }



        public bool KnownFormat()
        {
            return PixelFormat == PixelFormat.Format8bppIndexed || PixelFormat == PixelFormat.Format32bppArgb;
        }

        public bool ValidSize()
        {
            return !(Width <= 0 || Height <= 0 || Width > 1024 || Height > 1024);
        }


        public void LoadHeaderInfo(BinaryReader binReader, StreamWriter errorStream,String file)
        {
            int headerPadding = 0;

            byte[] extraHeader = new byte[24];
            binReader.Read(extraHeader, 0, extraHeader.Length);

            //19th extra byte should determine image format (8bppIndexed,32rgba)
            //20th extra byte should be FF
            int imagewidth = extraHeader[2];
            int imageHeight = extraHeader[3];

            int imageFormat = extraHeader[22];

            if (imageFormat == 0xB0)
            {
                if (imagewidth == 0x10)
                {
                    Width = 256;
                }
                else if (imagewidth == 0x08)
                {
                    Width = 128;
                }
                else if (imagewidth == 0x04)
                {
                    Width = 64;
                }
                else if (imagewidth == 0x02)
                {
                    Width = 32;
                }
                else if (imagewidth == 0x01)
                {
                    Width = 16;
                }
                if (imageHeight == 0x40)
                {
                    Height = 256;
                }
                else if(imageHeight == 0x20)
                {
                    Height = 128;
                }
                else if (imageHeight == 0x10)
                {
                    Height = 64;
                }
                else if (imageHeight == 0x08)
                {
                    Height = 32;
                }
                else if (imageHeight == 0x04)
                {
                    Height = 16;
                }



                else
                {
                    //errorStream.WriteLine(String.Format("Unknown size [{0}] File [{1}]", imageSize, file));
                }

                PixelFormat= PixelFormat.Format32bppArgb;
                headerPadding = 101;
            }
            else if (imageFormat == 0xD8 || imageFormat == 0x60)
            {
                PixelFormat = PixelFormat.Format8bppIndexed;
                headerPadding = 0x78 - 20;
            }
            else
            {
                errorStream.WriteLine(String.Format("Unknown Output Format [{0}] File [{1}]", imageFormat, file));
            }

            {
                binReader.BaseStream.Seek(headerPadding, SeekOrigin.Current);
                //return binReader.BaseStream.Position;
                
            }
        }

        public static bool PositionReaderAtNextImage(BinaryReader binReader,StreamWriter errorStream,String file)
        {
            return Common.FindCharsInStream(binReader, GladiusHeader.r2d2Header) && Common.FindCharsInStream(binReader, GladiusHeader.tmapHeader);
        }


        }



    public class ImageExtractor
    {

        String targetDirectory = @"D:/gladius-extracted/test-extract/";
        //String filepath = @"D:\gladius-extracted\ps2-decompressed\converted1\";
        String filepath = @"D:\gladius-extracted\ps2-decompressed\PTTP\";
        String errorFile = @"D:\gladius-extracted\ps2-decompressed-errors";

        public static char[] PTTPHeader = new char[] { 'P', 'T', 'T', 'P' };
        public static char[] NAMEHeader = new char[] { 'N', 'A', 'M', 'E' };

        public static char[] NMPTHeader = new char[] { 'N', 'M', 'P', 'T' };
        public static char[] PFHDHeader = new char[] { 'P', 'F', 'H', 'D' };

        public static char[] r2d2Header = new char[] { 'R', '2', 'D', '2','p','s','x','2' };
        public static char[] tmapHeader = new char[] { 't', 'm', 'a', 'p' };

        static char[][] allTags = { PTTPHeader, NAMEHeader, NMPTHeader, PFHDHeader, r2d2Header };


        //String filepath = @"C:\gladius-extracted\ps2-decompressed\PTTP\";
        //String errorFile = @"C:\gladius-extracted\ps2-decompressed-errors";
        public void ExtractImages()
        {
            GladiusImage targaImage = new GladiusImage();

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

            string[] filePaths = Directory.GetFiles(filepath);




            DirectoryInfo d = new DirectoryInfo(filepath);
            FileInfo[] files = d.GetFiles(); //Getting Text files
            using (StreamWriter errorStream = new StreamWriter(new FileStream(errorFile, FileMode.OpenOrCreate)))
            {
                GladiusImage image = null;
                foreach (FileInfo file in files)
                {
                    //if (file.Name != "File_000957")
                    if (file.Name != "File_024114")
                    {
                        //continue;
                    }
                    using (FileStream fs = new FileStream(filepath + file.Name, FileMode.Open))
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        int headerPadding = 0;
                        //PixelFormat pf = PixelFormat.Format8bppIndexed;
                        //int width = 0;
                        //int height = 0;

                        int subImageCounter = 0;
                        ImageInfo imageInfo = new ImageInfo();
                        imageInfo.m_name = file.Name;
                        List<String> textureNameList = GladiusHeader.BuildImageList(binReader);
                        DumpHeaderSection(binReader, imageInfo);


                        //bool foundtmap = false;
                        //bool shouldHaveTMap = false;


                        //if(ReadR2D2psxBlock(binReader,ref shouldHaveTMap))
                        //{
                        //    subImageCounter++;
                        //    if(shouldHaveTMap)
                        //    {
                        //        foundtmap = FindCharsInStream(binReader, GladiusHeader.tmapHeader);
                        //    }
                        //}


                        while (GladiusHeader.ReadToNextTMapBlock(binReader, ref subImageCounter))
                        {
                            image = new GladiusImage();

                            image.Header.LoadHeaderInfo(binReader, errorStream, file.Name);

                            bool saveImage = true;
                            String outputFileName = null;
                            if (imageInfo.Next() && textureNameList.Count > 0)
                            {
                                outputFileName = textureNameList[imageInfo.imageCounter];
                            }
                            else
                            {
                                outputFileName = file.Name+"-"+(subImageCounter);
                            }


                            //if (textureNameList.Count > 0 && subImageCounter < textureNameList.Count)
                            //{
                            //    //outputFileName = file.Name + "-" + textureNameList[subImageCounter];
                            //    outputFileName = textureNameList[subImageCounter];
                            //}
                            //else
                            //{
                            //    outputFileName = file.Name + "-" + (subImageCounter);
                            //}


                            errorStream.WriteLine(String.Format("Extracting [{0}][{1}][{2}]", file.Name, subImageCounter, outputFileName));
                            int imagePadding = 0x13;// 0x24;


                            //if (pf == PixelFormat.Format8bppIndexed)
                            if (image.Header.PixelFormat == PixelFormat.Format8bppIndexed)
                            {
                                //image.Header.Se
                                image.Header.SetColorMapLength(256);
                                image.Header.SetColorMapEntrySize(32);

                                //int mapPadding = 0x78;
                                //long offset = position + mapPadding;


                                //binReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                                if (binReader.BaseStream.Position + 1024 > binReader.BaseStream.Length)
                                {
                                    // not enough room for a colour map?
                                    break;
                                }
                                image.Header.SetPixelDepth(8);
                                image.LoadColourMapInfo(binReader);
                                //saveImage = false;

                            }
                            else if (image.Header.PixelFormat == PixelFormat.Format32bppArgb)
                            {
                                image.Header.SetPixelDepth(32);
                            }
                            else
                            {
                                int ibreak = 0;
                            }

                            if (image.Header.ValidSize() && image.Header.KnownFormat())
                            {


                                binReader.BaseStream.Seek(imagePadding, SeekOrigin.Current);
                                try
                                {
                                    image.LoadGladiusImage(binReader);
                                    if (saveImage)
                                    {
                                        image.Image.Save(targetDirectory + outputFileName + ".png", ImageFormat.Png);
                                    }
                                }
                                catch (AccessViolationException e)
                                {
                                    // bleugh.
                                }

                                bool writePalette = false;
                                if (writePalette)
                                {
                                    using (var fs2 = new FileStream(targetDirectory + file.Name + "-pal.bin", FileMode.CreateNew))
                                    {
                                        using (var bw = new BinaryWriter(fs2))
                                        {
                                            bw.Write(image.Header.RawColourMap);
                                        }
                                    }

                                    using (var fs2 = new FileStream(targetDirectory + file.Name + "-cpal.bin", FileMode.CreateNew))
                                    {
                                        using (var bw = new BinaryWriter(fs2))
                                        {
                                            bw.Write(image.Header.SwizzledColourMap);
                                        }
                                    }
                                }
                            }

                            //foundR2D2 = FindCharsInStream(binReader, GladiusHeader.r2d2Header);
                            //foundtmap = FindCharsInStream(binReader, GladiusHeader.tmapHeader);


                            //imageExists = PositionReaderAtNextImage(binReader, errorStream, file.Name);
                            //if (imageExists)
                            //{
                            //    image = new GladiusImage();
                            //}

                        }
                    }
                }
            }

        }

        public void ListImageData()
        {
            //String filepath = @"C:\gladius-extracted\ps2-decompressed\PTTP\";
            //String errorFile = @"C:\gladius-extracted\ps2-decompressed-errors";

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

            string[] filePaths = Directory.GetFiles(filepath);


            String infoFile2 = "D:/gladius-extracted/test-extract/image-texture-header.txt";


            DirectoryInfo d = new DirectoryInfo(filepath);
            FileInfo[] files = d.GetFiles(); //Getting Text files
            using (StreamWriter errorStream = new StreamWriter(new FileStream(infoFile2, FileMode.OpenOrCreate)))
            {
                GladiusImage image = null;
                foreach (FileInfo file in files)
                {
                    using (FileStream fs = new FileStream(filepath + file.Name, FileMode.Open))
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        int headerPadding = 0;
                        //PixelFormat pf = PixelFormat.Format8bppIndexed;
                        //int width = 0;
                        //int height = 0;

                        int subImageCounter = 0;
                        ImageInfo imageInfo = new ImageInfo();

                        List<String> textureNameList = GladiusHeader.BuildImageList(binReader);

                        errorStream.WriteLine(file.Name);
                        if (textureNameList.Count == 1 && textureNameList[0] == "skygold_R.tga")
                        {
                            errorStream.WriteLine("SingleSkyGold!");
                        }


                        foreach (String textureName in textureNameList)
                        {
                            errorStream.WriteLine(textureName);
                        }

                        DumpHeaderSection(binReader, imageInfo);
                        foreach (HeaderSegment hs in imageInfo.m_segments)
                        {
                            errorStream.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}]", hs.unk1, hs.unk2, hs.width, hs.height, hs.unk3, hs.containsDefinition, hs.unk5));
                        }


                        //while (GladiusHeader.ReadToNextTMapBlock(binReader, ref subImageCounter))
                        //{
                        //    int val1 = binReader.ReadInt32();
                        //    int val1 = binReader.ReadInt32();
                        //    int val1 = binReader.ReadInt32();
                        //    int val1 = binReader.ReadInt32();
                        //    int val1 = binReader.ReadInt32();
                        //    int val1 = binReader.ReadInt32();

                        //    byte[] extraHeader = new byte[24];
                        //    binReader.Read(extraHeader, 0, extraHeader.Length);

                        //    //19th extra byte should determine image format (8bppIndexed,32rgba)
                        //    //20th extra byte should be FF
                        //    int imageSize = extraHeader[2];
                        //    int imageFormat = extraHeader[22];

                        //    if (imageFormat == 0xB0)
                        //    {
                        //        if (imageSize == 0x10)
                        //        {
                        //            Width = Height = 256;
                        //        }
                        //        else if (imageSize == 0x08)
                        //        {
                        //            Width = Height = 0x20;
                        //        }
                        //        else if (imageSize == 0x04)
                        //        {
                        //            Width = Height = 64;
                        //        }
                        //        else
                        //        {
                        //            errorStream.WriteLine(String.Format("Unknown size [{0}] File [{1}]", imageSize, file));
                        //        }

                        //        PixelFormat = PixelFormat.Format32bppArgb;
                        //        headerPadding = 101;
                        //    }
                        //    else if (imageFormat == 0xD8 || imageFormat == 0x60)
                        //    {
                        //        PixelFormat = PixelFormat.Format8bppIndexed;
                        //        headerPadding = 0x78 - 20;
                        //    }
                        //    else
                        //    {
                        //        errorStream.WriteLine(String.Format("Unknown Output Format [{0}] File [{1}]", imageFormat, file));
                        //    }



                        //}

                        errorStream.WriteLine();
                        errorStream.WriteLine();
                    }
                }
            }

        }

        public void DumpSectionLengths()
        {
            String infoFile2 = "D:/gladius-extracted/test-extract/image-section-lengths.txt";

            string[] filePaths = Directory.GetFiles(filepath);
            List<ImageInfo> imageInfoList = new List<ImageInfo>();
            DirectoryInfo d = new DirectoryInfo(filepath);
            FileInfo[] files = d.GetFiles(); //Getting Text files
            using (StreamWriter infoStream = new StreamWriter(new FileStream(infoFile2, FileMode.OpenOrCreate)))
            {
                GladiusImage image = null;
                foreach (FileInfo file in files)
                {

                    using (FileStream fs = new FileStream(filepath + file.Name, FileMode.Open))
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        ImageInfo imageInfo = new ImageInfo();
                        imageInfoList.Add(imageInfo);
                        infoStream.WriteLine("File : " + imageInfo.m_name);
                        foreach (char[] tag in allTags)
                        {
                            if (Common.FindCharsInStream(binReader, tag, true))
                            {
                                int blockSize = binReader.ReadInt32();
                                imageInfo.m_tagSizes[tag] = blockSize;
                                infoStream.WriteLine(String.Format("\t {0} : {1}", new String(tag), blockSize));
                            }
                            else
                            {
                                imageInfo.m_tagSizes[tag] = -1;
                            }
                        }
                    }
                }
            }
        }


        public void DumpHeaderInfo()
        {
            String infoFile2 = "D:/gladius-extracted/test-extract/image-header-info.txt";

            string[] filePaths = Directory.GetFiles(filepath);
            List<ImageInfo> imageInfoList = new List<ImageInfo>();
            DirectoryInfo d = new DirectoryInfo(filepath);
            FileInfo[] files = d.GetFiles(); //Getting Text files
            using (StreamWriter infoStream = new StreamWriter(new FileStream(infoFile2, FileMode.OpenOrCreate)))
            {
                GladiusImage image = null;
                foreach (FileInfo file in files)
                {

                    using (FileStream fs = new FileStream(filepath + file.Name, FileMode.Open))
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        ImageInfo imageInfo = new ImageInfo();
                        imageInfo.m_name = file.Name;
                        imageInfoList.Add(imageInfo);
                        DumpHeaderSection(binReader, imageInfo);
                        infoStream.WriteLine("File : " + imageInfo.m_name);
                        foreach (HeaderSegment hs in imageInfo.m_segments)
                        {
                            infoStream.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}]", hs.unk1, hs.unk2, hs.width, hs.height, hs.unk3, hs.containsDefinition, hs.unk5));
                        }

                    }
                }
            }
        }




        public void DumpHeaderSection(BinaryReader binReader, ImageInfo imageInfo)
        {
            if (Common.FindCharsInStream(binReader, PFHDHeader, true))
            {
                int sectionSide = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int numTextures = binReader.ReadInt32();

                for (int u = 0; u < numTextures; ++u)
                {
                    HeaderSegment hs = new HeaderSegment();
                    imageInfo.m_segments.Add(hs);
                    hs.unk1 = binReader.ReadInt32();
                    hs.unk2 = binReader.ReadInt32();
                    hs.width = binReader.ReadInt16();
                    hs.height = binReader.ReadInt16();
                    hs.unk3 = binReader.ReadInt32();
                    hs.containsDefinition = binReader.ReadInt32();
                    hs.unk5 = binReader.ReadInt32();
                    hs.unk6 = binReader.ReadInt32();
                    hs.unk7 = binReader.ReadInt32();
                }

            }
             
            
        }


        static int Main(string[] args)
        {
            //TargaHeader.LoadBECFile();
            //new ImageExtractor().ListImageData();
            //new ImageExtractor().DumpSectionLengths();
            //new ImageExtractor().DumpHeaderInfo();
            new ImageExtractor().ExtractImages();
            return 0;
        }
    }

    public class HeaderSegment
    {
        public int unk1;
        public int unk2;
        public int width;
        public int height;
        public int unk3;
        public int containsDefinition;
        public int unk5;
        public int unk6;
        public int unk7;
    }

    public class ImageInfo
    {
        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();
        public List<HeaderSegment> m_segments = new List<HeaderSegment>();
        public String m_name;

        public int imageCounter = -1;
        public bool Next()
        {
            while(imageCounter <m_segments.Count)
            {
                imageCounter++;
                if (m_segments[imageCounter].containsDefinition != -1)
                {
                    break;
                }
            }
            return imageCounter < m_segments.Count;
        }

    }

}
