using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Editor;
using TMPro;
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
    private bool m_displaySingle = false;

    public int IndentLevel = 20;

    private GameObject m_originalModel;
    private GameObject m_rebuiltModel;


    private GCModel m_originalGCModel;
    private GCModel m_rebuiltGCModel;
    private GCModel m_sanityCheckGCModel;

    public override void OnInspectorGUI()
    {
        TestCreateSkinDataStub stub = target as TestCreateSkinDataStub;

        base.OnInspectorGUI();

        m_diffData = EditorGUILayout.Toggle("Diff Data", m_diffData);

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


            // Load the skin data into a model.
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(stub.OriginalModel.bytes)))
            {
                m_originalGCModel = GCModel.ReadData(binReader, "", null);
                originalCommonModel = m_originalGCModel.ToCommon();
            }

            int filteredMeshCount = m_originalGCModel.CountMeshesForLodLevel(stub.LodLevel);
            m_foldOutsCompareList.Clear();
            for (int i = 0; i < filteredMeshCount; i++)
            {
                m_foldOutsCompareList.Add((false, false, false,false));
            }

            if (originalCommonModel != null)
            {
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
                    m_originalModel.transform.position = new Vector3(-10, 0, 0);


                    m_rebuiltGCModel = GCModel.CreateFromGameObject(m_originalModel,(short)stub.AnimShift);

                    if (m_rebuiltGCModel != null)
                    {
                        byte[] buffer = null;

                        using (MemoryStream writeMemoryStream = new MemoryStream())
                        {
                            using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                            {
                                m_rebuiltGCModel.WriteData(binWriter);
                            }

                            buffer = writeMemoryStream.ToArray();
                        }

                        if (buffer != null)
                        {
                            using (MemoryStream readMemoryStream = new MemoryStream(buffer))
                            {
                                using (BinaryReader binReader = new BinaryReader(readMemoryStream))
                                {
                                    m_sanityCheckGCModel = GCModel.ReadData(binReader, "SanityCheck", null);
                                }
                            }
                        }
                    }


                    if (m_originalGCModel?.GetChunk<SKINChunk>().SkinDataList.Count > 0 &&
                        m_rebuiltGCModel?.GetChunk<SKINChunk>().SkinDataList.Count > 0)
                    {
                        m_originalSkinData.Clear();
                        for (int i = 0; i < m_originalGCModel.GetChunk<MESHChunk>().NumElements; i++)
                        {
                            if ((m_originalGCModel.LodLevelForMesh(i) & stub.LodLevel) == stub.LodLevel)
                            {
                                m_originalSkinData.Add(m_originalGCModel.GetChunk<SKINChunk>().SkinDataList[i]);
                            }
                        }

                        m_newSkinData.Clear();
                        for (int i = 0; i < m_rebuiltGCModel.GetChunk<MESHChunk>().NumElements; i++)
                        {
                            if ((m_rebuiltGCModel.LodLevelForMesh(i) & stub.LodLevel) == stub.LodLevel)
                            {
                                m_newSkinData.Add(m_rebuiltGCModel.GetChunk<SKINChunk>().SkinDataList[i]);
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
                        m_rebuiltModel.transform.position = new Vector3(10, 0, 0);
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
            outputInfo.AppendLine(
                "Model,SkinData#,Flags,AnimShift,CSK1 size,CSK2 size,CSKA size,NumPacket1,Packet1Start,Packet1Size,Packet1Info,NumPacket2,Packet2Start,Packet2Size,Packet2Info");

            foreach (String file in files)
            {
                try
                {
                    GCModel model = GCModelReader.LoadSingleModel(file, true);
                    if (model != null)
                    {
                        models.Add(model);
                        if (model.GetChunk<SKINChunk>() != null)
                        {
                            int count = 0;
                            foreach (SkinData skinData in model.GetChunk<SKINChunk>().SkinDataList)
                            {
                                string packet1Info = "";
                                for (int i = 0; i < skinData.NumPackets1; i++)
                                {
                                    packet1Info += $"[{skinData.Packet1Starts[i]}->{skinData.Packet1Sizes[i]}]**";
                                }

                                string packet2Info = "";
                                for (int i = 0; i < skinData.NumPackets2; i++)
                                {
                                    packet2Info += $"[{skinData.Packet2Starts[i]}->{skinData.Packet2Sizes[i]}]**";
                                }

                                outputInfo.AppendLine(
                                    $"{model.m_name}, {count++}, {skinData.Flags},{skinData.AnimShift},{skinData.CSK1List.Count},{skinData.CSK2List.Count},{skinData.CSKAList.Count},{skinData.NumPackets1},{skinData.PacketStart1},{skinData.PacketSize1},{packet1Info},{skinData.NumPackets2},{skinData.PacketStart2},{skinData.PacketSize2},{packet2Info}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }

            File.WriteAllText(stub.DumpFileName, outputInfo.ToString());
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

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Mesh {i}", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumList1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumList2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumListA", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumPackets1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets1Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets1Sizes", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumPackets2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets2Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets2Sizes", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(origSD.Size.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumList1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumList2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumListA.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumPackets1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketStart1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketSize1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumPackets2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketStart2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketSize2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));

            GUILayout.EndHorizontal();

            
            bool csk1FoldOut = m_foldOutsSingleList[i].Item1;
            bool csk2FoldOut = m_foldOutsSingleList[i].Item2;
            bool cskAFoldOut = m_foldOutsSingleList[i].Item3;
            bool packetsFoldout = m_foldOutsSingleList[i].Item4;

            packetsFoldout = StartIndentedFoldoutHeader("Packets", packetsFoldout);
            if (packetsFoldout)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Packet1 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Packet1 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                for (int j = 0; j < origSD.NumPackets1; j++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{origSD.Packet1Starts[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{origSD.Packet1Sizes[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Packet2 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Packet2 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                for (int j = 0; j < origSD.NumPackets2; j++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{origSD.Packet2Starts[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{origSD.Packet2Sizes[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();
                }

            }
            EndIndentedFoldoutHeader();
            
            csk1FoldOut = StartIndentedFoldoutHeader("CSK1", csk1FoldOut);
            if (csk1FoldOut)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                foreach (CSK1 csk1 in origSD.CSK1List)
                {
                    GUILayout.BeginHorizontal();
                    DrawCSK1(csk1, Color.green);
                    GUILayout.EndHorizontal();
                }
            }

            EndIndentedFoldoutHeader();

            csk2FoldOut = StartIndentedFoldoutHeader("CSK2", csk2FoldOut);
            if (csk2FoldOut)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IdxBone1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("IdxBone2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                foreach (CSK2 csk2 in origSD.CSK2List)
                {
                    GUILayout.BeginHorizontal();
                    DrawCSK2(csk2, Color.green);
                    GUILayout.EndHorizontal();
                }
            }

            EndIndentedFoldoutHeader();

            cskAFoldOut = StartIndentedFoldoutHeader("CSKA", cskAFoldOut);
            if (cskAFoldOut)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("VertSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("WeightSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();
                foreach (CSKA cska in origSD.CSKAList)
                {
                    GUILayout.BeginHorizontal();
                    DrawCSKA(cska, Color.green);
                    GUILayout.EndHorizontal();
                }
            }

            EndIndentedFoldoutHeader();

            m_foldOutsSingleList[i] = (csk1FoldOut, csk2FoldOut, cskAFoldOut,packetsFoldout);
        }
    }


    public void DisplayModelCompare()
    {
        if (m_rebuiltGCModel != null && m_sanityCheckGCModel != null)
        {
            DrawChunkCompare(m_rebuiltGCModel, m_sanityCheckGCModel, Color.green, Color.yellow);
        }

        if (m_originalSkinData.Count > 0 && m_newSkinData.Count > 0)
        {
            int numRows = Math.Min(m_originalSkinData.Count, m_newSkinData.Count);

            for (int i = 0; i < numRows; i++)
            {
                SkinData origSD = m_originalSkinData[i];
                SkinData newSD = m_newSkinData[i];

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Mesh {i}", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("NumList1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("NumList2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("NumListA", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("NumPackets1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("PacketsStart1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("PacketsSizes1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("NumPackets2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("PacketsStart2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Packets2Sizes2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(origSD.Size.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.NumList1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.NumList2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.NumListA.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.NumPackets1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.PacketStart1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.PacketSize1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.NumPackets2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.PacketStart2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(origSD.PacketSize2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(newSD.Size.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.NumList1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.NumList2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.NumListA.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.NumPackets1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.PacketStart1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.PacketSize1.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.NumPackets2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.PacketStart2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField(newSD.PacketSize2.ToString(), GetTableStyle(),
                    GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                bool csk1FoldOut = m_foldOutsCompareList[i].Item1;
                bool csk2FoldOut = m_foldOutsCompareList[i].Item2;
                bool cskAFoldOut = m_foldOutsCompareList[i].Item3;
                bool packetsFoldout = m_foldOutsCompareList[i].Item4;

                packetsFoldout = StartIndentedFoldoutHeader("Packets", packetsFoldout);
                if (packetsFoldout)
                {
                    DrawPacketsCompare(origSD, newSD, Color.green, Color.yellow);

                }
                EndIndentedFoldoutHeader();

                
                
                csk1FoldOut = StartIndentedFoldoutHeader("CSK1", csk1FoldOut);
                if (csk1FoldOut)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();

                    if (m_diffData)
                    {
                        for (int j = 0; j < origSD.CSK1List.Count; j++)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSK1Compare(origSD.CSK1List[j], newSD.CSK1List[j], Color.green,
                                Color.yellow);
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        foreach (CSK1 csk1 in origSD.CSK1List)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSK1(csk1, Color.green);
                            GUILayout.EndHorizontal();
                        }

                        foreach (CSK1 csk1 in newSD.CSK1List)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSK1(csk1, Color.yellow);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                EndIndentedFoldoutHeader();

                csk2FoldOut = StartIndentedFoldoutHeader("CSK2", csk2FoldOut);
                if (csk2FoldOut)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("IdxBone1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("IdxBone2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();

                    if (m_diffData)
                    {
                        for (int j = 0; j < origSD.CSK2List.Count; j++)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSK2Compare(origSD.CSK2List[j], newSD.CSK2List[j], Color.green,
                                Color.yellow);
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        foreach (CSK2 csk2 in origSD.CSK2List)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSK2(csk2, Color.green);
                            GUILayout.EndHorizontal();
                        }

                        foreach (CSK2 csk2 in newSD.CSK2List)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSK2(csk2, Color.yellow);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                EndIndentedFoldoutHeader();

                cskAFoldOut = StartIndentedFoldoutHeader("CSKA", cskAFoldOut);
                if (cskAFoldOut)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("VertSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("WeightSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();
                    if (m_diffData)
                    {
                        for (int j = 0; j < origSD.CSKAList.Count; j++)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSKACompare(origSD.CSKAList[j], newSD.CSKAList[j], Color.green,
                                Color.yellow);
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        foreach (CSKA cska in origSD.CSKAList)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSKA(cska, Color.green);
                            GUILayout.EndHorizontal();
                        }

                        foreach (CSKA cska in newSD.CSKAList)
                        {
                            GUILayout.BeginHorizontal();
                            DrawCSKA(cska, Color.yellow);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                

                m_foldOutsCompareList[i] = (csk1FoldOut, csk2FoldOut, cskAFoldOut,packetsFoldout);
            }
        }

    }
    

    public void DrawCSK1(CSK1 csk1, Color color)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = color;
        EditorGUILayout.LabelField(csk1.idxBone.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField(csk1.count.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField(csk1.vertSrc.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField(csk1.vertDst.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = oldColor;
    }

    public Color CompareVals(int val1, int val2, Color matchColor, Color diffColor)
    {
        return val1 == val2 ? matchColor : diffColor;
    }

    public void DrawCSK1Compare(CSK1 oldCsk1, CSK1 newCsk1, Color matchColor, Color diffColor)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = CompareVals(oldCsk1.idxBone, newCsk1.idxBone, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.idxBone} / {newCsk1.idxBone}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCsk1.count, newCsk1.count, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.count} / {newCsk1.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk1.vertSrc, (int)newCsk1.vertSrc, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.vertSrc} / {newCsk1.vertSrc}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk1.vertDst, (int)newCsk1.vertDst, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.vertDst} / {newCsk1.vertDst}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = oldColor;
    }


    public void DrawCSK2(CSK2 csk2, Color color)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = color;
        EditorGUILayout.LabelField(csk2.idxBone[0].ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField(csk2.idxBone[1].ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField(csk2.count.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField(csk2.vertSrc.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField(csk2.vertDst.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = oldColor;
    }

    public void DrawCSK2Compare(CSK2 oldCsk2, CSK2 newCsk2, Color matchColor, Color diffColor)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = CompareVals(oldCsk2.idxBone[0], newCsk2.idxBone[0], matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.idxBone[0]} / {newCsk2.idxBone[0]}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCsk2.idxBone[1], newCsk2.idxBone[1], matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.idxBone[1]} / {newCsk2.idxBone[1]}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCsk2.count, newCsk2.count, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.count} / {newCsk2.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk2.vertSrc, (int)newCsk2.vertSrc, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.vertSrc} / {newCsk2.vertSrc}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk2.vertDst, (int)newCsk2.vertDst, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.vertDst} / {newCsk2.vertDst}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));

        GUI.contentColor = oldColor;
    }


    public void DrawCSKA(CSKA cska, Color color)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = color;
        EditorGUILayout.LabelField($"{cska.idxBone}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField($"{cska.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField($"{cska.vertSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField($"{cska.weightsSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = oldColor;
    }

    public void DrawCSKACompare(CSKA oldCSKA, CSKA newCSKA, Color matchColor, Color diffColor)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = CompareVals(oldCSKA.idxBone, newCSKA.idxBone, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.idxBone} / {newCSKA.idxBone}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCSKA.count, newCSKA.count, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.count} / {newCSKA.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCSKA.vertSrc, (int)newCSKA.vertSrc, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.vertSrc} / {newCSKA.vertSrc}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCSKA.weightsSrc, (int)newCSKA.weightsSrc, matchColor, diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.weightsSrc} / {newCSKA.weightsSrc}", GetTableStyle(),
            GUILayout.Width(ColumnWidth));
        GUI.contentColor = oldColor;
    }


    public void DrawChunkCompare(GCModel model1, GCModel model2, Color matchColor, Color diffColor)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Chunk", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField($"Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField($"Elements", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
        GUILayout.EndHorizontal();
        Color oldColor = GUI.contentColor;

        foreach (BaseChunk model1Chunk in model1.m_chunkList)
        {
            if (model1Chunk == null)
            {
                int ibreak = 0;
            }

            BaseChunk model2Chunk =
                model2.m_chunkList.Find(x => Enumerable.SequenceEqual(model1Chunk.Signature, x.Signature));

            if (model2Chunk == null)
            {
                GUI.contentColor = Color.red;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{model1Chunk.Signature}", GetTableHeaderStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField($"{model1Chunk.Length}", GetTableHeaderStyle(),
                    GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField($"{model1Chunk.NumElements}", GetTableHeaderStyle(),
                    GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{model1Chunk.Signature} / {model2Chunk.Signature}", GetTableHeaderStyle(),
                    GUILayout.Width(ColumnWidth));
                GUI.contentColor = CompareVals((int)model1Chunk.Length, (int)model2Chunk.Length, matchColor, diffColor);
                EditorGUILayout.LabelField($"{model1Chunk.Length} / {model2Chunk.Length}", GetTableHeaderStyle(),
                    GUILayout.Width(ColumnWidth));
                GUI.contentColor = CompareVals((int)model1Chunk.NumElements, (int)model2Chunk.NumElements, matchColor,
                    diffColor);
                EditorGUILayout.LabelField($"{model1Chunk.NumElements} / {model2Chunk.NumElements}",
                    GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();
            }
        }

        GUI.contentColor = oldColor;
    }


    public void DrawPacketsCompare(SkinData oldSkinData, SkinData newSkinData, Color matchColor, Color diffColor)
    {
        Color oldColor = GUI.contentColor;
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Packet1 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField("Packet1 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
        GUILayout.EndHorizontal();

        for (int j = 0; j < oldSkinData.NumPackets1; j++)
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = CompareVals(oldSkinData.Packet1Starts[j], newSkinData.Packet1Starts[j], matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldSkinData.Packet1Starts[j]} / {newSkinData.Packet1Starts[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUI.contentColor = CompareVals(oldSkinData.Packet1Sizes[j], newSkinData.Packet1Sizes[j], matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldSkinData.Packet1Sizes[j]} / {newSkinData.Packet1Sizes[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));   
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Packet2 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
        EditorGUILayout.LabelField("Packet2 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
        GUILayout.EndHorizontal();

        for (int j = 0; j < oldSkinData.NumPackets2; j++)
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = CompareVals(oldSkinData.Packet2Starts[j], newSkinData.Packet2Starts[j], matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldSkinData.Packet2Starts[j]} / {newSkinData.Packet2Starts[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUI.contentColor = CompareVals(oldSkinData.Packet2Sizes[j], newSkinData.Packet2Sizes[j], matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldSkinData.Packet2Sizes[j]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();
        }
       
    }
    
    public int ColumnWidth = 100;

    private GUIStyle GetTableHeaderStyle()
    {
        GUIStyle gs = new GUIStyle(GUI.skin.label);
        gs.fontStyle = FontStyle.Bold;
        gs.normal.textColor = Color.white;
        return gs;
    }

    private GUIStyle GetTableStyle()
    {
        GUIStyle gs = new GUIStyle(GUI.skin.label);
        gs.normal.textColor = Color.white;
        return gs;
    }

    private bool StartIndentedFoldoutHeader(string name, bool foldout)
    {
        bool returnValue = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, name,
            GetFoldoutSectionHeaderStyle(), null, GUI.skin.GetStyle("IconButton"));
        GUILayout.BeginHorizontal();
        GUILayout.Space(IndentLevel);
        GUILayout.BeginVertical();

        return returnValue;
    }

    private void EndIndentedFoldoutHeader()
    {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private GUIStyle GetFoldoutSectionHeaderStyle()
    {
        GUIStyle gs = new GUIStyle(GUI.skin.GetStyle("FoldoutHeader"));
        gs.fontStyle = FontStyle.Bold;
        gs.fontSize = 20;
        gs.normal.textColor = Color.white;
        return gs;
    }
}