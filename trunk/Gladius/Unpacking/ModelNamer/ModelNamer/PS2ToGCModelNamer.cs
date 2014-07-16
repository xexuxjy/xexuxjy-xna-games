using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class PS2ToGCModelNamer
    {
        public List<ModelTextures> m_ps2List = new List<ModelTextures>();
        public List<ModelTextures> m_gcList = new List<ModelTextures>();

        static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };


        public void DoComparison()
        {
            string ps2ModelPath = @"D:\gladius-extracted\ps2-decompressed\VERSModelFilesRenamed";
            string gcModelPath = @"D:\gladius-extracted-archive\gc-compressed\probable-models";
            string infoFile = @"D:\gladius-extracted-archive\gc-compressed\model-rename-info";

            ReadTexture(ps2ModelPath, infoFile, m_ps2List);
            ReadTexture(gcModelPath,infoFile,m_gcList);

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                infoStream.WriteLine("PS2 Files");
                foreach (ModelTextures mt in m_ps2List)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(mt.modelName);
                    sb.Append(" : ");
                    foreach (string texturename in mt.m_textures)
                    {
                        sb.Append(texturename);
                        sb.Append(" ");
                    }
                    infoStream.WriteLine(sb.ToString());
                }
                
                infoStream.WriteLine();
                infoStream.WriteLine();
                infoStream.WriteLine();

                infoStream.WriteLine("GC Files");
                foreach (ModelTextures mt in m_gcList)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(mt.modelName);
                    sb.Append(" : ");
                    foreach (string texturename in mt.m_textures)
                    {
                        sb.Append(texturename);
                        sb.Append(" ");
                    }
                    infoStream.WriteLine(sb.ToString());
                }


            }
        }


        public void ReadTexture(String sourceDirectory, String infoFile, List<ModelTextures> modelTextureList)
        {
            String[] files = Directory.GetFiles(sourceDirectory, "*");
            int counter = 0;

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        FileInfo sourceFile = new FileInfo(file);
                        if (sourceFile.Name != "File 000489")
                        {
                            //continue;
                        }

                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {

                            ModelTextures modelTextures = new ModelTextures();
                            modelTextures.modelName = sourceFile.Name;
                            modelTextureList.Add(modelTextures);

                            if (Common.FindCharsInStream(binReader, txtrTag))
                            {
                                int dslsSectionLength = binReader.ReadInt32();
                                int uk2a = binReader.ReadInt32();
                                int numTextures = binReader.ReadInt32();
                                int textureSlotSize = 0x98;

                                for (int i = 0; i < numTextures; ++i)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    bool valid = true;
                                    for (int j = 0; j < textureSlotSize; ++j)
                                    {
                                        char b = binReader.ReadChar();
                                        if (valid && b != 0x00)
                                        {
                                            sb.Append(b);
                                        }
                                        else
                                        {
                                            valid = false;
                                        }
                                    }
                                    modelTextures.m_textures.Add(sb.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }


        static void Main(string[] args)
        {
            PS2ToGCModelNamer modelNamer = new PS2ToGCModelNamer();
            modelNamer.DoComparison();



        }

    
    
    }

    


    public class ModelTextures
    {
        public String modelName;
        public String newModelName;
        public List<String> m_textures = new List<String>();
    }

}
