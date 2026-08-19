using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gladius.util.Test
{
    public class TestCreatePANFromAnimClip : MonoBehaviour
    {
        public AnimationClip animClip;
        public string LogFileName;
        public string OutputFileName;
        public Transform RootBone;

        
        public void Awake()
        {
            Dictionary<string,List<(float,float)>> values = new  Dictionary<string ,List<(float,float)>>(); 
            List<string> uniquePaths = new  List<string>();
            
            EditorCurveBinding[] ecba = AnimationUtility.GetCurveBindings(animClip);
            foreach (EditorCurveBinding ecb in ecba)
            {
                AnimationCurve acv = AnimationUtility.GetEditorCurve(animClip, ecb);
                Keyframe[] keyFrames = acv.keys;
                string key = ecb.path+"/"+ecb.propertyName;

                if (!uniquePaths.Contains(ecb.path))
                {
                    uniquePaths.Add(ecb.path);
                }

                if (!values.ContainsKey(key))
                {
                    values.Add(key, new List<(float,float)>());
                }

                foreach (Keyframe keyframe in keyFrames)
                {
                    values[key].Add((keyframe.time, keyframe.value));
                }
                // do something clever

            }

            List<(string, List<(float, Vector3)>, List<(float, Quaternion)>)> results =
                new List<(string, List<(float, Vector3)>, List<(float, Quaternion)>)>();
            
            List<(string,List<(float,Vector3)>)> positionDataResults = new  List<(string,List<(float,Vector3)>)>();
            List<(string,List<(float,Quaternion)>)> rotationDataResults = new  List<(string,List<(float,Quaternion)>)>();

            foreach (string path in uniquePaths)
            {
                string positionPrefix = "/m_LocalPosition";
                string positionKey = path + positionPrefix;

                if (values.ContainsKey(positionKey + ".x"))
                {

                    List<(float, Vector3)> positionData = new List<(float, Vector3)>();
                    List<(float, float)> xdataPos = values[positionKey + ".x"];
                    List<(float, float)> ydataPos = values[positionKey + ".y"];
                    List<(float, float)> zdataPos = values[positionKey + ".z"];

                    for (int i = 0; i < xdataPos.Count; i++)
                    {
                        Debug.Assert(xdataPos[i].Item1 == ydataPos[i].Item1 && xdataPos[i].Item1 == zdataPos[i].Item1);
                        positionData.Add((xdataPos[i].Item1,
                            new Vector3(xdataPos[i].Item2, ydataPos[i].Item2, zdataPos[i].Item2)));
                    }

                    positionDataResults.Add((path, positionData));

                }

                string rotationPrefix = "/m_LocalRotation";
                string rotationKey = path + rotationPrefix;

                if (values.ContainsKey(rotationKey + ".x"))
                {
                    List<(float, Quaternion)> rotationData = new List<(float, Quaternion)>();
                    List<(float, float)> xdataRot = values[rotationKey + ".x"];
                    List<(float, float)> ydataRot = values[rotationKey + ".y"];
                    List<(float, float)> zdataRot = values[rotationKey + ".z"];
                    List<(float, float)> wdataRot = values[rotationKey + ".w"];

                    for (int i = 0; i < xdataRot.Count; i++)
                    {
                        Debug.Assert(xdataRot[i].Item1 == ydataRot[i].Item1 && xdataRot[i].Item1 == zdataRot[i].Item1 &&
                                     xdataRot[i].Item1 == wdataRot[i].Item1);
                        rotationData.Add((xdataRot[i].Item1,
                            new Quaternion(xdataRot[i].Item2, ydataRot[i].Item2, zdataRot[i].Item2, wdataRot[i].Item2)));
                    }
                    rotationDataResults.Add((path,rotationData));
                }

            
            }

            Dictionary<Transform, List<(float, Vector3)>> transformDictionary =
                new Dictionary<Transform, List<(float, Vector3)>>();
            
            Dictionary<Transform, List<(float, Quaternion)>> rotationDictionary =
                new Dictionary<Transform, List<(float, Quaternion)>>();
            
            foreach (string path in uniquePaths)
            {
                if (GameObject.Find(path) != null)
                {
                    Transform t = GameObject.Find(path).transform;

                    if (positionDataResults.Exists(x => x.Item1 == path))
                    {
                        transformDictionary.Add(t, positionDataResults.Find(x=>x.Item1==path).Item2);
                    }

                    if (rotationDataResults.Exists(x => x.Item1 == path))
                    {
                        rotationDictionary.Add(t, rotationDataResults.Find(x=>x.Item1==path).Item2);
                    }
                }
            }

            
            
            List<float> animationTimes = new List<float>();

            float time = 0f;
            while (time < animClip.length)
            {
                animationTimes.Add(time);
                time += (1f/animClip.frameRate);
            }
            
            List<Transform> transformList = new List<Transform>();
            
            BuildTransformList(RootBone, transformList);



            using (BinaryWriter bw = new BinaryWriter(File.Open(OutputFileName, FileMode.Create)))
            {
                AnimationUtils.WriteDataAsPAN(bw, RootBone, transformList, animationTimes, transformDictionary,
                    rotationDictionary);
            }

            int ibreak = 0;
            
        }
        
        void BuildTransformList(Transform t,List<Transform> transformList)
        {
            transformList.Add(t);
            foreach (Transform child in t)
            {
                BuildTransformList(child, transformList);
            }
        }
        
    }
}