using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimComparer : MonoBehaviour
{
	public GameObject LHS;
	public GameObject RHS;
	public String LHSDirectory = "e:/tmp/animcomparer/lhs/";
	public String RHSDirectory = "e:/tmp/animcomparer/rhs/";
	
	public int frameCount = 0;

    public void Start()
    {
        if (!Directory.Exists(LHSDirectory))
        {
            Directory.CreateDirectory(LHSDirectory);
        }

        if (!Directory.Exists(RHSDirectory))
        {
            Directory.CreateDirectory(RHSDirectory);
        }

        using (StreamWriter sw = new StreamWriter(LHSDirectory + "Skeleton"))
        {
            SkinnedMeshRenderer[] smra = LHS.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < smra.Length; ++i)
            {
                sw.WriteLine(String.Format("[{0}]", smra[i].sharedMesh.name));
                Matrix4x4[] bindPoses = smra[i].sharedMesh.bindposes;
                for (int j = 0; j < bindPoses.Length; ++j)
                {
                    sw.WriteLine(String.Format("[{0}] [{1}]", j, bindPoses[j]));
                }
                sw.WriteLine("------------------------------------------");
            }
        }

        using (StreamWriter sw = new StreamWriter(RHSDirectory + "Skeleton"))
        {
            SkinnedMeshRenderer[] smra = RHS.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < smra.Length; ++i)
            {
                sw.WriteLine(String.Format("[{0}]", smra[i].sharedMesh.name));
                Matrix4x4[] bindPoses = smra[i].sharedMesh.bindposes;
                for (int j = 0; j < bindPoses.Length; ++j)
                {
                    sw.WriteLine(String.Format("[{0}] [{1}]", j, bindPoses[j]));
                }
                sw.WriteLine("------------------------------------------");
            }
        }

    }

    public void Update()
	{
		frameCount++;

        if (frameCount > 30)
        {
            return;
        }

        using (StreamWriter sw = new StreamWriter(LHSDirectory+frameCount))
		{
			DumpTransformData(LHS,LHS.transform,sw);
		}
		using(StreamWriter sw = new StreamWriter(RHSDirectory+frameCount))
		{
			DumpTransformData(RHS,RHS.transform,sw);
		}
	}

    public void DumpTransformData(GameObject go, Transform t, StreamWriter sw)
    {

        float animTime = 0.0f;
        GladiusAnim gladiusAnim = go.GetComponent<GladiusAnim>();
        if (gladiusAnim != null)
        {
            if (gladiusAnim.m_currentAnimation != null)
            {
                animTime = gladiusAnim.m_currentAnimation.mAnimTime;
            }
            //BoneAnimData bad = gladiusAnim.m_orderedBoneList.Find(x => x.gameObjectTranform == t);
            //if (bad != null)
            //{
            //    sw.WriteLine(String.Format("[{0}] p[{1}] r[{2}] re[{3}] badPos[{4}] badRot[{5}] [{6}]", t.name, t.localPosition, t.localRotation, t.localEulerAngles, bad.CurrentPosition, bad.CurrentRotation, bad.gameObjectTranform.name));
            //}
        }
        else
        {
            Animation anim = go.GetComponent<Animation>();
            foreach (AnimationState animState in anim)
            {
            //    animState.time = (frameCount-1) * 1.0f / 30.0f;
                animTime = animState.time;
            }
        }
        if(true)
        {
            if (t.name.StartsWith("head"))
            {
                sw.WriteLine(String.Format("[{0}][{1:0.000}] p[{2:0.00000},{3:0.00000},{4:0.00000}] r[{5:0.00000},{6:0.00000},{7:0.00000},{8:0.00000}] re[{9}]", t.name, animTime,t.localPosition.x,t.localPosition.y,t.localPosition.z, t.localRotation.x,t.localRotation.y,t.localRotation.z,t.localRotation.w, t.localEulerAngles));
            }
        }
		foreach (Transform child in t)
        {
        	DumpTransformData(go,child,sw);
        }
	}

}