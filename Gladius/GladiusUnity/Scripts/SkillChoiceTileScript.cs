using UnityEngine;
using System.Collections;
using System;

public class SkillChoiceTileScript : MonoBehaviour
{
    private int m_index;
    //public int VirtualScrollItemIndex
    //{
    //    get
    //    {
    //        return m_index;
    //    }

    //    set
    //    {
    //        m_index = value;
    //    }
    //}

    //public AttackSkill AttackSkill
    //{
    //    get; set;
    //}

    public PlayerChoiceBar PlayerChoiceBar
    { get; set; }

    //// Use this for initialization
    //void Awake()
    //{
    //    m_panel = GetComponent<dfPanel>();
    //}

    //public dfPanel GetDfPanel()
    //{
    //    return m_panel;
    //}

    //public void OnScrollPanelItemVirtualize(object backingListItem)
    //{
    //    AttackSkill = backingListItem as AttackSkill;
    //    Refresh();
    //}

    //public void Refresh()
    //{
    //    if (AttackSkill != null)
    //    {
    //        String localisationText = GladiusGlobals.GameStateManager.LocalisationData[AttackSkill.DisplayNameId];
    //        GetComponentInChildren<dfButton>().Text = localisationText;
    //    }
    //}

    //    // Use this for initialization
    //    void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //private dfPanel m_panel;


}
