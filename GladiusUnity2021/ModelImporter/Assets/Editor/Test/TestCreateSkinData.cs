using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TestCreateSkinDataStub))]
public class TestCreateSkinData : Editor
{

    private SkinData m_originalSkinData;
    private SkinData m_newSkinData;
    
    
    public override void OnInspectorGUI()
    {
        TestCreateSkinDataStub stub = target as TestCreateSkinDataStub;
        
        base.OnInspectorGUI();
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
                        SkinData skinData = GCModel.CreateSkinData(renderer.sharedMesh);
                        skinDataList.Add(skinData);       
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

    
}
