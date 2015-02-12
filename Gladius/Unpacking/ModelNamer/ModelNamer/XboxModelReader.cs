using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ModelNamer
{
    public class XboxModelReader : BaseModelReader
    {

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
                        if (sourceFile.Name != "File 005496")
                        {
                            // 410, 1939

                            //continue;
                        }

                        XboxModel model = LoadSingleModel(sourceFile.FullName) as XboxModel;
                        m_models.Add(model);

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

        public override BaseModel LoadSingleModel(String modelPath, bool readDisplayLists = true)
        {
            XboxModel model = null;
            FileInfo sourceFile = new FileInfo(modelPath);

            using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
            {
                model = new XboxModel(sourceFile.Name);
                //model.LoadModelTags(binReader, Common.xboxTags);
                model.LoadData(binReader);

            }
            return model;
        }



        static void Main(string[] args)
        {
            String rootPath = @"d:\gladius-extracted-archive\xbox-decompressed\";
            //rootPath = @"c:\tmp\gladius-extracted-archive\gladius-extracted-archive\xbox-decompressed\";
            String modelPath = rootPath + "ModelFilesRenamed";
            String infoFile = rootPath + "ModelInfo.txt";
            XboxModelReader reader = new XboxModelReader();
            String objOutputPath = rootPath + @"ModelFilesRenamed-Obj\";

            String texturePath = @"c:\tmp\gladius-extracted-archive\gladius-extracted-archive\gc-compressed\textures.jpg\";
            texturePath = @"C:\tmp\xbox-texture-output\";

            List<string> filenames = new List<string>();

            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "**"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*armor_all*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*carafe_decanter*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*animalsk*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\characters\", "*PropPracticePost1*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\characters\", "*amazon*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\arenas\", "*imperiafield*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*"));
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\axeCS_declamatio.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordM_gladius.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordCS_unofan.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\bow_amazon.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\bow_black.mdl");
            filenames.Add(rootPath + @"ModelFilesRenamed\armor_all.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\wheel.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arcane_water_crown.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\amazon.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\yeti.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\armband_base.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\carafe_decanter.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\carafe_carafe.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arenas\palaceibliis.mdl");
            foreach (string name in filenames)
            {
                reader.m_models.Add(reader.LoadSingleModel(name));
            }

            //using (StreamWriter infoSW = new StreamWriter(rootPath + "index-details.txt"))
            //{
            //    foreach (XboxModel model in reader.m_models)
            //    {
            //        using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + ".obj"))
            //        {
            //            infoSW.WriteLine(String.Format("[{0}][{1}] Found IS at [{2}] count [{3}] VS[{4}][{5}] count [{6}].", model.m_name, model.NumMeshes, model.IndexStart, model.CountedIndices, model.VertexStart, model.VertexForm, model.CountedVertices));

            //            using (StreamWriter matSw = new StreamWriter(objOutputPath + model.m_name + ".mtl"))
            //            {
            //                model.WriteOBJ(objSw, matSw, texturePath,32);
            //            }
            //        }
            //    }
            //}

            using (StreamWriter infoSW = new StreamWriter(rootPath + "submesh-details.txt"))
            {
                foreach (XboxModel model in reader.m_models)
                {
                    int adjc = model.m_textures.Count;
                    foreach (string tn in model.m_textureNames)
                    {
                        if (tn.Contains("skygold"))
                        {
                            adjc--;
                        }
                    }

                    if (adjc > 1 && model.m_avgVertex == 24)
                    {
                        int ibreak = 0;
                    }

                    //if (model.m_subMeshData1List.Count > 1 && model.m_avgVertex == 24)
                    //{
                    //    int ibreak = 0;
                    //}

                    infoSW.WriteLine("SubMesh1");
                    foreach (SubMeshData1 smd1 in model.m_subMeshData1List)
                    {
                        smd1.WriteInfo(infoSW);
                    }

                    infoSW.WriteLine("SubMesh2");
                    foreach (SubMeshData2 smd2 in model.m_subMeshData2List)
                    {
                        smd2.WriteInfo(infoSW);
                    }

                    foreach (TextureData td in model.m_textures)
                    {
                        infoSW.WriteLine(td.ToString());
                    }


                    foreach (SubMeshData2 smd2 in model.m_subMeshData2List)
                    {
                        infoSW.WriteLine("*************************************************************");
                        for(int i=smd2.MinVertex;i<smd2.MaxVertex;++i)
                        {
                            infoSW.WriteLine(model.m_allVertices[i].ToString());
                        }
                        //smd2.WriteInfo(infoSW);
                    }


                    //foreach (XboxVertexInstance vbi in model.m_allVertices)
                    //{
                    //    infoSW.WriteLine(vbi.ToString());
                    //}
                    int startIndex = 0;

                    //foreach (SubMeshData2 headerBlock in model.m_subMeshData2List)
                    //{
                    //    infoSW.WriteLine("*************************************************************");
                    //    try
                    //    {
                    //        int end = startIndex + headerBlock.NumIndices - 2;
                    //        for (int i = startIndex; i < end; i++)
                    //        {
                    //            int index1 = i;
                    //            int index2 = i + 1;
                    //            int index3 = i + 2;
                    //            if (index3 >= model.m_allIndices.Count)
                    //            {
                    //                index3 = index1;
                    //            }
                    //            if (index2 >= model.m_allIndices.Count)
                    //            {
                    //                index2 = index1;
                    //            }
                    //            if (i >= model.m_allIndices.Count)
                    //            {
                    //                int ibreak = 0;
                    //            }

                    //            int i1 = model.m_allIndices[index1];
                    //            int i2 = model.m_allIndices[index2];
                    //            int i3 = model.m_allIndices[index3];

                    //            // 1 based.
                    //            //i1 += 1;
                    //            //i2 += 1;
                    //            //i3 += 1;

                    //            // alternate winding
                    //            infoSW.WriteLine(model.m_allVertices[i]);
                    //            infoSW.WriteLine(model.m_allVertices[i2]);
                    //            infoSW.WriteLine(model.m_allVertices[i3]);
                    //        }
                    //        startIndex += headerBlock.NumIndices;

                    //    }
                    //    catch (Exception e)
                    //    {
                    //        int ibreak = 0;
                    //    }
                    //    //break;
                    //}

                }
            }

            foreach (XboxModel model in reader.m_models)
            {
                try
                {
                    //foreach (ShaderData sd in model.m_shaderData)
                    //{
                    //    shadernames.Add(sd.shaderName);
                    //}
                    ////model.DumpSections(tagOutputPath);
                    using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + ".obj"))
                    {
                        using (StreamWriter matSw = new StreamWriter(objOutputPath + model.m_name + ".mtl"))
                        {
                            model.WriteOBJ(objSw, matSw, texturePath, -1);
                        }
                    }
                }
                catch (System.Exception ex)
                {

                }
            }



        }

        List<BaseModel> m_models = new List<BaseModel>();
    }

    /*
     * Model Amazon
     * 
     * mesh1 : teeth
     * 
    */

    public class XboxModel : BaseModel
    {
        public List<XboxVertexInstance> m_allVertices = new List<XboxVertexInstance>();
        public List<ushort> m_allIndices = new List<ushort>();
        public List<SubMeshData1> m_subMeshData1List = new List<SubMeshData1>();
        public List<SubMeshData2> m_subMeshData2List = new List<SubMeshData2>();
        
        public int NumMeshes = 0;
        public int m_avgVertex;
        public XboxModel(String name)
            : base(name)
        {

        }

        public override void LoadData(BinaryReader binReader)
        {
            binReader.BaseStream.Position = 0;
            Common.ReadNullSeparatedNames(binReader, Common.nameTag, m_boneNames);
            binReader.BaseStream.Position = 0;
            ReadTextureSection(binReader);
            binReader.BaseStream.Position = 0;
            ReadSKELSection(binReader);
            binReader.BaseStream.Position = 0;
            ReadXRNDSection(binReader);
        }

        public void ReadXRNDSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.xrndTag))
            {
                int sectionLength = binReader.ReadInt32();
                int uk2a = binReader.ReadInt32();
                int numEntries = binReader.ReadInt32();
                int numSkip = binReader.ReadInt32();
                binReader.BaseStream.Position += numSkip * 4;
                //int uk2c = binReader.ReadInt32();

                //int uk2d = binReader.ReadInt32();
                byte[] doegStart = binReader.ReadBytes(4);
                Debug.Assert(doegStart[0] == 'd' && doegStart[3] == 'g');
                if (doegStart[0] == 'd' && doegStart[3] == 'g')
                {
                    int doegLength = binReader.ReadInt32();
                    Debug.Assert(doegLength == 0x7c);
                    int unk1a = binReader.ReadInt32();
                    Debug.Assert(unk1a == 0x01);
                    int unk1b = binReader.ReadInt32();
                    int unk1c = binReader.ReadInt32();
                    int unk1d = binReader.ReadInt32();
                    Debug.Assert(unk1d == 0x01);

                    NumMeshes = binReader.ReadInt32();
                    int unk1e = binReader.ReadInt32();
                    int numMeshCopy = binReader.ReadInt32();
                    Debug.Assert(NumMeshes == numMeshCopy);
                    int numTextures = binReader.ReadInt32();
                    int unk1g = binReader.ReadInt32();
                    int unk1h = binReader.ReadInt32();
                    Debug.Assert(unk1h == 0x00);

                                        
                    int blockStart1 = binReader.ReadInt32();
                    Debug.Assert(blockStart1 == 0x70);
                    int blockStart2 = binReader.ReadInt32();
                    Debug.Assert(blockStart2 - blockStart1 == (NumMeshes * 4));
                    int blockStart3 = binReader.ReadInt32();
                    Debug.Assert(blockStart3 - blockStart2 == (NumMeshes * 56));
                    int blockStart4 = binReader.ReadInt32();
                    Debug.Assert(blockStart4 - blockStart3 == (NumMeshes * 20));
                    int unk1m = binReader.ReadInt32();

                    // if this is not -1 then it's some information on skinning/anims?
                    int skinIndicator = binReader.ReadInt32();


                    int unk1o = binReader.ReadInt32();
                    int unk1p = binReader.ReadInt32();
                    int unk1q = binReader.ReadInt32();

                    int minus1a = binReader.ReadInt32();
                    int minus1b = binReader.ReadInt32();
                    int minus1c = binReader.ReadInt32();
                    int minus1d = binReader.ReadInt32();

                    Debug.Assert(minus1a == -1 && minus1b == -1 && minus1c == -1 && minus1d == -1);

                    int unk1r = binReader.ReadInt32();
                    int unk1s = binReader.ReadInt32();
                    int unk1t = binReader.ReadInt32();
                    int unk1u = binReader.ReadInt32();
                    
                    int minus1e = binReader.ReadInt32();

                    Debug.Assert(minus1a == -1 && minus1b == -1 && minus1c == -1 && minus1d == -1 && minus1e == -1);

                    // this is the number of bytes from here to the beginning of the texture names if you include the end doeg section (4)

                    int doegToTextureSize = binReader.ReadInt32();

                    byte[] doegEnd = binReader.ReadBytes(4);
                    Debug.Assert(doegEnd[0] == 'd' && doegEnd[3] == 'g');
                    int doegEndVal = (int)(binReader.BaseStream.Position-4);

                    binReader.BaseStream.Position = doegEndVal + blockStart1;

                    List<int> blockOneValues = new List<int>();
                    for(int i=0;i<NumMeshes;++i)
                    {
                        blockOneValues.Add(binReader.ReadInt32());
                    }


                    for(int i=0;i<NumMeshes;++i)
                    {
                        SubMeshData1 smd = SubMeshData1.FromStream(binReader);
                        m_subMeshData1List.Add(smd);
                    }

                    int TotalIndices = 0;
                    

                    //NumMeshes += 1;

                    for (int i = 0; i < NumMeshes; ++i)
                    {
                        SubMeshData2 smd = SubMeshData2.FromStream(binReader);
                        m_subMeshData2List.Add(smd);
                        TotalIndices += smd.NumIndices;
                        int val1a = smd.NumIndices * 2;
                        smd.pad = val1a % 4;
                    }

                    binReader.BaseStream.Position += 4;
                    int TotalVertices = binReader.ReadInt32();
                    
                    // do stuff...
                    SubMeshData3 smd3 = SubMeshData3.FromStream(binReader, NumMeshes);
                    
                    
                    
                    //binReader.BaseStream.Position = doegEndVal + doegToTextureSize;

                    Common.ReadNullSeparatedNames(binReader, binReader.BaseStream.Position, numTextures, m_textureNames);

                    foreach(SubMeshData2 smd in m_subMeshData2List)
                    {
                        for (int i = 0; i < smd.NumIndices; ++i)
                        {
                            ushort val = binReader.ReadUInt16();
                            smd.indices.Add(val);
                        }
                        m_allIndices.AddRange(smd.indices);
                        smd.padBytes = binReader.ReadBytes(smd.pad);
                        smd.BuildMinMax();
                    }

                    long testPosition = binReader.BaseStream.Position;

                    if (m_skinned)
                    {
                        ReadSkinnedVertexData28(binReader, m_allVertices, TotalVertices);
                        //ReadSkinnedVertexData32(binReader, m_allVertices, TotalVertices);

                        int maxWeight = -1; 
                        foreach (XboxVertexInstance vbi in m_allVertices)
                        {
                            vbi.Weights = binReader.ReadBytes(6);
                            //if (vbi.Weights[0] / 3 >= m_bones.Count)
                            //{
                            //    int ibreak = 0;
                            //}
                            //if (vbi.Weights[2] / 3 >= m_bones.Count)
                            //{
                            //    int ibreak = 0;
                            //}

                            if (vbi.Weights[0] != 255 && vbi.Weights[0] % 3 !=0)
                            {
                                int ibreak = 0;
                            }

                            if (vbi.Weights[2] != 255 && vbi.Weights[2] % 3 != 0)
                            {
                                int ibreak = 0;
                            }

                            if (vbi.Weights[0] != 255)
                            {
                                maxWeight = Math.Max(vbi.Weights[0] / 3, maxWeight);
                            }
                            if (vbi.Weights[2] != 255)
                            {
                                maxWeight = Math.Max(vbi.Weights[2] / 3, maxWeight);
                            }
                            if (vbi.Weights[4] != 255)
                            {
                                maxWeight = Math.Max(vbi.Weights[4] / 3, maxWeight);
                            }


                            //if (vbi.Weights[0] / 3 >= m_bones.Count)
                            //{
                            //    int ibreak = 0;
                            //}

                        }
                        int ibreak2 = 0;
                    }
                    else
                    {

                        if (Common.FindCharsInStream(binReader, Common.endTag))
                        {
                            m_avgVertex = ((int)binReader.BaseStream.Position - 4 - (int)testPosition) / TotalVertices;
                            binReader.BaseStream.Position = testPosition;

                            //ReadUnskinnedVertexData36(binReader, m_allVertices, TotalVertices);
                            switch (m_avgVertex)
                            {
                                case 24:
                                    ReadUnskinnedVertexData24(binReader, m_allVertices, TotalVertices);
                                    break;
                                case 28:
                                    ReadUnskinnedVertexData28(binReader, m_allVertices, TotalVertices);
                                    break;
                                case 36:
                                    ReadUnskinnedVertexData36(binReader, m_allVertices, TotalVertices);
                                    break;
                                default:
                                    int ibreak = 0;
                                    break;
                            }
                        }
                    }
                    BuildBB();

                    for (int i = 0; i < m_textureNames.Count; ++i)
                    {
                        ShaderData shaderData = new ShaderData();
                        shaderData.shaderName = "test";
                        shaderData.textureId1 = (byte)i;
                        shaderData.textureId2 = (byte)(i + 1);
                        m_shaderData.Add(shaderData);
                    }
                }
                else
                {
                    //infoStream.WriteLine("Doeg not at expected - multi file? : " + sourceFile.FullName);
                }

            }

        }

        public bool IsEnd(BinaryReader binReader)
        {
            byte[] endBlock = binReader.ReadBytes(4);
            char[] endBlockChar = new char[endBlock.Length];
            for (int i = 0; i < endBlock.Length; ++i)
            {
                endBlockChar[i] = (char)endBlock[i];
            }
            if (endBlockChar[0] == 'E' && endBlockChar[1] == 'N' && endBlock[2] == 'D')
            {
                return true;
            }
            return false;
        }

        public bool ValidVertex(Vector3 v)
        {
            return Math.Abs(v.X) < 10000 && Math.Abs(v.Y) < 10000 && Math.Abs(v.Z) < 10000;
        }

        public void WriteOBJ(StreamWriter writer, StreamWriter materialWriter, String texturePath, int desiredLod = -1)
        {
            int vertexCountOffset = 0;
            int normalCountOffset = 0;
            int uvCountOffset = 0;

            //String materialName = null;

            string reflectname = "skygold_R.tga";
            // write material?
            if (m_textures.Count == 2 && (m_textures[0].textureName.Contains(reflectname) || m_textures[1].textureName.Contains(reflectname)))
            {
                int notsgindex = m_textures[0].textureName.Contains(reflectname) ? 1 : 0;

                String textureName = m_textures[notsgindex].textureName + ".png";
                materialWriter.WriteLine("newmtl " + textureName);
                materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                materialWriter.WriteLine("d 1.0");
                materialWriter.WriteLine("illum 3");
                materialWriter.WriteLine("map_Ka " + texturePath + textureName);
                materialWriter.WriteLine("map_Kd " + texturePath + textureName);

                materialWriter.WriteLine("refl -type sphere -mm 0 1 " + texturePath + reflectname + ".png");
            }
            else
            {

                foreach (TextureData textureData in m_textures)
                {
                    String textureName = textureData.textureName + ".png";
                    materialWriter.WriteLine("newmtl " + textureName);
                    materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                    materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                    materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                    materialWriter.WriteLine("d 1.0");
                    materialWriter.WriteLine("illum 2");
                    materialWriter.WriteLine("map_Ka " + texturePath + textureName);
                    materialWriter.WriteLine("map_Kd " + texturePath + textureName);

                    //materialWriter.WriteLine("refl -type sphere -mm 0 1 clouds.mpc");
                }
            }
            writer.WriteLine("mtllib " + m_name + ".mtl");
            int submeshCount = 0;

            //writer.WriteLine("g allObjects");

            foreach (XboxVertexInstance vpnt in m_allVertices)
            {
                writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z));
            }

            foreach (XboxVertexInstance vpnt in m_allVertices)
            {
                writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000} ", vpnt.UV.X, 1.0f - vpnt.UV.Y));
            }

            foreach (XboxVertexInstance vpnt in m_allVertices)
            {
                writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Normal.X, vpnt.Normal.Y, vpnt.Normal.Z));
            }

            if (m_skinned)
            {
                foreach (XboxVertexInstance vpnt in m_allVertices)
                {
                    writer.WriteLine(String.Format("# weights BI2[{0}] [{1}][{2}][{3}][{4}][{5}][{6}]", vpnt.BoneInfo2, vpnt.Weights[0], vpnt.Weights[1], vpnt.Weights[2], vpnt.Weights[3], vpnt.Weights[4], vpnt.Weights[5]));
                }
            }


            int startIndex = 0;

            for(int a=0;a<m_subMeshData2List.Count;++a)
            {
                SubMeshData2 headerBlock = m_subMeshData2List[a];
                SubMeshData1 data1 = m_subMeshData1List[a];

                try
                {
                    if(a != 3)
                    {
                        //continue;
                    }

                    submeshCount++;

                    //if (data1.LodLevel != 0 && ((data1.LodLevel & desiredLod) == 0))
                    //{
                    //    continue;
                    //}


                    if (submeshCount == m_subMeshData2List.Count)
                    {
                        //continue;
                    }

                    string groupName = String.Format("{0}-submesh{1}-LOD{2}", m_name, submeshCount, data1.LodLevel);

                    writer.WriteLine("o " + groupName);

                    //ShaderData shaderData = m_shaderData[headerBlock.MeshId];
                    //ShaderData shaderData = m_shaderData[0];
                    //ShaderData shaderData = m_shaderData[data1.pad5b];
                    ShaderData shaderData = m_shaderData[0];
                    string adjustedTexture = FindTextureName(shaderData);
                    String materialName = adjustedTexture + ".png";

                    writer.WriteLine("usemtl " + materialName);
                    bool swap = false;
                    //for (int i = 0; i < headerBlock.Indices.Count - 2; i++)
                    //int start = headerBlock.StartOffset / 2;
                    int end = startIndex + headerBlock.NumIndices - 2;
                    for (int i = startIndex; i < end; i++)
                    {
                        int index1 = i;
                        int index2 = i + 1;
                        int index3 = i + 2;
                        if (index3 >= m_allIndices.Count)
                        {
                            index3 = index1;
                        }
                        if (index2 >= m_allIndices.Count)
                        {
                            index2=index1;
                        }
                        if (i  >= m_allIndices.Count)
                        {
                            int ibreak = 0;
                        }

                        int i1 = m_allIndices[index1];
                        int i2 = m_allIndices[index2];
                        int i3 = m_allIndices[index3];

                        // 1 based.
                        i1 += 1;
                        i2 += 1;
                        i3 += 1;

                        // alternate winding
                        if (swap)
                        {
                            writer.WriteLine(String.Format("f {0}/{1}/{2}  {3}/{4}/{5} {6}/{7}/{8}", i3, i3, i3, i2, i2, i2, i1, i1, i1));
                        }
                        else
                        {
                            writer.WriteLine(String.Format("f {0}/{1}/{2}  {3}/{4}/{5} {6}/{7}/{8}", i1, i1, i1, i2, i2, i2, i3, i3, i3));
                        }
                        swap = !swap;
                    }
                    startIndex += headerBlock.NumIndices;

                }
                catch (Exception e)
                {
                    int ibreak = 0;
                }
                //break;
            }
        }

        public void ReadUnskinnedVertexData24(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            //while(true)
            {
                try
                {
                    if (IsEnd(binReader))
                    {
                        break;
                    }
                    else
                    {
                        binReader.BaseStream.Position -= 4;
                    }

                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    int normal = binReader.ReadInt32();
                    Vector3 normV = UncompressNormal(normal);
                    Vector2 u = Common.FromStreamVector2(binReader);
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV= u;
                    vpnt.Normal = normV;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }


        public void ReadUnskinnedVertexData28(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            //while(true)
            {
                try
                {
                    if (IsEnd(binReader))
                    {
                        break;
                    }
                    else
                    {
                        binReader.BaseStream.Position -= 4;
                    }

                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    Vector3 normV = UncompressNormal(binReader.ReadInt32());
                    //Vector3 normU = UncompressNormal(binReader.ReadInt32());
                    int unk1 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
                    vpnt.Normal = normV;
                    vpnt.ExtraData = -1;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }

        public void ReadUnskinnedVertexData36(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            //while(true)
            {
                try
                {
                    if (IsEnd(binReader))
                    {
                        break;
                    }
                    else
                    {
                        binReader.BaseStream.Position -= 4;
                    }

                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    Vector3 normV = UncompressNormal(binReader.ReadInt32());
                    int unk1 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    Vector2 u2 = Common.FromStreamVector2(binReader);
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
                    vpnt.UV2 = u2;
                    vpnt.Normal = normV;
                    vpnt.ExtraData = unk1;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }


        public void ReadSkinnedVertexData28(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            {
                try
                {
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = Common.FromStreamVector3(binReader);
                    //vpnt.BoneInfo1 = binReader.ReadInt32();
                    vpnt.Normal = UncompressNormal(binReader.ReadInt32());
                    //int unk1 = binReader.ReadInt32();
                    vpnt.UV = Common.FromStreamVector2(binReader);
                    vpnt.BoneInfo2 = binReader.ReadInt32();

                    if (vpnt.BoneInfo2 != 255)
                    {
                        int ibreak = 0;
                    }

                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }

        public void ReadSkinnedVertexData32(BinaryReader binReader, List<XboxVertexInstance> allVertices, int numVertices)
        {
            for (int i = 0; i < numVertices; ++i)
            {
                try
                {
                    // 24 bytes per entry? , or 28...
                    Vector3 p = Common.FromStreamVector3(binReader);
                    int boneInfo1 = binReader.ReadInt32();
                    int boneInfo2 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    int boneInfo3 = binReader.ReadInt32();
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
                    //vpnt.Normal = normV;
                    //vpnt.ExtraData = unk1;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
            int ibreak2 = 0;
        }



        // taken from old sol code and it seems to work. wow!
        public Vector3 UncompressNormal(int cv)
        {
            Vector3 v = new Vector3();
            int x = ((int)(cv & 0x7ff) << 21) >> 21,
                    y = ((int)(cv & 0x3ffe00) << 10) >> 21,
                    z = (int)(cv & 0xffc00000) >> 22;

            v.X = ((float)x) / (float)((1 << 10) - 1);
            v.Y = ((float)y) / (float)((1 << 10) - 1);
            v.Z = ((float)z) / (float)((1 << 9) - 1);
            return v;
        }




        public override void BuildBB()
        {
            MinBB = new Vector3(float.MaxValue);
            MaxBB = new Vector3(float.MinValue);
            foreach (ModelSubMesh subMesh in m_modelMeshes)
            {
                MinBB = Vector3.Min(MinBB, subMesh.MinBB);
                MaxBB = Vector3.Max(MaxBB, subMesh.MaxBB);
            }
        }

        public void ReadSKELSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.skelTag))
            {
                m_hasSkeleton = true;
                m_skinned = true;
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numBones = (blockSize - 16) / 32;

                for (int i = 0; i < numBones; ++i)
                {
                    BoneNode node = BoneNode.FromStream(binReader);
                    node.name = m_boneNames[i];
                    m_bones.Add(node);
                }
                ConstructSkeleton();
            }

        }


        void CalcBindFinalMatrix(BoneNode bone, Matrix parentMatrix)
        {
            bone.combinedMatrix = bone.localMatrix * parentMatrix;
            //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
            bone.finalMatrix = bone.combinedMatrix;

            foreach (BoneNode child in bone.children)
            {
                CalcBindFinalMatrix(child, bone.combinedMatrix);
            }
        }

        //public List<VertexPositionNormalTexture> m_points = new List<VertexPositionNormalTexture>();

        public void WriteFBXAHeader(StreamWriter writer)
        {
            // write header.
            writer.WriteLine("; FBX 6.1.0 project file");
            writer.WriteLine("; Created by Blender FBX Exporter");
            writer.WriteLine("; for support mail: ideasman42@gmail.com");
            writer.WriteLine("; ----------------------------------------------------");
            writer.WriteLine("");
            writer.WriteLine("FBXHeaderExtension:  {");
            writer.WriteLine("FBXHeaderVersion: 1003");
            writer.WriteLine("FBXVersion: 6100");
            writer.WriteLine("Creator: \"FBX SDK/FBX Plugins build 20070228\"");
            writer.WriteLine("OtherFlags:  {");
            writer.WriteLine("FlagPLE: 0");
            writer.WriteLine("}");
            writer.WriteLine("}");
            writer.WriteLine("CreationTime: \"2014-03-20 17:38:29:000\"");
            writer.WriteLine("Creator: \"Blender version 2.69 (sub 10)\"");

            writer.WriteLine("; Object definitions");
            writer.WriteLine(";------------------------------------------------------------------");

            writer.WriteLine("Definitions:  {");
            writer.WriteLine("Version: 100");
            writer.WriteLine("Count: 3");
            writer.WriteLine("ObjectType: \"Model\" {");
            writer.WriteLine("Count: 1");
            writer.WriteLine("}");
            writer.WriteLine("ObjectType: \"Geometry\" {");
            writer.WriteLine("Count: 1");
            writer.WriteLine("}");
            writer.WriteLine("	ObjectType: \"Material\" {");
            writer.WriteLine("Count: 1");
            writer.WriteLine("}");
            writer.WriteLine("	ObjectType: \"Pose\" {");
            writer.WriteLine("		Count: 1");
            writer.WriteLine("	}");
            writer.WriteLine("	ObjectType: \"GlobalSettings\" {");
            writer.WriteLine("		Count: 1");
            writer.WriteLine("	}");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("; Object properties");
            writer.WriteLine(";------------------------------------------------------------------");

        }

        public void WriteFBXA(StreamWriter writer, StreamWriter materialWriter, String texturePath, int desiredLod = -1)
        {
            WriteFBXAHeader(writer);
            writer.WriteLine("Objects:  {");
            writer.WriteLine("Model: \"" + m_name + "\", \"Mesh\" {");
            writer.WriteLine("Version: 232");
        }



        public void WriteVertices(StreamWriter writer)
        {
            // write vertices
            writer.WriteLine("Vertices:");
            for (int i = 0; i < m_allVertices.Count; ++i)
            {
                XboxVertexInstance vpnt = m_allVertices[i];
                writer.WriteLine(String.Format("{0:0.00000},{1:0.00000},{2:0.00000}", vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z));
                if (i < m_allVertices.Count - 1)
                {
                    writer.WriteLine(",");
                }
            }

        }

        public void WriteNormals(StreamWriter writer)
        {
            writer.WriteLine("LayerElementNormal: 0 {");
            writer.WriteLine("Version: 101");
            writer.WriteLine("Name: \"\"");
            writer.WriteLine("MappingInformationType: \"ByVertex\"");
            writer.WriteLine("ReferenceInformationType: \"Direct\"");
            writer.WriteLine("Normals:");
            for (int i = 0; i < m_allVertices.Count; ++i)
            {
                XboxVertexInstance vpnt = m_allVertices[i];
                writer.WriteLine(String.Format("{0:0.00000},{1:0.00000},{2:0.00000}", vpnt.Normal.X, vpnt.Normal.Y, vpnt.Normal.Z));
                if (i < m_allVertices.Count - 1)
                {
                    writer.WriteLine(",");
                }
            }
            writer.WriteLine("}");
        }
        public void WriteUVs(StreamWriter writer)
        {
            writer.WriteLine("LayerElementUV: 0 {");
            writer.WriteLine("Version: 101");
            writer.WriteLine("Name: \"\"");
            writer.WriteLine("MappingInformationType: \"ByVertex\"");
            writer.WriteLine("ReferenceInformationType: \"Direct\"");
            writer.WriteLine("Normals:");
            for (int i = 0; i < m_allVertices.Count; ++i)
            {
                XboxVertexInstance vpnt = m_allVertices[i];
                writer.WriteLine(String.Format("{0:0.00000},{1:0.00000}", vpnt.UV.X, vpnt.UV.Y));
                if (i < m_allVertices.Count - 1)
                {
                    writer.WriteLine(",");
                }
            }
            writer.WriteLine("}");
        }

        public int GenerateNodeId()
        {
            return 1;
        }

        public void WriteSkeleton(StreamWriter writer)
        {
            writer.WriteLine("; Object properties");
            writer.WriteLine(";------------------------------------------------------------------");

            writer.WriteLine("Objects: {");

            foreach(BoneNode boneNode in m_bones)
            {
                boneNode.fbxNodeId = GenerateNodeId();
                writer.WriteLine(String.Format("NodeAttribute: {0}, \"NodeAttribute::{1}\", \"LimbNode\" {",boneNode.fbxNodeId,boneNode.name));
                writer.WriteLine("Properties70: {");
                writer.WriteLine(String.Format("P: \"Size\", \"double\", \"Number\",\"\",{0}",1.0f));
                writer.WriteLine("}");
                writer.WriteLine("TypeFlags: \"Skeleton\"");
                writer.WriteLine("}");
            }
            writer.WriteLine("}");


            writer.WriteLine("; Object connections");
            writer.WriteLine(";------------------------------------------------------------------");

            writer.WriteLine("Connections: {");

            foreach(BoneNode boneNode in m_bones)
            {
                foreach (BoneNode childNode in boneNode.children)
                {
                    writer.WriteLine(String.Format(";  {0}::{1}", boneNode.name, childNode.name));
                    writer.WriteLine(String.Format("C: \"OO\",{0},{1}", boneNode.fbxNodeId, childNode.fbxNodeId));
                    writer.WriteLine();
                }
            }

            writer.WriteLine("}");

        }
    }

    



    public class SubMeshData1
    {
        public int minus1;
        public int zero1;
        public int zero2;
        public int counter1;
        //byte[] name1;
        //byte f1a;
        //byte f1b;
        //byte f1c;
        //byte f1d;

        //float f1;
        //float f2;
        //float f3;
        //float f4;

        int f1;
        int f2;
        int f3;
        int f4;
        //byte[] usefulBytes;
        //byte[] padBytes;

        public int pad1;
        public int pad2;
        public int pad3;
        public int pad4;
        public int pad5;
        //public byte pad5a;
        //public byte pad5b;
        //public byte pad5c;
        //public byte pad5d;
        public int LodLevel;
        public static HashSet<int> s_lodLevels = new HashSet<int>();


        public static SubMeshData1 FromStream(BinaryReader binReader)
        {
            SubMeshData1 smd = new SubMeshData1();
            smd.zero1 = binReader.ReadInt32();
            smd.zero2 = binReader.ReadInt32();
            smd.counter1 = binReader.ReadInt32();
            //smd.name1 = binReader.ReadBytes(16);
            //smd.f1 = binReader.ReadSingle();


            
            //smd.f1a = binReader.ReadByte();
            //smd.f1b = binReader.ReadByte();
            //smd.f1c = binReader.ReadByte();
            //smd.f1d = binReader.ReadByte();

            //smd.f1 = binReader.ReadSingle();
            //smd.f2 = binReader.ReadSingle();
            //smd.f3 = binReader.ReadSingle();
            //smd.f4 = binReader.ReadSingle();

            //smd.usefulBytes = binReader.ReadBytes(16);
            //smd.padBytes = binReader.ReadBytes(20);
            smd.f1 = binReader.ReadInt32();
            smd.f2 = binReader.ReadInt32();
            smd.f3 = binReader.ReadInt32();
            smd.f4 = binReader.ReadInt32();


            smd.pad1 = binReader.ReadInt32();
            smd.pad2 = binReader.ReadInt32();
            smd.pad3 = binReader.ReadInt32();
            smd.pad4 = binReader.ReadInt32();
            smd.pad5 = binReader.ReadInt32();
            //smd.pad5a = binReader.ReadByte();
            //smd.pad5b = binReader.ReadByte();
            //smd.pad5c = binReader.ReadByte();
            //smd.pad5d = binReader.ReadByte();
            smd.LodLevel = binReader.ReadInt32();
            smd.minus1 = binReader.ReadInt32();
            s_lodLevels.Add(smd.LodLevel);
            return smd;
        }

        public String PrintTidy(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                sb.Append("[" + data[i] + "]\t");
            }
            return sb.ToString();
        }


        public void WriteInfo(StreamWriter sw)
        {
            //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}][{7}][{8}][{9}][{10}] a[{11}] b[{12}] c[{13}] d[{14}] lod[{15}]", zero1, zero2, counter1, , pad1, pad2, pad3, pad4, pad5a, pad5b, pad5c, pad5d, LodLevel));
            sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}][{7}][{8}][{9}][{10}] a[{11}] lod[{12}]", zero1, zero2, counter1, f1,f2,f3,f4,pad1, pad2, pad3, pad4, pad5,LodLevel));
            //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}]", zero1, zero2, counter1,PrintTidy(usefulBytes),PrintTidy(padBytes),LodLevel));
            //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}]",f1a,f1b,f1c,f1d));
        }


    }

    public class SubMeshData2
    {   
        public float val1;
        public int StartOffset;
        public int val2;
        public int NumIndices;

        public int val3;
        public int pad;
        public byte[] padBytes;
        public List<ushort> indices = new List<ushort>();
        public int MinVertex = int.MaxValue;
        public int MaxVertex = int.MinValue;

        public static SubMeshData2 FromStream(BinaryReader binReader)
        {
            SubMeshData2 smd = new SubMeshData2();
            smd.val1 = binReader.ReadSingle();
            smd.StartOffset = binReader.ReadInt32();
            smd.val1 = binReader.ReadInt32();
            smd.NumIndices = binReader.ReadInt32();
            //smd.NumIndices *= 2;
            smd.val2 = binReader.ReadInt32();
            return smd;
        }
        public void WriteInfo(StreamWriter sw)
        {
            sw.WriteLine(String.Format("SO[{0}]NI[{1}]V1[{2}]V2[{3}]V3[{4}]", StartOffset,NumIndices,val1,val2,val3));
        }

        public void BuildMinMax()
        {
            for (int i = 0; i < indices.Count; ++i)
            {
                MinVertex = Math.Min(MinVertex, indices[i]);
                MaxVertex = Math.Max(MaxVertex, indices[i]);
            }

        }

    }

    public class SubMeshData3
    {
        public int val1;
        public int val2;
        public int val3;
        public int val4;
        public int val5;
        public int val6;
        public List<int> list1 = new List<int>();
        public List<int[]> list2 = new List<int[]>();
        public List<int[]> list3 = new List<int[]>();
        public float val7;
        public List<SubMeshData4> data4list = new List<SubMeshData4>();

        public int lastElementOffset;
        public int headerEnd2;
        public int headerEnd3;
        public int headerEnd4;
        public int headerEnd5;
        public float headerEnd6;
        public int[] headerEndZero = new int[8];
        public int[] headerEndZero2 = new int[3];

        public static SubMeshData3 FromStream(BinaryReader binReader, int numMeshes)
        {
            SubMeshData3 smd3 = new SubMeshData3();
            smd3.val1 = binReader.ReadInt32();
            smd3.val2 = binReader.ReadInt32();
            smd3.val3 = binReader.ReadInt32();
            smd3.val4 = binReader.ReadInt32();
            smd3.val5 = binReader.ReadInt32();
            smd3.val6 = binReader.ReadInt32();
            Debug.Assert(smd3.val2 == -1);
            Debug.Assert(smd3.val4 == -1);
            Debug.Assert(smd3.val6 == -1);
            for (int i = 0; i < numMeshes; ++i)
            {
                smd3.list1.Add(binReader.ReadInt32());
            }

            for (int i = 0; i < numMeshes; ++i)
            {
                int[] a = new int[3];
                for (int j = 0; j < a.Length; ++j)
                {
                    a[j] = binReader.ReadInt32();
                }
                smd3.list2.Add(a);
            }
            for (int i = 0; i < numMeshes; ++i)
            {
                int[] a = new int[11];
                for (int j = 0; j < a.Length; ++j)
                {
                    a[j] = binReader.ReadInt32();
                }
                smd3.list3.Add(a);
            }



            smd3.lastElementOffset = binReader.ReadInt32();
            smd3.headerEnd2 = binReader.ReadInt32();
            smd3.headerEnd3 = binReader.ReadInt32();
            smd3.headerEnd4 = binReader.ReadInt32();
            smd3.headerEnd5 = binReader.ReadInt32();
            smd3.headerEnd6 = binReader.ReadSingle();
            Debug.Assert(smd3.headerEnd6 == 1.0f);
            for (int i = 0; i < smd3.headerEndZero.Length; ++i)
            {
                smd3.headerEndZero[i] = binReader.ReadInt32();
                Debug.Assert(smd3.headerEndZero[i] == 0);
            }
            for(int i=0;i<numMeshes;++i)
            {
                int startVal = smd3.list3[i][5];
                int endVal = i < smd3.list3.Count-1? smd3.list3[i+1][5]:smd3.lastElementOffset;
                int sectionLength = endVal - startVal;

                smd3.data4list.Add(SubMeshData4.FromStream(binReader,numMeshes,sectionLength));
            }
            for (int i = 0; i < smd3.headerEndZero2.Length; ++i)
            {
                smd3.headerEndZero2[i] = binReader.ReadInt32();
                Debug.Assert(smd3.headerEndZero2[i] == 0);
            }

            return smd3;
        }

    }

    public class SubMeshData4
    {
        public float[] array0 = new float[4];

        public List<int> m_data = new List<int>();

        public static SubMeshData4 FromStream(BinaryReader binReader, int numMeshes,int sectionLength)
        {
            SubMeshData4 smd = new SubMeshData4();
            for(int i=0;i<smd.array0.Length;++i)
            {
                smd.array0[i] = binReader.ReadSingle();
                Debug.Assert(smd.array0[i] == 1.0f);
            }

            int offset = (smd.array0.Length * 4);
            int count = (sectionLength - offset) / 4;
            for (int i = 0; i < count; ++i)
            {
                smd.m_data.Add(binReader.ReadInt32());
            }


            return smd;
        }

    }



    public class XboxVertexInstance
    {


        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public Vector2 UV2;
        public int ExtraData;
        public byte[] Weights;
        public int BoneInfo1;
        public int BoneInfo2;
        public int BoneInfo3;

        // I use arithmetic here to show clearly where these numbers come from
        // You can just type 28 if you want
        //public static readonly int SizeInBytes = sizeof(float) * 11;

        //// Vertex Element array for our struct above
        //public static readonly VertexElement[] VertexElements = 
        //{
        //    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        //    new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 1),
        //    new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
        //    new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 3),
        //    new VertexElement(sizeof(float) * 10, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 4),
        //    new VertexElement(sizeof(float) * 11, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 5),
        //};

        //public static VertexDeclaration Declaration
        //{
        //    get { return new VertexDeclaration(VertexElements); }
        //}

        //VertexDeclaration IVertexType.VertexDeclaration
        //{
        //    get { return new VertexDeclaration(VertexElements); }
        //}

        public override string ToString()
        {
            return String.Format("P {0}\tN {1}\tUV {2}\tE {3}", Common.ToString(Position), Common.ToString(Normal), Common.ToString(UV), ExtraData);
        }
    }


}

