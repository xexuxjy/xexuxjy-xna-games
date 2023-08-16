using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GCTextureTools;
using UnityEngine;


public class GCModel
{
    public const int HeaderSize = 16;
    public const int MaxTextureNameSize = 0x80;
    public const int TextureBlockSize = 0x98;

    public const string DefaultShader = "lambert2";

    public const int AlignmentValue = 16;
    
    public GCModel(String name)
    {
        m_name = name;
    }

    public static GCModel CreateFromGameObject(GameObject gameObj)
    {
        
        MeshFilter meshFilter = gameObj.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObj.GetComponent<MeshRenderer>();
        if (meshFilter != null && meshRenderer != null)
        {
            // setup core data

            HashSet<Vector3> uniqueVertices = new HashSet<Vector3>();
            HashSet<Vector3> uniqueNormals = new HashSet<Vector3>();
            HashSet<Vector2> uniqueUVs = new HashSet<Vector2>();

            IndexedVector3 offset = IndexedVector3.Zero;
            Transform attachPoint = gameObj.transform.Find("attach");
            if (attachPoint != null)
            {
                offset = attachPoint.position;
            }
            
            foreach (Vector3 v in meshFilter.sharedMesh.vertices)
            {
                //uniqueVertices.Add(gameObj.transform.TransformPoint(v-offset));
                uniqueVertices.Add(v);
            }

            foreach (Vector3 v in meshFilter.sharedMesh.normals)
            {
                //uniqueNormals.Add(gameObj.transform.TransformDirection(v));
                uniqueNormals.Add(v);
            }

            foreach (Vector2 v in meshFilter.sharedMesh.uv)
            {
                uniqueUVs.Add(v);
            }
            

            GCModel model = new GCModel(gameObj.name);

            
            foreach (Vector3 v in uniqueVertices)
            {
                model.m_points.Add(v);
            }

            foreach (Vector3 v in uniqueNormals)
            {
                model.m_normals.Add(v);
            }

            foreach (Vector2 v in uniqueUVs)
            {
                model.m_uvs.Add(v);
            }
           
            DisplayListHeader dlh = new DisplayListHeader();
            for (int i = 0; i < meshFilter.sharedMesh.triangles.Length; i+=3)
            {
                int lookupIndex = meshFilter.sharedMesh.triangles[i];
                int posIndex = model.m_points.IndexOf(meshFilter.sharedMesh.vertices[lookupIndex]);
                int normIndex = model.m_normals.IndexOf(meshFilter.sharedMesh.normals[lookupIndex]);
                int uvIndex = model.m_uvs.IndexOf(meshFilter.sharedMesh.uv[lookupIndex]);
                
                dlh.entries.Add(new DisplayListEntry((ushort)posIndex,(ushort)normIndex,(ushort)uvIndex));
                
                lookupIndex = meshFilter.sharedMesh.triangles[i+2];
                posIndex = model.m_points.IndexOf(meshFilter.sharedMesh.vertices[lookupIndex]);
                normIndex = model.m_normals.IndexOf(meshFilter.sharedMesh.normals[lookupIndex]);
                uvIndex = model.m_uvs.IndexOf(meshFilter.sharedMesh.uv[lookupIndex]);
                
                dlh.entries.Add(new DisplayListEntry((ushort)posIndex,(ushort)normIndex,(ushort)uvIndex));

                lookupIndex = meshFilter.sharedMesh.triangles[i+1];
                posIndex = model.m_points.IndexOf(meshFilter.sharedMesh.vertices[lookupIndex]);
                normIndex = model.m_normals.IndexOf(meshFilter.sharedMesh.normals[lookupIndex]);
                uvIndex = model.m_uvs.IndexOf(meshFilter.sharedMesh.uv[lookupIndex]);
                
                dlh.entries.Add(new DisplayListEntry((ushort)posIndex,(ushort)normIndex,(ushort)uvIndex));
                
            }
            
            // go through and adjust positions and normals now that the lists have been built
            for (int i = 0; i < model.m_points.Count; ++i)
            {
                IndexedVector3 adjusted = model.m_points[i];
                adjusted = gameObj.transform.TransformPoint(adjusted);
                adjusted -= offset;
                model.m_points[i] = GladiusGlobals.UnityToGladius(adjusted);
            }

            for (int i = 0; i < model.m_normals.Count; ++i)
            {
                model.m_normals[i] = GladiusGlobals.UnityToGladius(gameObj.transform.TransformDirection(model.m_normals[i]));
            }

           
            model.m_displayListHeaders.Add(dlh);


            Material m = meshRenderer.sharedMaterial;
            string textureName = m.mainTexture.name;

            //textureName = "staff_bo";
            textureName += ".tga";
            textureName = textureName.ToLower();
           
            model.m_textures.Add(new TextureInfo()
                { Name = textureName, Width = m.mainTexture.width, Height = m.mainTexture.height });

            model.m_selsInfo.Add(DefaultShader);
            model.m_selsInfo.Add(textureName);
           
            return model;
        }

        return null;
    }


    public void LoadData(BinaryReader binReader)
    {
        Common.ReadNullSeparatedNames(binReader, GCModelReader.selsTag, m_selsInfo);

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
                Vector3 p = Common.FromStreamVector3BE(binReader);
                p = GladiusGlobals.GladiusToUnity(p);
                m_points.Add(p);
            }
        }

        if (Common.FindCharsInStream(binReader, GCModelReader.normTag))
        {
            int normSectionLength = binReader.ReadInt32();
            int uk4 = binReader.ReadInt32();
            int numNormals = binReader.ReadInt32();

            for (int i = 0; i < numNormals; ++i)
            {
                Vector3 n = Common.FromStreamVector3BE(binReader);
                n = GladiusGlobals.GladiusToUnity(n);
                m_normals.Add(n);
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
            for (int i = 0; i < numTextures; i++)
            {
                //byte[] textureBlock = binReader.ReadBytes(TextureBlockSize);
                byte[] textureNameData = binReader.ReadBytes(MaxTextureNameSize);
                //Array.Copy(textureBlock,textureNameData, MaxTextureNameSize);
                string s = Encoding.ASCII.GetString(textureNameData).Trim();
                ;

                int texNum = binReader.ReadInt32();
                int unknown = binReader.ReadInt32();
                int width = binReader.ReadInt32();
                int height = binReader.ReadInt32();
                int unknown2 = binReader.ReadInt32();
                int unknown3 = binReader.ReadInt32();

                m_textures.Add(new TextureInfo() { Name = s, Width = width, Height = height });
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

    public void BuildStandardMesh(List<int> indices, List<Vector3> points, List<Vector3> normals, List<Vector2> uvs)
    {
        foreach (DisplayListHeader dlh in m_displayListHeaders)
        {
            int counter = 0;
            for (int i = 0; i < dlh.entries.Count;i++)
            {
                DisplayListEntry entry = dlh.entries[i];

                if (entry.PosIndex >= m_points.Count)
                {
                    int ibreak = 0;
                }
                if (entry.NormIndex >= m_normals.Count)
                {
                    int ibreak = 0;
                }

                if (entry.UVIndex >= m_uvs.Count)
                {
                    int ibreak = 0;
                }

                
                points.Add(m_points[entry.PosIndex]);
                normals.Add(m_normals[entry.NormIndex]);
                uvs.Add(m_uvs[entry.UVIndex]);
                indices.Add(counter);
                counter++;
            }
        }
        
        
    }

    public void PadIfNeeded(BinaryWriter writer)
    {
        //return;
        int padValue = 64;
        if (writer.BaseStream.Position % padValue == 0)
        {
            WritePADD(writer);
        }
    }

    public void WriteData(BinaryWriter writer)
    {
        WriteVERS(writer);
        PadIfNeeded(writer);
        WriteCPRT(writer);
        PadIfNeeded(writer);
        WriteSELS(writer);
        PadIfNeeded(writer);
        WriteCNTR(writer);
        PadIfNeeded(writer);
        WriteSHDR(writer);
        PadIfNeeded(writer);
        WriteTXTR(writer);
        PadIfNeeded(writer);
        int dslsSize = WriteDSLS(writer);
        PadIfNeeded(writer);
        WriteDSLI(writer,dslsSize);
        PadIfNeeded(writer);
        WriteDSLC(writer);
        PadIfNeeded(writer);
        WritePOSI(writer);
        PadIfNeeded(writer);
        WriteNORM(writer);
        PadIfNeeded(writer);
        WriteUV0(writer);
        PadIfNeeded(writer);
        WriteVFLA(writer);
        PadIfNeeded(writer);
        WriteRAM(writer);
        PadIfNeeded(writer);
        WriteMSAR(writer);
        PadIfNeeded(writer);
        WriteNLVL(writer);
        PadIfNeeded(writer);
        WriteMESH(writer);
        PadIfNeeded(writer);
        WriteELEM(writer);
        PadIfNeeded(writer);
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

    public static int GetStringListSize(List<string> list)
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
        return total; //+pad;
    }

    public static void WriteASCIIString(BinaryWriter writer, string s, int padToLength = 0)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        writer.Write(bytes);
        if (padToLength > 0)
        {
            int pad = padToLength - bytes.Length;
            for (int i = 0; i < pad; ++i)
            {
                writer.Write((byte)0x00);
            }
        }
    }

    public void WriteVERS(BinaryWriter writer)
    {
        int total = HeaderSize+16;
        WriteASCIIString(writer, "VERS");
        // header size
        writer.Write(total);
        writer.Write(0);
        writer.Write(1);
        writer.Write(0);
        writer.Write(14);
        writer.Write(0);
        writer.Write(0);
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


    public int GetPadValue(int total)
    {
        int pad = total % AlignmentValue;
        if (pad != 0)
        {
            total += (AlignmentValue - pad);
        }

        return total;
    }
    

    public void WriteSELS(BinaryWriter writer)
    {
        int total = HeaderSize;
        int textureLength = GetStringListSize(m_selsInfo);
        total += textureLength;

        int paddedTotal = GetPadValue(total); 
        
        WriteASCIIString(writer, "SELS");

        //total = 0x50;
        writer.Write(paddedTotal);
        writer.Write(0x00);
        writer.Write(0x01);

        WriteStringList(writer, m_selsInfo, (paddedTotal - HeaderSize));
    }

    public void WriteCNTR(BinaryWriter writer)
    {
        int total = HeaderSize;
        int numV3 = 5;
        total += (12 * numV3);
        
        int paddedTotal = GetPadValue(total);
        //paddedTotal = 0x50;
        WriteASCIIString(writer, "CNTR");
        writer.Write(paddedTotal);
        writer.Write(0x01);
        writer.Write(0x01);
        
        Vector3 min = new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
        Vector3 max = new Vector3(float.MinValue,float.MinValue,float.MinValue);

        foreach(Vector3 v in m_points)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        Vector3 extents = max-min;
        extents /= 2f;
        float radius = Math.Max(extents.x,Math.Max(extents.y,extents.z));

        Vector3 midPoint = min + ((max - min) / 2f);
        
        Common.WriteVector3BE(writer, min);
        Common.WriteVector3BE(writer, max);
        Common.WriteVector3BE(writer, midPoint);
        Common.WriteVector3BE(writer,new IndexedVector3(radius,0,0));
        Common.WriteVector3BE(writer, midPoint);

        WriteNull(writer, (paddedTotal - total));

    }


    static byte[] MetalShaderData = new byte[]
    {
        0xFF, 0xFF, 0xFF, 0xFF, 0x6D, 0x65, 0x74, 0x61, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    };

    private static byte[] Lambert2ShaderData =
    {
        0xFF, 0xFF, 0xFF, 0xFF, 0x6C, 0x61, 0x6D, 0x62, 0x65, 0x72, 0x74, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    };


    public void WriteSHDR(BinaryWriter writer)
    {
        int total = HeaderSize;
        WriteASCIIString(writer, "SHDR");
        total += Lambert2ShaderData.Length;
        writer.Write(total); // block size
        writer.Write(0);
        writer.Write(1); // num materials, 1 for now
        writer.Write(Lambert2ShaderData);
    }

    public void WriteTXTR(BinaryWriter writer)
    {
        int total = HeaderSize;
        total += m_textures.Count * TextureBlockSize;
        
        int paddedTotal = GetPadValue(total);

        
        WriteASCIIString(writer, "TXTR");
        writer.Write(paddedTotal);
        writer.Write(0); 
        writer.Write(m_textures.Count);


        foreach (TextureInfo textureInfo in m_textures)
        {
            WriteASCIIString(writer, textureInfo.Name, MaxTextureNameSize);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(textureInfo.Width);
            writer.Write(textureInfo.Height);

            int unknown1 = 3;
            int unknown2 = 3;

            writer.Write(unknown1);
            writer.Write(unknown2);
        }
        
        WriteNull(writer, (paddedTotal - total));

    }


    public int WriteDSLS(BinaryWriter writer)
    {
        int total = HeaderSize;

        WriteASCIIString(writer, "DSLS");

        foreach (DisplayListHeader dlh in m_displayListHeaders)
        {
            total += dlh.GetSize();
        }

        int paddedTotal = GetPadValue(total);
        
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(1);

        foreach (DisplayListHeader dlh in m_displayListHeaders)
        {
            dlh.ToStream(writer);
        }

        WriteNull(writer, (paddedTotal - total));
        return paddedTotal - HeaderSize;
    }

    public void WriteDSLI(BinaryWriter writer,int dslsSize)
    {
        int total = HeaderSize+16;

        WriteASCIIString(writer, "DSLI");
        writer.Write(total); // block size
        writer.Write(0); 
        writer.Write(1);
        writer.Write(0);
        Common.WriteBigEndian(writer, (int)dslsSize);
        writer.Write(0);
        writer.Write(0);
    }

    public void WriteDSLC(BinaryWriter writer)
    {
        int total = HeaderSize+16;
        WriteASCIIString(writer, "DSLC");

        writer.Write(total); // block size
        writer.Write(1);
        writer.Write(1);
        writer.Write(1);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
    }

    public void WritePOSI(BinaryWriter writer)
    {
        int total = HeaderSize;
        total += (m_points.Count * 12);
        int paddedTotal = GetPadValue(total);

        WriteASCIIString(writer, "POSI");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(m_points.Count); // number of elements.

        
        foreach (IndexedVector3 v in m_points)
        {
            Common.WriteVector3BE(writer, v);
        }
        
        WriteNull(writer, (paddedTotal - total));
        
    }

    public void WriteNORM(BinaryWriter writer)
    {
        int total = HeaderSize;
        total +=  (m_normals.Count * 12);
        int paddedTotal = GetPadValue(total);

        WriteASCIIString(writer, "NORM");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(m_normals.Count); // number of elements.

        foreach (IndexedVector3 v in m_normals)
        {
            Common.WriteVector3BE(writer, v);
        }

        WriteNull(writer, (paddedTotal - total));
    }

    public void WriteUV0(BinaryWriter writer)
    {
        int total = HeaderSize;
        total +=  (m_uvs.Count * 8);
        int paddedTotal = GetPadValue(total);

        WriteASCIIString(writer, "UV0 ");

        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(m_uvs.Count); // number of elements.
        foreach (IndexedVector2 v in m_uvs)
        {
            Common.WriteVector2BE(writer, v);
        }

        WriteNull(writer, (paddedTotal - total));
    }

    static byte[] VFLAGSData = new byte[]
        { 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

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
        WriteNull(writer, blockSize - HeaderSize);
    }

    public void WriteMSAR(BinaryWriter writer)
    {
        int blockSize = 0x40;
        WriteASCIIString(writer, "MSAR");

        writer.Write(blockSize); // block size
        writer.Write(0);
        writer.Write(1);
        WriteNull(writer, blockSize - HeaderSize);
    }

    public void WriteNLVL(BinaryWriter writer)
    {
        int blockSize = 0x20;
        WriteASCIIString(writer, "NLVL");

        writer.Write(blockSize); // block size
        writer.Write(2); // number of elements.
        writer.Write(1);
        WriteNull(writer, 0x10);
    }

    public void WriteMESH(BinaryWriter writer)
    {
        int total = HeaderSize + 32;
        WriteASCIIString(writer, "MESH");
        writer.Write(total); // block size
        writer.Write(0); // number of elements.
        writer.Write(1);
        
        writer.Write(24); // union
        writer.Write(0);  // listptr
        writer.Write(0); // shader id
        writer.Write(1); // element count
        writer.Write(0); // vert array id 
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);

    }

    public void WriteELEM(BinaryWriter writer)
    {
        int total = HeaderSize+16;
        WriteASCIIString(writer, "ELEM");

        writer.Write(total); // block size
        writer.Write(0); 
        writer.Write(1);

        int val = (4 | (m_displayListHeaders[0].entries.Count << 8));
        
        writer.Write(val);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
    }

    public void WriteEND(BinaryWriter writer)
    {
        WriteASCIIString(writer, "END.");
        writer.Write(HeaderSize); // number of elements.
        writer.Write(0);
        writer.Write(0);
    }

    public void WritePADD(BinaryWriter writer)
    {
        WriteASCIIString(writer, "PADD");
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

    public int GetSize()
    {
        return 6 + (entries.Count * 6);
    }
    
    
    public void ToStream(BinaryWriter writer)
    {
        writer.Write((byte)0x98);
        writer.Write((byte)0x00);
        writer.Write((byte)0x00);
        writer.Write((byte)0x90);

        Common.WriteBigEndian(writer, (short)entries.Count);
        //writer.Write((short)entries.Count);
        foreach (DisplayListEntry ble in entries)
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


    public static DisplayListHeader CreateFromMeshData(int[] triangles, Vector3[] vertices,
        Vector3[] normals, Vector2[] uvs)
    {
        DisplayListHeader dlh = new DisplayListHeader();
        for (int i = 0; i < triangles.Length; i ++)
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

    public DisplayListEntry(ushort pos, ushort norm, ushort uv)
    {
        PosIndex = pos;
        NormIndex = norm;
        UVIndex = uv;
    }


    public void ToStream(BinaryWriter writer)
    {
        Common.WriteBigEndian(writer, (short)PosIndex);
        Common.WriteBigEndian(writer, (short)NormIndex);
        Common.WriteBigEndian(writer, (short)UVIndex);
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

    public static char[]
        selsTag = new char[]
        {
            'S', 'E', 'L', 'S'
        }; // External link information? referes to textures, other models, entities and so on? 

    public static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
    public static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
    public static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
    public static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
    public static char[] dslsTag = new char[] { 'D', 'S', 'L', 'S' }; // DisplayList information
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


    public static char[][] allTags =
    {
        versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag,
        dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag, vflaTag,
        ramTag, msarTag, nlvlTag, meshTag, elemTag, skelTag, skinTag,
        vflgTag, stypTag, nameTag, paddTag
    };

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
                infoStream.WriteLine(String.Format("File : {0} : {1} : {2}", model.m_name, model.m_points.Count,
                    model.m_normals.Count));
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


                    using (BinaryReader binReader =
                           new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
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

                        int numPadd = 0;
                        binReader.BaseStream.Position = 0;
                        while (Common.FindCharsInStream(binReader, paddTag, false))
                        {
                            numPadd++;
                            int blockSize = (int)binReader.ReadInt32();
                            infoStream.WriteLine("PADD : " + blockSize);
                        }

                        infoStream.WriteLine("Num PADD : " + numPadd);


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
                        foreach (TextureInfo textureInfo in model.m_textures)
                        {
                            sb.AppendLine($"\t {textureInfo.Name}  {textureInfo.Width}  {textureInfo.Height}");
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
                            sb.AppendLine(
                                $"Header , entries : {+header.entries.Count} , div by 3 [{(header.entries.Count % 3 == 0)}]");
                            sb.AppendLine($"MinPoint {header.entries.Min(entry => entry.PosIndex)}");
                            sb.AppendLine(
                                $"MaxPoint {header.entries.Max(entry => entry.PosIndex)}  less {(header.entries.Max(entry => entry.PosIndex) < model.m_points.Count)} ");
                            sb.AppendLine($"MinNormal {header.entries.Min(entry => entry.NormIndex)}");
                            sb.AppendLine(
                                $"MaxNormal {header.entries.Max(entry => entry.NormIndex)}  less {(header.entries.Max(entry => entry.NormIndex) < model.m_normals.Count)} ");
                            sb.AppendLine($"MinUV {header.entries.Min(entry => entry.UVIndex)}");
                            sb.AppendLine(
                                $"MaxUV {header.entries.Max(entry => entry.UVIndex)}  less {(header.entries.Max(entry => entry.UVIndex) < model.m_uvs.Count)} ");
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


public struct TextureInfo
{
    public string Name;
    public int Width;
    public int Height;
}