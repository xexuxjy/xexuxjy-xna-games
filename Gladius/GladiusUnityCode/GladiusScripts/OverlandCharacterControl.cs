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
        GameObject go = GameObject.Find("UIRoot");
        m_overlandGuiController = go.GetComponent<OverlandGUIController>();
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
                    adjust += Vector3.up;
                    break;
                }
            case (ActionButton.Move1Down):
                {
                    adjust += Vector3.down;
                    break;
                }
        }
        if (adjust.sqrMagnitude > 0)
        {
            adjust *= MovementSpeed * Time.deltaTime;
            //Quaternion q = Quaternion.LookRotation(adjust);

            transform.position += adjust;
            //transform.rotation = q;
            PlayRun();

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

    }


    public void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Town")
        {
            m_currentTownData = other.GetComponent<TownData>();
            if (m_overlandGuiController != null)
            {
                m_overlandGuiController.ReachedTown(m_currentTownData);
            }


            if (m_currentTownData != null)
            {
                Debug.Log("Found Town " + m_currentTownData.TownName);
            }

        

        }

    }


    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Town")
        {
            m_currentTownData = null;
            Debug.Log("Left Town " + m_currentTownData.TownName);
            if (m_overlandGuiController != null)
            {
                m_overlandGuiController.ReachedTown(m_currentTownData);
            }
        }
    }


    private TownData m_currentTownData;
}
