using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using System.Diagnostics;

namespace ModelNamer
{
    // Info taken from : http://smashboards.com/threads/melee-dat-format.292603/
    // much appreciated.

    //http://www.falloutsoftware.com/tutorials/gl/gl3.htm

    //case 0xB8: // (GL_POINTS)
    //case 0xA8: // (GL_LINES)
    //case 0xB0: // (GL_LINE_STRIP)
    //case 0x90: // (GL_TRIANGLES)
    //case 0x98: // (GL_TRIANGLE_STRIP)
    //case 0xA0: // (GL_TRIANGLE_FAN)
    //case 0x80: // (GL_QUADS)
    public class DisplayListHeader
    {
        public byte primitiveFlags;
        public short indexCount;
        public bool Valid = true;
        public List<DisplayListEntry> entries = new List<DisplayListEntry>();
        public int maxVertex;


        public static bool FromStream(BinaryReader reader, out DisplayListHeader header,DSLIInfo dsliInfo)
        {
            long currentPosition = reader.BaseStream.Position;
            bool success = false;
            byte header1 = reader.ReadByte();
            Debug.Assert(header1 == 0x98);
            short pad1 = reader.ReadInt16();

            header = new DisplayListHeader();
            header.primitiveFlags = reader.ReadByte();
            //Debug.Assert(header.primitiveFlags== 0x090);
            if (header.primitiveFlags == 0x90 || header.primitiveFlags == 0x00)
            {
                header.indexCount = Common.ToInt16BigEndian(reader);
                
                success = true;
                for (int i = 0; i < header.indexCount; ++i)
                {
                    DisplayListEntry e = DisplayListEntry.FromStream(reader);
                    header.entries.Add(e);
                }
            }
            else
            {
                reader.BaseStream.Position = currentPosition;
            }
            return success;
        }
    }


    public class BoneNode
    {
        public String name;
        public Vector3 offset;
        public Vector3 unk1;
        public byte pad1;
        public byte pad2;
        public byte flags1;
        public byte alwaysBF;
        public byte pad3;
        public byte id;
        public byte id2;
        public byte parentId;


        public BoneNode parent;
        public List<BoneNode> children = new List<BoneNode>();

        public static BoneNode FromStream(BinaryReader binReader)
        {
            BoneNode node = new BoneNode();
            node.offset = Common.FromStreamVector3(binReader);
            node.unk1 = Common.FromStreamVector3(binReader);
            node.pad1 = binReader.ReadByte();
            node.pad2 = binReader.ReadByte();
            node.flags1 = binReader.ReadByte();
            node.alwaysBF = binReader.ReadByte();
            node.pad3 = binReader.ReadByte();
            node.id = binReader.ReadByte();
            node.id2 = binReader.ReadByte();
            node.parentId = binReader.ReadByte();
            return node;
        }
    }

    public struct DisplayListEntry
    {
        public ushort PosIndex;
        public ushort NormIndex;
        public ushort UVIndex;

        public String ToString()
        {
            return "P:" + PosIndex + " N:" + NormIndex + " U:" + UVIndex;
        }

        public static DisplayListEntry FromStream(BinaryReader reader)
        {
            DisplayListEntry entry = new DisplayListEntry();
            entry.PosIndex = Common.ToUInt16BigEndian(reader);
            entry.NormIndex = Common.ToUInt16BigEndian(reader);
            entry.UVIndex = Common.ToUInt16BigEndian(reader);

            //if (entry.PosIndex < 0 || entry.NormIndex < 0 || entry.UVIndex < 0)
            //{
            //    int ibreak = 0;
            //}


            return entry;
        }
    }

    public class GCModel
    {
        public GCModel(String name)
        {
            m_name = name;
        }

        public void BuildBB()
        {
            if (!m_builtBB)
            {
                Vector3 min = new Vector3(float.MaxValue);
                Vector3 max = new Vector3(float.MinValue);

                //MinBB.X = MinBB.Y = MinBB.Z = float.MaxValue;
                //MaxBB.X = MaxBB.Y = MaxBB.Z = float.MinValue;

                for (int i = 0; i < m_points.Count; ++i)
                {
                    if (m_points[i].X < min.X) min.X = m_points[i].X;
                    if (m_points[i].Y < min.Y) min.Y = m_points[i].Y;
                    if (m_points[i].Z < min.Z) min.Z = m_points[i].Z;
                    if (m_points[i].X > max.X) max.X = m_points[i].X;
                    if (m_points[i].Y > max.Y) max.Y = m_points[i].Y;
                    if (m_points[i].Z > max.Z) max.Z = m_points[i].Z;

                }

                MinBB = min;
                MaxBB = max;
                m_builtBB = true;
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


            if (!m_builtBB)
            {
                Vector3 min = new Vector3(float.MaxValue);
                Vector3 max = new Vector3(float.MinValue);


                foreach (BoneNode node in m_bones)
                {
                    // build tranform from parent chain?
                    Vector3 offset = new Vector3();
                    BoneNode walker = node.parent;
                    while (walker != null)
                    {
                        offset += walker.offset;
                        walker = walker.parent;
                    }
                    if (offset.X < min.X) min.X = offset.X;
                    if (offset.Y < min.Y) min.Y = offset.Y;
                    if (offset.Z < min.Z) min.Z = offset.Z;
                    if (offset.X > max.X) max.X = offset.X;
                    if (offset.Y > max.Y) max.Y = offset.Y;
                    if (offset.Z > max.Z) max.Z = offset.Z;

                }



                //MinBB.X = MinBB.Y = MinBB.Z = float.MaxValue;
                //MaxBB.X = MaxBB.Y = MaxBB.Z = float.MinValue;


                MinBB = min;
                MaxBB = max;
                m_builtBB = true;
            }
        }

        public void ReadDSLISection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, GCModelReader.dsliTag, true))
            {
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numSections = (blockSize - 8 - 4 - 4) / 8;

                for (int i = 0; i < numSections; ++i)
                {
                    DSLIInfo info = DSLIInfo.ReadStream(binReader);
                    if (m_skinned)
                    {
                        //info.length /= 2;
                        info.startPos *= 2;
                    }

                    if (info.length > 0)
                    {
                        m_dsliInfos.Add(info);
                    }
                }
            }
        }

        public void ReadSKELSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, GCModelReader.skelTag))
            {
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numBones = (blockSize - 16) / 32;

                for (int i = 0; i < numBones; ++i)
                {
                    BoneNode node = BoneNode.FromStream(binReader);
                    m_bones.Add(node);
                }
            }
            ConstructSkeleton();
        }

        public void ReadDSLSSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, GCModelReader.dslsTag))
            {
                long dsllStartsAt = binReader.BaseStream.Position;
                int dslsSectionLength = binReader.ReadInt32();
                int uk2a = binReader.ReadInt32();
                int uk2b = binReader.ReadInt32();

                long startPos = binReader.BaseStream.Position;

                DisplayListHeader header = null;
                for (int i = 0; i < m_dsliInfos.Count; ++i)
                {
                    binReader.BaseStream.Position = startPos + m_dsliInfos[i].startPos;
                    DisplayListHeader.FromStream(binReader, out header, m_dsliInfos[i]);
                    if (header != null)
                    {
                        m_displayListHeaders.Add(header);
                    }

                }
                long nowAt = binReader.BaseStream.Position;

                long diff = (dsllStartsAt + (long)dslsSectionLength) - nowAt;
                int ibreak = 0;
            }


        }



        public void ConstructSkin(GCModel model)
        {

        }


        public void Validate()
        {
            foreach (DisplayListHeader header in m_displayListHeaders)
            {
                if (header.primitiveFlags == 0x90)
                {

                    for (int i = 0; i < header.entries.Count; ++i)
                    {
                        if (header.entries[i].PosIndex > m_maxVertex)
                        {
                            m_maxVertex = header.entries[i].PosIndex;
                        }
                        if (header.entries[i].PosIndex < 0 || header.entries[i].PosIndex >= m_points.Count)
                        {
                            header.Valid = false;
                            //break;
                        }
                        if (header.entries[i].NormIndex < 0 || header.entries[i].NormIndex >= m_normals.Count)
                        {
                            header.Valid = false;
                            //break;
                        }
                        if (header.entries[i].UVIndex < 0 || header.entries[i].UVIndex >= m_uvs.Count)
                        {
                            header.Valid = false;
                            //break;
                        }

                        
                    }
                }
            }


        }

        public void WriteOBJ(StreamWriter writer,StreamWriter materialWriter)
        {
            // write material?
            foreach (String name in m_textures)
            {
                String textureName = name + ".png";
                materialWriter.WriteLine("newmtl " + textureName);
                materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                materialWriter.WriteLine("d 1.0");
                materialWriter.WriteLine("illum 2");
                materialWriter.WriteLine("map_Ka ../Textures/" + textureName);
                materialWriter.WriteLine("map_Kd ../Textures/" + textureName);
            }

            writer.WriteLine("mtllib " + m_name + ".mtl");
            // and now points, uv's and normals.
            foreach (Vector3 v in m_points)
            {
                writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
            }
            foreach (Vector2 v in m_uvs)
            {
                writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000}", v.X, 1.0f-v.Y));
            }
            foreach (Vector3 v in m_points)
            {
                writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
            }

            writer.WriteLine("usemtl " + m_textures[m_textures.Count-1]+".png");

            foreach(DisplayListHeader dlh in m_displayListHeaders)
            {
                int counter = 0;
                int offset = 1;
                for (int i = 0; i < dlh.entries.Count; )
                {
                    writer.WriteLine(String.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", dlh.entries[i].PosIndex+offset, dlh.entries[i].UVIndex+offset, dlh.entries[i].NormIndex+offset,
                        dlh.entries[i + 1].PosIndex+offset, dlh.entries[i + 1].UVIndex+offset, dlh.entries[i + 1].NormIndex+offset,
                        dlh.entries[i + 2].PosIndex + offset, dlh.entries[i + 2].UVIndex + offset, dlh.entries[i + 2].NormIndex + offset));
                    i += 3;
                }
            }


        }


        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();
        public String m_name;
        public List<Vector3> m_points = new List<Vector3>();
        public List<Vector3> m_normals = new List<Vector3>();
        public List<Vector2> m_uvs = new List<Vector2>();
        public List<String> m_textures = new List<String>();
        public List<String> m_names = new List<String>();
        public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
        public List<Vector3> m_centers = new List<Vector3>();
        public List<String> m_selsInfo = new List<string>();
        public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
        public Vector3 MinBB;
        public Vector3 MaxBB;
        public Vector3 Center;
        public List<Matrix4> m_matrices = new List<Matrix4>();
        public List<BoneNode> m_bones = new List<BoneNode>();
        private bool m_builtBB = false;
        public bool m_skinned = true;
        public int m_maxVertex;

        //public bool Valid =true;
    }

    public class GCModelReader
    {
        public static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
        public static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
        public static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' }; // External link information? referes to textures, other models, entities and so on? 
        public static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
        public static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
        public static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
        public static char[] dslsTag = new char[] { 'D', 'S', 'L', 'S' };  // DisplayList information
        public static char[] dsliTag = new char[] { 'D', 'S', 'L', 'I' };
        public static char[] dslcTag = new char[] { 'D', 'S', 'L', 'C' };
        public static char[] posiTag = new char[] { 'P', 'O', 'S', 'I' };
        public static char[] normTag = new char[] { 'N', 'O', 'R', 'M' };
        public static char[] uv0Tag = new char[] { 'U', 'V', '0', ' ' };
        public static char[] vflaTag = new char[] { 'V', 'F', 'L', 'A' };
        public static char[] ramTag = new char[] { 'R', 'A', 'M', ' ' };
        public static char[] msarTag = new char[] { 'M', 'S', 'A', 'R' };
        public static char[] nlvlTag = new char[] { 'N', 'L', 'V', 'L' };
        public static char[] meshTag = new char[] { 'M', 'E', 'S', 'H' };
        public static char[] elemTag = new char[] { 'E', 'L', 'E', 'M' };
        public static char[] skelTag = new char[] { 'S', 'K', 'E', 'L' };
        public static char[] skinTag = new char[] { 'S', 'K', 'I', 'N' };
        public static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' };
        public static char[] vflgTag = new char[] { 'V', 'F', 'L', 'G' };
        public static char[] stypTag = new char[] { 'S', 'T', 'Y', 'P' };


        public static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag, 
                                      dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag, vflaTag, 
                                      ramTag, msarTag, nlvlTag, meshTag, elemTag, skelTag, skinTag,
                                      vflgTag,stypTag,nameTag };

        public List<GCModel> m_models = new List<GCModel>();

        public void LoadModels()
        {
            LoadModels(@"c:\tmp\unpacking\gc-models\", @"c:\tmp\unpacking\gc-models\results.txt");
        }

        public GCModel LoadSingleModel(String modelPath,bool readDisplayLists = true)
        {
            FileInfo sourceFile = new FileInfo(modelPath);

            using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
            {
                GCModel model = new GCModel(sourceFile.Name);

                Common.ReadTextureNames(binReader, txtrTag, model.m_textures);

                long currentPos = binReader.BaseStream.Position;
                model.ReadDSLISection(binReader);
                binReader.BaseStream.Position = currentPos;

                if (readDisplayLists)
                {
                    model.ReadDSLSSection(binReader);
                }

                model.ReadSKELSection(binReader);
                

                if (Common.FindCharsInStream(binReader, cntrTag, true))
                {


                    //int blockSize = binReader.ReadInt32();
                    //int unk2 = binReader.ReadInt32();
                    //int unk3 = binReader.ReadInt32();
                    //for (int i = 0; i < model.m_dsliInfos.Count; ++i)
                    //{
                    //    model.m_matrices.Add(Common.FromStreamMatrix4BE(binReader));
                    //}
                    //int ibreak = 0;
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
                }

                if (Common.FindCharsInStream(binReader, normTag))
                {
                    int normSectionLength = binReader.ReadInt32();
                    int uk4 = binReader.ReadInt32();
                    int numNormals = binReader.ReadInt32();

                    for (int i = 0; i < numNormals; ++i)
                    {
                        model.m_normals.Add(Common.FromStreamVector3BE(binReader));
                    }


                }

                if (Common.FindCharsInStream(binReader, uv0Tag))
                {
                    int normSectionLength = binReader.ReadInt32();
                    int uk4 = binReader.ReadInt32();
                    int numUVs = binReader.ReadInt32();

                    for (int i = 0; i < numUVs; ++i)
                    {
                        model.m_uvs.Add(Common.FromStreamVector2BE(binReader));
                    }

                }
                if (readDisplayLists)
                {
                    model.BuildBB();
                }
                model.Validate();
                return model;
            }

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
                        GCModel model = LoadSingleModel(file);
                        if (model != null)
                        {
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

                        if (sourceFile.Name != "File 005496")
                        {
                            //continue;
                        }


                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            GCModel model = new GCModel(sourceFile.Name);
                            m_models.Add(model);
                            infoStream.WriteLine("File : " + model.m_name);
                            foreach (char[] tag in allTags)
                            {
                                // reset for each so we don't worry about order
                                binReader.BaseStream.Position = 0;
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

                            //foreach(char[] tagName in model.m_tagSizes.Keys.Values)
                            //{
                            //    if(model.m_tagSizes[tagName] > 0)
                            //    {
                            //        infoStream.WriteLine("{ : " + (((model.m_tagSizes[dsliTag] - 16) / 8) - 1));
                            //    }
                            //}

                            infoStream.WriteLine("Num DSLS : " + (((model.m_tagSizes[dsliTag] - 16) / 8)-1));

                            binReader.BaseStream.Position = 0;
                            model.ReadDSLISection(binReader);

                            binReader.BaseStream.Position = 0;
                            model.ReadDSLSSection(binReader);

                            binReader.BaseStream.Position = 0;

                            Common.ReadNullSeparatedNames(binReader, selsTag, model.m_selsInfo);
                            Common.ReadNullSeparatedNames(binReader, nameTag, model.m_names);
                            Common.ReadTextureNames(binReader, txtrTag, model.m_textures);

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("DSLI : ");
                            foreach (DSLIInfo dsliInfo in model.m_dsliInfos)
                            {
                                sb.AppendLine(String.Format("\t {0} {1}",dsliInfo.startPos,dsliInfo.length));
                            }

                            
                            sb.AppendLine("SELS : ");
                            foreach (string selName in model.m_selsInfo)
                            {
                                sb.AppendLine("\t"+selName);
                            }

                            sb.AppendLine("NAME : ");
                            foreach (string name in model.m_names)
                            {
                                sb.AppendLine("\t" + name);
                            }

                            sb.AppendLine("Textures : ");
                            foreach (string textureName in model.m_textures)
                            {
                                sb.AppendLine("\t" + textureName);
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




        //        for (int i = 0; i < numBones; ++i)
        //        {
        //            BoneNode node = BoneNode.FromStream(binReader);
        //            model.m_bones.Add(node);
        //        }
        //    }
        //    model.ConstructSkeleton();
        //}


        static void Main(string[] args)
        {
            //String modelPath = @"C:\tmp\unpacking\gc-probable-models-renamed\probable-models-renamed";
            String modelPath = @"C:\tmp\unpacking\probable-skinned-models";
            String infoFile = @"c:\tmp\unpacking\gc-models\results.txt";
            //String sectionInfoFile = @"C:\tmp\unpacking\gc-probable-models-renamed\sectionInfo.txt";
            String objOutputPath = @"d:\gladius-extracted-archive\gc-compressed\obj-models\";

            modelPath = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed";
            infoFile = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed-info.txt";
            //sectionInfoFile = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed-sectionInfo.txt";
            GCModelReader reader = new GCModelReader();
            reader.LoadModels(modelPath, infoFile);
            foreach (GCModel model in reader.m_models)
            {
                using(StreamWriter objSw = new StreamWriter(objOutputPath+model.m_name+".obj"))
                {
                    using(StreamWriter matSw = new StreamWriter(objOutputPath+model.m_name+".mtl"))
                    {
                        model.WriteOBJ(objSw,matSw);
                    }
                }
                
            }
            //reader.LoadModels(modelPath,infoFile);
            //reader.DumpPoints(infoFile);
            //reader.DumpSectionLengths(modelPath, sectionInfoFile);



        }


    }
        public class DSLIInfo
        {
            public int startPos;
            public int length;

            public static DSLIInfo ReadStream(BinaryReader reader)
            {
                DSLIInfo info = new DSLIInfo();

                info.startPos = Common.ReadInt32BigEndian(reader);
                info.length = Common.ReadInt32BigEndian(reader);
                return info;
            }

        }


}
