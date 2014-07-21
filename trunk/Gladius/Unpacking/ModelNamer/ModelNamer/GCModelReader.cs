using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;

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
        public List<DisplayListEntry> entries = new List<DisplayListEntry>();

        public static bool FromStream(BinaryReader reader, out DisplayListHeader header)
        {
            long currentPosition = reader.BaseStream.Position;
            bool success = false;
            header = new DisplayListHeader();
            header.primitiveFlags = reader.ReadByte();
            if (header.primitiveFlags == 0x90 || header.primitiveFlags == 0x98 || header.primitiveFlags == 0xA0 || header.primitiveFlags == 0x80)
            {
                if (header.primitiveFlags != 0x90 && header.primitiveFlags != 0x98)
                {
                    int ibreak = 0;
                }

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
        public short PosIndex;
        public short NormIndex;
        public short UVIndex;

        public static DisplayListEntry FromStream(BinaryReader reader)
        {
            DisplayListEntry entry = new DisplayListEntry();
            entry.PosIndex = Common.ToInt16BigEndian(reader);
            entry.NormIndex = Common.ToInt16BigEndian(reader);
            entry.UVIndex = Common.ToInt16BigEndian(reader);

            if (entry.PosIndex < 0 || entry.NormIndex < 0 || entry.UVIndex < 0)
            {
                int ibreak = 0;
            }


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
                        if (header.entries[i].PosIndex >= m_points.Count)
                        {
                            Valid = false;
                            break;
                        }
                        if (header.entries[i].NormIndex >= m_normals.Count)
                        {
                            Valid = false;
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

        public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
        public Vector3 MinBB;
        public Vector3 MaxBB;
        public Vector3 Center;
        public bool Valid =true;
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
                        if (sourceFile.Name != "File 000489")
                        {
                            //continue;
                        }

                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            if (sourceFile.Name != "File 005496")
                            {
                                // 410, 1939

                                continue;
                            }


                            GCModel model = new GCModel(sourceFile.Name);

                            
                            m_models.Add(model);


                            Common.ReadTextureNames(binReader, txtrTag, model.m_textures);
                            if (Common.FindCharsInStream(binReader, dslsTag))
                            {
                                long dsllStartsAt = binReader.BaseStream.Position;
                                int dslsSectionLength = binReader.ReadInt32();
                                int uk2a = binReader.ReadInt32();
                                int uk2b = binReader.ReadInt32();


                                DisplayListHeader header = null;
                                while (DisplayListHeader.FromStream(binReader, out header))
                                {
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
                            continue;
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



        static void Main(string[] args)
        {
            String modelPath = @"C:\tmp\unpacking\gc-probable-models\probable-models";
            String infoFile = @"c:\tmp\unpacking\gc-models\results.txt";
            String sectionInfoFile = @"C:\tmp\unpacking\gc-probable-models\sectionInfo.txt";
            GCModelReader reader = new GCModelReader();
            reader.LoadModels(modelPath,infoFile);
            //reader.DumpPoints(infoFile);
            //reader.DumpSectionLengths(modelPath, sectionInfoFile);



        }


    }

}
