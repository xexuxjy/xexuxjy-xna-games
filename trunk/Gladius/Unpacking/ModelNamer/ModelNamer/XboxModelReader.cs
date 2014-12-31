using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

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
                model.LoadModelTags(binReader, Common.xboxTags);
                model.LoadData(binReader);

            }
            return model;
        }



        static void Main(string[] args)
        {
            String rootPath = @"c:\tmp\gladius-extracted-archive\gladius-extracted-archive\xbox-decompressed\";
            String modelPath = rootPath+"ModelFilesRenamed";
            String infoFile = rootPath+"ModelInfo.txt";
            XboxModelReader reader = new XboxModelReader();
            String objOutputPath = rootPath+@"ModelFilesRenamed-Obj\";

            String texturePath = @"c:\tmp\gladius-extracted-archive\gladius-extracted-archive\gc-compressed\textures.jpg\";


            List<string> filenames = new List<string>();


            //filenames.AddRange(Directory.GetFiles(rootPath+@"ModelFilesRenamed\weapons", "club*"));
            //filenames.AddRange(Directory.GetFiles(rootPath + @"ModelFilesRenamed", "*"));
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\axeCS_declamatio.mdl");
            filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordM_gladius.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordCS_unofan.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\bow_amazon.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\armor_all.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\wheel.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arcane_water_crown.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\charm_catseye.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\carafe_decanter.mdl");
            //filenames.Add(rootPath + @"ModelFilesRenamed\arenas\palaceibliis.mdl");
            foreach (string name in filenames)
            {
                reader.m_models.Add(reader.LoadSingleModel(name));
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
                            model.WriteOBJ(objSw, matSw, texturePath, 1);
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

    public class XboxModel : BaseModel
    {
        List<VertexPositionNormalTexture> m_allVertices = new List<VertexPositionNormalTexture>();
        List<ushort> m_allIndices = new List<ushort>();



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
                int uk2b = binReader.ReadInt32();
                int uk2c = binReader.ReadInt32();
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

                    m_numMeshes = binReader.ReadInt32();
                    int unk1e = binReader.ReadInt32();
                    int numMeshCopy = binReader.ReadInt32();
                    Debug.Assert(m_numMeshes == numMeshCopy);
                    int numTextures = binReader.ReadInt32();
                    int unk1g = binReader.ReadInt32();
                    int unk1h = binReader.ReadInt32();
                    Debug.Assert(unk1h == 0x00);
                    int unk1i = binReader.ReadInt32();
                    Debug.Assert(unk1i == 0x70);
                    int unk1j = binReader.ReadInt32();
                    int unk1k = binReader.ReadInt32();
                    int unk1l = binReader.ReadInt32();
                    int unk1m = binReader.ReadInt32();
                    
                    // if this is not -1 then it's some information on skinning/anims?
                    int unk1n = binReader.ReadInt32();


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
                    //binReader.BaseStream.Position += 22 * 4;
                    int minus1e = binReader.ReadInt32();

                    Debug.Assert(minus1a == -1 && minus1b == -1 && minus1c == -1 && minus1d == -1 && minus1e == -1);

                    // this is the number of bytes from here to the beginning of the texture names if you include the end doeg section (4)
                    
                    int doegToTextureSize = binReader.ReadInt32();

                    byte[] doegEnd = binReader.ReadBytes(4);    
                    Debug.Assert(doegEnd[0] == 'd' && doegEnd[3] == 'g');
                    int numIndexOffset = 180;

                    binReader.BaseStream.Position += numIndexOffset;

                    List<MeshInfo> meshInfoList = new List<MeshInfo>();
//                    List<int> vertexData = new List<int>();
                    //m_numMeshes = 1;

                    for (int i = 0; i < m_numMeshes; ++i)
                    {
                        meshInfoList.Add(MeshInfo.FromStream(binReader));
                    }


                    //for (int i = 0; i<m_numMeshes; ++i)
                    //{
                    //    int val = binReader.ReadInt32();
                    //    indexData.Add(val);
                    //    if (val == 0)
                    //    {
                    //        int ibreak = 0;
                    //    }
                    //    if (i > 2 && (indexData[indexData.Count - 1] < indexData[indexData.Count - 2]))
                    //    {
                    //        int ibreak = 0;
                    //    }
                    //}


                    //int totalIndices = indexData[indexData.Count - 1];

                    binReader.BaseStream.Position += doegToTextureSize-4-numIndexOffset-(m_numMeshes * 20);

                    int totalIndices = 0;
                    int totalVertices = 0;

                    //int skip1 = binReader.ReadInt32();
                    //int skip2 = binReader.ReadInt32();

                    //int totalVertices = binReader.ReadInt32();
                    //for (int i = 0; i<m_numMeshes; ++i)
                    //{
                    //    vertexData.Add(binReader.ReadInt32());
                    //}


                    //int jumpMultiplier = 1;
                    //jumpMultiplier = 5;
                    //jumpMultiplier = 3;
                    //int jumpOffset = (doegToTextureSize - numIndexOffset - (jumpMultiplier*4));
                    //binReader.BaseStream.Position += jumpOffset;

                    StringBuilder sb = new StringBuilder();
                    
                    
                    List<string> textureNames = new List<string>();
                    char b;
                    int count = 0;
                    while (count < numTextures)
                    {
                        while ((b = (char)binReader.ReadByte()) != 0x00)
                        {
                            sb.Append(b);
                        }
                        if (sb.Length > 0)
                        {
                            textureNames.Add(sb.ToString());
                            count++;
                        }
                        sb.Clear();
                    }


                    //foreach (MeshInfo mi in meshInfoList)
                    //{
                    //    totalIndices += mi.m_numIndices;
                    //    totalVertices += mi.m_numVertices;
                    //}

                    ushort val1 = 0;
                    while (true)
                    {
                        ushort val2 = (ushort)binReader.ReadInt16();
                        //if(val2 >= val1+1000) // yuck
                        if(Math.Abs(val1 - val2) > 1000)
                        {
                            binReader.BaseStream.Position -= 2;
                            break;
                        }
                        else
                        {
                            m_allIndices.Add(val2);
                            val1 = val2;
                        }
                    }


                    for (int i = 0; i < totalIndices; ++i)
                    {
                        //subMesh.Indices.Add((ushort)Common.ToInt16BigEndian(binReader));
                        m_allIndices.Add((ushort)binReader.ReadInt16());
                    }

                    //int startIndex = 0;
                    //for (int i = 0; i < indexData.Count; ++i)
                    //{
                    //    XBoxSubMesh subMesh = new XBoxSubMesh();
                    //    m_modelMeshes.Add(subMesh);
                    //    int endIndex = i < indexData.Count - 1 ? indexData[i + 1] : indexData[indexData.Count - 1];
                    //    subMesh.Indices.AddRange(m_allIndices.Skip(startIndex).Take(endIndex - startIndex));
                    //    startIndex = endIndex;
                    //}


                    int padding = (totalIndices * 2) % 4;
                    binReader.BaseStream.Position += padding;




                    if (m_skinned)
                    {
                        ReadSkinnedVertexData(binReader,m_allVertices,totalVertices);
                    }
                    else
                    {
                        ReadUnskinnedVertexData(binReader, m_allVertices, totalVertices);
                    }

                    binReader.BaseStream.Position -= 4;
                    Debug.Assert(IsEnd(binReader));

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
            if (endBlockChar[0] == 'E' && endBlockChar[1] == 'N' && endBlock[2]  == 'D')
            {
                return true;
            }
            return false;
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

                String textureName = m_textures[notsgindex].textureName + ".jpg";
                materialWriter.WriteLine("newmtl " + textureName);
                materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                materialWriter.WriteLine("d 1.0");
                materialWriter.WriteLine("illum 3");
                materialWriter.WriteLine("map_Ka " + texturePath + textureName);
                materialWriter.WriteLine("map_Kd " + texturePath + textureName);

                materialWriter.WriteLine("refl -type sphere -mm 0 1 " + texturePath + reflectname + ".jpg");
            }
            else
            {

                foreach (TextureData textureData in m_textures)
                {
                    String textureName = textureData.textureName + ".jpg";
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

            foreach (VertexPositionNormalTexture vpnt in m_allVertices)
            {
                writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z));
                writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000} ", vpnt.TextureCoordinate.X, 1.0f-vpnt.TextureCoordinate.Y));
                writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", vpnt.Normal.X, vpnt.Normal.Y, vpnt.Normal.Z));
            }

            foreach (XBoxSubMesh headerBlock in m_modelMeshes)
            {
                submeshCount++;

                // just want highest lod.
                if (headerBlock.LodLevel != 0 && ((headerBlock.LodLevel & desiredLod) == 0))
                {
                    //   continue;
                }

                if (headerBlock.MaxUV > m_modelMeshes[0].UVs.Count)
                {
                    int ibreak = 0;
                }

                if (headerBlock.MaxVertex > m_modelMeshes[0].Vertices.Count)
                {
                    int ibreak = 0;
                }


                string groupName = String.Format("{0}-submesh{1}-LOD{2}", m_name, submeshCount, headerBlock.LodLevel);

                writer.WriteLine("o " + groupName);

                ShaderData shaderData = m_shaderData[headerBlock.MeshId];

                string adjustedTexture = FindTextureName(shaderData);
                String materialName = adjustedTexture + ".jpg";

                List<String> testMaterialNames = new List<string>();
                testMaterialNames.Add("walltexture_extra.tga.jpg");


                if (!testMaterialNames.Contains(materialName))
                {
                    //continue;
                }

                writer.WriteLine("usemtl " + materialName);
                bool swap = false;
                for (int i = 0; i < headerBlock.Indices.Count - 2; i++)
                {
                    int i1 = headerBlock.Indices[i];
                    int i2 = headerBlock.Indices[i + 1];
                    int i3 = headerBlock.Indices[i + 2];

                    // 1 based.
                    i1 += 1;
                    i2 += 1;
                    i3 += 1;
                    
                    // alternate winding
                    if (swap)
                    {
                        writer.WriteLine(String.Format("f {0}/{1}/{2}  {3}/{4}/{5} {6}/{7}/{8}", i3, i3, i3,i2, i2, i2,i1, i1,i1));
                    }
                    else
                    {
                        writer.WriteLine(String.Format("f {0}/{1}/{2}  {3}/{4}/{5} {6}/{7}/{8}", i1, i1, i1, i2, i2, i2,i3,i3,i3));
                    }
                    swap = !swap;
                }
            }
        }

        public void ReadUnskinnedVertexData(BinaryReader binReader,List<VertexPositionNormalTexture> allVertices ,int numVertices)
        {
            //for (int i = 0; i < numVertices; ++i)
            while(true)
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

                    // 24 bytes per entry?
                    Vector3 p = Common.FromStreamVector3(binReader);

                    float nx = Common.ToFloatInt16(binReader);
                    float ny = Common.ToFloatInt16(binReader);

                    float nx2 = nx * nx;
                    float ny2 = ny * ny;
                    float rem = (float)(1.0 - (nx2 + ny2));
                    float nz = 0f;
                    if (!Common.FuzzyEquals(rem, 0.0f))
                    {
                        nz = (float)Math.Sqrt(Math.Abs(rem));
                        if (rem < 0)
                        {
                            nz *= -1f;
                        }
                    }

                    if (float.IsNaN(nx) || float.IsNaN(ny) || float.IsNaN(nz))
                    {
                        int ibreak = 0;
                    }

                    // convert to normal??
                    Vector2 u = Common.FromStreamVector2(binReader);
                    VertexPositionNormalTexture vpnt = new VertexPositionNormalTexture();
                    vpnt.Position = p;
                    vpnt.TextureCoordinate = u;

                    vpnt.Normal = new Vector3(nz, ny, nx);
                    allVertices.Add(vpnt);

                    //subMesh.Vertices.Add(vpnt.Position);
                    //subMesh.Normals.Add(vpnt.Normal);
                    //subMesh.UVs.Add(vpnt.TextureCoordinate);
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }
            }
        }

        public void ReadSkinnedVertexData(BinaryReader binReader, List<VertexPositionNormalTexture> allVertices, int numVertices)
        {
            //for (int i = 0; i < numVertices; ++i)
            while(true)
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
                    //subMesh.Vertices.Add(vpnt.Position);
                    //subMesh.Normals.Add(vpnt.Normal);
                    //subMesh.UVs.Add(vpnt.TextureCoordinate);
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

        private int m_numMeshes = 0;

    }



    public class XBoxSubMesh : ModelSubMesh
    {
        public override int NumIndices
        {
            get { return Indices.Count; }
        }

        public override int NumVertices
        {
            get { return Vertices.Count; }
        }

        public override List<Vector3> Vertices
        {
            get { return m_vertices; }
        }

        public override List<Vector3> Normals
        {
            get { return m_normals; }
        }

        public override List<Vector2> UVs
        {
            get { return m_uvs; }
        }

        public override List<ushort> Indices
        {
            get { return m_indices; }
        }


        public void BuildBB()
        {
            MinBB = new Vector3(float.MaxValue);
            MaxBB = new Vector3(float.MinValue);
            foreach (Vector3 v in Vertices)
            {
                MinBB = Vector3.Min(MinBB, v);
                MaxBB = Vector3.Max(MaxBB, v);
            }
        }
        private List<Vector3> m_vertices = new List<Vector3>();
        private List<Vector3> m_normals = new List<Vector3>();
        private List<Vector2> m_uvs = new List<Vector2>();
        private List<ushort> m_indices = new List<ushort>();




        public void ReadSkinnedSubMesh(BinaryReader binReader)

        {
            // block size 56 bytes.
            int spacer = binReader.ReadInt32();
            Debug.Assert(spacer == -1);
            int skip1 = binReader.ReadInt32();
            Debug.Assert(skip1 == 0);
            int skip2 = binReader.ReadInt32();
            Debug.Assert(skip2 == 0);
            int counter1 = binReader.ReadInt32();
            int skip3 = binReader.ReadInt32();
            int skip4 = binReader.ReadInt32();
            int skip5 = binReader.ReadInt32();
            int skip6 = binReader.ReadInt32();
            int zero1 = binReader.ReadInt32();
            int zero2 = binReader.ReadInt32();
            Debug.Assert(zero1 == 0 && zero2 == 0);
        }

        public void ReadSkinnedSubMesh2(BinaryReader binReader)
        {
            // block size 56 bytes.
            int spacer = binReader.ReadInt32();
            Debug.Assert(spacer == -1);
            int skip1 = binReader.ReadInt32();
            Debug.Assert(skip1 == 0);
            int skip2 = binReader.ReadInt32();
            Debug.Assert(skip2 == 0);
            int counter1 = binReader.ReadInt32();
            int skip3 = binReader.ReadInt32();
            int skip4 = binReader.ReadInt32();
            int skip5 = binReader.ReadInt32();
            int skip6 = binReader.ReadInt32();
            int zero1 = binReader.ReadInt32();
            int zero2 = binReader.ReadInt32();
            Debug.Assert(zero1 == 0 && zero2 == 0);
        }
    
    
    
    }

    public class MeshInfo
    {
        public int m_numIndices;
        public int m_pad1;
        public int m_pad2;
        public int m_pad3;
        public int m_numVertices;


        // seems to be 20 bytes for index, vertex data per mesh?
        //2C000000 5C000000 06000000 0000803F 00040000
        //2C000000 5C000000 06000000 0000803F B8040000
        //2C000000 5C000000 06000000 0000803F 70050000
        //2C000000 5C000000 06000000 0000803F 28060000
        //2C000000 5C000000 06000000 0000803F E0060000
        //2C000000 5C000000 06000000 0000803F 98070000
        
        public static MeshInfo FromStream(BinaryReader binReader)
        {
            MeshInfo mi = new MeshInfo();
            mi.m_numIndices = binReader.ReadInt32();
            mi.m_pad1 = binReader.ReadInt32();
            mi.m_pad2 = binReader.ReadInt32();
            mi.m_numVertices = binReader.ReadInt32();
            mi.m_pad3 = binReader.ReadInt32();
            return mi;
        }

    }

}

