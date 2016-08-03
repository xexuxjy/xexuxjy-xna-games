// Contains an example of Merging Unity Skinned Meshes - bone lookup map and so on. - MAN

using Newtonsoft.Json;
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
        public const string m_extension = ".mdl";
        public const string m_newExtension = ".asset";

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




        public static GameObject CreateSubMesh(CommonModelData model, CommonMeshData submesh)
        {
            GameObject submeshObject = new GameObject();

            MeshFilter mf = submeshObject.AddComponent<MeshFilter>();
            Renderer renderer = null;
            if (model.Skinned)
            {
                renderer = submeshObject.AddComponent<SkinnedMeshRenderer>();
            }
            else
            {
                renderer = submeshObject.AddComponent<MeshRenderer>();
            }
            

            CommonMaterialData commonMaterial = model.CommonMaterials[submesh.MaterialId];
            Material m = GetOrCreateMaterial(model, commonMaterial);
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
                Vector3 pos = model.VertexDataLists[0].VertexData[submesh.Vertices[i]].Position;
                tempPos[i] = pos;
            }

            Vector3[] tempNormal = new Vector3[submesh.Vertices.Count];
            //mesh.normals = new Vector3[submesh.Vertices.Count];
            for (int i = 0; i < tempNormal.Length; ++i)
            {
                tempNormal[i] = model.VertexDataLists[0].VertexData[submesh.Vertices[i]].Normal;
            }

            Vector2[] tempUV = new Vector2[submesh.Vertices.Count];
            for (int i = 0; i < tempUV.Length; ++i)
            {
                Vector2 uv = model.VertexDataLists[0].VertexData[submesh.Vertices[i]].UV;
                tempUV[i] = new Vector2(uv.x, 1.0f - uv.y);
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

            if (model.Skinned)
            {
                BoneWeight[] tempBoneWeights = new BoneWeight[mesh.vertices.Length];
                for (int i = 0; i < tempBoneWeights.Length; ++i)
                {
                    BoneWeight bw = new BoneWeight();
                    var vertex = model.VertexDataLists[0].VertexData[submesh.Vertices[i]];
                    int numWeights = vertex.ActiveWeights();
                    if (numWeights > 0)
                    {
                        bw.weight0 = vertex.Weight(0);
                        bw.boneIndex0 = vertex.TranslatedBoneIndices[0];
                    }
                    if (numWeights > 1)
                    {
                        bw.weight1 = vertex.Weight(1);
                        bw.boneIndex1 = vertex.TranslatedBoneIndices[1];
                    }
                    if (numWeights > 2)
                    {
                        bw.weight2 = vertex.Weight(2);
                        bw.boneIndex2 = vertex.TranslatedBoneIndices[2];
                    }
                    if (numWeights > 3)
                    {
                        bw.weight3 = vertex.Weight(3);
                        bw.boneIndex3 = vertex.TranslatedBoneIndices[3];
                    }
                    tempBoneWeights[i] = bw;
                }
                mesh.boneWeights = tempBoneWeights;
            }


            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            //AssetDatabase.CreateAsset(mesh, "Assets/Resources/Meshes/" + mesh.name);

            //Debug.LogFormat("Mesh [{0}][[{1}][{2}].", mesh.vertices.Length, mesh.normals.Length, mesh.uv.Length);

            return submeshObject;
        }

        // merge basic mesh properties.
        // assume that submesh list is filtered my material
        public static Mesh MergeMesh(MeshFilter[] subMeshList,Dictionary<int,int> boneConversionDictionary,List<BoneNode> originalBoneNodeList, List<BoneNode> remappedBoneNodeList)
        {
            Mesh mergedMesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();
            List<BoneWeight> boneWeights = new List<BoneWeight>();



            int vertexOffset = 0;

            foreach (MeshFilter subMesh in subMeshList)
            {
                vertices.AddRange(subMesh.sharedMesh.vertices);
                normals.AddRange(subMesh.sharedMesh.normals);
                uvs.AddRange(subMesh.sharedMesh.uv);
                if (subMesh.sharedMesh.boneWeights != null)
                {
                    //
                    //foreach(BoneWeight bw in subMesh.sharedMesh.boneWeights)
                    BoneWeight[] bonesCopy = new BoneWeight[subMesh.sharedMesh.boneWeights.Length];
                    for (int i = 0; i < subMesh.sharedMesh.boneWeights.Length; ++i)
                    {
                        BoneWeight bw = subMesh.sharedMesh.boneWeights[i];
                        bw.boneIndex0 = RemapBone(bw.boneIndex0, bw.weight0, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex1 = RemapBone(bw.boneIndex1, bw.weight1, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex2 = RemapBone(bw.boneIndex2, bw.weight2, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex3 = RemapBone(bw.boneIndex3, bw.weight3, boneConversionDictionary, originalBoneNodeList, remappedBoneNodeList);
                        //subMesh.sharedMesh.boneWeights[i] = bw;
                        bonesCopy[i] = bw;

                    }

                    boneWeights.AddRange(bonesCopy);
                }

                for (int i = 0; i < subMesh.sharedMesh.triangles.Length; ++i)
                {
                    triangles.Add(vertexOffset + subMesh.sharedMesh.triangles[i]);
                }
                vertexOffset += subMesh.sharedMesh.vertices.Length;
            }

            mergedMesh.vertices = vertices.ToArray();
            mergedMesh.triangles = triangles.ToArray();
            mergedMesh.normals = normals.ToArray();
            mergedMesh.uv = uvs.ToArray();
            mergedMesh.boneWeights = boneWeights.ToArray();


            return mergedMesh;

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
            foreach (CommonMeshData dlh in model.CommonMeshData)
            {
                if (dlh.MaterialId >= 0 && dlh.MaterialId < model.CommonMaterials.Count)
                {
                    CommonMaterialData cmd = model.CommonMaterials[dlh.MaterialId];
                    ;// model.gedlh.MaterialId
                    List<CommonMeshData> listResult = null;
                    if (!result.TryGetValue(cmd, out listResult))
                    {
                        listResult = new List<CommonMeshData>();
                        result[cmd] = listResult;
                    }

                    listResult.Add(dlh);
                }
            }
            return result;
        }

        public static Material GetOrCreateMaterial(CommonModelData model, CommonMaterialData sd)
        {
            //String materialName = sd.te
            CommonTextureData textureData1 = sd.diffuseTextureData;
            CommonTextureData textureData2 = sd.specularTextureData;
            Material m = null;
            if (textureData1 != null)
            {

                String tex2Name = textureData2 != null ? ("-" + textureData2.textureName) : "";
                String materialname = textureData1.textureName + tex2Name;
                materialname = materialname.Replace(".tga", "");
                materialname = materialname.Replace(".png", "");

                m = Resources.Load<Material>("Materials/" + materialname);
                if (m == null)
                {
                    m = new Material(Shader.Find("Standard"));
                    m.name = materialname;
                    Texture texture = Resources.Load<Texture>("Textures/" + textureData1.textureName);
                    //Debug.LogFormat("Found texture [{0}] as [{1}]", textureData1.textureName, texture != null ? "" + texture.GetHashCode() : "Null");
                    //m.SetTexture("Diffuse", texture);
                    m.mainTexture = texture;
                    // update for transparent
                    if (textureData1.textureName.Contains(".cc"))
                    {
                        m.SetFloat("_Mode", 3.0f);
                    }

                    AssetDatabase.CreateAsset(m, "Assets/Resources/Materials/" + m.name + ".mat");
                }
            }

            return m;
        }


        // Imports my asset from the file
        static void ImportMyAsset(string asset)
        {
            String rootPath = "Assets/XBoxModels/";
            Debug.Log("XboxModelProcessor importing : " + asset);
            String assetPath = asset.Substring(0, asset.LastIndexOf('/'));
            String filename = asset.Substring(asset.LastIndexOf('/') + 1);
            int startIndex = asset.IndexOf(rootPath) + rootPath.Length;
            int endIndex = asset.LastIndexOf('/');
            String assetSubDirectories = "";
            if (startIndex < endIndex)
            {
                assetSubDirectories = asset.Substring(startIndex, endIndex - startIndex);
            }
            try
            {

                Debug.LogFormat("[{0}] [{1}] [{2}]", filename, assetPath, assetSubDirectories);
                XboxModel model = new XboxModel("");
                TextAsset assetData = AssetDatabase.LoadAssetAtPath<TextAsset>(asset);

                model.LoadData(new BinaryReader(new MemoryStream(assetData.bytes)));

                string adjustedFilename = filename.Replace(".bytes", "");


                JSONModelData jsonModelData = null;

                String jsonFile = "Assets/ModelJsonData/" + adjustedFilename.Replace(".mdl", ".json");
                if (File.Exists(jsonFile))
                {
                    string jsonData = File.ReadAllText(jsonFile);
                    jsonModelData = JsonConvert.DeserializeObject<JSONModelData>(jsonData);
                    foreach (JSONMeshMaterial mm in jsonModelData.meshMaterials)
                    {
                        if (!String.IsNullOrEmpty(mm.textureDiffuse))
                        {
                            mm.textureDiffuse = mm.textureDiffuse.Replace(".png", "");
                        }
                        if (!String.IsNullOrEmpty(mm.textureSpecular))
                        {
                            mm.textureSpecular = mm.textureSpecular.Replace(".png", "");
                        }
                    }

                    Debug.Log("Have jsonData for " + model.m_name);
                }
                CommonModelData commonModel = model.ToCommon(jsonModelData);

                Dictionary<CommonMaterialData, List<CommonMeshData>> materialMeshLists = SplitByMaterial(commonModel);
                //Debug.LogFormat("Has {0} meshes {1} materials", model.m_modelMeshes.Count, materialMeshLists.Keys.Count);

                GameObject splitPrefab = new GameObject("SplitPrefab");
                //MeshFilter splitPrefabMeshFilter = splitPrefab.AddComponent<MeshFilter>();

                GameObject combinedPrefab = new GameObject(filename.Replace(".mdl.bytes", ""));
                //MeshFilter combinedPrefabMeshFilter = combinedPrefab.AddComponent<MeshFilter>();
                //combinedPrefab.AddComponent<MeshRenderer>();

                Matrix4x4[] bindPoses = null;
                // Build structure if mapped.
                var boneObjectMap = new Dictionary<BoneNode, GameObject>();
                GameObject rootGO = null;

                if (model.m_skinned)
                {
                    // create transform tree?
                    foreach (BoneNode bn in model.BoneList)
                    {
                        GameObject go = new GameObject(bn.UniqueName);
                        boneObjectMap[bn] = go;
                        go.transform.localPosition = bn.offset;
                        go.transform.localRotation = bn.rotation;

                        Quaternion q = bn.rotation;
                        //q.x *= -1.0f;
                        go.transform.localRotation = q;

                        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        //sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                        //sphere.transform.SetParent(go.transform);
                        if (bn.parent != null)
                        {
                            GameObject parentGo = boneObjectMap[bn.parent];
                            go.transform.SetParent(parentGo.transform);
                        }

                        go.transform.localPosition = bn.offset;
                        go.transform.localRotation = bn.rotation;

                        //sphere.transform.localPosition = Vector3.zero;

                    }



                    rootGO = boneObjectMap[model.BoneList[0]];
                    rootGO.transform.SetParent(combinedPrefab.transform);

                    //bindPoses = new Matrix4x4[model.BoneList.Count];

                    //for (int i = 0; i < bindPoses.Length; ++i)
                    //{
                    //    GameObject go = boneObjectMap[model.BoneList[i]];
                    //    bindPoses[i] = go.transform.worldToLocalMatrix * rootGO.transform.localToWorldMatrix;
                    //}

                    // and assign default components
                    BaseActor baseActor = combinedPrefab.AddComponent<BaseActor>();
                    CharacterMeshHolder cmh = combinedPrefab.AddComponent<CharacterMeshHolder>();

                    string[] files = Directory.GetFiles(Application.dataPath + "/Resources/Textures/", combinedPrefab.name + "_armor_var*", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (!fi.FullName.Contains("meta"))
                        {
                            cmh.ArmorVariants.Add(fi.Name);
                        }
                    }

                    files = Directory.GetFiles(Application.dataPath + "/Resources/Textures/", combinedPrefab.name + "_skin_var*", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (!fi.FullName.Contains("meta"))
                        {
                            cmh.SkinVariants.Add(fi.Name);
                        }
                    }


                    GladiusAnim gladiusAnim = combinedPrefab.AddComponent<GladiusAnim>();
                    gladiusAnim.BaseActor = baseActor;
                    String replacedName = combinedPrefab.name;
                    replacedName += ".pak1";
                    Debug.Log("Set pak1 name to " + replacedName);
                    gladiusAnim.Pak1File = replacedName;
                }

                int counter = 0;
                foreach (CommonMaterialData shader in materialMeshLists.Keys)
                {
                    GameObject splitParent = new GameObject();
                    splitParent.transform.SetParent(splitPrefab.transform);
                    List<CommonMeshData> dlhList = materialMeshLists[shader];


                    foreach (var dlh in dlhList)
                    {
                        GameObject submesh = CreateSubMesh(commonModel, dlh);
                        submesh.transform.SetParent(splitParent.transform);
                        //submesh.transform.SetParent(splitPrefab.transform);
                    }

                    GameObject combinedParent = new GameObject("Submesh-" + counter);
                    combinedParent.transform.SetParent(combinedPrefab.transform);
                    MeshFilter combinedParentMeshFilter = combinedParent.AddComponent<MeshFilter>();

                    Renderer renderer = null;
                    if (model.m_skinned)
                    {
                        renderer = combinedParent.AddComponent<SkinnedMeshRenderer>();
                    }
                    else
                    {
                        renderer = combinedParent.AddComponent<MeshRenderer>();
                    }

                    Material m = GetOrCreateMaterial(commonModel, shader);
                    if (m != null)
                    {
                        renderer.material = m;
                    }

                    MeshFilter[] meshFilters = splitParent.GetComponentsInChildren<MeshFilter>();
                    SkinnedMeshRenderer[] splitSkinnedRenderers = splitParent.GetComponentsInChildren<SkinnedMeshRenderer>();
                    Dictionary<int, int> boneConversionDictionary = new Dictionary<int, int>();
                    List<BoneNode> remappedBoneNodeList = new List<BoneNode>();


                    Mesh combinedMesh = MergeMesh(meshFilters,boneConversionDictionary,model.BoneList,remappedBoneNodeList);
                    //for (int i = 0; i < model.BoneList.Count; ++i)
                    //{
                    //    boneConversionDictionary[i] = i;
                    //    remappedBoneNodeList.Add(model.BoneList[i]);
                    //}

                    combinedParentMeshFilter.sharedMesh = combinedMesh;
                    combinedParentMeshFilter.sharedMesh.name = adjustedFilename + "_" + counter++;


                    if (model.m_skinned)
                    {
                        SkinnedMeshRenderer smr = (SkinnedMeshRenderer)renderer;

                        
                        smr.sharedMesh = combinedParentMeshFilter.sharedMesh;
                        smr.rootBone = boneObjectMap[model.BoneList[0]].transform;

						// need these as temporary arrays so we can assign at the end of processing.
						// trying to set them directly on the skinned mesh renderer while they are processing doesn't work
                        Transform[] bonesCopy = new Transform[remappedBoneNodeList.Count];
                        bindPoses = new Matrix4x4[remappedBoneNodeList.Count];

                        for (int i = 0; i < bindPoses.Length; ++i)
                        {
                            GameObject go = boneObjectMap[remappedBoneNodeList[i]];
                            bonesCopy[i] = go.transform;
                            bindPoses[i] = go.transform.worldToLocalMatrix;
                        }

                        smr.bones = bonesCopy;
                        combinedParentMeshFilter.sharedMesh.bindposes = bindPoses;
                        smr.sharedMesh.RecalculateBounds();

                        BoneWeight[] bwa = combinedParentMeshFilter.sharedMesh.boneWeights;
                        Matrix4x4[] bpa = combinedParentMeshFilter.sharedMesh.bindposes;
                        Transform[] ta = smr.bones;
                        int ibreak = 0;
                    }

                    AssetDatabase.CreateAsset(combinedParentMeshFilter.sharedMesh, "Assets/Resources/Meshes/" + combinedParentMeshFilter.sharedMesh.name + ".mesh");
                    //break;
                }

                UnityEngine.Object.DestroyImmediate(splitPrefab);
                // Path to out new asset
                //string newPath = ConvertToInternalPath(asset);
                combinedPrefab.transform.localRotation = Quaternion.Euler(270.0f, 0, 0);

                String outputDirName = "Resources/XboxModelPrefabs/" + assetSubDirectories + "/";
                String fullOuputDirName = Application.dataPath + "/" + outputDirName;
                if (!Directory.Exists(fullOuputDirName))
                {
                    Directory.CreateDirectory(fullOuputDirName);
                }
                PrefabUtility.CreatePrefab("Assets/" + outputDirName + adjustedFilename + ".prefab", combinedPrefab);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed [{0}] [{1}]", filename, e);
            }
        }
    }

}
