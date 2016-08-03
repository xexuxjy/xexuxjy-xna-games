public class ShopCharacterPanel : BaseGUIPanel
{
    SelectionGrid m_schoolGrid;

    public override void ChildStart()
    {
        base.ChildStart();
        //m_townGuiController.ShopItemPanel = new ShopItemMenuPanel();
        //    m_townGuiController.ShopItemPanel.SetupPanelData("", this);

        m_schoolGrid = gameObject.AddComponent<SelectionGrid>();
        //m_schoolGrid.Init(m_panel.Find("AvailablePanel"), "Available", SchoolGrid_Click);

        //if (control.gameObject.name == "ItemButton")
        //{
        //    m_townGuiController.SwitchPanel(m_townGuiController.m_shopItemPanel);
        //}

    }


    public override void PanelActivated()
    {
        base.PanelActivated();
        // fill in available panel with school gladiators.
        int count = 0;
        //m_availableGrid.SetStartDefault(0, 0);
        foreach (CharacterData characterData in m_townGuiController.GladiatorSchool.Gladiators)
        {
            if (count < m_schoolGrid.MaxSize)
            {
                m_schoolGrid.SetSlot(count++, characterData);
            }
        }

    }

    //void SchoolGrid_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    // some work here to turn character panel into a prefab?
    //    // need to display image of character when selected...?
    //    SlotData slotData = control.Tag as SlotData;
    //    CharacterData characterData = slotData.Current;
    //    if (characterData != null)
    //    {
    //        m_townGuiController.CharacterList.CurrentCharacter = characterData;
    //        m_townGuiController.SwitchPanel(m_townGuiController.ShopItemPanel);
    //    }
    //}

}
