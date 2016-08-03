using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ArenaMenuPanel : BaseGUIPanel
{
    public GameObject ChampionshipButton;
    public GameObject TournamentButton;
    public GameObject LeaguesButton;
    public GameObject RecruitingButton;
    public GameObject HistoryButton;

    public GameObject LeagueNameLabel;
    public GameObject LeagueImage;
    public GameObject OwnerThumbnail;

    public override void ChildStart()
    {
        base.ChildStart();
        ChampionshipButton.GetComponent<Button>().onClick.AddListener(() => { ArenaMenuPanel_Click(ChampionshipButton.GetComponent<Button>()); });
        TournamentButton.GetComponent<Button>().onClick.AddListener(() => { ArenaMenuPanel_Click(TournamentButton.GetComponent<Button>()); });
        LeaguesButton.GetComponent<Button>().onClick.AddListener(() => { ArenaMenuPanel_Click(LeaguesButton.GetComponent<Button>()); });
        RecruitingButton.GetComponent<Button>().onClick.AddListener(() => { ArenaMenuPanel_Click(RecruitingButton.GetComponent<Button>()); });
        HistoryButton.GetComponent<Button>().onClick.AddListener(() => { ArenaMenuPanel_Click(HistoryButton.GetComponent<Button>()); });
    }

    void ArenaMenuPanel_Click(Button control)
    {
        
        if (control.gameObject== ChampionshipButton)
        {
            m_townGuiController.SwitchPanel(m_townGuiController.LeaguePanel);
        }
        else if (control.gameObject == TournamentButton)
        {
            m_townGuiController.SwitchPanel(m_townGuiController.LeaguePanel);
        }
        else if (control.gameObject == LeaguesButton)
        {
            m_townGuiController.SwitchPanel(m_townGuiController.LeaguePanel);
        }
        else if (control.gameObject == HistoryButton)
        {
            string historyText = "Empty";
            GladiusGlobals.GameStateManager.LocalisationData.TryGetValue(m_townGuiController.TownData.ArenaData.HistoryText, out historyText);
            m_townGuiController.ShowPopupPanel(gameObject, historyText);
        }

    }

    public override void PanelActivated()
    {
        base.PanelActivated();
        LeagueNameLabel.GetComponent<Text>().text = TownData.ArenaData.ArenaName;
        //LeagueImage.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<UnityEngine.Sprite>("Resources/"+TownData.ArenaData.BackgroundTextureName+".png");
        //OwnerThumbnail.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<UnityEngine.Sprite>(TownData.ArenaData.OwnerThumbnailName + ".png");
        
        LeagueImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(TownData.ArenaData.BackgroundTextureName);
        string path = GladiusGlobals.UIElements + TownData.ArenaData.OwnerThumbnailName;
        Sprite s = Resources.Load<Sprite>(path);
        OwnerThumbnail.GetComponent<Image>().sprite = s;
    }
}
