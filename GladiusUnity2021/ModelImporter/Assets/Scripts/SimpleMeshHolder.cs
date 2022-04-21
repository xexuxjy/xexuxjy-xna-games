using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMeshHolder : MonoBehaviour
{
    public TextAsset AnimationFile;
    public bool RandomiseAnimationStart;

    public GladiusSimpleAnim GladiusAnim
    {
        get;
        set;
    }

    // Use this for initialization
    void Awake()
    {
        if (AnimationFile != null)
        {
            GladiusAnim = new GladiusSimpleAnim();
            GladiusAnim.Init(transform);
            GladiusAnim.AddAnim("",AnimationFile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GladiusAnim != null)
        {
            if (GladiusAnim.CurrentAnimation == null && GladiusAnim.m_allAnimations.Count > 0)
            {
                GladiusAnim.CurrentAnimation = GladiusAnim.m_allAnimations[0];
                GladiusAnim.CurrentAnimation.mFlags |= AnimationData.ANIM_LOOP;
                if (RandomiseAnimationStart)
                {
                    GladiusAnim.CurrentAnimation.RandomAnimTIme();
                }
            }
            GladiusAnim.Update();
        }
    }
}
