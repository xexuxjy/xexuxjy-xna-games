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


            //filenames.AddRange(Directory.GetFiles(rootPath+@"ModelFilesRenamed\weapons", "sword*"));
            //filenames.Add(rootPath + @"ModelFilesRenamed\weapons\axeCS_declamatio.mdl");
            filenames.Add(rootPath + @"ModelFilesRenamed\weapons\swordM_gladius.mdl");
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

        public XboxModel(String name)
            : base(name)
        {

        }

        public override void LoadData(BinaryReader binReader)
        {
            binReader.BaseStream.Position = 0;
            // check here as we need skinned info on building display lists.
            if (Common.FindCharsInStream(binReader, Common.skinTag))
            {
                m_skinned = true;
            }

            // Look for skeleton and skin if they exist. need to load names first.
            binReader.BaseStream.Position = 0;
            Common.ReadNullSeparatedNames(binReader, Common.nameTag, m_names);
            binReader.BaseStream.Position = 0;
            ReadTextureSection(binReader);
            binReader.BaseStream.Position = 0;
            ReadXRNDSection(binReader);
        }

        public void ReadXRNDSection(BinaryReader binReader)
        {
            binReader.BaseStream.Position = 0;

            Common.ReadNullSeparatedNames(binReader, Common.nameTag, m_names);
            binReader.BaseStream.Position = 0;
            ReadSKELSection(binReader);
            binReader.BaseStream.Position = 0;



            if (Common.FindCharsInStream(binReader, Common.xrndTag))
            {
                int sectionLength = binReader.ReadInt32();
                int uk2a = binReader.ReadInt32();
                int numEntries = binReader.ReadInt32();
                int uk2b = binReader.ReadInt32();
                int uk2c = binReader.ReadInt32();
                //int uk2d = binReader.ReadInt32();
                byte[] doegStart = binReader.ReadBytes(4);
                if (doegStart[0] == 'd' && doegStart[3] == 'g')
                {

                    Debug.Assert(doegStart[0] == 'd' && doegStart[3] == 'g');
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

                    if(m_numMeshes == 1)
                    {
                        //Debug.Assert(unk1f == 2 || unk1f == 1);
                        Debug.Assert(unk1g == 1);
                        Debug.Assert(unk1j == 0x74);
                        Debug.Assert(unk1k == 0xAC);
                        Debug.Assert(unk1l == 0xC0);
                        Debug.Assert(unk1n == -1);
                        Debug.Assert(unk1o == 0xE0);
                        Debug.Assert(unk1p == 0xE4);
                        Debug.Assert(unk1q == 0xF0);

                        int numIndexOffset = 180;

                        binReader.BaseStream.Position += numIndexOffset;


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

                        //infoStream.WriteLine(String.Format("[{0}]  I[{1}]", sourceFile.FullName, numIndices));

                        // then follows an int16 index buffer? not sure how many entries.

                        XBoxSubMesh subMesh = new XBoxSubMesh();
                        m_modelMeshes.Add(subMesh);

                        // end doeg tag.
                        //Common.FindCharsInStream(binReader, Common.doegTag);
                        //// jump forward?
                        //binReader.BaseStream.Position += 0xb4;


                        ////binReader.BaseStream.Position = 0x510;
                        ////numIndices = 33;
                        int numIndices = binReader.ReadInt32();
                        int skip1 = binReader.ReadInt32();
                        int skip2 = binReader.ReadInt32();
                        int numVertices = binReader.ReadInt32();


                        // jump forward and read textures.
                        binReader.BaseStream.Position += (doegToTextureSize - numIndexOffset - 4);

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


                        //binReader.BaseStream.Position = 0x536; // animal skull
                        for (int i = 0; i < numIndices; ++i)
                        {
                            //subMesh.Indices.Add((ushort)Common.ToInt16BigEndian(binReader));
                            subMesh.Indices.Add((ushort)binReader.ReadInt16());
                        }

                        //ushort indexCountMaybe = (ushort)Common.ToInt16BigEndian(binReader);

                        //numVertices = 24;//8;
                        //binReader.BaseStream.Position = 0xdb7; // queens anklet
                        //binReader.BaseStream.Position = 0x9AB; // animal skull
                        //binReader.BaseStream.Position += 6;

                        int padding = (numIndices * 2) % 4;

                        binReader.BaseStream.Position += padding;

                        List<Vector3> vertices = new List<Vector3>();
                        //numVertices -= 1;
                        for (int i = 0; i < numVertices; ++i)
                        {
                            try
                            {
                                Vector3 p = Common.FromStreamVector3(binReader);
                                
                                float nx = Common.ToFloatInt16(binReader);
                                float ny = Common.ToFloatInt16(binReader);
                                float nz = (float)Math.Sqrt(1.0 - ((nx * nx) + (ny * ny)));

                                // convert to normal??
                                Vector2 u = Common.FromStreamVector2(binReader);
                                VertexPositionNormalTexture vpnt = new VertexPositionNormalTexture();
                                vpnt.Position = p;
                                vpnt.TextureCoordinate = u;

                                vpnt.Normal = new Vector3(nz, ny, nx);

                                subMesh.Vertices.Add(vpnt.Position);
                                subMesh.Normals.Add(vpnt.Normal);
                                subMesh.UVs.Add(vpnt.TextureCoordinate);
                            }
                            catch (System.Exception ex)
                            {
                                int ibreak = 0;
                            }
                        }       

                        byte[] endBlock = binReader.ReadBytes(4);
                        char[] endBlockChar = new char[endBlock.Length];
                        for (int i = 0; i < endBlock.Length; ++i)
                        {
                            endBlockChar[i] = (char)endBlock[i];
                        }
                        if (endBlockChar[0] != 'E' && endBlockChar[2] != 'D')
                        {
                            Debug.Assert(false);
                        }

                        subMesh.BuildBB();

                    }
                    BuildBB();

                    ShaderData shaderData = new ShaderData();
                    shaderData.shaderName = "test";
                    shaderData.textureId1 = 1;
                    shaderData.textureId2 = 0;
                    m_shaderData.Add(shaderData);
                }
                else
                {
                    //infoStream.WriteLine("Doeg not at expected - multi file? : " + sourceFile.FullName);
                }

            }

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

            if (!m_skinned)
            {
                foreach (Vector3 v in m_modelMeshes[0].Vertices)
                {
                    writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
                }
                foreach (Vector2 v in m_modelMeshes[0].UVs)
                {
                    Vector2 va = v;
                    va.Y = 1.0f - v.Y;
                    writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000}", va.X, va.Y));
                }
                foreach (Vector3 v in m_modelMeshes[0].Normals)
                {
                    //writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
                }
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
                //writer.WriteLine("g " + groupName);
                // and now points, uv's and normals.
                if (m_skinned)
                {
                    foreach (Vector3 v in headerBlock.Vertices)
                    {
                        writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
                    }
                    foreach (Vector2 v in headerBlock.UVs)
                    {
                        Vector2 va = v;
                        va.Y = 1.0f - v.Y;
                        writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000}", va.X, va.Y));
                    }
                    foreach (Vector3 v in headerBlock.Normals)
                    {
                        //writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
                    }
                }

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
                        writer.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5}", i3, i3, i2, i2, i1, i1));
                    }
                    else
                    {
                        writer.WriteLine(String.Format("f {0}/{1} {2}/{3} {4}/{5}", i1, i1, i2, i2, i3, i3));
                    }
                    swap = !swap;
                }
            }
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


}

