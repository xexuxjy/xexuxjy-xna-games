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

        public static bool FromStream(BinaryReader reader, out DisplayListHeader header,DSLIInfo dsliInfo)
        {
            long currentPosition = reader.BaseStream.Position;
            bool success = false;
            byte header1 = reader.ReadByte();
            Debug.Assert(header1 == 0x098);
            short pad1 = reader.ReadInt16();

            header = new DisplayListHeader();
            header.primitiveFlags = reader.ReadByte();
            Debug.Assert(header.primitiveFlags== 0x090);
            if (header.primitiveFlags == 0x90)
            {
                header.indexCount = Common.ToInt16BigEndian(reader);
                
                success = true;
                for (int i = 0; i < header.indexCount; ++i)
                {
                    header.entries.Add(DisplayListEntry.FromStream(reader));
                }
            }
            else
            {
                reader.BaseStream.Position = currentPosition;
            }
            return success;
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

        }

        public void Validate()
        {
            foreach (DisplayListHeader header in m_displayListHeaders)
            {
                if (header.primitiveFlags == 0x90)
                {

                    for (int i = 0; i < header.entries.Count; ++i)
                    {
                        if (header.entries[i].PosIndex < 0 || header.entries[i].PosIndex >= m_points.Count)
                        {
                            header.Valid = false;
                            break;
                        }
                        if (header.entries[i].NormIndex < 0 || header.entries[i].NormIndex >= m_normals.Count)
                        {
                            header.Valid = false;
                            break;
                        }
                        if (header.entries[i].UVIndex < 0 || header.entries[i].UVIndex >= m_uvs.Count)
                        {
                            header.Valid = false;
                            break;
                        }

                        
                    }
                }
            }


        }

        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();
        public String m_name;
        public List<Vector3> m_points = new List<Vector3>();
        public List<Vector3> m_normals = new List<Vector3>();
        public List<Vector2> m_uvs = new List<Vector2>();
        public List<String> m_textures = new List<String>();
        public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
        public List<Vector3> m_centers = new List<Vector3>();
        public List<String> m_selsInfo = new List<string>();
        public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
        public Vector3 MinBB;
        public Vector3 MaxBB;
        public Vector3 Center;
        public List<Matrix4> m_matrices = new List<Matrix4>();
        //public bool Valid =true;
    }

    public class GCModelReader
    {
        static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
        static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
        static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' }; // External link information? referes to textures, other models, entities and so on? 
        static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
        static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
        static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
        static char[] dslsTag = new char[] { 'D', 'S', 'L', 'S' };  // DisplayList information
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

        public GCModel LoadSingleModel(String modelPath)
        {
            FileInfo sourceFile = new FileInfo(modelPath);

            using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
            {
                GCModel model = new GCModel(sourceFile.Name);

                Common.ReadTextureNames(binReader, txtrTag, model.m_textures);

                long currentPos = binReader.BaseStream.Position;
                ReadDSLISection(binReader, model);
                binReader.BaseStream.Position = currentPos;


                if (Common.FindCharsInStream(binReader, dslsTag))
                {
                    long dsllStartsAt = binReader.BaseStream.Position;
                    int dslsSectionLength = binReader.ReadInt32();
                    int uk2a = binReader.ReadInt32();
                    int uk2b = binReader.ReadInt32();

                    long startPos = binReader.BaseStream.Position;

                    DisplayListHeader header = null;
                    for (int i = 0; i < model.m_dsliInfos.Count; ++i)
                    {
                        binReader.BaseStream.Position = startPos + model.m_dsliInfos[i].startPos;
                        DisplayListHeader.FromStream(binReader, out header, model.m_dsliInfos[i]);
                        if (header != null)
                        {
                            model.m_displayListHeaders.Add(header);
                        }

                    }
                    long nowAt = binReader.BaseStream.Position;

                    long diff = (dsllStartsAt + (long)dslsSectionLength) - nowAt;
                    int ibreak = 0;

                }
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

                model.BuildBB();
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

                            infoStream.WriteLine("Num DSLS : " + (((model.m_tagSizes[dsliTag] - 16) / 8)-1));

                            binReader.BaseStream.Position = 0;

                            Common.ReadSELSNames(binReader, selsTag, model.m_selsInfo);


                            Common.ReadTextureNames(binReader, txtrTag, model.m_textures);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("SELS : ");
                            foreach (string selName in model.m_selsInfo)
                            {
                                sb.AppendLine("\t"+selName);
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

        public void ReadDSLISection(BinaryReader binReader,GCModel model)
        {
            if (Common.FindCharsInStream(binReader, dsliTag, true))
            {
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numSections = (blockSize - 8-4-4) / 8;

                for (int i = 0; i < numSections; ++i)
                {
                    DSLIInfo info = DSLIInfo.ReadStream(binReader);
                    if (info.length > 0)
                    {
                        model.m_dsliInfos.Add(info);
                    }
                }
            }
        }



        static void Main(string[] args)
        {
            String modelPath = @"C:\tmp\unpacking\gc-probable-models-renamed\probable-models-renamed";
            String infoFile = @"c:\tmp\unpacking\gc-models\results.txt";
            String sectionInfoFile = @"C:\tmp\unpacking\gc-probable-models-renamed\sectionInfo.txt";

            //modelPath = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed";
            //sectionInfoFile = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed-sectionInfo.txt";
            GCModelReader reader = new GCModelReader();
            //reader.LoadModels(modelPath,infoFile);
            //reader.DumpPoints(infoFile);
            reader.DumpSectionLengths(modelPath, sectionInfoFile);



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
