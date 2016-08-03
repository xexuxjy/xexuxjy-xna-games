using UnityEngine;
using System.Collections;
using System;
using Gladius;

public class OverlandCharacterControl : MonoBehaviour
{

    public float MovementSpeed = 2f;
    public float CurrentTime = 0f;
    public float TimeChangeRate = 0.1f;

    public int LightStartDirection = 100;
    public int LightEndDirection = 250;
    public Light WorldLight;
    public OverlandGUIController m_overlandGuiController;
    public CameraManager CameraManager;


    // Use this for initialization
    void Start()
    {
        RegisterListeners();
        GameObject go = GameObject.Find("UI Root");
        if (go != null)
        {
            m_overlandGuiController = go.GetComponent<OverlandGUIController>();
        }
        float lightDirection = Mathf.Lerp(LightStartDirection, LightEndDirection, CurrentTime);

        Quaternion q = Quaternion.AngleAxis(lightDirection, Vector3.up);
        WorldLight.transform.rotation = q;

    }

    public void RegisterListeners()
    {
        EventManager.ActionPressed +=EventManager_ActionPressed;
                
    }

    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        Vector3 adjust = new Vector3();
        switch (e.ActionButton)
        {
            case (ActionButton.ActionButton1):
                {
                    // enter town.
                    break;
                }

            case (ActionButton.Move1Left):
                {
                    adjust += Vector3.left;
                    break;
                }
            case (ActionButton.Move1Right):
                {
                    adjust += Vector3.right;
                    break;
                }
            case (ActionButton.Move1Up):
                {
                    adjust += Vector3.forward;
                    break;
                }
            case (ActionButton.Move1Down):
                {
                    adjust += Vector3.back;
                    break;
                }
        }
        UpdateMovement(adjust,false);
    }

    public void UpdateMovement(Vector3 v,bool scalar)
    {
        float vl = v.magnitude;
        v.Normalize();
        if (vl > 0)
        {
            float gravity = 10.0f;

            Quaternion q = Quaternion.LookRotation(v);

            transform.rotation = q;

            v *= vl * MovementSpeed * Time.deltaTime;

            v.y = -gravity * Time.deltaTime;

            CharacterController cc = GetComponent<CharacterController>();
            cc.Move(v);
        }
        else
        {
            PlayIdle();
        }

    }


    void PlayIdle()
    {
        //String animName = "idle-2";
        //if (!animation.IsPlaying(animName))
        //{
        //    animation.Play(animName);
        //}

    }

    void PlayRun()
    {
        //String animName = "walk";
        //if (!animation.IsPlaying(animName))
        //{
        //    animation.Play(animName);
        //}
    }

    // Update is called once per fame
    void Update()
    {
        //Vector2 leftHandJoystickPosition = dfTouchJoystick.GetJoystickPosition("TouchJoystickLeft");
        //Vector3 v3 = new Vector3(leftHandJoystickPosition.x, 0, leftHandJoystickPosition.y);
        //if (v3.magnitude > 0f)
        //{
            
        //    v3 = CameraManager.transform.TransformDirection(v3);
        //    v3.y = 0;
        //    v3.Normalize();
        //}
        //UpdateMovement(v3,true);

        //UpdateTime();

        //if (m_overlandGuiController != null && GladiusGlobals.GladiatorSchool != null)
        //{
        //    m_overlandGuiController.UpdateData(GladiusGlobals.GladiatorSchool, CurrentTime);
        //}
    }

    public void UpdateTime()
    {
        CurrentTime += (Time.deltaTime * TimeChangeRate);
        if (CurrentTime > 1f)
        {
            CurrentTime = 0f;
            if (GladiusGlobals.GladiatorSchool != null)
            {
                GladiusGlobals.GladiatorSchool.Days += 1;
            }
        }

        float lightDirection = Mathf.Lerp(LightStartDirection, LightEndDirection, CurrentTime);

        Vector3 v = new Vector3(0, lightDirection, 0);
        WorldLight.transform.localEulerAngles = v;

    
    
    }

    //public void OnActionClick(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    if(control.Tag is TownData)
    //    {
    //        GladiusGlobals.GameStateManager.SetNewState(GameState.Town);
    //    }
    //    else if (control.Tag is string)
    //    {
    //        string tagName = control.Tag as string;
    //        if (tagName.Contains("Portal"))
    //        {
    //            OverlandZone newZone = OverlandZone.None;
    //            String zoneName = tagName.Replace("Portal","");
    //            newZone = (OverlandZone)Enum.Parse(typeof(OverlandZone),zoneName);

    //            String existingZoneName = Application.loadedLevelName.Replace("WorldMap", "");
    //            OverlandZone existingZone = (OverlandZone)Enum.Parse(typeof(OverlandZone), existingZoneName);

    //            ZoneTransitionData ztd = new ZoneTransitionData();
    //            ztd.newZone = newZone; ;
    //            ztd.lastZone = existingZone;

    //            GladiusGlobals.ZoneTransitionData = ztd;
    //            GladiusGlobals.GameStateManager.SetNewState(GameState.Overland);

    //        }
    //    }
    //}


    public void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("Portal"))
        {
            if (m_overlandGuiController != null)
            {
                m_overlandGuiController.ReachedPortal(other.name);
            }
        }
        else if (other.tag == "Town")
        {
            m_currentTownData = GladiusGlobals.GameStateManager.TownManager.Find(other.name);
            if (m_overlandGuiController != null)
            {
                m_overlandGuiController.ReachedTown(m_currentTownData);
            }


            if (m_currentTownData != null)
            {
                Debug.Log("Found Town " + m_currentTownData.Name);
            }

        

        }

    }




    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Town")
        {
            Debug.Log("Left Town " + m_currentTownData.Name);
            m_currentTownData = null;
            if (m_overlandGuiController != null)
            {
                m_overlandGuiController.ReachedTown(m_currentTownData);
            }
        }
    }

    public TownData TownData
    {
        get
        {
            return m_currentTownData;
        }

        set
        {
            m_currentTownData = value;
        }
    }


    private TownData m_currentTownData;
}

public class ZoneTransitionData
{
    public OverlandZone lastZone;
    public OverlandZone newZone;
}

