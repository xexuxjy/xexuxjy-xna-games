using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Security.Policy;
using System.Text;
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


    public GCModel(String name) : base(name)
    {
        m_name = name;
        m_chunkList.Add(new VERSChunk());
        m_chunkList.Add(new CPRTChunk());
        m_chunkList.Add(new SELSChunk());
        m_chunkList.Add(new CNTRChunk());
        m_chunkList.Add(new SHDRChunk());
        m_chunkList.Add(new TXTRChunk());
        m_chunkList.Add(new DSLSChunk());
        m_chunkList.Add(new DSLIChunk());
        m_chunkList.Add(new DSLCChunk());
        m_chunkList.Add(new UV0Chunk());
        m_chunkList.Add(new SKINChunk());
        m_chunkList.Add(new SKELChunk());
        m_chunkList.Add(new VFLAChunk());
        m_chunkList.Add(new RAMChunk());
        m_chunkList.Add(new MSARChunk());
        m_chunkList.Add(new NLVLChunk());
        m_chunkList.Add(new MESHChunk());
        m_chunkList.Add(new ELEMChunk());
        m_chunkList.Add(new ENDChunk());
    }

    public static GCModel CreateFromGameObject(GameObject gameObj, short animShift)
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


        if (skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0)
        {
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                SkinData skinData = SkinBuilder.PrepareData(skinnedMeshRenderer, animShift);
                    
                if (skinData != null)
                {
                    model.AddSkinData(skinData);
                }
            }

            // build a skeleton from rootBone.

            byte boneId = 0;
            AnimationUtils.BuildSkeleton(gameObj.transform, null, ref boneId, model.GetChunk<SKELChunk>().BoneList);
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

        int subObjectCount = 0;


        model.GetChunk<SELSChunk>().Names.Add(DefaultShader);

        List<Mesh> meshes = new List<Mesh>();
        List<GameObject> gameObjects = new List<GameObject>();
        List<Material> materials = new List<Material>();

        POSIChunk posiChunk = model.GetChunk<POSIChunk>();
        NORMChunk normChunk = model.GetChunk<NORMChunk>();
        UV0Chunk uv0Chunk = model.GetChunk<UV0Chunk>();
        TXTRChunk txtrChunk = model.GetChunk<TXTRChunk>();
        SELSChunk selsChunk = model.GetChunk<SELSChunk>();

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

            // go through and adjust positions and normals now that the lists have been built
            for (int i = 0; i < posiChunk.Data.Count; ++i)
            {
                IndexedVector3 adjusted = posiChunk.Data[i];
                adjusted = gameObj.transform.TransformPoint(adjusted);
                adjusted -= offset;
                posiChunk.Data[i] = GladiusGlobals.UnityToGladius(adjusted);
            }

            for (int i = 0; i < normChunk.Data.Count; ++i)
            {
                normChunk.Data[i] =
                    GladiusGlobals.UnityToGladius(gameObj.transform.TransformDirection(normChunk.Data[i]));
            }
        }

        for (int m = 0; m < meshes.Count; m++)
        {
            Mesh mesh = meshes[m];
            GameObject gameObject = gameObjects[m];
            Material material = materials[m];

            List<Vector3> vertices = model.IsSkinned()
                ? model.GetChunk<SKINChunk>().Positions
                : model.GetChunk<POSIChunk>().Data;
            List<Vector3> normals =
                model.IsSkinned() ? model.GetChunk<SKINChunk>().Normals : model.GetChunk<NORMChunk>().Data;


            DisplayListHeader dlh = new DisplayListHeader();
            int[] triangleOrder = new[] { 0, 2, 1 };
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                foreach (int order in triangleOrder)
                {
                    int adjustedPoint = i + order;

                    int lookupIndex = mesh.triangles[adjustedPoint];

                    Vector3 sharedMeshV = mesh.vertices[lookupIndex] + gameObject.transform.position;
                    Vector3 sharedMeshN = gameObject.transform.TransformDirection(
                        mesh.normals[lookupIndex]);
                    Vector2 sharedMeshU = mesh.uv[lookupIndex];

                    int posIndex = lookupIndex;
                    int normIndex = lookupIndex;

                    int uvIndex = uv0Chunk.Data.IndexOf(sharedMeshU);

                    dlh.entries.Add(new DisplayListEntry((ushort)posIndex, (ushort)normIndex, (ushort)uvIndex));
                }

                //uniqueVertexIds.Add(posIndex);
            }

            dlh.indexCount = (ushort)dlh.entries.Count;


            model.AddDSLH(dlh);

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
            }

            PaxElement paxElement = new PaxElement((uint)subObjectCount, 0);
            paxElement.VertexCount = (uint)mesh.vertexCount;


            model.AddPaxElement(paxElement);

            subObjectCount++;
        }


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
            if (lodLevel == 0 || (paxElement.SelectSetMask & lodLevel) != 0)
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
            dslsChunk.BuildData(dsliChunk);
            Debug.Assert(dslsChunk.DisplayListHeaders.Count == meshChunk.PaxElements.Count);

            VertexDataAndDesc vertexDataAndDesc = new VertexDataAndDesc();
            commonModelData.VertexDataLists.Add(vertexDataAndDesc);

            commonModelData.OverallLodLevel = CommonModelImporter.GetBestLodLevel(selsChunk, stypChunk);

            if (skinChunk != null)
            {
                BuildSkinnedMesh(dslsChunk, commonModelData, meshChunk, skinChunk, uv0Chunk,
                    commonModelData.OverallLodLevel,
                    vertexDataAndDesc);
            }
            else
            {
                BuildUnskinnedMesh(dslsChunk, commonModelData, meshChunk, posiChunk, normChunk, uv0Chunk,
                    vertexDataAndDesc);
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

    private void BuildUnskinnedMesh(DSLSChunk dslsChunk, CommonModelData commonModelData, MESHChunk meshChunk,
        POSIChunk posiChunk, NORMChunk normChunk, UV0Chunk uv0Chunk, VertexDataAndDesc vertexDataAndDesc)
    {
        int meshCount = 0;

        foreach (DisplayListHeader dlh in dslsChunk.DisplayListHeaders)
        {
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
                cvi.Position = GladiusGlobals.GladiusToUnity(posiChunk.Data[entry.PosIndex]);
                cvi.Normal = GladiusGlobals.GladiusToUnity(normChunk.Data[entry.NormIndex]);
                //cvi.Normal = normChunk.Data[entry.NormIndex];
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

            meshCount++;
        }
    }

    private void BuildSkinnedMesh(DSLSChunk dslsChunk, CommonModelData commonModelData, MESHChunk meshChunk,
        SKINChunk skinChunk, UV0Chunk uv0Chunk, uint lodLevel, VertexDataAndDesc vertexDataAndDesc)
    {
        int meshCount = 0;
        int vertexCount = 0;

        //using (StreamWriter sw = new StreamWriter(new FileStream("d:/tmp/skin-data.txt", FileMode.OpenOrCreate)))
        {
            foreach (DisplayListHeader dlh in dslsChunk.DisplayListHeaders)
            {
                int maxPositionIndex = -1;
                foreach (DisplayListEntry entry in dlh.entries)
                {
                    maxPositionIndex = Math.Max(maxPositionIndex, entry.PosIndex);
                }

                // zero based.
                maxPositionIndex += 1;

                uint mask = meshChunk.PaxElements[meshCount].SelectSetMask;
                if (mask == 0 || (mask & lodLevel) != 0)
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
                        //int n = Mathf.Min(cska.count, Mathf.Min(cska.ExtractedDestinationIndices.Count, cska.ExtractedWeights.Count));
                        int n = csk.ExtractedDestinationIndices.Count;
                        for (int k = 0; k < n; k++)
                        {
                            // var foundVal = positionAndWeights.Find(x => x.Item1 == csk.ExtractedPositions[k]);
                            // int foundIndex = positionAndWeights.IndexOf(foundVal);


                            int dstIndex = csk.ExtractedDestinationIndices[k];
                            // if (dstIndex < 0 || dstIndex >= totalVerts)
                            //     continue;

                            //int count0 = positionAndWeights[csk.ExtractedDestinationIndices[k]].Item2.Count;

                            positionAndWeights[csk.ExtractedDestinationIndices[k]].Item2
                                .Add((csk.idxBone, csk.ExtractedWeights[k]));

                            //int count1 = positionAndWeights[csk.ExtractedDestinationIndices[k]].Item2.Count;
                            int ibreak = 0;
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

                        if (dlh.entries[i].PosIndex >= referencedPositions.Length)
                        {
                            int ibreak = 0;
                        }

                        if (!referencedPositions[entry.PosIndex])
                        {
                            CommonVertexInstance cvi = new CommonVertexInstance();

                            cvi.DebugDLEPos = entry.PosIndex;
                            cvi.DebugDLENorm = entry.NormIndex;


                            cvi.Position = GladiusGlobals.GladiusToUnity(positionAndWeights[entry.PosIndex].Item1);
                            cvi.Normal = GladiusGlobals.GladiusToUnity(normals[entry.NormIndex]);
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
                        //tempTriangles[i] = submesh.Indices[i];
                        int temp = meshIndices[i + 1];
                        meshIndices[i + 1] = meshIndices[i + 2];
                        meshIndices[i + 2] = temp;
                    }
                }

                meshCount++;
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

                if (chunk is ENDChunk)
                {
                    break;
                }

                binReader.BaseStream.Position = position + chunk.Length;
            }
        } while (count++ < 100);


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
            //BoneList.AddRange(skelChunk.BoneList);
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


    public void WriteData(BinaryWriter binWriter)
    {
        GetChunk<VERSChunk>().ToStream(binWriter);
        GetChunk<CPRTChunk>().ToStream(binWriter);
        GetChunk<SELSChunk>().ToStream(binWriter);
        GetChunk<CNTRChunk>().ToStream(binWriter,
            IsSkinned() ? GetChunk<SKINChunk>().Positions : GetChunk<POSIChunk>().Data);
        GetChunk<SHDRChunk>().ToStream(binWriter, GetChunk<TXTRChunk>().Textures);
        GetChunk<TXTRChunk>().ToStream(binWriter);
        GetChunk<DSLSChunk>().ToStream(binWriter);
        GetChunk<DSLIChunk>().ToStream(binWriter);
        GetChunk<DSLCChunk>().ToStream(binWriter, GetChunk<MESHChunk>().PaxElements);

        if (IsSkinned())
        {
            GetChunk<SKINChunk>().ToStream(binWriter);
        }
        else
        {
            GetChunk<POSIChunk>().ToStream(binWriter);
            GetChunk<NORMChunk>().ToStream(binWriter);
        }

        GetChunk<UV0Chunk>().ToStream(binWriter);
        GetChunk<VFLAChunk>().ToStream(binWriter);
        GetChunk<RAMChunk>().ToStream(binWriter);
        GetChunk<MSARChunk>().ToStream(binWriter);
        GetChunk<NLVLChunk>().ToStream(binWriter);
        GetChunk<MESHChunk>().ToStream(binWriter);
        GetChunk<ELEMChunk>().ToStream(binWriter, GetChunk<DSLSChunk>().DisplayListHeaders);
        GetChunk<ENDChunk>().ToStream(binWriter);
    }

    // public void WriteDSLI(BinaryWriter writer)
    // {
    //     int total = GladiusFileWriter.HeaderSize;
    //     total += (8 * m_displayListHeaders.Count); // (start,length for each
    //
    //     int paddedTotal = GladiusFileWriter.GetPadValue(total);
    //
    //
    //     GladiusFileWriter.WriteASCIIString(writer, "DSLI");
    //     writer.Write(paddedTotal); // block size
    //     writer.Write(1);
    //     writer.Write(1);
    //
    //
    //     int startPos = 0;
    //
    //     foreach (DisplayListHeader header in m_displayListHeaders)
    //     {
    //         DSLIInfo dsliInfo = new DSLIInfo();
    //         dsliInfo.startPos = startPos;
    //         dsliInfo.length = header.GetSize();
    //         ;
    //
    //         Common.WriteBigEndian(writer, dsliInfo.startPos);
    //         Common.WriteBigEndian(writer, dsliInfo.length);
    //
    //         startPos += dsliInfo.length;
    //     }
    //
    //     GladiusFileWriter.WriteNull(writer, (paddedTotal - total));
    //     //writer.Write(0);
    //     //writer.Write(0);
    // }

    // public void WriteDSLC(BinaryWriter writer)
    // {
    //     int total = GladiusFileWriter.HeaderSize + 16;
    //     GladiusFileWriter.WriteASCIIString(writer, "DSLC");
    //     writer.Write(total); // block size
    //     writer.Write(1);
    //     writer.Write(m_paxElements.Count);
    //
    //     int totalEntries = 12;
    //     for (int i = 0; i < m_paxElements.Count; ++i)
    //     {
    //         writer.Write((byte)1);
    //     }
    //
    //     GladiusFileWriter.WriteNull(writer, totalEntries - m_paxElements.Count);
    // }


    // public void WriteNLVL(BinaryWriter writer)
    // {
    //     int blockSize = 0x20;
    //     GladiusFileWriter.WriteASCIIString(writer, "NLVL");
    //
    //     writer.Write(blockSize); // block size
    //     writer.Write(2);
    //     writer.Write(1);
    //     GladiusFileWriter.WriteNull(writer, 0x10);
    // }
    //
    // // public void WriteMESH(BinaryWriter writer)
    // {
    //     int total = GladiusFileWriter.HeaderSize + (24 * m_paxElements.Count);
    //     GladiusFileWriter.WriteASCIIString(writer, "MESH");
    //
    //     int paddedTotal = GladiusFileWriter.GetPadValue(total);
    //
    //     writer.Write(paddedTotal); // block size
    //
    //     writer.Write(0);
    //     writer.Write(m_paxElements.Count); // number of elements
    //
    //
    //     for (int i = 0; i < m_paxElements.Count; ++i)
    //     {
    //         m_paxElements[i].ToStream(writer);
    //     }
    //
    //     GladiusFileWriter.WriteNull(writer, (paddedTotal - total));
    // }

    // public void WriteELEM(BinaryWriter writer)
    // {
    //     int total = GladiusFileWriter.HeaderSize + 16;
    //     GladiusFileWriter.WriteASCIIString(writer, "ELEM");
    //
    //     writer.Write(total); // block size
    //     writer.Write(0);
    //     writer.Write(m_paxElements.Count);
    //
    //     for (int i = 0; i < m_displayListHeaders.Count; ++i)
    //     {
    //         int val = (4 | (m_displayListHeaders[i].entries.Count << 8));
    //         writer.Write(val);
    //         writer.Write(0);
    //     }
    // }


    // public static SkinData CreateSkinData(Mesh mesh, short animShift)
    // {
    //     Debug.Assert(mesh != null);
    //     Debug.Assert(mesh.vertices.Length == mesh.boneWeights.Length);
    //
    //     SkinData skinData = new SkinData();
    //
    //     // convert back to common vertex to identify duplicates?
    //     List<CommonVertexInstance> cviList = new List<CommonVertexInstance>();
    //     for (int i = 0; i < mesh.vertices.Length; ++i)
    //     {
    //         CommonVertexInstance cvi = new CommonVertexInstance();
    //         cvi.Position = mesh.vertices[i];
    //         cvi.Normal = mesh.normals[i];
    //         cvi.UV = mesh.uv[i];
    //         cvi.BoneWeight = mesh.boneWeights[i];
    //         cviList.Add(cvi);
    //     }
    //
    //     int headerLength = 32;
    //     int posNormSize = 9;
    //     int memSize = SkinData.RawSize;
    //     
    //     
    //     
    //     BoneWeight[] boneWeightsCopy = new BoneWeight[mesh.boneWeights.Length];
    //     mesh.boneWeights.CopyTo(boneWeightsCopy, 0);
    //
    //     // do something here to shrink the weights to a max of 3 only. quick look shows that 4th bone often negligible weight
    //     // so renormalise and store it as a csk2 plus an extra in cska
    //
    //     for (int i = 0; i < boneWeightsCopy.Length; ++i)
    //     {
    //         boneWeightsCopy[i] = boneWeightsCopy[i].RenormaliseWeights(3);
    //     }
    //
    //     HashSet<(int, PosNorm)> uniqueVertexOneBone = new HashSet<(int, PosNorm)>();
    //     HashSet<((int, int), PosNorm)> uniqueVertexTwoBone = new HashSet<((int, int), PosNorm)>();
    //
    //     for (int i = 0; i < boneWeightsCopy.Length; i++)
    //     {
    //         BoneWeight bw = boneWeightsCopy[i];
    //         int activeWeights = bw.CountWeights();
    //         if (activeWeights == 1)
    //         {
    //             uniqueVertexOneBone.Add((bw.boneIndex0,
    //                 new PosNorm() { Position = mesh.vertices[i], Normal = mesh.normals[i] }));
    //         }
    //         else if (activeWeights == 2)
    //         {
    //             uniqueVertexTwoBone.Add(((bw.boneIndex0, bw.boneIndex1),
    //                 new PosNorm() { Position = mesh.vertices[i], Normal = mesh.normals[i] }));
    //         }
    //     }
    //
    //     int srcPosition = 0;
    //     Dictionary<int, List<int>> oneBoneDict = new Dictionary<int, List<int>>();
    //     List<PosNorm> oneBonePosNormals = new List<PosNorm>();
    //
    //     foreach (var entry in uniqueVertexOneBone)
    //     {
    //         int boneId = entry.Item1;
    //         if (!oneBoneDict.TryGetValue(boneId, out List<int> boneList))
    //         {
    //             boneList = new List<int>();
    //             oneBoneDict[boneId] = boneList;
    //         }
    //
    //         int positionIndex = oneBonePosNormals.IndexOf(entry.Item2);
    //         if (positionIndex == -1)
    //         {
    //             oneBonePosNormals.Add(entry.Item2);
    //             positionIndex = boneList.Count;
    //         }
    //
    //         boneList.Add(positionIndex);
    //     }
    //
    //     Dictionary<(int, int), List<int>> twoBoneDict = new Dictionary<(int, int), List<int>>();
    //     List<PosNorm> twoBonePosNormals = new List<PosNorm>();
    //
    //     // and all the twos
    //     foreach (var entry in uniqueVertexTwoBone)
    //     {
    //         var key = (Math.Min(entry.Item1.Item1, entry.Item1.Item2),
    //             Math.Max(entry.Item1.Item1, entry.Item1.Item2));
    //         //var key = (Math.Min(bw.boneIndex0, bw.boneIndex1), Math.Max(bw.boneIndex0, bw.boneIndex1));
    //         if (!twoBoneDict.TryGetValue(key, out List<int> boneList))
    //         {
    //             boneList = new List<int>();
    //             twoBoneDict[key] = boneList;
    //         }
    //
    //         int positionIndex = twoBonePosNormals.IndexOf(entry.Item2);
    //         if (positionIndex == -1)
    //         {
    //             twoBonePosNormals.Add(entry.Item2);
    //             positionIndex = boneList.Count;
    //         }
    //
    //         boneList.Add(positionIndex);
    //     }
    //
    //     List<(CSK1, int)> csk1LinkMap = new List<(CSK1, int)>();
    //
    //     foreach (var key in oneBoneDict.Keys)
    //     {
    //         CSK1 csk1 = SkinData.CreateCSK1(key, oneBoneDict[key], mesh.vertices, mesh.normals, boneWeightsCopy);
    //         srcPosition += CSK1.RawSize;
    //
    //         csk1LinkMap.Add((csk1, key));
    //         skinData.CSK1List.Add(csk1);
    //     }
    //     // store them in size
    //
    //     skinData.CSK1List.Sort((x, y) => y.count.CompareTo(x.count));
    //
    //     foreach (var csk1 in skinData.CSK1List)
    //     {
    //         int key = csk1LinkMap.Find(x => x.Item1 == csk1).Item2;
    //
    //         csk1.vertSrc = (uint)srcPosition;
    //         csk1.vertDst = (uint)(csk1.vertSrc - headerLength);
    //
    //         // write the vertices and normals associated with csk1.
    //         foreach (var index in oneBoneDict[key])
    //         {
    //             Vector3 pos = GladiusGlobals.UnityToGladius(oneBonePosNormals[index].Position);
    //             Vector3 normal = GladiusGlobals.UnityToGladius(oneBonePosNormals[index].Normal);
    //             csk1.ExtractedPositions.Add(pos);
    //             csk1.ExtractedNormals.Add(normal);
    //         }
    //
    //         srcPosition += (csk1.ExtractedPositions.Count * posNormSize);
    //     }
    //
    //
    //     List<(int, int, float)> additionalValues = new List<(int, int, float)>();
    //
    //     Dictionary<(int, int, int), List<int>> threeBoneDict = new Dictionary<(int, int, int), List<int>>();
    //     // // and all the twos
    //     // for (int i = 0; i < threeBoneVertices.Count; ++i)
    //     // {
    //     //     BoneWeight bw = boneWeightsCopy[threeBoneVertices[i]];
    //     //
    //     //     var twoPartKey = (Math.Min(bw.boneIndex0, Math.Min(bw.boneIndex1, bw.boneIndex2)),
    //     //         Math.Max(bw.boneIndex0, Math.Max(bw.boneIndex1, bw.boneIndex2)));
    //     //
    //     //     if (!twoBoneDict.TryGetValue(twoPartKey, out List<int> boneList))
    //     //     {
    //     //         boneList = new List<int>();
    //     //         twoBoneDict[twoPartKey] = boneList;
    //     //     }
    //     //
    //     //     boneList.Add(threeBoneVertices[i]);
    //     //
    //     //     // add an additional (vertexIndex, boneId, weight) 
    //     //     // FIXME - need to figure out which bone we haven't used.'
    //     //     additionalValues.Add((threeBoneVertices[i], bw.boneIndex2, bw.weight2));
    //     // }
    //
    //
    //     List<(CSK2, (int, int))> csk2LinkMap = new List<(CSK2, (int, int))>();
    //
    //     foreach (var key in twoBoneDict.Keys)
    //     {
    //         CSK2 csk2 = SkinData.CreateCSK2(key, twoBoneDict[key], mesh.vertices, mesh.normals, boneWeightsCopy);
    //         srcPosition += CSK2.RawSize;
    //
    //         csk2LinkMap.Add((csk2, key));
    //         skinData.CSK2List.Add(csk2);
    //     }
    //
    //     // store them in size
    //     skinData.CSK2List.Sort((x, y) => y.count.CompareTo(x.count));
    //
    //     // write positions and normals
    //     foreach (CSK2 csk2 in skinData.CSK2List)
    //     {
    //         var key = csk2LinkMap.Find(x => x.Item1 == csk2).Item2;
    //
    //         csk2.vertSrc = (uint)srcPosition;
    //         csk2.vertDst = (uint)(csk2.vertSrc - headerLength);
    //
    //         // write the vertices and normals associated with csk2.
    //         foreach (var index in twoBoneDict[key])
    //         {
    //             Vector3 pos = GladiusGlobals.UnityToGladius(twoBonePosNormals[index].Position);
    //             Vector3 normal = GladiusGlobals.UnityToGladius(twoBonePosNormals[index].Normal);
    //             csk2.ExtractedPositions.Add(pos);
    //             csk2.ExtractedNormals.Add(normal);
    //         }
    //
    //         srcPosition += (csk2.ExtractedPositions.Count * posNormSize);
    //     }
    //
    //     // write weight data
    //     foreach (CSK2 csk2 in skinData.CSK2List)
    //     {
    //         var key = csk2LinkMap.Find(x => x.Item1 == csk2).Item2;
    //
    //         csk2.weightsSrc = (uint)srcPosition;
    //         // write the weights
    //         foreach (var index in twoBoneDict[key])
    //         {
    //             BoneWeight bw = boneWeightsCopy[index];
    //             csk2.ExtractedWeightsFloats.Add((bw.weight0, bw.weight1));
    //             csk2.ExtractedWeightsBytes.Add(((byte)(0xFF * bw.weight0), (byte)(0xFF * bw.weight1)));
    //         }
    //
    //         srcPosition += (csk2.ExtractedWeightsBytes.Count * 2);
    //     }
    //
    //     // create all weights.
    //
    //
    //     foreach (var key in additionalValues)
    //     {
    //         CSKA cska = SkinData.CreateCSKA(key, mesh.vertices, mesh.normals, boneWeightsCopy);
    //         skinData.CSKAList.Add(cska);
    //     }
    //
    //
    //     skinData.NumList1 = (short)skinData.CSK1List.Count;
    //     skinData.NumList2 = (short)skinData.CSK2List.Count;
    //     skinData.NumListA = (short)skinData.CSKAList.Count;
    //
    //     skinData.Size = headerLength + srcPosition;
    //
    //     // general structure of files 
    //     // for each mesh
    //     // skindata info (csk1,csk2,cska)
    //     // vertex and normal data
    //     // weight data and idx dst mixed?
    //
    //
    //     return skinData;
    // }


    public String m_name;
    // public List<IndexedVector3> m_points = new List<IndexedVector3>();
    // public List<IndexedVector3> m_normals = new List<IndexedVector3>();
    // public List<IndexedVector2> m_uvs = new List<IndexedVector2>();
    // public List<TextureHeaderInfo> m_textures = new List<TextureHeaderInfo>();
    // public List<String> m_names = new List<String>();
    // public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
    //
    // public List<String> m_selsInfo = new List<string>();
    // public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
    // public List<PaxElement> m_paxElements = new List<PaxElement>();
    // public List<SkinData> m_skinData = new List<SkinData>();
    // public List<BoneNode> m_bones = new List<BoneNode>();
    //public bool Valid =true;

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
        GetChunk<DSLIChunk>().Data.Add(dsli);
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

        gcm.MatName = new string(gcm.MatNameRaw);
        gcm.SelectSetMask = reader.ReadUInt32();
        gcm.AttributeFlags = reader.ReadUInt32();
        gcm.AttributeValues = reader.ReadUInt32();

        gcm.TexIndex = reader.ReadBytes(8);
        gcm.BlendModes = reader.ReadBytes(8);
        gcm.GenModes = reader.ReadBytes(8);

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

public record PosNorm
{
    public virtual bool Equals(PosNorm other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Position.Equals(other.Position) && Normal.Equals(other.Normal);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Normal);
    }

    public Vector3 Position;
    public Vector3 Normal;
}