using UnityEngine;
using System.Collections;

public class OverlandGUIController : MonoBehaviour
{
    dfPanel m_townInfoPanel;
    
    // Use this for initialization
    void Start()
    {
        dfPanel[] panels =gameObject.GetComponentsInChildren<dfPanel>();
        foreach (dfPanel panel in panels)
        {
            if (panel.name == "TownInfoPanel")
            {
                m_townInfoPanel = panel;
                break;
            }
        }
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

            //dfTweenFloat[] tweens = m_townInfoPanel.GetComponents<dfTweenFloat>();
            
            //("AlphaTween").SetStartValue(0f).SetEndValue(1f).SetDuration(0.5f);

            //var tweenOpacity = m_townInfoPanel.Find<dfTweenFloat>("AlphaTween").SetStartValue(0f).SetEndValue(1f).SetDuration(0.5f);

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
