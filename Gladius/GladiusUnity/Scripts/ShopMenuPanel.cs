using UnityEngine;
using UnityEngine.UI;

public class ShopMenuPanel : BaseGUIPanel
{

    public GameObject ShopNameLabel;
    public GameObject OwnerThumbnail;

    public GameObject ChatButton;
    public GameObject ItemsButton;
    public GameObject TownImage;
    public GameObject ShopKeeperDialogue;

    public override void ChildStart()
    {
        base.ChildStart();
        //dfControl panel = m_panel.Find<dfControl>("ShopMenuPanel");
        //m_controlsList.Add(GameObject.Find("ItemButton").GetComponent<dfButton>());
        //m_controlsList.Add(GameObject.Find("ChatButton").GetComponent<dfButton>());

        //GladiusGlobals.LayoutControlsInContainer(m_controlsList, panel, 1, m_controlsList.Count);

        //foreach (dfControl dfc in m_controlsList)
        //{
        //    dfButton button = dfc as dfButton;
        //    if (button != null)
        //    {
        //        button.Click += ShopMenuPanel_Click;
        //    }
        //}
        //m_townGuiController.ShopCharacterPanel = new ShopCharacterPanel();
        //m_townGuiController.ShopCharacterPanel.SetupPanelData("", this);

    }


    //void ShopMenuPanel_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    if (control.gameObject.name == "ItemButton")
    //    {
    //        m_townGuiController.SwitchPanel(m_townGuiController.ShopCharacterPanel);
    //    }
    //    else if (control.gameObject.name == "ChatButton")
    //    {
    //        m_townGuiController.ShowPopupPanel(gameObject,"Welcome to the shop!");
    //    }

    //}

    public override void PanelActivated()
    {
        base.PanelActivated();

        ShopNameLabel.GetComponent<Text>().text = TownData.Shop.Name;
        ShopKeeperDialogue.GetComponent<Text>().text = GladiusGlobals.GameStateManager.LocalisationData[TownData.Shop.Opening];
        TownImage.GetComponent<Image>().sprite = Resources.Load<UnityEngine.Sprite>(TownData.Shop.Image);

        string path = GladiusGlobals.UIElements + TownData.ArenaData.OwnerThumbnailName;
        Sprite s = Resources.Load<Sprite>(path);
        OwnerThumbnail.GetComponent<Image>().sprite = s;

    }

}
