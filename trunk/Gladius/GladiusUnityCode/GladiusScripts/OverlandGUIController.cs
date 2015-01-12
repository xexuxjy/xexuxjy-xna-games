using UnityEngine;
using System.Collections;
using Gladius;

public class OverlandGUIController : MonoBehaviour
{
    dfPanel m_townInfoPanel;
    dfPanel m_overlandGUIPanel;
    dfPanel m_statusPanel;
    dfPanel m_dayNightPanel;

    // Use this for initialization
    void Start()
    {
        dfPanel[] panels =gameObject.GetComponentsInChildren<dfPanel>();
        foreach (dfPanel panel in panels)
        {
            if (panel.name == "TownInfoPanel")
            {
                m_townInfoPanel = panel;
            }
            else if(panel.name == "OverlandGUIPanel")
            {
                m_overlandGUIPanel = panel;
            }
            else if(panel.name == "StatusPanel")
            {
                m_statusPanel = panel;
            }
            else if(panel.name == "DayNightPanel")
            {
                m_dayNightPanel= panel;
            }
        }
    }

    public void UpdateData(GladiatorSchool school)
    {
        var rtLbl = m_statusPanel.Find<dfRichTextLabel>("Info");
        rtLbl.Text = string.Format("<h2 color=\"white\">Rank : {0}</h2>   <h2 color=\"yellow\">Gold {1}</h2>", school.SchoolRank,school.Gold);
        var lbl = m_dayNightPanel.Find<dfLabel>("Days");
        lbl.Text = "Days : "+school.Days;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReachedTown(TownData townData)
    {
        if (townData != null)
        {
            dfTweenFloat tween = m_townInfoPanel.GetComponent<dfTweenFloat>();
            tween.StartValue = 0f;
            tween.EndValue = 1f;
            tween.Length = 1f;
            tween.Play();

            m_townInfoPanel.Find<dfRichTextLabel>("Label").Text = string.Format("<h2 color=\"yellow\">{0}</h2>   <h2 color=\"blue\">{1}</h2>", townData.TownName,townData.Popularity);

            // fadein town view
        }
        else
        {
            dfTweenFloat tween = m_townInfoPanel.GetComponent<dfTweenFloat>();
            tween.StartValue = 1f;
            tween.EndValue = 0f;
            tween.Length = 1f;
            tween.Play();
        }
    }

    public void ReachedPortal(object o)
    {

    }


}
