using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{

    public class GCModel
    {
        public GCModel(String name)
        {
            m_name = name;
        }

        public String m_name;
        public sVector3 m_center;
        public List<sVector3> m_points = new List<sVector3>();
        public List<sVector3> m_normals = new List<sVector3>();
        public List<sVector2> m_uvs = new List<sVector2>();

        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();

    }


    public class GCModelReader
    {
        static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
        static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
        static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' };
        static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
        static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
        static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
        static char[] dslsTag = new char[] { 'D', 'S', 'L', 'S' };
        static char[] dsliTag = new char[] { 'D', 'S', 'L', 'I' };
        static char[] dslcTag = new char[] { 'D', 'S', 'L', 'C' };
        static char[] posiTag = new char[] { 'P', 'O', 'S', 'I' };
        static char[] normTag = new char[] { 'N', 'O', 'R', 'M' };
        static char[] uv0Tag = new char[] { 'U', 'V', '0', ' ' };
        static char[] vflaTag = new char[] { 'V', 'F', 'L', 'A' };
        static char[] ramTag = new char[] { 'R', 'A', 'M', ' ' };
        static char[] msarTag = new char[] { 'M', 'S', 'A', 'R' };
        static char[] nlvlTag = new char[] { 'N', 'L', 'V', 'L' };
        static char[] meshTag = new char[] { 'M', 'E', 'S', 'H' };
        static char[] elemTag = new char[] { 'E', 'L', 'E', 'M' };



        static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag, dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag,vflaTag,ramTag,msarTag,nlvlTag,meshTag,elemTag };

        public List<GCModel> m_models = new List<GCModel>();

        public void LoadModels()
        {
            LoadModels(@"c:\tmp\unpacking\gc-models\", @"c:\tmp\unpacking\gc-models\results.txt");
        }


        public void LoadModels(String sourceDirectory,String infoFile)
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
                            GCModel model = new GCModel(sourceFile.Name);
                            m_models.Add(model);
                            if (Common.FindCharsInStream(binReader, posiTag))
                            {
                                int posSectionLength = binReader.ReadInt32();
                                int uk2 = binReader.ReadInt32();
                                int numPoints = binReader.ReadInt32();
                                for (int i = 0; i < numPoints; ++i)
                                {
                                    model.m_points.Add(sVector3.FromStreamFloatBE(binReader));
                                }

                                if (Common.FindCharsInStream(binReader, normTag))
                                {
                                    int normSectionLength = binReader.ReadInt32();
                                    int uk4 = binReader.ReadInt32();
                                    int numNormals = binReader.ReadInt32();
                                    if (numNormals != numPoints)
                                    {
                                        int ibreak = 0;
                                    }

                                    for (int i = 0; i < numNormals; ++i)
                                    {
                                        model.m_normals.Add(sVector3.FromStreamFloatBE(binReader));
                                    }
                                }
                                if (Common.FindCharsInStream(binReader, uv0Tag))
                                {
                                    int uvSectionLength = binReader.ReadInt32();
                                    int uk4 = binReader.ReadInt32();
                                    int numUV = binReader.ReadInt32();
                                    //if (numNormals != numPoints)
                                    //{
                                    //    int ibreak = 0;
                                    //}

                                    for (int i = 0; i < numUV; ++i)
                                    {
                                        model.m_uvs.Add(sVector2.FromStreamFloatBE(binReader));
                                    }
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

        public void DumpPoints(String infoFile)
        {
            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (GCModel model in m_models)
                {
                    infoStream.WriteLine(String.Format("File : {0} : {1} : {2}", model.m_name, model.m_points.Count, model.m_normals.Count));
                    infoStream.WriteLine("Verts : ");
                    foreach (sVector3 sv in model.m_points)
                    {
                        sv.WriteInt(infoStream);
                    }
                    infoStream.WriteLine("Normals : ");
                    foreach (sVector3 sv in model.m_normals)
                    {
                        sv.WriteInt(infoStream);
                    }
                    infoStream.WriteLine();
                    infoStream.WriteLine();
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
                            GCModel model = new GCModel(sourceFile.Name);
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
            String modelPath = @"C:\tmp\unpacking\gc-probable-models\probable-models";
            String infoFile = @"c:\tmp\unpacking\gc-models\results.txt";
            String sectionInfoFile = @"C:\tmp\unpacking\gc-probable-models\sectionInfo.txt";
            GCModelReader reader = new GCModelReader();
            //reader.LoadModels(modelPath,infoFile);
            //reader.DumpPoints(infoFile);
            reader.DumpSectionLengths(modelPath, sectionInfoFile);



        }


    }

}
