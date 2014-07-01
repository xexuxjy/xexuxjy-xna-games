﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;

namespace ModelNamer
{

    public class GCModel
    {
        public GCModel(String name)
        {
            m_name = name;
        }

        public void BuildBB()
        {
            MinBB.X = MinBB.Y = MinBB.Z = float.MaxValue;
            MaxBB.X = MaxBB.Y = MaxBB.Z = float.MinValue;

            for (int i = 0; i < m_points.Count; ++i)
            {
                if (m_points[i].X < MinBB.X) MinBB.X = m_points[i].X;
                if (m_points[i].Y < MinBB.Y) MinBB.Y = m_points[i].Y;
                if (m_points[i].Z < MinBB.Z) MinBB.Z = m_points[i].Z;
                if (m_points[i].X > MaxBB.X) MaxBB.X = m_points[i].X;
                if (m_points[i].Y > MaxBB.Y) MaxBB.Y = m_points[i].Y;
                if (m_points[i].Z > MaxBB.Z) MaxBB.Z = m_points[i].Z;

            }

        }

        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();
        public String m_name;
        public List<Vector3> m_points = new List<Vector3>();
        public List<Vector3> m_normals = new List<Vector3>();
        public Vector3 MinBB;
        public Vector3 MaxBB;
        public Vector3 Center;
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



        static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag, dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag, vflaTag, ramTag, msarTag, nlvlTag, meshTag, elemTag };

        public List<GCModel> m_models = new List<GCModel>();

        public void LoadModels()
        {
            LoadModels(@"c:\tmp\unpacking\gc-models\", @"c:\tmp\unpacking\gc-models\results.txt");
        }


        public void LoadModels(String sourceDirectory, String infoFile,int maxFiles = -1)
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
                            GCModel model = new GCModel(sourceFile.Name);
                            m_models.Add(model);

                            if (Common.FindCharsInStream(binReader, cntrTag,true))
                            {
                                int unk1 = binReader.ReadInt32();
                                int unk2 = binReader.ReadInt32();
                                int unk3 = binReader.ReadInt32();

                                model.Center = Common.FromStreamVector3BE(binReader);
                                int ibreak = 0;
                            }
                            if (Common.FindCharsInStream(binReader, posiTag))
                            {
                                int posSectionLength = binReader.ReadInt32();
                                int uk2 = binReader.ReadInt32();
                                int numPoints = binReader.ReadInt32();
                                for (int i = 0; i < numPoints; ++i)
                                {
                                    model.m_points.Add(Common.FromStreamVector3BE(binReader));
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
                                        model.m_normals.Add(Common.FromStreamVector3BE(binReader));
                                    }
                                }
                                model.BuildBB();
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

        public void DumpPoints(String infoFile)
        {
            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (GCModel model in m_models)
                {
                    infoStream.WriteLine(String.Format("File : {0} : {1} : {2}", model.m_name, model.m_points.Count, model.m_normals.Count));
                    infoStream.WriteLine("Verts : ");
                    foreach (Vector3 sv in model.m_points)
                    {
                        Common.WriteInt(infoStream, sv);
                    }
                    infoStream.WriteLine("Normals : ");
                    foreach (Vector3 sv in model.m_normals)
                    {
                        Common.WriteInt(infoStream, sv);
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