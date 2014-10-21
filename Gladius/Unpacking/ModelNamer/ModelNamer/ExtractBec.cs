using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ModelNamer
{

    /*
     *
     * 
http://forum.xentax.com/viewtopic.php?f=10&t=1678
+---------------------+
| Gladius (PS2) *.bec |
+---------------------+

// Some file entries are listed more than once, with the same offset and length,
// so you need to remove all duplicates.

4 - Header ( ceb)
2 - Version (3)
2 - Padding Multiple (2048)
4 - Number Of Files
4 - null

// for each file
  4 - Hash?
  4 - File Offset
  4 - Unknown (0/2) (BIG)
  4 - File Length
  
0-2047 - null Padding to a multiple of 2048 bytes

// for each file
  X - File Data
  0-2047 - null Padding to a multiple of 2048 bytes
     */
   
    
    public class ExtractBec
    {
        public static void ExtractBecData(String inputFile, String outputDirectory)
        {
            List<BecFile> becFiles = new List<BecFile>();
            using (BinaryReader binReader = new BinaryReader(new FileStream(inputFile, FileMode.Open)))
            {
                byte[] header = binReader.ReadBytes(4);
                byte[] version = binReader.ReadBytes(2);
                short paddingMultuple = binReader.ReadInt16();
                int numFiles = binReader.ReadInt32();
                int skip = binReader.ReadInt32();
                for (int i = 0; i < numFiles; ++i)
                {
                    becFiles.Add(BecFile.FromStream(binReader));
                }
                List<BecFile> sortedList = becFiles.OrderBy(o => o.offSet).ToList();
                for (int i = 0; i < sortedList.Count(); ++i)
                {
                    String outputFile = outputDirectory + "\\File" + i;
                    sortedList[i].SaveDataToFile(binReader, outputDirectory,outputFile);
                }
                int ibreak = 0;
            }
        }



        static void Main()
        {
            //ExtractBecData(@"D:\xbox-games\gladius\gladius.bec", @"D:\xbox-games\gladius\bec-extract");
            //ExtractBecData(@"E:\LucasArts Gladius [GLSE64]\root\gladius.bec", @"D:\gladius-extract-all-systems\gcn");
            ExtractBecData(@"C:\tmp\ps2\gladius_ps2_NTSC\DATA.BEC", @"D:\gladius-extract-all-systems\ps2");
        }
    }

    public class BecFile
    {
        public int hashCode;
        public int offSet;
        public int unknown;
        public int length;
        public byte[] data;

        public static BecFile FromStream(BinaryReader reader)
        {
            BecFile becFile = new BecFile();
            becFile.hashCode = reader.ReadInt32();
            becFile.offSet = reader.ReadInt32();
            becFile.unknown = reader.ReadInt32();
            becFile.length = reader.ReadInt32();
            return becFile;
        }

        public void ReadData(BinaryReader reader)
        {
            reader.BaseStream.Position = offSet;
            data = reader.ReadBytes(length);
        }

        public void SaveDataToFile(BinaryReader reader,String outputDirectory,String filename)
        {

            ReadData(reader);
            byte[] result = Inflate(data);
            if (result != null && result.Length > 0)
            {

                if (result.Length > 4 && result[0] == 'P' && result[1] == 'A' && result[2] == 'K' && result[3] == '1')
                {
                    using (BinaryReader binReader = new BinaryReader(new MemoryStream(result)))
                    {
                        String pakOutput = outputDirectory + "\\pak";
                        PAK1Splitter.Read(binReader, pakOutput);
                    }
                }
                else
                {
                    using (System.IO.BinaryWriter outStream = new BinaryWriter(File.Open(filename, FileMode.Create)))
                    {
                        outStream.Write(result);
                    }
                }
            }
            // TODO - categorise files based on tags.
            // unpack pak1 files?
            // rename 'known' files?

            data = null;
        }

        static byte[] largeArray = new byte[1024 * 1024];

        public static byte[] Inflate(byte[] sourceData)
        {
            if (null == sourceData || sourceData.Length < 1) return null;
            if (sourceData[0] != 0x78)
            {
                return sourceData;
            }

            MemoryStream ms = new MemoryStream(sourceData);
            bool zlib = true;
            Inflater inflater = new Inflater(!zlib);
            InflaterInputStream inStream = new InflaterInputStream(ms, inflater);
            //byte[] buf2 = new byte[largeArray.Length];

            int currentIndex = 0;
            int count = largeArray.Length;

            try
            {
                while (true)
                {
                    int numRead = inStream.Read(largeArray, currentIndex, count);
                    if (numRead <= 0)
                    {
                        break;
                    }
                    currentIndex += numRead;
                    count -= numRead;
                }
                byte[] resultArray = new byte[currentIndex];
                Array.ConstrainedCopy(largeArray,0,resultArray,0,currentIndex);
                return resultArray;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception - '{0}'", ex.Message);
                throw;
            }
        }
    }
}



