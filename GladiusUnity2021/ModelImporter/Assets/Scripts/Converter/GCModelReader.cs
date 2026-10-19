using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Security.Policy;
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

    public static GCModel LoadSingleModel(String modelPath, bool readDisplayLists = true)
    {
        FileInfo sourceFile = new FileInfo(modelPath);

        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
        {
            GCModel model = GCModel.ReadData(binReader, sourceFile.Name, null);
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
                    GCModel model = LoadSingleModel(file, true);
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


public class GCModel : BaseModel
{
    public const int MaxTextureNameSize = 0x80;
    public const int TextureBlockSize = 0x98;
    public const int MaterialBlockSize = 0xA4;

    public const string DefaultShader = "lambert2";

    public Type[] ChunkTypeDefaultOrder = new Type[]
    {
        typeof(VERSChunk), typeof(CPRTChunk), typeof(SELSChunk), typeof(NAMEChunk), typeof(CNTRChunk),
        typeof(SHDRChunk), typeof(TXTRChunk),
        typeof(DSLSChunk), typeof(DSLIChunk), typeof(DSLCChunk), typeof(UV0Chunk), typeof(SKINChunk), typeof(SKELChunk),
        typeof(VFLAChunk), typeof(VFLAChunk), typeof(RAMChunk), typeof(MSARChunk),
        typeof(NLVLChunk), typeof(MESHChunk), typeof(ELEMChunk), typeof(STYPChunk), typeof(JLODChunk), typeof(ENDChunk)
    };


    public Type[] ChunkTypeWriteOrder = new Type[]
    {
        typeof(VERSChunk), typeof(CPRTChunk), typeof(SELSChunk), typeof(DSLCChunk), typeof(RAMChunk), typeof(SHDRChunk),
        typeof(TXTRChunk), typeof(MESHChunk), typeof(ELEMChunk),
        typeof(DSLSChunk), typeof(DSLIChunk), typeof(UV0Chunk), typeof(VFLGChunk), typeof(STYPChunk), typeof(NAMEChunk),
        typeof(SKELChunk), typeof(JLODChunk),
        typeof(CNTRChunk), typeof(SKINChunk), typeof(ENDChunk)
    };


    public GCModel(String name) : base(name)
    {
        m_name = name;
        foreach (Type type in ChunkTypeWriteOrder)
        {
            m_chunkList.Add(Activator.CreateInstance(type) as BaseChunk);
        }

        foreach (BaseChunk chunk in m_chunkList)
        {
            chunk.ChunkList = m_chunkList;
        }
    }

    public static GCModel CreateFromGameObject(GameObject gameObj, short animShift, uint lodLevel, int ramSize,Transform boneRoot)
    {
        // not valid
        if (gameObj == null)
        {
            return null;
        }

        MeshFilter[] meshFilters = gameObj.GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = gameObj.GetComponentsInChildren<SkinnedMeshRenderer>();

        if ((meshFilters == null || meshFilters.Length == 0) &&
            (skinnedMeshRenderers == null || skinnedMeshRenderers.Length == 0))
        {
            return null;
        }

        bool isSkinned = skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0;


        List<Vector3> adjustedVertices = new List<Vector3>();
        List<Vector3> adjustedNormals = new List<Vector3>();
        HashSet<Vector2> uniqueUVs = new HashSet<Vector2>();

        GCModel model = new GCModel(gameObj.name);

        IndexedVector3 offset = IndexedVector3.Zero;
        Transform attachPoint = gameObj.transform.Find("attach");
        if (attachPoint != null)
        {
            offset = attachPoint.position;
        }
        // build a skeleton from rootBone.


        if (isSkinned)
        {
            if (boneRoot == null)
            {
                // Look and see if theres a GladiusToUnity transform.

                boneRoot = gameObj.transform.Find("GladiusToUnity");
                if (boneRoot == null)
                {
                    boneRoot = gameObj.transform.GetChild(0);
                }
                else
                {
                    boneRoot = boneRoot.GetChild(0);
                }
            }

            NAMEChunk nameChunk = model.GetChunk<NAMEChunk>();
            SKELChunk skelChunk = model.GetChunk<SKELChunk>();
            RAMChunk ramChunk = model.GetChunk<RAMChunk>();
            ramChunk.RequestedRamSize = ramSize;

            byte boneId = 0;
            AnimationUtils.BuildSkeleton(boneRoot, null, ref boneId, skelChunk.BoneList);

            foreach (BoneNode boneNode in skelChunk.BoneList)
            {
                nameChunk.Names.Add(boneNode.name);
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                SkinData skinData = SkinBuilder.PrepareData(skinnedMeshRenderer, animShift,
                    model.GetChunk<SKELChunk>().BoneList.Count);

                if (skinData != null)
                {
                    model.AddSkinData(skinData);
                }

                foreach (Vector2 v in skinnedMeshRenderer.sharedMesh.uv)
                {
                    uniqueUVs.Add(v);
                }
            }

            int ibreak = 0;
        }
        else
        {
            foreach (MeshFilter meshFilter in meshFilters)
            {
                MeshRenderer meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
                if (meshFilter != null && meshRenderer != null)
                {
                    // setup core data

                    foreach (Vector3 v in meshFilter.sharedMesh.vertices)
                    {
                        adjustedVertices.Add(meshFilter.gameObject.transform.position + v);
                    }

                    foreach (Vector3 v in meshFilter.sharedMesh.normals)
                    {
                        adjustedNormals.Add(meshFilter.gameObject.transform.TransformDirection(v));
                    }

                    foreach (Vector2 v in meshFilter.sharedMesh.uv)
                    {
                        uniqueUVs.Add(v);
                    }
                }
            }

            foreach (Vector3 v in adjustedVertices)
            {
                model.AddUnskinnedPosition(v);
            }

            foreach (Vector3 v in adjustedNormals)
            {
                model.AddUnskinnedNormal(v);
            }
        }


        foreach (Vector2 v in uniqueUVs)
        {
            model.AddUV(v);
        }

        //int subObjectCount = 0;


        model.GetChunk<SELSChunk>().Names.Add(DefaultShader);

        List<Mesh> meshes = new List<Mesh>();
        List<GameObject> gameObjects = new List<GameObject>();
        List<Material> materials = new List<Material>();

        POSIChunk posiChunk = model.GetChunk<POSIChunk>();
        NORMChunk normChunk = model.GetChunk<NORMChunk>();
        UV0Chunk uv0Chunk = model.GetChunk<UV0Chunk>();
        TXTRChunk txtrChunk = model.GetChunk<TXTRChunk>();
        SELSChunk selsChunk = model.GetChunk<SELSChunk>();
        SHDRChunk shdrChunk = model.GetChunk<SHDRChunk>();

        if (skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0)
        {
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                meshes.Add(skinnedMeshRenderer.sharedMesh);
                gameObjects.Add(skinnedMeshRenderer.gameObject);
                materials.Add(skinnedMeshRenderer.sharedMaterial);
            }

            // Need to make bone lists here as well..
        }
        else
        {
            foreach (MeshFilter meshFilter in meshFilters)
            {
                MeshRenderer meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
                meshes.Add(meshFilter.sharedMesh);
                gameObjects.Add(meshFilter.gameObject);
                materials.Add(meshRenderer.sharedMaterial);
            }
        }

        int dsliStart = 0;

        SKINChunk skinChunk = model.GetChunk<SKINChunk>();

        List<Material> uniqueMaterials = new List<Material>();
        foreach (Material material in materials)
        {
            if (!uniqueMaterials.Contains(material))
            {
                uniqueMaterials.Add(material);
            }
        }


        for (int m = 0; m < meshes.Count; m++)
        {
            Mesh mesh = meshes[m];
            GameObject gameObject = gameObjects[m];
            Material material = materials[m];

            DisplayListHeader dlh = new DisplayListHeader();
            int[] triangleOrder = new[] { 0, 2, 1 };

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                foreach (int order in triangleOrder)
                {
                    int adjustedPoint = i + order;

                    int lookupIndex = mesh.triangles[adjustedPoint];
                    int posNormLookupIndex = lookupIndex;

                    if (model.IsSkinned())
                    {
                        int conversion = skinChunk.SkinDataList[m].RelocationTable[lookupIndex];
                        posNormLookupIndex = conversion;
                    }

                    Vector2 sharedMeshU = mesh.uv[lookupIndex];

                    int posIndex = posNormLookupIndex;
                    int normIndex = posNormLookupIndex;

                    int uvIndex = uv0Chunk.Data.IndexOf(sharedMeshU);

                    dlh.entries.Add(new DisplayListEntry((ushort)posIndex, (ushort)normIndex, (ushort)uvIndex));
                }

                //uniqueVertexIds.Add(posIndex);
            }

            dlh.indexCount = (ushort)dlh.entries.Count;

            DSLIInfo dsliInfo = new DSLIInfo();
            dsliInfo.startPos = dsliStart;
            dsliInfo.length = dlh.indexCount;
            dsliStart += dsliInfo.length;

            model.AddDSLIInfo(dsliInfo);
            model.AddDSLH(dlh);

            int materialIndex = uniqueMaterials.IndexOf(materials[m]);

            PaxElement paxElement = new PaxElement((uint)materialIndex, 0);
            paxElement.VertexCount = (uint)mesh.vertexCount;
            paxElement.SelectSetMask = (uint)lodLevel;
            model.AddPaxElement(paxElement);

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter binWriter = new BinaryWriter(ms))
                {
                    foreach (DisplayListHeader dslh in model.GetChunk<DSLSChunk>().DisplayListHeaders)
                    {
                        dslh.ToStream(binWriter);
                    }
                }

                ms.Flush();
                model.GetChunk<DSLSChunk>().Data = ms.GetBuffer();
            }
        }
        DSLCChunk dslcChunk = model.GetChunk<DSLCChunk>();
        DSLSChunk dslsChunk = model.GetChunk<DSLSChunk>();
        
        // one count per mesh.
        dslcChunk.DisplayListCounts.Clear();
        for (int i = 0; i < dslsChunk.DisplayListHeaders.Count; i++)
        {
            dslcChunk.DisplayListCounts.Add(1);
        }

        
        
        foreach (Material material in uniqueMaterials)
        {
            if (material != null && material.mainTexture != null)
            {
                string textureName = material.mainTexture.name;

                textureName += ".tga";
                textureName = textureName.ToLower();

                txtrChunk.Textures.Add(new PaxTexture()
                {
                    Name = textureName, Width = (uint)material.mainTexture.width,
                    Height = (uint)material.mainTexture.height
                });

                selsChunk.Names.Add(textureName);

                GCMaterial gcMaterial = new GCMaterial();
                gcMaterial.MatName = textureName;
                shdrChunk.Data.Add(gcMaterial);
            }
        }

        STYPChunk stypChunk = model.GetChunk<STYPChunk>();
        stypChunk.SelectSetList.Clear();
        for (int i = 0; i < selsChunk.Names.Count; i++)
        {
            stypChunk.SelectSetList.Add(new SelectSet(){NameIndex = (ushort)i,Mask=0});
        }

        model.GetChunk<RAMChunk>().Data = new byte[ramSize];
        

        return model;
    }

    public uint LodLevelForMesh(int meshIndex)
    {
        MESHChunk meshChunk = GetChunk<MESHChunk>();
        if (meshIndex < 0 || meshIndex >= meshChunk.NumElements)
        {
            return 0;
        }

        return meshChunk.PaxElements[meshIndex].SelectSetMask;
    }

    public int CountMeshesForLodLevel(uint lodLevel)
    {
        int count = 0;
        MESHChunk meshChunk = GetChunk<MESHChunk>();
        foreach (PaxElement paxElement in meshChunk.PaxElements)
        {
            //if (lodLevel == 0 || (paxElement.SelectSetMask & lodLevel) != 0)
            if ((paxElement.SelectSetMask & lodLevel) == lodLevel)
            {
                count++;
            }
        }

        return count;
    }

    public bool IsSkinned()
    {
        return GetChunk<SKELChunk>() != null && GetChunk<SKELChunk>().BoneList.Count > 0;
    }


    public CommonModelData ToCommon()
    {
        CommonModelData commonModelData = new CommonModelData();
        commonModelData.GCModel = this;

        commonModelData.Name = m_name;
        commonModelData.VertexDataLists = new List<VertexDataAndDesc>();
        commonModelData.IndexDataList = new List<List<int>>();


        SKELChunk skelChunk = GetChunk<SKELChunk>();
        if (skelChunk != null)
        {
            commonModelData.BoneList.AddRange(skelChunk.BoneList);
        }

        POSIChunk posiChunk = GetChunk<POSIChunk>();
        NORMChunk normChunk = GetChunk<NORMChunk>();
        UV0Chunk uv0Chunk = GetChunk<UV0Chunk>();
        DSLIChunk dsliChunk = GetChunk<DSLIChunk>();
        DSLSChunk dslsChunk = GetChunk<DSLSChunk>();
        DSLCChunk dslcChunk = GetChunk<DSLCChunk>();
        MESHChunk meshChunk = GetChunk<MESHChunk>();
        ELEMChunk elemChunk = GetChunk<ELEMChunk>();
        SKINChunk skinChunk = GetChunk<SKINChunk>();
        SELSChunk selsChunk = GetChunk<SELSChunk>();
        STYPChunk stypChunk = GetChunk<STYPChunk>();
        SHDRChunk shdrChunk = GetChunk<SHDRChunk>();
        TXTRChunk txtrChunk = GetChunk<TXTRChunk>();


        if (dsliChunk != null && dslsChunk != null)
        {
            // build approproate dsli chunk entries?

            //dslsChunk doesn't have the Data array set.

            if (dslsChunk.DisplayListHeaders.Count != dsliChunk.DisplayListIndices.Count)
            {
                dslsChunk.BuildData(dsliChunk);
            }
            
            //Debug.Assert(dslsChunk.DisplayListHeaders.Count == meshChunk.PaxElements.Count);

            VertexDataAndDesc vertexDataAndDesc = new VertexDataAndDesc();
            commonModelData.VertexDataLists.Add(vertexDataAndDesc);

            commonModelData.OverallLodLevel = CommonModelImporter.GetBestLodLevel(selsChunk, stypChunk);

            if (IsSkinned())
            {
                BuildSkinnedMesh(dslsChunk, dsliChunk,dslcChunk, meshChunk, skinChunk, uv0Chunk,
                    commonModelData.OverallLodLevel,
                    vertexDataAndDesc,commonModelData);
            }
            else
            {
                BuildUnskinnedMesh(dslsChunk,dsliChunk,dslcChunk,  meshChunk, posiChunk, normChunk, uv0Chunk,
                    vertexDataAndDesc,commonModelData);
            }
        }


        if (shdrChunk != null && txtrChunk != null)
        {
            foreach (GCMaterial gcMaterial in shdrChunk.Data)
            {
                CommonMaterialData commonMaterialData = new CommonMaterialData();
                commonModelData.CommonMaterials.Add(commonMaterialData);

                if (gcMaterial.TexIndex[0] != 0xFF)
                {
                    PaxTexture paxTexture = txtrChunk.Textures[gcMaterial.TexIndex[0]];
                    commonMaterialData.TextureData1 = paxTexture.ToCommon();
                    commonModelData.CommonTextures.Add(commonMaterialData.TextureData1);
                }

                if (gcMaterial.TexIndex[1] != 0xFF)
                {
                    PaxTexture paxTexture = txtrChunk.Textures[gcMaterial.TexIndex[1]];
                    commonMaterialData.TextureData2 = paxTexture.ToCommon();
                    commonModelData.CommonTextures.Add(commonMaterialData.TextureData2);
                }

                // if ((renderFlags & 0x04) != 0)
                //     cmd.isTransparent = true;
            }
        }


        foreach (VertexDataAndDesc vdad in commonModelData.VertexDataLists)
        {
            commonModelData.AllVertices.AddRange(vdad.VertexData);
        }


        return commonModelData;
    }

    private void BuildUnskinnedMesh(DSLSChunk dslsChunk,DSLIChunk dsliChunk,DSLCChunk dslcChunk, MESHChunk meshChunk,
        POSIChunk posiChunk, NORMChunk normChunk, UV0Chunk uv0Chunk, VertexDataAndDesc vertexDataAndDesc, CommonModelData commonModelData)
    {
        int displayListPosition = 0;

        for(int meshCount = 0;meshCount<meshChunk.NumElements;meshCount++)
        {
            for (int displayListCount = 0;
                 displayListCount < dslcChunk.DisplayListCounts[meshCount];
                 displayListCount++)
            {
                DisplayListHeader dlh = dslsChunk.DisplayListHeaders[displayListPosition];

                CommonMeshData commonMeshData = new CommonMeshData();
                commonModelData.CommonMeshData.Add(commonMeshData);

                commonMeshData.Name = m_name;
                commonMeshData.Index = meshCount;
                commonMeshData.MaterialId = (int)meshChunk.PaxElements[meshCount].MaterialId;

                List<int> meshIndices = new List<int>();
                commonModelData.IndexDataList.Add(meshIndices);
                commonMeshData.Indices = meshIndices;


                for (int i = 0; i < dlh.entries.Count; i++)
                {
                    DisplayListEntry entry = dlh.entries[i];

                    CommonVertexInstance cvi = new CommonVertexInstance();
                    cvi.Position = posiChunk.Data[entry.PosIndex];
                    cvi.Normal = normChunk.Data[entry.NormIndex];
                    cvi.UV = uv0Chunk.Data[entry.UVIndex];


                    int vertexIndex = vertexDataAndDesc.VertexData.IndexOf(cvi);
                    if (vertexIndex == -1)
                    {
                        vertexDataAndDesc.VertexData.Add(cvi);
                        vertexIndex = vertexDataAndDesc.VertexData.Count - 1;
                    }

                    if (!commonMeshData.Vertices.Contains(vertexIndex))
                    {
                        commonMeshData.Vertices.Add(vertexIndex);
                    }

                    int localIndex = commonMeshData.Vertices.IndexOf(vertexIndex);
                    ;
                    meshIndices.Add(localIndex);
                }

                // change winding order on indices.
                for (int i = 0; i < meshIndices.Count; i += 3)
                {
                    //tempTriangles[i] = submesh.Indices[i];
                    int temp = meshIndices[i + 1];
                    meshIndices[i + 1] = meshIndices[i + 2];
                    meshIndices[i + 2] = temp;
                }

                displayListPosition++;
            }
        }
    }

    private void BuildSkinnedMesh(DSLSChunk dslsChunk, DSLIChunk dsliChunk,DSLCChunk dslcChunk, MESHChunk meshChunk,
        SKINChunk skinChunk, UV0Chunk uv0Chunk, uint lodLevel, VertexDataAndDesc vertexDataAndDesc, CommonModelData commonModelData)
    {
        int vertexCount = 0;
        int displayListPosition = 0;

        for(int meshCount = 0;meshCount<meshChunk.NumElements;meshCount++)
        {
            for (int displayListCount = 0;
                 displayListCount < dslcChunk.DisplayListCounts[meshCount];
                 displayListCount++)
            {
                DisplayListHeader dlh = dslsChunk.DisplayListHeaders[displayListPosition];

                int maxPositionIndex = -1;
                foreach (DisplayListEntry entry in dlh.entries)
                {
                    maxPositionIndex = Math.Max(maxPositionIndex, entry.PosIndex);
                }

                // zero based.
                maxPositionIndex += 1;

                uint mask = meshChunk.PaxElements[meshCount].SelectSetMask;
                if ((mask & lodLevel) == lodLevel)
                {
                    CommonMeshData commonMeshData = new CommonMeshData();
                    commonModelData.CommonMeshData.Add(commonMeshData);

                    commonMeshData.Name = m_name;
                    commonMeshData.Index = meshCount;
                    commonMeshData.MaterialId = (int)meshChunk.PaxElements[meshCount].MaterialId;
                    commonMeshData.LodLevel = lodLevel;

                    List<int> meshIndices = new List<int>();
                    commonModelData.IndexDataList.Add(meshIndices);
                    commonMeshData.Indices = meshIndices;

                    if (meshCount < 0 || meshCount >= skinChunk.SkinDataList.Count)
                    {
                        int ibreak = 0;
                    }

                    SkinData skinData = skinChunk.SkinDataList[meshCount];
                    List<(Vector3, List<(int, float)>)> positionAndWeights = new List<(Vector3, List<(int, float)>)>();
                    List<Vector3> normals = new List<Vector3>();
                    // build all the skin data into temp lists
                    foreach (CSK1 csk in skinData.CSK1List)
                    {
                        foreach (Vector3 v3 in csk.ExtractedPositions)
                        {
                            List<(int, float)> weights = new List<(int, float)>();
                            weights.Add((csk.idxBone, 1f));
                            positionAndWeights.Add((v3, weights));
                        }

                        foreach (Vector3 v3 in csk.ExtractedNormals)
                        {
                            normals.Add(v3);
                        }
                    }

                    foreach (CSK2 csk in skinData.CSK2List)
                    {
                        int count = 0;
                        foreach (Vector3 v3 in csk.ExtractedPositions)
                        {
                            List<(int, float)> weights = new List<(int, float)>();
                            (float, float) weight = csk.ExtractedWeightsFloats[count];

                            weights.Add((csk.idxBone[0], weight.Item1));
                            weights.Add((csk.idxBone[1], weight.Item2));

                            positionAndWeights.Add((v3, weights));
                            count++;
                        }

                        foreach (Vector3 v3 in csk.ExtractedNormals)
                        {
                            normals.Add(v3);
                        }
                    }

                    foreach (CSKA csk in skinData.CSKAList)
                    {
                        int n = csk.ExtractedDestinationIndices.Count;
                        for (int k = 0; k < n; k++)
                        {
                            int dstIndex = csk.ExtractedDestinationIndices[k];

                            positionAndWeights[csk.ExtractedDestinationIndices[k]].Item2
                                .Add((csk.idxBone, csk.ExtractedWeightsFloats[k]));
                        }
                    }

                    bool[] referencedPositions = new bool[maxPositionIndex];
                    int[] mappedPositions = new int[maxPositionIndex];

                    int count1 = 0;
                    int vertexIndex = -1;
                    for (int i = 0; i < dlh.entries.Count; i++)
                    {
                        DisplayListEntry entry = dlh.entries[i];
                        vertexIndex = dlh.entries[i].PosIndex;

                        if (entry.PosIndex >= referencedPositions.Length)
                        {
                            int ibreak = 0;
                        }

                        if (entry.PosIndex >= positionAndWeights.Count)
                        {
                            int ibreak = 0;
                        }


                        if (!referencedPositions[entry.PosIndex])
                        {
                            CommonVertexInstance cvi = new CommonVertexInstance();

                            cvi.DebugDLEPos = entry.PosIndex;
                            cvi.DebugDLENorm = entry.NormIndex;

                            cvi.Position = positionAndWeights[entry.PosIndex].Item1;
                            cvi.Normal = normals[entry.NormIndex];


                            if (entry.UVIndex < 0 || entry.UVIndex >= uv0Chunk.Data.Count)
                            {
                                int ibreak = 0;
                            }

                            cvi.UV = uv0Chunk.Data[entry.UVIndex];

                            List<(int, float)> weightsList = positionAndWeights[entry.PosIndex].Item2;

                            int numWeights = weightsList.Count;
                            float sum = 0.0f;
                            if (numWeights > 0)
                            {
                                cvi.BoneWeight.weight0 = weightsList[0].Item2;
                                cvi.BoneWeight.boneIndex0 = weightsList[0].Item1;
                                sum += cvi.BoneWeight.weight0;
                            }

                            if (numWeights > 1)
                            {
                                cvi.BoneWeight.weight1 = weightsList[1].Item2;
                                cvi.BoneWeight.boneIndex1 = weightsList[1].Item1;
                                sum += cvi.BoneWeight.weight1;
                            }

                            if (numWeights > 2)
                            {
                                cvi.BoneWeight.weight2 = weightsList[2].Item2;
                                cvi.BoneWeight.boneIndex2 = weightsList[2].Item1;
                                sum += cvi.BoneWeight.weight2;
                            }

                            if (numWeights > 3)
                            {
                                cvi.BoneWeight.weight3 = weightsList[3].Item2;
                                cvi.BoneWeight.boneIndex3 = weightsList[3].Item1;
                                sum += cvi.BoneWeight.weight3;
                            }

                            vertexDataAndDesc.VertexData.Add(cvi);
                            vertexIndex = vertexCount;

                            referencedPositions[entry.PosIndex] = true;
                            mappedPositions[entry.PosIndex] = vertexIndex;
                            vertexCount++;
                        }

                        vertexIndex = mappedPositions[entry.PosIndex];

                        // don't do this, instead use a dle index

                        if (!commonMeshData.Vertices.Contains(vertexIndex))
                        {
                            commonMeshData.Vertices.Add(vertexIndex);
                        }

                        int localIndex = commonMeshData.Vertices.IndexOf(vertexIndex);
                        ;
                        meshIndices.Add(localIndex);
                    }

                    int ibreak2 = 0;
                    // change winding order on indices.
                    for (int i = 0; i < meshIndices.Count; i += 3)
                    {
                        int temp = meshIndices[i + 1];
                        meshIndices[i + 1] = meshIndices[i + 2];
                        meshIndices[i + 2] = temp;
                    }
                }

                displayListPosition++;
            }
            
        }
    }


    public void BuildMaterialData(GameObject go, GCModel model)
    {
        TXTRChunk txtrChunk = model.GetChunk<TXTRChunk>();
        SELSChunk selsChunk = model.GetChunk<SELSChunk>();

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


            txtrChunk.Textures.Add(new PaxTexture()
                { Name = textureName, Width = (uint)m.mainTexture.width, Height = (uint)m.mainTexture.height });

            selsChunk.Names.Add(DefaultShader);
            selsChunk.Names.Add(textureName);
        }
    }

    public static GCModel ReadData(BinaryReader binReader, string name, StringBuilder debugInfo)
    {
        GCModel gcModel = new GCModel(name);
        gcModel.m_chunkList.Clear();

        binReader.BaseStream.Position = 0;
        int count = 0;

        do
        {
            int position = (int)binReader.BaseStream.Position;
            BaseChunk chunk = BaseChunk.FromStreamMaster(name, binReader, debugInfo);
            if (chunk != null)
            {
                gcModel.m_chunkList.Add(chunk);
                chunk.ChunkList = gcModel.m_chunkList;

                if (chunk is ENDChunk)
                {
                    break;
                }

                binReader.BaseStream.Position = position + chunk.Length;
            }
        } while (count++ < 100);


        bool removePaddChunks = false;
        if (removePaddChunks)
        {
            gcModel.m_chunkList.RemoveAll(x => x is PADDChunk);
        }


        SKELChunk skelChunk = gcModel.GetChunk<SKELChunk>();
        NAMEChunk nameChunk = gcModel.GetChunk<NAMEChunk>();
        if (skelChunk != null)
        {
            foreach (BoneNode bn in skelChunk.BoneList)
            {
                bn.name = nameChunk.Names[bn.NameIndex];
                if (bn.Index != bn.ParentIndex)
                {
                    bn.parent = skelChunk.BoneList[bn.ParentIndex];
                }
            }
        }

        DSLSChunk dslsChunk = gcModel.GetChunk<DSLSChunk>();
        DSLIChunk dsliChunk = gcModel.GetChunk<DSLIChunk>();

        if (dslsChunk.DisplayListHeaders.Count != dsliChunk.DisplayListIndices.Count)
        {
            dslsChunk.BuildData(dsliChunk);
        }


        return gcModel;
    }


    public void ConstructSkin(GCModel model)
    {
    }


    public void BuildStandardMesh(List<int> indices, List<Vector3> points, List<Vector3> normals, List<Vector2> uvs)
    {
        DSLSChunk dslsChunk = GetChunk<DSLSChunk>();
        POSIChunk posiChunk = GetChunk<POSIChunk>();
        NORMChunk normChunk = GetChunk<NORMChunk>();
        UV0Chunk uv0Chunk = GetChunk<UV0Chunk>();

        foreach (DisplayListHeader dlh in dslsChunk.DisplayListHeaders)
        {
            int counter = 0;
            for (int i = 0; i < dlh.entries.Count; i++)
            {
                DisplayListEntry entry = dlh.entries[i];

                points.Add(posiChunk.Data[entry.PosIndex]);
                normals.Add(normChunk.Data[entry.NormIndex]);
                uvs.Add(uv0Chunk.Data[entry.UVIndex]);
                indices.Add(counter);
                counter++;
            }
        }
    }


    public void ToStreamRawData(BinaryWriter binWriter)
    {
        GetChunk<VERSChunk>().ToStreamRawData(binWriter);

        GetChunk<CPRTChunk>().ToStreamRawData(binWriter);

        GetChunk<SELSChunk>().ToStreamRawData(binWriter);

        GetChunk<DSLCChunk>().ToStreamRawData(binWriter);
        if (IsSkinned())
        {
            GetChunk<RAMChunk>()?.ToStreamRawData(binWriter);
        }

        GetChunk<SHDRChunk>().ToStreamRawData(binWriter);


        GetChunk<TXTRChunk>().ToStreamRawData(binWriter);


        GetChunk<MESHChunk>()?.ToStreamRawData(binWriter);


        GetChunk<ELEMChunk>()?.ToStreamRawData(binWriter);
        new PADDChunk().ToStream(binWriter);

        GetChunk<DSLSChunk>().ToStreamRawData(binWriter);


        GetChunk<DSLIChunk>().ToStreamRawData(binWriter);

        new PADDChunk().ToStream(binWriter);

        GetChunk<UV0Chunk>()?.ToStreamRawData(binWriter);


        GetChunk<VFLGChunk>()?.ToStreamRawData(binWriter);

        GetChunk<STYPChunk>()?.ToStreamRawData(binWriter);

        if (IsSkinned())
        {
            GetChunk<NAMEChunk>()?.ToStreamRawData(binWriter);
            GetChunk<SKELChunk>()?.ToStreamRawData(binWriter);
            GetChunk<JLODChunk>()?.ToStreamRawData(binWriter);
        }
        else
        {
            GetChunk<POSIChunk>()?.ToStreamRawData(binWriter);
            GetChunk<NORMChunk>()?.ToStreamRawData(binWriter);
        }

        GetChunk<CNTRChunk>().ToStreamRawData(binWriter);

        new PADDChunk().ToStream(binWriter);

        if (IsSkinned())
        {
            GetChunk<SKINChunk>()?.ToStreamRawData(binWriter);
        }


        // GetChunk<VFLAChunk>()?.ToStream(binWriter);
        // GetChunk<MSARChunk>()?.ToStream(binWriter);
        // GetChunk<NLVLChunk>()?.ToStream(binWriter);

        GetChunk<ENDChunk>().ToStreamRawData(binWriter);
    }

    public void ToStream(BinaryWriter binWriter)
    {
        GetChunk<VERSChunk>().ToStream(binWriter);
        GetChunk<CPRTChunk>().ToStream(binWriter);
        GetChunk<SELSChunk>().ToStream(binWriter);
        GetChunk<DSLCChunk>().ToStream(binWriter);

        if (IsSkinned())
        {
            if (GetChunk<RAMChunk>().Data.Length == 0)
            {
                GetChunk<RAMChunk>().Data = new byte[GetChunk<RAMChunk>().RequestedRamSize];
            }

            GetChunk<RAMChunk>()?.ToStream(binWriter);
        }

        // these are good with no crashes
        GetChunk<SHDRChunk>().ToStream(binWriter);
        GetChunk<TXTRChunk>().ToStream(binWriter);
        GetChunk<MESHChunk>()?.ToStream(binWriter);
        GetChunk<ELEMChunk>()?.ToStream(binWriter);
        

        // never on 32 boundary?
        if (binWriter.BaseStream.Position % 32 == 0)
        {
            new PADDChunk().ToStream(binWriter);
        }
        GetChunk<DSLSChunk>().ToStream(binWriter);
        
        // always on 32 boundary?
        if (binWriter.BaseStream.Position % 32 != 0)
        {
            new PADDChunk().ToStream(binWriter);
        }
        
        GetChunk<DSLIChunk>().ToStream(binWriter);
        
        //GetChunk<DSLIChunk>().ToStreamRawData(binWriter);



        // never on 32 boundary?
        if (binWriter.BaseStream.Position % 32 == 0)
        {
            new PADDChunk().ToStream(binWriter);
        }
        GetChunk<UV0Chunk>()?.ToStream(binWriter);
        

        GetChunk<VFLGChunk>()?.ToStream(binWriter);

        //GetChunk<STYPChunk>()?.ToStream(binWriter);

        if (IsSkinned())
        {
            GetChunk<NAMEChunk>()?.ToStream(binWriter);
            GetChunk<SKELChunk>()?.ToStream(binWriter);
            //GetChunk<JLODChunk>()?.ToStream(binWriter);
        }
        else
        {
            GetChunk<POSIChunk>()?.ToStream(binWriter);
            GetChunk<NORMChunk>()?.ToStream(binWriter);
        }

        GetChunk<CNTRChunk>().ToStream(binWriter);
        

        if (IsSkinned())
        {
            // never on 32 boundary?
            if (binWriter.BaseStream.Position % 32 == 0)
            {
                new PADDChunk().ToStream(binWriter);
            }
            GetChunk<SKINChunk>()?.ToStream(binWriter);
        }


        // GetChunk<VFLAChunk>()?.ToStream(binWriter);
        // GetChunk<MSARChunk>()?.ToStream(binWriter);
        // GetChunk<NLVLChunk>()?.ToStream(binWriter);

        GetChunk<ENDChunk>().ToStream(binWriter);

        foreach (BaseChunk chunk in m_chunkList)
        {
            if (chunk.WriteStart != 0 || chunk.WriteEnd != 0)
            {
                long diff = chunk.WriteEnd - chunk.WriteStart;
                if (diff != chunk.Length)
                {
                    int ibreak = 0;
                }
            }
        }
    }


    public void ToStreamPADD(BinaryWriter binWriter)
    {
        GetChunk<VERSChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);
        GetChunk<CPRTChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);
        GetChunk<SELSChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);
        GetChunk<DSLCChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);
        if (IsSkinned())
        {
            if (GetChunk<RAMChunk>().Data.Length == 0)
            {
                GetChunk<RAMChunk>().Data = new byte[GetChunk<RAMChunk>().RequestedRamSize];
            }

            GetChunk<RAMChunk>()?.ToStream(binWriter);
            GladiusFileWriter.AddPadChunkAlign32(binWriter);
        }

        GetChunk<SHDRChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        GetChunk<TXTRChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        GetChunk<MESHChunk>()?.ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        GetChunk<ELEMChunk>()?.ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        GetChunk<DSLSChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);
        GetChunk<DSLIChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        GetChunk<UV0Chunk>()?.ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        GetChunk<VFLGChunk>()?.ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        GetChunk<STYPChunk>()?.ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);
        
        if (IsSkinned())
        {
            GetChunk<NAMEChunk>()?.ToStream(binWriter);
            GladiusFileWriter.AddPadChunkAlign32(binWriter);
            GetChunk<SKELChunk>()?.ToStream(binWriter);
            GladiusFileWriter.AddPadChunkAlign32(binWriter);
            GetChunk<JLODChunk>()?.ToStream(binWriter);
            GladiusFileWriter.AddPadChunkAlign32(binWriter);
        }
        else
        {
            GetChunk<POSIChunk>()?.ToStream(binWriter);
            GladiusFileWriter.AddPadChunkAlign32(binWriter);
            GetChunk<NORMChunk>()?.ToStream(binWriter);
            GladiusFileWriter.AddPadChunkAlign32(binWriter);
        }

        GetChunk<CNTRChunk>().ToStream(binWriter);
        GladiusFileWriter.AddPadChunkAlign32(binWriter);
        //GetChunk<CNTRChunk>().ToStreamRawData(binWriter);
        
        

        if (IsSkinned())
        {
            GetChunk<SKINChunk>()?.ToStream(binWriter);
        }
        GladiusFileWriter.AddPadChunkAlign32(binWriter);

        // GetChunk<VFLAChunk>()?.ToStream(binWriter);
        // GetChunk<MSARChunk>()?.ToStream(binWriter);
        // GetChunk<NLVLChunk>()?.ToStream(binWriter);

        GetChunk<ENDChunk>().ToStream(binWriter);

        foreach (BaseChunk chunk in m_chunkList)
        {
            if (chunk.WriteStart != 0 || chunk.WriteEnd != 0)
            {
                long diff = chunk.WriteEnd - chunk.WriteStart;
                if (diff != chunk.Length)
                {
                    int ibreak = 0;
                }
            }
        }
    }

    
    public void AddSkinData(SkinData skinData)
    {
        GetChunk<SKINChunk>().SkinDataList.Add(skinData);
    }

    public void AddUnskinnedPosition(Vector3 position)
    {
        GetChunk<POSIChunk>().Data.Add(position);
    }

    public void AddUnskinnedNormal(Vector3 normal)
    {
        GetChunk<NORMChunk>().Data.Add(normal);
    }

    public void AddUV(Vector2 uv)
    {
        GetChunk<UV0Chunk>().Data.Add(uv);
    }

    public void AddDSLIInfo(DSLIInfo dsli)
    {
        GetChunk<DSLIChunk>().DisplayListIndices.Add(dsli);
    }

    public void AddDSLH(DisplayListHeader dslh)
    {
        GetChunk<DSLSChunk>().DisplayListHeaders.Add(dslh);
    }

    public void AddPaxElement(PaxElement paxElement)
    {
        GetChunk<MESHChunk>().PaxElements.Add(paxElement);
    }
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
    public const int DefaultEntryStride = 6;
    public const int ExtendedEntryStride = 7;

    public byte header1 = 0x98;
    public ushort pad1 = 0;
    public byte primitiveFlags = 0x90;
    public ushort indexCount;
    public int entryStride = DefaultEntryStride;


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

    public static bool FromStream(BinaryReader reader, out DisplayListHeader header, DSLIInfo dsliInfo,
        int forcedEntryStride = DefaultEntryStride)
    {
        long currentPosition = reader.BaseStream.Position;
        bool success = false;

        header = new DisplayListHeader();

        header.header1 = reader.ReadByte();
        header.pad1 = reader.ReadUInt16();
        header.primitiveFlags = reader.ReadByte();

        if (forcedEntryStride == 0)
        {
            forcedEntryStride = DetectEntryStride(reader, currentPosition, header.indexCount, dsliInfo);
        }

        header.entryStride = forcedEntryStride;


        if (header.primitiveFlags == 0x90 || header.primitiveFlags == 0x00)
        {
            header.indexCount = Common.ToUInt16BigEndian(reader);

            success = true;
            for (int i = 0; i < header.indexCount; ++i)
            {
                header.entries.Add(DisplayListEntry.FromStream(reader, header.entryStride));
            }
        }
        else
        {
            reader.BaseStream.Position = currentPosition;
        }

        return success;
    }


    public static int DetectEntryStride(BinaryReader reader, long displayListStart, int entryCount, DSLIInfo dsliInfo)
    {
        bool stride6Boundary =
            LooksLikeDisplayListBoundary(reader, displayListStart, entryCount, dsliInfo, DefaultEntryStride);
        bool stride7Boundary =
            LooksLikeDisplayListBoundary(reader, displayListStart, entryCount, dsliInfo, ExtendedEntryStride);
        return stride7Boundary && !stride6Boundary ? ExtendedEntryStride : DefaultEntryStride;
    }

    public static bool LooksLikeDisplayListBoundary(BinaryReader reader, long displayListStart, int entryCount,
        DSLIInfo dsliInfo, int stride)
    {
        if (entryCount < 0)
        {
            return false;
        }

        long streamLength = reader.BaseStream.Length;
        long displayListEnd = streamLength;
        if (dsliInfo != null && dsliInfo.length > 0)
        {
            long dsliEnd = displayListStart + ((long)dsliInfo.length * 2L);
            if (dsliEnd > displayListStart && dsliEnd < displayListEnd)
            {
                displayListEnd = dsliEnd;
            }
        }

        long entryEnd = displayListStart + 6L + ((long)entryCount * stride);
        if (entryEnd < displayListStart || entryEnd > displayListEnd || entryEnd > streamLength)
        {
            return false;
        }

        long oldPosition = reader.BaseStream.Position;
        try
        {
            reader.BaseStream.Position = entryEnd;
            int sampleLength = (int)Math.Min(16, Math.Min(displayListEnd, streamLength) - entryEnd);
            if (sampleLength <= 0)
            {
                return true;
            }

            int zeroCount = 0;
            for (int i = 0; i < sampleLength; ++i)
            {
                byte value = reader.ReadByte();
                if (value == 0)
                {
                    ++zeroCount;
                }
                else if (i == 0 && IsGcPrimitiveCommand(value))
                {
                    return true;
                }
            }

            return zeroCount >= Math.Min(8, sampleLength);
        }
        finally
        {
            reader.BaseStream.Position = oldPosition;
        }
    }

    private static bool IsGcPrimitiveCommand(byte value)
    {
        return value == 0x80 || value == 0x90 || value == 0x98 || value == 0xA0 ||
               value == 0xA8 || value == 0xB0 || value == 0xB8;
    }


    public static DisplayListHeader CreateFromMeshData(int[] triangles, Vector3[] vertices,
        Vector3[] normals, Vector2[] uvs)
    {
        DisplayListHeader dlh = new DisplayListHeader();
        for (int i = 0; i < triangles.Length; i++)
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
    public byte ExtraIndex;
    public ushort UVIndex;

    public String ToString()
    {
        return "P:" + PosIndex + " N:" + NormIndex + " X:" + ExtraIndex + " U:" + UVIndex;
    }

    public DisplayListEntry(ushort index)
    {
        PosIndex = index;
        NormIndex = index;
        ExtraIndex = 0;
        UVIndex = index;
    }

    public DisplayListEntry(ushort pos, ushort norm, ushort uv)
    {
        PosIndex = pos;
        NormIndex = norm;
        ExtraIndex = 0;
        UVIndex = uv;
    }


    public void ToStream(BinaryWriter writer)
    {
        ToStream(writer, 6);
    }

    public void ToStream(BinaryWriter writer, int entryStride)
    {
        Common.WriteBigEndian(writer, (short)PosIndex);
        Common.WriteBigEndian(writer, (short)NormIndex);
        if (entryStride == DisplayListHeader.ExtendedEntryStride)
        {
            writer.Write(ExtraIndex);
        }

        Common.WriteBigEndian(writer, (short)UVIndex);
    }

    public static DisplayListEntry FromStream(BinaryReader reader)
    {
        return FromStream(reader, DisplayListHeader.DefaultEntryStride);
    }

    public static DisplayListEntry FromStream(BinaryReader reader, int entryStride)
    {
        DisplayListEntry entry = new DisplayListEntry();
        entry.PosIndex = Common.ToUInt16BigEndian(reader);
        entry.NormIndex = Common.ToUInt16BigEndian(reader);
        if (entryStride == DisplayListHeader.ExtendedEntryStride)
        {
            entry.ExtraIndex = reader.ReadByte();
        }

        entry.UVIndex = Common.ToUInt16BigEndian(reader);
        return entry;
    }
}


public class DSLIInfo
{
    public int startPos;
    public int length;

    public static DSLIInfo FromStream(BinaryReader reader)
    {
        DSLIInfo info = new DSLIInfo();

        info.startPos = Common.ReadInt32BigEndian(reader);
        info.length = Common.ReadInt32BigEndian(reader);

        return info;
    }

    public void ToStream(BinaryWriter binWriter)
    {
        Common.WriteBigEndian(binWriter, startPos);
        Common.WriteBigEndian(binWriter, length);
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


public class GCMaterial
{
    public const uint RawSize = 164;

    public int MaterialId = -1;
    public char[] MatNameRaw = new char[124];
    public string MatName;
    public uint SelectSetMask;
    public uint AttributeFlags;
    public uint AttributeValues;
    public byte[] TexIndex = new byte[8];
    public byte[] BlendModes = new byte[8];
    public byte[] GenModes = new byte[8];


    public static GCMaterial FromStream(BinaryReader reader)
    {
        GCMaterial gcm = new GCMaterial();
        gcm.MaterialId = reader.ReadInt32();
        gcm.MatNameRaw = reader.ReadChars(124);

        gcm.SelectSetMask = reader.ReadUInt32();
        gcm.AttributeFlags = reader.ReadUInt32();
        gcm.AttributeValues = reader.ReadUInt32();

        gcm.TexIndex = reader.ReadBytes(8);
        gcm.BlendModes = reader.ReadBytes(8);
        gcm.GenModes = reader.ReadBytes(8);

        gcm.MatName = new string(gcm.MatNameRaw);

        return gcm;
    }

    public void ToStream(BinaryWriter writer)
    {
        writer.Write(MaterialId);
        GladiusFileWriter.WriteASCIIString(writer, MatName, 124);

        writer.Write(SelectSetMask);
        writer.Write(AttributeFlags);
        writer.Write(AttributeValues);

        writer.Write(TexIndex);
        writer.Write(BlendModes);
        writer.Write(GenModes);
    }
}