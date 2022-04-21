using System;
using System.Collections.Generic;
using UnityEngine;

public class BoneAnimData
{
    public const int kDontTranslate = 2;
    public const int kDontRotate = 4;
    public const int kUseRootForParent = 8;

    public bool ignoreTranslate;
    public bool ignoreRotate;

    public BoneAnimData(GladiusSimpleAnim gladiusAnim, BoneAnimData parent, string name, Transform t)
    {
        this.gladiusAnim = gladiusAnim;
        boneNameAndInfo = name;


        String[] tokens = boneNameAndInfo.Split(new String[] { "--" }, StringSplitOptions.None);
        if (tokens.Length == 3)
        {
            boneName = tokens[0];
            int.TryParse(tokens[1], out boneId);
            int.TryParse(tokens[2], out boneFlags);

            gameObjectTranform = t;
            bindPosePosition = t.localPosition;
            bindPoseQuaternion = t.localRotation;

            parentBad = parent;

        }

        //if((boneFlags & kDontTranslate )!=0)
        //{
        //    ignoreTranslate = true;
        //}

        //if (name.Contains("face") || name.Contains("joint"))
        //{
        //    ignoreTranslate = true;
        //}

    }

    public void Reset()
    {
        posSet = false;
        rotSet = false;
        currentPosition = Vector3.zero;
        currentRotationQuat = Quaternion.identity;
    }

    public void ApplyCurrent()
    {
        Vector3 tempPos = posSet ? currentPosition : bindPosePosition;
        Quaternion tempRot = rotSet ? currentRotationQuat : bindPoseQuaternion;

        if (parentBad == null)
        {
            //gameObjectTranform.localRotation = tempRot * GladiusGlobals.CharacterLocalRotation;
        }
        else
        {
            gameObjectTranform.localRotation = tempRot;
        }
        gameObjectTranform.localPosition = tempPos;
    }

    public Vector3 CurrentPosition
    {
        get
        {
            return currentPosition;
        }
        set
        {
            posSet = true;
            currentPosition = value;
            //currentPosition = new Vector3(-currentPosition.x, currentPosition.y, currentPosition.z);
        }
    }



    public Quaternion CurrentRotation
    {
        get
        {
            return currentRotationQuat;
        }
        set
        {
            rotSet = true;
            currentRotationQuat = value;
        }
    }

    private bool posSet;
    private bool rotSet;

    public String boneNameAndInfo;
    public String boneName;
    public int boneId = -1;
    public int boneFlags = 0;

    public Vector3 bindPosePosition;
    public Quaternion bindPoseQuaternion;
    public Transform gameObjectTranform;
    private Vector3 currentPosition;
    private Quaternion currentRotationQuat = Quaternion.identity;

    public BoneAnimData parentBad;
    public List<BoneAnimData> childBads = new List<BoneAnimData>();
    public GladiusSimpleAnim gladiusAnim;

}
