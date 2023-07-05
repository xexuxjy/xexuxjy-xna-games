using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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

    public static bool FromStream(BinaryReader reader, out DisplayListHeader header, DSLIInfo dsliInfo)
    {
        long currentPosition = reader.BaseStream.Position;
        bool success = false;
        byte header1 = reader.ReadByte();
        //Debug.Assert(header1 == 0x098);
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
        IndexedVector3 min = new IndexedVector3(float.MaxValue);
        IndexedVector3 max = new IndexedVector3(float.MinValue);

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
        //ConstructSkeleton();
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


    //public void ConstructSkeleton()
    //{
    //    Dictionary<int, BoneNode> dictionary = new Dictionary<int, BoneNode>();
    //    foreach (BoneNode node in m_bones)
    //    {
    //        dictionary[node.id] = node;
    //    }

    //    foreach (BoneNode node in m_bones)
    //    {
    //        if (node.id != node.parentId)
    //        {
    //            BoneNode parent = dictionary[node.parentId];
    //            parent.children.Add(node);
    //            node.parent = parent;
    //        }
    //    }

    //}

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

    public void BuildStandardMesh(List<int> indices, List<IndexedVector3> points, List<IndexedVector3> normals, List<IndexedVector2> uvs)
    {
        foreach (DisplayListHeader dlh in m_displayListHeaders)
        {
            int counter = 0;
            for (int i = 0; i < dlh.entries.Count;)
            {
                points.Add(m_points[dlh.entries[i].PosIndex]);
                normals.Add(m_normals[dlh.entries[i].NormIndex]);
                uvs.Add(m_uvs[dlh.entries[i].UVIndex]);
                indices.Add(counter);
                counter++;
                //writer.WriteLine(String.Format("{0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", dlh.entries[i].PosIndex, dlh.entries[i].UVIndex, dlh.entries[i].NormIndex,
                //    dlh.entries[i + 1].PosIndex, dlh.entries[i + 1].UVIndex, dlh.entries[i + 1].NormIndex,
                //    dlh.entries[i + 2].PosIndex, dlh.entries[i + 2].UVIndex, dlh.entries[i + 2].NormIndex));
                //i += 3;
            }
        }


    }

    public void LoadData(BinaryReader binReader)
    {
        Common.ReadTextureNames(binReader, GCModelReader.txtrTag, m_textures);

        long currentPos = binReader.BaseStream.Position;
        ReadDSLISection(binReader);
        binReader.BaseStream.Position = currentPos;

        ReadDSLSSection(binReader);

        ReadSKELSection(binReader);


        if (Common.FindCharsInStream(binReader, GCModelReader.cntrTag, true))
        {
        }

        if (Common.FindCharsInStream(binReader, GCModelReader.posiTag))
        {
            int posSectionLength = binReader.ReadInt32();
            int uk2 = binReader.ReadInt32();
            int numPoints = binReader.ReadInt32();
            for (int i = 0; i < numPoints; ++i)
            {
                m_points.Add(Common.FromStreamVector3BE(binReader));
            }
        }

        if (Common.FindCharsInStream(binReader, GCModelReader.normTag))
        {
            int normSectionLength = binReader.ReadInt32();
            int uk4 = binReader.ReadInt32();
            int numNormals = binReader.ReadInt32();

            for (int i = 0; i < numNormals; ++i)
            {
                m_normals.Add(Common.FromStreamVector3BE(binReader));
            }


        }

        if (Common.FindCharsInStream(binReader, GCModelReader.uv0Tag))
        {
            int normSectionLength = binReader.ReadInt32();
            int uk4 = binReader.ReadInt32();
            int numUVs = binReader.ReadInt32();

            for (int i = 0; i < numUVs; ++i)
            {
                m_uvs.Add(Common.FromStreamVector2BE(binReader));
            }

        }
    }

    public void WriteData(BinaryWriter bw)
    {
        WriteVERS(bw);
        WriteCPRT(bw);
        WriteSELS(bw);
        WriteCNTR(bw);
        WriteSHDR(bw);
        WriteTXTR(bw);
        WriteDSLS(bw);
        WriteDSLI(bw);
        WriteDSLC(bw);
        WritePOSI(bw);
        WriteNORM(bw);
        WriteUV0(bw);
        WriteVFLA(bw);
        WriteMSAR(bw);
        WriteNLVL(bw);
        WriteMESH(bw);
        WriteELEM(bw);
        WriteEND(bw);

    }

    public static void WriteNull(BinaryWriter bw, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            bw.Write((byte)0);
        }
    }

    public static void WriteNullPaddedString(BinaryWriter bw, string str, int requiredLength)
    {
        bw.Write(str);
        WriteNull(bw, requiredLength - str.Length);
    }

    public static void WriteStringList(BinaryWriter bw, List<string> list, int requiredPadding)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            bw.Write(list[i]);
            if (i < list.Count - 1)
            {
                bw.Write((byte)0x00);
            }
        }
    }

    public static int GetStringListSize(List<string> list, int requiredPadding)
    {
        int total = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            total += list[i].Length;
            if (i < list.Count - 1)
            {
                total += 1;
            }
        }
        return total;
    }
    public void WriteVERS(BinaryWriter bw)
    {
        // Write VERS
        bw.Write("VERS");
        WriteNull(bw, 8);
        bw.Write((byte)1);
        WriteNull(bw, 7);
        bw.Write((byte)0x0e);
        WriteNull(bw, 11);
    }

    public void WriteCPRT(BinaryWriter bw)
    {
        bw.Write("CPRT");
        bw.Write(0x90);
        bw.Write(0x00);
        bw.Write(0x80);
        bw.Write("(C) May 27 2003 LucasArts a division of LucasFilm, Inc.");
        WriteNull(bw, 0x4a);
    }


    public void WriteSELS(BinaryWriter bw)
    {
        // Texture names.
        bw.Write("SELS");
        int total = 16;
        int textureLength = GetStringListSize(m_textures, 4);
        total += textureLength;
        bw.Write(total);


    }

    public void WriteCNTR(BinaryWriter bw)
    {
        // Write VERS
        bw.Write("CNTR");
        WriteNull(bw, 8);
        bw.Write((byte)1);
        WriteNull(bw, 7);
        bw.Write((byte)0x0e);
        WriteNull(bw, 11);
    }


    public void WriteSHDR(BinaryWriter bw)
    {
        bw.Write("SHDR");
        bw.Write(0); // block size
        bw.Write(1); // num materials, 1 for now
        bw.Write(-1);
        bw.Write("metal");
        WriteNull(bw, 0x84);

    }

    public void WriteTXTR(BinaryWriter bw)
    {
        bw.Write("TXTR");
        bw.Write(0); // block size
        bw.Write(1); // num materials, 1 for now

    }



    public void WriteDSLS(BinaryWriter bw)
    {
        bw.Write("DSLS");
        bw.Write(0); // block size
    }

    public void WriteDSLI(BinaryWriter bw)
    {
        bw.Write("DSLI");
        bw.Write(0); // block size
    }

    public void WriteDSLC(BinaryWriter bw)
    {
        bw.Write("DSLC");
        bw.Write(0); // block size

    }

    public void WritePOSI(BinaryWriter bw)
    {
        bw.Write("POSI");
        bw.Write(0); // block size
        bw.Write(m_points.Count); // number of elements.
        bw.Write(0);
        bw.Write(0);
        foreach (IndexedVector3 v in m_points)
        {
            Common.WriteVector3BE(bw, v);
        }
    }

    public void WriteNORM(BinaryWriter bw)
    {
        bw.Write("NORM");
        bw.Write(0); // block size
        bw.Write(m_points.Count); // number of elements.
        bw.Write(0);
        bw.Write(0);
        foreach (IndexedVector3 v in m_normals)
        {
            Common.WriteVector3BE(bw, v);
        }
    }

    public void WriteUV0(BinaryWriter bw)
    {
        bw.Write("UV0 ");
        bw.Write(0); // block size
        bw.Write(m_uvs.Count); // number of elements.
        bw.Write(0);
        bw.Write(0);
        foreach (IndexedVector2 v in m_uvs)
        {
            Common.WriteVector2BE(bw, v);
        }
    }

    public void WriteVFLA(BinaryWriter bw)
    {
        bw.Write("VFLA");
        bw.Write(0); // block size
        bw.Write(0); // number of elements.
        bw.Write(0);
        bw.Write(0);
    }

    public void WriteMSAR(BinaryWriter bw)
    {
        bw.Write("MSAR");
        bw.Write(0); // block size
        bw.Write(0); // number of elements.
        bw.Write(0);
        bw.Write(0);
    }

    public void WriteNLVL(BinaryWriter bw)
    {
        bw.Write("NLVL");
        bw.Write(0); // block size
        bw.Write(0); // number of elements.
        bw.Write(0);
        bw.Write(0);
    }

    public void WriteMESH(BinaryWriter bw)
    {
        bw.Write("MESH");
        bw.Write(0); // block size
        bw.Write(0); // number of elements.
        bw.Write(0);
        bw.Write(0);
    }
    public void WriteELEM(BinaryWriter bw)
    {
        bw.Write("ELEM");
        bw.Write(0); // block size
        bw.Write(0); // number of elements.
        bw.Write(0);
        bw.Write(0);
    }

    public void WriteEND(BinaryWriter bw)
    {
        bw.Write("END.");
        bw.Write(0); // block size
        bw.Write(0); // number of elements.
        bw.Write(0);
        bw.Write(0);
    }


    public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();
    public String m_name;
    public List<IndexedVector3> m_points = new List<IndexedVector3>();
    public List<IndexedVector3> m_normals = new List<IndexedVector3>();
    public List<IndexedVector2> m_uvs = new List<IndexedVector2>();
    public List<IndexedVector2> m_uv2s = new List<IndexedVector2>();
    public List<String> m_textures = new List<String>();
    public List<String> m_names = new List<String>();
    public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
    public List<IndexedVector3> m_centers = new List<IndexedVector3>();
    public List<String> m_selsInfo = new List<string>();
    public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
    public IndexedVector3 MinBB;
    public IndexedVector3 MaxBB;
    public IndexedVector3 Center;
    public List<BoneNode> m_bones = new List<BoneNode>();
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

    public GCModel LoadSingleModel(String modelPath, GCModel model, bool readDisplayLists = true)
    {
        FileInfo sourceFile = new FileInfo(modelPath);

        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
        {
            if (model == null)
            {
                model = new GCModel(sourceFile.Name);
            }

            model.BuildBB();
            model.Validate();
            return model;
        }

    }



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
                    GCModel model = LoadSingleModel(file, null, true);
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
                foreach (IndexedVector3 sv in model.m_points)
                {
                    Common.WriteInt(infoStream, sv);
                }
                infoStream.WriteLine("Normals : ");
                foreach (IndexedVector3 sv in model.m_normals)
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
        String[] files = Directory.GetFiles(sourceDirectory, "*.pao",SearchOption.AllDirectories);

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

                    GCModel model = new GCModel(sourceFile.Name);
                    LoadSingleModel(sourceFile.FullName, model);


                    using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                    {
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

                        infoStream.WriteLine("Num DSLS : " + (((model.m_tagSizes[dsliTag] - 16) / 8) - 1));

                        StringBuilder sb = new StringBuilder();


                        sb.AppendLine("SELS : ");
                        foreach (string selName in model.m_selsInfo)
                        {
                            sb.AppendLine("\t" + selName);
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


                        sb.AppendLine("Num Points : " + model.m_points.Count);
                        sb.AppendLine("Num Normals: " + model.m_normals.Count);
                        sb.AppendLine("Num UVs : " + model.m_uvs.Count);
                        sb.AppendLine("DSLI : ");
                        foreach (DSLIInfo dsliInfo in model.m_dsliInfos)
                        {
                            sb.AppendLine(String.Format("\t {0} {1}", dsliInfo.startPos, dsliInfo.length));
                        }

                        sb.AppendLine("DisplayListHeaders : " + model.m_displayListHeaders.Count);
                        foreach (DisplayListHeader header in model.m_displayListHeaders)
                        {
                            sb.AppendLine($"Header , entries : {+header.entries.Count} , div by 3 [{(header.entries.Count % 3 == 0)}]");
                            sb.AppendLine($"MinPoint {header.entries.Min(entry => entry.PosIndex)}");
                            sb.AppendLine($"MaxPoint {header.entries.Max(entry => entry.PosIndex)}  less {(header.entries.Max(entry => entry.PosIndex) < model.m_points.Count)} ");
                            sb.AppendLine($"MinNormal {header.entries.Min(entry => entry.NormIndex)}");
                            sb.AppendLine($"MaxNormal {header.entries.Max(entry => entry.NormIndex)}  less {(header.entries.Max(entry => entry.NormIndex) < model.m_normals.Count)} ");
                            sb.AppendLine($"MinUV {header.entries.Min(entry => entry.UVIndex)}");
                            sb.AppendLine($"MaxUV {header.entries.Max(entry => entry.UVIndex)}  less {(header.entries.Max(entry => entry.UVIndex) < model.m_uvs.Count)} ");
                            int counter = 0;
                            //for (int i = 0; i < header.entries.Count;)
                            //{
                            //    sb.AppendLine(String.Format("{0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", header.entries[i].PosIndex, header.entries[i].UVIndex, header.entries[i].NormIndex,
                            //    header.entries[i + 1].PosIndex, header.entries[i + 1].UVIndex, header.entries[i + 1].NormIndex,
                            //    header.entries[i + 2].PosIndex, header.entries[i + 2].UVIndex, header.entries[i + 2].NormIndex));
                            //    i += 3;
                            //}
                        }


                        infoStream.WriteLine(sb.ToString());



                    }
                }
                catch (Exception e)
                {
                    infoStream.WriteLine(e.ToString());
                }
            }
        }

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


