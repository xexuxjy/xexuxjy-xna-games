using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public static class CommonModelProcessor
{
    public static bool MERGE_MESH = false;

    public const string TexturesPath = "Assets/Textures/";
    public const string MaterialsPath = "Assets/Materials/";

    public static Dictionary<String, Material> TempMaterialStore = new Dictionary<string, Material>();

    public static Texture GetTexture(string name)
    {
        string fullPath = TexturesPath + name + ".png";
        Texture t = AssetDatabase.LoadAssetAtPath<Texture>(fullPath);
        if (t == null && !name.EndsWith(".tga"))
        {
            t = AssetDatabase.LoadAssetAtPath<Texture>(TexturesPath + name + ".tga.png");
        }

        return t;
    }


    public static string TidyAssetName(string assetName, string originalModelDirectory)
    {
        string assetPath = assetName.Substring(0, assetName.LastIndexOf('/'));
        string filename = assetName.Substring(assetName.LastIndexOf('/') + 1);
        int startIndex = assetName.IndexOf(originalModelDirectory) + originalModelDirectory.Length;
        int endIndex = assetName.LastIndexOf('/');
        string assetSubDirectories = assetName.Substring(startIndex, endIndex - startIndex);

        string adjustedFilename = filename.Replace(".bytes", "");
        adjustedFilename = adjustedFilename.Replace(".mdl", "");
        adjustedFilename = adjustedFilename.Replace(".pax", "");
        return adjustedFilename;
    }

    public static string GetOutputHierarchy(string assetName, string originalModelDirectory)
    {
        string assetPath = assetName.Substring(0, assetName.LastIndexOf('/'));
        string filename = assetName.Substring(assetName.LastIndexOf('/') + 1);
        int startIndex = assetName.IndexOf(originalModelDirectory) + originalModelDirectory.Length;
        int endIndex = assetName.LastIndexOf('/');
        string assetSubDirectories = assetName.Substring(startIndex, endIndex - startIndex);


        if (originalModelDirectory.EndsWith("/"))
        {
            assetSubDirectories = assetPath.Replace("Assets/" + originalModelDirectory + "/", "");
        }
        else
        {
            assetSubDirectories = assetPath.Replace("Assets/" + originalModelDirectory, "");
        }

        return assetSubDirectories + "/";
    }


    public static Material GetOrCreateMaterial(string modelname,CommonMaterialData commonMaterialData,
        string outputHierarchy,bool isCharacter)
    {
        CommonTextureData textureData1 = commonMaterialData.TextureData1;
        CommonTextureData textureData2 = commonMaterialData.TextureData2;
        
        Material finalMaterial = null;

        try
        {
            commonMaterialData.GenerateNameFromTextures();
            
            finalMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialsPath + outputHierarchy + commonMaterialData.Name);
            if (finalMaterial == null)
            {
                TempMaterialStore.TryGetValue(commonMaterialData.Name, out finalMaterial);
            }

            //if (textureData1 != null)
            if(finalMaterial == null)
            {
                //m = AssetDatabase.LoadAssetAtPath<Material>(MaterialsPath + materialData.Name);
                Texture texture1 = GetTexture(textureData1.textureName);
                Texture texture2 = null;
                
                if (texture1 == null)
                {
                    Debug.LogWarningFormat("Can't find texture {0}", textureData1.textureName);
                }

                if (!isCharacter && DoesTextureHaveAlpha(texture1))
                {
                    commonMaterialData.isTransparent = true;
                }

                if (textureData2 != null)
                {
                    texture2 = GetTexture(textureData2.textureName);
                    if (DoesTextureHaveAlpha(texture2))
                    {
                        commonMaterialData.isTransparent = true;
                    }
                }

                Material templateMaterial = null;
                if (commonMaterialData.IsCutOut)
                {
                    templateMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialsPath + "Templates/TemplateMaterialCutout.mat");
                }
                else if (commonMaterialData.IsTransparent)
                {
                    templateMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialsPath + "Templates/TemplateMaterialAlpha.mat");    
                }
                else if (texture2 != null && commonMaterialData.IsDetailMap)
                {
                    templateMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialsPath + "Templates/TemplateMaterialDetail.mat");
                }
                else
                {
                    templateMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialsPath + "Templates/TemplateMaterialStandard.mat");    
                }

                if (templateMaterial != null)
                {
                    finalMaterial = UnityEngine.Object.Instantiate(templateMaterial);
                    finalMaterial.name = commonMaterialData.Name;

                    if (texture1 is Cubemap)
                    {
                        int ibreak = 0;
                    }
                    
                    finalMaterial.SetTexture("_BaseMap", texture1);

                    
                    if (texture2 != null && commonMaterialData.IsDetailMap)
                    {
                        finalMaterial.SetTexture("_Detail", texture2);
                    }

                    string assetOutputDirName = "Materials/" + outputHierarchy;
                    string fullOuputDirName = Application.dataPath + "/" + assetOutputDirName;

                    if (!Directory.Exists(fullOuputDirName))
                    {
                        Directory.CreateDirectory(fullOuputDirName);
                    }

                    if (TempMaterialStore.ContainsKey(finalMaterial.name))
                    {
                        int ibreak = 0;
                    }
                    
                    
                    
                    TempMaterialStore[finalMaterial.name] = finalMaterial;
                    AssetDatabase.CreateAsset(finalMaterial, "Assets/" + assetOutputDirName + finalMaterial.name + ".mat");
                }
                else
                {
                    int ibreak = 0;
                }
            }
        }
        catch (Exception e)
        {
            int ibreak = 0;
        }

        return finalMaterial;
    }

    public static void BuildArmourAndSkinVarients(string modelName,string outputHierarchy ,List<Material> armourMaterials,
        List<Material> skinMaterials,bool isCharacter)
    {
        TryAddMaterialSet(modelName, "skin", outputHierarchy, skinMaterials,isCharacter);
        TryAddMaterialSet(modelName, "armor", outputHierarchy, armourMaterials,isCharacter);
        
    }

    private static void TryAddMaterialSet(string modelName, string type, string outputHierarchy,List<Material> results,bool isCharacter)
    {
        bool foundSimple = false;
        for (int i = 1; i < 5; ++i)
        {
            string textureName = $"{modelName}_{type}_var0{i}";
    
            Texture t = GetTexture(textureName);
            if (t != null)
            {
                foundSimple = true;
                CommonMaterialData cmd = new CommonMaterialData();
                cmd.TextureData1 = new CommonTextureData();
                cmd.TextureData1.textureName = textureName;
                
                Material m = GetOrCreateMaterial(modelName, cmd, outputHierarchy,isCharacter);
                results.Add(m);
            }
        }


        if (!foundSimple)
        {
            for (int i = 1; i < 10; ++i)
            {
                string textureName = $"{modelName}_{type}_var0{i}_{modelName}_{type}A_var0{i}.ra";

                Texture t = GetTexture(textureName);
                if (t != null)
                {
                    CommonMaterialData cmd = new CommonMaterialData();
                    cmd.TextureData1 = new CommonTextureData();
                    cmd.TextureData1.textureName = textureName;
                    Material m = GetOrCreateMaterial(modelName, cmd, outputHierarchy,isCharacter);
                    results.Add(m);
                }
            }
        }

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


    public static Mesh CombineMeshes(Mesh mesh, Mesh invertedMesh, CommonModelData commonModel, bool remapBones = true)
    {
        List<Mesh> originalMeshes = new List<Mesh>();
        originalMeshes.Add(mesh);
        originalMeshes.Add(invertedMesh);
        Dictionary<int, int> boneConversionDictionary = new Dictionary<int, int>();
        List<BoneNode> remappedBoneNodeList = new List<BoneNode>();
        List<Mesh> combinedMeshes = MergeMesh(true, originalMeshes.ToArray(), boneConversionDictionary,
            commonModel.BoneList, remappedBoneNodeList, remapBones);
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


    public static string ProcessCommonModel(string fullAssetPath, string outputHierarchy, int lodLevel,
        CommonModelData commonModel, string outputDirectory)
    {
        //lodLevel = model.GetMeshLodLevel("set_LOD1");
        Debug.Log("Best lod level is : " + lodLevel);

        //TempMaterialStore.Clear();
        
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

        Dictionary<CommonMaterialData, List<CommonMeshData>> materialMeshLists =
            CommonModelProcessor.SplitByMaterial(commonModel);

        GameObject splitPrefab = new GameObject(commonModel.Name + "-SplitPrefab");

        GameObject combinedPrefab = new GameObject(commonModel.Name);
        GameObject gladiusToUnity = new GameObject("GladiusToUnity");
        gladiusToUnity.transform.SetParent(combinedPrefab.transform, false);


        // Build structure if mapped.
        var boneObjectMap = new Dictionary<BoneNode, GameObject>();
        GameObject rootGO = null;
        bool isCharacter = fullAssetPath.ContainsInvariantCultureIgnoreCase("characters");

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
            }

            rootGO = boneObjectMap[commonModel.BoneList[0]];
            rootGO.transform.SetParent(gladiusToUnity.transform, false);


            String noExtensionName = commonModel.Name.Replace(".mdl", "");

            bool isCrowdAnim = noExtensionName.StartsWith("crowd");

           
            if (isCrowdAnim)
            {
                List<TextAsset> anims = LoadAnimsAtPath(outputHierarchy);
                
                CrowdAnim crowdAnim = combinedPrefab.AddComponent<CrowdAnim>();
                crowdAnim.RandomiseAnimationStart = true;
                foreach (TextAsset textAsset in anims)
                {
                    crowdAnim.AddAnimationData(textAsset);
                }
            }
            else
            {
                String objectAnimName = "GladiusAnims/objectanims/" + noExtensionName;
                TextAsset objectAnimNameTextAsset =
                    AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + objectAnimName + ".bytes");

                bool isSingleAnimation = objectAnimNameTextAsset != null;

                if (isSingleAnimation)
                {
                    SimpleMeshHolder smh = combinedPrefab.AddComponent<SimpleMeshHolder>();
                    smh.AnimationFile = objectAnimNameTextAsset;
                    smh.RandomiseAnimationStart = true;
                }
                else
                {
                    CharacterMeshHolder cmh = combinedPrefab.AddComponent<CharacterMeshHolder>();

                    List<TextAsset> anims = LoadAnimsAtPath(outputHierarchy);
                    cmh.CharacterAnims = new TextAsset[anims.Count];

                    int count = 0;
                    foreach (TextAsset textAsset in anims)
                    {
                        cmh.CharacterAnims[count++] = textAsset;
                    }
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

        
        String outputDirName = outputDirectory + outputHierarchy + "/";
        outputDirName = outputDirName.Replace("//", "/"); 
            

        String fullOuputDirName = Application.dataPath + "/" + outputDirName;
        if (!Directory.Exists(fullOuputDirName))
        {
            Directory.CreateDirectory(fullOuputDirName);
        }
        
        bool isArena = fullAssetPath.Contains("levels/") && commonModel.XBoxModel.OBBTChunk != null;
        bool isRegion = fullAssetPath.Contains("regions/");

        if (isArena)
        {
            BuildArenaProps(combinedPrefab, commonModel.Name,outputDirectory);
            //BuildArenaLightVolume(combinedPrefab,commonModel.Name, outputDirName);
        }

        if (isRegion)
        {
            BuildRegionProps(combinedPrefab, commonModel.Name);
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
                Process(commonModel, outputHierarchy, commonMeshDataList, MERGE_MESH, splitPrefab, gladiusToUnity,
                    counter++, boneObjectMap,isCharacter);
            }
        }
        else
        {
            foreach (CommonMeshData mesh in commonModel.CommonMeshData)
            {
                List<CommonMeshData> commonMeshDataList = new List<CommonMeshData>();
                commonMeshDataList.Add(mesh);
                Process(commonModel, outputHierarchy, commonMeshDataList, MERGE_MESH, splitPrefab, gladiusToUnity,
                    counter++, boneObjectMap,isCharacter);
            }
        }

        if (isCharacter)
        {
            List<Material> armourMaterials = new List<Material>();
            List<Material> skinrMaterials = new List<Material>();
            BuildArmourAndSkinVarients(commonModel.Name, outputHierarchy, armourMaterials, skinrMaterials,isCharacter);
            CharacterMeshHolder characterMeshHolder = combinedPrefab.GetComponentInChildren<CharacterMeshHolder>();
            if (characterMeshHolder != null)
            {
                characterMeshHolder.ArmourMaterials.AddRange(armourMaterials);
                characterMeshHolder.SkinMaterials.AddRange(skinrMaterials);
            }
        }
        
        //"Resources/XboxModelPrefabs/"
        //String outputDirName = outputDirectory + commonModel.AssetSubDirectories + "/";

        String prefabName = "Assets/" + outputDirName + commonModel.Name + ".prefab";


        PrefabUtility.SaveAsPrefabAssetAndConnect(combinedPrefab, prefabName, InteractionMode.AutomatedAction);


        // UnityEngine.Object existingPrefab = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabName);
        // if (existingPrefab == null)
        // {
        //     PrefabUtility.CreatePrefab(prefabName, combinedPrefab, ReplacePrefabOptions.ReplaceNameBased);
        //
        // }
        // else
        // {
        //     PrefabUtility.ReplacePrefab(combinedPrefab, existingPrefab);
        // }


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


    public static List<TextAsset> LoadAnimsAtPath(string outputHierarchy)
    {
        List<TextAsset> results = new List<TextAsset>();
        outputHierarchy = outputHierarchy.Replace("characters/", "");
        
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + @"/GladiusAnims/" + outputHierarchy);

        if (di.Exists)
        {
            FileInfo[] animFiles = di.GetFiles("*.pan.bytes", SearchOption.TopDirectoryOnly);

            foreach (FileInfo fileInfo in animFiles)
            {
                string fullName = fileInfo.FullName;
                fullName = fullName.Replace('\\', '/');
                string relativeName = fullName.Replace(Application.dataPath + "/", "");
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + relativeName);

                if (textAsset != null)
                {
                    results.Add(textAsset);
                }
            }
        }

        return results;
    }
    
    public static void Process(CommonModelData commonModel, string outputHierarchy, List<CommonMeshData> meshList,
        bool merge, GameObject splitPrefab, GameObject combinedPrefab, int index,
        Dictionary<BoneNode, GameObject> boneObjectMap,bool isCharacter)
    {
        int count = 0;

        GameObject splitParent = new GameObject();
        splitParent.transform.SetParent(splitPrefab.transform, false);

        foreach (CommonMeshData mesh in meshList)
        {
            GameObject submesh =
                CommonModelProcessor.CreateSubMesh(commonModel, mesh, "Submesh_" + index + "_" + (count++),
                    outputHierarchy,isCharacter);
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

        // always merge here as the splits handled furhter up..
        List<Mesh> combinedMeshes = CommonModelProcessor.MergeMesh(merge, meshes, boneConversionDictionary,
            commonModel.BoneList, remappedBoneNodeList);

        CommonMaterialData commonMaterialData = commonModel.CommonMaterials[(int)meshList.First().MaterialId];

        foreach (Mesh combinedMesh in combinedMeshes)
        {
            GameObject combinedParent =
                new GameObject("Submesh-" + index + "-" + commonMaterialData.TextureData1.textureName);
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

            Material m =GetOrCreateMaterial(commonModel.Name, commonMaterialData, outputHierarchy,isCharacter);
            if (m != null)
            {
                renderer.sharedMaterial = m;
            }


            combinedMesh.RecalculateBounds();

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


            string assetOutputDirName = "Meshes/" + outputHierarchy;
            string fullOuputDirName = Application.dataPath + "/" + assetOutputDirName;

            if (!Directory.Exists(fullOuputDirName))
            {
                Directory.CreateDirectory(fullOuputDirName);
            }

            AssetDatabase.CreateAsset(combinedParentMeshFilter.sharedMesh, "Assets/" + assetOutputDirName + combinedParentMeshFilter.sharedMesh.name + ".mesh");
            
        }
    }

    private static T CreateOrReplaceAsset<T>(T asset, string path) where T : UnityEngine.Object
    {
        T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (existingAsset == null)
        {
            AssetDatabase.CreateAsset(asset, path);
        }
        else
        {
            if (typeof(Mesh).IsAssignableFrom(typeof(T)))
            {
                //(existingAsset as Mesh)?.Clear();
            }

            EditorUtility.CopySerialized(asset, existingAsset);


            existingAsset = asset;
        }

        return existingAsset;
    }

    // merge basic mesh properties.
    // assume that submesh list is filtered my material
    public static List<Mesh> MergeMesh(bool merge, Mesh[] meshes, Dictionary<int, int> boneConversionDictionary,
        List<BoneNode> originalBoneNodeList, List<BoneNode> remappedBoneNodeList, bool remapBones = true)
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
                        bw.boneIndex0 = RemapBone(bw.boneIndex0, bw.weight0, boneConversionDictionary,
                            originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex1 = RemapBone(bw.boneIndex1, bw.weight1, boneConversionDictionary,
                            originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex2 = RemapBone(bw.boneIndex2, bw.weight2, boneConversionDictionary,
                            originalBoneNodeList, remappedBoneNodeList);
                        bw.boneIndex3 = RemapBone(bw.boneIndex3, bw.weight3, boneConversionDictionary,
                            originalBoneNodeList, remappedBoneNodeList);
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

    public static int RemapBone(int boneId, float weight, Dictionary<int, int> boneMap,
        List<BoneNode> originalBoneNodeList, List<BoneNode> remappedBoneNodeList)
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
        Dictionary<CommonMaterialData, List<CommonMeshData>> result =
            new Dictionary<CommonMaterialData, List<CommonMeshData>>();
        foreach (CommonMeshData mesh in model.CommonMeshData)
        {
            CommonMaterialData materialData = model.CommonMaterials[(int)mesh.MaterialId];
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

    public static GameObject CreateSubMesh(CommonModelData commonModel, CommonMeshData submesh, string name,
        string outputHierarchy,bool isCharacter)
    {
        GameObject submeshObject = new GameObject();
        string fullName = commonModel.Name + name;

        submeshObject.name = fullName;
        if (submesh.Index == 9)
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


        CommonMaterialData commonMaterial = commonModel.CommonMaterials[(int)submesh.MaterialId];
        Material m = GetOrCreateMaterial(commonModel.Name,commonMaterial, outputHierarchy,isCharacter);
        if (m != null)
        {
            renderer.material = m;
        }

        Mesh mesh = new Mesh();
        mesh.name = submesh.Name;
        mf.sharedMesh = mesh;

        Vector3[] tempPos = new Vector3[submesh.Vertices.Count];

        mesh.vertices = new Vector3[submesh.Vertices.Count];
        for (int i = 0; i < tempPos.Length; ++i)
        {
            Vector3 pos = commonModel.AllVertices[submesh.Vertices[i]].Position;
            tempPos[i] = pos;
        }

        Vector3[] tempNormal = new Vector3[submesh.Vertices.Count];
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
        for (int i = 0; i < tempTriangles.Length; i += 3)
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
                for (int j = 0; j < vertex.BoneIndices.Length; ++j)
                {
                    int originalBoneId = -1;
                    originalBoneId =
                        commonModel.XBoxModel.XRenderSetup.AdjustBone(vertex.BoneIndices[j], submesh.Index);
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

        bool twoSidedMaterial = commonModel.Skinned && submesh.LodLevel == 0;

        if (mesh.triangles.Length == 135)
        {
            int ibreak = 0;
        }

        if (twoSidedMaterial || commonMaterial.IsTwoSided)
        {
            Mesh invertedMesh = CreateInvertedMesh(mesh);
            Mesh combinedMesh = CombineMeshes(mesh, invertedMesh, commonModel, false);
            mesh = combinedMesh;
        }

        mesh.name = fullName;
        mf.sharedMesh = mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return submeshObject;
    }

    public static void BuildRegionProps(GameObject prefabParent, string assetName)
    {
    }

    public static void BuildArenaLightVolume(GameObject prefabParent, string assetName, string outputDirectory)
    {
        String arenaName = assetName.Replace(".mdl", "");
        String lightVolumeName = assetName + "_lightvolumes.prefab";

        String prefabName = "Assets/" + outputDirectory + lightVolumeName;
        GameObject lightVolumeGO  = AssetDatabase.LoadAssetAtPath<GameObject>(prefabName);
        if (lightVolumeGO != null)
        {
            GameObject lightVolumeGroup = new GameObject("LightVolume");
            lightVolumeGroup.transform.parent = prefabParent.transform;
            GameObject modelInstance = UnityEngine.Object.Instantiate(lightVolumeGO,lightVolumeGroup.transform);
        }
        int ibreak = 0;
    }
    
    public static void BuildArenaProps(GameObject prefabParent, string assetName,string outputDirectory)
    {
        String arenaName = assetName.Replace(".mdl", "");

        String locatorPath = "GladiusConfig/LocatorFiles/" + arenaName+".txt";
        GameObject propGroup = new GameObject("PropGroup");

        //propGroup.transform.SetParent(prefabParent.transform,false);
        propGroup.transform.SetParent(prefabParent.transform, false);
        //propGroup.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);


        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + locatorPath);
        if(textAsset != null && !String.IsNullOrEmpty(textAsset.text))
        {
            String[] allLines = textAsset.text.Split('\n');
            for (int i = 1; i < allLines.Length; ++i)
            {
                try
                {
                    //LOCATOR: "ph_thefen_pig1" "levels/nordagh/thefen/thefen_pigAnim.ma" 2.330588 18.631809 -0.669269 0.000000 0.000000 0.843391 -0.537300
                    String[] dataLineElements = allLines[i].Split(' ');
                    String loc = dataLineElements[0];
                    String name1 = dataLineElements[1];
                    String model = dataLineElements[2];
                    if (model.Contains(".ma"))
                    {
                        model = model.Replace("\"", "");
                        model = model.Replace(".ma", "");

                        Vector3 position = GladiusGlobals.ReadV3(dataLineElements, 3);
                        Quaternion rotation = GladiusGlobals.ReadQuaternion(dataLineElements, 6);



                        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + outputDirectory + model+".prefab");

                        if (modelPrefab != null)
                        {
                            GameObject modelInstance = UnityEngine.Object.Instantiate(modelPrefab);
                            //modelGoCopy.transform.SetParent(propGroup.transform,false);
                            modelInstance.transform.SetParent(propGroup.transform);
                            modelInstance.transform.localPosition = position;
                            modelInstance.transform.localRotation = rotation;
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}

