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

    
    public static void ReadUnskinnedVertexData24(BinaryReader binReader, int numVertices, List<CommonVertexInstance> vertices)
    {
        for (int i = 0; i < numVertices; ++i)
        {
            CommonVertexInstance vpnt = new CommonVertexInstance();
            vpnt.Position = CommonModelImporter.FromStreamVector3(binReader);
            vpnt.Position = GladiusGlobals.GladiusToUnity(vpnt.Position);

            vpnt.Normal = UncompressNormal(binReader.ReadInt32());
            vpnt.Normal = GladiusGlobals.GladiusToUnity(vpnt.Normal);

            vpnt.UV = CommonModelImporter.FromStreamVector2(binReader);

            vertices.Add(vpnt);
        }
    }


    public static void ReadUnskinnedVertexData28(BinaryReader binReader, int numVertices, List<CommonVertexInstance> vertices)
    {
        for (int i = 0; i < numVertices; ++i)
        {
            CommonVertexInstance vpnt = new CommonVertexInstance();
            vpnt.Position = CommonModelImporter.FromStreamVector3(binReader);
            vpnt.Position = GladiusGlobals.GladiusToUnity(vpnt.Position);

            vpnt.Normal = UncompressNormal(binReader.ReadInt32());
            vpnt.Normal = GladiusGlobals.GladiusToUnity(vpnt.Normal);

            vpnt.DiffuseColor = CommonModelImporter.FromStreamColor32(binReader);
            vpnt.UV = CommonModelImporter.FromStreamVector2(binReader);

            vertices.Add(vpnt);
        }
    }

    public static void ReadUnskinnedVertexData36(BinaryReader binReader, int numVertices, List<CommonVertexInstance> vertices)
    {
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
                vpnt.UV2 = CommonModelImporter.FromStreamVector2(binReader);

                vertices.Add(vpnt);
            }
            catch (Exception e)
            {
                int ibreak = 0;
            }
        }
    }



    public static void ReadSkinnedVertexData28(BinaryReader binReader, int numVertices, List<CommonVertexInstance> vertices)
    {
        for (int i = 0; i < numVertices; ++i)
        {
            CommonVertexInstance vpnt = new CommonVertexInstance();
            vpnt.Position = CommonModelImporter.FromStreamVector3(binReader);
            vpnt.Position = GladiusGlobals.GladiusToUnity(vpnt.Position);

            //vpnt.BoneInfo1 = binReader.ReadInt32();
            vpnt.Normal = UncompressNormal(binReader.ReadInt32());
            vpnt.Normal = GladiusGlobals.GladiusToUnity(vpnt.Normal);
            vpnt.UV = CommonModelImporter.FromStreamVector2(binReader);
            vpnt.BoneWeights = binReader.ReadInt32();
            vpnt.DiffuseColor = Color.white;

            vertices.Add(vpnt);
        }
    }

    public static void ReadSkinnedVertexData32(BinaryReader binReader, int numVertices, List<CommonVertexInstance> vertices)
    {
        for (int i = 0; i < numVertices; ++i)
        {
            CommonVertexInstance vpnt = new CommonVertexInstance();
            vpnt.Position = CommonModelImporter.FromStreamVector3(binReader);
            vpnt.Position = GladiusGlobals.GladiusToUnity(vpnt.Position);


            vpnt.Normal = UncompressNormal(binReader.ReadInt32());
            vpnt.Normal = GladiusGlobals.GladiusToUnity(vpnt.Normal);

            vpnt.DiffuseColor = CommonModelImporter.FromStreamColor32(binReader);

            vpnt.UV = CommonModelImporter.FromStreamVector2(binReader);
            vpnt.BoneWeights = binReader.ReadInt32();
            vertices.Add(vpnt);
        }
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



    
    public List<BaseModel> m_models = new List<BaseModel>();
    public static string s_rootPath = "";
    public static string s_characterModelPath = "";
    public static string s_modelOutputPath = "";
    bool m_writeAnimationOnly = false;

}

public class XboxModel : BaseModel
{
    public List<int> m_rebuildOffsets = new List<int>();

    public const int s_textureBlockSize = 64;
    public const int s_materialBlockSize = 44;
    public const int s_mmoOffsetBlockSize = 12;


    public XboxModel(string name) : base(name) 
    {
        m_name = name;
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

        if(SKELChunk != null)
        {
            foreach(BoneNode bn in SKELChunk.BoneList)
            {
                bn.name = NAMEChunk.Names[bn.NameIndex];
                if(bn.Index != bn.ParentIndex)
                {
                    bn.parent = SKELChunk.BoneList[bn.ParentIndex];
                }
            }
            BoneList.AddRange(SKELChunk.BoneList);
        }


    }


    public int GetBestLodLevel()
    {
        return GetMeshLodLevel("set_LOD0");
    }

    public int GetMeshLodLevel(string setName)
    {
        if (SELSChunk != null)
        {
            //int nameIndex = SELSChunk.SelectSetList.IndexOf(setName);
            int nameIndex = 0;
            if (nameIndex != -1)
            {
                SelectSet selectSet = StypChunk.SelectSetList.Find(x => x.NameIndex == nameIndex);
                return selectSet.Mask;
            }
        }
        return -1;
    }

    public XRNDChunk XRNDChunk
    { get { return (m_chunkList.Find(x => x is XRNDChunk) as XRNDChunk); } }


    public XRenderSetup XRenderSetup
    { get { return XRNDChunk.XRenderSetup; } }


    public SELSChunk SELSChunk
    {
        get { return (m_chunkList.Find(x => x is SELSChunk) as SELSChunk); }
    }

    public STYPChunk StypChunk
    {
        get { return (m_chunkList.Find(x => x is STYPChunk) as STYPChunk); }
    }

    public SKELChunk SKELChunk
    {
        get { return (m_chunkList.Find(x => x is SKELChunk) as SKELChunk); }
    }

    public NAMEChunk NAMEChunk
    {
        get { return (m_chunkList.Find(x => x is NAMEChunk) as NAMEChunk); }
    }

    public OBBTChunk OBBTChunk
    {
        get { return (m_chunkList.Find(x => x is OBBTChunk) as OBBTChunk); }
    }

    public CommonModelData ToCommon()
    {
        CommonModelData commonModel = new CommonModelData();
        commonModel.XBoxModel = this;
        commonModel.BoneList.AddRange(BoneList);
        commonModel.Name = m_name;

        commonModel.CommonMaterials.AddRange(XRenderSetup.CommonMaterials);

        foreach(CLXRenderVertexBuffer vb in XRenderSetup.VertexBuffers)
        {
            commonModel.AllVertices.AddRange(vb.Vertices);
        }

        commonModel.HasUV2 = XRenderSetup.VertexBuffers[0].HasUV2;
        commonModel.HasColor = XRenderSetup.VertexBuffers[0].HasColor;


        // opaque meshes
        int pOpaque = XRenderSetup.Header.PointerToOpaqueMeshes;
        int pTranslucent = XRenderSetup.Header.PointerToTranslucentMeshes;

        int pOpaqueMG = XRenderSetup.Header.PointerToOpaqueMaterialGroups;
        int pTranslucentMG = XRenderSetup.Header.PointerToOTranslucentMaterialGroups;

        SortedSet<int> opaqueMaterials = new SortedSet<int>();
        SortedSet<int> translucentMaterials = new SortedSet<int>();

        int index = 0;
        foreach(CLXRenderMesh renderMesh in XRenderSetup.MeshList)
        {
            CommonMeshData commonMesh = new CommonMeshData();
            commonModel.CommonMeshData.Add(commonMesh);
            MaterialGroup materialGroup = XRenderSetup.MeshMaterialList[index];
            CLXRenderIndexBuffer indexBuffer = XRenderSetup.IndexBuffers[index];

            commonMesh.Index = index;
            commonMesh.Radius = renderMesh.Radius;
            commonMesh.Center = renderMesh.Center;
            commonMesh.Offset = renderMesh.Offset;
            commonMesh.LodLevel = (int)renderMesh.SelectionSetMask;

            //commonMesh.MaterialId = (int)XRenderSetup.MeshMaterialList[index].PointerToMaterial / s_materialBlockSize;
            int offset = XRenderSetup.MaterialGroupLists[index];
            commonMesh.MaterialId = (int)XRenderSetup.MeshMaterialList[offset].PointerToMaterial / s_materialBlockSize; //(int)materialGroup.PointerToMaterial / s_materialBlockSize;

            if (index < XRenderSetup.Header.OpaqueMeshCount)
            {
                opaqueMaterials.Add((int)commonMesh.MaterialId); 
            }
            else
            {
                commonMesh.IsTransparent = true;
                translucentMaterials.Add((int)commonMesh.MaterialId);
            }
            SortedSet<int> sortedVertices = new SortedSet<int>();


            int minIndex = int.MaxValue;

            bool swap = true;
            //
            //for (int i = 0; i < indexBuffer.IndexCount-3; i+=3)
            int a = indexBuffer.IndexCount % 2;
            int b = indexBuffer.IndexCount % 3;
            
            for (int i = 0; i < indexBuffer.IndexCount-2; i++)
            {
                int index1 = i + 2;
                int index2 = i + 1;
                int index3 = i;

                int i1 = (int)indexBuffer.Indices[swap ? index1 : index3];
                int i2 = (int)indexBuffer.Indices[index2];
                int i3 = (int)indexBuffer.Indices[swap ? index3 : index1];

                if(renderMesh.PointerToVertexBuffer == 0)
                {

                }
                else if(renderMesh.PointerToVertexBuffer == 20)
                {
                     i1 += XRenderSetup.VertexBuffers[0].VertexCount;
                    i2 += XRenderSetup.VertexBuffers[0].VertexCount;
                    i3 += XRenderSetup.VertexBuffers[0].VertexCount;
                }
                else 
                {
                    int ibreak = 0;
                }

                minIndex = Math.Min(minIndex, Math.Min(i1, Math.Min(i2, i3)));


                sortedVertices.Add(i1);
                sortedVertices.Add(i2);
                sortedVertices.Add(i3);

                bool removeDegenerate = true;
                if (!(removeDegenerate && (i1 == i2 || i2 == i3 || i3 == i1)))
                {
                    commonMesh.Indices.Add(i1);
                    commonMesh.Indices.Add(i2);
                    commonMesh.Indices.Add(i3);
                }
                swap = !swap;
            }

            for(int i=0;i<commonMesh.Indices.Count;++i)
            {
                commonMesh.Indices[i] -= minIndex;
            }


            commonMesh.Vertices.AddRange(sortedVertices);

            if(index == 3120)
            {
                int ibreak = 0;
            }

            index++;
        }

        return commonModel;
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
}




public class XRenderSetup
{
    public uint Signature;
    public int SizeOfHeader;
    public int KeepInPlace;
    public int DataSize;
    public uint DataPointer;
    public int NumMeshHeaders;
    public int NumMeshes;
    public int NumVertexBuffers;
    public int NumIndexBuffers;
    public int NumTextures;
    public int NumMaterials;

    public uint PointerToMeshHeader;
    public uint PointerToMeshLists;
    public uint PointerMeshses;
    public uint PointerToIndexBuffers;
    public uint PointerToVertexBuffers;
    public uint PointerToVertexResources;
    public uint PointerToBoneIndices;

    public uint PointerToMaterialGroupLists;
    public uint PointerToMaterialGroups;
    public uint PointerToMaterials;
    public uint PointerToMaterialTemplates;
    public uint PointerToMaterialProperties;
    public uint PointerToShaderBlocks;
    public uint PointerToTextureBlocks;
    public uint PointerToStreamData;

    public uint PointerToIndices;
    public uint PointerToVertices;
    public uint PointerToTextures;
    public uint PointerToBitmaps;
    public uint PointerToStrings;

    public CLXRenderMeshHeader Header;


    //public List<int> Indices = new List<int>();
    public List<CLXRenderMaterial> Materials = new List<CLXRenderMaterial>();
    public List<CLXRenderTexture> Textures = new List<CLXRenderTexture>();
    public List<CLXRenderIndexBuffer> IndexBuffers = new List<CLXRenderIndexBuffer>();
    public List<CLXRenderVertexBuffer> VertexBuffers = new List<CLXRenderVertexBuffer>();
    public List<MaterialGroup> OpaqueMaterialGroupList = new List<MaterialGroup>();
    public List<MaterialGroup> TranslucentMaterialGroupList = new List<MaterialGroup>();
    public List<MaterialGroup> MeshMaterialList = new List<MaterialGroup>();
    
    public List<CLXRenderMesh> MeshList = new List<CLXRenderMesh>();
    public List<CommonMaterialData> CommonMaterials = new List<CommonMaterialData>();

    public List<int> MaterialGroupLists = new List<int>();

    List<List<byte>> BoneIndicesLists = new List<List<byte>>();


    public int AdjustBone(int index, int subMeshIndex)
    {
        if (index != -1)
        {
            Debug.Assert(index % 3 == 0);
            index /= 3;

            int copy = index;
            if (index < BoneIndicesLists[subMeshIndex].Count)
            {
                index = BoneIndicesLists[subMeshIndex][index];
            }
        }

        return index;
    }



    public static XRenderSetup FromStream(string name,BinaryReader binReader)
    {
        long basePosition = binReader.BaseStream.Position;

        XRenderSetup xrs = new XRenderSetup();
        xrs.Signature = binReader.ReadUInt32();
        xrs.SizeOfHeader = binReader.ReadInt32();
        xrs.KeepInPlace = binReader.ReadInt32();
        xrs.DataSize = binReader.ReadInt32();
        xrs.DataPointer = binReader.ReadUInt32();
        xrs.NumMeshHeaders = binReader.ReadInt32();
        xrs.NumMeshes = binReader.ReadInt32();
        xrs.NumVertexBuffers = binReader.ReadInt32();
        xrs.NumIndexBuffers = binReader.ReadInt32();
        xrs.NumTextures = binReader.ReadInt32();
        xrs.NumMaterials = binReader.ReadInt32();

        xrs.PointerToMeshHeader = binReader.ReadUInt32();
        xrs.PointerToMeshLists = binReader.ReadUInt32();
        xrs.PointerMeshses = binReader.ReadUInt32();
        xrs.PointerToIndexBuffers = binReader.ReadUInt32();
        xrs.PointerToVertexBuffers = binReader.ReadUInt32();
        xrs.PointerToVertexResources = binReader.ReadUInt32();
        xrs.PointerToBoneIndices = binReader.ReadUInt32();

        xrs.PointerToMaterialGroupLists = binReader.ReadUInt32();
        xrs.PointerToMaterialGroups = binReader.ReadUInt32();
        xrs.PointerToMaterials = binReader.ReadUInt32();
        xrs.PointerToMaterialTemplates = binReader.ReadUInt32();
        xrs.PointerToMaterialProperties = binReader.ReadUInt32();
        xrs.PointerToShaderBlocks = binReader.ReadUInt32();
        xrs.PointerToTextureBlocks = binReader.ReadUInt32();
        xrs.PointerToStreamData = binReader.ReadUInt32();

        xrs.PointerToIndices = binReader.ReadUInt32();
        xrs.PointerToVertices = binReader.ReadUInt32();
        xrs.PointerToTextures = binReader.ReadUInt32();
        xrs.PointerToBitmaps = binReader.ReadUInt32();
        xrs.PointerToStrings = binReader.ReadUInt32();

        xrs.Header = CLXRenderMeshHeader.FromStream(binReader);

        long postHeaderPosition = basePosition + (long)xrs.SizeOfHeader;

        /*
         * pickaxe
        PointerToMesLists       112 uint
        PointerMeshses          116 uint
        PointerToIndexBuffers       172 uint
        PointerToVertexBuffers      192 uint
        PointerToVertexResources    212 uint
        PointerToMaterialGroupLists 224 uint
        PointerToMaterialGroups     228 uint
        PointerToMaterials      240 uint
        PointerToStreamData     284 uint
        PointerToTextures       408 uint
        PointerToStrings        536 uint
        PointerToIndices        580 uint
        PointerToVertices       1616    uint
        */


        /*
         * Wheel
         * 
 		PointerToMeshHeader	0	uint
        PointerToMesLists	112	uint
        PointerMeshses		144	uint
        PointerToIndexBuffers	592	uint
        PointerToVertexBuffers	752	uint
        PointerToVertexResources	772	uint
        PointerToMaterialGroupLists	784	uint
        PointerToMaterialGroups	816	uint
        PointerToMaterials	912	uint
        PointerToStreamData	1264	uint
        PointerToTextures	2184	uint
        PointerToStrings	2760	uint
        PointerToIndices	2899	uint
        PointerToVertices	9907	uint
        */


        // Material Lists (Material Group) - num opaque meshees * pointer to matrialgroup* , same for translucent.
        // so for each mesh there is a pointer to it's material?

        // sizeof R2::Material

        int minOpaque = int.MaxValue;
        int maxOpaque = int.MinValue;

        List<int> temp = new List<int>();
        List<int> temp2 = new List<int>();

        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToMaterialGroupLists;

        int numElements =(int) (xrs.PointerToMaterialGroups - xrs.PointerToMaterialGroupLists) / 4;

        for (int i=0;i< numElements;++i)
        {
            xrs.MaterialGroupLists.Add(binReader.ReadInt32()/12);
        }


        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToMaterialGroups;

        for (int i = 0; i < xrs.Header.OpaqueMeshCount; ++i)
        {
            MaterialGroup mg = MaterialGroup.FromStream(binReader);
            xrs.OpaqueMaterialGroupList.Add(mg);
            minOpaque = Math.Min(minOpaque, (int)mg.PointerToMaterial);
            maxOpaque = Math.Max(maxOpaque, (int)mg.PointerToMaterial);
            temp.Add((int)mg.PointerToMaterial);
        }



        int minTrans = int.MaxValue;
        int maxTrans = int.MinValue;

        for (int i = 0; i < xrs.Header.TranslucentMeshCount; ++i)
        {
            MaterialGroup mg = MaterialGroup.FromStream(binReader);
            xrs.TranslucentMaterialGroupList.Add(mg);
            minTrans = Math.Min(minTrans, (int)mg.PointerToMaterial);
            maxTrans = Math.Max(maxTrans, (int)mg.PointerToMaterial);
        }


        xrs.MeshMaterialList.AddRange(xrs.OpaqueMaterialGroupList);
        xrs.MeshMaterialList.AddRange(xrs.TranslucentMaterialGroupList);

        //xrs.MeshMaterialList.RemoveAll(x => x.PointerToMaterial == 0);
        //xrs.MeshMaterialList.Insert(0, xrs.OpaqueMaterialGroupList[0]);

        //for(int i=0;i<xrs.MeshMaterialList.Count;i++)
        //{
        //    if(i==0)
        //    {
        //        continue;
        //    }
        //    if(xrs.MeshMaterialList[i].PointerToMaterial == 0 && i < xrs.MeshMaterialList.Count -1)
        //    {
        //        xrs.MeshMaterialList[i].PointerToMaterial = xrs.MeshMaterialList[i-1].PointerToMaterial;
        //    }
        //}

        foreach(MaterialGroup mg in xrs.MeshMaterialList)
        {
            temp2.Add((int)mg.PointerToMaterial);
        }

        //xrs.MeshList.RemoveAll(x => x.LODModifier == 0);
        //for(int i=0;i<xrs.MeshList.Count;++i)
        //{
        //    if(xrs.MeshList[i].LODModifier == 0 && i > 0)
        //    {
        //        xrs.MeshList[i].PointerToMaterial = xrs.MeshList[i -1].PointerToMaterial;
        //    }
        //}
        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerMeshses;
        for (int i = 0; i < xrs.NumMeshes; ++i)
        {
            CLXRenderMesh mesh = CLXRenderMesh.FromStream(binReader);
            xrs.MeshList.Add(mesh);
        }


        // Now positioned at PointerToMaterials.

        int materialsPointer = (int)(postHeaderPosition + xrs.PointerToMaterials);
        //// Now positioned at StreamData
        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToStreamData;
        List<MaterialPropertyBase> mpsList = new List<MaterialPropertyBase>();
        while ((binReader.BaseStream.Position - postHeaderPosition) < xrs.PointerToTextures)
        {
            MaterialPropertyBase pb = MaterialPropertyBase.FromStream(binReader);
            mpsList.Add(pb);

        }

        List<int> renderStateTypeInfo = new List<int>();
        List<int> renderStateValueInfo = new List<int>();
        foreach (MaterialPropertyBase mpb in mpsList)
        {
            MaterialGenericPropertyRenderstate prs = mpb as MaterialGenericPropertyRenderstate;
            if (prs != null)
            {
                renderStateTypeInfo.Add(prs.StateType);
                renderStateValueInfo.Add(prs.Value);
            }
        }

        List<int> setShaderTypeInfo = new List<int>();
        List<int> setShaderFlagsInfo = new List<int>();
        foreach (MaterialPropertyBase mpb in mpsList)
        {
            MaterialGenericPropertySetShader prs = mpb as MaterialGenericPropertySetShader;
            if (prs != null)
            {
                setShaderTypeInfo.Add((int)prs.UnionPixelShaderInfo);
                setShaderFlagsInfo.Add(prs.Flags);
            }
        }



        List<CommonMaterialData> materialList = GetMaterials(name,mpsList);
        xrs.CommonMaterials.AddRange(materialList);

        //MaterialPropertySimple mps = MaterialPropertySimple.FromStream(binReader);

        //binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToMaterials;
        //for (int i = 0; i < xrs.NumTextures; ++i)
        //{
        //    CLXRenderMaterial t = CLXRenderMaterial.FromStream(binReader);
        //    xrs.Materials.Add(t);
        //}

        // Now positioned at textures
        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToTextures;
        for (int i = 0; i < xrs.NumTextures; ++i)
        {
            CLXRenderTexture t = CLXRenderTexture.FromStream(binReader);
            xrs.Textures.Add(t);
        }

        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToIndexBuffers;
        for (int i = 0; i < xrs.NumIndexBuffers; ++i)
        {
            CLXRenderIndexBuffer ib = CLXRenderIndexBuffer.FromStream(binReader);
            xrs.IndexBuffers.Add(ib);
        }

        bool hasSkeleton = (int)xrs.PointerToBoneIndices != -1;
        if (hasSkeleton)
        {
            binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToBoneIndices;

            for (int i = 0; i < xrs.NumMeshes; ++i)
            {
                List<byte> boneIndexList = new List<byte>();
                xrs.BoneIndicesLists.Add(boneIndexList);
                int numEntries = binReader.ReadByte();
                for (int j = 0; j < numEntries; ++j)
                {
                    boneIndexList.Add(binReader.ReadByte());
                }
            }
        }


        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToVertexBuffers;
        for (int i = 0; i < xrs.NumVertexBuffers; ++i)
        {
            CLXRenderVertexBuffer vb = CLXRenderVertexBuffer.FromStream(hasSkeleton,binReader);
            xrs.VertexBuffers.Add(vb);
        }


        //binReader.BaseStream.Position = basePosition + xrs.PointerToVertexResources;
        //List<int> test = new List<int>();
        //for(int i=0;i<6;++i)
        //{
        //    test.Add(binReader.ReadInt32());
        //}

        List<string> names = new List<string>();
        CommonModelImporter.ReadNullSeparatedNames(binReader, postHeaderPosition + xrs.PointerToStrings, xrs.NumTextures, names);


        foreach (CommonMaterialData md in materialList)
        {
            if (md.TextureData1 == null)
            {
                int index = md.PointerToTexture1 / 64;
                md.TextureData1 = new CommonTextureData();
                md.TextureData1.textureName = names[index];

            }

            if (md.PointerToTexture2 != -1)
            {
                int index = md.PointerToTexture2 / 64;
                md.TextureData2 = new CommonTextureData();
                md.TextureData2.textureName = names[index];

            }
        }

        foreach (CLXRenderIndexBuffer ib in xrs.IndexBuffers)
        {
            ib.Populate(binReader, postHeaderPosition + xrs.PointerToIndices);
        }


        long originalPosition = binReader.BaseStream.Position;

        binReader.BaseStream.Position = postHeaderPosition + xrs.PointerToVertices;
        foreach (CLXRenderVertexBuffer vb in xrs.VertexBuffers)
        {
            vb.Populate(binReader, postHeaderPosition + xrs.PointerToVertices);
        }

        return xrs;
    }


    static List<CommonMaterialData> GetMaterials(string name, List<MaterialPropertyBase> streamData)
    {
        List<CommonMaterialData> materials = new List<CommonMaterialData>();
        CommonMaterialData currentMaterial = null;
        int texture = 0;
        int count = 0;
        foreach (MaterialPropertyBase mp in streamData)
        {
            if (mp is MaterialGenericPropertyBindGeneric)
            {
                currentMaterial = new CommonMaterialData();
                currentMaterial.Name = name + "_" + (count++);
                materials.Add(currentMaterial);
                texture = 0;
            }
            if (mp is MaterialGenericPropertyTexture)
            {
                texture++;

                if (texture == 1)
                {
                    currentMaterial.PointerToTexture1 = (int)((MaterialGenericPropertyTexture)mp).PointerToXRenderTexture;

                }
                else if (texture == 2)
                {
                    currentMaterial.PointerToTexture2 = (int)((MaterialGenericPropertyTexture)mp).PointerToXRenderTexture;
                }
                else
                {
                    Debug.LogErrorFormat("Extra Texture in stream [{0}]", texture);
                }
            }
            if (mp is MaterialGenericPropertyColors)
            {
                currentMaterial.Ambient = ((MaterialGenericPropertyColors)mp).D3DMaterial.Ambient;
                currentMaterial.Diffuse = ((MaterialGenericPropertyColors)mp).D3DMaterial.Diffuse;
                currentMaterial.Specular = ((MaterialGenericPropertyColors)mp).D3DMaterial.Specular;
                currentMaterial.Emissive = ((MaterialGenericPropertyColors)mp).D3DMaterial.Emissive;
            }
            if (mp is MaterialGenericPropertySetShader)
            {
                //currentMaterial.Flags = ((MaterialGenericPropertySetShader)mp).Flags;
                //currentMaterial.DetailMapFromShaderSet = ((MaterialGenericPropertySetShader)mp).UnionPixelShaderInfo == 7;
                currentMaterial.ShaderSet = (int)((MaterialGenericPropertySetShader)mp).UnionPixelShaderInfo;
            }
            if (mp is MaterialGenericPropertyRenderstate)
            {
                currentMaterial.RenderState = ((MaterialGenericPropertyRenderstate)mp).StateType;
            }
        }

        return materials;
    }





}

public class CollisionData
{
    public int VertexCount;
    public uint PointerToVertexList;
    public int IndexCount;
    public int PrimitiveCount;
    public uint PointerToIndxList;

    public static CollisionData FromStream(BinaryReader binReader)
    {
        CollisionData collisionData = new CollisionData();
        collisionData.VertexCount = binReader.ReadInt32();
        collisionData.PointerToVertexList = binReader.ReadUInt32();
        collisionData.IndexCount = binReader.ReadInt32();
        collisionData.PrimitiveCount = binReader.ReadInt32();
        collisionData.PointerToIndxList = binReader.ReadUInt32();
        return collisionData;
    }
}


public class ShaderConstantAlignemnt
{
    public byte ShaderTypeFlags;
    public byte ShaderMaxBoneCount;
    public byte ShaderMatrixCount;
    public byte ShaderMaxLightCount;

    public static ShaderConstantAlignemnt FromStream(BinaryReader binReader)
    {
        ShaderConstantAlignemnt sca = new ShaderConstantAlignemnt();
        sca.ShaderTypeFlags = binReader.ReadByte();
        sca.ShaderMaxBoneCount = binReader.ReadByte();
        sca.ShaderMatrixCount = binReader.ReadByte();
        sca.ShaderMaxLightCount = binReader.ReadByte();
        return sca;
    }


}
public class CLXRenderMeshHeader
{
    public int Signature;
    public uint UserDataPointer;
    public uint CRCName;
    public uint ResourceFlags;
    public uint DeallocAddressPointer;
    public uint NamePointer;
    public uint Flags;

    public int OpaqueMeshCount;
    public int PointerToOpaqueMeshes;
    public int PointerToOpaqueMaterialGroups;
    public int TranslucentMeshCount;
    public int PointerToTranslucentMeshes;
    public int PointerToOTranslucentMaterialGroups;
    public int PointerToNext;
    public int PointerToPrev;
    public CollisionData CollisionData;

    public float ObjectRadius;
    public IndexedVector3 ObjectOffset;

    ShaderConstantAlignemnt sca;

    public int BoneCRCArrayCount;
    public uint PointerToBoneCRCArray;

    public uint PointerToSkeleton;

    public static CLXRenderMeshHeader FromStream(BinaryReader binReader)
    {
        CLXRenderMeshHeader header = new CLXRenderMeshHeader();

        header.Signature = binReader.ReadInt32();
        header.UserDataPointer = binReader.ReadUInt32();
        header.CRCName = binReader.ReadUInt32();
        header.ResourceFlags = binReader.ReadUInt32();
        header.DeallocAddressPointer = binReader.ReadUInt32();
        header.NamePointer = binReader.ReadUInt32();
        header.Flags = binReader.ReadUInt32();


        header.OpaqueMeshCount = binReader.ReadInt32();
        header.PointerToOpaqueMeshes = binReader.ReadInt32();
        header.PointerToOpaqueMaterialGroups = binReader.ReadInt32();
        header.TranslucentMeshCount = binReader.ReadInt32();
        header.PointerToTranslucentMeshes = binReader.ReadInt32();
        header.PointerToOTranslucentMaterialGroups = binReader.ReadInt32();
        header.PointerToNext = binReader.ReadInt32();
        header.PointerToPrev = binReader.ReadInt32();
        header.CollisionData = CollisionData.FromStream(binReader);

        header.ObjectRadius = binReader.ReadSingle();
        header.ObjectOffset = CommonModelImporter.FromStreamVector3(binReader);
        header.sca = ShaderConstantAlignemnt.FromStream(binReader);
        header.BoneCRCArrayCount = binReader.ReadInt32();
        header.PointerToBoneCRCArray = binReader.ReadUInt32();
        header.PointerToSkeleton = binReader.ReadUInt32();

        return header;
    }
}

public class CLXRenderMesh
{
    public int MeshType;
    public uint PointerToVertexBuffer;
    public uint PointerToVIndexBuffer;
    public float Radius;
    public IndexedVector3 Center;
    public IndexedVector3 Offset;
    public uint VertexFormatFlags;
    public uint PointerToBonePalette;
    public uint SelectionSetMask;
    public uint PointerToName;

    public static CLXRenderMesh FromStream(BinaryReader binaryReader)
    {
        CLXRenderMesh mesh = new CLXRenderMesh();
        mesh.MeshType = binaryReader.ReadInt32();
        mesh.PointerToVertexBuffer = binaryReader.ReadUInt32();
        mesh.PointerToVIndexBuffer = binaryReader.ReadUInt32();
        mesh.Radius = binaryReader.ReadSingle();
        mesh.Center = CommonModelImporter.FromStreamVector3(binaryReader);
        mesh.Offset = CommonModelImporter.FromStreamVector3(binaryReader);
        //mesh.LODCount = binaryReader.ReadInt32();
        //mesh.LODModifier = binaryReader.ReadSingle();

        mesh.VertexFormatFlags = binaryReader.ReadUInt32();
        mesh.PointerToBonePalette = binaryReader.ReadUInt32();
        mesh.SelectionSetMask = binaryReader.ReadUInt32();
        mesh.PointerToName = binaryReader.ReadUInt32();

        return mesh;
    }

}

public class D3DMaterial8
{
    public Color Diffuse;
    public Color Ambient;
    public Color Specular;
    public Color Emissive;
    public float Power;

    public static D3DMaterial8 FromStream(BinaryReader binReader)
    {
        D3DMaterial8 mat = new D3DMaterial8();
        mat.Diffuse = CommonModelImporter.FromStreamColor(binReader);
        mat.Ambient = CommonModelImporter.FromStreamColor(binReader);
        mat.Specular = CommonModelImporter.FromStreamColor(binReader);
        mat.Emissive = CommonModelImporter.FromStreamColor(binReader);
        mat.Power = binReader.ReadSingle();
        return mat;
    }
}

public class CLXRenderMaterial
{
    public int ShaderIndex;

    public D3DMaterial8 D3dMaterial;
    //ExtraProperties;



    public uint PointerToTextureGroup;

    public uint CallbackFunction;
    public uint CallbackData;
    public uint PointerToName;

    public static CLXRenderMaterial FromStream(BinaryReader binReader)
    {
        CLXRenderMaterial mat = new CLXRenderMaterial();
        mat.ShaderIndex = binReader.ReadInt32();
        mat.D3dMaterial = D3DMaterial8.FromStream(binReader);
        mat.PointerToTextureGroup = binReader.ReadUInt32();
        mat.CallbackFunction = binReader.ReadUInt32();
        mat.CallbackData = binReader.ReadUInt32();
        mat.PointerToName = binReader.ReadUInt32();
        return mat;
    }

}
public class D3DFormat
{

}

public class CLXRenderTexture
{
    public int Width;
    public int Height;
    public int Format;
    public int TextureType;
    public int TextureResourceType;
    public uint PointerToName;

    public static CLXRenderTexture FromStream(BinaryReader binReader)
    {
        CLXRenderTexture tex = new CLXRenderTexture();
        tex.Width = binReader.ReadInt32();
        tex.Height = binReader.ReadInt32();
        tex.Format = binReader.ReadInt32();
        tex.TextureType = binReader.ReadInt32();
        tex.TextureResourceType = binReader.ReadInt32();
        tex.PointerToName = binReader.ReadUInt32();
        return tex;
    }
}


public class CLXRenderIndexBuffer
{
    public float LODModifier;
    public uint PointerToIndices;
    public int PrimitiveCount;
    public int IndexCount;
    public int PrimitveType;

    public List<uint> Indices = new List<uint>();


    public static CLXRenderIndexBuffer FromStream(BinaryReader binReader)
    {
        CLXRenderIndexBuffer ib = new CLXRenderIndexBuffer();
        ib.LODModifier = binReader.ReadSingle();
        ib.PointerToIndices = binReader.ReadUInt32();
        ib.PrimitiveCount = binReader.ReadInt32();
        ib.IndexCount = binReader.ReadInt32();
        ib.PrimitveType = binReader.ReadInt32();
        return ib;
    }

    public void Populate(BinaryReader binReader, long basePosition)
    {
        long originalPosition = binReader.BaseStream.Position;
        binReader.BaseStream.Position = basePosition + PointerToIndices;
        for (int i = 0; i < IndexCount; ++i)
        {
            Indices.Add(binReader.ReadUInt16());
        }

        binReader.BaseStream.Position = originalPosition;
    }

}



public class MaterialGroup
{
    public float LODModifier;
    public int MaterialCount;
    public uint PointerToMaterial;

    // max materials in group = 8

    public static MaterialGroup FromStream(BinaryReader binReader)
    {
        MaterialGroup mg = new MaterialGroup();
        mg.LODModifier = binReader.ReadSingle();
        mg.MaterialCount = binReader.ReadInt32();
        mg.PointerToMaterial = binReader.ReadUInt32();
        return mg;
    }
}


public class PaxMaterial
{
    const int NameSize = 124;
    const int ArraySize = 8;

    public int MaterialNum;
    public char[] MaterialName; // max 124
    public uint SelectSetMask;
    public uint AttribFlags;
    public uint AttribValues;
    public byte[] TexIndexList;
    public byte[] TexBlendModes;
    public byte[] TexGenModes;

    public static PaxMaterial FromStream(BinaryReader binReader)
    {
        PaxMaterial pm = new PaxMaterial();
        pm.MaterialNum = binReader.ReadInt32();
        pm.MaterialName = binReader.ReadChars(NameSize);
        pm.SelectSetMask = binReader.ReadUInt32();
        pm.AttribFlags = binReader.ReadUInt32();
        pm.AttribValues = binReader.ReadUInt32();
        pm.TexIndexList = binReader.ReadBytes(ArraySize);
        pm.TexBlendModes = binReader.ReadBytes(ArraySize);
        pm.TexGenModes = binReader.ReadBytes(ArraySize);


        return pm;
    }


}



public class MaterialPropertyBase
{
    public byte MaterialPropertyType;
    public byte MaterialPropertySize;

    public void BaseFromStream(BinaryReader binReader)
    {
        MaterialPropertyType = binReader.ReadByte();
        MaterialPropertySize = binReader.ReadByte();
    }


    public static MaterialPropertyBase FromStream(BinaryReader binReader)
    {
        byte type = binReader.ReadByte();
        byte size = binReader.ReadByte();

        binReader.BaseStream.Position -= 2;


        if (type == 0)
        {
            // end stream
            return MaterialGenericPropertyEndStream.FromStream(binReader);
        }
        if (type == 1)
        {
            return MaterialGenericPropertyTexture.FromStream(binReader);
        }
        if (type == 2)
        {
            return MaterialGenericPropertyColors.FromStream(binReader);
        }
        if (type == 3)
        {
            return MaterialGenericPropertyRenderstate.FromStream(binReader);
        }
        if (type == 8)
        {
            return MaterialGenericPropertyDraw.FromStream(binReader);
        }
        if (type == 9)
        {
            return MaterialGenericPropertySetShader.FromStream(binReader);
        }
        if (type == 12)
        {
            // post stream
            return MaterialGenericPropertyPostStream.FromStream(binReader);
        }
        if (type == 14)
        {
            return MaterialGenericPropertyBindGeneric.FromStream(binReader);
        }

        Debug.LogErrorFormat("Unknown SteamData type : [{0}][{1}]", type, size);

        binReader.BaseStream.Position += size;

        return null;
    }


}
public class MaterialPropertySimple : MaterialPropertyBase
{
    public byte Pad0;
    public byte Pad1;

    public static MaterialPropertySimple FromStream(BinaryReader binReader)
    {
        MaterialPropertySimple mps = new MaterialPropertySimple();
        mps.BaseFromStream(binReader);
        mps.Pad0 = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        return mps;
    }
}

public class MaterialGenericPropertyTexture : MaterialPropertyBase
{
    public byte TexturePropertyCreationType;
    public byte Pad1;
    public uint Stage;
    public uint PointerToXRenderTexture;

    public static MaterialGenericPropertyTexture FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertyTexture mps = new MaterialGenericPropertyTexture();
        mps.BaseFromStream(binReader);

        mps.TexturePropertyCreationType = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        mps.Stage = binReader.ReadUInt32();
        mps.PointerToXRenderTexture = binReader.ReadUInt32();

        return mps;
    }
}



public class MaterialGenericPropertyColors : MaterialPropertyBase
{
    public byte Pad0;
    public byte Pad1;
    public D3DMaterial8 D3DMaterial;
    public uint PointerToXRenderTexture;

    public static MaterialGenericPropertyColors FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertyColors mps = new MaterialGenericPropertyColors();
        mps.BaseFromStream(binReader);
        mps.Pad0 = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        mps.D3DMaterial = D3DMaterial8.FromStream(binReader);
        if (mps.D3DMaterial.Power > 0)
        {
            int ibreak = 0;
        }
        return mps;
    }
}

public class MaterialGenericPropertyEndStream : MaterialPropertyBase
{
    public byte Pad0;
    public byte Pad1;

    public static MaterialGenericPropertyEndStream FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertyEndStream mps = new MaterialGenericPropertyEndStream();
        mps.BaseFromStream(binReader);
        mps.Pad0 = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        return mps;
    }
}

public class MaterialGenericPropertyBindGeneric : MaterialPropertyBase
{
    public byte Pad0;
    public byte Pad1;
    public D3DMaterial8 D3DMaterial;
    public uint PointerToXRenderTexture;

    public static MaterialGenericPropertyBindGeneric FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertyBindGeneric mps = new MaterialGenericPropertyBindGeneric();
        mps.BaseFromStream(binReader);
        mps.Pad0 = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        return mps;
    }
}

public class MaterialGenericPropertySetShader : MaterialPropertyBase
{
    public byte Flags;
    public byte Pad1;
    public uint UnionVertexShaderInfo;
    public uint UnionPixelShaderInfo;

    public static MaterialGenericPropertySetShader FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertySetShader mps = new MaterialGenericPropertySetShader();
        mps.BaseFromStream(binReader);
        mps.Flags = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        mps.UnionVertexShaderInfo = binReader.ReadUInt32();
        mps.UnionPixelShaderInfo = binReader.ReadUInt32();
        return mps;
    }
}


public class MaterialGenericPropertyPostStream : MaterialPropertyBase
{
    public byte Pad0;
    public byte Pad1;

    public static MaterialGenericPropertyPostStream FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertyPostStream mps = new MaterialGenericPropertyPostStream();
        mps.BaseFromStream(binReader);
        mps.Pad0 = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        return mps;
    }
}


public class MaterialGenericPropertyDraw : MaterialPropertyBase
{
    public byte Pad0;
    public byte Pad1;

    public static MaterialGenericPropertyDraw FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertyDraw mps = new MaterialGenericPropertyDraw();
        mps.BaseFromStream(binReader);
        mps.Pad0 = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        return mps;
    }
}

public class MaterialGenericPropertyRenderstate : MaterialPropertyBase
{
    public byte Pad0;
    public byte Pad1;
    public int StateType;
    public int Value;

    public static MaterialGenericPropertyRenderstate FromStream(BinaryReader binReader)
    {
        MaterialGenericPropertyRenderstate mps = new MaterialGenericPropertyRenderstate();
        mps.BaseFromStream(binReader);
        mps.Pad0 = binReader.ReadByte();
        mps.Pad1 = binReader.ReadByte();
        mps.StateType = binReader.ReadInt32();
        mps.Value = binReader.ReadInt32();
        return mps;
    }
}







// public class SelectSetTypesChunk : BaseChunk
// {
//     public List<SelectSet> SelectSetList = new List<SelectSet>();
//     public static BaseChunk FromStream(String name, BinaryReader binReader)
//     {
//         SelectSetTypesChunk chunk = new SelectSetTypesChunk();
//         chunk.BaseFromStream(binReader);
//         for (int i = 0; i < chunk.NumElements; ++i)
//         {
//             chunk.SelectSetList.Add(SelectSet.FromStream(binReader));
//         }
//         return chunk;
//     }
// }


// public class PaxTexture
// {
//     public String Name;
//     public int TexNum;
//     public uint PointerToImageArray;
//     public uint Width;
//     public uint Height;
//     public uint AttribFlags;
//     public uint AttribValues;
//
//     public static PaxTexture FromStream(BinaryReader binReader)
//     {
//         PaxTexture paxTexture = new PaxTexture();
//         byte[] buffer = binReader.ReadBytes(128);
//         paxTexture.Name = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
//         paxTexture.TexNum = binReader.ReadInt32();
//         paxTexture.PointerToImageArray = binReader.ReadUInt32();
//         paxTexture.Width = binReader.ReadUInt32();
//         paxTexture.Height = binReader.ReadUInt32();
//         paxTexture.AttribFlags = binReader.ReadUInt32();
//         paxTexture.AttribValues = binReader.ReadUInt32();
//         return paxTexture;
//     }
// }
//
//
//


public class CLXRenderVertexBuffer
{
    public uint PointerToVBResource;
    public int VertexCount;
    public int ResourceOffset;
    public uint PointerToVB;
    public int VertexStride;
    public bool HasSkeleton;
    public bool HasColor;
    public bool HasUV2;

    public List<CommonVertexInstance> Vertices = new List<CommonVertexInstance>();

    public static CLXRenderVertexBuffer FromStream(bool hasSkeleton,BinaryReader binReader)
    {
        CLXRenderVertexBuffer vb = new CLXRenderVertexBuffer();
        vb.PointerToVBResource = binReader.ReadUInt32();
        vb.VertexCount = binReader.ReadInt32();
        vb.ResourceOffset = binReader.ReadInt32();
        vb.PointerToVB = binReader.ReadUInt32();
        vb.VertexStride = binReader.ReadInt32();
        vb.HasSkeleton = hasSkeleton;
        return vb;
    }

    public void Populate(BinaryReader binReader, long basePosition)
    {
        if (VertexStride == 24)
        {
            XboxModelReader.ReadUnskinnedVertexData24(binReader, VertexCount, Vertices);
        }
        else if (HasSkeleton && VertexStride == 28)
        {
            XboxModelReader.ReadSkinnedVertexData28(binReader, VertexCount, Vertices);
            HasColor = true;
        }
        else if (VertexStride == 28)
        {
            XboxModelReader.ReadUnskinnedVertexData28(binReader, VertexCount, Vertices);
            HasColor = true;
        }
        else if (HasSkeleton && VertexStride == 32)
        {
            XboxModelReader.ReadSkinnedVertexData32(binReader, VertexCount, Vertices);
            HasColor = true;
        }
        else if (VertexStride == 36)
        {
            XboxModelReader.ReadUnskinnedVertexData36(binReader, VertexCount, Vertices);
            HasColor = true;
            HasUV2 = true;
        }

        if(HasSkeleton)
        {
            foreach (CommonVertexInstance vbi in Vertices)
            {
                vbi.BoneIndices = new short[3];
                vbi.BoneIndices[0] = binReader.ReadInt16();
                vbi.BoneIndices[1] = binReader.ReadInt16();
                vbi.BoneIndices[2] = binReader.ReadInt16();

                //DebugBoneWeights(vbi.BoneIndices[0]);
                //DebugBoneWeights(vbi.BoneIndices[1]);
                //DebugBoneWeights(vbi.BoneIndices[2]);

                //for (int z = 0; z < vbi.BoneIndices.Length; ++z)
                //{
                //    if (z == 6)
                //    {
                //        break;
                //    }
                //}


            }
        }


        //binReader.BaseStream.Position = originalPosition;
    }
}
