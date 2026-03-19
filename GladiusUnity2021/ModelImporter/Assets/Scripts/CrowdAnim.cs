using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrowdAnim : MonoBehaviour
{
    public bool RandomiseAnimationStart;

    public TextAsset IdleAnimTA;
    public TextAsset ExcitedAAnimTA;
    public TextAsset ExcitedBAnimTA;
    public TextAsset BooAAnimTA;
    public TextAsset BooBAnimTA;
    public TextAsset ThrowAnimTA;

    public AnimationData IdleAnim;
    public AnimationData ExcitedAAnim;
    public AnimationData ExcitedBAnim;
    public AnimationData BooAAnim;
    public AnimationData BooBAnim;
    public AnimationData ThrowAnim;

    private CrowdState m_crowdState = CrowdState.None;
    private AnimationData m_currentAnimation;
    public CrowdState CrowdState
    {
        get { return m_crowdState; }
        set
        {
            if (value != CrowdState)
            {
                if (value == CrowdState.Idle)
                {
                    m_currentAnimation = IdleAnim;
                }
                else if (value == CrowdState.ExcitedA)
                {
                    m_currentAnimation = ExcitedAAnim;
                }
                else if (value == CrowdState.ExcitedB)
                {
                    m_currentAnimation = ExcitedBAnim;
                }
                else if (value == CrowdState.BooA)
                {
                    m_currentAnimation = BooAAnim;
                }
                else if (value == CrowdState.BooB)
                {
                    m_currentAnimation = BooBAnim;
                }
                else if (value == CrowdState.Throw)
                {
                    m_currentAnimation = ThrowAnim;
                }

                SetAnim();

            }
        }
    }



    public GladiusSimpleAnim GladiusAnim
    {
        get;
        set;
    }

    public void AddAnimationData(TextAsset textAsset)
    {
        if(textAsset != null)
        {
            CrowdState crowdState = CrowdState.None;
            //if(textAsset.name.EndsWith("_excited") || textAsset.name.EndsWith("_exciteda"))
            if (textAsset.name.EndsWith("_exciteda.pan") || textAsset.name.EndsWith("_excited.pan"))
            {
                ExcitedAAnimTA = textAsset;
            }
            else if (textAsset.name.EndsWith("_excitedb.pan"))
            {
                ExcitedBAnimTA = textAsset;
            }
            else if (textAsset.name.EndsWith("_booa.pan"))
            {
                BooAAnimTA = textAsset;
            }
            else if (textAsset.name.EndsWith("_boob.pan"))
            {
                BooBAnimTA = textAsset;
            }
            else if (textAsset.name.EndsWith("_throw.pan"))
            {
                ThrowAnimTA = textAsset;
            }
            else if (textAsset.name.EndsWith("_idle.pan"))
            {
                IdleAnimTA = textAsset;
            }
            else
            {
                // sometimes there is only an anim asset with a the model name
                IdleAnimTA = textAsset;
            }
        }

    }

    // Use this for initialization
    void Awake()
    {
        GladiusAnim = new GladiusSimpleAnim();
        GladiusAnim.Init(transform);
        IdleAnim = GladiusAnim.AddAnim("idle",IdleAnimTA);
        ExcitedAAnim = GladiusAnim.AddAnim("exciteda", ExcitedAAnimTA);
        ExcitedBAnim = GladiusAnim.AddAnim("excitedb", ExcitedBAnimTA);
        BooAAnim = GladiusAnim.AddAnim("booa", BooAAnimTA);
        BooBAnim = GladiusAnim.AddAnim("boob", BooBAnimTA);
        ThrowAnim = GladiusAnim.AddAnim("throw", ThrowAnimTA);
        CrowdState = CrowdState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (GladiusAnim != null)
        {
            if (GladiusAnim.CurrentAnimation == null && GladiusAnim.m_allAnimations.Count > 0)
            {
                GladiusAnim.CurrentAnimation = m_currentAnimation;
                if (GladiusAnim.CurrentAnimation != null)
                {
                    GladiusAnim.CurrentAnimation.mFlags |= AnimationData.ANIM_LOOP;
                    if (RandomiseAnimationStart)
                    {
                        GladiusAnim.CurrentAnimation.RandomAnimTIme();
                    }
                }
            }
            GladiusAnim.Update();
        }
    }

    private void SetAnim()
    {
        if (GladiusAnim != null)
        {
            GladiusAnim.CurrentAnimation = m_currentAnimation;
            if (GladiusAnim.CurrentAnimation != null)
            {
                GladiusAnim.CurrentAnimation.mFlags |= AnimationData.ANIM_LOOP;
                if (RandomiseAnimationStart)
                {
                    GladiusAnim.CurrentAnimation.RandomAnimTIme();
                }
            }
        }
    }

}


