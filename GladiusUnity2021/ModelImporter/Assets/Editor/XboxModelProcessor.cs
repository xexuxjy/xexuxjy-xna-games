// Contains an example of Merging Unity Skinned Meshes - bone lookup map and so on. - MAN

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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
                    int lodLevel = 1;//model.m_selsInfo.GetBestLodData();
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

            Dictionary<CommonMaterialData, List<CommonMeshData>> materialMeshLists = CommonModelProcessor.SplitByMaterial(commonModel);

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
                    //RigTransform rigTransform = go.AddComponent<RigTransform>();
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

                bool isCrowdAnim = noExtensionName.StartsWith("crowd");
                if (isCrowdAnim)
                {
                    //CrowdAnim crowdAnim = combinedPrefab.AddComponent<CrowdAnim>();
//                    crowdAnim.RandomiseAnimationStart = true;

                    String[] crowAnimTypes = { "booa", "boob", "btd", "exciteda", "excitedb", "idle" };

                    foreach (String animType in crowAnimTypes)
                    {


                        DirectoryInfo di = new DirectoryInfo(@"F:\UnityProjects\GladiusDFGui\Assets\Resources\GladiusAnims\crowd\");
                        FileInfo[] animFiles = di.GetFiles(noExtensionName + "_" + animType + ".pan.bytes", SearchOption.AllDirectories);
                        foreach (FileInfo fileInfo in animFiles)
                        {

                            string fullName = fileInfo.FullName;
                            string relativeName = fullName.Replace(@"F:\UnityProjects\GladiusDFGui\Assets\Resources\GladiusAnims\crowd\", "GladiusAnims/crowd/");


                            //String crowdAnimName = "GladiusAnims /crowd/" + fileInfo.Name;
                            string crowdAnimName = relativeName.Replace(".bytes", "");
                            TextAsset textAsset = GladiusGlobals.LoadTextAsset(crowdAnimName);
                            if (textAsset != null)
                            {
  //                              crowdAnim.AddAnimationData(textAsset);
                            }
                        }
                    }
                }
                else
                {
                    String objectAnimName = "GladiusAnims/objectanims/" + noExtensionName;

                    TextAsset textAsset = GladiusGlobals.LoadTextAsset(objectAnimName);

                    bool isSingleAnimation = textAsset != null;

                    if (isSingleAnimation)
                    {
                        SimpleMeshHolder smh = combinedPrefab.AddComponent<SimpleMeshHolder>();
                        smh.AnimationFile = textAsset;
                        smh.RandomiseAnimationStart = true;
                    }
                    else
                    {
                        CharacterMeshHolder cmh = combinedPrefab.AddComponent<CharacterMeshHolder>();
                    }
                }


                BoneRenderer boneRenderer = rootGO.AddComponent<BoneRenderer>();
                boneRenderer.transforms = new Transform[commonModel.BoneList.Count - 1];
                for (int i = 1; i < commonModel.BoneList.Count; ++i)
                {
                    boneRenderer.transforms[i - 1] = boneObjectMap[commonModel.BoneList[i]].transform;
                }

                Rig rig = rootGO.AddComponent<Rig>();

                RigLayer rigLayer = new RigLayer(rig);
                
                RigBuilder rigBuilder = rootGO.AddComponent<RigBuilder>();
                rigBuilder.layers.Add(rigLayer);

                Animator animator = rootGO.AddComponent<Animator>();
                
                // look at adding human avatar - though not sure whgy....
                

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
                GameObject submesh = CommonModelProcessor.CreateSubMesh(commonModel, mesh, "Submesh_" + index + "_" + (count++));
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
            List<Mesh> combinedMeshes = CommonModelProcessor.MergeMesh(merge, meshes, boneConversionDictionary, commonModel.BoneList, remappedBoneNodeList);

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

                Material m = CommonModelProcessor.GetOrCreateMaterial(commonModel, commonMaterialData, meshList[0]);
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
