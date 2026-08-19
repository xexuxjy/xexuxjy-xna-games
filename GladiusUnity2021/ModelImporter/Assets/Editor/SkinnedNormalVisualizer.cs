using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkinnedMeshRenderer))]
public class SkinnedNormalsVisualizer : Editor
{
    private const string EDITOR_PREF_KEY = "_normals_length";
    private Mesh mesh;
    private SkinnedMeshRenderer smr;
    private Vector3[] verts;
    private Vector3[] normals;
    private float normalsLength = 1f;

    private void OnEnable()
    {
        smr = target as SkinnedMeshRenderer;
        if (smr != null)
        {
            mesh = smr.sharedMesh;
        }

        normalsLength = EditorPrefs.GetFloat(EDITOR_PREF_KEY);
    }

    private void OnSceneGUI()
    {
        if (mesh == null)
        {
            return;
        }

        //Handles.matrix = smr.transform.localToWorldMatrix;
        Handles.color = Color.yellow;
        verts = mesh.vertices;
        normals = mesh.normals;
        int len = mesh.vertexCount;


        for (int i = 0; i < len; i++)
        {
            Vector3 start = smr.transform.TransformPoint(verts[i]);
            Vector3 dir = smr.transform.TransformDirection(normals[i]);
            dir = normals[i];
            //dir *= -1.0f;
            Handles.DrawLine(start, start + dir * normalsLength);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        normalsLength = EditorGUILayout.FloatField("Normals length", normalsLength);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetFloat(EDITOR_PREF_KEY, normalsLength);
        }
    }
}