    using System;
    using System.IO;
    using UnityEngine;

    public class TestAnimationClip : MonoBehaviour
    {
        public AnimationClip AnimationClip;
        public TextAsset GladiusAnim;
        private GladiusSimpleAnim simpleAnim; 


        public void Awake()
        {
            if (AnimationClip != null)
            {
                GladiusAnimationClip gladiusAnimationClip = new GladiusAnimationClip(AnimationClip);
                int ibreak = 0;
            }

            if (GladiusAnim != null)
            {
                simpleAnim = new GladiusSimpleAnim();
                simpleAnim.AddAnim("test", GladiusAnim);
                int ibreak = 0;

                byte[] originalData = GladiusAnim.bytes;
                byte[] rewrittenData = null;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                    {
                        AnimationUtils.WriteData(binaryWriter, simpleAnim);
                        binaryWriter.Flush();
                    }

                    rewrittenData = memoryStream.ToArray();
                }

                Common.ByteArrayToFile("d:/tmp/original-anim.bytes", originalData);
                Common.ByteArrayToFile("d:/tmp/rewritten-anim.bytes", rewrittenData);
                
            }
            
        }
    }
