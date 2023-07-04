    using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class ItemStore 
{

    public void AddItem(string itemName)
    {
        ShopItem shopItem = null;

        if (!shopItems.TryGetValue(itemName, out shopItem))
        {
            shopItem = new ShopItem(itemName);
            shopItems[shopItem.InternalName] = shopItem;
            itemCount[shopItem.InternalName] = 0;
        }
        //update count
        itemCount[shopItem.InternalName]++;

    }

    public void RemoveItem(string itemName)
    {
        ShopItem shopItem = null;
        if (shopItems.TryGetValue(itemName, out shopItem))
        {
            int newLevel = Math.Max(0, itemCount[shopItem.InternalName] - 1);
            itemCount[shopItem.InternalName] = newLevel;
        }
    }

    public ShopItem GetItem(string name)
    {
        return shopItems[name];
    }

    public List<ShopItem> ItemListCopy
    {
        get
        {
            List<ShopItem> shopItemList = new List<ShopItem>();
            shopItemList.AddRange(shopItems.Values);
            //foreach (ShopItem shopItem in shopItems.Values)
            //{
            //    shopItemList.Add(shopItem);
            //}
            return shopItemList;
        }
    }

    public int GetItemCount(string name)
    {
        int count = 0;
        if (!itemCount.TryGetValue(name, out count))
        {
            int ibreak = 0;
        }
        return count;
    }


    private List<string> m_itemsAsStrings = new List<string>();
    List<string> m_schoolInventory = new List<string>();
    Dictionary<string, int> m_inventoryCounts = new Dictionary<string, int>();

    Dictionary<string, ShopItem> shopItems = new Dictionary<string, ShopItem>();
    Dictionary<string, int> itemCount = new Dictionary<string, int>();


}