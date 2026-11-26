using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Editor;
using Assets.Editor.Test;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TestCreateSkinDataStub))]
public class TestCreateSkinData : Editor
{
    public string OriginalName = "**Original**";
    public string RebuiltName = "**Rebuilt**";

    private List<SkinData> m_originalSkinData = new List<SkinData>();
    private List<SkinData> m_newSkinData = new List<SkinData>();

    private List<(bool, bool, bool,bool)> m_foldOutsCompareList = new List<(bool, bool, bool,bool)>();
    private List<(bool, bool, bool,bool)> m_foldOutsSingleList = new List<(bool, bool, bool,bool)>();

    private bool m_diffData = true;
    private bool m_overrideSort = true;
    
    private bool m_displaySingle = false;

    private GameObject m_originalModel;
    private GameObject m_rebuiltModel;


    private GCModel m_originalGCModel;
    private GCModel m_rebuiltGCModel;
    private GCModel m_sanityCheckGCModel;

    GCModel m_lhsCompareGCModel = null;
    GCModel m_rhsCompareGCModel = null;

    
    public override void OnInspectorGUI()
    {
        TestCreateSkinDataStub stub = target as TestCreateSkinDataStub;

        base.OnInspectorGUI();

        m_diffData = EditorGUILayout.Toggle("Diff Data", m_diffData);
        
        m_overrideSort = EditorGUILayout.Toggle("Override Sort", m_overrideSort);

        if (GUILayout.Button("Load model info"))
        {
            m_displaySingle = true;
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(stub.OriginalModel.bytes)))
            {
                m_originalGCModel = GCModel.ReadData(binReader, "", null);

                int count = m_originalGCModel.GetChunk<SKINChunk>().SkinDataList.Count;
                m_foldOutsSingleList.Clear();
                for (int i = 0; i < count; i++)
                {
                    m_foldOutsSingleList.Add((false, false, false,false));
                }
            }
        }

        if (GUILayout.Button("Rebuild and Compare model"))
        {
            m_lhsCompareGCModel = null;
            m_rhsCompareGCModel = null;
            
            m_displaySingle = false;
            m_originalGCModel = null;
            m_rebuiltGCModel = null;
            m_sanityCheckGCModel = null;

            GameObject go = GameObject.Find(OriginalName);
            if (go != null)
            {
                DestroyImmediate(go);
            }

            go = GameObject.Find(RebuiltName);
            if (go != null)
            {
                DestroyImmediate(go);
            }

            CommonModelData originalCommonModel = null;
            CommonModelData rebuiltCommonModel = null;

            string chunkComparePath = "d:/tmp/chunk-compare";
            
            string chunkComparePathOriginal = chunkComparePath + "/original";
            Directory.CreateDirectory(chunkComparePathOriginal);

            string chunkComparePathOriginalWrite = chunkComparePath + "/original-write";
            Directory.CreateDirectory(chunkComparePathOriginalWrite);

            string chunkComparePathRebuiltWrite = chunkComparePath + "/rebuilt-write";
            Directory.CreateDirectory(chunkComparePathRebuiltWrite);
            
            
            StringBuilder chunkAlignmentLHS = new StringBuilder();
            StringBuilder chunkAlignmentRHS = new StringBuilder();
            
            byte[] originalModelReadBuffer = null;
            byte[] originalModelWriteBuffer = null;

            // Load the skin data into a model.
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(stub.OriginalModel.bytes)))
            {
                originalModelReadBuffer = binReader.ReadBytes(stub.OriginalModel.bytes.Length);
                binReader.BaseStream.Position = 0;
                StringBuilder debugInfo = new StringBuilder();
                m_originalGCModel = GCModel.ReadData(binReader, "", debugInfo);
                m_lhsCompareGCModel = m_originalGCModel;

                using (MemoryStream writeMemoryStream = new MemoryStream())
                {
                    using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                    {
                        m_originalGCModel.ToStream(binWriter);
                    }

                    originalModelWriteBuffer = writeMemoryStream.ToArray();
                }

                foreach (BaseChunk chunk in m_originalGCModel.m_chunkList)
                {
                    File.WriteAllBytes($"{chunkComparePathOriginal}/{new string(chunk.Signature)}.bin",chunk.RawData);
                    chunkAlignmentLHS.Append($" {new string(chunk.Signature)} - {chunk.ReadStart} ");
                    foreach (int val in chunk.ReadAlignments.Keys.OrderByDescending(x => x))
                    {
                        chunkAlignmentLHS.Append($"{val}:{chunk.ReadAlignments[val]} ,");
                    }
                    chunkAlignmentLHS.AppendLine();
                }
                

                foreach (BaseChunk chunk in m_originalGCModel.m_chunkList)
                {
                    using (MemoryStream writeMemoryStream = new MemoryStream())
                    {
                        using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                        {
                            chunk.ToStream(binWriter);
                        }
                        File.WriteAllBytes($"{chunkComparePathOriginalWrite}/{new string(chunk.Signature)}.bin",
                            writeMemoryStream.ToArray());
                    }
                }

                
                using (MemoryStream writeMemoryStream = new MemoryStream())
                {
                    using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                    {
                        //m_originalGCModel.ToStreamRawData(binWriter);
                        m_originalGCModel.ToStream(binWriter);
                        
                    }

                    foreach (BaseChunk chunk in m_originalGCModel.m_chunkList)
                    {
                        chunkAlignmentRHS.Append($" {new string(chunk.Signature)} - {chunk.WriteStart} ");
                        foreach (int val in chunk.WriteAlignments.Keys.OrderByDescending(x => x))
                        {
                            chunkAlignmentRHS.Append($"{val}:{chunk.WriteAlignments[val]} ,");
                        }

                        chunkAlignmentRHS.AppendLine();
                    }
                }

                originalCommonModel = m_originalGCModel.ToCommon();

            }

            File.WriteAllText($"{chunkComparePathOriginal}/alignments.txt", chunkAlignmentLHS.ToString());
            File.WriteAllText($"{chunkComparePathOriginalWrite}/alignments.txt", chunkAlignmentRHS.ToString());

            
            int filteredMeshCount = m_originalGCModel.CountMeshesForLodLevel(stub.LodLevel);
            m_foldOutsCompareList.Clear();
            for (int i = 0; i < filteredMeshCount; i++)
            {
                m_foldOutsCompareList.Add((false, false, false,false));
            }

            if (originalCommonModel != null)
            {
            //     using (MemoryStream readMemoryStream = new MemoryStream(originalModelWriteBuffer))
            //     {
            //         using (BinaryReader binReader = new BinaryReader(readMemoryStream))
            //         {
            //                         
            //             StringBuilder debugInfo = new StringBuilder();
            //             try
            //             {
            //                 m_sanityCheckGCModel = GCModel.ReadData(binReader, "SanityCheck", debugInfo);
            //                 string chunkComparePathSanity = chunkComparePath + "/sanity";
            //                 Directory.CreateDirectory(chunkComparePathSanity);
            //                 
            //                 foreach (BaseChunk chunk in m_originalGCModel.m_chunkList)
            //                 {
            //                     using (MemoryStream writeMemoryStream = new MemoryStream())
            //                     {
            //                         using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
            //                         {
            //                             chunk.ToStream(binWriter);
            //                         }
            //                         File.WriteAllBytes($"{chunkComparePathSanity}/{new string(chunk.Signature)}.bin",
            //                             writeMemoryStream.ToArray());
            //                     }
            //                 }
            //                 
            //                 m_rhsCompareGCModel = m_sanityCheckGCModel;
            //             }
            //             catch (Exception e)
            //             {
            //                 Debug.Log(debugInfo.ToString());
            //             }
            //         }
            //     }

                
                
                string assetName = "test";
                string outputHierarchy = "";

                string prefabOutputDirectory = "";

                if (m_originalModel != null)
                {
                    DestroyImmediate(m_originalModel);
                }

                m_originalModel = CommonModelProcessor.CommonModelToGameObject(outputHierarchy, stub.LodLevel,
                    originalCommonModel, out Dictionary<BoneNode, GameObject> boneObjectMapOriginal);


                
                if (m_originalModel != null)
                {
                    
                    m_originalModel.name = OriginalName;
                    m_originalModel.transform.position = new Vector3(-2, 0, 0);

                    SkinnedMeshRenderer[] oldsmr =m_originalModel.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
                    
                    m_rebuiltGCModel = GCModel.CreateFromGameObject(m_originalModel,(short)stub.AnimShift,stub.LodLevel,stub.RamSize,null,false,stub.MaxWeights,stub.MaxVertices);

                    if (m_rebuiltGCModel != null)
                    {
                        m_rhsCompareGCModel = m_rebuiltGCModel;

                        List<BaseChunk>
                            writeChunks = new List<BaseChunk>(); //m_rebuiltGCModel.GetOrderedChunksForWriting();

                        foreach (BaseChunk basechunk in m_originalGCModel.m_chunkList)
                        {
                            if (basechunk is PADDChunk)
                            {
                                writeChunks.Add(new PADDChunk());
                            }
                            else
                            {
                                writeChunks.Add(m_rebuiltGCModel.m_chunkList.Find(chunk =>
                                    chunk.GetType() == basechunk.GetType()));
                            }
                        }

                        // m_rebuiltGCModel.m_chunkList.Clear();
                        // m_rebuiltGCModel.m_chunkList.AddRange(writeChunks);


                        foreach (BaseChunk chunk in m_rebuiltGCModel.m_chunkList)
                        {
                            using (MemoryStream writeMemoryStream = new MemoryStream())
                            {
                                using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                                {
                                    chunk.ToStream(binWriter);
                                }

                                File.WriteAllBytes($"{chunkComparePathRebuiltWrite}/{new string(chunk.Signature)}.bin",
                                    writeMemoryStream.ToArray());
                            }
                        }



                        byte[] buffer = null;

                        using (MemoryStream writeMemoryStream = new MemoryStream())
                        {
                            using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                            {
                                //foreach (BaseChunk chunk in m_rebuiltGCModel.m_chunkList)
                                // {
                                //     chunk.ToStream(binWriter);
                                // }
                                m_rebuiltGCModel.ToStream(binWriter);
                            }

                            writeMemoryStream.Flush();
                            buffer = writeMemoryStream.ToArray();
                        }

                        File.WriteAllBytes(@"D:\GladiusModding\GC\unpack\data\characters\galdrf\galdrf.pax", buffer);


                        if (m_lhsCompareGCModel != null && m_rhsCompareGCModel != null)
                        {

                            DSLSChunk oldDS = m_lhsCompareGCModel.GetChunk<DSLSChunk>();
                            DSLSChunk newDS = m_rhsCompareGCModel.GetChunk<DSLSChunk>();

                            StringBuilder oldSB = new StringBuilder();
                            StringBuilder newSB = new StringBuilder();

                            foreach (DisplayListEntry dse in oldDS.DisplayListHeaders[0].entries)
                            {
                                oldSB.AppendLine($"{dse.PosIndex},{dse.NormIndex},{dse.UVIndex}");
                            }

                            foreach (DisplayListEntry dse in newDS.DisplayListHeaders[0].entries)
                            {
                                newSB.AppendLine($"{dse.PosIndex},{dse.NormIndex},{dse.UVIndex}");
                            }

                            if (m_lhsCompareGCModel.IsSkinned())
                            {

                                SKELChunk oldSKEL = m_lhsCompareGCModel.GetChunk<SKELChunk>();
                                SKELChunk newSKEL = m_rhsCompareGCModel.GetChunk<SKELChunk>();

                                StringBuilder oldSBS = new StringBuilder();
                                StringBuilder newSBS = new StringBuilder();

                                foreach (BoneNode boneNode in oldSKEL.BoneList)
                                {
                                    oldSBS.AppendLine(
                                        $"{boneNode.Index} , {boneNode.ParentIndex} , {boneNode.offset},{boneNode.rotation}");
                                }

                                foreach (BoneNode boneNode in newSKEL.BoneList)
                                {
                                    newSBS.AppendLine(
                                        $"{boneNode.Index} , {boneNode.ParentIndex} , {boneNode.offset},{boneNode.rotation}");

                                }

                                SKINChunk oldSKIN = m_lhsCompareGCModel.GetChunk<SKINChunk>();
                                SKINChunk newSKIN = m_rhsCompareGCModel.GetChunk<SKINChunk>();

                                UV0Chunk oldUV0 = m_lhsCompareGCModel.GetChunk<UV0Chunk>();
                                UV0Chunk newUV0 = m_rhsCompareGCModel.GetChunk<UV0Chunk>();

                                List<Vector3> oldPositions = new List<Vector3>();
                                List<Vector3> oldNormals = new List<Vector3>();

                                foreach (CSK1 csk in oldSKIN.SkinDataList[0].CSK1List)
                                {
                                    oldPositions.AddRange(csk.ExtractedPositions);
                                    //oldNormals.AddRange(csk.ExtractedNormals);
                                }

                                foreach (CSK2 csk in oldSKIN.SkinDataList[0].CSK2List)
                                {
                                    oldPositions.AddRange(csk.ExtractedPositions);
                                    //oldNormals.AddRange(csk.ExtractedNormals);
                                }

                                List<Vector3> newPositions = new List<Vector3>();
                                List<Vector3> newNormals = new List<Vector3>();

                                foreach (CSK1 csk in newSKIN.SkinDataList[0].CSK1List)
                                {
                                    newPositions.AddRange(csk.ExtractedPositions);
                                    //newNormals.AddRange(csk.ExtractedNormals);
                                }

                                foreach (CSK2 csk in newSKIN.SkinDataList[0].CSK2List)
                                {
                                    newPositions.AddRange(csk.ExtractedPositions);
                                    //newNormals.AddRange(csk.ExtractedNormals);
                                }

                                StringBuilder oldPN = new StringBuilder();
                                StringBuilder newPN = new StringBuilder();

                                foreach (Vector3 p in oldPositions)
                                {
                                    oldPN.AppendLine(p.ToString());
                                }

                                foreach (Vector3 n in oldNormals)
                                {
                                    oldPN.AppendLine(n.ToString());
                                }

                                foreach (Vector3 p in newPositions)
                                {
                                    newPN.AppendLine(p.ToString());
                                }

                                foreach (Vector3 n in newNormals)
                                {
                                    newPN.AppendLine(n.ToString());
                                }

                                StringBuilder oldUV = new StringBuilder();
                                StringBuilder newUV = new StringBuilder();

                                foreach (Vector2 uv in oldUV0.Data)
                                {
                                    oldUV.AppendLine(uv.ToString());
                                }

                                foreach (Vector2 uv in newUV0.Data)
                                {
                                    newUV.AppendLine(uv.ToString());
                                }


                                File.WriteAllText("d:/tmp/old-dse.txt", oldSB.ToString());
                                File.WriteAllText("d:/tmp/new-dse.txt", newSB.ToString());
                                
                                File.WriteAllText("d:/tmp/old-skel.txt", oldSBS.ToString());
                                File.WriteAllText("d:/tmp/new-skel.txt", newSBS.ToString());
                                
                                File.WriteAllText("d:/tmp/old-pn.txt", oldPN.ToString());
                                File.WriteAllText("d:/tmp/new-pn.txt", newPN.ToString());
                                
                                File.WriteAllText("d:/tmp/old-uv.txt", oldUV.ToString());
                                File.WriteAllText("d:/tmp/new-uv.txt", newUV.ToString());
                            }
                        }
                    }

                    if (m_lhsCompareGCModel.IsSkinned())
                    {
                        if (m_lhsCompareGCModel?.GetChunk<SKINChunk>().SkinDataList.Count > 0 &&
                            m_rhsCompareGCModel?.GetChunk<SKINChunk>().SkinDataList.Count > 0)
                        {
                            m_originalSkinData.Clear();
                            for (int i = 0; i < m_lhsCompareGCModel.GetChunk<MESHChunk>().NumElements; i++)
                            {
                                if ((m_lhsCompareGCModel.LodLevelForMesh(i) & stub.LodLevel) == stub.LodLevel)
                                {
                                    SkinData skinData = m_lhsCompareGCModel.GetChunk<SKINChunk>().SkinDataList[i];
                                    m_originalSkinData.Add(skinData);

                                    if (m_overrideSort)
                                    {
                                        skinData.CSK1List = skinData.CSK1List.OrderByDescending(x => x.count)
                                            .ThenBy(x => x.idxBone).ToList();
                                        skinData.CSK2List = skinData.CSK2List.OrderByDescending(x => x.count)
                                            .ThenBy(y => y.idxBone[0]).ThenByDescending(y => y.idxBone[1]).ToList();
                                        skinData.CSKAList = skinData.CSKAList.OrderByDescending(x => x.count)
                                            .ThenBy(x => x.idxBone).ToList();

                                    }

                                }
                            }

                            m_newSkinData.Clear();
                            for (int i = 0; i < m_rhsCompareGCModel.GetChunk<MESHChunk>().NumElements; i++)
                            {
                                if ((m_rhsCompareGCModel.LodLevelForMesh(i) & stub.LodLevel) == stub.LodLevel)
                                {
                                    SkinData skinData = m_rhsCompareGCModel.GetChunk<SKINChunk>().SkinDataList[i];
                                    m_newSkinData.Add(skinData);

                                    if (m_overrideSort)
                                    {
                                        skinData.CSK1List = skinData.CSK1List.OrderByDescending(x => x.count)
                                            .ThenBy(x => x.idxBone).ToList();
                                        skinData.CSK2List = skinData.CSK2List.OrderByDescending(x => x.count)
                                            .ThenBy(y => y.idxBone[0]).ThenByDescending(y => y.idxBone[1]).ToList();
                                        skinData.CSKAList = skinData.CSKAList.OrderByDescending(x => x.count)
                                            .ThenBy(x => x.idxBone).ToList();

                                    }
                                }
                            }
                        }
                    }

                    if (m_rebuiltModel != null)
                    {
                        DestroyImmediate(m_rebuiltModel);
                    }

                    if (m_rebuiltGCModel != null)
                    {
                        rebuiltCommonModel = m_rebuiltGCModel.ToCommon();
                        m_rebuiltModel = CommonModelProcessor.CommonModelToGameObject(outputHierarchy, stub.LodLevel,
                            rebuiltCommonModel, out Dictionary<BoneNode, GameObject> boneObjectMapRebuilt);
                        m_rebuiltModel.name = RebuiltName;
                        m_rebuiltModel.transform.position = new Vector3(2, 0, 0);
                        
                        
                        SkinnedMeshRenderer[] newsmr =m_rebuiltModel.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

                        StringBuilder oldSB = new StringBuilder();
                        StringBuilder newSB = new StringBuilder();
                        
                        foreach (SkinnedMeshRenderer smr in oldsmr)
                        {
                            foreach (BoneWeight bw in smr.sharedMesh.boneWeights)
                            {
                                if (bw.CountWeights() != 1)
                                {
                                    continue;
                                }
                                oldSB.AppendLine(
                                    $"{bw.boneIndex0},{bw.boneIndex1},{bw.boneIndex2},{bw.boneIndex3} : {bw.weight0},{bw.weight1},{bw.weight2},{bw.weight3}");
                            }

                            break;
                        }
                        
                        foreach (SkinnedMeshRenderer smr in newsmr)
                        {
                            foreach (BoneWeight bw in smr.sharedMesh.boneWeights)
                            {
                                if (bw.CountWeights() != 1)
                                {
                                    continue;
                                }

                                newSB.AppendLine(
                                    $"{bw.boneIndex0},{bw.boneIndex1},{bw.boneIndex2},{bw.boneIndex3} : {bw.weight0},{bw.weight1},{bw.weight2},{bw.weight3}");
                            }

                            break;
                        }
                        
                        File.WriteAllText("d:/tmp/old-bp.txt", oldSB.ToString());
                        File.WriteAllText("d:/tmp/new-bp.txt", newSB.ToString());
                        
                        
                
                        StringBuilder chunkAlignmentRebuild = new StringBuilder();
                        foreach (BaseChunk chunk in m_rebuiltGCModel.m_chunkList)
                        {
                            chunkAlignmentRebuild.Append($" {new string(chunk.Signature)} - {chunk.WriteStart} ");
                            foreach (int val in chunk.WriteAlignments.Keys.OrderByDescending(x => x))
                            {
                                chunkAlignmentRebuild.Append($"{val}:{chunk.WriteAlignments[val]} ,");
                            }

                            chunkAlignmentRebuild.AppendLine();
                        }

                        File.WriteAllText($"{chunkComparePathRebuiltWrite}/alignments.txt", chunkAlignmentRebuild.ToString());
                        
                    }
                }
            }
        }

        if (GUILayout.Button("Dump All Skindata"))
        {
            List<GCModel> models = new List<GCModel>();
            string[] files = Directory.GetFiles(stub.DumpSearchDirectory, "*.pax", SearchOption.AllDirectories);
            int counter = 0;

            StringBuilder outputInfo = new StringBuilder();
            StringBuilder chunkOrderInfo = new StringBuilder();
            StringBuilder chunkAlignmentInfo = new StringBuilder();
            
            
            outputInfo.AppendLine(
                "Model,SkinData#,Flags,AnimShift,CSK1 size,CSK2 size,CSKA size,NumPacket1,Packet1Start,Packet1Size,Packet1Info,NumPacket2,Packet2Start,Packet2Size,Packet2Info");

            chunkAlignmentInfo.AppendLine("Model,Chunk,256,128,64,32,16");
            
            
            foreach (String file in files)
            {
                try
                {
                    GCModel model = GCModelReader.LoadSingleModel(file, true);
                    if (model != null && model.IsSkinned())
                    {
                        int paddChunks = model.m_chunkList.FindAll(x=>x is PADDChunk).Count;

                        chunkOrderInfo.Append($"{model.m_name} ({paddChunks}) ELEM({model.GetChunk<ELEMChunk>().NumElements}) MESH({model.GetChunk<MESHChunk>().NumElements}) DSLS({model.GetChunk<DSLSChunk>().NumElements})({model.GetChunk<DSLSChunk>().DisplayListHeaders.Count})  DSLI({model.GetChunk<DSLIChunk>().NumElements})({model.GetChunk<DSLIChunk>().DisplayListIndices.Count})  DSLC({model.GetChunk<DSLCChunk>().NumElements}) SKIN({model.GetChunk<SKINChunk>().SkinDataList.Count})");
                        
                        foreach (BaseChunk chunk in model.m_chunkList)
                        {
                            // chunkOrderInfo.Append(new String(chunk.Signature));
                            // //if (chunk is PADDChunk)
                            // {
                            //     chunkOrderInfo.Append($"({chunk.Length}) [16:{chunk.ReadStart%16} ,32:{chunk.ReadStart%32} , 64:{chunk.ReadStart%64}, 128:{chunk.ReadStart%128}]");
                            // }
                            // chunkOrderInfo.Append(",");
                            
                            chunkAlignmentInfo.AppendLine($"{model.m_name},{new string(chunk.Signature)},{chunk.ReadAlignments[256]},{chunk.ReadAlignments[128]},{chunk.ReadAlignments[64]},{chunk.ReadAlignments[32]},{chunk.ReadAlignments[16]}");
                        }

                        chunkOrderInfo.AppendLine();
                        
                        
                        models.Add(model);
                        model.GetChunk<SKINChunk>().DebugChunk(outputInfo);
                        
                    }
                }
                catch (Exception e)
                {
                }
            }

            File.WriteAllText(stub.DumpFileName, outputInfo.ToString());
            File.WriteAllText("d:/tmp/skinned-model-chunk-order.txt", chunkOrderInfo.ToString());
            File.WriteAllText("d:/tmp/skinned-model-chunk-alignment.csv", chunkAlignmentInfo.ToString());
            
        }


        if (m_displaySingle)
        {
            DisplaySingleModel();
        }
        else
        {
            DisplayModelCompare();
        }

    }
    
    public void DisplaySingleModel()
    {
        if (m_originalGCModel == null || m_originalGCModel.GetChunk<SKINChunk>() == null)
        {
            return;
        }
        
        int numRows = m_originalGCModel.GetChunk<SKINChunk>().SkinDataList.Count;

        for (int i = 0; i < numRows; i++)
        {
            SkinData origSD = m_originalGCModel.GetChunk<SKINChunk>().SkinDataList[i];
            ChunkComparisonTools.DrawSingleSkinData(i,origSD,m_foldOutsSingleList);
        }
    }


    public void DisplayModelCompare()
    {
        if (m_lhsCompareGCModel != null && m_rhsCompareGCModel != null)
        {
            ChunkComparisonTools.DrawChunkCompare(m_lhsCompareGCModel, m_rhsCompareGCModel, Color.green, Color.yellow);

            ChunkComparisonTools.DrawDSLIListsCompare(m_lhsCompareGCModel.GetChunk<DSLIChunk>().DisplayListIndices,
                m_rhsCompareGCModel.GetChunk<DSLIChunk>().DisplayListIndices, m_lhsCompareGCModel.GetChunk<DSLSChunk>(),
                m_rhsCompareGCModel.GetChunk<DSLSChunk>());

            ChunkComparisonTools.DrawSkinDataListsCompare(m_originalSkinData, m_newSkinData, m_diffData,
                m_foldOutsCompareList);
        }
    }

}