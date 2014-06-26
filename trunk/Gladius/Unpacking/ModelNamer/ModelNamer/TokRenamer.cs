using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    class TokRenamer
    {
        public void DoRename()
        {

            String sourceDirectory = @"d:\gladius-extracted\ps2-decompressed\ShopData\";
            String infoFile = @"d:\gladius-extracted\ps2-decompressed\findFaceResults.txt";

            String[] files = Directory.GetFiles(sourceDirectory, "*");

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    FileInfo sourceFile = new FileInfo(file);
                    String newFilename = null;
                    String[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("// ") && line.EndsWith(".tok"))
                        {
                            newFilename = line.Substring(3);
                            try
                            {
                                if (newFilename != sourceFile.Name && !File.Exists(sourceDirectory + newFilename))
                                {
                                    File.Move(file, sourceDirectory + newFilename);
                                }
                            }
                            catch (Exception e)
                            {
                            }

                            int ibreak = 0;
                        }
                    }

                }
            }
        }

        static void Main(string[] args)
        {
            new TokRenamer().DoRename();
        }


    }
}
