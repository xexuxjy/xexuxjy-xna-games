using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBaker : MonoBehaviour
{
    bool done = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!done)
        {
            GameObject mergedObject = new GameObject("Merged");
            Vector3 offset = mergedObject.transform.position - transform.position;

            List<Mesh> childMeshes = new List<Mesh>();

            SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            
            for (int i = 0; i < skinnedMeshRenderers.Length; ++i)
            {
                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                childMeshes.Add(mesh);
            }

            foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>())
            {
                if (meshFilter.gameObject.GetComponent<SkinnedMeshRenderer>() == null)
                {
                    Mesh clonedMesh = CopyMesh(meshFilter.sharedMesh, meshFilter.transform,offset);
                    childMeshes.Add(clonedMesh);
                }
                else
                {
                    int ibreak = 0;
                }
            }


            List<Mesh> mergedList = MergeMesh(true, childMeshes.ToArray());
            if (mergedList.Count > 0)
            {
                Mesh mergedMesh = mergedList[0];
                MeshFilter mf = mergedObject.AddComponent<MeshFilter>();
                MeshRenderer mr = mergedObject.AddComponent<MeshRenderer>();

                mf.sharedMesh = mergedMesh;
            }

        }
        done = true;
    }

    public static Mesh CopyMesh(Mesh source,Transform t,Vector3 offset)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector2> uv2s = new List<Vector2>();
        List<int> triangles = new List<int>();

        vertices.AddRange(source.vertices);
        if (t != null)
        {
            for (int i = 0; i < vertices.Count; ++i)
            {
                //vertices[i] = t.TransformVector(vertices[i]);
                //vertices[i] += t.position;
                vertices[i] = t.TransformPoint(vertices[i]) + offset;
                //vertices[i] = t.InverseTransformPoint(vertices[i]);
            }
        }

        normals.AddRange(source.normals);
        triangles.AddRange(source.triangles);
        uvs.AddRange(source.uv);
        uv2s.AddRange(source.uv2);

        Mesh destination = new Mesh();
        destination.vertices  = vertices.ToArray();
        destination.normals = normals.ToArray();
        destination.triangles = triangles.ToArray();
        destination.uv = uvs.ToArray();
        destination.uv2 = uv2s.ToArray();


        destination.RecalculateBounds();

        return destination;



    }

    public static List<Mesh> MergeMesh(bool merge, Mesh[] meshes)
    {
        List<Mesh> mergedMeshes = new List<Mesh>();

        //Mesh mergedMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector2> uv2s = new List<Vector2>();
        List<int> triangles = new List<int>();
        //List<Color> colors = new List<Color>();


        int vertexOffset = 0;

        foreach (Mesh subMesh in meshes)
        {
            if (!merge)
            {
                vertices.Clear();
                normals.Clear();
                uvs.Clear();
                uv2s.Clear();
                triangles.Clear();
                vertexOffset = 0;
            }

            vertices.AddRange(subMesh.vertices);
            normals.AddRange(subMesh.normals);
            uvs.AddRange(subMesh.uv);

            if (subMesh.uv2 != null)
            {
                uv2s.AddRange(subMesh.uv2);
            }


            for (int i = 0; i < subMesh.triangles.Length; ++i)
            {
                triangles.Add(vertexOffset + subMesh.triangles[i]);
            }
            vertexOffset += subMesh.vertices.Length;

            if (!merge)
            {
                Mesh mergedMesh = new Mesh();
                mergedMesh.vertices = vertices.ToArray();
                mergedMesh.triangles = triangles.ToArray();
                mergedMesh.normals = normals.ToArray();
                mergedMesh.uv = uvs.ToArray();

                if (uv2s.Count > 0)
                {
                    mergedMesh.uv2 = uv2s.ToArray();
                }

                mergedMeshes.Add(mergedMesh);

            }
        }

        bool validMesh = !(vertices.Count == 0 || triangles.Count == 0 || normals.Count == 0 || uvs.Count == 0);


        if (validMesh && merge)
        {
            Mesh mergedMesh = new Mesh();
            mergedMesh.vertices = vertices.ToArray();
            mergedMesh.triangles = triangles.ToArray();
            mergedMesh.normals = normals.ToArray();
            mergedMesh.uv = uvs.ToArray();

            if (uv2s.Count > 0)
            {
                mergedMesh.uv2 = uv2s.ToArray();
            }

            mergedMeshes.Add(mergedMesh);
        }


        return mergedMeshes;

    }


}
