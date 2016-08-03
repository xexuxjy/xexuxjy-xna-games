using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimStartup : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        //string assetPath = "Assets/Resources/GladiusModels/Characters/amazon.fbx";
        //UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        //Debug.Log("animStartup-assetPath=" + assetPath);
        //Debug.Log("animStartup-found " + objs.Length);

        //foreach (UnityEngine.Object ao in objs)
        //{
        //    if (ao.GetType() == typeof(AnimationClip))
        //    {
        //        Debug.Log("animStartup-clip=" + ao.name);
        //    }
        //}

        //Animation anim = gameObject.AddComponent<Animation>();
        //anim.AddClip
        //RuntimeAnimatorController animController = Resources.Load("AnimationControllers/PanAnimationController") as RuntimeAnimatorController;

        //GetComponent<Animator>().runtimeAnimatorController = animController;
        //animController.clips = mi.cl
        //ModelImporterClipAnimation mica;
        //        RuntimeAnimatorController rac = GetComponent<Animator>().runtimeAnimatorController;
        //        AnimatorOverrideController aoc = new AnimatorOverrideController();
        //        aoc.runtimeAnimatorController = rac;
        //        GetComponent<Animator>().runtimeAnimatorController = aoc;

        ////        Animation anim = GetComponent<Animation>();
        //        AnimationClip[] originals = aoc.animationClips;

        //        AnimationClipPair[] animClipPairs = new AnimationClipPair[s_animNames.Length];

        //        //AnimationClip[] modelClips = Resources.LoadAll<AnimationClip>("ModelPrefabs/amazonPrefab");
        //        //AnimationClip[] modelClips = Resources.LoadAll<AnimationClip>("GladiusModels/Characters/amazon");
        //       // AnimationClip[] internalClips = gameObject.GetComponentsInChildren<AnimationClip>();

        //        for (int i = 0; i < animClipPairs.Length; ++i)
        //        {
        //            AnimationClip original = originals[i];

        //            AnimationClip overriden = anim.GetClip(s_animNames[i]);

        //            AnimationClipPair acp = new AnimationClipPair();
        //            acp.originalClip = original;
        //            acp.overrideClip = overriden;
        //            animClipPairs[i] = acp;
        //        }
        //        aoc.clips = animClipPairs;

    }

    // Update is called once per frame
    void Update () {
	
	}

    public static string[] s_animNames = new string[] {"idle.pan",
"idleengaged.pan",
"idlewounded.pan",
"idlepassive.pan",
"idledeath.pan",
"death.pan",
"reactvictory.pan",
"reactloss.pan",
"reacthitweakf",
"reacthitstrongl",
"reacthitstrongb",
"reacthitstrongr",
"reacthitstrongf",
"moverun.pan",
"moveclimbhalf.pan",
"movejumpdownhalf.pan",
"reactdowned.pan",
"reactknockdownb.pan",
"movegetup.pan",
"idleknockdown.pan",
"reactknockback.pan" };

}
