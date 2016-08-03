//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEditor;
//using System.IO;
//using UnityEngine;

//namespace UnityClipGenerator
//{
//    public class AnimationClipAssetPostprocessor : AssetPostprocessor
//    {
//        public String m_modelPath;

//        public void OnPreprocessModel()
//        {
//            if (!assetImporter) return;

//            ModelImporter mi = assetImporter as ModelImporter;

//            if (!mi) return;
//            mi.animationType = ModelImporterAnimationType.Legacy;
            
//            String modelName = mi.name;
//            String assetPath = mi.assetPath.Substring(0, mi.assetPath.LastIndexOf('/'));
//            String filename = mi.assetPath.Substring(mi.assetPath.LastIndexOf('/') + 1);
//            m_modelPath = mi.assetPath;

//            if (m_modelPath.Contains("Arenas") || m_modelPath.Contains("Region"))
//            {
//                mi.addCollider = true;
//            }


//            BuildClipAnimations(mi);
//        }

//        public void OnPostprocessModel(GameObject go)
//        {
//            //go.transform.localScale = new Vector3(100, 100, 100);
//            //Vector3 rot = new Vector3(270, 180, 0);
//            //go.transform.localRotation = Quaternion.Euler(rot);
//            //go.transform.eulerAngles = rot;
//            if (m_modelPath.Contains("character"))
//            {
//                Debug.Log("Adding components to character model" + m_modelPath);
//                if (!go.GetComponent<BaseActor>())
//                {
//                    go.AddComponent<BaseActor>();
//                }
//                if (!go.GetComponent<CharacterMeshHolder>())
//                {
//                    go.AddComponent<CharacterMeshHolder>();
//                }
//                if (!go.GetComponent<GladiusAnim>())
//                {
//                    go.AddComponent<GladiusAnim>();
//                }
//            }
//        }


//        public void BuildClipAnimations(ModelImporter mi)
//        {

//            //if (mi.assetPath.Contains("characters"))
//            //{
//            //    go.AddComponent<CharacterMeshHolder>();
//            //}
//                //String newPath = assetPath + "/frameData/" + filename;

//                //String clipFileName = newPath.Replace(".fbx", ".framedata");
//                //if (File.Exists(clipFileName))
//                //{

//                //    try
//                //    {
//                //        Debug.Log("Reading " + clipFileName);
//                //        List<String> clipData = new List<string>();
//                //        using (StreamReader reader = new StreamReader(clipFileName))
//                //        {
//                //            while (!reader.EndOfStream)
//                //            {
//                //                clipData.Add(reader.ReadLine());
//                //            }

//                //        }

//                //        Debug.Log("Reading " + clipFileName + " done " + clipData.Count + " lines.");
//                //        List<ModelImporterClipAnimation> animClips = new List<ModelImporterClipAnimation>();
//                //        foreach (String line in clipData)
//                //        {
//                //            String[] tokens = line.Split(',');
//                //            //Debug.Log("Split to "+tokens.Length+" tokens");                    
//                //            if (tokens.Length == 4)
//                //            {
//                //                string animName = tokens[0];
//                //                int startFrame = int.Parse(tokens[1]);
//                //                int endFrame = int.Parse(tokens[2]);
//                //                bool loop = bool.Parse(tokens[3]);
//                //                //bool wrap = bool.Parse(tokens[4]);

//                //                ModelImporterClipAnimation clipAnimation = new ModelImporterClipAnimation();
//                //                clipAnimation.firstFrame = startFrame;
//                //                clipAnimation.lastFrame = endFrame;
//                //                clipAnimation.name = animName;
//                //                clipAnimation.loop = loop;
//                //                clipAnimation.wrapMode = loop ? UnityEngine.WrapMode.Loop : UnityEngine.WrapMode.Once;
//                //                animClips.Add(clipAnimation);
//                //            }

//                //        }

//                //        mi.clipAnimations = animClips.ToArray();
//                //    }
//                //    catch (System.Exception ex)
//                //    {
//                //        Debug.LogError(ex.Message);
//                //    }
//                //    }
//            }


//    }
//}
