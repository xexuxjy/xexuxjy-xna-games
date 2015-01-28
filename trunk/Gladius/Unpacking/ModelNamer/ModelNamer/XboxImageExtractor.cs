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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ModelNamer
{


    public class XboxGladiusImage
    {
        private XboxGladiusHeader gladiusHeader = null;
        private string strFileName = string.Empty;
        public string ImageName;
        public bool ContainsSkygold = false;

        public Texture2D XNATexture;
        public byte[] CompressedData;

        public XboxGladiusImage()
        {
            this.gladiusHeader = new XboxGladiusHeader();
        }


        /// <summary>
        /// Gets a TargaHeader object that holds the Targa Header information of the loaded file.
        /// </summary>
        public XboxGladiusHeader Header
        {
            get { return this.gladiusHeader; }
            set { this.gladiusHeader = value; }
        }


        /// <summary>
        /// Gets the full path and filename of the loaded file.
        /// </summary>
        public string FileName
        {
            get { return this.strFileName; }
        }



        ~XboxGladiusImage()
        {
        }


    }



    public class XboxGladiusHeader
    {
        public int Width = 0;
        public int Height = 0;
        public int CompressedSize = 0;
        public bool ContainsDefinition;
        public ushort DXTType = 0;
    }



    public class XboxImageExtractor : Game
    {
        GraphicsDeviceManager graphics;

        public XboxImageExtractor()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;


        }

        public void ExtractImages(string sourceDirectory, string targetDirectory)
        {
            List<string> fileNames = new List<string>();
            fileNames.AddRange(Directory.GetFiles(sourceDirectory, "*"));
            ExtractImages(fileNames, targetDirectory);
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            List<string> fileNames = new List<string>();
            String sourcePath = @"D:\gladius-extracted-archive\ps2-decompressed\ClassImages\";
            sourcePath = @"C:\tmp\gladius-extracted-archive\gladius-extracted-archive\xbox-decompressed\PTTPFiles";
            
            fileNames.AddRange(Directory.GetFiles(sourcePath, "**"));
            String outputDirectory = @"C:\tmp\xbox-texture-output\";
            ExtractImages(fileNames, outputDirectory);
            Exit();
        }


        public void ReadPTDTSection(BinaryReader binReader, List<XboxGladiusImage> imageList)
        {
            if (Common.FindCharsInStream(binReader, Common.ptdtTag))
            {
                uint sectionSize = binReader.ReadUInt32();
                int skip = 8;
                int adjustedSize = (int)sectionSize - skip;
                binReader.BaseStream.Position += skip;


                int bpp = 3;
                int imageCount = 0;
                int totalCount = 0;
                foreach (XboxGladiusImage image in imageList)
                {
                    totalCount += image.Header.CompressedSize;
                }

                foreach (XboxGladiusImage image in imageList)
                {
                    imageCount++;
                    image.CompressedData = binReader.ReadBytes(image.Header.CompressedSize);
                    int expanded = image.Header.CompressedSize * 3;
                    int imgSize = image.Header.Width * image.Header.Height;


                    try
                    {

                        ManagedSquish.SquishFlags flag;
                        if (image.Header.DXTType == 1)
                        {
                            flag = ManagedSquish.SquishFlags.Dxt1;
                        }
                        else if (image.Header.DXTType == 5)
                        {
                            flag = ManagedSquish.SquishFlags.Dxt5;
                        }
                        else
                        {
                            flag = ManagedSquish.SquishFlags.Dxt5;
                        }

                        byte[] result = ManagedSquish.Squish.DecompressImage(image.CompressedData, image.Header.Width, image.Header.Height, flag);

                        int a1 = result.Length / 4;
                        int b1 = image.Header.Width * image.Header.Height;

                        Microsoft.Xna.Framework.Color[] colorData = new Microsoft.Xna.Framework.Color[result.Length / 4];
                        for (int i = 0; i < result.Length; i += 4)
                        {
                            byte r = result[i + 0];
                            byte g = result[i + 1];
                            byte b = result[i + 2];
                            byte a = result[i + 3];
                            //a = 0xff;
                            Microsoft.Xna.Framework.Color c = new Microsoft.Xna.Framework.Color(r, g, b, a);
                            colorData[i / 4] = c;
                            //result[i + 0] = a;
                            //result[i + 1] = r;
                            //result[i + 2] = g;
                            //result[i + 3] = b;
                        }

                        image.XNATexture = new Texture2D(graphics.GraphicsDevice, image.Header.Width, image.Header.Height, false, SurfaceFormat.Color);
                        image.XNATexture.SetData<Microsoft.Xna.Framework.Color>(colorData);
                    }
                    catch (Exception e)
                    {
                        System.Console.Out.WriteLine(e.StackTrace);
                        int ibreak = 0;
                    }
                }
            }
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
                    List<XboxGladiusImage> imageList = new List<XboxGladiusImage>();
                    try
                    {
                        FileInfo file = new FileInfo(fileName);
                        using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                        using (BinaryReader binReader = new BinaryReader(fs))
                        {
                            List<String> textureNameList = new List<string>();
                            Common.ReadNullSeparatedNames(binReader, Common.nameTag, textureNameList);
                            {
                                long currentPos = binReader.BaseStream.Position;

                                ReadHeaderSection(binReader, imageList,textureNameList);
                                binReader.BaseStream.Position = 0;
                                ReadPTDTSection(binReader, imageList);
                            
                                foreach(XboxGladiusImage gi in imageList)
                                {
                                    if (gi.XNATexture != null)
                                    {
                                        //using (FileStream fs2 = new FileStream(targetDirectory + gi.ImageName + ".png", FileMode.OpenOrCreate))
                                        {
                                            //gi.XNATexture.SaveAsPng(fs2, gi.Header.Width, gi.Header.Height);
                                            TextureToPng(gi.XNATexture, gi.Header.Width, gi.Header.Height, ImageFormat.Png, targetDirectory + gi.ImageName + ".png");
                                        }
                                    }
                                    gi.CompressedData = null;
                                    gi.XNATexture = null;
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


        public void ReadHeaderSection(BinaryReader binReader, List<XboxGladiusImage> imageList,List<string> textureNames)
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
                    XboxGladiusImage image = new XboxGladiusImage();
                    imageList.Add(image);
                    image.ImageName = textureNames[u];
                    
                    short compressType = binReader.ReadInt16();
                    image.Header.DXTType = (ushort)(compressType == 17152? 5 : 1);
                    binReader.ReadInt16();
                    binReader.ReadInt32();
                    image.Header.Width = binReader.ReadInt16();
                    image.Header.Height = binReader.ReadInt16();
                    image.Header.CompressedSize = binReader.ReadInt32();

                    binReader.BaseStream.Position += 16;

                }

            }


        }

        public static void TextureToPng(Texture2D texture, int width, int height, ImageFormat imageFormat, string filename)
        {
            using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                byte blue;
                IntPtr safePtr;
                BitmapData bitmapData;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
                byte[] textureData = new byte[4 * width * height];

                texture.GetData<byte>(textureData);
                for (int i = 0; i < textureData.Length; i += 4)
                {
                    blue = textureData[i];
                    textureData[i] = textureData[i + 2];
                    textureData[i + 2] = blue;
                }
                bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                safePtr = bitmapData.Scan0;
                Marshal.Copy(textureData, 0, safePtr, textureData.Length);
                bitmap.UnlockBits(bitmapData);
                bitmap.Save(filename, imageFormat);
            }
        }

        static int Main(string[] args)
        {
            using (Game g = new XboxImageExtractor())
            {
                g.Run();
            }

            return 0;
        }
    }


}
