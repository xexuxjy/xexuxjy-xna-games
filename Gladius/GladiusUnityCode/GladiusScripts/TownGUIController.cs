using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TownGUIController : MonoBehaviour
{
    float time;
    float maxTime = 5f;
    int panelCounter = 0;

    BaseGUIPanel m_currentPanel = null;
    TownMenuPanel m_townPanel;
    SchoolMenuPanel m_schoolPanel;
    ArenaMenuPanel m_arenaMenuPanel;
    LeagueMenuPanel m_leaguePanel;
    ShopMenuPanel m_shopPanel;
    ShopItemMenuPanel m_shopItemPanel;
    CharacterPanel m_characterPanel;
    EncounterPanel m_encounterPanel;

    GladiatorSchool m_school;
    CharacterData m_currentCharacterData;
    LeagueData m_selectedLeagueData;
    Encounter m_selectedEncounter;
    
    // Use this for initialization
    void Start()
    {
        SetupMainMenu();
        SetupPanels();
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


    public void SetupPanels()
    {
        m_schoolPanel = new SchoolMenuPanel(m_townPanel, this);
        m_arenaMenuPanel = new ArenaMenuPanel(m_townPanel, this);
        m_shopPanel = new ShopMenuPanel(m_townPanel, this);
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
        if (characterData != m_currentCharacterData)
        {
            m_currentCharacterData = characterData;
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
            m_currentPanel.PanelDeactivated();
            m_currentPanel.m_panel.RelativePosition = lastPanelPosition;
            m_currentPanel.m_panel.IsVisible = false;
        }
        if (newPanel != null)
        {
            newPanel.m_panel.Focus();
            lastPanelPosition = newPanel.m_panel.RelativePosition;
            newPanel.m_panel.RelativePosition = new Vector3();
            newPanel.m_panel.BringToFront();
            newPanel.PanelActivated();
            //newPanel.SetTownData(m_townData);
            newPanel.m_panel.IsVisible = true;
        }
        m_currentPanel = newPanel;
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
        protected TownData m_townData;

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
            //m_panel.gameObject.SetActive(false);
            //m_panel.gameObject.GetComponent<Renderer>().enabled = false;
            m_panel.IsVisible = false;
            m_panel.IsVisibleChanged += m_panel_IsVisibleChanged;
        }

        public virtual void PanelActivated()
        {
        }

        public virtual void PanelDeactivated()
        {

        }

        public TownData TownData
        { get { return m_townGuiController.m_townData; } }

        public CharacterData CharacterData
        { get { return m_townGuiController.m_currentCharacterData; } }

        void m_panel_IsVisibleChanged(dfControl control, bool value)
        {
            if (value == true)
            {
                int ibreak = 0;
            }
        }

        public void leaveButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (control.gameObject.name == m_leaveName)
            {
                m_townGuiController.SwitchPanel(m_parentPanel);
            }
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
            m_buttonList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/ShopButton"));
            m_buttonList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/ArenaButton"));
            m_buttonList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/SchoolButton"));
            m_buttonList.Add(m_panel.FindPath<dfButton>("TownMenuPanel/LeaveButton"));


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

        public override void PanelActivated()
        {
            m_panel.Find<dfRichTextLabel>("TownTitleLabel").Text = TownData.Name;
            m_panel.Find<dfButton>("ShopButton").Text = TownData.Shop.Name;
            m_panel.Find<dfButton>("ArenaButton").Text = TownData.ArenaData.ArenaName;
            m_panel.Find<dfButton>("SchoolButton").Text = "School";
            m_panel.Find<dfButton>("LeaveButton").Text = "Leave Town";

            string regionTextureName = "GladiusUI/TownOverland/TownBackground/";
            switch (TownData.Region)
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
            m_panel.Find<dfTextureSprite>("TownImage").Texture = Resources.Load<Texture2D>(TownData.BackgroundTextureName);


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

        public override void PanelActivated()
        {
            m_panel.Find<dfLabel>("Label").Text = TownData.Shop.Name;
            m_panel.Find<dfRichTextLabel>("ShopKeeperDialogueLabel").Text = GladiusGlobals.GameStateManager.LocalisationData[TownData.Shop.Opening];
            m_panel.FindPath<dfTextureSprite>("TownPicture/Texture").Texture = Resources.Load<Texture>(TownData.Shop.Image);
        }

    }

    public class ShopItemMenuPanel : BaseGUIPanel
    {
        public ShopItemMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("ShopItemPanel",parentPanel, townGuiController)
        {
        }

        public override void PanelActivated()
        {
            m_panel.GetComponent<ItemPanelController>().SetData(TownData.Shop);
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

        public override void PanelActivated()
        {
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            m_panel.Find<dfLabel>("LeagueName").Text = TownData.ArenaData.ArenaName;
            m_panel.Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(TownData.ArenaData.BackgroundTextureName);
            m_panel.Find<dfTextureSprite>("OwnerThumbnail").Texture = Resources.Load<Texture2D>(TownData.ArenaData.OwnerThumbnailName);
        }
    }



    public class LeagueMenuPanel : BaseGUIPanel
    {
        public GameObject SlotPrefab;
        private List<dfPanel> m_leaguePanels = new List<dfPanel>();
        int SlotsH = 3;
        int SlotsV = 2;
        public LeagueMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("LeaguePanel",parentPanel, townGuiController)
        {
            SlotPrefab = Resources.Load("Prefabs/LeagueSlotPrefab") as GameObject;
            townGuiController.m_encounterPanel = new EncounterPanel(this, townGuiController);
            dfPanel leagueDisplayPanel = m_panel.Find<dfPanel>("LeagueDisplay");
            int width = (int)(leagueDisplayPanel.Width / SlotsH);
            int height = (int)(leagueDisplayPanel.Height / SlotsV);

            int counter = 0;
            //string background1 = "king of the hill ke.tga";
            //string background2 = "mystical zo.tga";
            for (int i = 0; i < SlotsV; ++i)
            {
                for (int j = 0; j < SlotsH; ++j)
                {
                    int index = (i * SlotsH) + j;

                    dfPanel panel = leagueDisplayPanel.AddPrefab(SlotPrefab) as dfPanel;
                    m_leaguePanels.Add(panel);
                    panel.Width = width;
                    panel.Height = height;
                    //panel.Position = new Vector3();
                    panel.RelativePosition = new Vector3(j * width, i * height);
                    panel.Click += panel_Click;
                }
            }
        }

        public override void PanelActivated()
        {
            dfPanel headerPanel = m_panel.Find<dfPanel>("LeagueHeaderPanel");
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            headerPanel.Find<dfRichTextLabel>("ArenaName").Text = TownData.ArenaData.ArenaName;
            headerPanel.Find<dfSprite>("OwnerThumbnail").SpriteName = TownData.ArenaData.OwnerThumbnailName;

            dfPanel leagueDisplayPanel = m_panel.Find<dfPanel>("LeagueDisplay");
            int width = (int)(leagueDisplayPanel.Width / SlotsH);
            int height = (int)(leagueDisplayPanel.Height / SlotsV);
           
            int counter = 0;
            //string background1 = "king of the hill ke.tga";
            //string background2 = "mystical zo.tga";
            for (int i = 0; i < SlotsV; ++i)
            {
                for (int j = 0; j < SlotsH; ++j)
                {
                    int index = (i * SlotsH) + j;
                    dfPanel panel = m_leaguePanels[index];

                    if (index < TownData.ArenaData.Leagues.Count)
                    {
                        panel.enabled = true;
                        LeagueData leagueData = TownData.ArenaData.Leagues[index];
                        leagueData.ImageName = leagueData.Name + ".tga";//(index % 2 == 0) ? background1 : background2;
                        panel.Width = width;
                        panel.Height = height;
                        panel.Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
                        panel.Find<dfRichTextLabel>("LeagueStatus").Text = leagueData.Name;
                        panel.Tag = leagueData;
                    }
                    else
                    {
                        panel.IsVisible = false;
                    }
                }
            }
            m_townGuiController.m_selectedLeagueData = null;
        }

        void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            LeagueData leagueData = control.Tag as LeagueData;
            if (m_townGuiController.m_selectedLeagueData == leagueData) // we've selected the same one again
            {
                m_townGuiController.SwitchPanel(m_townGuiController.m_encounterPanel);
            }
            m_panel.Find("LeagueDataPanel").Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
            m_panel.Find("LeagueDataPanel").Find<dfRichTextLabel>("LeagueName").Text = GladiusGlobals.GameStateManager.LocalisationData[leagueData.DescriptionId];
            m_townGuiController.m_selectedLeagueData = leagueData;
        }

    }

    public class SchoolMenuPanel : BaseGUIPanel
    {
        public SchoolMenuPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("SchoolPanel",parentPanel, townGuiController)
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

        public override void PanelActivated()
        {
            m_panel.FindPath<dfRichTextLabel>("NameAndClass").GetComponent<dfRichTextLabel>().Text = "" + CharacterData.Name + "\n" + CharacterData.ActorClass;
            //GameObject.FindPath(PanelName + "NameAndClass").GetComponent<dfRichTextLabel>().Text = "" + characterData.Name;
            m_panel.FindPath<dfLabel>("StatsPanel/LevelPanel/Value").Text = "" + CharacterData.Level;
            m_panel.FindPath<dfLabel>("StatsPanel/XPPanel/Value").Text = "" + CharacterData.XP;
            m_panel.FindPath<dfLabel>("StatsPanel/NextXPPanel/Value").Text = "" + CharacterData.NEXTXP;
            m_panel.FindPath<dfLabel>("StatsPanel/HPPanel/Value").Text = "" + CharacterData.HP;
            m_panel.FindPath<dfLabel>("StatsPanel/DAMPanel/Value").Text = "" + CharacterData.DAM;
            m_panel.FindPath<dfLabel>("StatsPanel/PWRPanel/Value").Text = "" + CharacterData.PWR;
            m_panel.FindPath<dfLabel>("StatsPanel/ACCPanel/Value").Text = "" + CharacterData.ACC;
            m_panel.FindPath<dfLabel>("StatsPanel/DEFPanel/Value").Text = "" + CharacterData.DEF;
            m_panel.FindPath<dfLabel>("StatsPanel/INIPanel/Value").Text = "" + CharacterData.INI;
            m_panel.FindPath<dfLabel>("StatsPanel/CONPanel/Value").Text = "" + CharacterData.CON;
            m_panel.FindPath<dfLabel>("StatsPanel/MOVPanel/Value").Text = "" + CharacterData.MOV;
            m_panel.FindPath<dfLabel>("StatsPanel/ArmourPanel/Value").Text = "" + "armour";
            m_panel.FindPath<dfLabel>("StatsPanel/WeaponPanel/Value").Text = "" + "weapon";

            // Try and get a class image?
            Texture2D classTex = Resources.Load<Texture2D>(GladiusGlobals.UIRoot+"ClassImages/"+CharacterData.ActorClassData.MeshName);
            if(classTex != null)
            {
                m_panel.FindPath<dfTextureSprite>("CharacterSprite").Texture = classTex;
            }

        }
    }

    public class TeamSelectionPanel : BaseGUIPanel
    {
        SelectionGrid m_availableGrid;
        SelectionGrid m_selectedGrid;

        public TeamSelectionPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController) : base("TeamSelectionPanel",parentPanel,townGuiController)
        {
            m_availableGrid = m_panel.gameObject.AddComponent<SelectionGrid>();
            m_selectedGrid = m_panel.gameObject.AddComponent<SelectionGrid>();
            m_availableGrid.Init(m_panel.Find("AvailablePanel"), "Available",AvailableGrid_Click);
            m_selectedGrid.Init(m_panel.Find("SelectedPanel"), "Selected",SelectedGrid_Click);
        }

        void AvailableGrid_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {

        }

        void SelectedGrid_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {

        }

    }

    public class EncounterPanel : BaseGUIPanel
    {
        private List<dfPanel> m_encounterPanels = new List<dfPanel>();
        public GameObject SlotPrefab;
        public int NumEncounters = 8;

        public EncounterPanel(BaseGUIPanel parentPanel, TownGUIController townGuiController)
            : base("EncounterPanel", parentPanel, townGuiController)
        {
            SlotPrefab = Resources.Load("Prefabs/EncounterListPrefab") as GameObject;
            
            dfPanel leagueDisplayPanel = m_panel.Find<dfPanel>("EncounterDisplay");
            int width = (int)(leagueDisplayPanel.Width);
            int height = (int)(leagueDisplayPanel.Height / NumEncounters);

            int counter = 0;
            //string background1 = "king of the hill ke.tga";
            //string background2 = "mystical zo.tga";
            for (int i = 0; i < NumEncounters; ++i)
            {
                int index = i;
                dfPanel panel = leagueDisplayPanel.AddPrefab(SlotPrefab) as dfPanel;
                m_encounterPanels.Add(panel);
                panel.Width = width;
                panel.Height = height;
                //panel.Position = new Vector3();
                panel.RelativePosition = new Vector3(0, i * height);
                panel.Click += panel_Click;
            }
        }

        public override void PanelActivated()
        {
            dfPanel headerPanel = m_panel.Find<dfPanel>("EncounterHeaderPanel");
            //m_panel.Find<dfLabel>("Label").Text = "League Data";
            headerPanel.Find<dfRichTextLabel>("ArenaName").Text = TownData.ArenaData.ArenaName;
            headerPanel.Find<dfSprite>("OwnerThumbnail").SpriteName = TownData.ArenaData.OwnerThumbnailName;

            // use the data in selected league to populate the encounter
            m_panel.Find<dfTextureSprite>("EncounterImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + m_townGuiController.m_selectedLeagueData.ImageName);
            m_panel.Find<dfRichTextLabel>("EncounterName").Text = "";

            for (int i = 0; i < NumEncounters; ++i)
            {
                dfPanel panel = m_encounterPanels[i];
                if (i < m_townGuiController.m_selectedLeagueData.ArenaEncounters.Count)
                {
                    panel.IsVisible = true;
                    ArenaEncounter encounter = m_townGuiController.m_selectedLeagueData.ArenaEncounters[i];
                    panel.Find<dfRichTextLabel>("Name").Text = GladiusGlobals.GameStateManager.LocalisationData[encounter.Id]; 
                    //panel.Find<dfTextureSprite>("Points").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
                    panel.Tag = encounter;
                }
                else
                {
                    panel.IsVisible = false;
                }

            }


        }

        void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            ArenaEncounter encounter = control.Tag as ArenaEncounter;
            //m_panel.Find("EncounterDataPanel").Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
            String info = GladiusGlobals.GameStateManager.LocalisationData[encounter.EncounterDescId];
            m_panel.Find("EncounterDataPanel").Find<dfRichTextLabel>("EncounterName").Text = info;
        }
    }
}








