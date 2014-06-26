using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class GridFileReader
    {

        public void ReadFile()
        {
            String inputFileDir = @"D:\gladius-extracted\ps2-decompressed\FStartMaybeGridFiles\";
            String outputFileDir = @"D:\gladius-extracted\ps2-decompressed\FStartMaybeGridFiles-Converted\";

            String filename = "File_000022";

            int rowLength = 32;
            int offset = 0x540;
            byte[] singleRow = new byte[rowLength];

            String[] files = Directory.GetFiles(inputFileDir, "*");


            foreach (String file in files)
            {
                FileInfo sourceFile = new FileInfo(file);
                if (sourceFile.Name != "File_000376")
                {
                    //continue;
                }

                using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(outputFileDir + sourceFile.Name+".txt"))
                {
                    using (BinaryReader binReader = new BinaryReader(new FileStream(inputFileDir + sourceFile.Name, FileMode.Open)))
                    {
                        try
                        {
                            int header = binReader.ReadInt32();
                            int numEntries = binReader.ReadInt32();
                            int totalEntries = 30;
                            int entryOffset = totalEntries * 0x20;

                            binReader.BaseStream.Position += entryOffset;


                            int counter = 0;
                            StringBuilder line = null;
                            while (true)
                            {
                                if (counter == 0)
                                {
                                    line = new StringBuilder();
                                }

                                int v = binReader.ReadInt32();
                                char toAdd = ' ';
                                switch(v)
                                {
                                    case 0x00:
                                        toAdd = ' ';
                                        break;
                                    case 0x01:
                                        toAdd = '#';
                                        break;
                                    case 0x02:
                                        toAdd = '*';
                                        break;
                                    case 0x04:
                                        toAdd = '@';
                                        break;
                                    case 0x08:
                                        toAdd = '}';
                                        break;
                                    case 0x0A:
                                        toAdd = '>';
                                        break;
                                    case 0x10:
                                        toAdd = '!';
                                        break;
                                    case 0x20:
                                        toAdd = '?';
                                        break;
                                    case 0x40:
                                        toAdd = '+';
                                        break;
                                    case 0x80:
                                        toAdd = '&';
                                        break;
                                    case 768:
                                        toAdd = '$';
                                        break;
                                    case -1073741822:
                                        toAdd = '<';
                                        break;
                                    case -2147483640:
                                        toAdd = '%';
                                        break;
                                    case -2147483647:
                                        toAdd = '{';
                                        break;
                                    case -2147483644:
                                        toAdd = ':';
                                        break;
                                    default:
                                        int ibreak = 0;
                                        break;
                                }

                                line.Append(toAdd);
                                counter++;
                                if (counter == rowLength)
                                {
                                    infoStream.WriteLine(line.ToString());
                                    counter = 0;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            new GridFileReader().ReadFile();
        }



    }
}
