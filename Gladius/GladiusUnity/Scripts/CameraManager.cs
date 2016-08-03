using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gladius;

public class CameraManager : MonoBehaviour
{


    public GameObject CameraTarget;
    public GameObject BirdsEyeCamera;
    public GameObject MainCamera;

    private float lastStickVal = 0f;
    public CameraManager()
    {
    }

    public void Start()
    {
        CurrentCameraMode = GladiusCameraMode.None;
        CameraTarget = GameObject.Find("CameraTarget");
        EventManager.BaseActorChanged += EventManager_BaseActorChanged;
    }

    private void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs baca)
    {
        ReparentTarget(baca.New.gameObject.transform);
        CurrentCameraMode = GladiusCameraMode.Normal;
    }

    public void ReparentTarget(Transform newParent)
    {
        ReparentTarget(newParent, Vector3.forward);
    }

    public void ReparentTarget(Transform newParent,Vector3 forward)
    {
        if (CameraTarget.transform.parent != newParent)
        {
            CameraTarget.transform.parent = newParent;

            //Vector3 offset = new Vector3(0, 0.2f, -1f);
            //Quaternion qoffset = Quaternion.LookRotation(offset, Vector3.up);
            Vector3 offset = new Vector3(0, 0, 0);
            Quaternion qoffset = Quaternion.identity;

            //CameraTarget.transform.localRotation = qoffset;
            CameraTarget.transform.localRotation = Quaternion.LookRotation(forward);
            CameraTarget.transform.localPosition = offset;
            CameraTarget.transform.localScale = Vector3.one;
        }
    }


    public void FixedUpdate()
    {
        if (Input.GetButtonDown("CameraNormal") || Input.GetButtonDown("PadLeftStickPress"))
        {
            CurrentCameraMode = GladiusCameraMode.Normal;
        }
        if (Input.GetButtonDown("CameraManual") || Input.GetButtonDown("PadRightStickPress"))
        {
            CurrentCameraMode = GladiusCameraMode.Manual;
        }

        if (CurrentCameraMode == GladiusCameraMode.Normal)
        {
            float rightStickVal = Input.GetAxis("PadRightStickH");
            if (lastStickVal != 0 && rightStickVal == 0)
            {
                //NormalModeSettings.Apply();
            }
            lastStickVal = rightStickVal;
        }

        if (CurrentCameraMode == GladiusCameraMode.BirdsEye)
        {
            if (Input.GetKey(KeyCode.PageDown))
            {
                BirdsEyeCamera.transform.position += new Vector3(0, -0.1f, 0);
            }
            if (Input.GetKey(KeyCode.PageUp))
            {
                BirdsEyeCamera.transform.position += new Vector3(0, 0.1f, 0);
            }
        }


        //UpdateCameraManual();

    }



    private GladiusCameraMode m_cameraMode;
    public GladiusCameraMode CurrentCameraMode
    {
        get
        {
            return m_cameraMode;
        }
        set
        {
            if (value != m_cameraMode)
            {
                Camera c = MainCamera.GetComponent<Camera>();
                c.enabled = true;

                switch (value)
                {
                    case (GladiusCameraMode.Normal):
                        ApplyCharacterSettings();
                        break;
                    case (GladiusCameraMode.Manual):
                        //ManualModeSettings.Apply();
                        break;
                    case (GladiusCameraMode.Combat):
                        ApplyCombatSettings();
                        //CombatModeSettings.Apply();
                        break;
                    case(GladiusCameraMode.Overland):
                        //OverlandModeSettings.Apply();
                        break;
                    case (GladiusCameraMode.BirdsEye):
                        c.enabled = false;
                        BirdsEyeCamera.GetComponent<Camera>().enabled = true;
                        break;
                    case (GladiusCameraMode.MovementCursor):
                        ApplyMovementCursorSettings();
                        break;
                }

            }
            m_cameraMode = value;

        }
    }

    public void ApplyCharacterSettings()
    {
        //ThirdPersonCamera.Follow followComponent = GetComponent<ThirdPersonCamera.Follow>();
        //followComponent.tiltVector = new Vector3(0, -0.2f, 0);
        //followComponent.tiltVector = new Vector3(0, 1f, 1f);
    }


    public void ApplyMovementCursorSettings()
    {
        //ThirdPersonCamera.Follow followComponent = GetComponent<ThirdPersonCamera.Follow>();
        //followComponent.tiltVector = new Vector3(0, -0.7f, 0);


    }

    public void ApplyCombatSettings()
    {

    }

    }
