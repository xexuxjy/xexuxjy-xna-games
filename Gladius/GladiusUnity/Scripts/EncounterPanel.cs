using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncounterPanel : BaseGUIPanel
{
    private List<GameObject> m_encounterPanels = new List<GameObject>();
    public GameObject SlotPrefab;
    public int NumEncounters = 8;

    public override void ChildStart()
    {
        base.ChildStart();
        SlotPrefab = Resources.Load("Prefabs/EncounterListPrefab") as GameObject;
        //m_townGuiController.TeamSelectionPanel = new TeamSelectionPanel();
        //m_townGuiController.TeamSelectionPanel.SetupPanelData("", this);

        //Transform leagueDisplayPanel = GladiusGlobals.FuzzyFindChild("EncounterDisplay", m_panel) ;
        //string background1 = "king of the hill ke.tga";
        //string background2 = "mystical zo.tga";
        // let component layout handle this.
        //RectTransform
        for (int i = 0; i < NumEncounters; ++i)
        {
            //int index = i;
            //GameObject go = Instantiate(SlotPrefab);
            //go.transform.SetParent(leagueDisplayPanel);
            //m_encounterPanels.Add(go);
            //panel.Width = width;
            //panel.Height = height;
            ////panel.Position = new Vector3();
            //panel.RelativePosition = new Vector3(0, i * height);
            //panel.Click += panel_Click;
        }

    }

    //void proceedButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    // do some checks here to make sure all slots are filled and that we have a team to fight with...
    //    List<CharacterData> newParty = new List<CharacterData>();
    //    if (GladiusGlobals.GladiatorSchool != null)
    //    {
    //        GladiusGlobals.GladiatorSchool.SetCurrentParty(newParty);
    //    }
    //}

    public override void PanelActivated()
    {
        //dfPanel headerPanel = m_panel.Find<dfPanel>("EncounterHeaderPanel");
        //Transform headerPanel = GladiusGlobals.FuzzyFindChild("EncounterHeaderPanel", m_panel);
        ////m_panel.Find<dfLabel>("Label").Text = "League Data";
        //headerPanel.Find("ArenaName").GetComponent<Text>().text = TownData.ArenaData.ArenaName;
        //headerPanel.Find("OwnerThumbnail").GetComponent<Image>().sprite = Resources.Load<UnityEngine.Sprite>(TownData.ArenaData.OwnerThumbnailName);

        //// use the data in selected league to populate the encounter
        //m_panel.Find("EncounterImage").GetComponent<Image>().sprite = Resources.Load<UnityEngine.Sprite>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + m_townGuiController.SelectedLeagueData.ImageName);
        //m_panel.Find("EncounterName").GetComponent<Text>().text = "";

        for (int i = 0; i < NumEncounters; ++i)
        {
            GameObject panel = m_encounterPanels[i];
            if (i < m_townGuiController.SelectedLeagueData.ArenaEncounters.Count)
            {
                //panel.IsVisible = true;
                ArenaEncounter encounter = m_townGuiController.SelectedLeagueData.ArenaEncounters[i];
                //panel.Find("Name").GetComponent<Text>().text = GladiusGlobals.GameStateManager.LocalisationData[encounter.Id];
                //panel.Find<dfTextureSprite>("Points").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
                //panel.Tag = encounter;
            }
            else
            {
            // panel.IsVisible = false;
            }

        }
        m_townGuiController.ArenaEncounter = null;

    }

    //void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    ArenaEncounter encounter = control.Tag as ArenaEncounter;
    //    //m_panel.Find("EncounterDataPanel").Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
    //    String info = GladiusGlobals.GameStateManager.LocalisationData[encounter.EncounterDescId];
    //    m_panel.Find("EncounterDataPanel/EncounterName").GetComponent<Text>().text = info;
    //    if (encounter == m_townGuiController.ArenaEncounter)
    //    {
    //        m_townGuiController.SwitchPanel(m_townGuiController.TeamSelectionPanel);
    //    }

    //    m_townGuiController.ArenaEncounter = encounter;

    //}
}
