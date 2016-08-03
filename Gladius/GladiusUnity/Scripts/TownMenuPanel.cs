using System;
using UnityEngine;
using UnityEngine.UI;

public class TownMenuPanel : BaseGUIPanel
{
    public GameObject TownMenuPanelControl;
    public GameObject ShopButton;
    public GameObject ArenaButton;
    public GameObject SchoolButton;

    public GameObject TownTitleLabel;
    public GameObject ShopNameLabel;
    public GameObject RegionImage;
    public GameObject TownImage;


    public override void ChildStart()
    {
        base.ChildStart();
        Button shopButton = ShopButton.GetComponent<Button>();
        shopButton.onClick.AddListener(() => { TownMenuPanel_Click(shopButton); });
        Button arenaButton = ArenaButton.GetComponent<Button>();
        arenaButton.onClick.AddListener(() => { TownMenuPanel_Click(arenaButton); });
        Button schoolButton = SchoolButton.GetComponent<Button>();
        schoolButton.onClick.AddListener(() => { TownMenuPanel_Click(schoolButton); });

    }


    void TownMenuPanel_Click(Button control)
    {
        Debug.Log("Clicked : " + control.gameObject.name);
        if (control.gameObject == ShopButton)
        {
            m_townGuiController.SwitchPanel(m_townGuiController.ShopPanel);
        }
        else if (control.gameObject == SchoolButton)
        {
            m_townGuiController.SwitchPanel(m_townGuiController.SchoolPanel);
        }
        else if (control.gameObject == ArenaButton)
        {
            m_townGuiController.SwitchPanel(m_townGuiController.ArenaMenuPanel);
        }
    }

    public override void ActionPressed(object sender, ActionButtonPressedArgs e)
    {
    }

    public override void PanelActivated()
    {
        base.PanelActivated();
        TownTitleLabel.GetComponent<Text>().text = TownData.Name;
        ShopButton.transform.Find("Text").GetComponent<Text>().text = TownData.Shop.Name;
        ArenaButton.transform.Find("Text").GetComponent<Text>().text = TownData.ArenaData.ArenaName;
        SchoolButton.transform.Find("Text").GetComponent<Text>().text = GladiatorSchool.Name;
        //SchoolButton.transform.Find("Text").GetComponent<Text>().text = TownData.ArenaData.ArenaName;
        LeaveButton.transform.Find("Text").GetComponent<Text>().text = "Leave Town";
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
        RegionImage.GetComponent<Image>().overrideSprite = Resources.Load<UnityEngine.Sprite>(regionTextureName); 
        TownImage.GetComponent<Image>().overrideSprite = Resources.Load<UnityEngine.Sprite>(TownData.BackgroundTextureName);
    }

}
