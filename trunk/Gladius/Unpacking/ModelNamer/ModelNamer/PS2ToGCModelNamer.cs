using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class MTPair
    {
        public ModelTextures m_ps2;
        public ModelTextures m_gc;
        public MTPair(ModelTextures ps2, ModelTextures gc)
        {
            m_ps2 = ps2; m_gc = gc;
        }
    }



    public class PS2ToGCModelNamer
    {
        public List<ModelTextures> m_ps2List = new List<ModelTextures>();
        public List<ModelTextures> m_gcList = new List<ModelTextures>();

        public List<MTPair> m_results = new List<MTPair>();
        static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };


        public void DoComparison()
        {
            string ps2ModelPath = @"D:\gladius-extracted\ps2-decompressed\VERSModelFilesRenamed";
            string gcModelPath = @"D:\gladius-extracted-archive\gc-compressed\probable-models";
            string gcModelOutputPath = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed";
            string infoFile = @"D:\gladius-extracted-archive\gc-compressed\model-rename-info";

            ReadTexture(ps2ModelPath, infoFile, m_ps2List);
            ReadTexture(gcModelPath,infoFile,m_gcList);

            

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                if (true)
                {

                    foreach (ModelTextures ps2mt in m_ps2List)
                    {
                        foreach (ModelTextures gcmt in m_gcList)
                        {
                            if (ps2mt.m_textures.Count > 0 && gcmt.m_textures.Count > 0)
                            {

                                if (ps2mt.m_textures.SequenceEqual<String>(gcmt.m_textures))
                                {
                                    MTPair pair = new MTPair(ps2mt, gcmt);
                                    m_results.Add(pair);
                                    ps2mt.m_pairResults.Add(pair);
                                }
                            }
                        }
                    }


                    foreach (ModelTextures ps2mt in m_ps2List)
                    {
                        if (ps2mt.m_pairResults.Count == 1)
                        {
                            FileInfo inFile = new FileInfo(gcModelPath+"\\"+ps2mt.m_pairResults[0].m_gc.modelName);
                            FileInfo outFile = new FileInfo(gcModelOutputPath + "\\" + ps2mt.modelName);

                            infoStream.WriteLine("Renaming "+inFile.FullName+" to "+outFile.FullName);
                            File.Copy(inFile.FullName,outFile.FullName);

                            //foreach (MTPair pair in ps2mt.m_pairResults)
                            //{

                            //    infoStream.WriteLine("PS2 : " + pair.m_ps2.modelName + "  GC : " + pair.m_gc.modelName);
                            //    StringBuilder sb = new StringBuilder();
                            //    foreach (string texturename in pair.m_ps2.m_textures)
                            //    {
                            //        sb.Append(texturename);
                            //        sb.Append(" ");
                            //    }
                            //    infoStream.WriteLine(sb.ToString());
                            //    infoStream.WriteLine();

                            //}
                        }
                    }
                }

                if (false)
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
                        if (sourceFile.Name != "wandering_soul_steppes.mdl")
                        {
                            //int ibreak = 0;
                            //continue;
                        }

                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {

                            ModelTextures modelTextures = new ModelTextures();
                            modelTextures.modelName = sourceFile.Name;
                            modelTextureList.Add(modelTextures);

                            Common.ReadTextureNames(binReader, txtrTag, modelTextures.m_textures);
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
        public List<MTPair> m_pairResults = new List<MTPair>();

    }

}
