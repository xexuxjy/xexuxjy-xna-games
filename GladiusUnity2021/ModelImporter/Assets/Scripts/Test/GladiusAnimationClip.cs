using System.Collections.Generic;
using System.IO;
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


    public static bool IsValidCurveBinding(EditorCurveBinding ecb)
    {
        return ecb != null && ecb.path == "";
    }
    public GladiusAnimationClip(AnimationClip animationClip)
    {
        m_animationClip = animationClip;
        m_numEvents = (int)(m_animationClip.length / AnimationUtils.FrameTime);

        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(m_animationClip);

        if (curveBindings != null)
        {

            List<float> timeData = new List<float>();
            Dictionary<string, List<float>> propertyData = new Dictionary<string, List<float>>();

            foreach (var curveBinding in curveBindings)
            {
                if (IsValidCurveBinding(curveBinding))
                {
                    propertyData.Add(curveBinding.propertyName, new List<float>());
                }
            }

            using (TextWriter tw = new StreamWriter("d:/tmp/animation-data.txt"))
            {
                float time = 0;
                while (time < m_animationClip.length)
                {
                    timeData.Add(time);
                    for (int i = 0; i < curveBindings.Length; i++)
                    {
                        if(IsValidCurveBinding(curveBindings[i]))
                        {
                            var curve = AnimationUtility.GetEditorCurve(animationClip, curveBindings[i]);
                            propertyData[curveBindings[i].propertyName].Add(curve.Evaluate(time));

                            tw.WriteLine($"{curveBindings[i].propertyName}  time {time}  value {curve.Evaluate(time)}");
                        }
                    }
                    time += AnimationUtils.FrameTime;
                }
            }

            int ibreak = 0;
        }
    }
}