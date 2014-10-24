﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
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
        static char[] endTag = new char[] { 'E', 'N', 'D', (char)0x2E };

        // NAME and SKEL always exist together
        // STYP can appear with them or without them, or not at all.



        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };

        static char[][] allTags = { versTag, cprtTag, selsTag, txtrTag, stypTag, nameTag, skelTag, xrndTag };


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

                                //continue;
                            }


                            XboxModel model = new XboxModel(sourceFile.Name);


                            m_models.Add(model);
                            if (Common.FindCharsInStream(binReader, versTag))
                            {
                                int sectionLength = binReader.ReadInt32();
                                int uk2a = binReader.ReadInt32();
                                int numEntries = binReader.ReadInt32();
                            }
                            if (Common.FindCharsInStream(binReader, selsTag))
                            {
                                int sectionLength = binReader.ReadInt32();
                                int uk2a = binReader.ReadInt32();
                                int numEntries = binReader.ReadInt32();
                            }
                            if (Common.FindCharsInStream(binReader, txtrTag))
                            {

                            }
                            binReader.BaseStream.Position = 0;

                            if (Common.FindCharsInStream(binReader, xrndTag))
                            {
                                int sectionLength = binReader.ReadInt32();
                                int uk2a = binReader.ReadInt32();
                                int numEntries = binReader.ReadInt32();
                                int uk2b = binReader.ReadInt32();
                                int uk2c = binReader.ReadInt32();
                                //int uk2d = binReader.ReadInt32();
                                byte[] doegStart = binReader.ReadBytes(4);
                                if(doegStart[0] == 'd' && doegStart[3] == 'g')
                                {
                                    Debug.Assert(doegStart[0] == 'd' && doegStart[3] == 'g');
                                    int doegLength = binReader.ReadInt32();
                                    byte[] stuff = binReader.ReadBytes(doegLength - 8);
                                    byte[] doegEnd = binReader.ReadBytes(4);
                                    Debug.Assert(doegEnd[0] == 'd' && doegEnd[3] == 'g');
                                    binReader.BaseStream.Position += 0xB0;
                                    //int numVertices = binReader.ReadInt32();
                                    short numIndices = binReader.ReadInt16();

                                    // block of 0x214 to next section?
                                    //binReader.BaseStream.Position += 0x214;
                                    //List<String> textureNames = new List<string>();
                                    //Common.ReadNullSeparatedNames(binReader, textureNames);
                                    //long currentPosition = binReader.BaseStream.Position;
                                    //Debug.Assert(Common.FindCharsInStream(binReader, endTag));
                                    //long endPosition = binReader.BaseStream.Position;

                                    //long remainder = endPosition - 4 - currentPosition;
                                    //long sum = (numIndices*2) + (numVertices*(4*3));
                                    //long diff = remainder - sum;

                                    //infoStream.WriteLine(String.Format("[{0}]  I[{1}] V[{2}] R[{3}] S[{4}] D[{5}]",
                                    //    sourceFile.FullName, numIndices,
                                    //    numVertices, remainder,sum, diff));

                                    infoStream.WriteLine(String.Format("[{0}]  I[{1}]",sourceFile.FullName, numIndices));

                                    // then follows an int16 index buffer? not sure how many entries.
                                    //List<ushort> indices = new List<ushort>();

                                    //for (int i = 0; i < numIndices; ++i)
                                    //{
                                    //    indices.Add(binReader.ReadUInt16());
                                    //}

                                    //List<Vector3> vertices = new List<Vector3>();

                                    //for (int i = 0; i < numVertices; ++i)
                                    //{
                                    //    Vector3 v = Common.FromStreamVector3BE(binReader);
                                    //    vertices.Add(v);
                                    //}
                                }
                            else
                                {
                                    infoStream.WriteLine("Doeg not at expected - multi file? : " + sourceFile.FullName);
                                }

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

        public void ReadTextureSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.txtrTag))
            {
                int blocksize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int numElements = binReader.ReadInt32();
                for (int i = 0; i < numElements; ++i)
                {
                    TextureData textureData = TextureData.FromStream(binReader);

                    //m_textures.Add(textureData);
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
            String modelPath = @"C:\tmp\unpacking\xbox-ModelFiles\RenamedModelFiles";
            String infoFile = @"c:\tmp\unpacking\xbox-ModelFiles\model-reader-results.txt";
            String sectionInfoFile = @"C:\tmp\unpacking\xbox-ModelFiles\index-normal-data.txt";
            XboxModelReader reader = new XboxModelReader();
            reader.LoadModels(modelPath, infoFile);
            //reader.DumpPoints(infoFile);
            //reader.DumpSectionLengths(modelPath, sectionInfoFile);



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

