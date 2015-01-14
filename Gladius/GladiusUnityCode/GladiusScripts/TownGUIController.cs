using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TownGUIController : MonoBehaviour
{
    float time;
    float maxTime = 5f;
    int panelCounter = 0;
    List<string> panelList = new List<string>();


    // Use this for initialization
    void Start()
    {
        //dfControl[] controls = gameObject.GetComponentsInChildren<dfControl>();
        //foreach (dfControl control in controls)
        //{
        //    if(control.name == "TownMenuPanel
        //}
        panelList.Add("MainMenuPanel");
        panelList.Add("ShopPanel");
        panelList.Add("SchoolPanel");
        panelList.Add("LeaguePanel");
    }

    public void SetData(TownData townData)
    {
        if (townData != m_townData)
        {
            m_townData = townData;
            m_townData.BuildData();
           
            dfPanel p = GameObject.Find("MainMenuPanel").GetComponent<dfPanel>();
            p.GUIManager.BringToFront(p);
            //p.Position();
            GameObject.Find("TownTitleLabel").GetComponent<dfRichTextLabel>().Text = m_townData.Name;
            GameObject.Find("ShopButton").GetComponent<dfButton>().Text = m_townData.Shop.Name;
            GameObject.Find("LeaguesButton").GetComponent<dfButton>().Text = m_townData.Arena;
            GameObject.Find("SchoolButton").GetComponent<dfButton>().Text = "School";
            GameObject.Find("LeaveButton").GetComponent<dfButton>().Text = "Leave Town";


            Texture2D bgTexture = m_townData.BackgroundTexture;
            if(bgTexture != null)
            {
                GameObject.Find("TownImage").GetComponent<dfTextureSprite>().Texture = bgTexture;
            }
        }

    }

    string lastPanelName = "";
    private Vector3 lastPanelPosition = new Vector3();
    public void SwitchPanel(String newPanelName)
    {
        if (!String.IsNullOrEmpty(lastPanelName))
        {
            dfPanel oldPanel = GameObject.Find(lastPanelName).GetComponent<dfPanel>();
            Debug.Log("Moving panel : " + lastPanelName + " to " + newPanelName);
            oldPanel.RelativePosition = lastPanelPosition;
            //oldPanel.IsVisible = false;
            //oldPanel.enabled = false;
        }
        dfPanel newPanel = GameObject.Find(newPanelName).GetComponent<dfPanel>();
        //newPanel.IsVisible = true;
        //newPanel.enabled = true;
        newPanel.Focus();
        lastPanelPosition = newPanel.RelativePosition;
        newPanel.RelativePosition = new Vector3();
        newPanel.BringToFront();
    }

    
    public void Update()
    {
        time += Time.deltaTime;
        if (time > maxTime)
        {
            time = 0f;
            panelCounter++;
            panelCounter %= panelList.Count;
            string newPanelName = panelList[panelCounter];
            SwitchPanel(newPanelName);
        }
    }


    private TownData m_townData = null;


}



