using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using ExtractBEC;
using Ionic.Zlib;
using CompressionLevel = Ionic.Zlib.CompressionLevel;
using DeflateStream = Ionic.Zlib.DeflateStream;

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

    static ulong[] m_crcTable = null;

    public static void BuildHashesForFile(string filename)
    {
        string[] files = File.ReadAllLines(filename);

        using (StreamWriter sw = new StreamWriter("d:/tmp/gladius-python-hashes.txt"))
        {
            foreach (string file in files)
            {
                sw.WriteLine(String.Format("0x{0:X} : \"{1}\",", ExtractBec.stringToHash(file), file));
            }

        }




    }




    public static void BuildTable()
    {

        m_crcTable = new ulong[]{
                0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,
    0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
    0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
    0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
    0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
    0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
    0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
    0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
    0x76dc4190, 0x01db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
    0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01,
    0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,
    0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
    0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,
    0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
    0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
    0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
    0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683,
    0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
    0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,
    0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
    0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
    0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
    0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,
    0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
    0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713,
    0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21,
    0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
    0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
    0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,
    0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
    0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,
    0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d,
    }; /* _table */

    }



    public static ulong stringToHash(string val)
    {
        int length = val.Length;
        ulong hashVal = 0;
        int i = 0;
        while (length-- > 0)
        {
            char currentChar = val[i++];
            hashVal = m_crcTable[(hashVal ^ (currentChar)) & 0xff] ^ (hashVal >> 8);
        }
        return hashVal;

    }

    public static void ExtractBecData(String inputFile, String outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }


        Dictionary<uint, int> offsetDictionary = new Dictionary<uint, int>();
        Dictionary<uint, List<String>> offsetNamesDictionary = new Dictionary<uint, List<String>>();

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

            int unknownCount = becFiles.FindAll(o => o.unknown != 0).Count;
            int emptyCount = becFiles.FindAll(o => o.length == 0).Count;



            for (int i = 0; i < sortedList.Count(); ++i)
            {
                int val = 0;
                if (HashData.hashpaths.ContainsKey(sortedList[i].hashCode))
                {
                    if (!offsetDictionary.TryGetValue(sortedList[i].offSet, out val))
                    {
                        offsetDictionary[sortedList[i].offSet] = 1;
                        offsetNamesDictionary[sortedList[i].offSet] = new List<string>();
                        offsetNamesDictionary[sortedList[i].offSet].Add(HashData.hashpaths[sortedList[i].hashCode]);
                    }
                    else
                    {
                        offsetDictionary[sortedList[i].offSet]++;
                        offsetNamesDictionary[sortedList[i].offSet].Add(HashData.hashpaths[sortedList[i].hashCode]);
                    }
                }

                String fileName = "";
                if (!HashData.hashpaths.TryGetValue(sortedList[i].hashCode, out fileName))
                {
                    fileName = "File" + i;
                }
                else
                {
                    int oval = 1;
                    if (!HashData.accessedHashpaths.TryGetValue(sortedList[i].hashCode, out oval))
                    {
                        HashData.accessedHashpaths[sortedList[i].hashCode] = 1;
                    }
                    else
                    {
                        HashData.accessedHashpaths[sortedList[i].hashCode]++;
                    }
                }


                //String outputFile = outputDirectory + "\\" + fileName;
                String outputFile = fileName;

                sortedList[i].fileName = outputFile;
                sortedList[i].SaveDataToFile(binReader, outputDirectory, outputFile);
            }

            uint firstOffset = (uint)sortedList[0].offSet;
            using (StreamWriter sw = new StreamWriter(outputDirectory + "/offsets.txt"))
            {
                //for (int i = 0; i < sortedList.Count - 1; ++i)
                //{
                //    uint val1 = (uint)sortedList[i].offSet + (uint)sortedList[i].length;
                //    if ((uint)sortedList[i].offSet != (uint)sortedList[i + 1].offSet)
                //    {
                //        uint diff = (uint)sortedList[i + 1].offSet - val1;
                //        //sw.WriteLine(String.Format("{0:X}\t{1}\t{2}\t{3}", sortedList[i].hashCode, sortedList[i].offSet - firstOffset, sortedList[i].length, diff));
                //        if (sortedList[i].unknown != 0)
                //        {
                //            sw.WriteLine(String.Format("NonZero Unknown {0:X}\t{1}\t{2}\t{3:G}", sortedList[i].hashCode, sortedList[i].offSet, sortedList[i].length, sortedList[i].unknown));
                //        }
                //    }
                //    else
                //    {
                //        //sw.WriteLine(String.Format("DUPLICATE OFFSET {0:X}\t{1}\t{2}\t{3}\t{4:X}", sortedList[i].hashCode, sortedList[i+1].hashCode, sortedList[i].offSet,sortedList[i].length,sortedList[i].unknown));
                //    }
            }
        }

        using (StreamWriter sw = new StreamWriter(outputDirectory + "/bec-file-hashcodes.txt"))
        {
            //foreach (BecFile bf in sortedList)
            //{

            //    uint beformat = BitConverter.ToUInt32(bf.hashCodeBABE, 0);
            //    string found = "";
            //    string foundVal = "";
            //    if (hashpaths.TryGetValue(bf.hashCode, out foundVal))
            //    {
            //        found = "found in normal";
            //    }
            //    else if (hashpaths.TryGetValue(beformat, out foundVal))
            //    {
            //        found = "found in BE";
            //    }

            //    sw.WriteLine(String.Format("{0}\t{1:X}\t{2:X}\t{3}\t{4}", bf.fileName, bf.hashCode, beformat, found, foundVal));
            //}
        }

        using (StreamWriter sw = new StreamWriter(outputDirectory + "/bec-file-hashcodes-accessinfo.txt"))
        {
            foreach (uint key in offsetNamesDictionary.Keys)
            {
                if (offsetNamesDictionary[key].Count > 1)
                {
                    sw.Write("Offset  {0} has ", key);
                    foreach (string path in offsetNamesDictionary[key])
                    {
                        sw.Write(path);
                        sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }

            //foreach (ulong key in accessedHashpaths.Keys)
            //{
            //    if (accessedHashpaths[key] == 0)
            //    {
            //        sw.WriteLine("File {0} has no references ", hashpaths[key]);
            //    }
            //    else if (accessedHashpaths[key] > 1)
            //    {
            //        sw.WriteLine("File {0} has multiple {1} references ", hashpaths[key], accessedHashpaths[key]);
            //    }
            //}
        }
        int ibreak = 0;
    }


    public static void SummariseBECInfo(String inputFile, StringBuilder results)
    {
        List<BecFile> becFiles = new List<BecFile>();
        using (BinaryReader binReader = new BinaryReader(new FileStream(inputFile, FileMode.Open)))
        {
            byte[] header = binReader.ReadBytes(4);
            short version = binReader.ReadInt16();
            short paddingMultuple = binReader.ReadInt16();
            int numFiles = binReader.ReadInt32();

            if (version != 3)
            {
                numFiles = version;
                version = -1;
            }

            results.AppendFormat("{0}  version {1}  numFiles {2} \n", inputFile, version, numFiles);

        }
    }


    public static List<BecFile> ExtractBecDataOld(String inputFile, String outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        List<BecFile> becFiles = new List<BecFile>();
        using (BinaryReader binReader = new BinaryReader(new FileStream(inputFile, FileMode.Open)))
        {
            byte[] header = binReader.ReadBytes(4);
            int numFiles = binReader.ReadInt32();
            int skip = binReader.ReadInt32();
            //int skip2 = binReader.ReadInt32();

            for (int i = 0; i < numFiles; ++i)
            {
                becFiles.Add(BecFile.FromStreamOld(binReader));
            }
            List<BecFile> sortedList = becFiles.OrderBy(o => o.offSet).ToList();

            BecFile entry = becFiles.Find(x => x.offSet == 177560);
            BecFile entry1 = becFiles.Find(x => x.length == 177560);
            BecFile entry2 = becFiles.Find(x => x.hashCode == 177560);



            for (int i = 0; i < sortedList.Count(); ++i)
            {
                String fileName = "";
                if (!HashData.hashpaths.TryGetValue(sortedList[i].hashCode, out fileName))
                {
                    fileName = "File" + i;
                }
                String outputFile = fileName;

                sortedList[i].fileName = outputFile;
                sortedList[i].SaveDataToFile(binReader, outputDirectory, outputFile);
            }



            using (StreamWriter sw = new StreamWriter(outputDirectory + "/bec-file-hashcodes.txt"))
            {
                foreach (BecFile bf in sortedList)
                {

                    uint beformat = BitConverter.ToUInt32(bf.hashCodeBABE, 0);
                    string found = "";
                    string foundVal = "";
                    if (HashData.hashpaths.TryGetValue(bf.hashCode, out foundVal))
                    {
                        found = "found in normal";
                    }
                    else if (HashData.hashpaths.TryGetValue(beformat, out foundVal))
                    {
                        found = "found in BE";
                    }

                    sw.WriteLine(String.Format("{0}\t{1:X}\t{2:X}\t{3}\t{4}", bf.fileName, bf.hashCode, beformat, found, foundVal));
                }
            }
            int ibreak = 0;
        }
        return becFiles;
    }


    static void Main(string[] args)
    {
        BuildTable();
        HashData.BuildHashInfo();


        ExtractBecData(@"D:\tmp\gladius-xbox\gladius.bec", @"D:\tmp\gladius-xbox\csharp-extracted");

    }


    public static bool incNextAvailable(int[] indices, int length, int pos)
    {
        // tried every combination
        if (pos == length - 1 && indices[pos] == length - 1)
        {
            return false;
        }

        if (indices[pos] < length)
        {
            indices[pos]++;
            if (indices[pos] == length)
            {
                indices[pos] = 0;
                return incNextAvailable(indices, length, pos + 1);
            }
        }
        return true;

    }






}

public class BecFile
{
    public String fileName;
    public uint hashCode;
    public uint hashCodeBE;
    public byte[] hashCodeBA = new byte[4];
    public byte[] hashCodeBABE = new byte[4];
    public uint offSet;
    public uint unknown;
    public uint length;
    public byte[] data;

    public static BecFile FromStream(BinaryReader reader)
    {
        BecFile becFile = new BecFile();
        becFile.hashCode = reader.ReadUInt32();
        reader.BaseStream.Position -= 4;
        becFile.hashCodeBA = reader.ReadBytes(4);
        reader.BaseStream.Position -= 4;
        becFile.hashCodeBABE = reader.ReadBytes(4);
        becFile.hashCodeBE = BitConverter.ToUInt32(becFile.hashCodeBABE, 0);
        Array.Reverse(becFile.hashCodeBABE);
        becFile.offSet = reader.ReadUInt32();
        becFile.unknown = reader.ReadUInt32();
        becFile.length = reader.ReadUInt32();
        return becFile;
    }
    public static BecFile FromStreamOld(BinaryReader reader)
    {
        BecFile becFile = new BecFile();
        //becFile.offSet = reader.ReadUInt32();
        //becFile.hashCode = reader.ReadUInt32();
        //becFile.unknown = reader.ReadUInt32();
        //becFile.length = reader.ReadUInt32();
        becFile.offSet = reader.ReadUInt32();
        becFile.unknown = reader.ReadUInt32();
        becFile.length = reader.ReadUInt32();
        becFile.hashCode = reader.ReadUInt32();


        return becFile;
    }


    public void ReadData(BinaryReader reader)
    {
        reader.BaseStream.Position = offSet;
        data = reader.ReadBytes((int)length);
    }

    public void SaveDataToFile(BinaryReader reader, String outputDirectory, String filename)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        ReadData(reader);
        byte[] result = Inflate(data);
        byte[] recompress = Deflate(result);
        byte[] andBack = Inflate(recompress);
        byte[] andIn = Deflate(andBack);
        byte[] andBack2 = Inflate(andIn);



        int maxLength = Math.Min(result.Length,recompress.Length);
        for(int i=0;i<maxLength;i++)
        {
            if(data[i] != recompress[i])
            {
                int ibreak = 0;
            }
        }

        for(int i=0;i<andBack2.Length;++i)
        {
            if(andBack2[i] != data[i])
            {
                int ibreak = 0;
            }
        }


        //if (result != null && result.Length > 0)
        if (result != null && result.Length > 0)
        {

            FileInfo fileInfo = new FileInfo(outputDirectory + "\\" + filename);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }


            using (System.IO.BinaryWriter outStream = new BinaryWriter(File.Open(fileInfo.FullName, FileMode.Create)))
            {
                outStream.Write(result);
            }
        }

        data = null;
    }


    public static byte[] Inflate(byte[] sourceData)
    {
        if (null == sourceData || sourceData.Length < 1) return null;
        if (sourceData[0] != 0x78)
        {
            return sourceData;
        }

        MemoryStream inputMS = new MemoryStream(sourceData);
        MemoryStream outputMS = new MemoryStream();

        ZlibStream zlStream = new ZlibStream(inputMS, Ionic.Zlib.CompressionMode.Decompress);
        CopyStream(zlStream,outputMS );

        return outputMS.ToArray();

    }

    public static byte[] Deflate(byte[] sourceData)
    {
        MemoryStream inputMS = new MemoryStream(sourceData);
        MemoryStream outputMS = new MemoryStream();

        Stream deflateStream = new ZlibStream(outputMS, Ionic.Zlib.CompressionMode.Compress, CompressionLevel.BestSpeed, true);
        //Stream deflateStream = new DeflateStream(outputMS,Ionic.Zlib.CompressionMode.Compress);
        CopyStream(inputMS, deflateStream);
        deflateStream.Close();
        //outputMS.Flush();
        //outputMS.Close();
        byte[] results = outputMS.ToArray();
        return results;

    }


    static void CopyStream(System.IO.Stream src, System.IO.Stream dest)
    {
        byte[] buffer = new byte[1024];
        int len;
        while ((len = src.Read(buffer, 0, buffer.Length)) > 0)
        {
            dest.Write(buffer, 0, len);
        }
        dest.Flush();
    }


}






