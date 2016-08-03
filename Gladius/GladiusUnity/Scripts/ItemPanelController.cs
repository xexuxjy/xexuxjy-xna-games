using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPanelController : MonoBehaviour
{

    //public GameObject itemSlotPrefab;
    ////dfControl lastControl;
    //private ShopItem lastShopItem;


    ////public dfRichTextLabel detailsPanelLabel;
    ////public dfControl imagePanel;

    //CharacterData characterData;
    //public CharacterData CurrentCharacter
    //{
    //    get { return characterData; }
    //    set
    //    {
    //        characterData = value;
    //        UpdateFilter();
    //    }
    //}

    ////GameObject modelImageSprite;

    //int previousSelection;
    //int currentSelection;

    //List<string> shopItemNames;
    //List<ShopItem> shopItems;
    //List<ShopItem> filteredShopItem = new List<ShopItem>();

    //float scrollIncrement;

    //public ItemLocation itemLocation = ItemLocation.Weapon;

    //public ItemLocation ActiveItemLocation
    //{
    //    get
    //    {
    //        return itemLocation;
    //    }

    //    set
    //    {
    //        itemLocation = value;
    //        UpdateFilter();
    //    }

    //}

    //public void Start()
    //{
    //    //dfScrollPanel container = GameObject.Find("ItemPanel").GetComponent<dfScrollPanel>();
    //    // this should get the child clicks
    //    //container.Click += Container_Click;
    //}

    ////private void Container_Click(dfControl control, dfMouseEventArgs mouseEvent)
    ////{
    ////    if (mouseEvent.Source != null)
    ////    {
    ////        GameObject go = FindParentWithName(mouseEvent.Source.gameObject, "ItemSlotPanel");
    ////        if (go != null)
    ////        {
    ////            ShopItemGUI shopItemGUI = go.GetComponent<ShopItemGUI>();
    ////            if (shopItemGUI != null)
    ////            {
    ////                ShopItem currentShopItem = shopItemGUI.ShopItem;
    ////                ItemSelectionChanged(lastShopItem, currentShopItem);
    ////            }
    ////        }
    ////        lastControl = mouseEvent.Source;

    ////    }
    ////}

    //public void UpdateFilter()
    //{
    //    //    dfScrollPanel container = GameObject.Find("ItemPanel").GetComponent<dfScrollPanel>();
    //    //    container.FlowDirection = dfScrollPanel.LayoutDirection.Vertical;
    //    //    dfPanel itemPanel = itemSlotPrefab.GetComponent<dfPanel>();

    //    //    if (container != null)
    //    //    {
    //    //        filteredShopItem.Clear();
    //    //        //filteredShopItem = new List<ShopItem>();
    //    //        foreach (ShopItem shopItem in shopItems)
    //    //        {
    //    //            if (shopItem.Item != null && shopItem.Item.Location == ActiveItemLocation)
    //    //            {
    //    //                if (CurrentCharacter == null || ActorGenerator.ItemValidForClass(shopItem.Item, CurrentCharacter.ActorClassData))
    //    //                {
    //    //                    filteredShopItem.Add(shopItem);
    //    //                }
    //    //            }
    //    //        }

    //    //        container.Virtualize<ShopItem>(filteredShopItem, itemPanel);

    //    //        currentSelection = 0;

    //    //        //scrollIncrement = container.VertScrollbar.MaxValue / shopItems.Count;
    //    //        if (filteredShopItem.Count > 0)
    //    //        {
    //    //            ItemSelectionChanged(null, filteredShopItem[0]);
    //    //        }
    //    //    }
    //}



    //public void ControlPress(ActionButton ab)
    //{
    //    //dfScrollPanel sp = GameObject.Find("ItemPanel").GetComponent<dfScrollPanel>();
    //    //Vector2 scrollPos = sp.ScrollPosition;

    //    //scrollPos.y += scrollIncrement;
    //    //sp.ScrollPosition = scrollPos;

    //    //if (ab == ActionButton.Move1Up)
    //    //{
    //    //    scrollPos.y -= scrollIncrement;
    //    //    sp.ScrollPosition = scrollPos;
    //    //}
    //    //else if (ab == ActionButton.Move1Down)
    //    //{
    //    //    scrollPos.y += scrollIncrement;
    //    //    sp.ScrollPosition = scrollPos;
    //    //}

    //    //previousSelection = currentSelection;

    //    ////currentSelection = (int)(shopItems.Count * (sp.VertScrollbar.Value / sp.VertScrollbar.MaxValue));

    //    //ItemSelectionChanged(shopItems[previousSelection], shopItems[currentSelection]);

    //}



    //// Use this for initialization
    //void Awake()
    //{
    //    //GameObject panel = GameObject.Find("ShopItemDescriptionLabel");
    //    //detailsPanelLabel = panel.GetComponent<dfRichTextLabel>();

    //    //modelImageSprite = GameObject.Find("ItemSprite");

    //}


    //public void SetData(Shop currentShop)
    //{
    //    if (currentShop != null)
    //    {
    //        shopItemNames = currentShop.GetItemList();
    //        shopItems = new List<ShopItem>();

    //        //dfScrollPanel container = GameObject.Find("ItemPanel").GetComponent<dfScrollPanel>();

    //        //dfPanel itemPanel = itemSlotPrefab.GetComponent<dfPanel>();

    //        //if (container != null)
    //        //{
    //        //    //container.
    //        //    foreach (string itemName in shopItemNames)
    //        //    {
    //        //        shopItems.Add(new ShopItem(itemName));
    //        //    }

    //        //    UpdateFilter();
    //        //}
    //    }
    //}


    //// Update is called once per frame
    //void Update()
    //{

    //    //// test
    //    //if (Input.GetMouseButton(0))
    //    //{
    //    //    dfControl controlUnderMouse = dfInputManager.ControlUnderMouse;
    //    //    controlUnderMouse.on
    //    //    if (controlUnderMouse != null)
    //    //    {
    //    //        GameObject go = FindParentWithName(controlUnderMouse.gameObject, "ItemSlotPanel");
    //    //        if (go != null)
    //    //        {
    //    //            ShopItemGUI shopItemGUI = go.GetComponent<ShopItemGUI>();
    //    //            if (shopItemGUI != null)
    //    //            {
    //    //                ShopItem currentShopItem = shopItemGUI.ShopItem;
    //    //                ItemSelectionChanged(lastShopItem, currentShopItem);
    //    //            }
    //    //        }
    //    //        lastControl = controlUnderMouse;

    //    //    }
    //    //}
    //}

    //public GameObject FindParentWithName(GameObject go, string name)
    //{
    //    Transform current = go.transform;
    //    while (current != null)
    //    {
    //        if (current.gameObject.name.StartsWith(name))
    //        {
    //            return current.gameObject;
    //        }
    //        current = current.transform.parent;
    //    }
    //    return null;
    //}

    //public void ItemSelectionChanged(ShopItem previous, ShopItem current)
    //{
    //    if (previous != null)
    //    {
    //        previous.Selected = false;
    //    }


    //    //if (current != null && current.Item != null && detailsPanelLabel != null)
    //    //{
    //    //    current.Selected = true;
    //    //    detailsPanelLabel.Text = current.Item.Name;
    //    //    detailsPanelLabel.Text = string.Format("<h2 color=\"yellow\">{0}</h1><p>PWR: {1} CON:{2} INI: {3}</p><p><i>{4}</i></p>", current.Name, 10, 10, 10, current.Item.Description);


    //    //    //string modelPath = GladiusGlobals.ModelsRoot + current.Item.MeshName;
    //    //    ////GameObject goPrefab = (GameObject)(Resources.Load(modelPath));
    //    //    //GameObject load = Resources.Load(modelPath) as GameObject;
    //    //    //if (load != null)
    //    //    //{
    //    //    //    modelImageSprite.GetComponent<ModelWindowHolder>().AttachModelPrefabToWindow(load);
    //    //    //}
    //    //    //else
    //    //    //{
    //    //    //    Debug.LogWarning("Can't find : " + current.Item.ShortMeshName);
    //    //    //}

    //    //    previous = current;
    //    //}
    //}


    //public void PurchaseItem(ShopItemGUI itemGui)
    //{
    //    //GladiusGlobals.GladiatorSchool.CurrentCharacter.ReplaceItem(itemGui.ShopItem.Item.Name);
    //}

}

