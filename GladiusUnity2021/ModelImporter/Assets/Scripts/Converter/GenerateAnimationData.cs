// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEngine;
// public class GenerateAnimationData : MonoBehaviour
// {
//
//     [MenuItem("GladiusGameData/GenerateAnimationData")]
//     private static void GenerateAssets()
//     {
//         string animationAssetName = "Assets/animations/amazon/amazon_idle.pan.bytes";
//         string characterAssetName = "Assets/XBoxModelPrefabs/characters/amazon/amazon.mdl.prefab";
//
//         AnimationClip clip = new AnimationClip();
//         clip.legacy = false;
//
//         GameObject characterInstance = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(characterAssetName));
//
//         GladiusSimpleAnim simpleAnim = new GladiusSimpleAnim();
//
//         simpleAnim.Init(characterInstance.transform);
//
//         OriginalAnimationLoader.CreateKeyFrames = true;
//
//
//         TextAsset assetData = AssetDatabase.LoadAssetAtPath<TextAsset>(animationAssetName);
//         if (assetData != null)
//         {
//             using(BinaryReader binReader = new BinaryReader(new MemoryStream(assetData.bytes)))
//             {
//                 List<BoneAnimData>  orderedBoneList = simpleAnim.m_orderedBoneList;//new List<BoneAnimData>();      
//                 AnimationData animData = OriginalAnimationLoader.ReadSingleAnimationFile("",simpleAnim,binReader);
//                 animData.AssignModelAndSkeleton(simpleAnim,characterInstance.transform);
//                 animData.Start(0,1.0f,1.0f);
//
//                 foreach(BoneAnimData boneAnimData in orderedBoneList)
//                 {
//                     boneAnimData.SetupAnimCurves();
//                 }
//
//                 float timeDelta = 1.0f/24;
//
//
//                 while(!animData.Complete)
//                 {
//                     foreach (BoneAnimData bad in orderedBoneList)
//                     {
//                         bad.Reset();
//                     }
//
//                     animData.AnimateTracksSpecificDelta(timeDelta,1.0f);
//                 }
//
//                 foreach(BoneAnimData boneAnimData in orderedBoneList)
//                 {
//                     boneAnimData.CopyCurvesToClip(clip);
//                 }
//
//             }
//         }
//
//         DestroyImmediate(characterInstance);
//
//         AssetDatabase.CreateAsset(clip, "Assets/test-amazon-anim.asset");
//
//
//     }
// }