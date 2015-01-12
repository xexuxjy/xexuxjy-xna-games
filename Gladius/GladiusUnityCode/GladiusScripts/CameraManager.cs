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
    public KGFOrbitCamSettings OverlandModeSettings;

    public GameObject CameraTarget;


    private float lastStickVal = 0f;
    public CameraManager()
    {
    }

    public void Start()
    {
        CurrentCameraMode = CameraMode.None;
        CameraTarget = GameObject.Find("CameraTarget");
    }

    public void ReparentTarget(GameObject newParent)
    {
        CameraTarget.transform.parent = newParent.transform;
        CameraTarget.transform.localRotation = Quaternion.identity;
        CameraTarget.transform.localPosition = Vector3.zero;
        CameraTarget.transform.localScale = Vector3.one;

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



    private CameraMode m_cameraMode;
    public CameraMode CurrentCameraMode
    {
        get
        {
            return m_cameraMode;
        }
        set
        {
            if (value != m_cameraMode)
            {
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
                    case(CameraMode.Overland):
                        OverlandModeSettings.Apply();
                        break;
                }

            }
            m_cameraMode = value;

        }
    }


}
