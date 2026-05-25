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
        public float CumulativeTime = 0f;
        
        public int AnimationRate = 30;
        public float FrameDuration = 0;
        
        public List<float> AnimationTimes = new List<float>();
        public List<Transform> TransformList = new List<Transform>();
        public Dictionary<Transform, List<(float,Vector3)>> BonePositionData = new Dictionary<Transform, List<(float,Vector3)>>();
        public Dictionary<Transform, List<(float,Quaternion)>> BoneRotationData = new Dictionary<Transform, List<(float,Quaternion)>>();

        public bool DataWritten = false;

        
        public void Awake()
        {
            BuildBoneData(RootBone);
            Time = 0f;
            CumulativeTime = 0f;
            FrameDuration = (1f / (float)AnimationRate);
            
            
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
            CumulativeTime += UnityEngine.Time.deltaTime;
            if (CumulativeTime >= FrameDuration)
            {
                CumulativeTime = 0f;
                AnimatorStateInfo currentInfo = Animator.GetCurrentAnimatorStateInfo(0);
                if(!DataWritten)
                {
                    Time += FrameDuration;
                    AnimationTimes.Add(Time);

                    foreach (Transform t in BonePositionData.Keys)
                    {
                        BonePositionData[t].Add((Time, t.localPosition));
                    }

                    foreach (Transform t in BoneRotationData.Keys)
                    {
                        BoneRotationData[t].Add((Time, t.localRotation));
                    }
                    
                    if(currentInfo.normalizedTime >= 1.0f)
                    {
                        using (TextWriter tw = new StreamWriter(DebugOutputPath))
                        {
                            for (int i = 0; i < AnimationTimes.Count; i++)
                            {
                                foreach (Transform t in TransformList)
                                {
                                    tw.WriteLine(
                                        $"{t.name} lp ({BonePositionData[t][i]})   lr ({BoneRotationData[t][i]})");
                                }
                            }
                        }

                        using (BinaryWriter bw = new BinaryWriter(File.Open(OutputPath, FileMode.Create)))
                        {
                            AnimationUtils.WriteDataAsPAN(bw, RootBone, TransformList, AnimationTimes, BonePositionData,
                                BoneRotationData);
                        }

                        GladiusSimpleAnim simpleAnim = new GladiusSimpleAnim();
                        AnimationData druidData = null;
                        AnimationData barbarianData = null;
                        AnimationData urlanPosRotData = null;

                        // now try and load that as a gladius anim.
                        using (BinaryReader binReader = new BinaryReader(new FileStream(OutputPath, FileMode.Open)))
                        {
                            druidData = AnimationLoader.ReadSingleAnimationFile("", simpleAnim, binReader);
                        }


                        using (BinaryReader binReader = new BinaryReader(
                                   new FileStream("Assets/GladiusAnims/barbarian/barbarian_idle.pan.bytes",
                                       FileMode.Open)))
                        {
                            barbarianData = AnimationLoader.ReadSingleAnimationFile("", simpleAnim, binReader);
                        }

                        using (BinaryReader binReader = new BinaryReader(
                                   new FileStream("Assets/GladiusAnims/urlan/urlan_act2axeswingbr.pan.bytes",
                                       FileMode.Open)))
                        {
                            urlanPosRotData = AnimationLoader.ReadSingleAnimationFile("", simpleAnim, binReader);
                        }


                        DataWritten = true;

                    }
                }

            }
        }
    }
}