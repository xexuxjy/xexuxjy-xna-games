using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static AnimationLoader;

public class AnimationLifetimeEvent : EventArgs
{
    public AnimationLifetimeEvent(String name)
    {
        Name = name;
    }
    public String Name { get; set; }
}

public class GladiusSimpleAnim
{
    public void Init(Transform ownerTransform)
    {
        m_ownerTransform = ownerTransform;
        Transform characterTransform = ownerTransform.FuzzyFindChild("character");
        // none character transforms.
        if (characterTransform == null)
        {
            characterTransform = ownerTransform.FuzzyFindChild("root");
        }
        if (characterTransform == null)
        {
            characterTransform = ownerTransform.transform.GetChild(0);
        }

            BuildInitialMap(null, characterTransform);

        m_orderedBoneList.Sort((lhs, rhs) => Comparer<int>.Default.Compare(lhs.boneId, rhs.boneId));

        BuildChildBoneLists();
        CurrentPlayInfo.PlayRate = 1.0f;
    }

    public virtual void Update()
    {
        //if(FixedRate)
        //{
        //    AnimationSpeed = FixedAnimFrameRate;
        //}
        //else
        //{
        //    AnimationSpeed = Time.deltaTime;
        //}
        UpdateAnimationTracks();
    }

    public void BuildInitialMap(BoneAnimData baParent, Transform transform)
    {
        // slightly ugly way of stopping attachments being added as animated bone nodes
        if (transform.name.Contains("--"))
        {
            BoneAnimData bad = new BoneAnimData(this, baParent, transform.name, transform);
            m_orderedBoneList.Add(bad);

            foreach (Transform child in transform)
            {
                BuildInitialMap(bad, child);
            }
        }
    }

    protected void BuildChildBoneLists()
    {
        Dictionary<BoneAnimData, List<BoneAnimData>> nodeChildrenMap = new Dictionary<BoneAnimData, List<BoneAnimData>>();
        foreach (BoneAnimData bad in m_orderedBoneList)
        {
            if (bad.parentBad != null)
            {
                List<BoneAnimData> parentList;
                if (!nodeChildrenMap.TryGetValue(bad.parentBad, out parentList))
                {
                    parentList = new List<BoneAnimData>();
                    nodeChildrenMap[bad.parentBad] = parentList;
                }
                parentList.Add(bad);
            }
        }

        foreach (BoneAnimData bad in m_orderedBoneList)
        {
            List<BoneAnimData> childList;
            if (nodeChildrenMap.TryGetValue(bad, out childList))
            {
                bad.childBads = childList;
            }
        }
    }


    public virtual void UpdateAnimationTracks()
    {
        if (CurrentAnimation != null)
        {
            //m_elapsed += Time.deltaTime * AnimationSpeed;
            //if (m_elapsed > AnimFrameRate)
            {
                //m_elapsed -= AnimFrameRate;
                DoAnimation();
            }
        }
    }


    public void DoAnimation()
    {
        foreach (BoneAnimData bad in m_orderedBoneList)
        {
            bad.Reset();
        }

        if (CurrentAnimation != null)
        {
            CurrentAnimation.AnimateTracks(CurrentPlayInfo.PlayRate);
        }

        foreach (BoneAnimData bad in m_orderedBoneList)
        {
            bad.ApplyCurrent();
        }

    }

    public AnimationData CurrentAnimation
    {
        get
        {
            return m_currentAnimation;
        }
        set
        {
            if (m_currentAnimation != null)
            {
                m_currentAnimation.Reset();
            }
            m_currentAnimation = value;
        }
    }

    public AnimationData AddAnim(String animName,TextAsset textAsset)
    {
        AnimationData newAnimation = null;
        if (textAsset != null)
        {
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(textAsset.bytes)))
            {
                newAnimation = AnimationLoader.ReadSingleAnimationFile("", this, binReader);
                if (newAnimation != null)
                {
                    newAnimation.AssignModelAndSkeleton(this, m_ownerTransform);
                    if (!String.IsNullOrEmpty(animName))
                    {
                        newAnimation.name = animName;
                    }
                    m_allAnimations.Add(newAnimation);
                    m_animationMap.Add(newAnimation.name, newAnimation);
                }
            }
        }
        return newAnimation;
    }

    public const float NOTAG_DEFAULTTIME = 0.15f;

    public float ActorAnimGetTrackTagPoint(string animName, string tagName)
    {
        uint TagMask;
        float TagTime;
        AnimationData animationData;
        if(m_animationMap.TryGetValue(animName,out animationData))
        {
		    TagMask = animationData.GetEventMask(tagName);
		    if (TagMask != 0x00000000) 
            {
			    TagTime=animationData.FindFirstEvent(TagMask);
			    if (TagTime >= 0.0f) 
                {
				    return(TagTime);
			    }
		    }
	    }
        return (NOTAG_DEFAULTTIME);
    }



    protected Transform m_ownerTransform;

    public List<BoneAnimData> m_orderedBoneList = new List<BoneAnimData>();

    //protected float m_elapsed;
    public const float FixedAnimFrameRate = 1.0f / 30.0f;

    public float AnimationSpeed = 1.0f;
    protected float nextAnimBlendFactor = 0.0f;
    public float m_animationBlend = 0.2f;

    public bool ForceLoop = false;
    public bool FixedRate = true;

    public AnimPlayInfo CurrentPlayInfo;

    protected AnimationData m_currentAnimation;
    public List<AnimationData> m_allAnimations = new List<AnimationData>();
    public Dictionary<String, AnimationData> m_animationMap = new Dictionary<String, AnimationData>();



}



public class GladiusCharacterAnim : GladiusSimpleAnim
{
    // unity properties
    public string Pak1File;


#if DEBUG
    public FixedSizedQueue<string> AnimationStarts = new FixedSizedQueue<string>(10);
    public FixedSizedQueue<string> AnimationStops = new FixedSizedQueue<string>(10);
    public FixedSizedQueue<string> LastAnims = new FixedSizedQueue<string>(10);
#endif


    public String ExtraAnimDirectory = "";

    private ActorClassDef m_actorClassDef;

    public delegate void AnimationEventArgs(object sender, AnimationLifetimeEvent e);

    public event AnimationEventArgs AnimationStarted;
    public event AnimationEventArgs AnimationEnded;
    public event AnimationEventArgs AnimationLooped;

    public void Init(Transform ownerTransform, ActorClassDef actorClassDef)
    {
        base.Init(ownerTransform);
        m_actorClassDef = actorClassDef;

        String replacedName = actorClassDef.name;
        replacedName += ".pak1";
        Pak1File = replacedName;

        //TextAsset textAsset = Resources.Load<TextAsset>(GladiusGlobals.GetFileName("GladiusAnims/" + Pak1File));
        //if (textAsset != null)
        //{
        //    using (BinaryReader binReader = new BinaryReader(new MemoryStream(textAsset.bytes)))
        //    {
        //        AnimationLoader.ReadPak1File(binReader, this);
        //        foreach (AnimationData animationData in m_allAnimations)
        //        {
        //            animationData.AssignModelAndSkeleton(this, ownerTransform);
        //        }
        //    }
        //}

        if (String.IsNullOrEmpty(ExtraAnimDirectory))
        {
            ExtraAnimDirectory = m_actorClassDef.mesh;
        }

        if (!String.IsNullOrEmpty(ExtraAnimDirectory))
        {
            TryAddAnimDirectory(ExtraAnimDirectory);
        }


        FixAnimationMovement();

        m_allAnimations.Sort((x, y) => x.TrueLength.CompareTo(y.TrueLength));

    }

    public void LoadAnimations()
    {

    }


    public override void Update()
    {
        CheckForNewAnimation();
        base.Update();
    }


    public void FixAnimationMovement()
    {
        AnimationData moveRunAnim = null;
        AnimationData climbUpAnim = null;
        AnimationData jumpDownAnim = null;
        m_animationMap.TryGetValue(MoveRun, out moveRunAnim);
        m_animationMap.TryGetValue(MoveClimbHalfUp, out climbUpAnim);
        m_animationMap.TryGetValue(MoveJumpDownHalf, out jumpDownAnim);

        if (moveRunAnim != null && moveRunAnim.mTranslation == 0.0f)
        {
            float totalTime = moveRunAnim.TrueLength;
            float trans = 2.0f / totalTime;

            trans = 2.0f;//BaseActor.MovementSpeed;

            //moveRunAnim.mTranslation = trans;
        }
        if (climbUpAnim != null && moveRunAnim.mTranslation == 0.0f)
        {
            //climbUpAnim.mTranslation = moveRunAnim.mTranslation;
        }
        if (moveRunAnim != null && jumpDownAnim != null)
        {
            //jumpDownAnim.mTranslation = moveRunAnim.mTranslation;
        }
    }

    public void TryAddAnimDirectory(String name)
    {
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(GladiusGlobals.GetFileName("GladiusAnims/" + name));
        if (textAssets != null)
        {
            foreach (TextAsset ta in textAssets)
            {
                if (ta != null)
                {
                    using (BinaryReader binReader = new BinaryReader(new MemoryStream(ta.bytes)))
                    {
                        AnimationData newAnimation = AnimationLoader.ReadSingleAnimationFile(ta.name, this, binReader);
                        if (newAnimation != null)
                        {
                            newAnimation.AssignModelAndSkeleton(this, m_ownerTransform);
                            if (ForceLoop)
                            {
                                newAnimation.mFlags |= AnimationData.ANIM_LOOP;
                            }
                            m_allAnimations.Add(newAnimation);
                            m_animationMap.Add(newAnimation.name, newAnimation);
                        }
                    }
                }
            }
        }
    }


    public bool TryAddSingleAnim(String animName)
    {
        bool success = false;
        if (!String.IsNullOrEmpty(animName))
        {
            TextAsset textAsset = Resources.Load<TextAsset>(GladiusGlobals.GetFileName("GladiusAnims/" + animName));
            if (textAsset != null)
            {
                success = (AddAnim(animName,textAsset) != null);
            }

        }
        return success;
    }



    private List<AnimPlayInfo> m_animationQueue = new List<AnimPlayInfo>();
    public String CurrentAnimName
    { get { return CurrentAnimation != null ? CurrentAnimation.name : null; } }

    public void QueueAnimation(AnimPlayInfo animPlayInfo)
    {
        if(HasAnimation(animPlayInfo.Name))
        {
            if (CurrentAnimName != animPlayInfo.Name && (m_animationQueue.Count == 0 || m_animationQueue[m_animationQueue.Count - 1].Name != animPlayInfo.Name))
            {
                m_animationQueue.Add(animPlayInfo);
            }
        }
    }

    public void QueueAnimation(String anim)
    {
        AnimPlayInfo newAnim = new AnimPlayInfo(anim, 0, 1.0f, 1.0f,false);
        QueueAnimation(newAnim);
    }

    public void CheckForNewAnimation()
    {
        if (CurrentAnimation == null)
        {
            if (m_animationQueue.Count > 0)
            {
                AnimPlayInfo nextAnim = m_animationQueue[0];
                m_animationQueue.RemoveAt(0);
                PlayAnimation(nextAnim);
            }
        }
        else if (CurrentAnimation != null)
        {
            if (CurrentAnimation.IsLooping)
            {
                if (CurrentAnimation.DidLoop)
                {
                    NotifyAnimationLooped(CurrentAnimation.name);
                }

                // looping but have something to do.	
                if (m_animationQueue.Count > 0)
                {
                    StopAnimation();
                }
            }
            else if (CurrentAnimation.Complete)
            {
                StopAnimation();
            }
        }

    }

    //public override void UpdateAnimationTracks()
    //{
    //    if (CurrentAnimation != null)
    //    {
    //        //m_elapsed += Time.deltaTime * AnimationSpeed;
    //        //if (m_elapsed > AnimFrameRate)
    //        {
    //            //m_elapsed -= AnimFrameRate;
    //            DoAnimation();
    //        }
    //    }
    //}


    public void StopAnimation()
    {
        if (CurrentAnimation != null)
        {
            string animName = CurrentAnimation.name;
            CurrentAnimation = null;
            NotifyAnimationStopped(animName);
        }
    }

    public bool HasAnimation(String anim)
    {
        return FetchAnimation(anim) != null;
    }

    public void PlayAnimation(string name)
    {
        PlayAnimation(new AnimPlayInfo(name, 0, 1.0f, 0.0f,false));
    }

    public void PlayAnimation(AnimPlayInfo animInfo)
    {
        if (animInfo.Name != CurrentAnimName)
        {
            try
            {
                if (Play(animInfo))
                {
                    NotifyAnimationStarted(CurrentAnimName);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("PlayAnimation : " + animInfo.Name);
                Debug.LogWarning(e.StackTrace);
            }
        }
    }

    public void NotifyAnimationStarted(String anim)
    {
        AnimationStarts.Enqueue(anim);

        if (AnimationStarted != null)
        {
            AnimationStarted(this, new AnimationLifetimeEvent(anim));
        }
    }

    public void NotifyAnimationStopped(String anim)
    {
        AnimationStops.Enqueue(anim);
        if (AnimationEnded != null)
        {
            AnimationEnded(this, new AnimationLifetimeEvent(anim));
        }
    }

    public void NotifyAnimationLooped(String anim)
    {
        if (AnimationLooped != null)
        {
            AnimationLooped(this, new AnimationLifetimeEvent(anim));
        }
    }


    public BoneAnimData GetChildAtDepth(BoneAnimData bad, int depth)
    {
        if (depth == 0 && bad.childBads.Count > 0)
        {
            return bad.childBads[0];
        }
        else if (depth > 0 && bad.childBads.Count > 0)
        {
            return GetChildAtDepth(bad.childBads[0], depth - 1);
        }
        return null;
    }

    public int CountChildDepth(BoneAnimData bad)
    {
        int chainLength = 0;
        // check if all children are same length?
        List<BoneAnimData> children = bad.childBads;
        while (children.Count > 0)
        {
            chainLength++;
            children = children[0].childBads;
        }
        return chainLength;
    }



    public float CurrentZeroYOffset = 0.0f;


    public void AddAnimationData(AnimationData animationData)
    {
        animationData.AssignModelAndSkeleton(this, m_ownerTransform);
        m_allAnimations.Add(animationData);
        if (!m_animationMap.ContainsKey(animationData.name))
        {
            m_animationMap.Add(animationData.name, animationData);
        }
        else
        {
            Debug.LogWarningFormat("[{0}] already have anim [{1}].", m_ownerTransform.name, animationData.name);
        }
    }

    private AnimationData FetchAnimation(String animationName)
    {
        AnimationData animData = null;
        if (m_animationMap.TryGetValue(animationName, out animData))
        {
        }
        else if (TryAddSingleAnim(animationName))
        {
            // see if it's not a preloaded one and try and get it.
            m_animationMap.TryGetValue(animationName, out animData);
        }
        else
        {
            Debug.LogErrorFormat("[{0}] asked to play anim [{1}] but doesn't exist.", m_ownerTransform.name, animationName);

        }
        return animData;
    }

    public bool Play(AnimPlayInfo animPlayInfo)
    {
        AnimationData animData = FetchAnimation(animPlayInfo.Name);
        if (animData != null)
        {
            //LastAnims.Enqueue(animPlayInfo.Name);
            CurrentAnimation = animData;
            CurrentPlayInfo = animPlayInfo;
            CurrentPlayInfo.CalcPlayRate(animData);
            if(animPlayInfo.Loop)
            {
                CurrentAnimation.mFlags |= AnimationData.ANIM_LOOP;
            }
            else
            {
                CurrentAnimation.mFlags &= ~AnimationData.ANIM_LOOP;
            }

            if (CurrentPlayInfo.Speed < 0)
            {
                float animLengthHit = ActorAnimGetTrackTagPoint(CurrentAnimation.name, "hit");
                float animLengthReact = ActorAnimGetTrackTagPoint(CurrentAnimation.name, "  react");

                float animLength = (animLengthHit != NOTAG_DEFAULTTIME) ? animLengthHit : animLengthReact;

                animLength -= animPlayInfo.TimeStart;
                float speed = animLength / -(CurrentPlayInfo.Speed);
                if (speed < 0)
                {
                    int ibreak = 0;
                    speed = 1;
                }

                CurrentPlayInfo.Speed = speed;
            }
            CurrentAnimation.Start(CurrentPlayInfo.TimeStart,CurrentPlayInfo.Speed,CurrentPlayInfo.PlayRate);
        }
        return animData != null;
    }

    public bool IsPlaying
    {
        get
        {
            return CurrentAnimation != null ? CurrentAnimation.IsPlaying : false;
        }
    }


    public bool IsPlayingLoop
    {
        get
        {
            return IsPlaying && CurrentAnimation.IsLooping;
        }
    }




    public const string Idle = "idle";
    public const string IdleEngaged = "idleengaged";
    public const string IdleWounded = "idlewounded";
    public const string IdleKnockdown = "idleknockdown";
    public const string IdleDeath = "idledeath";
    public const string MoveClimbHalfUp = "moveclimbhalf";
    public const string MoveJumpDownHalf = "movejumpdownhalf";
    public const string MoveRun = "moverun";
    public const string GetUp = "movegetup";
    public const string KnockBack = "reactknockback";
    public const string KnockDown = "reactknockdownb";
    public const string Defend = "reactdefend";
    public const string Evade = "reactevade";
    public const string Victory= "reactvictory";
    public const string Defeat = "reactdefeat";
    public const string ChangeFormInto = "changeforminto";
    public const string ChangeFormAway = "changeformaway";
    public const string Death = "death";
}


