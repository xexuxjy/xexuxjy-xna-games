using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TownGUIController : MonoBehaviour
{
    float time;
    float maxTime = 5f;
    int panelCounter = 0;
    //List<string> m_panelList = new List<string>();

    //List<dfControl> m_mainMenuButtonList = new List<dfControl>();
    //List<dfControl> m_schoolMenuButtonList = new List<dfControl>();
    //List<dfControl> m_leagueMenuButtonList = new List<dfControl>();
    //List<dfControl> m_shopMenuButtonList = new List<dfControl>();

    public string activePanel;

    public int m_mainMenuSelection;

    BaseGUIPanel m_currentPanel;
    TownMenuPanel m_townMenuPanel;
    SchoolMenuPanel m_schoolMenuPanel;
    LeagueMenuPanel m_leagueMenuPanel;
    ShopMenuPanel m_shopMenuPanel;
    ShopItemMenuPanel m_shopItemPanel;

    // Use this for initialization
    void Start()
    {
        //m_panelList.Add("MainMenuPanel");
        //m_panelList.Add("ShopPanel");
        //m_panelList.Add("SchoolPanel");
        //m_panelList.Add("LeaguePanel");
        //m_panelList.Add("ShopItemPanel");

        SetupMainMenu();
        SetupSchoolMenu();
        SetupLeagueMenu();
        SetupShopMenu();
        
        //SwitchPanel(m_panelList[0]);
        //SwitchPanel("ShopItemPanel");
        SwitchPanel("MainMenuPanel");

    }

    void SetupMainMenu()
    {
        //m_panelList.Add("MainMenuPanel");


        GameObject.Find("ShopButton").GetComponent<dfButton>().Click += MainMenuShopButtonClick;
        GameObject.Find("LeaguesButton").GetComponent<dfButton>().Click += MainMenuLeaguesButtonClick;
        GameObject.Find("SchoolButton").GetComponent<dfButton>().Click += MainMenuSchoolButtonClick;
        GameObject.Find("LeaveButton").GetComponent<dfButton>().Click += MainMenuLeaveButtonClick;

        EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);
    }


    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        m_currentPanel.ActionPressed(sender, e);
    }


    public void SetupSchoolMenu()
    {
        m_schoolMenuPanel = new SchoolMenuPanel("MainMenuPanel", this);
    }

    public void SetupLeagueMenu()
    {
        m_leagueMenuPanel = new LeagueMenuPanel("MainMenuPanel", this);
    }

    public void SetupShopMenu()
    {
        m_shopMenuPanel = new ShopMenuPanel("MainMenuPanel", this);
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
        //HandleUpDownList(m_mainMenuButtonList, up);
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

    
    
    public void MainMenuAction(object sender, ActionButtonPressedArgs e)
    {
        switch (e.ActionButton)
        {
            case (ActionButton.Move1Up):
                {
                    m_mainMenuSelection++;
                    //m_mainMenuSelection %= m
                    break;
                }
            case (ActionButton.Move1Down):
                {
                    break;
                }
        }
            


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


            GameObject.Find("ShopItemPanel").GetComponent<ItemPanelController>().SetData(m_townData.Shop);


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

    public class BaseGUIPanel
    {
        public TownGUIController m_townGuiController;
        public int m_currentSelection;
        public String m_parentPanel;
        public List<dfButton> m_buttonList = new List<dfButton>();

        public BaseGUIPanel(String parentPanel, TownGUIController townGuiController)
        {
            m_parentPanel = parentPanel;
            m_townGuiController = townGuiController;
        }

        public virtual void ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }

    }

    public class TownMenuPanel : BaseGUIPanel
    {

        public TownMenuPanel(String parentPanel, TownGUIController townGuiController)
            : base(parentPanel,townGuiController)
        {
            m_buttonList.Add(GameObject.Find("ShopButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("LeaguesButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("SchoolButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("LeaveButton").GetComponent<dfButton>());

            m_buttonList[0].Click += TownMenuPanel_Click;
            m_buttonList[1].Click += TownMenuPanel_Click;
            m_buttonList[2].Click += TownMenuPanel_Click;
            m_buttonList[3].Click += TownMenuPanel_Click;


        }

        void TownMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ShopButton")
            {
                m_townGuiController.SwitchPanel("ShopPanel");
            }
            else if (control.gameObject.name == "SchoolButton")
            {
                m_townGuiController.SwitchPanel("SchoolPanel");
            }
            else if (control.gameObject.name == "LeagueButton")
            {
                m_townGuiController.SwitchPanel("LeaguePanel");
            }
            else if (control.gameObject.name == "LeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
        }

        public override void ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }


    }


    public class ShopMenuPanel : BaseGUIPanel
    {
        public const string PanelName = "ShopMenuPanel";
        public ShopMenuPanel(String parentPanel, TownGUIController townGuiController)
            : base(parentPanel,townGuiController)
        {
            m_buttonList.Add(GameObject.Find("ShopItemButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("ShopChatButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("ShopLeaveButton").GetComponent<dfButton>());

            foreach (dfControl dfc in m_buttonList)
            {
                dfButton button = dfc as dfButton;
                if (button != null)
                {
                    button.Click += ShopMenuPanel_Click;
                }
            }
            townGuiController.m_shopItemPanel = new ShopItemMenuPanel(PanelName, townGuiController);

        }

        void ShopMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ShopItemButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_shopItemPanel.PanelName);
            }
            else if (control.gameObject.name == "ShopLeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
        }


    }

    public class ShopItemMenuPanel : BaseGUIPanel
    {
        public string PanelName = "ShopItemPanel";
        public ShopItemMenuPanel(String parentPanel, TownGUIController townGuiController)
            : base(parentPanel, townGuiController)
        {
            m_buttonList.Add(GameObject.Find("ShopLeaveButton").GetComponent<dfButton>());
            GameObject.Find("ShopLeaveButton").GetComponent<dfButton>().Click += ShopItemMenuPanel_Click;
        }

        void ShopItemMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ShopItemLeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }

        }
    }

    public class LeagueMenuPanel : BaseGUIPanel
    {
        public string PanelName = "LeagueMenuPanel";
        public LeagueMenuPanel(String parentPanel, TownGUIController townGuiController)
            : base(parentPanel,townGuiController)
        {
            m_buttonList.Add(GameObject.Find("LeagueLeaveButton").GetComponent<dfButton>());
            GameObject.Find("LeagueLeaveButton").GetComponent<dfButton>().Click += LeagueMenuPanel_Click;
        }

        void LeagueMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "LeagueLeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }

        }
    }

    public class SchoolMenuPanel : BaseGUIPanel
    {
        public string PanelName = "SchoolMenuPanel";
        public SchoolMenuPanel(String parentPanel, TownGUIController townGuiController)
            : base(parentPanel,townGuiController)
        {
            m_buttonList.Add(GameObject.Find("SchoolLeaveButton").GetComponent<dfButton>());
            GameObject.Find("SchoolLeaveButton").GetComponent<dfButton>().Click += SchoolMenuPanel_Click;
        }

        void SchoolMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "SchoolLeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }

        }
    }


}






