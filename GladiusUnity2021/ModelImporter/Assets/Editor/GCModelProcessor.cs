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
        public const string m_extension = ".pax";
        public const string m_newExtension = ".asset";
        public const string OriginalModelDirectory = "GCModels";
        public const string OutputDirectory = "Resources/GCModelPrefabs/";
        public const string PrefabOutputDirectory = "Resources/GCModelPrefabs/";

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
                if (asset.ToUpper().Contains(("Assets/" + OriginalModelDirectory).ToUpper()))
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
                int lodLevel = 1;

                TextAsset assetData = AssetDatabase.LoadAssetAtPath<TextAsset>(assetName);
                if (assetData != null)
                {
                    string adjustedFilename = CommonModelProcessor.TidyAssetName(assetName, OriginalModelDirectory);

                    GCModel model = new GCModel(adjustedFilename);

                    using (BinaryReader binReader = new BinaryReader(new MemoryStream(assetData.bytes)))
                    {
                        model.LoadData(binReader, null);

                        CommonModelData commonModel = model.ToCommon();
                        CommonModelProcessor.ProcessCommonModel(assetName, lodLevel, commonModel,
                            PrefabOutputDirectory);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed [{0}] [{1}]", assetName, e.StackTrace);
            }
        }


}