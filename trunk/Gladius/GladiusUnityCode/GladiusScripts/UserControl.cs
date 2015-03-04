using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;

public class UserControl : MonoBehaviour
{

    public const String ActionButton1Name = "Action1";
    public float GridMoveDelay = 0.2f;
    public float ActionMoveDelta = 0.2f;

    private float CurrentMoveDelay = 0;
    private bool HaveMovedGrid = false;
    private bool HaveMovedDpad= false;

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

    public void PeformActionOnAxis(String axisName, ActionButton negAction, ActionButton posAction, ref bool flag)
    {
        if (flag == false || GridMoveDelay == 0.0f)
        {
            float axisVal = Input.GetAxis(axisName);
            if (Math.Abs(axisVal) > ActionMoveDelta)
            {
                EventManager.PerformAction(this, axisVal < 0 ? negAction : posAction);
                flag = true;
            }
        }
    }


    public void Update()
    {
        foreach (String key in m_actionButtonDictionary.Keys)
        {
            if (Input.GetButtonDown(key))
            {
                EventManager.PerformAction(this, m_actionButtonDictionary[key]);
            }
        }


        PeformActionOnAxis("PadDpadH", ActionButton.Move2Left, ActionButton.Move2Right, ref HaveMovedDpad);
        PeformActionOnAxis("PadDpadV", ActionButton.Move2Down, ActionButton.Move2Up, ref HaveMovedDpad);
        if (HaveMovedDpad)
        {
            CurrentMoveDelay += Time.deltaTime;
            if (CurrentMoveDelay > GridMoveDelay)
            {
                HaveMovedDpad = false;
                CurrentMoveDelay = 0f;
            }
        }


        if (GladiusGlobals.GameStateManager.CurrentStateData.CameraManager != null)
        {
            if (GladiusGlobals.GameStateManager.CurrentStateData.CameraManager.CurrentCameraMode == CameraMode.Normal)
            {
                PeformActionOnAxis("PadLeftStickH", ActionButton.Move1Left, ActionButton.Move1Right,ref HaveMovedGrid);
                PeformActionOnAxis("PadLeftStickV", ActionButton.Move1Down, ActionButton.Move1Up,ref HaveMovedGrid);
            }
            else if (GladiusGlobals.GameStateManager.CurrentStateData.CameraManager.CurrentCameraMode == CameraMode.Overland)
            {
                PeformActionOnAxis("PadLeftStickH", ActionButton.Move1Left, ActionButton.Move1Right, ref HaveMovedGrid);
                PeformActionOnAxis("PadLeftStickV", ActionButton.Move1Down, ActionButton.Move1Up, ref HaveMovedGrid);
                Vector2 leftJoystickPos = dfTouchJoystick.GetJoystickPosition("TouchJoystickLeft");
            }
            if (HaveMovedGrid)
            {
                CurrentMoveDelay += Time.deltaTime;
                if (CurrentMoveDelay > GridMoveDelay)
                {
                    HaveMovedGrid = false;
                    CurrentMoveDelay = 0f;
                }

            }


        }
    }

    public Dictionary<String, ActionButton> m_actionButtonDictionary = new Dictionary<String,ActionButton>();

}


