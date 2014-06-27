using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Gladius
{
    public class ShopManager : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static Shop LoadOriginalShopFile(String filename)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("ExtractedData/Shops/ShopAkarAnGeneric");
            if (textAsset != null)
            {
                Debug.Log("Loading shop file");
                String data = textAsset.text;
                return ParseExtractedData(data);
            }
            return null;
        }


        public static Shop ParseExtractedData(String data)
        {
            String[] allLines = data.Split('\n');

            int counter = 0;
            char[] splitTokens = new char[] { ':', ',','\t' };
            Shop shop = new Shop();
            GladiusGlobals.CurrentShop = shop;
    
            while (counter < allLines.Length)
            {
                String line = allLines[counter];
                //if(line.StartsWith("//"))
                //{
                //    continue;
                //}
                String[] tokens = GladiusGlobals.SplitAndTidyString(line,splitTokens);

                if(line.StartsWith("Name"))
                {
                    shop.Name = tokens[1];
                }
                else if (line.StartsWith("SHOPKEEPER"))
                {
                    shop.Owner = tokens[1];                                    
                }
                else if (line.StartsWith("ITEM"))
                {
                    shop.AddItemData(tokens);
                }
                counter++;
            }
            return shop;
        }

        public Dictionary<String, Shop> AllShops = new Dictionary<String, Shop>();

    }

    public class Shop
    {
        public String Name
        { get; set; }

        public String Owner
        { get; set; }

        public String Town
        { get; set; }


        public void AddItemData(String[] data)
        {
            // 12 tokens here... not sure 

            String itemName = data[1];
            ShopItem shopItem = null;
            
            if (!shopItems.TryGetValue(itemName,out shopItem))
            {
                itemList.Add(itemName);
                shopItem = new ShopItem(data[1]);
                shopItems[shopItem.Name] = shopItem;
                itemCount[shopItem.Name] = 1;
            }
            //update count
            itemCount[shopItem.Name]++;
        }

        public ShopItem GetItem(String name)
        {
            return shopItems[name];
        }

        public int GetItemCount(String name)
        {
            return itemCount[name];
        }

        public List<string> GetItemList()
        {
            return itemList;
        }

        List<string> itemList = new List<string>();
        Dictionary<String, ShopItem> shopItems = new Dictionary<String, ShopItem>();
        Dictionary<String, int> itemCount = new Dictionary<String, int>();
    }

    

    public class ShopItem
    {
        public ShopItem(String name)
        {
            Name = name;

            if (GladiusGlobals.ItemManager.ContainsKey(name))
            {
                Item = GladiusGlobals.ItemManager[name];
            }
            else
            {
                Debug.LogWarning("Can't find : " + name);
            }

        }

        public String Name
        { get; set; }

        public int Cost
        {
            get;
            set;
        }

        public Item Item
        {
            get;
            set;
        }

    }



}

