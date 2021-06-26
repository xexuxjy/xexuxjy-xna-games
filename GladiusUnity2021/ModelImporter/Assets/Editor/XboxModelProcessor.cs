// Contains an example of Merging Unity Skinned Meshes - bone lookup map and so on. - MAN

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    //http://answers.unity3d.com/questions/600046/importing-custom-assets.html
    public class XboxModelProcessor : AssetPostprocessor
    {
        public static bool MERGE_MESH = true;
        public const string m_extension = ".mdl";
        public const string m_newExtension = ".asset";
        public const string OriginalModelDirectory = "XboxModels";

        public static bool HasExtension(string asset)
        {
            return asset.EndsWith(m_extension);
        }

        public static string ConvertToInternalPath(string asset)
        {
            string left = asset.Substring(0, asset.Length - m_extension.Length);
            return left + m_newExtension;
        }

        // This is called always when importing something
        static void OnPostprocessAllAssets
            (
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths
            )
        {
            Debug.Log("In xbox processor");
            foreach (string asset in importedAssets)
            {
                Debug.Log(asset);
                // This is our detection of file - by extension
                //if (HasExtension(asset))
                if (asset.ToUpper().Contains("Assets/XBoxModels/".ToUpper()))
                {
                    ImportMyAsset(asset);
                }
            }
        }

        static void ImportMyAsset(string asset)
        {
            ImportXBoxModel(asset);
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
            Material m = GetOrCreateMaterial(commonModel, commonMaterial, submesh);
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


        public static Material GetOrCreateMaterial(CommonModelData modelData, CommonMaterialData materialData, CommonMeshData commonMeshData)
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

                Shader shader = Shader.Find("Gladius/Standard");

                m = UnityEngine.Resources.Load<Material>("Materials/" + materialData.Name);
                if (m == null)
                {
                    TempMaterialStore.TryGetValue(materialData.Name, out m);
                }
                if (textureData1 != null)
                {
                    Texture texture1 = GetTexture(textureData1.textureName);

                    if (texture1 == null)
                    {
                        Debug.LogWarningFormat("Can't find texture {0}", textureData1.textureName);
                    }

                    // force rebuild all.
                    //m = null;

                    if (m == null)
                    {
                        if (commonMeshData.IsTransparent || textureData1.textureName.Contains(".cc"))
s                        {
                            shader = Shader.Find("Gladius/Transparent");
                        }
                        else if (materialData.IsCubeMapReflect)
                        {
                            shader = Shader.Find("Gladius/CubemapReflection");
                        }
                        else if (materialData.IsDetailMap)
                        {
                            shader = Shader.Find("Gladius/DetailMap");

                        }
                        else if (materialData.IsCutOut)
                        {
                            //shader = Shader.Find("Gladius/Cutout");
                            shader = Shader.Find("Standard");
                        }
                        else if (materialData.IsTwoSided)
                        {
                            // done this by creating extra inverted mesh
                            //shader = Shader.Find("Gladius/TwoSided");
                            //shader = Shader.Find("Standard");
                        }
                        else if (materialData.IsColouredVertex)
                        {
                            //shader = Shader.Find("Gladius/VertexColorTexture");
                        }


                        m = new Material(shader);
                        m.name = materialData.Name;

                        if (texture1 != null)
                        {
                            try
                            {
                                m.SetTexture("_MainTex", texture1);
                            }
                            catch (Exception e)
                            {
                                int ibreak = 0;
                            }
                        }

                        if (materialData.IsCubeMapReflect)
                        {
                            Cubemap cubeMap = UnityEngine.Resources.Load<Cubemap>("Textures/" + textureData2.textureName);
                            m.SetTexture("_Cube", cubeMap);
                        }
                        else if (materialData.IsDetailMap)
                        {
                            Texture texture2 = GetTexture(textureData2.textureName);

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


        // Imports my asset from the file
        static void ImportXBoxModel(string assetName)
        {
            Debug.Log("XboxModelProcessor importing : " + assetName);


            try
            {

                //Debug.LogFormat("[{0}] [{1}] [{2}]", filename, assetPath, assetSubDirectories);

                TextAsset assetData = AssetDatabase.LoadAssetAtPath<TextAsset>(assetName);
                if (assetData != null)
                {
                    BinaryReader binReader = new BinaryReader(new MemoryStream(assetData.bytes));


                    String assetPath = assetName.Substring(0, assetName.LastIndexOf('/'));
                    String filename = assetName.Substring(assetName.LastIndexOf('/') + 1);
                    int startIndex = assetName.IndexOf(OriginalModelDirectory) + OriginalModelDirectory.Length;
                    int endIndex = assetName.LastIndexOf('/');
                    String assetSubDirectories = assetName.Substring(startIndex, endIndex - startIndex);
                    string adjustedFilename = filename.Replace(".bytes", "");

                    XboxModel model = new XboxModel("");


                    model.m_name = adjustedFilename;

                    // deal with empty files

                    binReader.BaseStream.Position = 0;
                    model.LoadData(binReader);
                    model.GetIndices(null);


                    CommonModelData commonModel = model.ToCommon();

                    commonModel.Name = adjustedFilename;
                    int lodLevel = -1; //model.m_selsInfo.GetBestLodData();
                    ProcessModel(assetName, lodLevel, commonModel);


                    Debug.Log("Best lod level is : " + lodLevel);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed [{0}] [{1}]", assetName, e.StackTrace);
            }
        }


        public static string ProcessModel(string fullAssetPath, int lodLevel, CommonModelData commonModel)
        {


            //lodLevel = model.GetMeshLodLevel("set_LOD1");
            Debug.Log("Best lod level is : " + lodLevel);

            List<CommonMeshData> filteredList = new List<CommonMeshData>();

            foreach (CommonMeshData mesh in commonModel.CommonMeshData)
            {
                if (mesh.LodLevel != 0 && (mesh.LodLevel & lodLevel) == 0)
                {
                    continue;
                }
                filteredList.Add(mesh);
            }

            commonModel.CommonMeshData = filteredList;

            Dictionary<CommonMaterialData, List<CommonMeshData>> materialMeshLists = SplitByMaterial(commonModel);

            GameObject splitPrefab = new GameObject(commonModel.Name + "-SplitPrefab");

            GameObject combinedPrefab = new GameObject(commonModel.Name);
            GameObject gladiusToUnity = new GameObject("GladiusToUnity");
            gladiusToUnity.transform.SetParent(combinedPrefab.transform, false);


            // Build structure if mapped.
            var boneObjectMap = new Dictionary<BoneNode, GameObject>();
            GameObject rootGO = null;

            if (commonModel.Skinned)
            {
                // create transform tree?
                foreach (BoneNode bn in commonModel.BoneList)
                {
                    GameObject go = new GameObject(bn.UniqueName);
                    boneObjectMap[bn] = go;
                    Quaternion q = bn.rotation;

                    if (bn.Index != bn.ParentIndex)
                    {
                        GameObject parentGo = boneObjectMap[bn.parent];
                        go.transform.SetParent(parentGo.transform, false);
                    }

                    go.transform.localPosition = bn.offset;
                    go.transform.localRotation = bn.rotation;

                    bool useSpheres = false;
                    if (useSpheres)
                    {
                        //sphere.GetComponent<Renderer>().material.color = bn.name.Contains("_L") ? Color.green : bn.name.Contains("_R") ? Color.red : Color.black;
                        GameObject dummy = bn.name.Contains("_L") ? GameObject.CreatePrimitive(PrimitiveType.Sphere) : bn.name.Contains("_R") ? GameObject.CreatePrimitive(PrimitiveType.Cube) : null;
                        if (dummy != null)
                        {
                            dummy.transform.SetParent(go.transform, false);
                            dummy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            dummy.transform.localPosition = Vector3.zero;
                        }
                        //sphere.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));
                        //sphere.GetComponent<Renderer>().material.color = bn.name.Contains("_L") ? Color.green : bn.name.Contains("_R") ? Color.red : Color.black;
                    }

                }

                rootGO = boneObjectMap[commonModel.BoneList[0]];
                rootGO.transform.SetParent(gladiusToUnity.transform, false);


                String noExtensionName = commonModel.Name.Replace(".mdl", "");

            }

            int everyMeshCounter = 0;
            int everyMeshLowBound = -1;
            int everyMeshHighBound = everyMeshLowBound + 300;

            int counter = 0;
            if (MERGE_MESH)
            {

                foreach (CommonMaterialData commonMaterialData in materialMeshLists.Keys)
                {
                    List<CommonMeshData> commonMeshDataList = materialMeshLists[commonMaterialData];
                    //Process(assetName, adjustedFilename, commonModel, model, commonMeshDataList, MERGE_MESH, splitPrefab, gladiusToUnity, counter++, boneObjectMap);
                    Process(commonModel, commonMeshDataList, MERGE_MESH, splitPrefab, gladiusToUnity, counter++, boneObjectMap);
                }
            }
            else
            {

                foreach (CommonMeshData mesh in commonModel.CommonMeshData)
                {
                    List<CommonMeshData> commonMeshDataList = new List<CommonMeshData>();
                    commonMeshDataList.Add(mesh);
                    //Process(assetName, adjustedFilename, commonModel, model, commonMeshDataList, MERGE_MESH, splitPrefab, gladiusToUnity, counter++, boneObjectMap);
                    Process(commonModel, commonMeshDataList, MERGE_MESH, splitPrefab, gladiusToUnity, counter++, boneObjectMap);
                }
            }


            //gladiusToUnity.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);


            String outputDirName = "Resources/XboxModelPrefabs/" + commonModel.AssetSubDirectories + "/";
            String fullOuputDirName = Application.dataPath + "/" + outputDirName;
            if (!Directory.Exists(fullOuputDirName))
            {
                Directory.CreateDirectory(fullOuputDirName);
            }

            String prefabName = "Assets/" + outputDirName + commonModel.Name + ".prefab";


            UnityEngine.Object existingPrefab = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabName);
            if (existingPrefab == null)
            {
                PrefabUtility.CreatePrefab(prefabName, combinedPrefab, ReplacePrefabOptions.ReplaceNameBased);

            }
            else
            {
                PrefabUtility.ReplacePrefab(combinedPrefab, existingPrefab);
            }



            if (splitPrefab != null)
            {
                UnityEngine.Object.DestroyImmediate(splitPrefab);
            }

            if (combinedPrefab != null)
            {
                UnityEngine.Object.DestroyImmediate(combinedPrefab);
            }

            return outputDirName + commonModel.Name;

        }




        //public static void Process(String assetName, String adjustedFilename, CommonModelData commonModel, XboxModelRW xboxModel, List<CommonMeshData> meshList, bool merge, GameObject splitPrefab, GameObject combinedPrefab, int index, Dictionary<BoneNode, GameObject> boneObjectMap)
        public static void Process(CommonModelData commonModel, List<CommonMeshData> meshList, bool merge, GameObject splitPrefab, GameObject combinedPrefab, int index, Dictionary<BoneNode, GameObject> boneObjectMap)
        {
            int count = 0;

            GameObject splitParent = new GameObject();
            splitParent.transform.SetParent(splitPrefab.transform, false);

            foreach (CommonMeshData mesh in meshList)
            {
                GameObject submesh = CreateSubMesh(commonModel, mesh, "Submesh_" + index + "_" + (count++));
                submesh.transform.SetParent(splitParent.transform, false);
            }

            MeshFilter[] meshFilters = splitParent.GetComponentsInChildren<MeshFilter>();
            Mesh[] meshes = new Mesh[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; ++i)
            {
                meshes[i] = meshFilters[i].sharedMesh;
            }


            SkinnedMeshRenderer[] splitSkinnedRenderers = splitParent.GetComponentsInChildren<SkinnedMeshRenderer>();
            Dictionary<int, int> boneConversionDictionary = new Dictionary<int, int>();
            List<BoneNode> remappedBoneNodeList = new List<BoneNode>();

            if (index == 9)
            {
                int ibreak = 0;
            }
            // always merge here as the splits handled furhter up..
            List<Mesh> combinedMeshes = MergeMesh(merge, meshes, boneConversionDictionary, commonModel.BoneList, remappedBoneNodeList);

            CommonMaterialData commonMaterialData = commonModel.CommonMaterials[meshList.First().MaterialId];

            foreach (Mesh combinedMesh in combinedMeshes)
            {
                GameObject combinedParent = new GameObject("Submesh-" + index + "-" + commonMaterialData.TextureData1.textureName);
                combinedParent.transform.SetParent(combinedPrefab.transform, false);
                MeshFilter combinedParentMeshFilter = combinedParent.AddComponent<MeshFilter>();

                Renderer renderer = null;
                if (commonModel.Skinned)
                {
                    renderer = combinedParent.AddComponent<SkinnedMeshRenderer>();
                }
                else
                {
                    renderer = combinedParent.AddComponent<MeshRenderer>();
                }

                Material m = GetOrCreateMaterial(commonModel, commonMaterialData, meshList[0]);
                if (m != null)
                {
                    renderer.sharedMaterial = m;
                }


                combinedMesh.RecalculateBounds();
                //modelInfo.NumVertices += combinedMesh.vertexCount;
                //modelInfo.NumIndices += combinedMesh.triangles.Length;
                //modelInfo.Bounds.Encapsulate(combinedMesh.bounds);

                combinedParentMeshFilter.sharedMesh = combinedMesh;
                combinedParentMeshFilter.sharedMesh.name = commonModel.Name + "_" + index;

                if (commonModel.Skinned)
                {
                    SkinnedMeshRenderer smr = (SkinnedMeshRenderer)renderer;


                    smr.sharedMesh = combinedParentMeshFilter.sharedMesh;
                    smr.rootBone = boneObjectMap[commonModel.BoneList[0]].transform;

                    Transform[] bonesCopy = new Transform[remappedBoneNodeList.Count];

                    Matrix4x4[] bindPoses = new Matrix4x4[remappedBoneNodeList.Count];

                    for (int i = 0; i < bindPoses.Length; ++i)
                    {
                        GameObject go = boneObjectMap[remappedBoneNodeList[i]];
                        bonesCopy[i] = go.transform;
                        bindPoses[i] = go.transform.worldToLocalMatrix;
                    }

                    smr.bones = bonesCopy;
                    combinedParentMeshFilter.sharedMesh.bindposes = bindPoses;
                    smr.sharedMesh.RecalculateBounds();
                    smr.receiveShadows = true;
                    smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

                }
                AssetDatabase.CreateAsset(combinedParentMeshFilter.sharedMesh, "Assets/Resources/Meshes/" + combinedParentMeshFilter.sharedMesh.name + ".mesh");
            }
        }

    }

}
