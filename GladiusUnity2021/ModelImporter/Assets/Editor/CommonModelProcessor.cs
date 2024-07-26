using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CommonModelProcessor
{
    static Dictionary<String, Material> TempMaterialStore = new Dictionary<string, Material>();

    public static Texture GetTexture(string name)
    {
        Texture t = UnityEngine.Resources.Load<Texture>("Textures/" + name);
        if(t == null && !name.EndsWith(".tga"))
        {
            t = UnityEngine.Resources.Load<Texture>("Textures/" + name+".tga");
        }
        return t;
    }


    
    public static Material GetOrCreateMaterial(CommonModelData modelData, CommonMaterialData materialData,
        CommonMeshData commonMeshData)
    {
        Debug.Log("GetOrCreatingMaterial " + materialData.Name);
        //String materialName = sd.te
        CommonTextureData textureData1 = materialData.TextureData1;
        CommonTextureData textureData2 = materialData.TextureData2;

        Material m = null;

        try
        {
            textureData1.fullPathName = TidyTextureName(textureData1.fullPathName);
            textureData1.textureName = TidyTextureName(textureData1.textureName);

            if (textureData2 != null)
            {
                textureData2.fullPathName = TidyTextureName(textureData2.fullPathName);
                textureData2.textureName = TidyTextureName(textureData2.textureName);
            }

            //Shader shader = Shader.Find("Shader Graphs/GladiusStandard");

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");

            m = UnityEngine.Resources.Load<Material>("Materials/" + materialData.Name);
            if (m == null)
            {
                TempMaterialStore.TryGetValue(materialData.Name, out m);
            }

            if (textureData1 != null)
            {
                Texture texture1 = UnityEngine.Resources.Load<Texture>("Textures/" + textureData1.textureName);

                if (texture1 == null)
                {
                    Debug.LogWarningFormat("Can't find texture {0}", textureData1.textureName);
                }

                // force rebuild all.
                //m = null;

                if (m == null)
                {
                    m = new Material(shader);

                    m.name = materialData.Name;

                    if (texture1 != null)
                    {
                        try
                        {
                            m.SetTexture("_BaseMap", texture1);
                        }
                        catch (Exception e)
                        {
                            int ibreak = 0;
                        }
                    }

                    if (materialData.IsCubeMapReflect)
                    {
                        Cubemap cubeMap = UnityEngine.Resources.Load<Cubemap>("Textures/" + textureData2.textureName);
                        //m.SetTexture("_Cube", cubeMap);
                        m.SetTexture("_CubeMap", cubeMap);
                    }
                    else if (materialData.IsDetailMap)
                    {
                        Texture texture2 = UnityEngine.Resources.Load<Texture>("Textures/" + textureData2.textureName);

                        if (texture2 == null)
                        {
                            Debug.LogWarningFormat("Can't find texture {0}", textureData2.textureName);
                        }

                        m.SetTexture("_Detail", texture2);
                    }
                    else if (materialData.IsCutOut)
                    {
                        //shader = Shader.Find("Gladius/Cutout");
                        m.SetFloat("_Mode", 1.0f);
                    }


                    TempMaterialStore[m.name] = m;
                    AssetDatabase.CreateAsset(m, "Assets/Resources/Materials/" + m.name + ".mat");
                }
            }
        }
        catch (Exception e)
        {
            int ibreak = 0;
        }

        return m;
    }

    public static bool DoesTextureHaveAlpha(Texture texture)
    {
        bool hasAlpha = false;
        // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(texture, tmp);
        // Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;
        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;
        // Create a new readable Texture2D to copy the pixels to it
        Texture2D myTexture2D = new Texture2D(texture.width, texture.width);
        // Copy the pixels from the RenderTexture to the new Texture
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();

        Color32[] pixels = myTexture2D.GetPixels32();
        for (int i = 0; i < pixels.Length; ++i)
        {
            if (pixels[i].a < 240)
            {
                hasAlpha = true;
                break;
            }
        }


        // Reset the active RenderTexture
        RenderTexture.active = previous;
        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);

        // "myTexture2D" now has the same pixels from "texture" and it's readable.
        return hasAlpha;
    }
    
    static string TidyTextureName(string texturename)
    {
        if (texturename != null)
        {
            texturename = texturename.Replace(".png", "");
            texturename = texturename.Replace(".tga", "");
        }
        return texturename;
    }

    static string TidyMaterialName(string texturename)
    {
        if (texturename != null)
        {
            texturename = texturename.Replace(".tga", "");
            texturename = texturename.Replace(".png", "");
        }
        return texturename;
    }

        public static Mesh CombineMeshes(Mesh mesh, Mesh invertedMesh,CommonModelData commonModel,bool remapBones = true)
        {
            List<Mesh> originalMeshes = new List<Mesh>();
            originalMeshes.Add(mesh);
            originalMeshes.Add(invertedMesh);
            Dictionary<int, int> boneConversionDictionary = new Dictionary<int, int>();
            List<BoneNode> remappedBoneNodeList = new List<BoneNode>();
            List<Mesh> combinedMeshes = MergeMesh(true, originalMeshes.ToArray(), boneConversionDictionary, commonModel.BoneList, remappedBoneNodeList,remapBones);
            return combinedMeshes[0];
        }
        public static Mesh CreateInvertedMesh(Mesh mesh)
        {
            Vector3[] normals = mesh.normals;
            Vector3[] invertedNormals = new Vector3[normals.Length];
            for (int i = 0; i < invertedNormals.Length; i++)
            {
                invertedNormals[i] = -normals[i];
            }
            return new Mesh
            {
                vertices = mesh.vertices,
                uv = mesh.uv,
                uv2 = mesh.uv2,
                colors = mesh.colors,
                boneWeights = mesh.boneWeights,
                bindposes = mesh.bindposes,
                normals = invertedNormals,
                triangles = mesh.triangles.Reverse().ToArray()
            };
        }
        // merge basic mesh properties.
        // assume that submesh list is filtered my material
        public static List<Mesh> MergeMesh(bool merge, Mesh[] meshes, Dictionary<int, int> boneConversionDictionary, List<BoneNode> originalBoneNodeList, List<BoneNode> remappedBoneNodeList,bool remapBones=true)
        {
            List<Mesh> mergedMeshes = new List<Mesh>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector2> uv2s = new List<Vector2>();
            List<int> triangles = new List<int>();
            List<BoneWeight> boneWeights = new List<BoneWeight>();
            List<Color> colors = new List<Color>();


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
                    boneWeights.Clear();
                    colors.Clear();
                    vertexOffset = 0;
                }
                vertices.AddRange(subMesh.vertices);
                normals.AddRange(subMesh.normals);
                uvs.AddRange(subMesh.uv);
                if (subMesh.uv2 != null)
                {
                    uv2s.AddRange(subMesh.uv2);
                }
                if (subMesh.colors != null)
                {
                    colors.AddRange(subMesh.colors);
                }
                if (subMesh.boneWeights != null)
                {
                    //
                    //foreach(BoneWeight bw in subMesh.sharedMesh.boneWeights)
                    BoneWeight[] bonesCopy = new BoneWeight[subMesh.boneWeights.Length];
                    for (int i = 0; i < subMesh.boneWeights.Length; ++i)
                    {
                        BoneWeight bw = subMesh.boneWeights[i];
                        if (remapBones)
                        {
                        bw.boneIndex0 = RemapBone(bw.boneIndex0, bw.weight0, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex1 = RemapBone(bw.boneIndex1, bw.weight1, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex2 = RemapBone(bw.boneIndex2, bw.weight2, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex3 = RemapBone(bw.boneIndex3, bw.weight3, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        }
                        //subMesh.sharedMesh.boneWeights[i] = bw;
                        bonesCopy[i] = bw;

                    }

                    boneWeights.AddRange(bonesCopy);
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
                    mergedMesh.boneWeights = boneWeights.ToArray();
                    if (uv2s.Count > 0)
                    {
                        mergedMesh.uv2 = uv2s.ToArray();
                    }
                    if (colors.Count > 0)
                    {
                        mergedMesh.colors = colors.ToArray();
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
            mergedMesh.boneWeights = boneWeights.ToArray();

                if (uv2s.Count > 0)
                {
                    mergedMesh.uv2 = uv2s.ToArray();
                }
                if (colors.Count > 0)
                {
                    mergedMesh.colors = colors.ToArray();
                }

                mergedMeshes.Add(mergedMesh);
            }
            return mergedMeshes;

        }

        public static int RemapBone(int boneId, float weight, Dictionary<int, int> boneMap, List<BoneNode> originalBoneNodeList, List<BoneNode> remappedBoneNodeList)
        {
            int newVal = boneId;
            if (weight > 0f)
            {
                if (!boneMap.ContainsKey(boneId))
                {
                    boneMap[boneId] = remappedBoneNodeList.Count;
                    remappedBoneNodeList.Add(originalBoneNodeList[boneId]);
                }
                newVal = boneMap[boneId];
            }
            return newVal;
        }

        public static Dictionary<CommonMaterialData, List<CommonMeshData>> SplitByMaterial(CommonModelData model)
        {
            Dictionary<CommonMaterialData, List<CommonMeshData>> result = new Dictionary<CommonMaterialData, List<CommonMeshData>>();
            foreach (CommonMeshData mesh in model.CommonMeshData)
            {
                CommonMaterialData materialData = model.CommonMaterials[mesh.MaterialId];
                List<CommonMeshData> listResult = null;
                if (!result.TryGetValue(materialData, out listResult))
                {
                    listResult = new List<CommonMeshData>();
                    result[materialData] = listResult;
                }

                listResult.Add(mesh);
            }
            return result;
        }

    public static GameObject CreateSubMesh(CommonModelData commonModel, CommonMeshData submesh, string name)
        {
            GameObject submeshObject = new GameObject();
            string fullName = commonModel.Name + name;

            submeshObject.name = fullName;
            if(submesh.Index == 9)
            {
                int ibreak = 0;
            }
            MeshFilter mf = submeshObject.AddComponent<MeshFilter>();
            Renderer renderer = null;
            if (commonModel.Skinned)
            {
                renderer = submeshObject.AddComponent<SkinnedMeshRenderer>();
            }
            else
            {
                renderer = submeshObject.AddComponent<MeshRenderer>();
            }
            

            CommonMaterialData commonMaterial = commonModel.CommonMaterials[submesh.MaterialId];
            Material m = CommonModelProcessor.GetOrCreateMaterial(commonModel, commonMaterial, submesh);
            if (m != null)
            {
                renderer.material = m;
            }

            //dlh.BuildCommonVertexData();
            Mesh mesh = new Mesh();
            mesh.name = submesh.Name;
            mf.sharedMesh= mesh;

            Vector3[] tempPos = new Vector3[submesh.Vertices.Count];

            mesh.vertices = new Vector3[submesh.Vertices.Count];
            for (int i = 0; i < tempPos.Length; ++i)
            {
                Vector3 pos = commonModel.AllVertices[submesh.Vertices[i]].Position;
                tempPos[i] = pos;
            }

            Vector3[] tempNormal = new Vector3[submesh.Vertices.Count];
            //mesh.normals = new Vector3[submesh.Vertices.Count];
            for (int i = 0; i < tempNormal.Length; ++i)
            {
                tempNormal[i] = commonModel.AllVertices[submesh.Vertices[i]].Normal;
            }

            Vector2[] tempUV = new Vector2[submesh.Vertices.Count];
            float maxy = 1.0f;
            for (int i = 0; i < tempUV.Length; ++i)
            {
                Vector2 uv = commonModel.AllVertices[submesh.Vertices[i]].UV;
                tempUV[i] = new Vector2(uv.x, 1.0f - uv.y);
                maxy = Math.Max(maxy, uv.y);
            }
            for (int i = 0; i < tempUV.Length; ++i)
            {
                Vector2 uv = commonModel.AllVertices[submesh.Vertices[i]].UV;
                tempUV[i] = new Vector2(uv.x, maxy - uv.y);
            }
            Vector2[] tempUV2 = null;
            if (commonModel.HasUV2)
            {
                tempUV2 = new Vector2[submesh.Vertices.Count];
                maxy = 1.0f;
                for (int i = 0; i < tempUV.Length; ++i)
                {
                    Vector2 uv = commonModel.AllVertices[submesh.Vertices[i]].UV2;
                    tempUV2[i] = new Vector2(uv.x, 1.0f - uv.y);
                    maxy = Math.Max(maxy, uv.y);
                }
                for (int i = 0; i < tempUV.Length; ++i)
                {
                    Vector2 uv = commonModel.AllVertices[submesh.Vertices[i]].UV2;
                    tempUV2[i] = new Vector2(uv.x, maxy - uv.y);
                }
            }

            int[] tempTriangles = new int[submesh.Indices.Count];
            for (int i = 0; i < tempTriangles.Length; i+=3)
            {
                tempTriangles[i] = submesh.Indices[i];
                tempTriangles[i + 1] = submesh.Indices[i + 1];
                tempTriangles[i + 2] = submesh.Indices[i + 2];
            }

            mesh.vertices = tempPos;
            mesh.normals = tempNormal;
            mesh.uv = tempUV;
            mesh.triangles = tempTriangles;

            if (commonModel.HasUV2)
            {
                mesh.uv2 = tempUV2;
            }
            if (commonModel.HasColor)
            {
                Color[] tempColors = new Color[submesh.Vertices.Count];
                for (int i = 0; i < tempPos.Length; ++i)
                {
                    Color color = commonModel.AllVertices[submesh.Vertices[i]].DiffuseColor;
                    tempColors[i] = color;
                }
                mesh.colors = tempColors;
            }
            if (commonModel.Skinned)
            {
                BoneWeight[] tempBoneWeights = new BoneWeight[mesh.vertices.Length];
                for (int i = 0; i < tempBoneWeights.Length; ++i)
                {
                    BoneWeight bw = new BoneWeight();
                    var vertex = commonModel.AllVertices[submesh.Vertices[i]];
                    vertex.TranslatedBoneIndices = new short[vertex.BoneIndices.Length];
                    for(int j=0; j < vertex.BoneIndices.Length;++j)
                    {
                        int originalBoneId = -1;
                        originalBoneId = commonModel.XBoxModel.AdjustBone(vertex.BoneIndices[j], submesh.Index);
                        vertex.TranslatedBoneIndices[j] = (short)originalBoneId;
                    }
                    int numWeights = vertex.ActiveWeights();
                    float sum = 0.0f;
                    if (numWeights > 0)
                    {
                        bw.weight0 = vertex.Weight(0);
                        bw.boneIndex0 = vertex.TranslatedBoneIndices[0];
                        sum += vertex.Weight(0);
                    }
                    if (numWeights > 1)
                    {
                        bw.weight1 = vertex.Weight(1);
                        bw.boneIndex1 = vertex.TranslatedBoneIndices[1];
                        sum += vertex.Weight(1);
                    }
                    if (numWeights > 2)
                    {
                        bw.weight2 = vertex.Weight(2);
                        bw.boneIndex2 = vertex.TranslatedBoneIndices[2];
                        sum += vertex.Weight(2);
                    }
                    if (numWeights > 3)
                    {
                        bw.weight3 = vertex.Weight(3);
                        bw.boneIndex3 = vertex.TranslatedBoneIndices[3];
                        sum += vertex.Weight(3);
                    }
                    tempBoneWeights[i] = bw;
                    if (sum > 1.01f)
                    {
                        int ibreak = 0;
                    }
                }
                mesh.boneWeights = tempBoneWeights;
            }

            if (commonMaterial.IsTwoSided)
            {
                Mesh invertedMesh = CreateInvertedMesh(mesh);
                Mesh combinedMesh = CombineMeshes(mesh, invertedMesh, commonModel,false);
                mesh = combinedMesh;
            }

            mesh.name = fullName;
            mf.sharedMesh = mesh;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            //AssetDatabase.CreateAsset(mesh, "Assets/Resources/Meshes/" + mesh.name);

            //Debug.LogFormat("Mesh [{0}][[{1}][{2}].", mesh.vertices.Length, mesh.normals.Length, mesh.uv.Length);

            return submeshObject;
        }


}