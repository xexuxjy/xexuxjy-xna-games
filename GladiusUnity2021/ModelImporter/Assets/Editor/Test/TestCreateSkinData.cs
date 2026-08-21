using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TestCreateSkinDataStub))]
public class TestCreateSkinData : Editor
{
    
    private SkinData m_originalSkinData;
    private SkinData m_newSkinData;

    private bool m_csk1Foldout = false;
    private bool m_csk2Foldout = false;
    private bool m_diffData = true;
    
    public int IndentLevel = 20;
    
    public override void OnInspectorGUI()
    {
        TestCreateSkinDataStub stub = target as TestCreateSkinDataStub;
        
        base.OnInspectorGUI();

        m_diffData = EditorGUILayout.Toggle("Diff Data", m_diffData);
        
        if (GUILayout.Button("Process model"))
        {
            CommonModelData commonModel = null;

            GCModel gcModel = new GCModel("");
            // Load the skin data into a model.
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(stub.OriginalModel.bytes)))
            {
                gcModel.LoadData(binReader, null);
                commonModel = gcModel.ToCommon();
            }

            if (commonModel != null)
            {
                
                HashSet<CommonVertexInstance> cviSet = new HashSet<CommonVertexInstance>();
                HashSet<Vector3> positionSet = new HashSet<Vector3>();
                HashSet<Vector3> normalSet = new HashSet<Vector3>();
                
                string assetName = "test";
                string outputHierarchy = "";
                uint lodLevel = 0;
                string prefabOutputDirectory = "";
            
                GameObject gameObject = CommonModelProcessor.CommonModelToGameObject(outputHierarchy, lodLevel,
                    commonModel,out Dictionary<BoneNode,GameObject> boneObjectMap);

                if (gameObject != null)
                {
                    // back to skin.
                    SkinnedMeshRenderer[] skinnedRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                    List<SkinData> skinDataList = new List<SkinData>();
                    foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                    {
                        int numBones = commonModel.BoneList.Count;
                        List<Vector3> positions = new List<Vector3>();
                        List<Vector3> normals = new List<Vector3>();
                        List<BoneWeight> boneWeights = new List<BoneWeight>();
                        
                        positions.AddRange(renderer.sharedMesh.vertices);
                        normals.AddRange(renderer.sharedMesh.normals);
                        boneWeights.AddRange(renderer.sharedMesh.boneWeights);

                        
                        for (int i = 0; i < positions.Count; i++)
                        {
                            CommonVertexInstance cvi = new CommonVertexInstance();
                            cvi.Position = positions[i];
                            cvi.Normal = normals[i];
                            cvi.BoneWeight = boneWeights[i];
                            cviSet.Add(cvi);
                            positionSet.Add(positions[i]);
                            normalSet.Add(normals[i]);

                        }
                        
                        
                        
                        SkinData skinData = SkinBuilder.PrepareData(numBones,positions,normals,boneWeights);
                        skinDataList.Add(skinData);
                        // SkinData skinData = GCModel.CreateSkinData(renderer.sharedMesh);
                        // skinDataList.Add(skinData);       
                    }

                    
                    
                    
                    if (gcModel?.SKINChunk().SkinDataList.Count > 0 && skinDataList.Count > 0)
                    {
                        m_originalSkinData = gcModel.SKINChunk().SkinDataList[0];
                        m_newSkinData = skinDataList[0];
                    }

                    DestroyImmediate(gameObject);
                    
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
                    GCModel model = GCModelReader.LoadSingleModel(file, null, true);
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

        
        
        
        if (m_originalSkinData != null && m_newSkinData != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumList1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumList2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumListA", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_originalSkinData.Size.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(m_originalSkinData.NumList1.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(m_originalSkinData.NumList2.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(m_originalSkinData.NumListA.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_newSkinData.Size.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(m_newSkinData.NumList1.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(m_newSkinData.NumList2.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(m_newSkinData.NumListA.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            m_csk1Foldout = StartIndentedFoldoutHeader("CSK1", m_csk1Foldout);
            if (m_csk1Foldout)
            {

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                if (m_diffData)
                {
                    for (int i = 0; i < m_originalSkinData.CSK1List.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        DrawCSK1Compare(m_originalSkinData.CSK1List[i],m_newSkinData.CSK1List[i], Color.green,Color.yellow);
                        GUILayout.EndHorizontal();
                    }                    
                }
                else
                {
                    foreach (CSK1 csk1 in m_originalSkinData.CSK1List)
                    {
                        GUILayout.BeginHorizontal();
                        DrawCSK1(csk1, Color.green);
                        GUILayout.EndHorizontal();
                    }

                    foreach (CSK1 csk1 in m_newSkinData.CSK1List)
                    {
                        GUILayout.BeginHorizontal();
                        DrawCSK1(csk1, Color.yellow);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EndIndentedFoldoutHeader();
            
            m_csk2Foldout = StartIndentedFoldoutHeader("CSK2",m_csk2Foldout);
            if (m_csk2Foldout)
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
                    for (int i = 0; i < m_originalSkinData.CSK2List.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        DrawCSK2Compare(m_originalSkinData.CSK2List[i],m_newSkinData.CSK2List[i], Color.green,Color.yellow);
                        GUILayout.EndHorizontal();
                    }                    
                }
                else
                {
                    foreach (CSK2 csk2 in m_originalSkinData.CSK2List)
                    {
                        GUILayout.BeginHorizontal();
                        DrawCSK2(csk2, Color.green);
                        GUILayout.EndHorizontal();
                    }

                    foreach (CSK2 csk2 in m_newSkinData.CSK2List)
                    {
                        GUILayout.BeginHorizontal();
                        DrawCSK2(csk2, Color.yellow);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EndIndentedFoldoutHeader();
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
