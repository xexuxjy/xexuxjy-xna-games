using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.Editor;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TestCreateSkinDataStub))]
public class TestCreateSkinData : Editor
{
    
    public string OriginalName = "**Original**";
    public string RebuiltName = "**Rebuilt**";
    
    private List<SkinData> m_originalSkinData = new  List<SkinData>();
    private List<SkinData> m_newSkinData = new  List<SkinData>();

    private List<(bool, bool,bool)> m_foldOutsList = new List<(bool, bool,bool)>();
    
    private bool m_diffData = true;
    
    public int IndentLevel = 20;

    private GameObject m_originalModel;
    private GameObject m_rebuiltModel;
    
    public override void OnInspectorGUI()
    {
        TestCreateSkinDataStub stub = target as TestCreateSkinDataStub;
        
        base.OnInspectorGUI();

        m_diffData = EditorGUILayout.Toggle("Diff Data", m_diffData);
        
        if (GUILayout.Button("Process model"))
        {
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

            GCModel originalGCModel = null;
            // Load the skin data into a model.
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(stub.OriginalModel.bytes)))
            {
                originalGCModel = GCModel.ReadData(binReader,"", null);
                originalCommonModel = originalGCModel.ToCommon();
            }

            int filteredMeshCount = originalGCModel.CountMeshesForLodLevel(stub.LodLevel);
            m_foldOutsList.Clear();
            for (int i = 0; i < filteredMeshCount; i++)
            {
                m_foldOutsList.Add((false, false,false));
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
                    originalCommonModel,out Dictionary<BoneNode,GameObject> boneObjectMapOriginal);

                if (m_originalModel != null)
                {
                    m_originalModel.name = OriginalName;
                    m_originalModel.transform.position = new Vector3(-10, 0, 0);
                    
                        
                    GCModel rebuiltGCModel = GCModel.CreateFromGameObject(m_originalModel);
                    GCModel sanityCheckGCModel;
                    
                    MemoryStream memoryStream = new MemoryStream();
                    using(BinaryWriter binWriter = new BinaryWriter(memoryStream))
                    {
                        rebuiltGCModel.WriteData(binWriter);
                    }

                    memoryStream.Position = 0;
                    using (BinaryReader binReader = new BinaryReader(memoryStream))
                    {
                        sanityCheckGCModel = GCModel.ReadData(binReader, "SanityCheck", null);
                    }
                    
                    // compare the two :)
                    
                    
                    if (originalGCModel?.SKINChunk().SkinDataList.Count > 0 && rebuiltGCModel?.SKINChunk().SkinDataList.Count > 0)
                    {
                        m_originalSkinData.Clear();
                        for (int i = 0; i < originalGCModel.MESHChunk().NumElements; i++)
                        {
                            if ((originalGCModel.LodLevelForMesh(i) & stub.LodLevel) == stub.LodLevel)
                            {
                                m_originalSkinData.Add(originalGCModel.SKINChunk().SkinDataList[i]);
                            }
                        }
                        
                        m_newSkinData.Clear();
                        for (int i = 0; i < rebuiltGCModel.MESHChunk().NumElements; i++)
                        {
                            if ((rebuiltGCModel.LodLevelForMesh(i) & stub.LodLevel) == stub.LodLevel)
                            {
                                m_newSkinData.Add(rebuiltGCModel.SKINChunk().SkinDataList[i]);
                            }
                        }
                    }

                    if (m_rebuiltModel != null)
                    {
                        DestroyImmediate(m_rebuiltModel);
                    }
                    
                    rebuiltCommonModel = rebuiltGCModel.ToCommon();
                    m_rebuiltModel = CommonModelProcessor.CommonModelToGameObject(outputHierarchy, stub.LodLevel,
                        rebuiltCommonModel,out Dictionary<BoneNode,GameObject> boneObjectMapRebuilt);
                    m_rebuiltModel.name = RebuiltName;
                    m_rebuiltModel.transform.position = new Vector3(10, 0, 0);
                    
                }
            }
            
        }

        if (GUILayout.Button("Dump All Skindata"))
        {
            List<GCModel> models = new List<GCModel>();
            string[] files = Directory.GetFiles(stub.DumpSearchDirectory, "*.pax",SearchOption.AllDirectories);
            int counter = 0;
                
            StringBuilder outputInfo = new StringBuilder();
            outputInfo.AppendLine("Model,SkinData#,Flags,AnimShift,CSK1 size,CSK2 size,CSKA size");

            foreach (String file in files)
            {
                try
                {
                    GCModel model = GCModelReader.LoadSingleModel(file,true);
                    if (model != null)
                    {
                        models.Add(model);
                        if (model.SKINChunk() != null)
                        {
                            int count = 0;
                            foreach (SkinData skinData in model.SKINChunk().SkinDataList)
                            {
                                outputInfo.AppendLine(
                                    $"{model.m_name}, {count++}, {skinData.Flags},{skinData.AnimShift},{skinData.CSK1List.Count},{skinData.CSK2List.Count},{skinData.CSKAList.Count}");
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
                GUILayout.EndHorizontal();

                bool csk1FoldOut = m_foldOutsList[i].Item1;
                bool csk2FoldOut = m_foldOutsList[i].Item2;
                bool cskAFoldOut = m_foldOutsList[i].Item3;
                
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

                m_foldOutsList[i] = (csk1FoldOut, csk2FoldOut,cskAFoldOut);
            }
        }
        
        
    }

    public void DrawCSK1(CSK1 csk1,Color color)
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
        return val1 == val2? matchColor: diffColor;
    }
    public void DrawCSK1Compare(CSK1 oldCsk1,CSK1 newCsk1,Color matchColor,Color diffColor)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = CompareVals(oldCsk1.idxBone, newCsk1.idxBone,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.idxBone} / {newCsk1.idxBone}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCsk1.count, newCsk1.count,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.count} / {newCsk1.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk1.vertSrc, (int)newCsk1.vertSrc,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.vertSrc} / {newCsk1.vertSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk1.vertDst, (int)newCsk1.vertDst,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk1.vertDst} / {newCsk1.vertDst}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = oldColor;
    }

    
    public void DrawCSK2(CSK2 csk2,Color color)
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

    public void DrawCSK2Compare(CSK2 oldCsk2,CSK2 newCsk2,Color matchColor,Color diffColor)
    {
        Color oldColor = GUI.contentColor;
        GUI.contentColor = CompareVals(oldCsk2.idxBone[0], newCsk2.idxBone[0],matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.idxBone[0]} / {newCsk2.idxBone[0]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCsk2.idxBone[1], newCsk2.idxBone[1],matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.idxBone[1]} / {newCsk2.idxBone[1]}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCsk2.count, newCsk2.count,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.count} / {newCsk2.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk2.vertSrc, (int)newCsk2.vertSrc,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.vertSrc} / {newCsk2.vertSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCsk2.vertDst, (int)newCsk2.vertDst,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCsk2.vertDst} / {newCsk2.vertDst}", GetTableStyle(), GUILayout.Width(ColumnWidth));

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
        GUI.contentColor = CompareVals(oldCSKA.idxBone, newCSKA.idxBone,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.idxBone} / {newCSKA.idxBone}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals(oldCSKA.count, newCSKA.count,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.count} / {newCSKA.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCSKA.vertSrc, (int)newCSKA.vertSrc,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.vertSrc} / {newCSKA.vertSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = CompareVals((int)oldCSKA.weightsSrc, (int)newCSKA.weightsSrc,matchColor,diffColor);
        EditorGUILayout.LabelField($"{oldCSKA.weightsSrc} / {newCSKA.weightsSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
        GUI.contentColor = oldColor;
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
