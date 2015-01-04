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
using System.Diagnostics;

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
        public string ImageName;

        public static Dictionary<string, Point> s_sizeMap = new Dictionary<string, Point>();
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


        public int Stride
        {
            get { return this.intStride; }
        }

        ~GladiusImage()
        {
            Dispose(false);
        }

        static HashSet<byte> alphaValues = new HashSet<byte>();

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

            bool allAlpha = true;
            for (int i = 0; i < tgt.Length; i += 4)
            {
                if (tgt[i + 3] != 0)
                {
                    allAlpha = false;
                    break;
                }
            }


            if (allAlpha)
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

            if (widthCopy != gladiusHeader.Width)
            {
                int ibreak = 0;
            }
            if (heightCopy != gladiusHeader.Height)
            {
                int ibreak = 0;
            }


            //this.gladiusHeader.Width = binReader.ReadInt16();
            //this.gladiusHeader.Height = binReader.ReadInt16();
            int ibreak2 = 0;
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
                if(Header.BytesPerPixel == 1)
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        data[i] = binReader.ReadByte();
                    }
                }
                else if (Header.BytesPerPixel == 4)
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
                // if we read past the end, somethings wrong, but try and return as much data as we could get..
            }
            // return the image byte array
            return data;

        }

        public void LoadGladiusImage(BinaryReader binReader)
        {
            intStride = gladiusHeader.Width * gladiusHeader.BytesPerPixel;

            byte[] bimagedata = this.LoadImageBytes(binReader);

            this.ImageByteHandle = GCHandle.Alloc(bimagedata, GCHandleType.Pinned);

            PixelFormat pf = this.Header.BytesPerPixel == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format32bppArgb;


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
                        //Color c = Color.FromArgb(255, map[i * 4], map[(i * 4) + 1], map[(i * 4) + 2]);
                        Color c = Color.FromArgb(map[(i * 4)+3], map[i * 4], map[(i * 4) + 1], map[(i * 4) + 2]);
                        byte A = c.A;
                        if (A == 0)
                        {
                            //A = 20;
                        }
                        Color c2 = Color.FromArgb(A, c.B, c.G, c.R);
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

        public int Width = 0;
        public int Height = 0;
        public int BytesPerPixel;
        public bool ContainsDefinition;

        public byte[] RawColourMap;
        public byte[] SwizzledColourMap;



        public static char[] PTTPHeader = new char[] { 'P', 'T', 'T', 'P' };
        public static char[] NAMEHeader = new char[] { 'N', 'A', 'M', 'E' };

        public static char[] NMPTHeader = new char[] { 'N', 'M', 'P', 'T' };
        public static char[] PFHDHeader = new char[] { 'P', 'F', 'H', 'D' };

        public static char[] r2d2Header = new char[] { 'R', '2', 'D', '2','p','s','x','2' };
        public static char[] tmapHeader = new char[] { 't', 'm', 'a', 'p' };

        static char[][] allTags = { PTTPHeader, NAMEHeader, NMPTHeader, PFHDHeader, r2d2Header };


        public static bool ReadToNextTMapBlock(BinaryReader binReader)
        {
            return Common.FindCharsInStream(binReader,GladiusHeader.tmapHeader);

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
                    {
                        StringBuilder sb = new StringBuilder();
                        char b = (char)binReader.ReadByte();
                        char lastb = 'a';
                        while (true)
                        {
                            if (b != 0x00)
                            {
                                sb.Append(b);
                            }
                            else
                            {
                                if(sb.Length > 0)
                                {
                                    textureNameList.Add(sb.ToString());
                                    sb.Clear();
                                }
                            }
                            if((numTextures != 0 && textureNameList.Count == numTextures))
                            {
                                break;
                            }

                            lastb = b;
                            b = (char)binReader.ReadByte();
                            if (b == 0x00 && lastb == 0x00)
                            {
                                break;
                            }
                        }
                        
                    }
                }

            }
            return textureNameList;
        }

        public bool ValidSize()
        {
            return !(Width <= 0 || Height <= 0 || Width > 1024 || Height > 1024);
        }


        public static bool PositionReaderAtNextImage(BinaryReader binReader,StreamWriter errorStream,String file)
        {
            return Common.FindCharsInStream(binReader, GladiusHeader.r2d2Header) && Common.FindCharsInStream(binReader, GladiusHeader.tmapHeader);
        }


    }



    public class ImageExtractor
    {

        //String targetDirectory = @"d:/gladius-extracted/test-extract/";
        ////String filepath = @"D:\gladius-extracted\ps2-decompressed\converted1\";
        //String filepath = @"d:\gladius-extracted\ps2-decompressed\PTTP\";
        //String errorFile = @"d:\gladius-extracted\ps2-decompressed-errors";

        public static char[] PTTPHeader = new char[] { 'P', 'T', 'T', 'P' };
        public static char[] NAMEHeader = new char[] { 'N', 'A', 'M', 'E' };

        public static char[] NMPTHeader = new char[] { 'N', 'M', 'P', 'T' };
        public static char[] PFHDHeader = new char[] { 'P', 'F', 'H', 'D' };

        public static char[] r2d2Header = new char[] { 'R', '2', 'D', '2','p','s','x','2' };
        public static char[] tmapHeader = new char[] { 't', 'm', 'a', 'p' };

        static char[][] allTags = { PTTPHeader, NAMEHeader, NMPTHeader, PFHDHeader, r2d2Header };


        //String filepath = @"C:\gladius-extracted\ps2-decompressed\PTTP\";
        //String errorFile = @"C:\gladius-extracted\ps2-decompressed-errors";

        public void ExtractImages(string sourceDirectory,string targetDirectory)
        {
            List<string> fileNames = new List<string>();
            fileNames.AddRange(Directory.GetFiles(sourceDirectory, "*"));
            ExtractImages(fileNames,targetDirectory);
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
                            int headerPadding = 0;

                            int subImageCounter = 0;
                            List<String> textureNameList = GladiusHeader.BuildImageList(binReader);
                            List<string> ignoreList = new List<string>();

                            long currentPos = binReader.BaseStream.Position;
                            int tmapSections = Common.CountCharsInStream(binReader, GladiusHeader.tmapHeader);
                            binReader.BaseStream.Position = currentPos;

                            ReadHeaderSection(binReader, imageList);

                            binReader.BaseStream.Position = 0;

                            int counter = 0;
                            foreach(GladiusImage image in imageList)
                            {
                                image.ImageName = textureNameList[counter++];

                                if (image.ImageName == "citizenNor_skin_var04.tga")
                                {
                                    int ibreak = 0;
                                }

                                if (image.Header.ContainsDefinition)
                                {
                                    Debug.Assert(GladiusHeader.ReadToNextTMapBlock(binReader));
                                    subImageCounter++;

                                    binReader.BaseStream.Position += 024;
                                    headerPadding = 0;
                                    if (image.Header.BytesPerPixel == 1)
                                    {
                                        headerPadding = 0x78 - 20;
                                    }
                                    else if (image.Header.BytesPerPixel == 4)
                                    {
                                        headerPadding = 101;
                                    }

                                    binReader.BaseStream.Position += headerPadding;

                                    bool saveImage = true;
                                    String outputFileName = image.ImageName;

                                    errorStream.WriteLine(String.Format("Extracting [{0}][{1}][{2}]", file.Name, subImageCounter, outputFileName));
                                    int imagePadding = 0x13;// 0x24;

                                    if (image.ImageName.Contains("valens"))
                                    {
                                        int ibreak = 0;
                                    }


                                    //if (pf == PixelFormat.Format8bppIndexed)
                                    if (image.Header.BytesPerPixel == 1)
                                    {
                                        if (binReader.BaseStream.Position + 1024 > binReader.BaseStream.Length)
                                        {
                                            // not enough room for a colour map?
                                            break;
                                        }
                                        image.LoadColourMapInfo(binReader);

                                    }

                                    if (image.Header.ValidSize())
                                    {
                                        binReader.BaseStream.Seek(imagePadding, SeekOrigin.Current);
                                        try
                                        {
                                            image.LoadGladiusImage(binReader);
                                            if (saveImage)
                                            {
                                                bool checkSize = true;
                                                bool saveImage2 = true;
                                                if (checkSize)
                                                {
                                                    Point p = new Point();
                                                    if (GladiusImage.s_sizeMap.TryGetValue(image.ImageName, out p))
                                                    {
                                                        if (p.X > image.Header.Width || p.Y > image.Header.Height)
                                                        {
                                                            saveImage2 = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        GladiusImage.s_sizeMap.Add(image.ImageName, new Point(image.Header.Width, image.Header.Height));
                                                    }
                                                }

                                                if(checkSize)
                                                {
                                                    if(saveImage2)
                                                    {
                                                        errorStream.WriteLine(String.Format("Saving image [{0}] [{1}][{2}]", image.ImageName, image.Header.Width, image.Header.Height));
                                                    }
                                                    else
                                                    {
                                                        errorStream.WriteLine(String.Format("skipping image [{0}] [{1}][{2}] as larger exists", image.ImageName, image.Header.Width, image.Header.Height));
                                                    }
                                                }
                                                if (saveImage && saveImage2)
                                                {
                                                    image.Image.Save(targetDirectory + outputFileName + ".png", ImageFormat.Png);
                                                }
                                            }
                                        }
                                        catch (AccessViolationException e)
                                        {
                                            // bleugh.
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

        public void ListImageData(string sourceDirectory, string targetDirectory)
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

            string[] filePaths = Directory.GetFiles(sourceDirectory);


            //String infoFile2 = "c:/gladius-extracted/test-extract/image-texture-header.txt";


            DirectoryInfo d = new DirectoryInfo(sourceDirectory);
            FileInfo[] files = d.GetFiles(); //Getting Text files
            using (StreamWriter errorStream = new StreamWriter(new FileStream(sourceDirectory+@"\errors.txt", FileMode.OpenOrCreate)))
            {
                GladiusImage image = null;
                foreach (FileInfo file in files)
                {
                    using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        int headerPadding = 0;
                        //PixelFormat pf = PixelFormat.Format8bppIndexed;
                        //int width = 0;
                        //int height = 0;

                        int subImageCounter = 0;
                        
                        List<String> textureNameList = GladiusHeader.BuildImageList(binReader);

                        errorStream.WriteLine(file.Name);


                        foreach (String textureName in textureNameList)
                        {
                            errorStream.WriteLine(textureName);
                        }


                        errorStream.WriteLine();
                        errorStream.WriteLine();
                    }
                }
            }

        }


        public void ReadHeaderSection(BinaryReader binReader,List<GladiusImage> imageList)
        {
            if (Common.FindCharsInStream(binReader, PFHDHeader, true))
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
                    imageList.Add(image);



                    binReader.ReadInt32();
                    binReader.ReadInt32();
                    //image.Header.BytesPerPixel = binReader.ReadInt16();
                    image.Header.BytesPerPixel = 1;
                    image.Header.Width = binReader.ReadInt16();
                    image.Header.Height = binReader.ReadInt16();
                    
                    binReader.ReadInt32();

                    image.Header.ContainsDefinition = (binReader.ReadInt32() != -1);
                    if (pad1 == 1)
                    {
                        image.Header.ContainsDefinition = false;
                    }

                    binReader.ReadInt32();
                    binReader.ReadInt32();
                    binReader.ReadInt32();


                    if (image.Header.Height == 0 || image.Header.Width == 0)
                    {
                        int ibreak = 0;
                    }

                }

            }


        }


        static int Main(string[] args)
        {
            //TargaHeader.LoadBECFile();
            //new ImageExtractor().ListImageData();
            //new ImageExtractor().DumpSectionLengths();
            //new ImageExtractor().DumpHeaderInfo();
            List<string> fileNames = new List<string>();
            //fileNames.AddRange(Directory.GetFiles(@"D:\gladius-extract-all-systems\ps2\pak", "*.r2t"));

            String sourcePath = @"D:\gladius-extracted-archive\ps2-decompressed\PTTP\";
            fileNames.AddRange(Directory.GetFiles(sourcePath, "*"));

            //fileNames.Add(sourcePath + "File_023484");
            //fileNames.Add(sourcePath + "File_023911");
            //fileNames.Add(sourcePath + "File_000024");
            //fileNames.Add(sourcePath + "File_001178");
            //fileNames.Add(sourcePath + "File_006095"); // thepit textures - small
            //fileNames.Add(sourcePath + "File_020217"); // thepit textures - large

            String outputDirectory = @"D:\gladius-extracted-archive\ps2-decompressed\texture-output-large\";


            new ImageExtractor().ExtractImages(fileNames,outputDirectory);
            return 0;
        }
    }


}
