using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gladius;

public class CameraManager : MonoBehaviour
{

    public KGFOrbitCamSettings NormalModeSettings;
    public KGFOrbitCamSettings ManualModeSettings;
    public KGFOrbitCamSettings CombatModeSettings;

    public GameObject CameraTarget;


    private float lastStickVal = 0f;
    public CameraManager()
    {
    }

    public void Start()
    {
        GladiusGlobals.CameraManager = this;
        CurrentCameraMode = CameraMode.Normal;
        
        Camera c = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        GladiusGlobals.Camera = c;

        CameraTarget = GameObject.Find("CameraTarget");

    }


    public void FixedUpdate()
    {
        if (Input.GetButtonDown("CameraNormal") || Input.GetButtonDown("PadLeftStickPress"))
        {
            CurrentCameraMode = CameraMode.Normal;
        }
        if (Input.GetButtonDown("CameraManual") || Input.GetButtonDown("PadRightStickPress"))
        {
            CurrentCameraMode = CameraMode.Manual;
        }

        if (CurrentCameraMode == CameraMode.Normal)
        {
            float rightStickVal = Input.GetAxis("PadRightStickH");
            if (lastStickVal != 0 && rightStickVal == 0)
            {
                NormalModeSettings.Apply();
            }
            lastStickVal = rightStickVal;
        }

        //UpdateCameraManual();

    }



    //public void SetCombatModeActive(bool active ,BaseActor actor1, BaseActor actor2)
    //{
    //    m_combatModeActive = active;
    //    m_actor1 = actor1;
    //    m_actor2 = actor2;
    //}


    public GameObject TargetObject
    {
        get;
        set;
    }


    private CameraMode m_cameraMode;
    public CameraMode CurrentCameraMode
    {
        get
        {
            return m_cameraMode;
        }
        set
        {
            m_cameraMode = value;
            switch (value)
            {
                case (CameraMode.Normal):
                    NormalModeSettings.Apply();
                    break;
                case (CameraMode.Manual):
                    ManualModeSettings.Apply();
                    break;
                case (CameraMode.Combat):
                    CombatModeSettings.Apply();
                    break;
            }

        }
    }


}
