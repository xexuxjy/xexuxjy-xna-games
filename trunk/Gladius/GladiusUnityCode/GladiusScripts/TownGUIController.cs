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
    ArenaMenuPanel m_arenaMenuPanel;
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
        SetupArenaMenu();
        SetupShopMenu();
        SetupCharacterPanel();
        SwitchPanel(m_townPanel);
        //SwitchPanel(m_characterPanel);
        //SwitchPanel(m_shopItemPanel);

        m_school = new GladiatorSchool();
        //m_school.Load("Orins-school");
        m_school.Load("Legionaires-School");
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

    public void SetupArenaMenu()
    {
        m_arenaMenuPanel = new ArenaMenuPanel(m_townPanel, this);
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

    void MainMenuLeagueButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
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
        if (newPanel != null)
        {
            newPanel.m_panel.Focus();
            lastPanelPosition = newPanel.m_panel.RelativePosition;
            newPanel.m_panel.RelativePosition = new Vector3();
            newPanel.m_panel.BringToFront();
            newPanel.SetTownData(m_townData);
        }
    }


    public void Update()
    {
    }


    private TownData m_townData = null;

    public abstract class BaseGUIPanel
    {
        public TownGUIController m_townGuiController;
        public dfPanel m_panel;
        public String m_panelName;
        public int m_currentSelection;
        public BaseGUIPanel m_parentPanel;
        public List<dfButton> m_buttonList = new List<dfButton>();

        public String m_leaveName = "LeaveButton";

        public BaseGUIPanel(String panelName,BaseGUIPanel parentPanel, TownGUIController townGuiController)
        {
            m_panelName = panelName;
            m_parentPanel = parentPanel;
            m_townGuiController = townGuiController;
            m_panel = GameObject.Find(m_panelName).GetComponent<dfPanel>();

            dfControl leaveButton = m_panel.Find(m_leaveName);
            if (leaveButton != null)
            {
                leaveButton.Click += leaveButton_Click;
            }
        }

        public void leaveButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == m_leaveName)
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
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

        public String PanelName
        { get { return m_panelName; } }
    }

    public class TownMenuPanel : BaseGUIPanel
    {

        public TownMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("MainMenuPanel",parentPanel, townGuiController)
        {
            m_buttonList.Add(GameObject.Find("ShopButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("ArenaButton").GetComponent<dfButton>());
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
                m_townGuiController.SwitchPanel(m_townGuiController.m_shopPanel);
            }
            else if (control.gameObject.name == "SchoolButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_schoolPanel);
            }
            else if (control.gameObject.name == "ArenaButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_arenaMenuPanel);
            }
        }

        public override void ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }

        public override void SetTownData(TownData townData)
        {
            m_panel.Find<dfRichTextLabel>("TownTitleLabel").Text = townData.Name;
            m_panel.Find<dfButton>("ShopButton").Text = townData.Shop.Name;
            m_panel.Find<dfButton>("ArenaButton").Text = townData.ArenaData.ArenaName;
            m_panel.Find<dfButton>("SchoolButton").Text = "School";
            m_panel.Find<dfButton>("LeaveButton").Text = "Leave Town";

            string regionTextureName = "GladiusUI/TownOverland/TownBackground/";
            switch (townData.Region)
            {
                case "expanse":
                    regionTextureName += "town_headere.tga";
                    break;
                case "imperia":
                    regionTextureName += "town_headeri.tga";
                    break;
                case "nordargh":
                    regionTextureName += "town_headern.tga";
                    break;
                case "steppes":
                    regionTextureName += "town_headers.tga";
                    break;
            }
            m_panel.Find<dfTextureSprite>("TownRegionImage").Texture = Resources.Load<Texture2D>(regionTextureName);
            m_panel.Find<dfTextureSprite>("TownImage").Texture = Resources.Load<Texture2D>(townData.BackgroundTextureName);


        }

    }


    public class ShopMenuPanel : BaseGUIPanel
    {
        public ShopMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("ShopPanel",parentPanel, townGuiController)
        {
            m_buttonList.Add(GameObject.Find("ShopItemButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("ShopChatButton").GetComponent<dfButton>());
            //m_buttonList.Add(GameObject.Find("ShopLeaveButton").GetComponent<dfButton>());

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

        void ShopMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ShopItemButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_shopItemPanel);
            }
        }

        public override void SetTownData(TownData townData)
        {
            m_panel.Find<dfLabel>("Label").Text = townData.Shop.Name;
            m_panel.Find<dfRichTextLabel>("ShopKeeperDialogueLabel").Text = GladiusGlobals.GameStateManager.LocalisationData[townData.Shop.Opening];
            m_panel.FindPath<dfTextureSprite>("TownPicture/Texture").Texture = Resources.Load<Texture>(townData.Shop.Image);
        }

    }

    public class ShopItemMenuPanel : BaseGUIPanel
    {
        public ShopItemMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("ShopItemPanel",parentPanel, townGuiController)
        {
        }

        public override void SetTownData(TownData townData)
        {
            m_panel.GetComponent<ItemPanelController>().SetData(townData.Shop);
        }
    }

    public class ArenaMenuPanel : BaseGUIPanel
    {

        public ArenaMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("ArenaPanel", parentPanel, townGuiController)
        {
            townGuiController.m_leaguePanel = new LeagueMenuPanel(this, townGuiController);
            m_buttonList.Add(GameObject.Find("ChampionshipButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("TournamentButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("LeaguesButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("RecruitingButton").GetComponent<dfButton>());
            m_buttonList.Add(GameObject.Find("HistoryButton").GetComponent<dfButton>());

            m_buttonList[0].Click += ArenaMenuPanel_Click;
            m_buttonList[1].Click += ArenaMenuPanel_Click;
            m_buttonList[2].Click += ArenaMenuPanel_Click;
            m_buttonList[3].Click += ArenaMenuPanel_Click;
            m_buttonList[4].Click += ArenaMenuPanel_Click;

        }

        void ArenaMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == "ChampionshipButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_leaguePanel);
            }
            if (control.gameObject.name == "TournamentButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_leaguePanel);
            }
            if (control.gameObject.name == "LeaguesButton")
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_leaguePanel);
            }
            
        }

        public override void SetTownData(TownData townData)
        {
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            m_panel.Find<dfLabel>("LeagueName").Text = townData.ArenaData.ArenaName;
            m_panel.Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(townData.ArenaData.BackgroundTextureName);
            m_panel.Find<dfTextureSprite>("OwnerThumbnail").Texture = Resources.Load<Texture2D>(townData.ArenaData.OwnerThumbnailName);
        }
    }



    public class LeagueMenuPanel : BaseGUIPanel
    {

        public LeagueMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("LeaguePanel",parentPanel, townGuiController)
        {
        }

        public override void SetTownData(TownData townData)
        {
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            m_panel.Find<dfLabel>("Label").Text = townData.ArenaData.ArenaName;

        }
    }

    public class SchoolMenuPanel : BaseGUIPanel
    {
        public SchoolMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("SchoolPanel",parentPanel, townGuiController)
        {
        }

        public override void SetTownData(TownData townData)
        {

        }

    }


    public class CharacterPanel : BaseGUIPanel
    {
        public CharacterPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("CharacterPanel",parentPanel, townGuiController)
        {
            m_panel.FindPath<dfLabel>("StatsPanel/LevelPanel/Label").Text = "LVL";
            m_panel.FindPath<dfLabel>("StatsPanel/XPPanel/Label").Text = "XP";
            m_panel.FindPath<dfLabel>("StatsPanel/NextXPPanel/Label").Text = "NEXT";
            m_panel.FindPath<dfLabel>("StatsPanel/HPPanel/Label").Text = "HP";
            m_panel.FindPath<dfLabel>("StatsPanel/DAMPanel/Label").Text = "DAM";
            m_panel.FindPath<dfLabel>("StatsPanel/PWRPanel/Label").Text = "PWR";
            m_panel.FindPath<dfLabel>("StatsPanel/ACCPanel/Label").Text = "ACC";
            m_panel.FindPath<dfLabel>("StatsPanel/DEFPanel/Label").Text = "DEF";
            m_panel.FindPath<dfLabel>("StatsPanel/INIPanel/Label").Text = "INI";
            m_panel.FindPath<dfLabel>("StatsPanel/CONPanel/Label").Text = "CON";
            m_panel.FindPath<dfLabel>("StatsPanel/MOVPanel/Label").Text = "MOV";
            m_panel.FindPath<dfLabel>("StatsPanel/ArmourPanel/Label").Text = "Arm";
            m_panel.FindPath<dfLabel>("StatsPanel/WeaponPanel/Label").Text = "Wpn";
        }

        public override void SetCharacterData(CharacterData characterData)
        {
            m_panel.FindPath<dfRichTextLabel>("NameAndClass").GetComponent<dfRichTextLabel>().Text = "" + characterData.Name + "\n" + characterData.ActorClass;
            //GameObject.FindPath(PanelName + "NameAndClass").GetComponent<dfRichTextLabel>().Text = "" + characterData.Name;
            m_panel.FindPath<dfLabel>("StatsPanel/LevelPanel/Value").Text = "" + characterData.Level;
            m_panel.FindPath<dfLabel>("StatsPanel/XPPanel/Value").Text = "" + characterData.XP;
            m_panel.FindPath<dfLabel>("StatsPanel/NextXPPanel/Value").Text = "" + characterData.NEXTXP;
            m_panel.FindPath<dfLabel>("StatsPanel/HPPanel/Value").Text = "" + characterData.HP;
            m_panel.FindPath<dfLabel>("StatsPanel/DAMPanel/Value").Text = "" + characterData.DAM;
            m_panel.FindPath<dfLabel>("StatsPanel/PWRPanel/Value").Text = "" + characterData.PWR;
            m_panel.FindPath<dfLabel>("StatsPanel/ACCPanel/Value").Text = "" + characterData.ACC;
            m_panel.FindPath<dfLabel>("StatsPanel/DEFPanel/Value").Text = "" + characterData.DEF;
            m_panel.FindPath<dfLabel>("StatsPanel/INIPanel/Value").Text = "" + characterData.INI;
            m_panel.FindPath<dfLabel>("StatsPanel/CONPanel/Value").Text = "" + characterData.CON;
            m_panel.FindPath<dfLabel>("StatsPanel/MOVPanel/Value").Text = "" + characterData.MOV;
            m_panel.FindPath<dfLabel>("StatsPanel/ArmourPanel/Value").Text = "" + "armour";
            m_panel.FindPath<dfLabel>("StatsPanel/WeaponPanel/Value").Text = "" + "weapon";

            // Try and get a class image?
            Texture2D classTex = Resources.Load<Texture2D>(GladiusGlobals.UIRoot+"ClassImages/"+characterData.ActorClassData.MeshName);
            if(classTex != null)
            {
                m_panel.FindPath<dfTextureSprite>("CharacterSprite").Texture = classTex;
            }

        }




    }


}






