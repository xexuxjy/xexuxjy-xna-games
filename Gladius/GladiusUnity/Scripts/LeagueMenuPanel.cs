using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeagueMenuPanel : BaseGUIPanel
{
    public GameObject SlotPrefab;
    public GameObject LeagueNameLabel;
    public GameObject OwnerThumbnail;

    private List<Button> m_leaguePanels = new List<Button>();

    public override void ChildStart()
    {
        base.ChildStart();

        SlotPrefab = Resources.Load("Prefabs/LeagueSlotPrefab") as GameObject;
        //m_townGuiController.EncounterPanel = new EncounterPanel();
        //m_townGuiController.EncounterPanel.SetupPanelData("", this);
        //Transform leagueDisplayPanel = m_panel.Find("LeagueDisplay");
        //int width = (int)(leagueDisplayPanel.Width / SlotsH);
        //int height = (int)(leagueDisplayPanel.Height / SlotsV);

        //int numSlots = 18;
        //for (int i = 0; i < numSlots; ++i)
        //{
        //    GameObject copy = Instantiate(SlotPrefab);
        //    Button b = copy.GetComponent<Button>();
        //    copy.transform.SetParent(leagueDisplayPanel);
        //    //fButton panel = leagueDisplayPanel.AddPrefab(SlotPrefab) as dfButton;
        //}
        //            dfButton panel = leagueDisplayPanel.AddPrefab(SlotPrefab) as dfButton;
        //            m_leaguePanels.Add(panel);
        //            panel.Width = width;
        //            panel.Height = height;
        //            //panel.Position = new Vector3();
        //            panel.RelativePosition = new Vector3(j * width, i * height);
        //            panel.Click += panel_Click;


        //int counter = 0;
        //string background1 = "king of the hill ke.tga";
        //string background2 = "mystical zo.tga";

        //for (int i = 0; i < SlotsV; ++i)
        //    {
        //        for (int j = 0; j < SlotsH; ++j)
        //        {
        //            int index = (i * SlotsH) + j;

        //            dfButton panel = leagueDisplayPanel.AddPrefab(SlotPrefab) as dfButton;
        //            m_leaguePanels.Add(panel);
        //            panel.Width = width;
        //            panel.Height = height;
        //            //panel.Position = new Vector3();
        //            panel.RelativePosition = new Vector3(j * width, i * height);
        //            panel.Click += panel_Click;
        //        }
        //    }
    }

    public override void PanelActivated()
    {
        //Transform headerPanel = m_panel.Find("LeagueHeaderPanel");
        //m_panel.Find<dfLabel>("Label").Text = "League Data";
        LeagueNameLabel.GetComponent<Text>().text = TownData.ArenaData.ArenaName;
        OwnerThumbnail.GetComponent<Image>().sprite = Resources.Load<UnityEngine.Sprite>(TownData.ArenaData.OwnerThumbnailName);
        //Transform leagueDisplayPanel = m_panel.Find("LeagueDisplay");
        //int width = (int)(leagueDisplayPanel.Width / SlotsH);
        //int height = (int)(leagueDisplayPanel.Height / SlotsV);

        int counter = 0;
        //string background1 = "king of the hill ke.tga";
        //string background2 = "mystical zo.tga";
        //for (int i = 0; i < SlotsV; ++i)
        //{
        //    for (int j = 0; j < SlotsH; ++j)
        //    {
        //        int index = (i * SlotsH) + j;
        //        dfButton panel = m_leaguePanels[index];

        //        if (index < TownData.ArenaData.Leagues.Count)
        //        {
        //            panel.enabled = true;
        //            LeagueData leagueData = TownData.ArenaData.Leagues[index];
        //            leagueData.ImageName = leagueData.Name + ".tga";//(index % 2 == 0) ? background1 : background2;
        //            panel.Width = width;
        //            panel.Height = height;
        //            panel.Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
        //            panel.Find<dfRichTextLabel>("LeagueStatus").Text = leagueData.Name;
        //            panel.Tag = leagueData;
        //        }
        //        else
        //        {
        //            panel.IsVisible = false;
        //        }
        //    }
        //}
        m_townGuiController.SelectedLeagueData = null;
    }

    //void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    //LeagueData leagueData = control.Tag as LeagueData;
    //    //if (m_townGuiController.SelectedLeagueData == leagueData) // we've selected the same one again
    //    //{
    //    //    m_townGuiController.SwitchPanel(m_townGuiController.EncounterPanel);
    //    //}
    //    //else
    //    //{
    //    //    m_panel.Find("LeagueDataPanel").Find<dfTextureSprite>("LeagueImage").Texture = Resources.Load<Texture2D>(GladiusGlobals.UIRoot + "TownOverland/LeagueImages/" + leagueData.ImageName);
    //    //    m_panel.Find("LeagueDataPanel").Find<dfRichTextLabel>("LeagueName").Text = GladiusGlobals.GameStateManager.LocalisationData[leagueData.DescriptionId];
    //    //}
    //    //m_townGuiController.SelectedLeagueData = leagueData;
    //}

}
