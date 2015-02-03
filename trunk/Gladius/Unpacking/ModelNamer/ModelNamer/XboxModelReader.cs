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

            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*carafe_dec*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*armor_all*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*animalsk*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed\arenas\", "*belfortgatenor*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*"));
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\axeCS_declamatio.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordM_gladius.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordCS_unofan.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\bow_amazon.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\armor_all.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\wheel.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arcane_water_crown.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\characters\amazon.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\charm_catseye.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\carafe_decanter.mdl");
            filenames.Add(rootPath + @"ModelFilesRenamed\arenas\palaceibliis.mdl");
            foreach (string name in filenames)
            {
                reader.m_models.Add(reader.LoadSingleModel(name));
            }

            using (StreamWriter infoSW = new StreamWriter(rootPath + "index-details.txt"))
            {
                foreach (XboxModel model in reader.m_models)
                {
                    using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + ".obj"))
                    {
                        infoSW.WriteLine(String.Format("[{0}][{1}] Found IS at [{2}] count [{3}] VS[{4}][{5}] count [{6}].",model.m_name,model.NumMeshes,model.IndexStart,model.CountedIndices,model.VertexStart,model.VertexForm,model.CountedVertices));

                        using (StreamWriter matSw = new StreamWriter(objOutputPath + model.m_name + ".mtl"))
                        {
                            model.WriteOBJ(objSw, matSw, texturePath);
                        }
                    }
                }
            }




            //foreach (XboxModel model in reader.m_models)
            //{
            //    try
            //    {
            //        //foreach (ShaderData sd in model.m_shaderData)
            //        //{
            //        //    shadernames.Add(sd.shaderName);
            //        //}
            //        ////model.DumpSections(tagOutputPath);
            //        using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + ".obj"))
            //        {
            //            using (StreamWriter matSw = new StreamWriter(objOutputPath + model.m_name + ".mtl"))
            //            {
            //                model.WriteOBJ(objSw, matSw, texturePath, 1);
            //            }
            //        }
            //    }
            //    catch (System.Exception ex)
            //    {

            //    }
            //}



        }

        List<BaseModel> m_models = new List<BaseModel>();
    }

    public class XboxModel : BaseModel
    {
        List<XboxVertexInstance> m_allVertices = new List<XboxVertexInstance>();
        List<ushort> m_allIndices = new List<ushort>();
        List<SubMeshData1> m_subMeshData1List = new List<SubMeshData1>();
        List<SubMeshData2> m_subMeshData2List = new List<SubMeshData2>();
        
        public int IndexStart = -1;
        public int CountedIndices = -1;
        public int VertexStart = -1;
        public int CountedVertices = -1;
        public int VertexForm = -1;
        public int NumMeshes = 0;

        public XboxModel(String name)
            : base(name)
        {

        }

        public override void LoadData(BinaryReader binReader)
        {
            binReader.BaseStream.Position = 0;
            Common.ReadNullSeparatedNames(binReader, Common.nameTag, m_names);
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

                    List<SubMeshData1> subMeshData1List = new List<SubMeshData1>();
                    for(int i=0;i<NumMeshes;++i)
                    {
                        SubMeshData1 smd = SubMeshData1.FromStream(binReader);
                        subMeshData1List.Add(smd);
                    }

                    int TotalIndices = 0;
                    

                    //NumMeshes += 1;
                    int maxVal5 = 0;
                    for (int i = 0; i < NumMeshes; ++i)
                    {
                        SubMeshData2 smd = SubMeshData2.FromStream(binReader);
                        m_subMeshData2List.Add(smd);
                        TotalIndices += smd.NumIndices;
                        int val1a = smd.NumIndices * 2;
                        smd.pad = val1a % 4;

                        //if (i > 1)
                        //{
                        //    int val1 = (m_subMeshData2List[i - 1].NumIndices *2);
                        //    //smd.pad = val1 % 4;
                        //    int diff = m_subMeshData2List[i - 1].StartOffset + val1 + smd.pad;
                        //    Debug.Assert(diff == smd.StartOffset);
                        //    int ibreak = 0;
                        //}

                        maxVal5 = Math.Max(smd.val5, maxVal5);

                    }



                    binReader.BaseStream.Position += 4;
                    int TotalVertices = binReader.ReadInt32();
                    binReader.BaseStream.Position = doegEndVal + doegToTextureSize;
                    
                    Common.ReadNullSeparatedNames(binReader,binReader.BaseStream.Position,numTextures,m_names);

                    int rem = TotalIndices % 3;
                    //TotalIndices += rem;

                    foreach(SubMeshData2 smd in m_subMeshData2List)
                    {
                        for (int i = 0; i < smd.NumIndices; ++i)
                        {
                            int val = binReader.ReadUInt16();
                            m_allIndices.Add((ushort)val);
                        }
                        binReader.BaseStream.Position += smd.pad;
                    }

                    //for(int i=0;i<TotalIndices;++i)
                    //{
                    //    int val = binReader.ReadUInt16();
                    //    //if (val >= TotalIndices)
                    //    //{
                    //    //    TotalIndices = Math.Max(TotalIndices,val);
                    //    //    TotalIndices++;
                    //    //}
                    //    m_allIndices.Add((ushort)val);
                    //}


                    // fixme - figure out why sometimes there is a gap here, maybe to do with num mesh count being wrong
                    // e.g. belfrort has either 334 or 333 meshes?

                    //int pad = ((int)binReader.BaseStream.Position - doegEndVal) % 4;
                    //int pad = TotalIndices % 16;
                    //pad = 0x176;
                    //pad = 2;
                    //binReader.BaseStream.Position += pad;
                    int fixedPos = 0x7aFa0;
                    int diff1 = fixedPos- (int)binReader.BaseStream.Position;
                    //binReader.BaseStream.Position = fixedPos;

                    long testPosition = binReader.BaseStream.Position;
                    int pad = (int)testPosition % 16;
                    pad = 8;
                    //testPosition += pad;
                    binReader.BaseStream.Position = testPosition;

                    ReadUnskinnedVertexData36(binReader,m_allVertices,TotalVertices);
                    bool valid = ValidVertex(m_allVertices[0].Position) && ValidVertex(m_allVertices[1].Position);
                    if(!valid)
                    {
                        binReader.BaseStream.Position = testPosition;
                        m_allVertices.Clear();
                        ReadUnskinnedVertexData28(binReader,m_allVertices,TotalVertices);
                        valid = ValidVertex(m_allVertices[0].Position) && ValidVertex(m_allVertices[1].Position);
                    }
                    if(!valid)
                    {
                        binReader.BaseStream.Position = testPosition;
                        m_allVertices.Clear();
                        ReadUnskinnedVertexData24(binReader,m_allVertices,TotalVertices);
                        valid = ValidVertex(m_allVertices[0].Position) && ValidVertex(m_allVertices[1].Position);
                    }

                    if (!valid)
                    {
                        int ibreak = 0;
                    }
                    
                    BuildBB();

                    ShaderData shaderData = new ShaderData();
                    shaderData.shaderName = "test";
                    shaderData.textureId1 = 0;
                    shaderData.textureId2 = 1;
                    m_shaderData.Add(shaderData);
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

            int startIndex = 0;

            foreach (SubMeshData2 headerBlock in m_subMeshData2List)
            {
                try
                {
                    submeshCount++;

                    if (submeshCount == m_subMeshData2List.Count)
                    {
                        //continue;
                    }

                    string groupName = String.Format("{0}-submesh{1}-LOD{2}", m_name, submeshCount, headerBlock.val1);

                    writer.WriteLine("o " + groupName);

                    //ShaderData shaderData = m_shaderData[headerBlock.MeshId];
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
                    int unk1 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    XboxVertexInstance vpnt = new XboxVertexInstance();
                    vpnt.Position = p;
                    vpnt.UV = u;
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


        public void ReadSkinnedVertexData(BinaryReader binReader, List<VertexPositionNormalTexture> allVertices, int numVertices)
        {
            //for (int i = 0; i < numVertices; ++i)
            while (true)
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

                    Vector3 p = Common.FromStreamVector3(binReader);
                    int skip3 = binReader.ReadInt32();
                    Vector2 u = Common.FromStreamVector2(binReader);
                    //Vector2 u2 = Common.FromStreamVector2(binReader);
                    int skip4 = binReader.ReadInt32();
                    VertexPositionNormalTexture vpnt = new VertexPositionNormalTexture();
                    vpnt.Position = p;
                    vpnt.TextureCoordinate = u;
                    //vpnt.TextureCoordinate = new Vector2();

                    if (float.IsNaN(u.X) || float.IsNaN(u.Y))
                    {
                        int ibreak = 0;
                    }

                    vpnt.Normal = Vector3.Up;
                    allVertices.Add(vpnt);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }

            // Now weight info.
            binReader.BaseStream.Position += (numVertices * 6);


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
                    node.name = m_names[i];
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


    }





    public class SubMeshData1
    {
        public int minus1;
        public int zero1;
        public int zero2;
        public int counter1;
        //byte[] name1;
        float f1;
        float f2;
        float f3;
        float f4;
        public int pad1;
        public int pad2;
        public int pad3;
        public int pad4;
        public int pad5;
        public int pad6;


        public static SubMeshData1 FromStream(BinaryReader binReader)
        {
            SubMeshData1 smd = new SubMeshData1();
            smd.zero1 = binReader.ReadInt32();
            smd.zero2 = binReader.ReadInt32();
            smd.counter1 = binReader.ReadInt32();
            //smd.name1 = binReader.ReadBytes(16);
            smd.f1 = binReader.ReadSingle();
            smd.f2 = binReader.ReadSingle();
            smd.f3 = binReader.ReadSingle();
            smd.f4 = binReader.ReadSingle();
            smd.pad1 = binReader.ReadInt32();
            smd.pad2 = binReader.ReadInt32();
            smd.pad3 = binReader.ReadInt32();
            smd.pad4 = binReader.ReadInt32();
            smd.pad5 = binReader.ReadInt32();
            smd.pad6 = binReader.ReadInt32();
            smd.minus1 = binReader.ReadInt32();

            return smd;
        }
    }

    public class SubMeshData2
    {   
        public float val1;
        public int StartOffset;
        public int val3;
        public int NumIndices;
        public int val5;
        public int pad;

        public static SubMeshData2 FromStream(BinaryReader binReader)
        {
            SubMeshData2 smd = new SubMeshData2();
            smd.val1 = binReader.ReadSingle();
            smd.StartOffset = binReader.ReadInt32();
            smd.val3 = binReader.ReadInt32();
            smd.NumIndices = binReader.ReadInt32();
            //smd.NumIndices *= 2;
            smd.val5 = binReader.ReadInt32();
            return smd;
        }
    }


    public struct XboxVertexInstance : IVertexType
    {

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public Vector2 UV2;
        public int ExtraData;
        

        // I use arithmetic here to show clearly where these numbers come from
        // You can just type 28 if you want
        public static readonly int SizeInBytes = sizeof(float) * 11;

        // Vertex Element array for our struct above
        public static readonly VertexElement[] VertexElements = 
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 1),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(sizeof(float) * 10, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 5),
        };

        public static VertexDeclaration Declaration
        {
            get { return new VertexDeclaration(VertexElements); }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return new VertexDeclaration(VertexElements); }
        }

        public override string ToString()
        {
            return String.Format("P {0} N {1} UV {2} E {3}", ToString(Position), ToString(Normal), ToString(UV), ExtraData);
        }

        public String ToString(Vector3 v3)
        {
            return String.Format("{0:0.00000000} {1:0.00000000} {2:0.00000000}", v3.X, v3.Y, v3.Z);
        }

        public String ToString(Vector2 v2)
        {
            return String.Format("{0:0.00000000} {1:0.00000000}", v2.X, v2.Y);
        }


    }


}

