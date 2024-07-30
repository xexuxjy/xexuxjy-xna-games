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
                    CommonModelProcessor.ProcessCommonModel(assetName, lodLevel, commonModel,"Resources/XboxModelPrefabs/");


                    Debug.Log("Best lod level is : " + lodLevel);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed [{0}] [{1}]", assetName, e.StackTrace);
            }
        }






        //public static void Process(String assetName, String adjustedFilename, CommonModelData commonModel, XboxModelRW xboxModel, List<CommonMeshData> meshList, bool merge, GameObject splitPrefab, GameObject combinedPrefab, int index, Dictionary<BoneNode, GameObject> boneObjectMap)
        
    }
}
