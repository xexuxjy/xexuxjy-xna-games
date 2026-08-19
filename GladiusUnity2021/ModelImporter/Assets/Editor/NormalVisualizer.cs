using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshFilter))]
public class NormalsVisualizer : Editor
{
    private const string EDITOR_PREF_KEY = "_normals_length";
    private Mesh mesh;
    private MeshFilter mf;
    private Vector3[] verts;
    private Vector3[] normals;
    private float normalsLength = 1f;

    private void OnEnable()
    {
        this.mf = target as MeshFilter;
        if (this.mf != null)
        {
            mesh = this.mf.sharedMesh;
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


        if (len == mesh.normals.Length && len == mesh.vertices.Length)
        {

            for (int i = 0; i < len; i++)
            {
                Vector3 start = mf.transform.TransformPoint(verts[i]);
                Vector3 dir = mf.transform.TransformDirection(normals[i]);

                Handles.DrawLine(start, start + dir * normalsLength);
            }
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