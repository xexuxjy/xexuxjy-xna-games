using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TownGUIController : MonoBehaviour
{
    float time;
    float maxTime = 5f;
    int panelCounter = 0;

    private bool m_setup;


    BaseGUIPanel m_currentPanel = null;
    public GameObject m_townPanel;
    public GameObject m_schoolPanel;
    public GameObject m_arenaMenuPanel;
    public GameObject m_leaguePanel;
    public GameObject m_shopPanel;
    public GameObject m_shopItemPanel;
    public GameObject m_encounterPanel;
    public GameObject m_teamSelectionPanel;
    public GameObject m_shopCharacterPanel;
    public GameObject m_popupPanel;

    public GameObject m_modelWindowHolder;

    private List<GameObject> AvailablePanels = new List<GameObject>();

    //CharacterData m_currentCharacterData;
    CharacterDataList m_characterDataList = new CharacterDataList();
    LeagueData m_selectedLeagueData;

    ArenaEncounter m_arenaEncounter;


    public CharacterDataList CharacterList
    { get { return m_characterDataList; } }

    public ArenaEncounter ArenaEncounter
    {
        get { return m_arenaEncounter; }
        set
        {
            m_arenaEncounter = value;
        }
    }

    public GladiatorSchool GladiatorSchool
    {
        get { return GladiusGlobals.GladiatorSchool; }
    }

    // Use this for initialization
    void SetupPanels()
    {
        AddIfAvailable(m_townPanel);
        AddIfAvailable(m_schoolPanel);
        AddIfAvailable(m_arenaMenuPanel);
        AddIfAvailable(m_leaguePanel);
        AddIfAvailable(m_shopPanel);
        AddIfAvailable(m_shopItemPanel);
        AddIfAvailable(m_encounterPanel);
        AddIfAvailable(m_teamSelectionPanel);
        AddIfAvailable(m_shopCharacterPanel);
        AddIfAvailable(m_popupPanel);

        SetStateOnAllPanels(false);
        SwitchPanel(TownPanel);
    }

    private void AddIfAvailable(GameObject panel)
    {
        if (panel != null)
        {
            AvailablePanels.Add(panel);
        }
    }

    public void Update()
    {
        if (!m_setup && GameObject.Find("TownStartup").GetComponent<TownStartup>().TownDataReady)
        {
            m_setup = true;
            m_characterDataList.SetData(GladiatorSchool.Gladiators);
            SetupPanels();
        }
    }

    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        m_currentPanel.ActionPressed(sender, e);
    }


    void SetStateOnAllPanels(bool active)
    {
        foreach (GameObject go in AvailablePanels)
        {
            go.SetActive(active);
        }
    }


public void HandleUpDownList(List<Selectable> controls, bool up)
    {
        //int i = 0;
        //// if main menu
        //for (i = 0; i < controls.Count; ++i)
        //{
        //    if (controls[i].HasFocus)
        //    {
        //        break;
        //    }
        //}

        //if (up)
        //{
        //    i = (i + 1) % controls.Count;
        //}
        //else
        //{
        //    i -= 1;
        //    if (i < 0)
        //    {
        //        i += controls.Count;
        //    }
        //}
        //controls[i].Focus();
    }

    public void OnKeyPressed()
    {
        bool up = true;
        //HandleUpDownList(m_mainMenuButtonList, up);
    }


    //void MainMenuLeaveButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    //{

    //}

    //void MainMenuLeagueButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    SwitchPanel(LeaguePanel);
    //}

    //void MainMenuShopButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    SwitchPanel(ShopPanel);
    //}

    //void MainMenuSchoolButtonClick(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    SwitchPanel(SchoolPanel);
    //}



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

    //public void SetCharacterData(CharacterData characterData)
    //{
    //    if (characterData != m_currentCharacterData)
    //    {
    //        m_currentCharacterData = characterData;
    //    }
    //}

    public TownData TownData
    {
        set
        {

            if (value != TownData1)
            {
                TownData1 = value;
                TownData1.BuildData();
            }
        }
        get
        {
            return TownData1;
        }
    }

    public GameObject TownPanel
    {
        get
        {
            return m_townPanel;
        }

        set
        {
            m_townPanel = value;
        }
    }

    public GameObject SchoolPanel
    {
        get
        {
            return m_schoolPanel;
        }

        set
        {
            m_schoolPanel = value;
        }
    }

    public GameObject ArenaMenuPanel
    {
        get
        {
            return m_arenaMenuPanel;
        }

        set
        {
            m_arenaMenuPanel = value;
        }
    }

    public GameObject LeaguePanel
    {
        get
        {
            return m_leaguePanel;
        }

        set
        {
            m_leaguePanel = value;
        }
    }

    public GameObject ShopPanel
    {
        get
        {
            return m_shopPanel;
        }

        set
        {
            m_shopPanel = value;
        }
    }

    public GameObject ShopItemPanel
    {
        get
        {
            return m_shopItemPanel;
        }

        set
        {
            m_shopItemPanel = value;
        }
    }

    public GameObject EncounterPanel
    {
        get
        {
            return m_encounterPanel;
        }

        set
        {
            m_encounterPanel = value;
        }
    }

    public GameObject TeamSelectionPanel
    {
        get
        {
            return m_teamSelectionPanel;
        }

        set
        {
            m_teamSelectionPanel = value;
        }
    }

    public GameObject ShopCharacterPanel
    {
        get
        {
            return m_shopCharacterPanel;
        }

        set
        {
            m_shopCharacterPanel = value;
        }
    }

    public LeagueData SelectedLeagueData
    {
        get
        {
            return m_selectedLeagueData;
        }

        set
        {
            m_selectedLeagueData = value;
        }
    }

    public TownData TownData1
    {
        get
        {
            return m_townData;
        }

        set
        {
            m_townData = value;
        }
    }

    string lastPanelName = "";
    private Vector3 lastPanelPosition = new Vector3();
    public void SwitchPanel(GameObject newPanel,bool hideExisting = true)
    {
        BaseGUIPanel newGuiPanel = newPanel.GetComponent<BaseGUIPanel>();
        if (newGuiPanel != null)
        {
            if (m_currentPanel != null)
            {
                Debug.Log("Moving panel : " + m_currentPanel.PanelName + " to " + newPanel.name);
                m_currentPanel.PanelDeactivated();
                //m_currentPanel.m_panel.RelativePosition = lastPanelPosition;
                //m_currentPanel.m_panel.IsVisible = false;
            }
            if (newGuiPanel != null)
            {
                newGuiPanel.PanelActivated();
                //newGuiPanel.m_panel.Focus();
                //lastPanelPosition = newGuiPanel.m_panel.RelativePosition;
                //newGuiPanel.m_panel.RelativePosition = new Vector3();
                //newGuiPanel.m_panel.BringToFront();
                //newGuiPanel.PanelActivated();
                ////newPanel.SetTownData(m_townData);
                //newGuiPanel.m_panel.IsVisible = true;
            }
            m_currentPanel = newGuiPanel;
        }
    }


    private TownData m_townData = null;




    public void ShowPopupPanel(GameObject caller,String displayText)
    {
        PopupPanel popupPanel = m_popupPanel.GetComponent<PopupPanel>();

        //if (!popupPanel.m_panel.IsVisible)
        //{
        //    dfGUIManager.PushModal(popupPanel.m_panel);
        //    //popupPanel.m_panel.GUIManager.
        //    popupPanel.m_parentGUIPanel = caller;
        //    popupPanel.SetPanelText(displayText);
        //    popupPanel.m_panel.RelativePosition = new Vector3();
        //    popupPanel.m_panel.IsVisible = true;
        //    popupPanel.m_panel.BringToFront();
        //    popupPanel.m_panel.Focus();

        //}
        //else
        //{
        //    // shouldn't really be here
        //    Debug.LogErrorFormat("Popup asked for visibility when already visible : [{0}][{1}]", caller.name, displayText);
        //}

    }


    public void HidePopupPanel()
    {
        //PopupPanel popupPanel = m_popupPanel.GetComponent<PopupPanel>();
        //if (popupPanel.m_panel.IsVisible)
        //{
        //    dfGUIManager.PopModal();
        //    popupPanel.m_panel.RelativePosition = new Vector3();
        //    popupPanel.m_panel.SendToBack();
        //    popupPanel.m_panel.IsVisible = false;

        //    BaseGUIPanel parentPanel = popupPanel.m_parentGUIPanel.GetComponent<BaseGUIPanel>();

        //    parentPanel.m_panel.Focus();
        //    parentPanel.m_panel.BringToFront();
        //}
        //else
        //{
        //    Debug.LogErrorFormat("Popup asked for hide when already hidden." );
        //}

    }


}


public class CharacterDataList
{
    private int m_currentSelection = -1;
    private List<CharacterData> m_characters;

    public delegate void CharacterChangedEventHandler(object sender, EventArgs e);

    public event CharacterChangedEventHandler CharacterChanged;

    public void SetData(List<CharacterData> characters)
    {
        m_characters = characters;
        if (m_characters.Count > 0)
        {
            m_currentSelection = 0;
            CurrentCharacter = m_characters[m_currentSelection];
        }
    }

    public int NumCharacters
    {
        get { return m_characters.Count; }
    }

    public CharacterData CharacterAtPosition(int position)
    {
        if (position < NumCharacters)
        {
            return m_characters[position];
        }
        return null;
    }

    public void CycleLeft()
    {
        m_currentSelection--;
        if (m_currentSelection < 0)
        {
            m_currentSelection += m_characters.Count;
        }
        CurrentCharacter = m_characters[m_currentSelection];
    }

    public void CycleRight()
    {
        m_currentSelection++;
        m_currentSelection = m_currentSelection % m_characters.Count;
        CurrentCharacter = m_characters[m_currentSelection];
    }

    public void OnCharacterChanged(EventArgs e)
    {
        if (CharacterChanged != null)
        {
            CharacterChanged(this, e);
        }
    }

    public CharacterData CurrentCharacter
    {
        get
        {
            if (m_currentSelection >= 0 && m_currentSelection <= m_characters.Count)
            {
                return m_characters[m_currentSelection];
            }
            return null;
        }
        set
        {
            if (m_characters.Contains(value))
            {
                int newSelection = m_characters.IndexOf(value);
                //if (newSelection != m_currentSelection)
                {
                    EventArgs ea = new EventArgs();
                    OnCharacterChanged(ea);
                }
                m_currentSelection = m_characters.IndexOf(value);
            }
        }
    }
}
