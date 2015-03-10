using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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

        public static Shop Load(String filename)
        {
            //TextAsset textAsset = (TextAsset)Resources.Load("ExtractedData/Shops/ShopAkarAnGeneric");
            TextAsset textAsset = (TextAsset)Resources.Load(filename);
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
            char[] splitTokens = new char[] { ':', ',','\t'};
            Shop shop = new Shop();
            //GladiusGlobals.CurrentShop = shop;
    
            while (counter < allLines.Length)
            {
                String line = allLines[counter];
                //if(line.StartsWith("//"))
                //{
                //    continue;
                //}
                String[] tokens = GladiusGlobals.SplitAndTidyString(line,splitTokens);

                if(line.StartsWith("NAME"))
                {
                    String descIdStr = tokens[2];
                    int shopDescId = int.Parse(descIdStr);
                    shop.Name = GladiusGlobals.GameStateManager.LocalisationData[shopDescId];
                }
                else if (line.StartsWith("IMAGE"))
                {
                    shop.Image = tokens[1];
                }
                else if (line.StartsWith("SHOPKEEPER"))
                {
                    shop.Owner = tokens[1];                                    
                }
                else if (line.StartsWith("OPENING"))
                {
                    shop.Opening = int.Parse(tokens[1]);
                }
                else if (line.StartsWith("SHOPCONFIRM"))
                {
                    shop.ShopConfirm = int.Parse(tokens[1]);
                }
                else if (line.StartsWith("TALKCONFIRM"))
                {
                    shop.TalkConfirm = int.Parse(tokens[1]);
                }
                else if (line.StartsWith("EXITPHRASE"))
                {
                    shop.ExitPhrase = int.Parse(tokens[1]);
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
        public Boolean DataLoaded
        { get; set; }

        public String Name
        { get; set; }

        public String Image
        { get; set; }

        public String Owner
        { get; set; }

        public String Town
        { get; set; }

        public int Opening
        { get; set; }

        public int ShopConfirm
        { get; set; }

        public int TalkConfirm
        { get; set; }

        public int ExitPhrase
        { get; set; }

        public String OwnerThumbnailName
        { get; set; }

        public Texture OwnerThumnnailTexture
        { get; set; }

        public String ShopFile
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
                shopItems[shopItem.LongName] = shopItem;
                itemCount[shopItem.LongName] = 1;
            }
            //update count
            itemCount[shopItem.LongName]++;
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


        public void LoadData()
        {
            if (!DataLoaded)
            {
                //TextAsset textAsset = (TextAsset)Resources.Load("ExtractedData/Shops/ShopAkarAnGeneric");
                TextAsset textAsset = (TextAsset)Resources.Load(ShopFile);
                if (textAsset != null)
                {
                    Debug.Log("Loading shop file");
                    String data = textAsset.text;
                    ParseExtractedData(data);
                }
                DataLoaded = true;
            }
        }


        public void ParseExtractedData(String data)
        {
            String[] allLines = data.Split('\n');

            int counter = 0;
            char[] splitTokens = new char[] { ':', ',', '\t' };

            while (counter < allLines.Length)
            {
                String line = allLines[counter];
                //if(line.StartsWith("//"))
                //{
                //    continue;
                //}
                String[] tokens = GladiusGlobals.SplitAndTidyString(line, splitTokens);

                if (line.StartsWith("NAME"))
                {
                    String descIdStr = tokens[2];
                    int shopDescId = int.Parse(descIdStr);
                    Name = GladiusGlobals.GameStateManager.LocalisationData[shopDescId];
                }
                else if (line.StartsWith("IMAGE"))
                {
                    Image = tokens[1];
                }
                else if (line.StartsWith("SHOPKEEPER"))
                {
                    Owner = tokens[1];
                }
                else if (line.StartsWith("OPENING"))
                {
                    Opening = int.Parse(tokens[1]);
                }
                else if (line.StartsWith("SHOPCONFIRM"))
                {
                    ShopConfirm = int.Parse(tokens[1]);
                }
                else if (line.StartsWith("TALKCONFIRM"))
                {
                    TalkConfirm = int.Parse(tokens[1]);
                }
                else if (line.StartsWith("EXITPHRASE"))
                {
                    ExitPhrase = int.Parse(tokens[1]);
                }
                else if (line.StartsWith("ITEM"))
                {
                    AddItemData(tokens);
                }
                counter++;
            }
        }



        List<string> itemList = new List<string>();
        Dictionary<String, ShopItem> shopItems = new Dictionary<String, ShopItem>();
        Dictionary<String, int> itemCount = new Dictionary<String, int>();
    }

    

    public class ShopItem
    {
        public ShopItem(String name)
        {
            LongName = name;

            AdjustNameAndDamageType(name);

            //Name = name;

            if (GladiusGlobals.GameStateManager.ItemManager.ContainsKey(name))
            {
                Item = GladiusGlobals.GameStateManager.ItemManager[name];
            }
            else
            {
                Debug.LogWarning("Can't find : " + name);
            }

        }
        public void AdjustNameAndDamageType(String name)
        {
            ReplaceKey(name, "Air ", DamageType.Air);
            ReplaceKey(name, "Earth ", DamageType.Earth);
            ReplaceKey(name, "Fire ", DamageType.Fire);
            ReplaceKey(name, "Water ", DamageType.Water);
            ReplaceKey(name, "Dark ", DamageType.Dark);
            ReplaceKey(name, "Light ", DamageType.Light);
        }

        public bool ReplaceKey(String name, String key, DamageType damageType)
        {
            if (name.StartsWith(key))
            {
                name = name.Remove(0, key.Length);
                Name = name;
                DamageType = damageType;
                return true;
            }
            return false;
        }

        public String LongName
        {
            get;
            set;
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

        public bool Selected
        {
            get;
            set;
        }

        public DamageType DamageType
        {
            get;
            set;
        }

    }

