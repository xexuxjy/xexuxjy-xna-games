using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GladiusAnimationClip
{
    public AnimationClip m_animationClip;
    public int m_numEvents = 0;

    public List<string> TrackBoneNames = new List<string>();
    public List<List<Vector3>> PositionData = new List<List<Vector3>>();
    public List<List<Quaternion>> RotationData = new List<List<Quaternion>>();
    public List<List<float>> TimeData = new List<List<float>>();

    public GladiusAnimationClip(AnimationClip animationClip)
    {
        m_animationClip = animationClip;
        m_numEvents = (int)(m_animationClip.length / AnimationUtils.FrameTime);

        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(m_animationClip);

        if (curveBindings != null && curveBindings.Length > 3)
        {
            AnimationCurve rootTx = AnimationUtility.GetEditorCurve(animationClip, curveBindings[0]);
            AnimationCurve rootTy = AnimationUtility.GetEditorCurve(animationClip, curveBindings[1]);
            AnimationCurve rootTz = AnimationUtility.GetEditorCurve(animationClip, curveBindings[2]);


            StringBuilder sb = new StringBuilder();
            float time = 0;
            while (time < m_animationClip.length)
            {
                sb.AppendLine(""+new Vector3(rootTx.Evaluate(time), rootTy.Evaluate(time), rootTz.Evaluate(time)));
                time += AnimationUtils.FrameTime;
            }

            Animator animator = null;
            
            
            
            Debug.Log(sb.ToString());
            //Keyframe[] keys = animationCurve.keys;
            int ibreak = 0;
        }

        //curveBindings[0].propertyName;

        //https://stackoverflow.com/questions/57846333/how-can-i-store-or-read-a-animation-clip-data-in-runtime


        // look at bone name map. to translate some anim data from existing files?
    }
}