using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;


public class GCModelReader : BaseModelReader
{

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

            model.LoadData(binReader,null);

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

}


public class GCModel
{
    public const int MaxTextureNameSize = 0x80;
    public const int TextureBlockSize = 0x98;

    public const string DefaultShader = "lambert2";


    public List<BaseChunk> m_chunkList = new List<BaseChunk>();

    
    public GCModel(String name)
    {
        m_name = name;
    }

    public static GCModel CreateFromGameObject(GameObject gameObj)
    {

        // not valid
        if (gameObj == null || gameObj.GetComponentsInChildren<MeshFilter>().Length == 0 ||
            gameObj.GetComponentsInChildren<MeshRenderer>().Length == 0)
        {
            return null;
        }
        
        HashSet<Vector3> uniqueVertices = new HashSet<Vector3>();
        HashSet<Vector3> uniqueNormals = new HashSet<Vector3>();
        HashSet<Vector2> uniqueUVs = new HashSet<Vector2>();

        GCModel model = new GCModel(gameObj.name);

        IndexedVector3 offset = IndexedVector3.Zero;
        Transform attachPoint = gameObj.transform.Find("attach");
        if (attachPoint != null)
        {
            offset = attachPoint.position;
        }
        
        foreach (MeshFilter meshFilter in gameObj.GetComponentsInChildren<MeshFilter>())
        {
            MeshRenderer meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
            if (meshFilter != null && meshRenderer != null)
            {
                // setup core data

                foreach (Vector3 v in meshFilter.sharedMesh.vertices)
                {
                    uniqueVertices.Add(v);
                }

                foreach (Vector3 v in meshFilter.sharedMesh.normals)
                {
                    uniqueNormals.Add(v);
                }

                foreach (Vector2 v in meshFilter.sharedMesh.uv)
                {
                    uniqueUVs.Add(v);
                }
            }
        }

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

        int subObjectCount = 0;

        
        model.m_selsInfo.Add(DefaultShader);

        
        
        foreach (MeshFilter meshFilter in gameObj.GetComponentsInChildren<MeshFilter>())
        {
            MeshRenderer meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
            if (meshFilter != null && meshRenderer != null)
            {
                DisplayListHeader dlh = new DisplayListHeader();
                for (int i = 0; i < meshFilter.sharedMesh.triangles.Length; i += 3)
                {
                    int lookupIndex = meshFilter.sharedMesh.triangles[i];
                    int posIndex = model.m_points.IndexOf(meshFilter.sharedMesh.vertices[lookupIndex]);
                    int normIndex = model.m_normals.IndexOf(meshFilter.sharedMesh.normals[lookupIndex]);
                    int uvIndex = model.m_uvs.IndexOf(meshFilter.sharedMesh.uv[lookupIndex]);

                    dlh.entries.Add(new DisplayListEntry((ushort)posIndex, (ushort)normIndex, (ushort)uvIndex));

                    lookupIndex = meshFilter.sharedMesh.triangles[i + 2];
                    posIndex = model.m_points.IndexOf(meshFilter.sharedMesh.vertices[lookupIndex]);
                    normIndex = model.m_normals.IndexOf(meshFilter.sharedMesh.normals[lookupIndex]);
                    uvIndex = model.m_uvs.IndexOf(meshFilter.sharedMesh.uv[lookupIndex]);

                    dlh.entries.Add(new DisplayListEntry((ushort)posIndex, (ushort)normIndex, (ushort)uvIndex));

                    lookupIndex = meshFilter.sharedMesh.triangles[i + 1];
                    posIndex = model.m_points.IndexOf(meshFilter.sharedMesh.vertices[lookupIndex]);
                    normIndex = model.m_normals.IndexOf(meshFilter.sharedMesh.normals[lookupIndex]);
                    uvIndex = model.m_uvs.IndexOf(meshFilter.sharedMesh.uv[lookupIndex]);

                    dlh.entries.Add(new DisplayListEntry((ushort)posIndex, (ushort)normIndex, (ushort)uvIndex));

                }

                dlh.indexCount = (ushort)dlh.entries.Count;

                model.m_displayListHeaders.Add(dlh);

                Material m = meshRenderer.sharedMaterial;
                string textureName = m.mainTexture.name;

                textureName += ".tga";
                textureName = textureName.ToLower();

                model.m_textures.Add(new TextureHeaderInfo()
                    { Name = textureName, Width = m.mainTexture.width, Height = m.mainTexture.height });

                model.m_selsInfo.Add(textureName);

                PaxElement paxElement = new PaxElement((uint)subObjectCount, 0);
                model.m_paxElements.Add(paxElement);
                
                subObjectCount++;

            }
            
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
            model.m_normals[i] =
                GladiusGlobals.UnityToGladius(gameObj.transform.TransformDirection(model.m_normals[i]));
        }
       
        return model;

    }

    
    public CommonModelData ToCommon()
    {
        CommonModelData commonModelData = new CommonModelData();
        commonModelData.GCModel = this;
        
        commonModelData.Name = m_name;
        commonModelData.VertexDataLists = new List<VertexDataAndDesc>();
        commonModelData.IndexDataList = new List<List<int>>();

        
        SKELChunk skelChunk = (SKELChunk)m_chunkList.Find(x => x is SKELChunk);
        if (skelChunk != null)
        {
            commonModelData.BoneList.AddRange(skelChunk.BoneList);
        }
        
        POSIChunk posiChunk =  (POSIChunk)m_chunkList.Find(x => x is POSIChunk);
        NORMChunk normChunk =  (NORMChunk)m_chunkList.Find(x => x is NORMChunk);
        UV0Chunk uv0Chunk =  (UV0Chunk)m_chunkList.Find(x => x is UV0Chunk);
        DSLIChunk dsliChunk  = (DSLIChunk)m_chunkList.Find(x => x is DSLIChunk);
        DSLSChunk dslsChunk  = (DSLSChunk)m_chunkList.Find(x => x is DSLSChunk);
        DSLCChunk dslcChunk = (DSLCChunk)m_chunkList.Find(x => x is DSLCChunk);
        MESHChunk meshChunk = (MESHChunk)m_chunkList.Find(x => x is MESHChunk);
        
        if (posiChunk != null && normChunk != null && uv0Chunk != null && dsliChunk != null && dslsChunk != null && meshChunk != null)
        {
            dslsChunk.BuildData(dsliChunk);

            Debug.Assert(dslsChunk.DisplayListHeaders.Count == meshChunk.PaxElements.Count);
            
            VertexDataAndDesc vertexDataAndDesc = new VertexDataAndDesc();
            commonModelData.VertexDataLists.Add(vertexDataAndDesc);

            int meshCount = 0;
            int previousMeshVertexCount = 0;
            int vertexCount = 0;
            
            foreach (DisplayListHeader dlh in dslsChunk.DisplayListHeaders)
            {
                int meshIndexCount = 0;
                
                CommonMeshData commonMeshData = new CommonMeshData();
                commonModelData.CommonMeshData.Add(commonMeshData);

                commonMeshData.Name = m_name;
                commonMeshData.Index = meshCount;
                commonMeshData.MaterialId = meshChunk.PaxElements[meshCount].MaterialId;
                
                // commonMeshData.Indices.AddRange(cmd.IndexDataList[0]);
                // for (int i = 0; i < cmd.AllVertices.Count; ++i)
                // {
                //     commonMeshData.Vertices.Add(i);
                // }

                List<int> meshIndices = new List<int>();
                commonModelData.IndexDataList.Add(meshIndices);
                
                for (int i = 0; i < dlh.entries.Count; i++)
                {
                    DisplayListEntry entry = dlh.entries[i];

                    CommonVertexInstance cvi = new CommonVertexInstance();
                    cvi.Position = posiChunk.Data[entry.PosIndex];
                    cvi.Normal = normChunk.Data[entry.NormIndex];
                    cvi.UV = uv0Chunk.Data[entry.UVIndex];

                    vertexDataAndDesc.VertexData.Add(cvi);
                    
                    int vertexIndex = dlh.entries[i].PosIndex;
                    vertexIndex = vertexCount;

                    vertexCount++;
                    
                    // adjust vertexIndex to be in bounds for this submesh
                    //vertexIndex -= previousMeshVertexCount;
                   
                    commonMeshData.Vertices.Add(vertexIndex);                    

                    meshIndices.Add(meshIndexCount);
                    meshIndexCount++;
                }
                
                previousMeshVertexCount += meshIndices.Count;
                commonMeshData.Indices.AddRange(meshIndices);

                meshCount++;

            }
        }

        
        TXTRChunk txtrChunk = (TXTRChunk)m_chunkList.Find(x => x is TXTRChunk);
        if (txtrChunk != null)
        {
            int textureCount = 0;
            foreach (PaxTexture paxTexture in txtrChunk.Textures)
            {
                CommonTextureData commonTextureData = paxTexture.ToCommon(); 
                commonModelData.CommonTextures.Add(commonTextureData);
                
                CommonMaterialData commonMaterialData = new CommonMaterialData();    
                commonMaterialData.Name = m_name + (textureCount++);
                commonMaterialData.TextureData1 = commonTextureData;
                commonModelData.CommonMaterials.Add(commonMaterialData);
            }
        }
        
        
        foreach(VertexDataAndDesc vdad in commonModelData.VertexDataLists)
        {
            commonModelData.AllVertices.AddRange(vdad.VertexData);
        }

        
        
        return commonModelData;
    }


    public SELSChunk SelsChunk
    {
        get { return (m_chunkList.Find(x => x is SELSChunk) as SELSChunk); }
    }

    public SKELChunk SkelChunk
    {
        get { return (m_chunkList.Find(x => x is SKELChunk) as SKELChunk); }
    }

    public NAMEChunk NameChunk
    {
        get { return (m_chunkList.Find(x => x is NAMEChunk) as NAMEChunk); }
    }

    public OBBTChunk OBBTChunk
    {
        get { return (m_chunkList.Find(x => x is OBBTChunk) as OBBTChunk); }
    }


    public void BuildMaterialData(GameObject go,GCModel model)
    {
        HashSet<Material> materials = new HashSet<Material>();
        foreach (MeshRenderer mr in go.GetComponentsInChildren<MeshRenderer>())
        {
            materials.Add(mr.sharedMaterial);
        }

        foreach (Material m in materials)
        {
           string textureName = m.mainTexture.name;

            textureName += ".tga";
            textureName = textureName.ToLower();


            model.m_textures.Add(new TextureHeaderInfo()
                { Name = textureName, Width = m.mainTexture.width, Height = m.mainTexture.height });

            model.m_selsInfo.Add(DefaultShader);
            model.m_selsInfo.Add(textureName);
        }

    }
    
    public void LoadData(BinaryReader binReader,StringBuilder debugInfo)
    {
        binReader.BaseStream.Position = 0;
        int count = 0;

        do
        {
            int position = (int)binReader.BaseStream.Position;
            BaseChunk chunk = BaseChunk.FromStreamMaster(m_name, binReader,debugInfo);
            if (chunk != null)
            {
                m_chunkList.Add(chunk);

                if (chunk is EndChunk)
                {
                    break;
                }
                binReader.BaseStream.Position = position + chunk.Length;
            }
        }
        while (count++ < 100);

        
        if(SkelChunk != null)
        {
            foreach(BoneNode bn in SkelChunk.BoneList)
            {
                bn.name = NameChunk.Names[bn.NameIndex];
                if(bn.Index != bn.ParentIndex)
                {
                    bn.parent = SkelChunk.BoneList[bn.ParentIndex];
                }
            }
            //BoneList.AddRange(SkeletonChunk.BoneList);
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


    public void WriteData(BinaryWriter writer)
    {
        GladiusFileWriter.WriteVERS(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteCPRT(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteSELS(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteCNTR(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteSHDR(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteTXTR(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        int dslsSize = WriteDSLS(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteDSLI(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteDSLC(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WritePOSI(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteNORM(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteUV0(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteVFLA(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteRAM(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteMSAR(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteNLVL(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteMESH(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteELEM(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteEND(writer);
    }

    

    public void WriteSELS(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;
        int textureLength = GladiusFileWriter.GetStringListSize(m_selsInfo);
        total += textureLength;

        int paddedTotal = GladiusFileWriter.GetPadValue(total); 
        
        GladiusFileWriter.WriteASCIIString(writer, "SELS");

        //total = 0x50;
        writer.Write(paddedTotal);
        writer.Write(0x00);
        writer.Write(0x01);

        GladiusFileWriter.WriteStringList(writer, m_selsInfo, (paddedTotal - GladiusFileWriter.HeaderSize));
    }

    public void WriteCNTR(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;
        int numV3 = 5;
        total += (12 * numV3);
        
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        //paddedTotal = 0x50;
        GladiusFileWriter.WriteASCIIString(writer, "CNTR");
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

        GladiusFileWriter.WriteNull(writer, (paddedTotal - total));

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
        int total = GladiusFileWriter.HeaderSize;
        GladiusFileWriter.WriteASCIIString(writer, "SHDR");
        total += Lambert2ShaderData.Length;
        writer.Write(total); // block size
        writer.Write(0);
        writer.Write(1); // num materials, 1 for now
        writer.Write(Lambert2ShaderData);
    }

    public void WriteTXTR(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;
        total += m_textures.Count * TextureBlockSize;
        
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        
        GladiusFileWriter.WriteASCIIString(writer, "TXTR");
        writer.Write(paddedTotal);
        writer.Write(0); 
        writer.Write(m_textures.Count);


        foreach (TextureHeaderInfo textureInfo in m_textures)
        {
            GladiusFileWriter.WriteASCIIString(writer, textureInfo.Name, MaxTextureNameSize);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(textureInfo.Width);
            writer.Write(textureInfo.Height);

            int unknown1 = 3;
            int unknown2 = 3;

            writer.Write(unknown1);
            writer.Write(unknown2);
        }
        
        GladiusFileWriter.WriteNull(writer, (paddedTotal - total));

    }


    public int WriteDSLS(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;

        GladiusFileWriter.WriteASCIIString(writer, "DSLS");

        foreach (DisplayListHeader dlh in m_displayListHeaders)
        {
            total += dlh.GetSize();
        }

        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(1);
        
        // end of standard header.

        foreach (DisplayListHeader dlh in m_displayListHeaders)
        {
            dlh.ToStream(writer);
        }

        GladiusFileWriter.WriteNull(writer, (paddedTotal - total));
        return paddedTotal - GladiusFileWriter.HeaderSize;
    }

    public void WriteDSLI(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;
        total += (8 * m_displayListHeaders.Count); // (start,length for each
        GladiusFileWriter.WriteASCIIString(writer, "DSLI");
        writer.Write(total); // block size
        writer.Write(0); 
        writer.Write(1);


        
        int startPos = 0;
        
        foreach (DisplayListHeader header in m_displayListHeaders)
        {
            DSLIInfo dsliInfo = new DSLIInfo();
            dsliInfo.startPos = startPos;
            dsliInfo.length = header.GetSize();;
            
            Common.WriteBigEndian(writer,dsliInfo.startPos);
            Common.WriteBigEndian(writer,dsliInfo.length);
            
            startPos += dsliInfo.length;
        }
        
        //writer.Write(0);
        //writer.Write(0);
    }

    public void WriteDSLC(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize+16;
        GladiusFileWriter.WriteASCIIString(writer, "DSLC");

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
        int total = GladiusFileWriter.HeaderSize;
        total += (m_points.Count * 12);
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        GladiusFileWriter.WriteASCIIString(writer, "POSI");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(m_points.Count); // number of elements.

        
        foreach (IndexedVector3 v in m_points)
        {
            Common.WriteVector3BE(writer, v);
        }
        
        GladiusFileWriter.WriteNull(writer, (paddedTotal - total));
        
    }

    public void WriteNORM(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;
        total +=  (m_normals.Count * 12);
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        GladiusFileWriter.WriteASCIIString(writer, "NORM");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(m_normals.Count); // number of elements.

        foreach (IndexedVector3 v in m_normals)
        {
            Common.WriteVector3BE(writer, v);
        }

        GladiusFileWriter.WriteNull(writer, (paddedTotal - total));
    }

    public void WriteUV0(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;
        total +=  (m_uvs.Count * 8);
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        GladiusFileWriter.WriteASCIIString(writer, "UV0 ");

        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(m_uvs.Count); // number of elements.
        foreach (IndexedVector2 v in m_uvs)
        {
            Common.WriteVector2BE(writer, v);
        }

        GladiusFileWriter.WriteNull(writer, (paddedTotal - total));
    }

    static byte[] VFLAGSData = new byte[]
        { 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    public void WriteVFLA(BinaryWriter writer)
    {
        GladiusFileWriter.WriteASCIIString(writer, "VFLA");
        writer.Write(0x20); // block size
        writer.Write(1);
        writer.Write(1);
        writer.Write(VFLAGSData);
    }

    public void WriteRAM(BinaryWriter writer)
    {
        int blockSize = 0x90;
        GladiusFileWriter.WriteASCIIString(writer, "RAM ");
        writer.Write(blockSize); // block size
        writer.Write(1);
        writer.Write(1);
        GladiusFileWriter.WriteNull(writer, blockSize - GladiusFileWriter.HeaderSize);
    }

    public void WriteMSAR(BinaryWriter writer)
    {
        int blockSize = 0x40;
        GladiusFileWriter.WriteASCIIString(writer, "MSAR");

        writer.Write(blockSize); // block size
        writer.Write(0);
        writer.Write(1);
        GladiusFileWriter.WriteNull(writer, blockSize - GladiusFileWriter.HeaderSize);
    }

    public void WriteNLVL(BinaryWriter writer)
    {
        int blockSize = 0x20;
        GladiusFileWriter.WriteASCIIString(writer, "NLVL");

        writer.Write(blockSize); // block size
        writer.Write(2); // number of elements.
        writer.Write(1);
        GladiusFileWriter.WriteNull(writer, 0x10);
    }

    public void WriteMESH(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize + (24 * m_paxElements.Count);
        GladiusFileWriter.WriteASCIIString(writer, "MESH");
        writer.Write(total); // block size
        writer.Write(0); // number of elements.
        writer.Write(1);

        for (int i = 0; i < m_paxElements.Count;++i)
        {
            m_paxElements[i].ToStream(writer);
        }
        
        
        
        // writer.Write(24); // union
        // writer.Write(0);  // listptr
        // writer.Write(0); // shader id
        // writer.Write(1); // element count
        // writer.Write(0); // vert array id 
        // writer.Write(0);
        // writer.Write(0);
        // writer.Write(0);

    }

    public void WriteELEM(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize+16;
        GladiusFileWriter.WriteASCIIString(writer, "ELEM");

        writer.Write(total); // block size
        writer.Write(0); 
        writer.Write(1);

        int val = (4 | (m_displayListHeaders[0].entries.Count << 8));
        
        writer.Write(val);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
    }




    public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();
    public String m_name;
    public List<IndexedVector3> m_points = new List<IndexedVector3>();
    public List<IndexedVector3> m_normals = new List<IndexedVector3>();
    public List<IndexedVector2> m_uvs = new List<IndexedVector2>();
    public List<IndexedVector2> m_uv2s = new List<IndexedVector2>();
    public List<TextureHeaderInfo> m_textures = new List<TextureHeaderInfo>();
    public List<String> m_names = new List<String>();
    public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
    
    public List<String> m_selsInfo = new List<string>();
    public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
    public List<PaxElement> m_paxElements = new List<PaxElement>();

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
    public byte header1 = 0x98;
    public ushort pad1 = 0;
    public byte primitiveFlags;
    public ushort indexCount;
    public bool Valid = true;
    public List<DisplayListEntry> entries = new List<DisplayListEntry>();

    public int GetSize()
    {
        return 6 + (entries.Count * 6);
    }
    
    
    public void ToStream(BinaryWriter writer)
    {
        writer.Write(header1);
        writer.Write(pad1);
        writer.Write(primitiveFlags);
        indexCount = (ushort)entries.Count;

        Common.WriteBigEndian(writer, (short)entries.Count);

        foreach (DisplayListEntry ble in entries)
        {
            ble.ToStream(writer);
        }
    }

    public static bool FromStream(BinaryReader reader, out DisplayListHeader header, DSLIInfo dsliInfo)
    {
        long currentPosition = reader.BaseStream.Position;
        bool success = false;

        header = new DisplayListHeader();
        
        header.header1 = reader.ReadByte();
        header.pad1 = reader.ReadUInt16();
        header.primitiveFlags = reader.ReadByte();

        
        if (header.primitiveFlags == 0x90 || header.primitiveFlags == 0x00)
        {
            header.indexCount = Common.ToUInt16BigEndian(reader);

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


public class TextureHeaderInfo
{
    public string Name;
    public int Width;
    public int Height;
    
    public int CompressedSize;
    public int UncompressedSize;
    
    public bool ContainsDefinition;
    public ushort DXTType = 0;
}