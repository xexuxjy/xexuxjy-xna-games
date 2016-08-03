using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace UnityClipGenerator
{
    public class AnimationClipAssetPostprocessor : AssetPostprocessor
    {
        public String m_modelPath;
        public String m_modelName;

        public void OnPreprocessModel()
        {
            if (!assetImporter) return;

            ModelImporter mi = assetImporter as ModelImporter;
            String modelName = mi.name;
            String assetPath = mi.assetPath.Substring(0, mi.assetPath.LastIndexOf('/'));
            String filename = mi.assetPath.Substring(mi.assetPath.LastIndexOf('/') + 1);
            m_modelPath = mi.assetPath;
            m_modelName = filename;

            if (m_modelPath.Contains("Arenas") || m_modelPath.Contains("Region"))
            {
                mi.addCollider = true;
            }
            if (!mi) return;
            mi.animationType = ModelImporterAnimationType.Legacy;
            mi.materialName = ModelImporterMaterialName.BasedOnTextureName;
            
        }

        public void OnPostprocessModel(GameObject go)
        {
            //go.transform.localScale = new Vector3(100, 100, 100);
            //Vector3 rot = new Vector3(270, 180, 0);
            //go.transform.localRotation = Quaternion.Euler(rot);
            //go.transform.eulerAngles = rot;
            if (m_modelPath.Contains("character"))
            {
                Debug.Log("Adding components to character model" + m_modelPath);
                go.AddComponent<BaseActor>();
                go.AddComponent<CharacterMeshHolder>();
                GladiusAnim gladiusAnim = go.AddComponent<GladiusAnim>();
                String replacedName= m_modelName.Replace(".mdl", "");
                replacedName = replacedName.Replace(".fbx", "");
                replacedName += ".pak1";
                Debug.Log("Set pak1 name to " + replacedName);
                gladiusAnim.Pak1File = replacedName;
                
            }
        }




    }
}
