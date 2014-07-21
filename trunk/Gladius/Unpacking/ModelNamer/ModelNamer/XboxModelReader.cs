using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.IO;

namespace ModelNamer
{
    public class XboxModelReader
    {

        static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
        static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
        static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' };
        static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
        static char[] stypTag = new char[] { 'S', 'T', 'Y', 'P' };
        static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' };
        static char[] skelTag = new char[] { 'S', 'K', 'E', 'L' };

        static char[] xrndTag = new char[] { 'X', 'R', 'N', 'D' };
        static char[] doegTag = new char[] { 'd', 'o', 'e', 'g' };

        // NAME and SKEL always exist together
        // STYP can appear with them or without them, or not at all.



        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };

        static char[][] allTags = { versTag, cprtTag, selsTag, txtrTag,stypTag,nameTag,skelTag,xrndTag};


        public void LoadModels(String sourceDirectory, String infoFile, int maxFiles = -1)
        {
            m_models.Clear();
            String[] files = Directory.GetFiles(sourceDirectory, "*");
            int counter = 0;

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        FileInfo sourceFile = new FileInfo(file);
                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            if (sourceFile.Name != "File 005496")
                            {
                                // 410, 1939

                                continue;
                            }


                            XboxModel model = new XboxModel(sourceFile.Name);


                            m_models.Add(model);
                            if (Common.FindCharsInStream(binReader, versTag))
                            {
                                int dslsSectionLength = binReader.ReadInt32();
                                int uk2a = binReader.ReadInt32();
                                int uk2b = binReader.ReadInt32();


                            }
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    counter++;
                    if (maxFiles > 0 && counter > maxFiles)
                    {
                        break;
                    }

                }
            }
        }

        public void DumpSectionLengths(String sourceDirectory, String infoFile)
        {
            m_models.Clear();
            String[] files = Directory.GetFiles(sourceDirectory, "*");

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        FileInfo sourceFile = new FileInfo(file);
                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            XboxModel model = new XboxModel(sourceFile.Name);
                            m_models.Add(model);
                            infoStream.WriteLine("File : " + model.m_name);
                            foreach (char[] tag in allTags)
                            {
                                if (Common.FindCharsInStream(binReader, tag, true))
                                {
                                    int blockSize = binReader.ReadInt32();
                                    model.m_tagSizes[tag] = blockSize;
                                    infoStream.WriteLine(String.Format("\t {0} : {1}", new String(tag), blockSize));
                                }
                                else
                                {
                                    model.m_tagSizes[tag] = -1;
                                }
                            }

                            binReader.BaseStream.Position = 0;
                            Common.ReadTextureNames(binReader, txtrTag, model.m_textures);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Textures : ");
                            foreach (string textureName in model.m_textures)
                            {
                                sb.AppendLine(textureName);
                            }
                            infoStream.WriteLine(sb.ToString());

                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

        }

        public void DumpTextureNames()
        {


        }





        static void Main(string[] args)
        {
            String modelPath = @"C:\tmp\unpacking\xbox-ModelFiles\VERSModelFiles\doegfiles";
            String infoFile = @"c:\tmp\unpacking\xbox-ModelFiles\model-reader-results.txt";
            String sectionInfoFile = @"C:\tmp\unpacking\xbox-ModelFiles\model-reader-sectionInfo.txt";
            XboxModelReader reader = new XboxModelReader();
            //reader.LoadModels(modelPath,infoFile);
            //reader.DumpPoints(infoFile);
            reader.DumpSectionLengths(modelPath, sectionInfoFile);



        }




        List<XboxModel> m_models = new List<XboxModel>();
    }

    public class XboxModel
    {

        public XboxModel(String name)
        {
            m_name = name;
        }

        public String m_name;
        public Vector3 m_center;
        public List<Vector3> m_points = new List<Vector3>();
        public List<Vector3> m_normals = new List<Vector3>();
        public List<Vector2> m_uvs = new List<Vector2>();

        public List<String> m_textures = new List<string>();

        // doeg tag always 7C (124) , encloes start and end doeg values , 2 per file , has FFFF following

        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();

    }

    public class DoegSection
    {

    }


}

