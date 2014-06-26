using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class ModelNamer
    {
        public void ProcessModels()
        {
            //String sourceDirectory = @"D:\gladius-extracted\ps2-decompressed\VERSModelFiles";
            String sourceDirectory = @"D:\gladius-extracted-archive\xbox-decompressed\VERSModelFiles";
            //String targetDirectory = @"D:\gladius-extracted\ps2-decompressed\VERSModelFilesRenamed\";
            String targetDirectory = @"D:\gladius-extracted-archive\xbox-decompressed\VERSModelFilesRenamed\";
            //String infoFile = @"D:\gladius-extracted\ps2-decompressed\modelRenameResults.txt";
            String infoFile = @"D:\gladius-extracted-archive\xbox-decompressed\modelRenameResults.txt";
            char[] modelName = new char[32];
            
            char[] searchString = new char[]{'R','2','D','2','p','s','x','2'};
            String[] files = Directory.GetFiles(sourceDirectory, "*");
            long modelNameOffset = 68;

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        FileInfo sourceFile = new FileInfo(file);
                        if (sourceFile.Name != "File_000376")
                        {
                            //continue;
                        }

                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            if (FindCharsInStream(binReader, searchString))
                            {
                                // found the start of the header
                                binReader.BaseStream.Position += modelNameOffset;
                                binReader.Read(modelName, 0, modelName.Length);

                                int firstNull = 0;
                                while (modelName[firstNull++] != '\0') ;

                                String extractedModelName = new String(modelName, 0, firstNull - 1) + ".mdl";

                                infoStream.WriteLine(String.Format("Moving file [{0}] to [{1}].", sourceFile.Name, targetDirectory + extractedModelName));
                                File.Copy(sourceFile.FullName, targetDirectory + extractedModelName, true);

                            }
                            else
                            {
                                infoStream.WriteLine("Unabled to find search string in file : " + file);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        infoStream.WriteLine("Unable to convert : " + file);
                    }
                }
            }
        }

        public bool FindCharsInStream(BinaryReader binReader, char[] charsToFind)
        {
            bool found = false;
            byte b = (byte)' ';
            int lastFoundIndex = 0;
            try
            {
                while (true)
                {
                    b = binReader.ReadByte();
                    if (b == charsToFind[lastFoundIndex])
                    {
                        lastFoundIndex++;
                        if (lastFoundIndex == charsToFind.Length)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        lastFoundIndex = 0;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return found;

        }


        static void Main(string[] args)
        {
            new ModelNamer().ProcessModels();
        }
    }
}
