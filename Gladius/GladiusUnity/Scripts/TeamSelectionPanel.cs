using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionPanel : BaseGUIPanel
{
    SelectionGrid m_availableGrid;
    SelectionGrid m_selectedGrid;
    Transform m_requiredPanel;
    Transform m_prohibitedPanel;
    Transform m_rhsPanel;
    //dfPanel m_characterPanel;

    CharacterDataList AvailableCharacterList = new CharacterDataList();
    CharacterDataList SelectedCharacterList = new CharacterDataList();

    public override void ChildStart()
    {
        base.ChildStart();
        //m_availableGrid = m_panel.gameObject.AddComponent<SelectionGrid>();
        //m_selectedGrid = m_panel.gameObject.AddComponent<SelectionGrid>();
        ////m_availableGrid.Init(m_panel.Find("AvailablePanel"), "Available", AvailableGrid_Click);
        ////m_selectedGrid.Init(m_panel.Find("SelectedPanel"), "Selected", SelectedGrid_Click);
        //m_rhsPanel = m_panel.Find("RHSPanel");
        //m_requiredPanel = m_rhsPanel.Find("RequiredPanel");
        //m_prohibitedPanel = m_rhsPanel.Find("ProhibitedPanel");

        //dfControl panel = m_panel.Find<dfControl>("ButtonPanel");

        //dfButton proceedButton = m_panel.Find<dfButton>("ProceedButton");
        //proceedButton.Click += proceedButton_Click;
        //m_controlsList.Add(proceedButton);
        //m_controlsList.Add(m_leaveButton);

        //LayoutControlsInContainer(m_controlsList, panel, m_controlsList.Count, 1);


    }



    public override void PanelActivated()
    {
        base.PanelActivated();
        if (m_townGuiController.ArenaEncounter != null)
        {
            m_townGuiController.ArenaEncounter.LoadEncounterData();

            // fill in available panel with school gladiators.
            int count = 0;
            //m_availableGrid.SetStartDefault(0, 0);
            foreach (CharacterData characterData in m_townGuiController.GladiatorSchool.Gladiators)
            {
                if (count < m_availableGrid.MaxSize)
                {
                    m_availableGrid.SetSlot(count++, characterData);
                }
            }

            EncounterSide heroSide = m_townGuiController.ArenaEncounter.Encounter.Sides[0];
            // side 0 is player/ heros team?
            SelectedCharacterList.SetData(heroSide.CharacterDataList);
            m_selectedGrid.SetStartDefault(SelectedCharacterList);

            //m_selectedGrid.SetStartDefault(2, 4);

            // setup required and probhibited text.
            m_requiredPanel.Find("RTL").GetComponent<Text>().text = BuildStringForReqPro(heroSide.CharacterDataList[0], true);
            m_prohibitedPanel.Find("RTL").GetComponent<Text>().text = BuildStringForReqPro(heroSide.CharacterDataList[0], false);
        }
    }

    public String BuildStringForReqPro(CharacterData characterData, bool required)
    {



        return required ? "Required" : "Prohibited";
    }


    //void AvailableGrid_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    // some work here to turn character panel into a prefab?
    //    // need to display image of character when selected...?
    //    SlotData slotData = control.Tag as SlotData;
    //    CharacterData characterData = slotData.Current;
    //    if (characterData != null)
    //    {
    //        m_townGuiController.CharacterList.CurrentCharacter = characterData;
    //        ShowCharacterPanel();
    //        // move control
    //        AvailableToSelected(control);
    //    }


    //}

    //void SelectedGrid_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    SelectedToAvailable(control);
    //}

    void ShowCharacterPanel()
    {
        //m_requiredPanel.IsVisible = false;
        //m_prohibitedPanel.IsVisible = false;
        //m_townGuiController.m_characterPanel.m_panel.IsVisible = true;
        //m_townGuiController.m_characterPanel.m_panel.transform.parent = m_rhsPanel.transform;
        //m_townGuiController.m_characterPanel.m_panel.Size = m_rhsPanel.Size;
        //m_townGuiController.m_characterPanel.UpdatePanel();
        ////m_townGuiController.m_characterPanel.Yield;
        ////m_townGuiController.m_characterPanel.m_panel.transform.localPosition = new Vector3(0f, 0f, 0f);
        //m_townGuiController.m_characterPanel.m_panel.Position = new Vector3();// transform.localPosition = new Vector3(0f, 0f, 0f);
        // size to fit.?
    }

    void HideCharacterPanel()
    {
        //m_requiredPanel.IsVisible = true;
        //m_prohibitedPanel.IsVisible = true;
        //m_townGuiController.m_characterPanel.m_panel.IsVisible = false;
        //m_townGuiController.m_characterPanel.m_panel.transform.parent = m_rhsPanel.transform;
        //m_townGuiController.m_characterPanel.m_panel.Size = m_rhsPanel.Size;
        // size to fit.?
    }

    //void AvailableToSelected(dfControl control)
    //{
    //    //if (m_selectedGrid.EmptySlots > 0)
    //    //{
    //    //    SlotData slotData = control.Tag as SlotData;
    //    //    CharacterData characterData = slotData.Current;

    //    //    if (characterData != null)
    //    //    {
    //    //        int nextSlot = m_selectedGrid.NextAvailableSlot();
    //    //        if (nextSlot >= 0)
    //    //        {
    //    //            m_selectedGrid.SetSlot(nextSlot, characterData);
    //    //            m_availableGrid.SetSlot(control as dfButton, null);
    //    //        }
    //    //    }
    //    //}
    //}

    //void SelectedToAvailable(dfControl control)
    //{
    //    SlotData slotData = control.Tag as SlotData;
    //    CharacterData characterData = slotData.Current;

    //    if (characterData != null)
    //    {
    //        int nextSlot = m_availableGrid.NextAvailableSlot();
    //        if (nextSlot >= 0)
    //        {
    //            //m_availableGrid.SetSlot(nextSlot, characterData);
    //            //m_selectedGrid.SetSlot(control as dfButton, null);
    //        }
    //    }
    //}

    //void proceedButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    // do some checks here to make sure all slots are filled and that we have a team to fight with...
    //    List<CharacterData> newParty = new List<CharacterData>();
    //    m_selectedGrid.FillParty(newParty);
    //    GladiusGlobals.GladiatorSchool.SetCurrentParty(newParty);
    //    GladiusGlobals.GameStateManager.SetNewState(GameState.Arena);
    //}

}