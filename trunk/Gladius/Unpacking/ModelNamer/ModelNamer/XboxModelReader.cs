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


                            XboxModel model = LoadSingleModel(sourceFile.Name);
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

        public XboxModel LoadSingleModel(String modelPath, bool readDisplayLists = true)
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

        public XboxModel(String name) : base(name)
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
            ReadSKELSection(binReader);
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
                    byte[] stuff = binReader.ReadBytes(doegLength - 8);
                    byte[] doegEnd = binReader.ReadBytes(4);
                    Debug.Assert(doegEnd[0] == 'd' && doegEnd[3] == 'g');
                    binReader.BaseStream.Position += 0xB0;
                    int numVertices = binReader.ReadInt16();
                    short numIndices = binReader.ReadInt16();
                    //binReader.BaseStream.Position += numIndices * 2 * 3;



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


                    for (int i = 0; i < numIndices; ++i)
                    {
                        m_indices.Add(binReader.ReadUInt16());
                    }

                    List<Vector3> vertices = new List<Vector3>();

                    for (int i = 0; i < numVertices; ++i)
                    {
                        Vector3 p = Common.FromStreamVector3BE(binReader);
                        Vector2 u = Common.FromStreamVector2BE(binReader);
                        VertexPositionNormalTexture vpnt = new VertexPositionNormalTexture();
                        vpnt.Position = p;
                        vpnt.TextureCoordinate = u;
                        float unk = binReader.ReadSingle();
                        vpnt.Normal = Vector3.Up;

                        m_points.Add(vpnt);
                    }
                }
                else
                {
                    //infoStream.WriteLine("Doeg not at expected - multi file? : " + sourceFile.FullName);
                }

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

        public void ConstructSkeleton()
        {
            Dictionary<int, BoneNode> dictionary = new Dictionary<int, BoneNode>();
            foreach (BoneNode node in m_bones)
            {
                dictionary[node.id] = node;
            }

            foreach (BoneNode node in m_bones)
            {
                if (node.id != node.parentId)
                {
                    BoneNode parent = dictionary[node.parentId];
                    parent.children.Add(node);
                    node.parent = parent;
                }
            }


            if (m_bones.Count > 0)
            {

                m_rootBone = m_bones[0];
                CalcBindFinalMatrix(m_rootBone, Matrix.Identity);

                if (!m_builtSkelBB)
                {
                    Vector3 min = new Vector3(float.MaxValue);
                    Vector3 max = new Vector3(float.MinValue);


                    foreach (BoneNode node in m_bones)
                    {
                        // build tranform from parent chain?
                        Vector3 offset = node.finalMatrix.Translation;

                        if (offset.X < min.X) min.X = offset.X;
                        if (offset.Y < min.Y) min.Y = offset.Y;
                        if (offset.Z < min.Z) min.Z = offset.Z;
                        if (offset.X > max.X) max.X = offset.X;
                        if (offset.Y > max.Y) max.Y = offset.Y;
                        if (offset.Z > max.Z) max.Z = offset.Z;

                    }

                    MinBB = min;
                    MaxBB = max;
                    m_builtSkelBB = true;
                }


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

        public String m_name;
        public Vector3 m_center;
        public List<VertexPositionNormalTexture> m_points = new List<VertexPositionNormalTexture>();
        public List<ushort> m_indices = new List<ushort>();
        //public List<Vector3> m_normals = new List<Vector3>();
        //public List<Vector2> m_uvs = new List<Vector2>();
        public List<String> m_selsInfo = new List<string>();
        public List<TextureData> m_textures = new List<TextureData>();
        public List<ShaderData> m_shaderData = new List<ShaderData>();
        public List<BoneNode> m_bones = new List<BoneNode>();
        public List<String> m_names = new List<String>();
        public Vector3 MinBB;
        public Vector3 MaxBB;

        private bool m_builtBB = false;
        private bool m_builtSkelBB = false;
        public bool m_skinned = false;
        public bool m_hasSkeleton = false;
        public int m_maxVertex;
        public int m_maxNormal;
        public int m_maxUv;

        public BoneNode m_rootBone;



        // doeg tag always 7C (124) , encloes start and end doeg values , 2 per file , has FFFF following

        public Dictionary<char[], TagSizeAndData> m_tagSizes = new Dictionary<char[], TagSizeAndData>();

    }

    

    public class DoegSection
    {

    }


}

