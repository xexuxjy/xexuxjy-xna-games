using System.Collections.Generic;
using System.IO;
using UnityEditor.Animations;
using UnityEngine;

namespace Gladius.util.Test
{
    public class TestCreatePAN : MonoBehaviour
    {
        public AnimationClip AnimationClip;
        public string OutputPath;
        public string DebugOutputPath;
        public Transform RootBone;

        public Animator Animator;
        public string AnimationState;

        public float Time;
        public List<float> AnimationTimes = new List<float>();
        public List<Transform> TransformList = new List<Transform>();
        public Dictionary<Transform, List<(float,Vector3)>> BonePositionData = new Dictionary<Transform, List<(float,Vector3)>>();
        public Dictionary<Transform, List<(float,Quaternion)>> BoneRotationData = new Dictionary<Transform, List<(float,Quaternion)>>();

        public bool DataWritten = false;

        
        public void Awake()
        {
            // if (AnimationClip != null)
            // {
            //     GladiusAnimationClip gladiusAnimationClip = new GladiusAnimationClip(AnimationClip);
            //     int ibreak = 0;
            //     // using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(OutputPath, FileMode.Create)))
            //     // {
            //     //     AnimationUtils.WriteDataAsPAN(binaryWriter, RootBone, gladiusAnimationClip);
            //     //     binaryWriter.Flush();
            //     // }
            // }

            BuildBoneData(RootBone);
            Time = 0f;
            //Animation.Play(AnimationState);
            Animator.Play(AnimationState);
        }

        void BuildBoneData(Transform t)
        {
            TransformList.Add(t);
            BonePositionData.Add(t, new List<(float,Vector3)>());
            BoneRotationData.Add(t, new List<(float,Quaternion)>());
            foreach (Transform child in t)
            {
                BuildBoneData(child);
            }
        }

        public void Update()
        {
            AnimatorStateInfo currentInfo = Animator.GetCurrentAnimatorStateInfo(0); 
            if(currentInfo.normalizedTime < 1f)
            {
                Time += 1f / 30f;
                AnimationTimes.Add(Time);
                
                foreach (Transform t in BonePositionData.Keys)
                {
                    BonePositionData[t].Add((Time,t.localPosition));
                }

                foreach (Transform t in BoneRotationData.Keys)
                {
                    BoneRotationData[t].Add((Time,t.localRotation));
                }
            }

            if (currentInfo.normalizedTime >= 1f)
            {
                if (!DataWritten)
                {
                    using (TextWriter tw = new StreamWriter(DebugOutputPath))
                    {
                        for (int i = 0; i < AnimationTimes.Count; i++)
                        {
                            foreach (Transform t in TransformList)
                            {
                                tw.WriteLine($"{t.name} lp ({BonePositionData[t][i]})   lr ({BoneRotationData[t][i]})");
                            }
                        }
                    }

                    using (BinaryWriter bw = new BinaryWriter(File.Open(OutputPath, FileMode.Create)))
                    {
                        AnimationUtils.WriteDataAsPAN(bw, RootBone, TransformList, AnimationTimes, BonePositionData,
                            BoneRotationData);
                    }

                    DataWritten = true;

                }
            }

        }
    }
}