using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;

namespace Assets.Editor
{
    public class GCModelProcessor : AssetPostprocessor
    {
        public static bool MERGE_MESH = true;
        public const string m_extension = ".mdl";
        public const string m_newExtension = ".asset";
        public const string OriginalModelDirectory = "OriginalGCModel";
        public const string OutputDirectory = "Resources/GCModelPrefabs/";
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
            Debug.Log("In GC processor");
            foreach (string asset in importedAssets)
            {
                Debug.Log(asset);
                // This is our detection of file - by extension
                //if (HasExtension(asset))
                if (asset.ToUpper().Contains(("Assets/"+OriginalModelDirectory).ToUpper()))
                {
                    ImportMyAsset(asset);
                }
            }
        }

        static void ImportMyAsset(string asset)
        {
            ImportGCModel(asset);
        }

        static void ImportGCModel(string assetName)
        {
            Debug.Log("GCModelProcessor importing : " + assetName);

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

                    GCModel model = new GCModel("");

                    model.m_name = adjustedFilename;

                    // deal with empty files

                    binReader.BaseStream.Position = 0;
                    model.LoadData(binReader);

                    int lodLevel = 1;

                    Debug.Log("Best lod level is : " + lodLevel);

                    GameObject combinedPrefab = new GameObject(adjustedFilename);
                    GameObject gladiusToUnity = new GameObject("GladiusToUnity");
                    gladiusToUnity.transform.SetParent(combinedPrefab.transform, false);


                    Mesh populatedMesh= PopulateMeshData(model);
                    populatedMesh.name = adjustedFilename + "_" + 0;

                    string meshOutputDir = "Assets/Resources/Meshes/GC/";
                    if (!Directory.Exists(meshOutputDir))
                    {
                        Directory.CreateDirectory(meshOutputDir);
                    }
                    
                    AssetDatabase.CreateAsset(populatedMesh, meshOutputDir + populatedMesh.name + ".mesh");


                    MeshFilter filter = gladiusToUnity.AddComponent<MeshFilter>();
                    MeshRenderer renderer = gladiusToUnity.AddComponent<MeshRenderer>();
                        
                    filter.sharedMesh = populatedMesh;

                    String outputDirName = OutputDirectory + assetSubDirectories;// + "/";
                    String fullOuputDirName = Application.dataPath + "/" + outputDirName;
                    if (!Directory.Exists(fullOuputDirName))
                    {
                        Directory.CreateDirectory(fullOuputDirName);
                    }

                    String prefabName = "Assets/" + outputDirName + adjustedFilename + ".prefab";


                    UnityEngine.Object existingPrefab = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabName);
                    if (existingPrefab == null)
                    {
                        PrefabUtility.CreatePrefab(prefabName, combinedPrefab, ReplacePrefabOptions.ReplaceNameBased);
                    }
                    else
                    {
                        PrefabUtility.ReplacePrefab(combinedPrefab, existingPrefab);
                    }



                    if (combinedPrefab != null)
                    {
                        UnityEngine.Object.DestroyImmediate(combinedPrefab);
                    }


                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed [{0}] [{1}]", assetName, e.StackTrace);
            }
        }

        public static Mesh PopulateMeshData(GCModel model,bool merge = false)
        {
            List<int> indices = new List<int>();
            List<Vector3> points = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            model.BuildStandardMesh(indices,points,normals,uvs);

            Mesh mesh = new Mesh();
            mesh.vertices = points.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uvs.ToArray();
            
            return mesh;
        }

    }
}
