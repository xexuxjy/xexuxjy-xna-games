using UnityEngine;
using System.Collections;

public class SimpleQuad : MonoBehaviour
{
    private Mesh _m1;

    void Start()
    {
        DrawTexture();
    }

    // Create a quad mesh
    public Mesh CreateMesh()
    {

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3( 1, 1,  0),
            new Vector3( 1, -1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(-1, -1, 0),
        };

        Vector2[] uv = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    // Create a mesh and bind the 'msn' texture to it
    public void DrawTexture()
    {
        //// Create object
        //_m1 = CreateMesh();
        //var item = (GameObject)new GameObject(
        // "HelloWorld",
        // typeof(MeshRenderer), // Required to render
        // typeof(MeshFilter)    // Required to have a mesh
        //);
        //item.GetComponent().mesh = _m1;

        //// Set texture
        //var tex = (Texture)Resources.Load("msn");
        //item.renderer.material.mainTexture = tex;

        //// Set shader for this sprite; unlit supporting transparency
        //// If we dont do this the sprite seems 'dark' when drawn. 
        //var shader = Shader.Find("Unlit/Transparent");
        //item.renderer.material.shader = shader;

        //// Set position
        //item.transform.position = new Vector3(1, 1, 1);
    }

    void Update()
    {
    }
}