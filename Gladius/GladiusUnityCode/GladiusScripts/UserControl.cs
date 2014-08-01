using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;

public class UserControl : MonoBehaviour
{

    public const String ActionButton1Name = "Action1";

    public void Start()
    {
        m_actionButtonDictionary["Move1Left"] = ActionButton.Move1Left;
        m_actionButtonDictionary["Move1Right"] = ActionButton.Move1Right;
        m_actionButtonDictionary["Move1Up"] = ActionButton.Move1Up;
        m_actionButtonDictionary["Move1Down"] = ActionButton.Move1Down;

        m_actionButtonDictionary["Move2Left"] = ActionButton.Move2Left;
        m_actionButtonDictionary["Move2Right"] = ActionButton.Move2Right;
        m_actionButtonDictionary["Move2Up"] = ActionButton.Move2Up;
        m_actionButtonDictionary["Move2Down"] = ActionButton.Move2Down;

        m_actionButtonDictionary["Action1"] = ActionButton.ActionButton1;
        m_actionButtonDictionary["Action2"] = ActionButton.ActionButton2;
        m_actionButtonDictionary["Action3"] = ActionButton.ActionButton3;
        m_actionButtonDictionary["Action4"] = ActionButton.ActionButton4;

    }

    //public bool Move1LeftPressed()
    //{
    //    return Input.GetButtonUp("Move1Left");
    //}

    //public bool Move1RightPressed()
    //{
    //    return Input.GetButtonUp("Move1Right");

    //}

    //public bool Move1UpPressed()
    //{
    //    return Input.GetButtonUp("Move1Up");

    //}

    //public bool Move1DownPressed()
    //{
    //    return Input.GetButtonUp("Move1Down");
    //}

    //public bool Move1LeftHeld()
    //{
    //    return Input.GetButton("Move1Left");
    //}
    //public bool Move1RightHeld()
    //{
    //    return Input.GetButton("Move1Right");
    //}
    //public bool Move1UpHeld()
    //{
    //    return Input.GetButton("Move1Up");
    //}
    //public bool Move1DownHeld()
    //{
    //    return Input.GetButton("Move1Down");
    //}


    //public bool Action1Pressed()
    //{
    //    return Input.GetButtonDown("Action1");
    //}

    //public bool Action2Pressed()
    //{
    //    return Input.GetButtonDown("Action2");

    //}

    //public bool Action3Pressed()
    //{
    //    return Input.GetButtonDown("Action3");

    //}

    //public bool Action4Pressed()
    //{
    //    return Input.GetButtonDown("Action4");
    //}

    public void Update()
    {
        foreach (String key in m_actionButtonDictionary.Keys)
        {
            if (Input.GetButtonDown(key))
            {
                EventManager.PerformAction(this, m_actionButtonDictionary[key]);
            }
        }

        //if (Action1Pressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.ActionButton1);
        //}
        //if (Action2Pressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.ActionButton2);
        //}
        //if (Action3Pressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.ActionButton3);
        //}
        //if (Action4Pressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.ActionButton4);
        //}
        //if (Move1LeftPressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.Move1Left);
        //}
        //if (Move1RightPressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.Move1Right);
        //}
        //if (Move1UpPressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.Move1Up);
        //}
        //if (Move1DownPressed())
        //{
        //    EventManager.PerformAction(this, ActionButton.Move1Down);
        //}
    }

    public Dictionary<String, ActionButton> m_actionButtonDictionary = new Dictionary<String,ActionButton>();

}


