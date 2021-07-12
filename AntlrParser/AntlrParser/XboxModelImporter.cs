using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

public class XboxModelReader : BaseModelReader
{
    public XboxModelReader()
    {
    }

    public override BaseModel LoadSingleModel(string modelPath, bool readDisplayLists = true)
    {
        FileInfo fileInfo = new FileInfo(modelPath);
        XboxModel model = new XboxModel(fileInfo.Name);
        using (BinaryReader binReader = new BinaryReader(new FileStream(modelPath, FileMode.Open)))
        {
            model.LoadData(binReader);
        }
        return model;
    }

    public void LoadModels(String sourceDirectory, String infoFile, String searchString,int maxFiles = -1)
    {
        m_models.Clear();
        String[] files = Directory.GetFiles(sourceDirectory, searchString, SearchOption.AllDirectories);
        int counter = 0;

        foreach (String file in files)
        {
            if (file.Contains("extraanim") || file.Contains("bonename") || file.Contains("meta"))
            {
                continue;
            }

            try
            {
                FileInfo sourceFile = new FileInfo(file);

                XboxModel model = LoadSingleModel(sourceFile.FullName) as XboxModel;
                CommonModelData commonModel = model.ToCommon(null);

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

    //public DoegData m_doegData;
    public List<List<byte>> m_materialDataByteBlocks = new List<List<byte>>();
    //    public List<OBBTEntry> m_obbtEntries = new List<OBBTEntry>();

    //public List<byte> m_obbtBytes = new List<byte>();
    public byte[] m_obbtBytes;
    public List<IndexedMatrix> m_obbtValues = new List<IndexedMatrix>();
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
        SwapLeftRight = true;
    }

    public void DebugOBBT(StreamWriter sw)
    {
        sw.WriteLine("OBBT Bytes : " + m_obbtBytes.Length);
        sw.WriteLine("OBBT Matrix : " + m_obbtValues.Count);
        sw.WriteLine("NumMeshes : " + m_subMeshData1List.Count);
        //foreach (byte b in m_obbtBytes)
        //{
        //    sw.Write(b);
        //    sw.Write(",");
        //}

        List<short> temp = new List<short>();
        List<short> temp2 = new List<short>();
        sw.WriteLine();
        for(int i=0;i<m_obbtBytes.Length;i+=2)
        {
            short val = Common.ToInt16(m_obbtBytes, i);
            if (val != 26985)
            {
                temp.Add(val);
            }
            if(val < 0)
            {
                temp2.Add(val);
            }
            //temp2.Add(Common.ToInt16BigEndian(m_obbtBytes, i));
        }

        short minValue = temp.Min();
        short maxValue = temp.Max();    
        sw.WriteLine("Size : " + temp.Count);
        sw.WriteLine("Mod2 : " + ""+((temp.Count % 2) == 0));
        sw.WriteLine("Mod3 : " + "" + ((temp.Count % 3) == 0));
        sw.WriteLine("Mod4 : " + "" + ((temp.Count % 4) == 0));
        sw.WriteLine("Min : " + minValue);
        sw.WriteLine("Max : " + maxValue);
        
        //sw.Write();
        //sw.Write(",")
        WriteShorts("BE",sw,temp);
        //WriteShorts("LE",sw, temp2);

        OBBTTreeNode rootNode = new OBBTTreeNode();
        OBBTTreeNode currentNode = rootNode;


        foreach (short s in temp)
        {
            if(s < 0)
            {
                if (currentNode.meshNodes.Count > 0)
                {
                    if (currentNode.left == null)
                    {
                        currentNode.left = new OBBTTreeNode();
                        currentNode.left.matrixIndex = s;
                        currentNode.left.parent = currentNode;
                        //currentNode = currentNode.left;
                    }
                    else if (currentNode.right == null)
                    {
                        currentNode.right = new OBBTTreeNode();
                        currentNode.right.matrixIndex = s;
                        currentNode.right.parent = currentNode;
                        //currentNode = currentNode.right;
                    }
                }
                else
                {
                    currentNode = currentNode.parent;
                }


                //sw.WriteLine();
                //sw.WriteLine("Node : " + s);
                //IndexedMatrix im = m_obbtValues[s * -1];
                //sw.Write("Pos : ");
                //Common.WriteFloat(sw, im._origin);
                //IndexedVector3 v3 = new IndexedVector3(im.M14, im.M24, im.M34);
                //sw.WriteLine(String.Format("Extents : {0:0.00000000}", v3.Length()));
                //sw.Write("\t");
            }
            else
            {
                currentNode.meshNodes.Add(s);
            }
        }


        //int step = 1;
        //int count2 = 0;
        //while(count2 < temp2.Count)
        //{
        //    for(int i=0;i<step;++i)
        //    {
        //        sw.Write("" + temp2[count2]);
        //        sw.Write(",");
        //        count2++;
        //    }
        //    sw.WriteLine("");
        //    step *= 2;
        //}

        //temp.Sort();
        //temp2.Sort();
        //WriteShorts("BESorted",sw, temp);
        //WriteShorts("LESorted", sw, temp2);



        int count = 0;
        foreach(IndexedMatrix im in m_obbtValues)
        {
            IndexedVector3 v3 = new IndexedVector3(im.M14, im.M24, im.M34);
            sw.WriteLine(String.Format("Extents : {0:0.00000000}",v3.Length()));
            sw.WriteLine("" + (count++));
            sw.WriteLine(Common.ToStringF(im));
        }

        //foreach(OBBTEntry entry in m_obbtEntries)
        //{
        //    sw.WriteLine(entry.keyVal);
        //    foreach(short s in entry.dataVals)
        //    {
        //        sw.Write(s);
        //        sw.Write(",");
        //    }
        //    sw.WriteLine("");
        //}
    }

    public static void WriteShorts(String debug,StreamWriter sw,List<short> shorts)
    {
        HashSet<short> set = new HashSet<short>();
        sw.WriteLine(debug); 
        foreach (short s in shorts)
        {
            sw.Write(s);
            sw.Write(",");
            set.Add(s);
        }
        if(set.Count != shorts.Count)
        {
            int ibreak = 0;
        }
        sw.WriteLine();
    }


    public CommonModelData ToCommon(JSONModelData jsonModelData)
    {
        CommonModelData cmd = new CommonModelData();
        cmd.XBoxModel = this;
        cmd.BoneList = BoneList;
        cmd.RootBone = m_rootBone;
        cmd.BoneIdDictionary = m_boneIdDictionary;
        cmd.Name = m_name;
        cmd.VertexDataLists = new List<VertexDataAndDesc>();
        cmd.VertexDataLists.Add(m_vertexDataAndDesc);
        cmd.Skinned = m_skinned;
        cmd.IndexDataList = new List<List<int>>();
        cmd.IndexDataList.Add(m_allIndices);
        foreach (MaterialData md in m_materialDataList)
        {
            cmd.CommonMaterials.Add(md.ToCommon());
        }

        List<int> meshIdList = new List<int>();
        Dictionary<string, int> materialLookup = new Dictionary<string, int>();

        if (jsonModelData != null)
        {
            // we're going to create new ones based on our json data.
            cmd.CommonMaterials.Clear();

            int materialIdCount = 0;

            foreach (JSONMeshMaterial mm in jsonModelData.meshMaterials)
            {
                CommonMaterialData commonMaterial = new CommonMaterialData();
                commonMaterial.name = mm.materialName;
                commonMaterial.diffuseTextureData = new CommonTextureData();
                commonMaterial.diffuseTextureData.textureName = mm.textureDiffuse;
                commonMaterial.diffuseTextureData.fullPathName = commonMaterial.diffuseTextureData.textureName;
                if (!String.IsNullOrEmpty(mm.textureSpecular))
                {
                    commonMaterial.specularTextureData = new CommonTextureData();
                    commonMaterial.specularTextureData.textureName = mm.textureSpecular;
                    commonMaterial.specularTextureData.fullPathName = commonMaterial.specularTextureData.textureName;
                }

                materialLookup[mm.materialName] = materialIdCount++;
                cmd.CommonMaterials.Add(commonMaterial);
                foreach (int meshId in mm.MeshIds)
                {
                    meshIdList.Add(meshId);
                }
            }

        }
        else
        {
            for (int i = 0; i < m_subMeshData1List.Count; ++i)
            {
                meshIdList.Add(i);
            }
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

        if (jsonModelData != null)
        {
            // fixup materials?
            foreach (CommonMeshData meshData in cmd.CommonMeshData)
            {
                string materialName = jsonModelData.GetMaterial(meshData.Index);
                int materialIndex = materialLookup[materialName];
                meshData.MaterialId = materialIndex;
            }
        }




        return cmd;
    }





    public override void LoadData(BinaryReader binReader)
    {
        binReader.BaseStream.Position = 0;
        binReader.BaseStream.Position = 0;
        CommonModelImporter.ReadNullSeparatedNames(binReader, CommonModelImporter.nameTag, m_boneNames);
        binReader.BaseStream.Position = 0;
        ReadOBBTSection(binReader);
        binReader.BaseStream.Position = 0;
        ReadTextureSection(binReader);
        binReader.BaseStream.Position = 0;
        ReadSKELSection(binReader);
        binReader.BaseStream.Position = 0;
        ReadXRNDSection(binReader);
        //LoadAnimationData();
        //DumpSkeletonAndAnimation();

    }



    public void ReadOBBTSection(BinaryReader binReader)
    {
        if (Common.FindCharsInStream(binReader, Common.obbtTag))
        {
            int sectionLength = binReader.ReadInt32();
            int pad1 = binReader.ReadInt32();
            int numItems = binReader.ReadInt32();

            int startPos = (int)binReader.BaseStream.Position - 16;
            int endPos = startPos + sectionLength;

            // read matrixes...
            //int currentPos = endPos - 64;
            //while (currentPos > startPos)
            //{
            //    binReader.BaseStream.Position = currentPos;
            //    IndexedMatrix m = Common.FromStreamMatrixBE(binReader);
            //    //Vector3 forward = new Vector3(m.m00, m.m01, m.m02);
            //    IndexedVector3 forward = m.Forward;
            //    if (Common.FuzzyEquals(forward.LengthSquared(), 1.0f, 0.01f))
            //    {
            //        m_obbtValues.Add(m);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //    currentPos -= 64;
            //}

            //int matrixStartPos = currentPos;

            // read sections
            binReader.BaseStream.Position = startPos + 16;
            //while (binReader.BaseStream.Position < matrixStartPos)

            m_obbtBytes = binReader.ReadBytes(numItems);

            int numMatrices = sectionLength - 16 - numItems;
            numMatrices /= 64;
            for(int i=0;i<numMatrices;++i)
            {
                //IndexedMatrix m = Common.FromStreamMatrixBE(binReader);
                IndexedMatrix m = Common.FromStreamMatrix(binReader);
                //Vector3 forward = new Vector3(m.m00, m.m01, m.m02);
                IndexedVector3 forward = m.Forward;
                if (Common.FuzzyEquals(forward.LengthSquared(), 1.0f, 0.01f))
                {
                    m_obbtValues.Add(m);
                }
                else
                {
                    break;
                }
            }

            if(binReader.BaseStream.Position != endPos)
            {
                int ibreak = 0;
            }



            //for(int i=0;i<numItems;++i)
            //{
            //    short keyVal = binReader.ReadInt16();
            //    OBBTEntry obbe = new OBBTEntry();
            //    m_obbtEntries.Add(obbe);
            //    obbe.keyVal = keyVal;
            //    short dataVal = 0;
            //    do
            //    {
            //        dataVal = binReader.ReadInt16();
            //        if (dataVal < 0)
            //        {
            //            obbe.dataVals.Add(dataVal);
            //        }
            //        else
            //        {
            //            binReader.BaseStream.Position -= 2;
            //            break;
            //        }
            //    }
            //    while (dataVal < 0);
            //}

            int ibreak2 = 0;
        }

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

            // and set things back to where they were before extra vertex block
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


            }

            bool wrappedInt16 = false;
            int lastIndex = -1;
            int lastIndexWrapVal = 0;

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
                    if(m_xrndSection.TotalVertices2 > 0 && m_xrndSection.TotalVertices2 != m_xrndSection.TotalVertices1)
                    {
                        totalVertices += m_xrndSection.TotalVertices2;
                    }

                    m_avgVertex = diff / totalVertices;
                    binReader.BaseStream.Position = testPosition;

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
                swap = false;

                int end = startIndex + headerBlock.NumIndices - 2;
                if (!includeList.Contains(submeshIndex))
                {
                    startIndex += headerBlock.NumIndices;
                    continue;
                }

                SubmeshData smi = new SubmeshData();
                smi.index = submeshIndex;
                smi.originalMeshIndex = submeshIndex;
                smi.subMeshData = data1;
                smi.indices = new List<int>();

                //int materialIndex = Math.Min(submeshIndex, m_meshMaterialOffsetList.Count - 1);

                //smi.meshMaterial = m_meshMaterialOffsetList[materialIndex];
                ////GetTextureId(smi.index, out smi.originalMaterialId, out smi.materialId);
                //smi.originalMaterialId = smi.meshMaterial.convertedOffset;
                //smi.materialId = smi.originalMaterialId;


                //SubMeshData1 data1 = m_subMeshData1List[submeshIndex];
                int materialBlockId = 0;

                int offsetId = m_subMeshData3.offsetList[submeshIndex] / s_mmoOffsetBlockSize;
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

                    if (SwapLeftRight)
                    {
                        index1 = i;
                        index2 = i + 1;
                        index3 = i + 2;
                    }

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

                bool validMesh = true;
                int lookup = smi.materialId;

                while (lookup < m_materialDataList.Count - 1)
                {
                    MaterialData materialData = m_materialDataList[lookup];
                    // find the next valid material?
                    if (materialData.m_materialSlotInfoList.Count == 0)
                    {
                        //validMesh = false;
                        lookup++;
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
            catch(Exception e)
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
                vpnt.Position = p;
                vpnt.UV = u;
                vpnt.Normal = normV;
                vpnt.UV = u;
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
                vpnt.Position = p;
                vpnt.UV = u;
                vpnt.Normal = normV;
                vpnt.ExtraData = -1;
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
                vpnt.Position = p;
                vpnt.UV = u;
                vpnt.UV2 = u2;
                vpnt.Normal = normV;
                vpnt.ExtraData = unk1;
                desc.VertexData.Add(vpnt);
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }
        int ibreak2 = 0;
    }

    public bool SwapLeftRight = true;


    public void ReadSkinnedVertexData28(BinaryReader binReader, VertexDataAndDesc desc, int numVertices)
    {
        desc.Description = "SkinnedVertexData28";
        for (int i = 0; i < numVertices; ++i)
        {
            try
            {
                CommonVertexInstance vpnt = new CommonVertexInstance();
                vpnt.Position = CommonModelImporter.FromStreamVector3(binReader);
                if (SwapLeftRight)
                {
                    vpnt.Position = new IndexedVector3(-vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z);
                }


                //vpnt.BoneInfo1 = binReader.ReadInt32();
                vpnt.Normal = UncompressNormal(binReader.ReadInt32());
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

                if (SwapLeftRight)
                {
                    vpnt.Position = new IndexedVector3(-vpnt.Position.X, vpnt.Position.Y, vpnt.Position.Z);
                }


                vpnt.Normal = UncompressNormal(binReader.ReadInt32());
                vpnt.Tangent = UncompressNormal(binReader.ReadInt32());
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
    public IndexedVector3 UncompressNormal(int cv)
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
                BoneNode node = BoneNode.FromStream(binReader,SwapLeftRight);

                node.name = m_boneNames[i];
                m_boneNameDictionary[i] = node.name;
                List<string> names;
                if (!BoneNode.pad1ByteNames.TryGetValue(node.flags, out names))
                {
                    names = new List<string>();
                    BoneNode.pad1ByteNames[node.flags] = names;
                }
                names.Add(node.name);
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


    //public void GetTextureId(int submeshIndex, out int originalMaterialId, out int adjustedMaterialId)
    //{
    //    //SubMeshData1 data1 = m_subMeshData1List[submeshIndex];
    //    int materialBlockId = 0;
    //    if (submeshIndex < m_meshMaterialOffsetList.Count)
    //    {
    //        materialBlockId = m_meshMaterialOffsetList[submeshIndex].convertedOffset
    //    }
    //    originalMaterialId = 0;
    //    adjustedMaterialId = 0;

    //    //int adjustment = 0;

    //    //originalMaterialId = materialBlockId;

    //    ////int lookup = m_materialDataList[materialBlockId].diffuseTextureId / s_textureBlockSize;
    //    //int lookup = originalMaterialId;

    //    ////int adjustedIndex = AdjustForModel(materialBlockId);
    //    //int adjustedIndex = TextureForMesh(submeshIndex);
    //    //if (adjustedIndex == -1)
    //    //{
    //    //    adjustedIndex = AdjustForModel(lookup);
    //    //}


    //    //if (adjustedIndex >= m_textureNames.Count)
    //    //{
    //    //    adjustedIndex = m_textureNames.Count - 1;
    //    //}

    //    //adjustedMaterialId = adjustedIndex;
    //}

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

    public IndexedVector4 boundingSphere;

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

        smd.boundingSphere.W = binReader.ReadSingle();
        smd.boundingSphere.X = binReader.ReadSingle();
        smd.boundingSphere.Y = binReader.ReadSingle();
        smd.boundingSphere.Z = binReader.ReadSingle();

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
        sw.WriteLine(String.Format("[{0}][{1}] counter[{2}] bounds[{3:0.0000}] pad1[{4}][{5}][{6}][{7}] pad5[{8}] lod[{9]", zero1, zero2, counter1, boundingSphere, pad1, pad2, pad3, pad4, pad5, LodLevel));
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
        // find next int starting with 0,12


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
        // found the blocks, so put us back at the start of them
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
            //model.m_materialDataList.Add(MaterialData.FromByteBlock(bytesList, numMeshes, sectionLength));
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

public class JSONMeshMaterial
{
    public string materialName;
    public string textureDiffuse;
    public string textureSpecular;
    public List<int> MeshIds = new List<int>();
}

public class JSONModelData
{
    public string modelName;
    public List<JSONMeshMaterial> meshMaterials = new List<JSONMeshMaterial>();

    public string GetMaterial(int meshId)
    {
        foreach (JSONMeshMaterial mm in meshMaterials)
        {
            if (mm.MeshIds.Contains(meshId))
            {
                return mm.materialName;
            }
        }
        return "";
    }
}


public class OBBTEntry
{
    public short keyVal;
    public List<short> dataVals = new List<short>();
}


public class SELSData
{
    List<string> m_allData;
    List<string> m_meshSetData = new List<string>();
    List<string> m_tintSetData = new List<string>();

    public void BuildData(List<String> sourceData)
    {
        m_allData = sourceData;
        if (m_allData != null)
        {
            foreach (string s in sourceData)
            {
                if (s.StartsWith("set_LOD") || s.StartsWith("set_face") || s.StartsWith("set_shield"))
                {
                    m_meshSetData.Add(s);
                }
                else if (s.StartsWith("set_tint"))
                {
                    m_tintSetData.Add(s);
                }
            }
        }
    }

    public int GetMeshLodLevel(string val)
    {
        int index = m_meshSetData.IndexOf(val);
        return index;
    }

    public int GetBestLodData()
    {
        int[] allVals = new int[6];
        for(int i=0;i<allVals.Length;++i)
        {
            allVals[i] = -1;
        }
        int count = 0;

        allVals[count++] = GetMeshLodLevel("set_LOD0");
        //allVals[count++] = GetMeshLodLevel("set_faceEyes");
        //allVals[count++] = GetMeshLodLevel("set_faceFront");
        //allVals[count++] = GetMeshLodLevel("set_faceEars");
        //allVals[count++] = GetMeshLodLevel("set_faceTop");
        //allVals[count++] = GetMeshLodLevel("set_faceBack");

        int result = 0;
        for(int i=0;i<allVals.Length;++i)
        {
            if(allVals[i] != -1)
            {
                result += (1 << allVals[i]);
            }
        }

        return result;
    }



}
public class OBBTTreeNode
{
    public int matrixIndex;
    public OBBTTreeNode parent;
    public OBBTTreeNode left;
    public OBBTTreeNode right;
    public List<short> meshNodes = new List<short>();
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




