//using UnityEngine;
//using System.Collections;
//using EnhancedUI.EnhancedScroller;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using UnityEngine.EventSystems;
//using TMPro;

//public class ShopItemScript : EnhancedScrollerCellView,IPointerDownHandler,IPointerUpHandler
//{
//    public Highlightable Highlightable;
//    public ShopItem ShopItem;
//    public Image DamageSprite;
//    public TextMeshProUGUI Name;
//    public TextMeshProUGUI Cost;
//    private ItemPanelController Controller;
           

//    public void Awake()
//    {
        
//    }

//    public void SetData(ShopItem item,ItemPanelController controller)
//    {
//        SetSelected(false);
//        ShopItem = item;
//        Name.text = ShopItem.Name;
//        Cost.text = ""+ShopItem.Item.Cost;
//        DamageSprite.sprite = UIFactory.GetUISprite(UIFactory.SpriteIconNameForDamageType(item.DamageType));
//        Controller = controller;
//        Cost.color = item.Usable? (item.Affordable ? Color.green : Color.red) : Color.gray;
//        Name.color = item.Usable ? Color.white : Color.grey;
//    }




//    public void OnPointerDown(PointerEventData eventData)
//    {
//        if (Controller != null)
//        {
//            Controller.NewItemSelected(this);
//        }
//    }

//    public void OnPointerUp(PointerEventData eventData)
//    {
//    }

//    public void SetSelected(bool val)
//    {
//        if (val)
//        {
//            Highlightable.HighlightState = HighlightState.Selected;
//        }
//        else
//        {
//            Highlightable.HighlightState = HighlightState.Inactive;
//        }
//    }

//}
