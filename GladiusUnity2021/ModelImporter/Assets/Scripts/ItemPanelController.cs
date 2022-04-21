//using EnhancedUI.EnhancedScroller;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ItemPanelController : MonoBehaviour, IEnhancedScrollerDelegate
//{
//    public Button WeaponButton;
//    public Button ShieldButton;
//    public Button ArmourButton;
//    public Button HelmButtton;
//    public Button AccessoryButton;

//    public ShopItem CurrentItem
//    {
//        get
//        {
//            return m_current != null ? m_current.ShopItem : null;
//        }
//    }

//    public CharacterListController CharacterListController;


//    public EnhancedScroller scroller;
//    public EnhancedScrollerCellView localCellViewPrefab;


//    // click once
//    public event ItemSelectedEventHandler ItemSelected;
//    // click twice
//    public event ItemSelectedEventHandler ItemChosen;

//    public event ItemSelectedEventHandler FilterChanged;


//    private List<ShopItem> m_items;
//    private List<ShopItem> m_filteredItems = new List<ShopItem>();
//    private ShopItemScript m_current;

//    public delegate void ItemSelectedEventHandler(object sender, EventArgs e);



//    public void Awake()
//    {
//        WeaponButton.onClick.AddListener(() =>  FilterList(ItemLocation.Weapon));
//        ShieldButton.onClick.AddListener(() => FilterList(ItemLocation.Shield));
//        ArmourButton.onClick.AddListener(() => FilterList(ItemLocation.Armor));
//        HelmButtton.onClick.AddListener(() => FilterList(ItemLocation.Helmet));
//        AccessoryButton.onClick.AddListener(() => FilterList(ItemLocation.Accessory));

//        scroller.Delegate = this;
//    }


//    public void FilterList(ItemLocation itemLocation)
//    {
//        NewItemSelected(null);
//        m_filteredItems.Clear();
//        //m_filteredItems.AddRange(m_items.FindAll(x => x.Item.ItemLocation == itemLocation));

//        CharacterData currrentCharacter = CharacterListController.CurrentCharacter;

//        foreach (ShopItem shopItem in m_items)
//        {
//            bool valid = false;
//            bool available = false;
//            if (shopItem.Item != null )
//            {
//                if (itemLocation == ItemLocation.None || shopItem.Item.ItemLocation == itemLocation)
//                {
//                    valid = true;
//                    if (currrentCharacter == null)
//                    {
//                        shopItem.Affordable = true;
//                        shopItem.Available = true;
//                    }
//                    else
//                    {
//                        bool classValid = ActorGenerator.ItemValidForClass(shopItem.Item, currrentCharacter.CurrentClassDef);
//                        shopItem.Affordable = true;

//                        shopItem.Usable = shopItem.Item.MinLevel < currrentCharacter.Level && classValid;
//                    }
//                }
//            }
//            if (valid)
//            {
//                shopItem.Available = available;
//                m_filteredItems.Add(shopItem);
//            }

//        }

//        scroller.ReloadData();
//        if(m_filteredItems.Count > 0)
//        {
//            NewItemSelected(scroller.GetCellViewAtDataIndex(0).GetComponent<ShopItemScript>());
//        }
//        if(FilterChanged != null)
//        {
//            FilterChanged(this, new ShopItemEventArgs(m_current != null ? m_current.ShopItem:null));
//        }

//    }


//    public void NewItemSelected(ShopItemScript item)
//    {
//        if (m_current != null && m_current != item)
//        {
//            m_current.SetSelected(false);
//        }
//        m_current = item;
//        if (m_current != null)
//        {
//            m_current.SetSelected(true);

//            if (ItemSelected != null)
//            {
//                ItemSelected(this, new ShopItemEventArgs(m_current.ShopItem));
//            }
//        }
//    }


//    public void SetItemList(List<ShopItem> itemList)
//    {
//        m_items = itemList;
//        m_current = null;
//        FilterList(ItemLocation.None);
//    }

//    public int GetNumberOfCells(EnhancedScroller scroller)
//    {
//        return m_filteredItems.Count;
//    }

//    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
//    {
//        return 50.0f;
//    }

//    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
//    {
//        ShopItemScript cellView = scroller.GetCellView(localCellViewPrefab) as ShopItemScript;

//        // set the name of the game object to the cell's data index.
//        // this is optional, but it helps up debug the objects in 
//        // the scene hierarchy.
//        cellView.name = "Cell " + dataIndex.ToString();

//        // in this example, we just pass the data to our cell's view which will update its UI
//        cellView.SetData(m_filteredItems[dataIndex], this);


//        return cellView;
//    }
//}


//public class ShopItemEventArgs : EventArgs
//{
//    public ShopItem ShopItem;
//    public ShopItemEventArgs(ShopItem si)
//    {
//        ShopItem = si;
//    }
//}
