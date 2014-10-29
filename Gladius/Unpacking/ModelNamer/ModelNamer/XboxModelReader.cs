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
                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            if (sourceFile.Name != "File 005496")
                            {
                                // 410, 1939

                                //continue;
                            }


                            XboxModel model = LoadSingleModel(sourceFile.Name) as XboxModel;
                            m_models.Add(model);




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
            String modelPath = @"D:\gladius-extracted-archive\xbox-decompressed\ModelFilesRenamed";
            String infoFile = @"D:\gladius-extracted-archive\xbox-decompressed\ModelInfo.txt";
            //String sectionInfoFile = @"C:\tmp\unpacking\xbox-ModelFiles\index-normal-data.txt";
            XboxModelReader reader = new XboxModelReader();
            reader.LoadModels(modelPath, infoFile);
            //reader.DumpPoints(infoFile);
            //reader.DumpSectionLengths(modelPath, sectionInfoFile);



        }




        List<XboxModel> m_models = new List<XboxModel>();
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
                    int unk1b = binReader.ReadInt32();
                    int unk1c = binReader.ReadInt32();

                    int unk1d = binReader.ReadInt32();
                    m_numMeshes = binReader.ReadInt32();
                    int unk1e = binReader.ReadInt32();
                    int numMeshCopy = binReader.ReadInt32();
                    Debug.Assert(m_numMeshes == numMeshCopy);

                    binReader.BaseStream.Position += 22 * 4;

                    byte[] doegEnd = binReader.ReadBytes(4);
                    Debug.Assert(doegEnd[0] == 'd' && doegEnd[3] == 'g');
                    binReader.BaseStream.Position += 0xB4;
                    int numIndices = binReader.ReadInt32();
                    int skip1 = binReader.ReadInt32();
                    int skip2 = binReader.ReadInt32();
                    int numVertices = binReader.ReadInt32();
                    binReader.BaseStream.Position += 0x17c;



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
                    //numIndices = (short)binReader.ReadInt32();


                    for (int i = 0; i < numIndices; ++i)
                    {
                        subMesh.Indices.Add((ushort)Common.ToInt16BigEndian(binReader));
                    }

                    //ushort indexCountMaybe = (ushort)Common.ToInt16BigEndian(binReader);

                    //numVertices = 24;//8;
                    List<Vector3> vertices = new List<Vector3>();

                    for (int i = 0; i < numVertices; ++i)
                    {
                        Vector3 p = Common.FromStreamVector3BE(binReader);
                        subMesh.Vertices.Add(p);
                        float unk = binReader.ReadSingle();
                        Vector2 u = Common.FromStreamVector2BE(binReader);
                        subMesh.UVs.Add(u);
                        VertexPositionNormalTexture vpnt = new VertexPositionNormalTexture();
                        vpnt.Position = p;
                        vpnt.TextureCoordinate = u;

                        vpnt.Normal = Vector3.Up;
                        subMesh.Normals.Add(vpnt.Normal);

                        //Vertices.Add(vpnt);
                    }

                    byte[] endBlock = binReader.ReadBytes(4);
                    char[] endBlockChar = new char[endBlock.Length];
                    for (int i = 0; i < endBlock.Length; ++i)
                    {
                        endBlockChar[i] = (char)endBlock[i];
                    }
                    subMesh.BuildBB();
                    BuildBB();

                    ShaderData shaderData = new ShaderData();
                    shaderData.shaderName = "test";
                    shaderData.textureId1 = 0;
                    shaderData.textureId2 = 0;
                    m_shaderData.Add(shaderData);
                }
                else
                {
                    //infoStream.WriteLine("Doeg not at expected - multi file? : " + sourceFile.FullName);
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

    }


}

