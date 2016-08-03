using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillButtonEventHandler : MonoBehaviour, IPointerClickHandler
{
    public PlayerChoiceBar PlayerChoiceBar
    { get; set; }

    public List<AttackSkill> SkillList
    { get; set; }

    public Button Button
    { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerChoiceBar.ShowHideSkillPanel(this);
    }

    RectTransform m_playerSkillsPanelRT;
}
