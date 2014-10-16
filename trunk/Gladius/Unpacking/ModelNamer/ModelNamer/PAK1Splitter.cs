using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class PAK1Splitter
    {
        public void Read(FileInfo inputFileInfo, String outputPath)
        {
            using (BinaryReader binReader = new BinaryReader(new FileStream(inputFileInfo.FullName, FileMode.Open)))
            {


                Common.FindCharsInStream(binReader, Common.pak1Tag);
                int numFiles = binReader.ReadInt32();
                List<HeaderBlock> headers = new List<HeaderBlock>();

                for (int i = 0; i < numFiles; ++i)
                {
                    headers.Add(HeaderBlock.FromBinaryStream(binReader));
                }

                for (int i = 0; i < numFiles; ++i)
                {
                    binReader.BaseStream.Position = headers[i].filenamePosition;
                    StringBuilder sb = new StringBuilder();
                    char b;
                    int count = 0;
                    while ((b = (char)binReader.ReadByte()) != 0x00)
                    {
                        count++;
                        sb.Append(b);
                    }
                    headers[i].filename = sb.ToString();
                }

                for (int i = 0; i < numFiles; ++i)
                {
                    binReader.BaseStream.Position = headers[i].fileDataPosition;
                    long lengthToRead = (i < numFiles - 1) ? (headers[i + 1].fileDataPosition - headers[i].fileDataPosition) : (binReader.BaseStream.Length - headers[i].fileDataPosition);
                    headers[i].filedata = binReader.ReadBytes((int)lengthToRead);
                }

                foreach (HeaderBlock header in headers)
                {
                    String tagOutputDirname = outputPath + "\\"+inputFileInfo.Name+".out";
                    try
                    {
                        Directory.CreateDirectory(tagOutputDirname);
                    }
                    catch (Exception e) { }

                    String tagOutputFilename = tagOutputDirname + "\\" + (header.filename);
                    using (System.IO.BinaryWriter outStream = new BinaryWriter(File.Open(tagOutputFilename, FileMode.Create)))
                    {
                        outStream.Write(header.filedata);
                    }
                }

            }
        }

        static void Main(string[] args)
        {
            String sourceDirectory = @"D:\gladius-extracted\ps2-decompressed\PAK1Animations";
            String[] files = Directory.GetFiles(sourceDirectory, "*");
            int counter = 0;

            foreach (String file in files)
            {
                FileInfo sourceFile = new FileInfo(file);

                new PAK1Splitter().Read(sourceFile, @"D:\gladius-extracted\ps2-decompressed\PAK1Animations-Unpacked");
            }
        }
    }


    public class HeaderBlock
    {
        public int filenamePosition;
        public int fileDataPosition;
        public int val3;
        public int val4;
        public String filename;
        public byte[] filedata;


        public static HeaderBlock FromBinaryStream(BinaryReader binReader)
        {
            HeaderBlock header = new HeaderBlock();
            header.filenamePosition = binReader.ReadInt32();
            header.fileDataPosition = binReader.ReadInt32();
            header.val3 = binReader.ReadInt32();
            header.val4 = binReader.ReadInt32();

            return header;


        }

    }
}
