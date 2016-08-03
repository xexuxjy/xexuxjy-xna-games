using UnityEngine;
using UnityEngine.UI;

public class SchoolMenuPanel : BaseGUIPanel
{
    private GladiatorSchool m_school;
    public GameObject SelectionGrid;
    public GameObject CharacterPanel;
    public GameObject SchoolNameLabel;

    private CharacterDataList m_characterDataList = new CharacterDataList();

    public void Start()
    {
    }

    public GladiatorSchool School
    {
        get
        {
            return m_school;
        }
        set
        {
            m_school = value;
            m_characterDataList.SetData(m_school.Gladiators);
            SelectionGrid.GetComponent<SelectionGrid>().SetStartDefault(m_characterDataList);
            CharacterPanel.GetComponent<CharacterPanel>().CharacterDataList = m_characterDataList;
            SchoolNameLabel.GetComponent<Text>().text = m_school.Name;
        }

    }


    public override void PanelActivated()
    {
        base.PanelActivated();
        if (GladiatorSchool != null)
        {
            SelectionGrid.GetComponent<SelectionGrid>().SetStartDefault(m_characterDataList);
        }
    }
}
