using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TownGUIController : MonoBehaviour
{
    float time;
    float maxTime = 5f;
    int panelCounter = 0;
    List<string> m_panelList = new List<string>();

    List<dfControl> m_mainMenuButtonList = new List<dfControl>();
    List<dfControl> m_schoolMenuButtonList = new List<dfControl>();
    List<dfControl> m_leagueMenuButtonList = new List<dfControl>();
    List<dfControl> m_shopMenuButtonList = new List<dfControl>();


    // Use this for initialization
    void Start()
    {
        m_panelList.Add("MainMenuPanel");
        m_panelList.Add("ShopPanel");
        m_panelList.Add("SchoolPanel");
        m_panelList.Add("LeaguePanel");

        m_mainMenuButtonList.Add(GameObject.Find("ShopButton").GetComponent<dfButton>());
        m_mainMenuButtonList.Add(GameObject.Find("LeaguesButton").GetComponent<dfButton>());
        m_mainMenuButtonList.Add(GameObject.Find("SchoolButton").GetComponent<dfButton>());
        m_mainMenuButtonList.Add(GameObject.Find("LeaveButton").GetComponent<dfButton>());

        SetupMainMenu();
        SetupSchoolMenu();
        SetupLeagueMenu();
        SetupShopMenu();
        
        SwitchPanel(m_panelList[0]);


    }

    void SetupMainMenu()
    {
        m_panelList.Add("MainMenuPanel");
        m_panelList.Add("ShopPanel");
        m_panelList.Add("SchoolPanel");
        m_panelList.Add("LeaguePanel");

        m_mainMenuButtonList.Add(GameObject.Find("ShopButton").GetComponent<dfButton>());
        m_mainMenuButtonList.Add(GameObject.Find("LeaguesButton").GetComponent<dfButton>());
        m_mainMenuButtonList.Add(GameObject.Find("SchoolButton").GetComponent<dfButton>());
        m_mainMenuButtonList.Add(GameObject.Find("LeaveButton").GetComponent<dfButton>());

        GameObject.Find("ShopButton").GetComponent<dfButton>().Click += MainMenuShopButtonClick;
        GameObject.Find("LeaguesButton").GetComponent<dfButton>().Click += MainMenuLeaguesButtonClick;
        GameObject.Find("SchoolButton").GetComponent<dfButton>().Click += MainMenuSchoolButtonClick;
        GameObject.Find("LeaveButton").GetComponent<dfButton>().Click += MainMenuLeaveButtonClick;

    }

    void SetupSchoolMenu()
    {
        m_schoolMenuButtonList.Add(GameObject.Find("SchoolLeaveButton").GetComponent<dfButton>());
        GameObject.Find("SchoolLeaveButton").GetComponent<dfButton>().Click += SchoolMenuLeaveButtonClick;
    }

    void SetupLeagueMenu()
    {
        m_leagueMenuButtonList.Add(GameObject.Find("LeagueLeaveButton").GetComponent<dfButton>());
        GameObject.Find("LeagueLeaveButton").GetComponent<dfButton>().Click += LeagueMenuLeaveButtonClick;

    }

    void SetupShopMenu()
    {
        m_shopMenuButtonList.Add(GameObject.Find("ShopLeaveButton").GetComponent<dfButton>());
        GameObject.Find("ShopLeaveButton").GetComponent<dfButton>().Click += ShopMenuLeaveButtonClick;

    }





    public void HandleUpDownList(List<dfControl> controls, bool up)
    {
        int i = 0;
        // if main menu
        for (i = 0; i < controls.Count; ++i)
        {
            if (controls[i].HasFocus)
            {
                break;
            }
        }

        if (up)
        {
            i = (i + 1) % controls.Count;
        }
        else
        {
            i -= 1;
            if (i < 0)
            {
                i += controls.Count;
            }
        }
        controls[i].Focus();
    }

    public void OnKeyPressed()
    {
        bool up = true;
        HandleUpDownList(m_mainMenuButtonList, up);
    }



    void SchoolMenuLeaveButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel("MainMenuPanel");
    }

    void LeagueMenuLeaveButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel("MainMenuPanel");
    }

    void ShopMenuLeaveButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel("MainMenuPanel");
    }

    void MainMenuLeaveButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {

    }

    void MainMenuLeaguesButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel("LeaguePanel");
    }

    void MainMenuShopButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel("ShopPanel");
    }


    void MainMenuSchoolButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel("SchoolPanel");
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
        //time += Time.deltaTime;
        //if (time > maxTime)
        //{
        //    time = 0f;
        //    panelCounter++;
        //    panelCounter %= m_panelList.Count;
        //    string newPanelName = m_panelList[panelCounter];
        //    SwitchPanel(newPanelName);
        //}
    }


    private TownData m_townData = null;


}



