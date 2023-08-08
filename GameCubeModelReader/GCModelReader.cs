using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


public class GCModel
{
    public const int HeaderSize = 16;
    public const int MaxTextureNameSize = 0x80;
    public const int TextureBlockSize = 0x98;

    public GCModel(String name)
    {
        m_name = name;
    }

    public void LoadData(BinaryReader binReader)
    {
        Common.ReadNullSeparatedNames(binReader, GCModelReader.selsTag, m_selsInfo);

        if (Common.FindCharsInStream(binReader, GCModelReader.cntrTag, true))
        {
            binReader.BaseStream.Position += 12;

            m_centers.Add(Common.FromStreamVector4BE(binReader));
            m_centers.Add(Common.FromStreamVector4BE(binReader));
            m_centers.Add(Common.FromStreamVector4BE(binReader));
            m_centers.Add(Common.FromStreamVector4BE(binReader));
            //m_centers.Add(Common.FromStreamVector4(binReader));
            //m_centers.Add(Common.FromStreamVector4(binReader));
            //m_centers.Add(Common.FromStreamVector4(binReader));
            //m_centers.Add(Common.FromStreamVector4(binReader));

        }

        ReadTXTRSection(binReader);


        long currentPos = binReader.BaseStream.Position;
        ReadDSLISection(binReader);
        binReader.BaseStream.Position = currentPos;

        ReadDSLSSection(binReader);

        ReadSKELSection(binReader);



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


    public void ReadTXTRSection(BinaryReader binReader)
    {
        //Common.ReadNullSeparatedNames(binReader, GCModelReader.txtrTag, m_textures);
        if (Common.FindCharsInStream(binReader, GCModelReader.txtrTag, true))
        {
            int blockSize = binReader.ReadInt32();
            int uk4 = binReader.ReadInt32();
            int numTextures = binReader.ReadInt32();
            for(int i=0;i<numTextures; i++)
            {
                //byte[] textureBlock = binReader.ReadBytes(TextureBlockSize);
                byte[] textureNameData = binReader.ReadBytes(MaxTextureNameSize);
                //Array.Copy(textureBlock,textureNameData, MaxTextureNameSize);
                string s = Encoding.ASCII.GetString(textureNameData).Trim();;

                int texNum = binReader.ReadInt32();
                //Debug.Assert(texNum == -1);
                int unknown = binReader.ReadInt32();
                int width = binReader.ReadInt32();
                int height = binReader.ReadInt32();
                int unknown2 = binReader.ReadInt32();
                int unknown3 = binReader.ReadInt32();

                m_textures.Add(new TextureInfo(){Name=s, Width=width,Height=height });


            }

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
            int numBones = (blockSize - HeaderSize) / 32;

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


    public void WriteData(BinaryWriter writer)
    {
        WriteVERS(writer);
        WriteCPRT(writer);
        WriteSELS(writer);
        WriteCNTR(writer);
        WriteSHDR(writer);
        WriteTXTR(writer);
        WriteDSLS(writer);
        WritePADD(writer);
        WriteDSLI(writer);
        WriteDSLC(writer);
        WritePOSI(writer);
        WritePADD(writer);
        WriteNORM(writer);
        WritePADD(writer);
        WriteUV0(writer);
        WritePADD(writer);
        WriteVFLA(writer);
        WriteRAM(writer);
        WriteMSAR(writer);
        WriteNLVL(writer);
        WriteMESH(writer);
        WriteELEM(writer);
        WriteEND(writer);

    }

    public static void WriteNull(BinaryWriter writer, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            writer.Write((byte)0);
        }
    }

    public static void WriteNullPaddedString(BinaryWriter writer, string str, int requiredLength)
    {
        writer.Write(str);
        WriteNull(writer, requiredLength - str.Length);
    }

    public static void WriteStringList(BinaryWriter writer, List<string> list, int totalLength)
    {
        int ongoingTotal = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            ongoingTotal += list[i].Length;
            WriteASCIIString(writer, list[i]);
            if (i < list.Count - 1)
            {
                writer.Write((byte)0x00);
                ongoingTotal += 1;
            }
        }
        while (ongoingTotal < totalLength)
        {
            writer.Write((byte)0x00);
            ongoingTotal++;
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
        //int pad = total % requiredPadding;

        return total;//+pad;
    }

    public static void WriteASCIIString(BinaryWriter writer, string s,int padToLength=0)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        writer.Write(bytes);
        if(padToLength > 0)
        {
            int pad = padToLength - bytes.Length;
            for(int i= 0; i < pad; ++i)
            {
                writer.Write((byte)0x00);
            }
        }
    }

    public void WriteVERS(BinaryWriter writer)
    {
        // Write VERS
        WriteASCIIString(writer, "VERS");
        // header size
        writer.Write(0x20);
        writer.Write(0x00);
        writer.Write(0x01);
        writer.Write(0x00);
        writer.Write(0x0E);
        writer.Write(0x00);
        writer.Write(0x00);
    }

    public void WriteCPRT(BinaryWriter writer)
    {
        WriteASCIIString(writer, "CPRT");
        writer.Write(0x90);
        writer.Write(0x00);
        writer.Write(0x80);
        WriteASCIIString(writer, "(C) May 27 2003 LucasArts a division of LucasFilm, Inc.");
        WriteNull(writer, 0x49);
    }


    public void WriteSELS(BinaryWriter writer)
    {
        int total = HeaderSize;
        int textureLength = GetStringListSize(m_selsInfo, 4);
        total += textureLength;
        //total += (shader.Length+1);

        int padSize = 4;
        int pad = total % padSize;
        if (pad != 0)
        {
            total += (padSize - pad);
        }
        WriteASCIIString(writer, "SELS");

        //total = 0x50;
        writer.Write(total);
        writer.Write(0x00);
        writer.Write(0x01);


        WriteStringList(writer, m_selsInfo, (total - HeaderSize));

    }

    public void WriteCNTR(BinaryWriter writer)
    {
        // Write VERS
        WriteASCIIString(writer, "CNTR");
        writer.Write(0x50);
        writer.Write(0x01);
        writer.Write(0x01);

        // 4x4 matrix.
        foreach (IndexedVector4 v in m_centers)
        {
            Common.WriteVector4BE(writer, v);
            //Common.WriteVector4(writer, v);
        }
    }


    static byte[] MetalShaderData = new byte[]{0xFF,0xFF,0xFF,0xFF,0x6D,0x65,0x74,0x61,0x6C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0x00,0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00 };
    public void WriteSHDR(BinaryWriter writer)
    {
        int total = HeaderSize;
        WriteASCIIString(writer, "SHDR");
        total+= MetalShaderData.Length;
        writer.Write(total); // block size
        writer.Write(0);
        writer.Write(1); // num materials, 1 for now
        writer.Write(MetalShaderData);
    }

    public void WriteTXTR(BinaryWriter writer)
    {
        int total = HeaderSize;
        total += m_textures.Count * TextureBlockSize;
        WriteASCIIString(writer, "TXTR");
        writer.Write(total);
        writer.Write(0); // block size
        writer.Write(m_textures.Count);


        foreach(TextureInfo textureInfo in m_textures)
        {
            WriteASCIIString(writer,textureInfo.Name,MaxTextureNameSize);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(textureInfo.Width);
            writer.Write(textureInfo.Height);

            int unknown1 = 3;
            int unknown2 = 3;

            writer.Write(unknown1);
            writer.Write(unknown2);
        }


    }



    public void WriteDSLS(BinaryWriter writer)
    {
        int total = HeaderSize;

        WriteASCIIString(writer, "DSLS");
        
        foreach(DisplayListHeader dlh in m_displayListHeaders)
        {
            total += 4;
            total += dlh.entries.Count * 6;
        }

        writer.Write(total); // block size
        writer.Write(1);
        writer.Write(m_displayListHeaders.Count);

        foreach(DisplayListHeader dlh in m_displayListHeaders)
        {
            dlh.ToStream(writer);
        }
        WriteNull(writer,2);
    }

    public void WriteDSLI(BinaryWriter writer)
    {
        int blockSize = 0x20;
        WriteASCIIString(writer, "DSLI");

        writer.Write(blockSize); // block size
        writer.Write(2); // number of elements.
        writer.Write(1);
        WriteNull(writer,0x10);
    }

    public void WriteDSLC(BinaryWriter writer)
    {
        int blockSize = 0x20;
        WriteASCIIString(writer, "DSLC");

        writer.Write(blockSize); // block size
        writer.Write(2); // number of elements.
        writer.Write(1);
        WriteNull(writer,0x10);
    }

    public void WritePOSI(BinaryWriter writer)
    {
        int blockSize = HeaderSize + (m_points.Count * 12);
        WriteASCIIString(writer, "POSI");

        writer.Write(blockSize); // block size
        writer.Write(1);
        writer.Write(m_points.Count); // number of elements.
        foreach (IndexedVector3 v in m_points)
        {
            Common.WriteVector3BE(writer, v);
        }
    }

    public void WriteNORM(BinaryWriter writer)
    {
        int blockSize = HeaderSize + (m_normals.Count * 12);

        WriteASCIIString(writer, "NORM");

        writer.Write(blockSize); // block size
        writer.Write(1);
        writer.Write(m_normals.Count); // number of elements.
        foreach (IndexedVector3 v in m_normals)
        {
            Common.WriteVector3BE(writer, v);
        }
        // fixme
        WriteNull(writer,8);
    }

    public void WriteUV0(BinaryWriter writer)
    {
        int blockSize = HeaderSize + (m_uvs.Count * 8);
        WriteASCIIString(writer, "UV0 ");

        writer.Write(blockSize); // block size
        writer.Write(1);
        writer.Write(m_uvs.Count); // number of elements.
        foreach (IndexedVector2 v in m_uvs)
        {
            Common.WriteVector2BE(writer, v);
        }
        // fixme
        WriteNull(writer,8);
    }

    static byte[] VFLAGSData = new byte[]{0x00,0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    public void WriteVFLA(BinaryWriter writer)
    {
        WriteASCIIString(writer, "VFLA");
        writer.Write(0x20); // block size
        writer.Write(1); 
        writer.Write(1);
        writer.Write(VFLAGSData);
    }

    public void WriteRAM(BinaryWriter writer)
    {
        int blockSize = 0x90;
        WriteASCIIString(writer, "RAM ");
        writer.Write(blockSize); // block size
        writer.Write(1); 
        writer.Write(1);
        WriteNull(writer,blockSize-HeaderSize);
    }

    public void WriteMSAR(BinaryWriter writer)
    {
        int blockSize = 0x40;
        WriteASCIIString(writer, "MSAR");

        writer.Write(blockSize); // block size
        writer.Write(0); 
        writer.Write(1);
        WriteNull(writer,blockSize-HeaderSize);
    }

    public void WriteNLVL(BinaryWriter writer)
    {
        int blockSize = 0x20;
        WriteASCIIString(writer, "NLVL");

        writer.Write(blockSize); // block size
        writer.Write(2); // number of elements.
        writer.Write(1);
        WriteNull(writer,0x10);
    }

    public void WriteMESH(BinaryWriter writer)
    {
        WriteASCIIString(writer, "MESH");

        writer.Write(0); // block size
        writer.Write(0); // number of elements.
        writer.Write(0);
        writer.Write(0);
    }
    public void WriteELEM(BinaryWriter writer)
    {
        WriteASCIIString(writer, "ELEM");

        writer.Write(0); // block size
        writer.Write(0); // number of elements.
        writer.Write(0);
        writer.Write(0);
    }

    public void WriteEND(BinaryWriter writer)
    {
        WriteASCIIString(writer, "END.");

        writer.Write(HeaderSize); // block size
        writer.Write(0x00); // number of elements.
        writer.Write(0x00);
    }

    public void WritePADD(BinaryWriter writer)
    {
        WriteASCIIString(writer,"PADD");
        writer.Write(HeaderSize);
        writer.Write(0x00);
        writer.Write(0x00);
    }


    public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();
    public String m_name;
    public List<IndexedVector3> m_points = new List<IndexedVector3>();
    public List<IndexedVector3> m_normals = new List<IndexedVector3>();
    public List<IndexedVector2> m_uvs = new List<IndexedVector2>();
    public List<IndexedVector2> m_uv2s = new List<IndexedVector2>();
    public List<TextureInfo> m_textures = new List<TextureInfo>();
    public List<String> m_names = new List<String>();
    public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
    public List<IndexedVector4> m_centers = new List<IndexedVector4>();
    public List<String> m_selsInfo = new List<string>();
    public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
    public IndexedVector3 MinBB;
    public IndexedVector3 MaxBB;
    public IndexedVector3 Center;
    public List<BoneNode> m_bones = new List<BoneNode>();
    //public bool Valid =true;
}



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

    public void ToStream(BinaryWriter writer)
    {
        writer.Write((byte)0x98);
        writer.Write((byte)0x00);
        writer.Write((byte)0x00);
        writer.Write((byte)0x90);
    
        Common.WriteBigEndian(writer,(short)entries.Count);
        //writer.Write((short)entries.Count);
        foreach(DisplayListEntry ble in entries)
        {
            ble.ToStream(writer);
        }

    }

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


    public static DisplayListHeader CreateFromMeshData(List<int> triangles,List<IndexedVector3> vertices,List<IndexedVector3> normals,List<IndexedVector2> uvs)
    {
        DisplayListHeader dlh = new DisplayListHeader();
        for(int i=0; i<triangles.Count;i+=3)
        {
            dlh.entries.Add(new DisplayListEntry((ushort)triangles[i]));
        }

        return dlh;
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

    public DisplayListEntry(ushort index)
    {
        PosIndex = index;
        NormIndex = index;
        UVIndex = index;
    }

    public DisplayListEntry(ushort pos,ushort norm, ushort uv)
    {
        PosIndex = pos;
        NormIndex = norm;
        UVIndex = uv;

    }



    public void ToStream(BinaryWriter writer)
    {
        Common.WriteBigEndian(writer,(short)PosIndex);
        Common.WriteBigEndian(writer,(short)NormIndex);
        Common.WriteBigEndian(writer,(short)UVIndex);
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




public class GCModelReader
{
    public static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
    public static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
    public static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' }; // External link information? referes to textures, other models, entities and so on? 
    public static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
    public static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
    public static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
    public static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
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
                                      vflgTag,stypTag,nameTag,paddTag };

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

            model.LoadData(binReader);

            model.BuildBB();
            model.Validate();
            return model;
        }

    }



    public void LoadModels(String sourceDirectory, String infoFile, int maxFiles = -1)
    {
        m_models.Clear();
        String[] files = Directory.GetFiles(sourceDirectory, "*.pax");
        int counter = 0;

        using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
        {
            foreach (String file in files)
            {
                if(!file.Contains("candycane"))
                {
                    continue;
                }

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

    public static string GetStructure(BinaryReader binReader)
    {
        string result = "";
        while(binReader.BaseStream.Position < binReader.BaseStream.Length)
        { 
            long position = binReader.BaseStream.Position;
            char[] tag = binReader.ReadChars(4);
            int size = binReader.ReadInt32();
            int ver = binReader.ReadInt32();
            int numElements = binReader.ReadInt32();

            string tagString = new String(tag);

            int newPosition = (int)binReader.BaseStream.Position + (size - 8);
            if(newPosition >= binReader.BaseStream.Length)
            {
                int ibreak  =0;
            }

            binReader.BaseStream.Position += (size - 16);
            
            result += $"{tagString}  Pos[{position}]  Size[{size}] Ver[{ver}]  Elements[{numElements}]\n";
        }
        return result;
    }




    public void DumpSectionLengths(String sourceDirectory, String infoFile)
    {
        m_models.Clear();
        String[] files = Directory.GetFiles(sourceDirectory, "*.pax", SearchOption.AllDirectories);

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
                                //infoStream.WriteLine(String.Format("\t {0} : {1}", new String(tag), blockSize));
                            }
                            else
                            {
                                model.m_tagSizes[tag] = -1;
                            }
                        }
                        
                        binReader.BaseStream.Position = 0;
                        infoStream.WriteLine(GetStructure(binReader));


                        //int[] alignVals = new int[]{128,64,32,16 };
                        //int numPadd = 0;
                        //binReader.BaseStream.Position = 0;
                        //while(Common.FindCharsInStream(binReader,paddTag,false))
                        //{
                        //    numPadd++;
                        //    int blockSize = (int)binReader.ReadInt32();
                        //    int startPos = (int)(binReader.BaseStream.Position-8);

                        //    int alignValue = 1;
                        //    foreach(int val in alignVals)
                        //    {
                        //        if(startPos % val ==0)
                        //        {
                        //            alignValue = val;
                        //            break;
                        //        }
                        //    }

                        //    infoStream.WriteLine("PADD : " + blockSize+ " Position = "+startPos+" Align "+alignValue);

                        //}

                        //infoStream.WriteLine("Num PADD : " + numPadd);


                        //foreach(char[] tagName in model.m_tagSizes.Keys.Values)
                        //{
                        //    if(model.m_tagSizes[tagName] > 0)
                        //    {
                        //        infoStream.WriteLine("{ : " + (((model.m_tagSizes[dsliTag] - 16) / 8) - 1));
                        //    }
                        //}

                        //binReader.BaseStream.Position = 0;
                        //Common.FindCharsInStream(binReader,dsliTag,false);
                        //infoStream.WriteLine($"DSLI  : {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()}");
                       


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
                        foreach (TextureInfo textureInfo in model.m_textures)
                        {
                            sb.AppendLine($"\t {textureInfo.Name.Trim()}  {textureInfo.Width}  {textureInfo.Height}");
                        }


                        sb.AppendLine("Num Points : " + model.m_points.Count);
                        sb.AppendLine("Num Normals: " + model.m_normals.Count);
                        sb.AppendLine("Num UVs : " + model.m_uvs.Count);

                        sb.AppendLine("DSLI : ");
                        foreach (DSLIInfo dsliInfo in model.m_dsliInfos)
                        {
                            sb.AppendLine(String.Format("\t {0} {1}", dsliInfo.startPos, dsliInfo.length));
                        }

                        //binReader.BaseStream.Position = 0;
                        //Common.FindCharsInStream(binReader,dsliTag,false);
                        //infoStream.WriteLine($"DSLI  : {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()}");

                        binReader.BaseStream.Position = 0;
                        Common.FindCharsInStream(binReader,dslcTag,false);
                        infoStream.WriteLine($"DSLC  : {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()} {binReader.ReadInt32()}");


                        sb.AppendLine("DisplayListHeaders : " + model.m_displayListHeaders.Count);
                        foreach (DisplayListHeader header in model.m_displayListHeaders)
                        {
                            sb.AppendLine($"Header , entries : {+header.entries.Count} , div by 3 [{(header.entries.Count % 3 == 0)}]");
                            //sb.AppendLine($"MinPoint {header.entries.Min(entry => entry.PosIndex)}");
                            //sb.AppendLine($"MaxPoint {header.entries.Max(entry => entry.PosIndex)}  less {(header.entries.Max(entry => entry.PosIndex) < model.m_points.Count)} ");
                            //sb.AppendLine($"MinNormal {header.entries.Min(entry => entry.NormIndex)}");
                            //sb.AppendLine($"MaxNormal {header.entries.Max(entry => entry.NormIndex)}  less {(header.entries.Max(entry => entry.NormIndex) < model.m_normals.Count)} ");
                            //sb.AppendLine($"MinUV {header.entries.Min(entry => entry.UVIndex)}");
                            //sb.AppendLine($"MaxUV {header.entries.Max(entry => entry.UVIndex)}  less {(header.entries.Max(entry => entry.UVIndex) < model.m_uvs.Count)} ");
                            int counter = 0;
                            //for (int i = 0; i < header.entries.Count;)
                            //{
                            //    sb.AppendLine(String.Format("{0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", header.entries[i].PosIndex, header.entries[i].UVIndex, header.entries[i].NormIndex,
                            //    header.entries[i + 1].PosIndex, header.entries[i + 1].UVIndex, header.entries[i + 1].NormIndex,
                            //    header.entries[i + 2].PosIndex, header.entries[i + 2].UVIndex, header.entries[i + 2].NormIndex));
                            //    i += 3;
                            //}
                        }

                        binReader.BaseStream.Position = 0;
                        Common.FindCharsInStream(binReader,meshTag,false);
                        infoStream.WriteLine($"MESH  : size[{binReader.ReadInt32()}] ver[{binReader.ReadInt32()}] num[{binReader.ReadInt32()}] Union[{binReader.ReadInt32()}] ListPtr[{binReader.ReadInt32()}] ShaderId[{binReader.ReadInt32()}] elementCount[{binReader.ReadInt32()}] vertArrayId[{binReader.ReadInt32()}] ssMask[{binReader.ReadInt32()}]");

                        binReader.BaseStream.Position = 0;
                        Common.FindCharsInStream(binReader,elemTag,false);
                        infoStream.WriteLine($"ELEM  : size[{binReader.ReadInt32()}] ver[{binReader.ReadInt32()}] num[{binReader.ReadInt32()}] A[{binReader.ReadInt32()}] B[{binReader.ReadInt32()}] C[{binReader.ReadInt32()}] D[{binReader.ReadInt32()}]");




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
        //info.startPos = reader.ReadInt32();
        //info.length = reader.ReadInt32();


        return info;
    }

}


public struct TextureInfo
{
    public string Name;
    public int Width;
    public int Height;
}

