using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class XboxModelReader : BaseModelReader
{
    public XboxModelReader()
    {
    }

    public void LoadModels(String sourceDirectory, String infoFile, int maxFiles = -1)
    {
        m_models.Clear();
        String[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
        int counter = 0;

        foreach (String file in files)
        {
            if (file.Contains("extraanim") || file.Contains("bonename"))
            {
                continue;
            }

            try
            {
                FileInfo sourceFile = new FileInfo(file);

                XboxModel model = LoadSingleModel(sourceFile.FullName) as XboxModel;

                CommonModelData commonModel = model.ToCommon();

                m_models.Add(model);

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

    public void WriteModel(XboxModel model, int bestSkinnedLod, Dictionary<string, int> modelMeshMap, String texturePath, String objOutputPath)
    {
        try
        {
            List<int> excludeList = new List<int>();
            List<int> includeList = new List<int>();

            if (model.m_skinned)
            {
                int limit = 0;
                modelMeshMap.TryGetValue(model.m_name, out limit);

                for (int j = 0; j < model.m_xrndSection.m_doegData.NumMeshes; ++j)
                {
                    //if (model.m_subMeshData1List[j].LodLevel < bestSkinnedLod)
                    if ((model.m_subMeshData1List[j].LodLevel & bestSkinnedLod) != 0)
                    {
                        if (limit == 0 || j < limit)
                        {
                            includeList.Add(j);
                        }
                    }
                }
            }

            if (includeList.Count > 0)
            {
                for (int j = 0; j < model.m_xrndSection.m_doegData.NumMeshes; ++j)
                {
                    if (!includeList.Contains(j))
                    {
                        excludeList.Add(j);
                    }
                }
            }

            //XBoxColladaWriter colladaWriter = new XBoxColladaWriter(model,m_itemFileLoader);
            //colladaWriter.Write(s_rootPath, null, texturePath, excludeList);


        }
        catch (System.Exception ex)
        {

        }

    }



    public List<BaseModel> m_models = new List<BaseModel>();
    public static string s_rootPath = "";
    public static string s_characterModelPath = "";
    public static string s_modelOutputPath = "";
    bool m_writeAnimationOnly = false;

}

public class XboxModel : BaseModel
{
    public VertexDataAndDesc m_vertexDataAndDesc = new VertexDataAndDesc();
    public List<int> m_allIndices = new List<int>();
    public List<SubMeshData1> m_subMeshData1List = new List<SubMeshData1>();
    public List<SubMeshData2> m_subMeshData2List = new List<SubMeshData2>();
    public List<MaterialData> m_materialDataList = new List<MaterialData>();
    public List<MeshMaterialOffsets> m_meshMaterialOffsetList = new List<MeshMaterialOffsets>();
    public List<MaterialBlock> m_materialBlockList = new List<MaterialBlock>();
    public XRNDSection m_xrndSection;
    public List<String> m_selsInfo = new List<string>();
    public List<List<byte>> m_materialDataByteBlocks = new List<List<byte>>();

    public List<int> m_rebuildOffsets = new List<int>();

    public StringBuilder m_animFrameData = new StringBuilder();

    public const int s_textureBlockSize = 64;
    public const int s_materialBlockSize = 44;
    public const int s_mmoOffsetBlockSize = 12;

    public SubMeshData3 m_subMeshData3;
    public int m_avgVertex;
    public XboxModel(String name)
        : base(name)
    {
    }

    public void ReadXBOXDataChunk(BinaryReader binReader)
    {
        if (CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.xrndTag))
        {
            int sectionLength = binReader.ReadInt32();
            int version = binReader.ReadInt32();
            int numEntries = binReader.ReadInt32();
            int numLodSets = binReader.ReadInt32();
            List<uint> lodSetMasks = new List<uint>();
            for (int i = 0; i < numLodSets; ++i)
            {
                lodSetMasks.Add(binReader.ReadUInt32());
            }
        }
    }


    public CommonModelData ToCommon()
    {
        CommonModelData cmd = new CommonModelData();
        cmd.XBoxModel = this;
        cmd.BoneList = BoneList;
        //cmd.RootBone = m_rootBone;
        cmd.BoneIdDictionary = m_boneIdDictionary;
        cmd.Name = m_name;
        cmd.VertexDataLists = new List<VertexDataAndDesc>();
        cmd.VertexDataLists.Add(m_vertexDataAndDesc);
        cmd.IndexDataList = new List<List<int>>();
        cmd.IndexDataList.Add(m_allIndices);

        foreach(VertexDataAndDesc vdad in cmd.VertexDataLists)
        {
            cmd.AllVertices.AddRange(vdad.VertexData);
        }

        //foreach (List<int> list in cmd.IndexDataList)
        //{
        //    cmd.
        //}
        int count = 0;

        foreach (MaterialData md in m_materialDataList)
        {
            CommonMaterialData cm = md.ToCommon();
            cm.Name = m_name + (count++);
            cmd.CommonMaterials.Add(cm);
        }

        List<int> meshIdList = new List<int>();
        Dictionary<string, int> materialLookup = new Dictionary<string, int>();

        for (int i = 0; i < m_subMeshData1List.Count; ++i)
        {
            meshIdList.Add(i);
        }


        foreach (string textureName in m_textureNames)
        {
            CommonTextureData ctd = new CommonTextureData();
            ctd.textureName = textureName;
            cmd.CommonTextures.Add(ctd);
        }


        List<SubmeshData> submeshList = GetIndices(meshIdList);
        foreach (SubmeshData smd in submeshList)
        {
            cmd.CommonMeshData.Add(smd.ToCommon());
        }

        return cmd;
    }



    public override void LoadData(BinaryReader binReader)
    {
        binReader.BaseStream.Position = 0;
        ReadSELSSection(binReader);
        binReader.BaseStream.Position = 0;
        CommonModelImporter.ReadNullSeparatedNames(binReader, CommonModelImporter.nameTag, m_boneNames);
        binReader.BaseStream.Position = 0;
        ReadTextureSection(binReader);
        binReader.BaseStream.Position = 0;
        ReadSKELSection(binReader);
        binReader.BaseStream.Position = 0;
        ReadXRNDSection(binReader);
        //LoadAnimationData();
        //DumpSkeletonAndAnimation();

    }


    public void ReadSELSSection(BinaryReader binReader)
    {
        // this is selection set data? or scene entites list?
        // mNumLodSelectSets
        //	pJointLODs = mMeshData->mPaxFile->mJointLODs;
        //	lodHideMask = mMeshData->mLODSelectSetMask[mMeshData->mLOD];

        CommonModelImporter.ReadNullSeparatedNames(binReader, CommonModelImporter.selsTag, m_selsInfo);

    }

    public void ReadXRNDSection(BinaryReader binReader)
    {
        if (CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.xrndTag))
        {
            m_xrndSection = new XRNDSection();
            m_xrndSection.sectionLength = binReader.ReadInt32();
            m_xrndSection.uk2a = binReader.ReadInt32();
            m_xrndSection.numEntries = binReader.ReadInt32();
            m_xrndSection.numSkip = binReader.ReadInt32();
            binReader.BaseStream.Position += m_xrndSection.numSkip * 4;
            //int uk2c = binReader.ReadInt32();

            //int uk2d = binReader.ReadInt32();
            m_xrndSection.m_doegData = DoegData.FromStream(binReader);

            int doegEndVal = (int)(binReader.BaseStream.Position - 4);
            binReader.BaseStream.Position = doegEndVal + m_xrndSection.m_doegData.Block1Start;

            for (int i = 0; i < m_xrndSection.m_doegData.NumMeshes; ++i)
            {
                m_xrndSection.blockOneValues.Add(binReader.ReadInt32());
            }


            for (int i = 0; i < m_xrndSection.m_doegData.NumMeshes; ++i)
            {
                SubMeshData1 smd = SubMeshData1.FromStream(binReader);
                m_subMeshData1List.Add(smd);
            }

            int TotalIndices = 0;


            //NumMeshes += 1;

            for (int i = 0; i < m_xrndSection.m_doegData.NumMeshes; ++i)
            {
                SubMeshData2 smd = SubMeshData2.FromStream(binReader);
                string groupName = String.Format("{0}-submesh{1}", m_name, i);
                smd.fbxNodeId = groupName;
                m_subMeshData2List.Add(smd);

                TotalIndices += smd.NumIndices;
                int val1a = smd.NumIndices * 2;
                smd.pad = val1a % 4;
            }

            binReader.BaseStream.Position += 4;
            m_xrndSection.TotalVertices1 = binReader.ReadInt32();
            binReader.BaseStream.Position += 16;
            m_xrndSection.TotalVertices2 = binReader.ReadInt32();

            binReader.BaseStream.Position -= 20;
            // do stuff...
            m_subMeshData3 = SubMeshData3.FromStream(this, binReader, m_xrndSection.m_doegData.NumMeshes, m_xrndSection.m_doegData.Block5Start, m_skinned);


            binReader.BaseStream.Position = doegEndVal + m_xrndSection.m_doegData.TextureBlockOffset;

            CommonModelImporter.ReadNullSeparatedNames(binReader, binReader.BaseStream.Position, m_xrndSection.m_doegData.NumTextures, m_textureNames);

            for (int i = 0; i < m_textureNames.Count; ++i)
            {
                if (!m_textureNames[i].EndsWith(".tga"))
                {
                    m_textureNames[i] += ".tga";
                }
            }

            int skygoldIndex = -1;
            int textureCount = 0;
            //foreach (MaterialData materialData in m_materialDataList)
            for (int i = 0; i < m_materialDataList.Count; ++i)
            {

                MaterialData.FixupMaterialSlots(m_materialDataList[i], m_textureNames);

                //materialData.textureId = AdjustForModel(materialData.textureId);
                //int textureIndex = materialData.diffuseTextureId / s_textureBlockSize;
                //materialData.TextureData1 = m_textures[textureIndex];

                //if (materialData.TextureData1.textureName.Contains("skygold"))
                //{
                //    skygoldIndex = i;
                //}
            }
            // if we have skygold then make the specular of the following it and remove from list.
            //if (skygoldIndex != -1)
            //{
            //    //Debug.Assert(skygoldIndex < m_materialDataList.Count - 1);
            //    if (skygoldIndex < m_materialDataList.Count - 1)
            //    {
            //        MaterialData skyGoldMaterial = m_materialDataList[skygoldIndex];
            //        MaterialData skyGoldFollowMaterial = m_materialDataList[skygoldIndex + 1];

            bool wrappedInt16 = false;
            int lastIndex = -1;
            int lastIndexWrapVal = 0;
            //    }
            //}

            bool hasDoubleVertex = m_name.Contains("worldmap");

            foreach (SubMeshData2 smd in m_subMeshData2List)
            {
                for (int i = 0; i < smd.NumIndices; ++i)
                {
                    int val = binReader.ReadUInt16();
                    if (hasDoubleVertex && val < (lastIndex - 100))
                    {
                        wrappedInt16 = true;
                        lastIndexWrapVal = lastIndex;
                    }
                    lastIndex = val;
                    if (wrappedInt16)
                    {
                        val += (lastIndexWrapVal + 1);
                    }
                    smd.indices.Add(val);
                }
                m_allIndices.AddRange(smd.indices);
                smd.padBytes = binReader.ReadBytes(smd.pad);
                smd.BuildMinMax();
            }

            long testPosition = binReader.BaseStream.Position;

            if (m_skinned)
            {
                if (CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.endTag))
                {
                    int diff = ((int)binReader.BaseStream.Position - 4 - (int)testPosition);

                    diff -= m_xrndSection.TotalVertices1 * 6;

                    m_avgVertex = diff / m_xrndSection.TotalVertices1;
                    binReader.BaseStream.Position = testPosition;

                    if (m_avgVertex == 28)
                    {
                        ReadSkinnedVertexData28(binReader, m_vertexDataAndDesc, m_xrndSection.TotalVertices1);
                    }
                    else if (m_avgVertex == 32)
                    {
                        m_hasColorInfo = true;
                        ReadSkinnedVertexData32(binReader, m_vertexDataAndDesc, m_xrndSection.TotalVertices1);
                    }



                    //

                    int maxWeight = -1;
                    foreach (CommonVertexInstance vbi in m_vertexDataAndDesc.VertexData)
                    {
                        vbi.BoneIndices = new short[3];
                        vbi.BoneIndices[0] = binReader.ReadInt16();
                        vbi.BoneIndices[1] = binReader.ReadInt16();
                        vbi.BoneIndices[2] = binReader.ReadInt16();

                        DebugBoneWeights(vbi.BoneIndices[0]);
                        DebugBoneWeights(vbi.BoneIndices[1]);
                        DebugBoneWeights(vbi.BoneIndices[2]);

                        for (int z = 0; z < vbi.BoneIndices.Length; ++z)
                        {
                            if (z == 6)
                            {
                                break;
                            }
                        }


                    }
                }
                int ibreak2 = 0;
            }
            else
            {

                if (CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.endTag))
                {
                    int diff = ((int)binReader.BaseStream.Position - 4 - (int)testPosition);

                    int totalVertices = m_xrndSection.TotalVertices1;
                    if (m_xrndSection.TotalVertices2 > 0 && m_xrndSection.TotalVertices2 != m_xrndSection.TotalVertices1)
                    {
                        totalVertices += m_xrndSection.TotalVertices2;
                    }
                    m_avgVertex = diff / totalVertices;
                    binReader.BaseStream.Position = testPosition;

                    //ReadUnskinnedVertexData36(binReader, m_allVertices, TotalVertices);
                    switch (m_avgVertex)
                    {
                        case 24:
                            ReadUnskinnedVertexData24(binReader, m_vertexDataAndDesc, totalVertices);
                            break;
                        case 28:
                            //ReadUnskinnedVertexData28(binReader, m_vertexDataAndDesc, TotalVertices);
                            ReadUnskinnedVertexData28(binReader, m_vertexDataAndDesc, totalVertices);
                            break;
                        case 36:
                            ReadUnskinnedVertexData36(binReader, m_vertexDataAndDesc, totalVertices);
                            break;
                        default:
                            Debug.LogErrorFormat("Model [{0}] unexpected vertex average tv1[{1}] tv2[{2}] t[{3}] avg[{4}] diff[{5}] ",
                                m_name, m_xrndSection.TotalVertices1, m_xrndSection.TotalVertices2, totalVertices, m_avgVertex, diff);
                            int ibreak = 0;
                            break;
                    }
                }
            }
            BuildBB();

            for (int i = 0; i < m_textureNames.Count; ++i)
            {
                ShaderData shaderData = new ShaderData();
                shaderData.shaderName = "test";
                shaderData.textureId1 = (byte)i;
                shaderData.textureId2 = (byte)(i + 1);
                m_shaderData.Add(shaderData);
            }
        }
        else
        {
            //infoStream.WriteLine("Doeg not at expected - multi file? : " + sourceFile.FullName);
        }


    }

    public void DebugBoneWeights(short index)
    {

        CommonVertexInstance.sBoneIndices.Add(index);
        int count = 0;
        if (!CommonVertexInstance.sBoneIndicesCount.TryGetValue(index, out count))
        {
            CommonVertexInstance.sBoneIndicesCount[index] = count;
        }
        count++;
        CommonVertexInstance.sBoneIndicesCount[index] = count;
    }

    public bool IsEnd(BinaryReader binReader)
    {
        byte[] endBlock = binReader.ReadBytes(4);
        char[] endBlockChar = new char[endBlock.Length];
        for (int i = 0; i < endBlock.Length; ++i)
        {
            endBlockChar[i] = (char)endBlock[i];
        }
        if (endBlockChar[0] == 'E' && endBlockChar[1] == 'N' && endBlock[2] == 'D')
        {
            return true;
        }
        return false;
    }

    public bool ValidVertex(IndexedVector3 v)
    {
        return Math.Abs(v.X) < 10000 && Math.Abs(v.Y) < 10000 && Math.Abs(v.Z) < 10000;
    }


    public List<SubmeshData> GetIndices(List<int> includeList)
    {
        List<SubmeshData> result = new List<SubmeshData>();
        bool swap = true;
        int startIndex = 0;
        int limit = m_subMeshData2List.Count;
        for (int submeshIndex = 0; submeshIndex < limit; submeshIndex++)
        {
            try
            {
            SubMeshData2 headerBlock = m_subMeshData2List[submeshIndex];
            SubMeshData1 data1 = m_subMeshData1List[submeshIndex];
                swap = true;

            int end = startIndex + headerBlock.NumIndices - 2;
                //if (!includeList.Contains(submeshIndex))
                //{
                //    startIndex += headerBlock.NumIndices;
                //    continue;
                //}

            SubmeshData smi = new SubmeshData();
            smi.index = submeshIndex;
            smi.originalMeshIndex = submeshIndex;
            smi.subMeshData = data1;
            smi.indices = new List<int>();

                //int materialIndex = Math.Min(submeshIndex, m_meshMaterialOffsetList.Count - 1);

                //smi.meshMaterial = m_meshMaterialOffsetList[materialIndex];
                ////GetTextureId(smi.index, out smi.originalMaterialId, out smi.materialId);

                int materialBlockId = 0;
                Debug.Assert(submeshIndex < m_subMeshData3.offsetList.Count);
                int offsetId = m_subMeshData3.offsetList[submeshIndex] / s_mmoOffsetBlockSize;
                Debug.Assert(offsetId < m_meshMaterialOffsetList.Count);
                materialBlockId = m_meshMaterialOffsetList[offsetId].convertedOffset;
                smi.originalMaterialId = materialBlockId;
                smi.materialId = materialBlockId;
            //smi.vertices = new List<CommonVertexInstance>();
            List<int> indexList = smi.indices;

            for (int i = startIndex; i < end; i++)
            {

                int index1 = i + 2;
                int index2 = i + 1;
                int index3 = i;

                if (index3 >= m_allIndices.Count)
                {
                    index3 = index1;
                }
                if (index2 >= m_allIndices.Count)
                {
                    index2 = index1;
                }
                if (i >= m_allIndices.Count)
                {
                    int ibreak = 0;
                }

                int i1 = m_allIndices[swap ? index1 : index3];
                int i2 = m_allIndices[index2];
                int i3 = m_allIndices[swap ? index3 : index1];

                    bool removeDegenerate = false;
                if (!(removeDegenerate && (i1 == i2 || i2 == i3 || i3 == i1)))
                {
                    indexList.Add(i1);
                    indexList.Add(i2);
                    indexList.Add(i3);
                }
                swap = !swap;
            }

            int lowestIndex = Math.Min(indexList[0], Math.Min(indexList[1], indexList[2]));
            for (int i = 0; i < indexList.Count; ++i)
            {
                smi.adjustedIndices.Add(indexList[i] - lowestIndex);
            }
            smi.lowestIndex = lowestIndex;

            if (submeshIndex == 0)
            {
                //smi.startIndex = 24;
                //smi.endIndex = 215;
            }

            if (smi.startIndex != -1 && smi.endIndex != -1)
            {
                for (int i = smi.startIndex; i < smi.endIndex; ++i)
                {
                    smi.indicesTest.Add(smi.indices[i]);
                }
            }
            else
            {
                smi.indicesTest.AddRange(smi.indices);
            }

            foreach (int index in smi.indicesTest)
            {
                if (!smi.verticesInMesh.Contains(index))
                {
                    smi.verticesInMesh.Add(index);
                }
            }

            smi.verticesInMesh.Sort();
            foreach (int i in smi.verticesInMesh)
            {
                if (i < m_vertexDataAndDesc.VertexData.Count)
                {

                    CommonVertexInstance xbvi = m_vertexDataAndDesc.VertexData[i];
                    if (xbvi.ActiveWeights() > 0)
                    {
                        xbvi.TranslatedBoneIndices = new short[xbvi.BoneIndices.Length];

                        for (int j = 0; j < xbvi.ActiveWeights(); ++j)
                        {
                            int originalBoneId = AdjustBone(xbvi.BoneIndices[j], smi.index);
                            xbvi.TranslatedBoneIndices[j] = (short)originalBoneId;
                            if (!smi.boneIdMap.ContainsKey(originalBoneId))
                            {
                                smi.boneIdMap[originalBoneId] = smi.boneIds.Count;
                                smi.reverseBoneIdMap[smi.boneIds.Count] = originalBoneId;
                                smi.boneIds.Add(originalBoneId);
                            }
                        }
                    }
                }
            }
            //smi.boneIds.Sort();
                bool validMesh = true;
                int lookup = smi.materialId;
                while (lookup < m_materialDataList.Count - 1)
                {
                    MaterialData materialData = m_materialDataList[lookup];
                    if (materialData.m_materialSlotInfoList.Count == 0)
                    {
                        lookup++;
                        Debug.LogWarningFormat("Mesh [{0}] has no materials [{1}]", smi.index, smi.materialId);
                    }
                    else
                    {
                        break;
                    }
                }
                smi.materialId = lookup;
                if (validMesh)
                {
            result.Add(smi);
                }
            startIndex += headerBlock.NumIndices;
            }
            catch (Exception e)
            {
                int ibreak = 0;
            }
        }
        return result;
    }






    public void ReadUnskinnedVertexData24(BinaryReader binReader, VertexDataAndDesc desc, int numVertices)
    {
        desc.Description = "UnskinnedVertexData24";
        for (int i = 0; i < numVertices; ++i)
        //while(true)
        {
            try
            {
                if (IsEnd(binReader))
                {
                    break;
                }
                else
                {
                    binReader.BaseStream.Position -= 4;
                }

                // 24 bytes per entry? , or 28...
                IndexedVector3 p = CommonModelImporter.FromStreamVector3(binReader);
                int normal = binReader.ReadInt32();
                IndexedVector3 normV = UncompressNormal(normal);
                IndexedVector2 u = CommonModelImporter.FromStreamVector2(binReader);
                CommonVertexInstance vpnt = new CommonVertexInstance();
                vpnt.Position = GladiusGlobals.GladiusToUnity(p);
                vpnt.UV = u;
                vpnt.Normal = GladiusGlobals.GladiusToUnity(normV);
                desc.VertexData.Add(vpnt);
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }
        int ibreak2 = 0;
    }


    public void ReadUnskinnedVertexData28(BinaryReader binReader, VertexDataAndDesc desc, int numVertices)
    {
        desc.Description = "UnskinnedVertexData28";

        for (int i = 0; i < numVertices; ++i)
        //while(true)
        {
            try
            {
                if (IsEnd(binReader))
                {
                    break;
                }
                else
                {
                    binReader.BaseStream.Position -= 4;
                }

                // 24 bytes per entry? , or 28...
                IndexedVector3 p = CommonModelImporter.FromStreamVector3(binReader);
                IndexedVector3 normV = UncompressNormal(binReader.ReadInt32());
                if (!CommonModelImporter.FuzzyEquals(normV.Length(), 1.0f, 0.01f))
                {
                    int ibreak = 0;
                }
                //IndexedVector3 normU = UncompressNormal(binReader.ReadInt32());
                int unk1 = binReader.ReadInt32();
                IndexedVector2 u = CommonModelImporter.FromStreamVector2(binReader);
                CommonVertexInstance vpnt = new CommonVertexInstance();
                vpnt.Position = GladiusGlobals.GladiusToUnity(p); ;
                vpnt.UV = u;
                vpnt.Normal = GladiusGlobals.GladiusToUnity(normV);
                desc.VertexData.Add(vpnt);
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }
        int ibreak2 = 0;
    }

    public void ReadUnskinnedVertexData36(BinaryReader binReader, VertexDataAndDesc desc, int numVertices)
    {
        desc.Description = "UnskinnedVertexData36";
        desc.HasUV2 = true;
        for (int i = 0; i < numVertices; ++i)
        //while(true)
        {
            try
            {
                if (IsEnd(binReader))
                {
                    break;
                }
                else
                {
                    binReader.BaseStream.Position -= 4;
                }

                // 24 bytes per entry? , or 28...
                IndexedVector3 p = CommonModelImporter.FromStreamVector3(binReader);
                IndexedVector3 normV = UncompressNormal(binReader.ReadInt32());
                int unk1 = binReader.ReadInt32();
                IndexedVector2 u = CommonModelImporter.FromStreamVector2(binReader);
                IndexedVector2 u2 = CommonModelImporter.FromStreamVector2(binReader);
                CommonVertexInstance vpnt = new CommonVertexInstance();
                vpnt.Position = GladiusGlobals.GladiusToUnity(p);
                vpnt.UV = u;
                vpnt.UV2 = u2;
                vpnt.Normal = GladiusGlobals.GladiusToUnity(normV);
                //vpnt.ExtraData = unk1;
                desc.VertexData.Add(vpnt);
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }
        int ibreak2 = 0;
    }
    public bool SwapLeftRight = false;


    public void ReadSkinnedVertexData28(BinaryReader binReader, VertexDataAndDesc desc, int numVertices)
    {
        desc.Description = "SkinnedVertexData28";
        for (int i = 0; i < numVertices; ++i)
        {
            try
            {
                CommonVertexInstance vpnt = new CommonVertexInstance();
                vpnt.Position = CommonModelImporter.FromStreamVector3(binReader);
                vpnt.Position = GladiusGlobals.GladiusToUnity(vpnt.Position);
                //vpnt.BoneInfo1 = binReader.ReadInt32();
                vpnt.Normal = UncompressNormal(binReader.ReadInt32());
                vpnt.Normal = GladiusGlobals.GladiusToUnity(vpnt.Normal);
                //int unk1 = binReader.ReadInt32();
                vpnt.UV = CommonModelImporter.FromStreamVector2(binReader);
                vpnt.BoneWeights = binReader.ReadInt32();

                desc.VertexData.Add(vpnt);
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }
        int ibreak2 = 0;
    }

    public void ReadSkinnedVertexData32(BinaryReader binReader, VertexDataAndDesc desc, int numVertices)
    {
        desc.Description = "SkinnedVertexData32";

        for (int i = 0; i < numVertices; ++i)
        {
            try
            {
                CommonVertexInstance vpnt = new CommonVertexInstance();
                vpnt.Position = CommonModelImporter.FromStreamVector3(binReader);
                vpnt.Position = GladiusGlobals.GladiusToUnity(vpnt.Position);
                vpnt.Normal = UncompressNormal(binReader.ReadInt32());
                vpnt.Normal = GladiusGlobals.GladiusToUnity(vpnt.Normal);
                vpnt.DiffuseColor = CommonModelImporter.FromStreamColor32(binReader);
                vpnt.UV = CommonModelImporter.FromStreamVector2(binReader);
                vpnt.BoneWeights = binReader.ReadInt32();
                desc.VertexData.Add(vpnt);
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }
        int ibreak2 = 0;
    }



    // taken from old sol code and it seems to work. wow!
    public static IndexedVector3 UncompressNormal(int cv)
    {
        IndexedVector3 v = new IndexedVector3();
        int x = ((int)(cv & 0x7ff) << 21) >> 21,
                y = ((int)(cv & 0x3ffe00) << 10) >> 21,
                z = (int)(cv & 0xffc00000) >> 22;

        v.X = ((float)x) / (float)((1 << 10) - 1);
        v.Y = ((float)y) / (float)((1 << 10) - 1);
        v.Z = ((float)z) / (float)((1 << 9) - 1);
        return v;
    }

    public List<int> GetMeshListForModel()
    {
        List<int> returnList = new List<int>();
        if (m_name == "barbarian.mdl")
        {

        }
        return returnList;
    }


    public int TextureForMesh(int meshId)
    {
        if (m_name == "amazon.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;

            if (meshId == 2 || meshId == 3 || meshId == 4 || meshId == 15 || meshId == 16) return 2;
            return 3;
        }
        else
            if (m_name == "archer.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;

            if (meshId == 2 || meshId == 3 || meshId == 4 || meshId == 15 || meshId == 16) return 2;
            return 3;
        }
        else
                if (m_name == "archerF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;

            if (meshId == 2 || meshId == 3 || meshId == 4 || meshId == 5 || meshId == 19) return 2;
            return 4;
        }
        else if (m_name == "banditA.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2 || meshId == 3 || meshId == 16 || meshId == 17 || meshId == 18 || meshId == 19) return 2;
            return 4;
        }
        else if (m_name == "banditAF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2 || meshId == 3 || meshId == 16 || meshId == 17 || meshId == 18 || meshId == 19) return 2;
            return 4;
        }
        else if (m_name == "banditBF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2) return 2;
            if (meshId == 15 || meshId == 16 || meshId == 17 || meshId == 18 || meshId == 19 || meshId == 20 || meshId == 21) return 5;
            return 4;
        }
        else if (m_name == "barbarian.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId >= 2 && meshId <= 10) return 2;
            if (meshId >= 11 && meshId <= 14) return 3;
        }
        else if (m_name == "barbarianF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId >= 2 && meshId <= 11) return 2;
            if (meshId >= 12 && meshId <= 16) return 3;
        }
        else if (m_name == "bear.mdl")
        {
            if (meshId == 3 || meshId == 5) return 1;
            return 0;
        }
        else if (m_name == "bearGreater.mdl")
        {
            if (meshId == 1 || meshId == 2) return 1;
            return 0;
        }
        else if (m_name == "berserker.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 7 || meshId == 11) return 3;
            return 2;
        }
        else if (m_name == "berserkerF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 16 || meshId == 17) return 3;
            return 2;
        }
        else if (m_name == "berserkerRage.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 19) return 3;
            if (meshId == 6 || meshId == 10 || meshId == 11 || meshId == 14) return 2;
            return 1;
        }
        else if (m_name == "berserkerRageF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 3 || meshId == 4 || meshId == 16 || meshId == 18) return 2;
            return 3;
        }
        else if (m_name == "cat.mdl")
        {
            // nothing to do.
            if (meshId == 4) return 0;

        }
        else if (m_name == "catGreater.mdl")
        {
            // nothing to do.
        }
        else if (m_name == "centurion.mdl")
        {
            if (meshId >= 0 && meshId <= 8) return 0;
            if (meshId >= 9 && meshId <= 12) return 1;
            if (meshId == 14) return 2;
            if (meshId == 15) return 3;

        }
        else if (m_name == "centurionF.mdl")
        {
            if (meshId >= 4 && meshId <= 15) return 0;
            if (meshId >= 16 && meshId <= 19) return 2;
            if (meshId == 20) return 3;
        }


        else if (m_name == "darkGodCat.mdl")
        {
            // nothing to do.
        }
        else if (m_name == "legionnaire.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId >= 4 && meshId <= 13) return 4;
            if (meshId == 2 || meshId == 14 || meshId == 15 || meshId == 16) return 2;
            return 3;
        }
        else if (m_name == "legionnaireDark.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId >= 1 && meshId <= 6) return 1;
            return 2;
        }
        else if (m_name == "legionnaireF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId >= 4 && meshId <= 15) return 4;
            //if (meshId == 2 || meshId == 14 || meshId == 15 || meshId == 16) return 2;
            return 2;
        }
        else if (m_name == "minotaur.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId >= 2 && meshId <= 4) return 0;
            if (meshId >= 2 && meshId <= 15) return 2;
            //if (meshId == 2 || meshId == 14 || meshId == 15 || meshId == 16) return 2;
            return 2;
        }
        else if (m_name == "mongrel.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId >= 1 && meshId <= 7) return 1;
            return 2;
        }
        else if (m_name == "mongrelShaman.mdl")
        {
            if (meshId == 3) return 1;
            if (meshId >= 4 && meshId <= 10) return 0;
            if (meshId == 11) return 0;
            if (meshId == 12) return 1;

            //if (meshId == 0) return 0;
            //if (meshId >= 1 && meshId <= 3) return 1;
            //if(
            //return 3;
        }
        else if (m_name == "murmillo.mdl")
        {
            if (meshId == 0) return 0; // armour
            if (meshId == 1) return 2; // teeth
            if (meshId == 2) return 4; // eyes
            if (meshId >= 3 && meshId <= 13) return 4; //skin
            return 0;
        }
        else if (m_name == "murmilloF.mdl")
        {
            if (meshId == 0) return 0; // eyes
            if (meshId == 1) return 1; // teeth
            if (meshId == 2) return 4; // armour
            if (meshId == 3) return 4; // shield straps
            if (meshId >= 4 && meshId <= 13) return 5; //skin
            return 2;
        }
        else if (m_name == "ogre.mdl")
        {
            if (meshId == 3) return 2;
            //if (meshId == 1) return 1;
            if (meshId == 0 || meshId == 2 || meshId == 11 || meshId == 12 || meshId == 13 || meshId == 14 || meshId == 19) return 1;
            return 3;
        }
        else if (m_name == "peltast.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2) return 2;
            if (meshId == 17 || meshId == 18) return 2;
            return 4;
        }
        else if (m_name == "peltastF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2) return 4;
            if (meshId == 14 || meshId == 15) return 1;
            return 4;
        }
        else if (m_name == "peltastNor.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2) return 2;
            if (meshId >= 3 && meshId <= 10) return 4;
            if (meshId == 16) return 4;
            return 5;
        }
        else if (m_name == "peltastNorF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2) return 2;
            if (meshId >= 14 && meshId <= 24) return 5;
            return 4;
        }
        else if (m_name == "samniteImp.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId >= 2 && meshId <= 10) return 2;
            return 3;
        }
        else if (m_name == "samniteImpF.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 2) return 3;
            if (meshId >= 3 && meshId <= 14) return 5;

            return 2;
        }
        else if (m_name == "samniteExp.mdl")
        {
            if (meshId == 0) return 0; // shield straps
            if (meshId == 1) return 2; // eyes
            if (meshId == 2) return 3; // teeth
            if (meshId == 3) return 4;// armour
            if (meshId >= 4 && meshId <= 9) return 5; // skin
            if (meshId == 11) return 5;
            return 4;
        }
        else if (m_name == "samniteExpF.mdl")
        {
            if (meshId == 0) return 0; // armour
            if (meshId == 1) return 2;
            if (meshId >= 4 && meshId <= 13) return 5; // skin
            return 0;
        }
        else if (m_name == "samniteSte.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId == 3) return 4;
            if (meshId >= 2 && meshId <= 8) return 2;
            if (meshId == 10) return 2;
            return 4;
        }
        else if (m_name == "samniteSteF.mdl")
        {
            if (meshId == 0) return 0; // shield straps
            if (meshId == 1) return 2; //mouth 
            if (meshId >= 2 && meshId <= 3) return 4; // armour
            if (meshId == 4) return 5; // eyes
            if (meshId >= 15 && meshId <= 18) return 4;
            return 3; // skin
        }
        else if (m_name == "satyr.mdl")
        {
            // nothing to do
        }
        else if (m_name == "scarab.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 2;
            if (meshId == 2) return 0;
        }
        else if (m_name == "scorpion.mdl")
        {
            //if (meshId == 0) return 0;
            //if (meshId == 1) return 2;
            //if (meshId == 2) return 0;
            if (meshId == 3) return 2;
        }
        else if (m_name == "secutorImp.mdl")
        {
            if (meshId == 0) return 0; // teeth
            if (meshId == 1) return 1; // eyes
            if (meshId >= 2 && meshId <= 11) return 2;
            return 3;
        }
        else if (m_name == "secutorImpF.mdl")
        {
            if (meshId == 0) return 0; // eye
            if (meshId == 1) return 1; // teeth
            if (meshId == 2 || meshId == 13 || meshId == 16 || meshId == 17) return 2; // armour
            if (meshId == 18) return 5; // shield straps
            return 4;
        }
        else if (m_name == "secutorSte.mdl")
        {
            if (meshId == 0) return 0; // teeth
            if (meshId == 1) return 1; // eyes
                                       //if (meshId >= 2 && meshId <= 11) return 2;
            if (meshId == 12 || meshId == 13) return 2;
            if (meshId >= 4 && meshId <= 18) return 4;
            if (meshId == 19) return 5; // shielf straps
            return 2;
        }
        else if (m_name == "secutorSteF.mdl")
        {
            if (meshId == 0) return 0; // eye
            if (meshId == 1) return 1; // teeth
            if (meshId == 2 || meshId == 3 || meshId == 15 || meshId == 16 || meshId == 17) return 2; // armour
            if (meshId == 18) return 5; // shield straps
            return 5;
        }
        else if (m_name == "skeleton.mdl")
        {
        }
        else if (m_name == "skeletonExp1.mdl")
        {
            if (meshId == 0 || meshId == 1) return 0;
            return 2;
        }
        else if (m_name == "skeletonImp1.mdl")
        {
            if (meshId == 0 || meshId == 1) return 0;
            return 1;
        }
        else if (m_name == "skeletonImp2.mdl")
        {
            if (meshId == 0 || meshId == 1 || meshId == 3) return 3;
            if (meshId == 2) return 2;
            return 4;
        }
        else if (m_name == "skeletonNor1.mdl")
        {
            if (meshId == 0 || meshId == 1 || meshId == 2) return 2;
            if (meshId == 2) return 1;
            return 3;
        }
        else if (m_name == "skeletonNor2.mdl")
        {
            if (meshId == 0 || meshId == 1 || meshId == 2) return 0;
            return 2;
        }
        else if (m_name == "skeletonSte1.mdl")
        {
            if (meshId == 0 || meshId == 1) return 0;
            return 2;
        }
        else if (m_name == "skeletonSte2.mdl")
        {
            if (meshId == 0 || meshId == 1) return 0;
            return 2;
        }

        else if (m_name == "urlan.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 19) return 4;
            if (meshId >= 14 && meshId <= 18) return 2;
            return 1;
        }
        else if (m_name == "ursula.mdl")
        {
            if (meshId == 0) return 0;
            if (meshId == 1) return 1;
            if (meshId >= 2 && meshId <= 5) return 2;
            if (meshId >= 6 && meshId <= 7) return 3;
            if (meshId == 14 || meshId == 15) return 3;
            if (meshId == 16) return 6;
            return 4;
        }
        else if (m_name == "valens.mdl")
        {
            if (meshId == 0) return 0; // teeth
            if (meshId == 1 || meshId == 3 || meshId == 14 || meshId == 15 || meshId == 17) return 1;
            if (meshId == 16) return 3;
            if (meshId == 19) return 4; // eyes

            //if (meshId >= 14 && meshId <= 18) return 2;
            return 3;
        }
        else if (m_name == "wolf.mdl")
        {
            if (meshId == 2) return 1;
            //if (meshId == 3) return 2;
            return 0;
        }
        else if (m_name == "yeti.mdl")
        {
            if (meshId == 0 || meshId == 4) return 0;
            return 1;
        }


        return -1;
    }

    public int AdjustForModel(int adjustedIndex)
    {
        if (m_name == "southern_worldmap.mdl")
        {
            if (adjustedIndex == 0)
            {
                adjustedIndex = 1;
            }


        }
        if (m_name == "calthaArena")
        {
            if (adjustedIndex == 10)
            {
                adjustedIndex = 9;
            }
            else if (adjustedIndex == 13)
            {
                adjustedIndex = 12;
            }
        }
        else if (m_name.Contains("onon"))
        {
            if (adjustedIndex == 5)
            {
                adjustedIndex = 6;
            }
            else if (adjustedIndex > 5)
            {
                adjustedIndex += 2;
            }
            //if (adjustedIndex == 5)
            //{
            //    adjustedIndex = 6;
            //}
            //else if (adjustedIndex == 6)
            //{
            //    adjustedIndex = 8;
            //}
            //else if (adjustedIndex == 8)
            //{
            //    adjustedIndex = 10;
            //}
            //else if (adjustedIndex == 9)
            //{
            //    adjustedIndex = 11;
            //}

        }
        else if (m_name.Contains("galdr"))
        {
            if (adjustedIndex == 8)
            {
                adjustedIndex = 7;
            }
            else if (adjustedIndex == 9)
            {
                adjustedIndex = 8;
            }
            else if (adjustedIndex == 12)
            {
                adjustedIndex = 11;
            }

        }
        else if (m_name.Contains("wandering"))
        {
            if (adjustedIndex >= 15)
            {
                adjustedIndex--;
            }
        }
        else if (m_name.Contains("bloody"))
        {
            if (adjustedIndex == 18)
            {
                adjustedIndex = 10;
            }
            else if (adjustedIndex >= 13)
            {
                adjustedIndex += 1;
            }

            //else if (adjustedIndex == 10)
            //{
            //    adjustedIndex = 17;
            //}
            //if (adjustedIndex > 14 && adjustedIndex < 16)
            //{
            //    adjustedIndex--;
            //}
        }
        else if (m_name.Contains("valenssc"))
        {
            if (adjustedIndex >= 12)
            {
                adjustedIndex++;
            }
        }
        else if (m_name == "barbarian.mdl")
        {
            //if (adjustedIndex >= m_textureNames.Count)
            //{
            //    int ibreak = 0;
            //}
            //if (m_textureNames[adjustedIndex].Contains("skygold"))
            //{
            //    adjustedIndex--;
            //}
        }
        else if (m_name == "palaceibliis.mdl")
        {
            //int ibreak = 0;
            //string texname1 = m_textureNames[33];
            //string texname2 = m_textureNames[34];
            //if (adjustedIndex == 33)
            //{
            //    adjustedIndex = 32;
            //}
            //else if (adjustedIndex == 32)
            //{
            //    adjustedIndex = 33;
            //}
            //if (adjustedIndex < 5)
            //{
            //    adjustedIndex += 1;
            //}
        }

        return adjustedIndex;
    }


    public override void BuildBB()
    {
        MinBB = new IndexedVector3(float.MaxValue);
        MaxBB = new IndexedVector3(float.MinValue);
        foreach (ModelSubMesh subMesh in m_modelMeshes)
        {
            MinBB = IndexedVector3.Min(MinBB, subMesh.MinBB);
            MaxBB = IndexedVector3.Max(MaxBB, subMesh.MaxBB);
        }
    }

    public void ReadSKELSection(BinaryReader binReader)
    {
        if (CommonModelImporter.FindCharsInStream(binReader, CommonModelImporter.skelTag))
        {
            m_hasSkeleton = true;
            m_skinned = true;
            int blockSize = binReader.ReadInt32();
            int pad1 = binReader.ReadInt32();
            int pad2 = binReader.ReadInt32();
            int numBones = (blockSize - 16) / 32;

            for (int i = 0; i < numBones; ++i)
            {
                BoneNode node = BoneNode.FromStream(binReader);
                node.name = m_boneNames[i];
                m_boneNameDictionary[i] = node.name;
                List<string> names;
                //if (!BoneNode.pad1ByteNames.TryGetValue(node.Flags, out names))
                //{
                //    names = new List<string>();
                //    BoneNode.pad1ByteNames[node.flags] = names;
                //}
                //names.Add(node.name);
                BoneList.Add(node);
            }


            BuildSkeleton();
        }

    }

    //void CalcBindFinalIndexedMatrix(BoneNode bone, IndexedMatrix parentIndexedMatrix)
    //{
    //    bone.combinedIndexedMatrix = bone.localIndexedMatrix * parentIndexedMatrix;
    //    //bone.finalIndexedMatrix = bone.offsetIndexedMatrix * bone.combinedIndexedMatrix;
    //    bone.finalIndexedMatrix = bone.combinedIndexedMatrix;

    //    foreach (BoneNode child in bone.children)
    //    {
    //        CalcBindFinalIndexedMatrix(child, bone.combinedIndexedMatrix);
    //    }
    //}

    //public List<VertexPositionNormalTexture> m_points = new List<VertexPositionNormalTexture>();


    public void GetTextureId(int submeshIndex, out int originalMaterialId, out int adjustedMaterialId)
    {
        //SubMeshData1 data1 = m_subMeshData1List[submeshIndex];
        int materialBlockId = 0;
        if (submeshIndex < m_meshMaterialOffsetList.Count)
        {
            materialBlockId = m_meshMaterialOffsetList[submeshIndex].offset / s_materialBlockSize;
        }

        int adjustment = 0;

        originalMaterialId = materialBlockId;

        //int lookup = m_materialDataList[materialBlockId].diffuseTextureId / s_textureBlockSize;
        int lookup = originalMaterialId;

        //int adjustedIndex = AdjustForModel(materialBlockId);
        int adjustedIndex = TextureForMesh(submeshIndex);
        if (adjustedIndex == -1)
        {
            adjustedIndex = AdjustForModel(lookup);
        }


        if (adjustedIndex >= m_textureNames.Count)
        {
            adjustedIndex = m_textureNames.Count - 1;
        }

        adjustedMaterialId = adjustedIndex;
    }

    public int AdjustBone(int index, int subMeshIndex)
    {
        if (index != -1)
        {
            Debug.Assert(index % 3 == 0);
            index /= 3;

            int copy = index;
            if (index < m_subMeshData3.boneListInfo[subMeshIndex].Count)
            {
                index = m_subMeshData3.boneListInfo[subMeshIndex][index];
                //index -= 1;
            }
            else
            {
                int ibreak = 0;
            }
        }

        return index;
    }

}


public class SubMeshData1
{
    public int zero1;
    public int zero2;
    public int counter1;
    public int counter2;

    public int minus1;

    public Vector4 boundingSphere;

    public int pad1;
    public int pad2;
    public int pad3;
    public int pad4;
    public int pad5;

    public int LodLevel;
    public static HashSet<int> s_lodLevels = new HashSet<int>();


    public static SubMeshData1 FromStream(BinaryReader binReader)
    {
        SubMeshData1 smd = new SubMeshData1();
        smd.zero1 = binReader.ReadInt32();
        Debug.Assert(smd.zero1 == 0);
        smd.zero2 = binReader.ReadInt32();
        smd.counter1 = binReader.ReadInt32();
        //smd.counter2 = binReader.ReadInt32();

        smd.boundingSphere.w = binReader.ReadSingle();
        smd.boundingSphere.x = binReader.ReadSingle();
        smd.boundingSphere.y = binReader.ReadSingle();
        smd.boundingSphere.z = binReader.ReadSingle();

        smd.pad1 = binReader.ReadInt32();
        smd.pad2 = binReader.ReadInt32();
        smd.pad3 = binReader.ReadInt32();
        smd.pad4 = binReader.ReadInt32();
        smd.pad5 = binReader.ReadInt32();

        smd.LodLevel = binReader.ReadInt32();
        smd.minus1 = binReader.ReadInt32();
        s_lodLevels.Add(smd.LodLevel);
        return smd;
    }

    public String PrintTidy(byte[] data)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.Length; ++i)
        {
            sb.Append("[" + data[i] + "]\t");
        }
        return sb.ToString();
    }


    public void WriteInfo(StreamWriter sw)
    {
        //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}][{7}][{8}][{9}][{10}] a[{11}] b[{12}] c[{13}] d[{14}] lod[{15}]", zero1, zero2, counter1, , pad1, pad2, pad3, pad4, pad5a, pad5b, pad5c, pad5d, LodLevel));
        //sw.WriteLine(String.Format("[{0}][{1}] counter[{2}] f1[{3}][{4}][{5}] f4[{6}] pad1[{7}][{8}][{9}][{10}] pad5[{11}] lod[{12}]", zero1, zero2, counter1, f1, f2, f3, f4, pad1, pad2, pad3, pad4, pad5, LodLevel));
        //sw.WriteLine(String.Format("[{0}][{1}] counter[{2}] f1[{3:0.0000}][{4:0.0000}][{5:0.0000}] f4[{6:0.0000}] pad1[{7}][{8}][{9}][{10}] pad5[{11}] lod[{12}]", zero1, zero2, counter1, f1, f2, f3, f4, pad1, pad2, pad3, pad4, pad5, LodLevel));
        //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}][{4}]", zero1, zero2, counter1,PrintTidy(usefulBytes),PrintTidy(padBytes),LodLevel));
        //sw.WriteLine(String.Format("[{0}][{1}][{2}][{3}]",f1a,f1b,f1c,f1d));
    }


}

public class SubMeshData2
{
    public float val1;
    public int StartOffset;
    public int val2;
    public int NumIndices;

    public int val3;
    public int pad;
    public byte[] padBytes;
    public List<int> indices = new List<int>();
    public int MinVertex = int.MaxValue;
    public int MaxVertex = int.MinValue;
    public string fbxNodeId;
    public string fbxNodeName;

    public static SubMeshData2 FromStream(BinaryReader binReader)
    {
        SubMeshData2 smd = new SubMeshData2();
        smd.val1 = binReader.ReadSingle();
        smd.StartOffset = binReader.ReadInt32();
        smd.val2 = binReader.ReadInt32();
        smd.NumIndices = binReader.ReadInt32();
        //smd.NumIndices *= 2;
        smd.val3 = binReader.ReadInt32();
        return smd;
    }
    public void WriteInfo(StreamWriter sw)
    {
        sw.WriteLine(String.Format("SO[{0}] NI[{1}] V1[{2}] V2[{3}] V3[{4}] pbl[{5}]", StartOffset, NumIndices, val1, val2, val3, padBytes.Length));
    }

    public void BuildMinMax()
    {
        for (int i = 0; i < indices.Count; ++i)
        {
            MinVertex = Math.Min(MinVertex, indices[i]);
            MaxVertex = Math.Max(MaxVertex, indices[i]);
        }

    }

}

public class SubMeshData3
{
    public List<int> initialValsList = new List<int>();
    public List<int> list1 = new List<int>();
    //public List<MaterialBlock> materialBlockList = new List<MaterialBlock>();
    public float val7;

    public int lastElementOffset;
    public int headerEnd2;
    public int headerEnd3;
    public int headerEnd4;
    public int headerEnd5;
    public float headerEnd6;
    public int[] headerEndZero = new int[8];
    public int[] headerEndZero2 = new int[3];

    public List<List<byte>> boneListInfo = new List<List<byte>>();

    public int mmoSearchStartPos = -1;
    public int mmoSearchEndPos = -1;
    public byte[] mmoData = null;
    public List<int> offsetList = new List<int>();
    public XboxModel model;

    public void WriteInfo(StreamWriter sw)
    {
        sw.WriteLine("**********************************************************************");
        sw.WriteLine(model.m_name);
        sw.WriteLine();
        foreach (int val in initialValsList)
        {
            sw.Write(val);
            sw.Write(",");
        }
        sw.WriteLine();
        //foreach (int[] ia in model.m_meshMaterialList)
        //{
        //    sw.WriteLine(String.Format("{0,10} {1,10} {2,10}", ia[0], ia[1], ia[2]));
        //}
        sw.WriteLine("**********************************************************************");
        foreach (MaterialData smd4 in model.m_materialDataList)
        {
            smd4.WriteInfo(sw);
        }
    }

    //1073742880 is 2.0 (ish)
    //1065353216 is 1.0 (ish)
    // read those vals to sensible floats.


    public static SubMeshData3 FromStream(XboxModel model, BinaryReader binReader, int numMeshes, int endOffset, bool skinned)
    {
        SubMeshData3 smd3 = new SubMeshData3();
        smd3.model = model;
        int maxOffset = 0;

        if (skinned)
        {
            byte[] boneSearchBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x14 };
            if (CommonModelImporter.FindCharsInStream(binReader, boneSearchBytes))
            {
                // jump to bone data.
                //binReader.BaseStream.Position += 40;
                binReader.BaseStream.Position += 51;
                // this may specify number per mesh?
                for (int i = 0; i < numMeshes; ++i)
                {
                    List<byte> boneList = new List<byte>();
                    smd3.boneListInfo.Add(boneList);
                    int numEntries = binReader.ReadByte();
                    for (int j = 0; j < numEntries; ++j)
                    {
                        boneList.Add(binReader.ReadByte());
                    }
                }
            }
        }

        smd3.mmoSearchStartPos = (int)binReader.BaseStream.Position;

        while (true)
        {
            int val1 = binReader.ReadInt32();
            if (val1 == 0)
            {
                int val2 = binReader.ReadInt32();
                if (val2 == 12 || val2 == 0)
                {
                    break;
                }
                else
                {
                    binReader.BaseStream.Position -= 4;
                }
            }
        }
        // This works for both skinned and none skinned objects to find the mesh material offsets.
        binReader.BaseStream.Position -= (2 * 4);
        for (int i = 0; i < smd3.model.m_subMeshData1List.Count; ++i)
        {
            smd3.offsetList.Add(binReader.ReadInt32());
        }
        binReader.BaseStream.Position = smd3.mmoSearchStartPos;
        byte[] texSearchBytes = new byte[] { 0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x00, 0x00 };
        if (Common.FindCharsInStream(binReader, texSearchBytes))
        {
            binReader.BaseStream.Position -= texSearchBytes.Length;
            MeshMaterialOffsets mm = null;
            do
            {
                mm = MeshMaterialOffsets.FromStream(binReader);
                if (mm != null)
                {
                    model.m_meshMaterialOffsetList.Add(mm);
                    maxOffset = Math.Max(maxOffset, mm.offset);
                }
            }
            while (mm != null);
        }
        smd3.mmoSearchEndPos = (int)binReader.BaseStream.Position;
        binReader.BaseStream.Position = smd3.mmoSearchStartPos;
        smd3.mmoData = binReader.ReadBytes(smd3.mmoSearchEndPos - smd3.mmoSearchStartPos);

        if (model.m_meshMaterialOffsetList.Count == 0)
        {
            MeshMaterialOffsets mmo = new MeshMaterialOffsets();
            mmo.offset = 0;
            model.m_meshMaterialOffsetList.Add(mmo);
        }

        //for (int i = 0; i < model.m_meshMaterialOffsetList.Count; ++i)
        //{
        //    int val = model.m_meshMaterialOffsetList[i].offset / XboxModel.s_materialBlockSize;
        //    if (!model.m_rebuildOffsets.Contains(val))
        //    {
        //        model.m_rebuildOffsets.Add(val);
        //    }
        //}

        Debug.Assert(maxOffset % XboxModel.s_materialBlockSize == 0);
        maxOffset /= XboxModel.s_materialBlockSize;
        maxOffset += 1;
        int ibreak2 = 0;

        for (int i = 0; i < maxOffset; ++i)
        {
            model.m_materialBlockList.Add(MaterialBlock.FromStream(binReader));
        }

        smd3.lastElementOffset = binReader.ReadInt32();
        smd3.headerEnd2 = binReader.ReadInt32();
        smd3.headerEnd3 = binReader.ReadInt32();
        smd3.headerEnd4 = binReader.ReadInt32();
        smd3.headerEnd5 = binReader.ReadInt32();
        smd3.headerEnd6 = binReader.ReadSingle();
        //Debug.Assert(smd3.headerEnd6 == 1.0f || smd3.headerEnd6 == 0.0f);
        for (int i = 0; i < smd3.headerEndZero.Length; ++i)
        {
            smd3.headerEndZero[i] = binReader.ReadInt32();
            //Debug.Assert(smd3.headerEndZero[i] == 0);
        }
        //maxOffset -= 1;

        //float[] toFind = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        //int index = 0;
        //while (CommonModelImporter.PositionAtFloats(binReader, toFind) && index < model.m_materialBlockList.Count)
        //{
        //    int startVal = model.m_materialBlockList[index].Offset;
        //    int endVal = index < model.m_materialBlockList.Count - 1 ? model.m_materialBlockList[index + 1].Offset : startVal + (28 * 4);//100;
        //    int sectionLength = endVal - startVal;
        //    MaterialBlock materialBlock = model.m_materialBlockList[index];

        //    model.m_materialDataList.Add(MaterialData.FromStream(binReader, numMeshes, sectionLength, materialBlock));
        //    ++index;
        //}
        long positionRecord = binReader.BaseStream.Position;
        ReadMaterial2(model, binReader);

        //binReader.BaseStream.Position = positionRecord;
        //float[] toFind = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        //int index = 0;
        //while (Common.PositionAtFloats(binReader, toFind) && index < model.m_materialBlockList.Count)
        //{
        //    int startVal = model.m_materialBlockList[index].Offset;
        //    int endVal = index < model.m_materialBlockList.Count - 1 ? model.m_materialBlockList[index + 1].Offset : startVal + (28 * 4);//100;
        //    int sectionLength = endVal - startVal;
        //    MaterialBlock materialBlock = model.m_materialBlockList[index];

        //    model.m_materialDataList.Add(MaterialData.FromStream(binReader, numMeshes, sectionLength));
        //    ++index;
        //}
        int index = 0;
        foreach (List<byte> bytesList in model.m_materialDataByteBlocks)
        {
            int startVal = model.m_materialBlockList[index].Offset;
            int endVal = index < model.m_materialBlockList.Count - 1 ? model.m_materialBlockList[index + 1].Offset : startVal + (28 * 4);//100;
            int sectionLength = endVal - startVal;
            model.m_materialDataList.Add(MaterialData.FromByteBlock(bytesList, numMeshes, sectionLength));
            index++;
        }

        return smd3;
    }

    public static void ReadMaterial2(XboxModel model, BinaryReader binReader)
    {

        for (int i = 0; i < model.m_materialBlockList.Count; ++i)
        {
            int size = 0;
            if (i < model.m_materialBlockList.Count - 1)
            {
                size = model.m_materialBlockList[i + 1].Offset - model.m_materialBlockList[i].Offset;
            }
            else
            {
                //int endVal = index < model.m_materialBlockList.Count - 1 ? model.m_materialBlockList[index + 1].Offset : startVal + (28 * 4);//100;
                size = (28 * 4);
            }
            List<byte> byteList = new List<byte>();
            byteList.AddRange(binReader.ReadBytes(size));
            model.m_materialDataByteBlocks.Add(byteList);
        }

    }
}




    public class MeshMaterialOffsets
{
    public float val1;
    public int always1;
    public int offset;
    public int convertedOffset;

    public static MeshMaterialOffsets FromStream(BinaryReader binReader)
    {
        MeshMaterialOffsets meshMaterial = new MeshMaterialOffsets();
        meshMaterial.val1 = binReader.ReadSingle();
        if (meshMaterial.val1 == 1.0f)
        {
            meshMaterial.always1 = binReader.ReadInt32();
            Debug.Assert(meshMaterial.always1 == 1);
            meshMaterial.offset = binReader.ReadInt32();
            Debug.Assert(meshMaterial.offset % XboxModel.s_materialBlockSize == 0);
            meshMaterial.convertedOffset = meshMaterial.offset / XboxModel.s_materialBlockSize;
        }
        else
        {
            binReader.BaseStream.Position -= 4;
            meshMaterial = null;
        }

        return meshMaterial;
    }
}

public class MaterialBlock
{
    // s_materialBlockSize (44) / 4 == 11
    public int[] blockData = new int[11];

    public int Offset
    {
        get { return blockData[5]; }
    }

    public int Lod
    {
        get { return blockData[4]; }
    }


    public static MaterialBlock FromStream(BinaryReader binReader)
    {
        MaterialBlock materialBlock = new MaterialBlock();
        for (int i = 0; i < materialBlock.blockData.Length; ++i)
        {
            materialBlock.blockData[i] = binReader.ReadInt32();
        }
        return materialBlock;
    }
}

public class SubmeshData
{
    public int index = 0;
    public int originalMeshIndex = 0;
    public SubMeshData1 subMeshData;
    public List<int> indices;
    public List<int> indicesTest = new List<int>();
    public List<int> adjustedIndices = new List<int>();

    public int startIndex = -1;
    public int endIndex = -1;
    public int lowestIndex = -1;

    public List<int> verticesInMesh = new List<int>();
    public List<int> boneIds = new List<int>();
    public Dictionary<int, int> boneIdMap = new Dictionary<int, int>();
    public Dictionary<int, int> reverseBoneIdMap = new Dictionary<int, int>();
    public MeshMaterialOffsets meshMaterial;

    public int originalMaterialId;
    public int materialId;

    public void AddIndexAndWeight(int boneId, int vertexIndex, float weight)
    {
        List<int> indexList = null;
        List<float> floatList = null;

        if (!weightVertexIndices.TryGetValue(boneId, out indexList))
        {
            indexList = new List<int>();
            weightVertexIndices[boneId] = indexList;

        }

        if (!weightVertexWeights.TryGetValue(boneId, out floatList))
        {
            floatList = new List<float>();
            weightVertexWeights[boneId] = floatList;

        }
        indexList.Add(vertexIndex);
        floatList.Add(weight);

    }

    public void GetWeights(int boneId, out List<int> indices, out List<float> weights)
    {
        weightVertexIndices.TryGetValue(boneId, out indices);
        weightVertexWeights.TryGetValue(boneId, out weights);
    }

    public Dictionary<int, List<int>> weightVertexIndices = new Dictionary<int, List<int>>();
    public Dictionary<int, List<float>> weightVertexWeights = new Dictionary<int, List<float>>();



    public String Name
    {
        get
        {
            return "Mesh" + index;
        }
    }


    public CommonMeshData ToCommon()
    {
        CommonMeshData cmd = new CommonMeshData();
        cmd.Name = Name;
        cmd.Index = index;
        cmd.MaterialId = materialId;
        cmd.Indices = adjustedIndices;
        cmd.Vertices = verticesInMesh;
        cmd.boundingSphere = subMeshData.boundingSphere;
        cmd.LodLevel = subMeshData.LodLevel;
        return cmd;
    }

}

public class BoneDistance
{
    public BoneNode m_boneNode;
    public float m_dist2;
}


public class DoegData
{
    public int Length;
    public int Unknown1;
    public int Unknown2;
    public int Unknown3;
    public int Unknown4;
    public int Unknown5;

    public int NumMeshes;
    public int NumMeshesCopy;
    public int NumTextures;
    public int NumMaterials;

    public int Block1Start;
    public int Block2Start;
    public int Block3Start;
    public int Block4Start;
    public int Block5Start;

    public int SkinIndicator;

    public int Unknown6;
    public int Unknown7;
    public int Unknown8;
    public int Unknown9;

    public int TextureBlockOffset;

    public static DoegData FromStream(BinaryReader binReader)
    {
        DoegData doegData = new DoegData();
        byte[] doegStart = binReader.ReadBytes(4);
        Debug.Assert(doegStart[0] == 'd' && doegStart[3] == 'g');
        if (doegStart[0] == 'd' && doegStart[3] == 'g')
        {
            doegData.Length = binReader.ReadInt32();
            Debug.Assert(doegData.Length == 0x7c);
            int numElements = binReader.ReadInt32();
            Debug.Assert(numElements == 0x01);
            doegData.Unknown1 = binReader.ReadInt32();
            doegData.Unknown2 = binReader.ReadInt32();
            doegData.Unknown3 = binReader.ReadInt32();
            Debug.Assert(doegData.Unknown3 == 0x01);

            doegData.NumMeshes = binReader.ReadInt32();
            doegData.Unknown4 = binReader.ReadInt32();
            doegData.NumMeshesCopy = binReader.ReadInt32();
            Debug.Assert(doegData.NumMeshes == doegData.NumMeshesCopy);
            doegData.NumTextures = binReader.ReadInt32();
            doegData.NumMaterials = binReader.ReadInt32();
            doegData.Unknown5 = binReader.ReadInt32();
            Debug.Assert(doegData.Unknown5 == 0x00);


            doegData.Block1Start = binReader.ReadInt32();
            Debug.Assert(doegData.Block1Start == 0x70);
            doegData.Block2Start = binReader.ReadInt32();
            Debug.Assert(doegData.Block2Start - doegData.Block1Start == (doegData.NumMeshes * 4));
            doegData.Block3Start = binReader.ReadInt32();
            Debug.Assert(doegData.Block3Start - doegData.Block2Start == (doegData.NumMeshes * 56));
            doegData.Block4Start = binReader.ReadInt32();
            Debug.Assert(doegData.Block4Start - doegData.Block3Start == (doegData.NumMeshes * 20));
            doegData.Block5Start = binReader.ReadInt32();
            //Debug.Assert(doegData.Block5Start - doegData.Block4Start == (doegData.NumMeshes * 20));
            Debug.Assert(doegData.Block5Start - doegData.Block4Start == (doegData.Unknown4 * 20));

            int val = doegData.Block5Start - doegData.Block4Start;

            // if this is not -1 then it's some information on skinning/anims?
            doegData.SkinIndicator = binReader.ReadInt32();


            doegData.Unknown7 = binReader.ReadInt32();
            doegData.Unknown8 = binReader.ReadInt32();
            doegData.Unknown9 = binReader.ReadInt32();

            int minus1a = binReader.ReadInt32();
            int minus1b = binReader.ReadInt32();
            int minus1c = binReader.ReadInt32();
            int minus1d = binReader.ReadInt32();

            Debug.Assert(minus1a == -1 && minus1b == -1 && minus1c == -1 && minus1d == -1);

            int unk1r = binReader.ReadInt32();
            int unk1s = binReader.ReadInt32();
            int unk1t = binReader.ReadInt32();
            int unk1u = binReader.ReadInt32();

            int minus1e = binReader.ReadInt32();

            Debug.Assert(minus1a == -1 && minus1b == -1 && minus1c == -1 && minus1d == -1 && minus1e == -1);

            // this is the number of bytes from here to the beginning of the texture names if you include the end doeg section (4)

            doegData.TextureBlockOffset = binReader.ReadInt32();

            byte[] doegEnd = binReader.ReadBytes(4);
            Debug.Assert(doegEnd[0] == 'd' && doegEnd[3] == 'g');
            //int doegEndVal = (int)(binReader.BaseStream.Position - 4);
        }
        return doegData;


    }
    public void ToStream(StreamWriter sw)
    {
        sw.WriteLine(String.Format("NumMeshes [{0:0000}] NumTextures [{1:0000}] NumMaterials [{2:0000}]  Unk4[{3:00000000}] B1[{4:00000000}] B2[{5:00000000}] B3[{6:00000000}] B4[{7:00000000}] B6[{8:00000000}] Skin[{9}]",
            NumMeshes, NumTextures, NumMaterials, Unknown4, Block1Start, Block2Start, Block3Start, Block4Start, Block5Start, SkinIndicator));
    }

}


public class XRNDSection
{
    public int sectionLength;
    public int uk2a;
    public int numEntries;
    public int numSkip;
    public DoegData m_doegData;
    public List<int> blockOneValues = new List<int>();
    public int TotalVertices1;
    public int TotalVertices2;
    public void WriteInfo(StreamWriter sw)
    {
        sw.WriteLine(String.Format("XRND : SL[{0}] NE[{1}] ns[{2}] Tv1[{3}] TV2[{4}] Block1[{5}]", sectionLength, numEntries, numSkip, TotalVertices1, TotalVertices2, blockOneValues.Count));
        foreach (int i in blockOneValues)
        {
            sw.Write(i);
            sw.Write(",");
        }
        sw.WriteLine();
    }
}
