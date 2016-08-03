using UnityEngine;
using System.Collections;
using Gladius;
using System;

public class OverlandGUIController : MonoBehaviour
{
    //dfPanel m_townInfoPanel;
    //dfPanel m_overlandGUIPanel;
    //dfPanel m_statusPanel;
    //dfPanel m_dayNightPanel;
    //dfPanel m_joystickPanel;
    //dfButton m_actionButton;

    OverlandCharacterControl m_characterController;

    // Use this for initialization
    void Start()
    {
        //dfPanel[] panels = gameObject.GetComponentsInChildren<dfPanel>();
        //foreach (dfPanel panel in panels)
        //{
        //    if (panel.name == "TownInfoPanel")
        //    {
        //        m_townInfoPanel = panel;
        //    }
        //    else if (panel.name == "OverlandGUIPanel")
        //    {
        //        m_overlandGUIPanel = panel;
        //    }
        //    else if (panel.name == "StatusPanel")
        //    {
        //        m_statusPanel = panel;
        //    }
        //    else if (panel.name == "DayNightPanel")
        //    {
        //        m_dayNightPanel = panel;
        //    }
        //    else if (panel.name == "JoystickPanel")
        //    {
        //        m_joystickPanel = panel;
        //    }
        //}

        //m_actionButton = m_joystickPanel.FindPath<dfButton>("RightActionPanel/Button");
        //m_actionButton.Click += m_actionButton_Click;
    }

    //void m_actionButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    GameObject.Find("PlayerParty").GetComponent<OverlandCharacterControl>().OnActionClick(control, mouseEvent);
    //}

    public void UpdateData(GladiatorSchool school, float timeOfDay)
    {
        //var rtLbl = m_statusPanel.Find<dfRichTextLabel>("Info");
        //rtLbl.Text = string.Format("<h2 color=\"white\">Rank : {0}</h2>   <h2 color=\"yellow\">Gold {1}</h2>", school.SchoolRank, school.Gold);
        //var lbl = m_dayNightPanel.Find<dfLabel>("Days");
        //lbl.Text = "Days : " + school.Days;

        //var sprite = m_dayNightPanel.Find<dfSprite>("Sprite");
        //// -45 is midday.
        //// 135 is midnight
        //float angle = Mathf.Lerp(-220f, 135f, timeOfDay);


        //sprite.transform.localEulerAngles = new Vector3(0, 0, angle);


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReachedPortal(string portalName)
    {
        //if (!String.IsNullOrEmpty(portalName))
        //{
        //    m_actionButton.enabled = true;
        //    m_actionButton.BackgroundSprite = "Button_A_032";
        //    m_actionButton.Text = "Enter Portal";
        //    m_actionButton.Tag = portalName;
        //}
        //else
        //{
        //    m_actionButton.enabled = false;
        //}
    }



    public void ReachedTown(TownData townData)
    {
        if (townData != null)
        {
            //    dfTweenFloat tween = m_townInfoPanel.GetComponent<dfTweenFloat>();
            //    tween.StartValue = 0f;
            //    tween.EndValue = 1f;
            //    tween.Length = 1f;
            //    tween.Play();

            //    m_townInfoPanel.Find<dfRichTextLabel>("Label").Text = string.Format("<h2 color=\"yellow\">{0}</h2>   <h2 color=\"blue\">{1}</h2>", townData.Name, townData.Popularity);

            //    // fadein town view

            //    m_actionButton.enabled = true;
            //    m_actionButton.BackgroundSprite = "Button_A_032";
            //    m_actionButton.Text = "Enter Town";
            //    m_actionButton.Tag = townData;
            //}
            //else
            //{
            //    dfTweenFloat tween = m_townInfoPanel.GetComponent<dfTweenFloat>();
            //    tween.StartValue = 1f;
            //    tween.EndValue = 0f;
            //    tween.Length = 1f;
            //    tween.Play();

            //    m_actionButton.enabled = false;
        }
    }

}
