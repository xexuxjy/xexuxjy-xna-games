using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseGUIPanel : MonoBehaviour
{
    public TownGUIController m_townGuiController;

    public GameObject LeaveButton;
    public GameObject ProceedButton;

    public GameObject m_parentGUIPanel;
    public int m_currentSelection;

    public void Start()
    {
        ChildStart();
    }

    public virtual void ChildStart()
    {
        if (LeaveButton != null)
        {
            LeaveButton.GetComponent<Button>().onClick.AddListener(() => { LeaveButtonClick(); });
        }

        if (ProceedButton != null)
        {
            ProceedButton.GetComponent<Button>().onClick.AddListener(() => { ProceedButtonClick(); });
        }

        gameObject.SetActive(false);

    }


    public virtual void PanelActivated()
    {
        gameObject.SetActive(true);

    }

    public virtual void PanelDeactivated()
    {
        gameObject.SetActive(false);
    }

    public TownData TownData
    { get { return m_townGuiController.TownData; } }

    public GladiatorSchool GladiatorSchool
    { get { return m_townGuiController.GladiatorSchool; } }


    //public CharacterData CharacterData
    //{ get { return m_townGuiController.m_currentCharacterData; } }

    //void m_panel_IsVisibleChanged(dfControl control, bool value)
    //{
    //    if (value == true)
    //    {
    //        int ibreak = 0;
    //    }
    //}

    public virtual void LeaveButtonClick()
    {
        m_townGuiController.SwitchPanel(m_parentGUIPanel);
        Debug.Log("Leave Button Pressed");
    }

    public virtual void ProceedButtonClick()
    {
        Debug.Log("Proceed Button Pressed");
    }

    public virtual void ActionPressed(object sender, ActionButtonPressedArgs e)
    {
    }

    public String PanelName
    { get { return gameObject.name; } }
}
