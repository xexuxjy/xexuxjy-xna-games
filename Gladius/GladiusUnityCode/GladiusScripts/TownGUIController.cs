using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TownGUIController : MonoBehaviour
{
    float time;
    float maxTime = 5f;
    int panelCounter = 0;

    public string activePanel;

    public int m_mainMenuSelection;

    BaseGUIPanel m_currentPanel = null;
    TownMenuPanel m_townPanel;
    SchoolMenuPanel m_schoolPanel;
    LeagueMenuPanel m_leaguePanel;
    ShopMenuPanel m_shopPanel;
    ShopItemMenuPanel m_shopItemPanel;
    CharacterPanel m_characterPanel;
    GladiatorSchool m_school;
    CharacterData m_characterData;

    // Use this for initialization
    void Start()
    {
        SetupMainMenu();
        SetupSchoolMenu();
        SetupLeagueMenu();
        SetupShopMenu();
        SetupCharacterPanel();
        //SwitchPanel(m_townPanel);
        SwitchPanel(m_characterPanel);

        m_school = new GladiatorSchool();
        m_school.Load("Orins-school");
        if (m_school.Gladiators.Count > 0)
        {
            foreach (CharacterData cd in m_school.Gladiators.Values)
            {
                SetCharacterData(cd);
                break;
            }
        }
    }

    void SetupMainMenu()
    {
        m_townPanel = new TownMenuPanel(null, this);
    }


    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        m_currentPanel.ActionPressed(sender, e);
    }


    public void SetupSchoolMenu()
    {
        m_schoolPanel = new SchoolMenuPanel(m_townPanel, this);
    }

    public void SetupLeagueMenu()
    {
        m_leaguePanel = new LeagueMenuPanel(m_townPanel, this);
    }

    public void SetupShopMenu()
    {
        m_shopPanel = new ShopMenuPanel(m_townPanel, this);
    }

    public void SetupCharacterPanel()
    {
        m_characterPanel = new CharacterPanel(m_townPanel, this);

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
        SwitchPanel(m_leaguePanel);
    }

    void MainMenuShopButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel(m_shopPanel);
    }

    void MainMenuSchoolButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        SwitchPanel(m_schoolPanel);
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

    public void SetCharacterData(CharacterData characterData)
    {
        if (characterData != m_characterData)
        {
            m_characterData = characterData;
            m_characterPanel.SetCharacterData(m_characterData);
        }
    }

    public void SetTownData(TownData townData)
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
            if (bgTexture != null)
            {
                GameObject.Find("TownImage").GetComponent<dfTextureSprite>().Texture = bgTexture;
            }


            //m_townMenuPanel.SetData(townData);
            //m_schoolMenuPanel.SetData(townData);
            //m_leagueMenuPanel.SetData(townData);
            //m_shopMenuPanel.SetData(townData);
            //m_shopItemPanel.SetData(townData);




        }
    }

    string lastPanelName = "";
    private Vector3 lastPanelPosition = new Vector3();
    public void SwitchPanel(BaseGUIPanel newPanel)
    {
        if (m_currentPanel != null)
        {
            Debug.Log("Moving panel : " + m_currentPanel.PanelName + " to " + newPanel.PanelName);
            m_currentPanel.m_panel.RelativePosition = lastPanelPosition;
        }

        newPanel.m_panel.Focus();
        lastPanelPosition = newPanel.m_panel.RelativePosition;
        newPanel.m_panel.RelativePosition = new Vector3();
        newPanel.m_panel.BringToFront();
        newPanel.SetTownData(m_townData);
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

    public abstract class BaseGUIPanel
    {
        public TownGUIController m_townGuiController;
        public dfPanel m_panel;
        public int m_currentSelection;
        public BaseGUIPanel m_parentPanel;
        public List<dfButton> m_buttonList = new List<dfButton>();

        public BaseGUIPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
        {
            m_parentPanel = parentPanel;
            m_townGuiController = townGuiController;
        }

        public virtual void SetTownData(TownData townData)
        {
        }

        public virtual void SetCharacterData(CharacterData characterData)
        {

        }

        public virtual void ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }

        public abstract String PanelName
        { get; }
    }

    public class TownMenuPanel : BaseGUIPanel
    {

        public TownMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base(parentPanel, townGuiController)
        {
            m_panel = GameObject.Find("MainMenuPanel").GetComponent<dfPanel>();
            m_buttonList.Add(GameObject.Find("ShopButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("LeaguesButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("SchoolButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("LeaveButton").GetComponent<dfButton>());

            m_buttonList[0].Click += TownMenuPanel_Click;
            m_buttonList[1].Click += TownMenuPanel_Click;
            m_buttonList[2].Click += TownMenuPanel_Click;
            m_buttonList[3].Click += TownMenuPanel_Click;
        }

        public override string PanelName
        {
            get { return "MainMenuPanel"; }
        }

        void TownMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ShopButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_shopPanel);
            }
            else if (control.gameObject.name == "SchoolButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_schoolPanel);
            }
            else if (control.gameObject.name == "LeagueButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_leaguePanel);
            }
            else if (control.gameObject.name == "LeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
        }

        public override void ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }

        public override void SetTownData(TownData townData)
        {
            GameObject.Find("TownTitleLabel").GetComponent<dfRichTextLabel>().Text = townData.Name;
            GameObject.Find("ShopButton").GetComponent<dfButton>().Text = townData.Shop.Name;
            GameObject.Find("LeaguesButton").GetComponent<dfButton>().Text = townData.Arena;
            GameObject.Find("SchoolButton").GetComponent<dfButton>().Text = "School";
            GameObject.Find("LeaveButton").GetComponent<dfButton>().Text = "Leave Town";
        }

    }


    public class ShopMenuPanel : BaseGUIPanel
    {
        public ShopMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base(parentPanel, townGuiController)
        {
            m_panel = GameObject.Find("ShopPanel").GetComponent<dfPanel>();
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
            townGuiController.m_shopItemPanel = new ShopItemMenuPanel(this, townGuiController);

        }

        public override string PanelName
        {
            get { return "ShopMenuPanel"; }
        }


        void ShopMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ShopItemButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_shopItemPanel);
            }
            else if (control.gameObject.name == "ShopLeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
        }

        public override void SetTownData(TownData townData)
        {
            GameObject.Find("ShopPanel/Label").GetComponent<dfLabel>().Text = townData.Shop.Name;
            GameObject.Find("ShopPanel/ShopKeeperDialogueLabel").GetComponent<dfRichTextLabel>().Text = GladiusGlobals.GameStateManager.LocalisationData[townData.Shop.Opening];
            GameObject.Find("ShopPanel/TownPicture/Texture").GetComponent<dfTextureSprite>().Texture = Resources.Load<Texture>(townData.Shop.Image);
        }

    }

    public class ShopItemMenuPanel : BaseGUIPanel
    {
        public ShopItemMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base(parentPanel, townGuiController)
        {
            m_panel = GameObject.Find("ShopItemPanel").GetComponent<dfPanel>();
            m_buttonList.Add(GameObject.Find("ShopLeaveButton").GetComponent<dfButton>());
            GameObject.Find("ShopLeaveButton").GetComponent<dfButton>().Click += ShopItemMenuPanel_Click;
        }

        public override string PanelName
        {
            get { return "ShopItemPanel"; }
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

        public LeagueMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base(parentPanel, townGuiController)
        {
            m_panel = GameObject.Find("LeaguePanel").GetComponent<dfPanel>();
            m_buttonList.Add(GameObject.Find("LeagueLeaveButton").GetComponent<dfButton>());
            GameObject.Find("LeagueLeaveButton").GetComponent<dfButton>().Click += LeagueMenuPanel_Click;
        }

        public override string PanelName
        {
            get { return "LeaguePanel"; }
        }

        void LeagueMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "LeagueLeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
        }

        public override void SetTownData(TownData townData)
        {

        }
    }

    public class SchoolMenuPanel : BaseGUIPanel
    {
        public SchoolMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base(parentPanel, townGuiController)
        {
            m_panel = GameObject.Find("SchoolPanel").GetComponent<dfPanel>();
            m_buttonList.Add(GameObject.Find("SchoolLeaveButton").GetComponent<dfButton>());
            GameObject.Find("SchoolLeaveButton").GetComponent<dfButton>().Click += SchoolMenuPanel_Click;
        }

        public override string PanelName
        {
            get { return "SchoolPanel"; }
        }

        void SchoolMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "SchoolLeaveButton")
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
        }

        public override void SetTownData(TownData townData)
        {

        }

    }


    public class CharacterPanel : BaseGUIPanel
    {
        public CharacterPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base(parentPanel, townGuiController)
        {
            m_panel = GameObject.Find("CharacterPanel").GetComponent<dfPanel>();
            GameObject.Find(PanelName + "/StatsPanel/LevelPanel/Label").GetComponent<dfLabel>().Text = "LVL";
            GameObject.Find(PanelName + "/StatsPanel/XPPanel/Label").GetComponent<dfLabel>().Text = "XP";
            GameObject.Find(PanelName + "/StatsPanel/NextXPPanel/Label").GetComponent<dfLabel>().Text = "NEXT";
            GameObject.Find(PanelName + "/StatsPanel/HPPanel/Label").GetComponent<dfLabel>().Text = "HP";
            GameObject.Find(PanelName + "/StatsPanel/DAMPanel/Label").GetComponent<dfLabel>().Text = "DAM";
            GameObject.Find(PanelName + "/StatsPanel/PWRPanel/Label").GetComponent<dfLabel>().Text = "PWR";
            GameObject.Find(PanelName + "/StatsPanel/ACCPanel/Label").GetComponent<dfLabel>().Text = "ACC";
            GameObject.Find(PanelName + "/StatsPanel/DEFPanel/Label").GetComponent<dfLabel>().Text = "DEF";
            GameObject.Find(PanelName + "/StatsPanel/INIPanel/Label").GetComponent<dfLabel>().Text = "INI";
            GameObject.Find(PanelName + "/StatsPanel/CONPanel/Label").GetComponent<dfLabel>().Text = "CON";
            GameObject.Find(PanelName + "/StatsPanel/MOVPanel/Label").GetComponent<dfLabel>().Text = "MOV";
            GameObject.Find(PanelName + "/StatsPanel/ArmourPanel/Label").GetComponent<dfLabel>().Text = "Arm";
            GameObject.Find(PanelName + "/StatsPanel/WeaponPanel/Label").GetComponent<dfLabel>().Text = "Wpn";
        }

        public override string PanelName
        {
            get { return "CharacterPanel"; }
        }

        public override void SetCharacterData(CharacterData characterData)
        {
            GameObject.Find(PanelName + "/StatsPanel/LevelPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.Level;
            GameObject.Find(PanelName + "/StatsPanel/XPPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.XP;
            GameObject.Find(PanelName + "/StatsPanel/NextXPPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.NEXTXP;
            GameObject.Find(PanelName + "/StatsPanel/HPPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.HP;
            GameObject.Find(PanelName + "/StatsPanel/DAMPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.DAM;
            GameObject.Find(PanelName + "/StatsPanel/PWRPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.PWR;
            GameObject.Find(PanelName + "/StatsPanel/ACCPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.ACC;
            GameObject.Find(PanelName + "/StatsPanel/DEFPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.DEF;
            GameObject.Find(PanelName + "/StatsPanel/INIPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.INI;
            GameObject.Find(PanelName + "/StatsPanel/CONPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.CON;
            GameObject.Find(PanelName + "/StatsPanel/MOVPanel/Value").GetComponent<dfLabel>().Text = "" + characterData.MOV;
            GameObject.Find(PanelName + "/StatsPanel/ArmourPanel/Value").GetComponent<dfLabel>().Text = "" + "armour";
            GameObject.Find(PanelName + "/StatsPanel/WeaponPanel/Value").GetComponent<dfLabel>().Text = "" + "weapon";

        }




    }


}






