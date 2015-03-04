using UnityEngine;
using System.Collections;
using System;
using Gladius;

public class OverlandCharacterControl : MonoBehaviour
{

    public float MovementSpeed = 2f;
    OverlandGUIController m_overlandGuiController;

    // Use this for initialization
    void Start()
    {
        RegisterListeners();
        GameObject go = GameObject.Find("UI Root");
        if (go != null)
        {
            m_overlandGuiController = go.GetComponent<OverlandGUIController>();
        }
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
        Vector2 leftHandJoystickPosition = dfTouchJoystick.GetJoystickPosition("TouchJoystickLeft");
        Vector3 v3 = new Vector3(leftHandJoystickPosition.x, 0, leftHandJoystickPosition.y);
        UpdateMovement(v3,true);



        if (m_overlandGuiController != null && GladiusGlobals.GameStateManager.OverlandStateCommon.GladiatorSchool != null)
        {
            m_overlandGuiController.UpdateData(GladiusGlobals.GameStateManager.OverlandStateCommon.GladiatorSchool);
        }
    }


    public void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Town")
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
