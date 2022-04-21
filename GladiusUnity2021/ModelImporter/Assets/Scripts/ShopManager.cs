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
        //TextAsset textAsset = (TextAsset)Resources.Load(filename);
        String data = GladiusGlobals.ReadTextAsset(filename);
        if (data != null)
        {
            Debug.Log("Loading shop file");
            return ParseExtractedData(data);
        }
        return null;
    }


    public static Shop ParseExtractedData(String data)
    {
        String[] allLines = data.Split('\n');

        int counter = 0;
        char[] splitTokens = new char[] { ':', ',', '\t' };
        Shop shop = new Shop();
        //GladiusGlobals.CurrentShop = shop;

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
                shop.Name = LocalisationData.GetValue(shopDescId);
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
    public bool DataLoaded;

    public String Name;

    public String Image;

    public String Owner;

    public String Town;

    public int Opening;

    public int ShopConfirm;

    public int TalkConfirm;

    public int ExitPhrase;

    public String OwnerThumbnailName;

    public Texture OwnerThumnnailTexture;

    public String ShopFile;

    private ItemStore m_itemStore = new ItemStore();

    public ItemStore ItemStore
    {
        get { return m_itemStore; }
    }

    public void AddItemData(String[] data)
    {
        // 12 tokens here... not sure 

        String itemName = data[1];
        AddItem(itemName);
    }

    public void AddItem(String itemName)
    {
        m_itemStore.AddItem(itemName);
    }

    public void RemoveItem(String itemName)
    {
        m_itemStore.RemoveItem(itemName);
    }

    public ShopItem GetItem(String name)
    {
        return m_itemStore.GetItem(name);
    }

    public List<ShopItem> ItemListCopy
    {
        get
        {
            return m_itemStore.ItemListCopy;
        }
    }

    public int GetItemCount(String name)
    {
        return m_itemStore.GetItemCount(name);
    }

    public void LoadData()
    {
        if (!DataLoaded)
        {
            String data = GladiusGlobals.ReadTextAsset(ShopFile);
            if (data != null)
            {
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
                Name = LocalisationData.GetValue(shopDescId);
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

    Dictionary<String, ShopItem> shopItems = new Dictionary<String, ShopItem>();
    Dictionary<String, int> itemCount = new Dictionary<String, int>();
}



public class ShopItem
{
    public ShopItem(String name)
    {
        InternalName = name;


        //Name = name;

        if (ItemManager.Items.ContainsKey(name))
        {
            Item = ItemManager.Items[name];
        }
        else
        {
            //Debug.LogWarning("Can't find : " + name);
        }
        AdjustNameAndDamageType(name);
    }


    public void AdjustNameAndDamageType(String name)
    {
        ReplaceKey(ref name, "Air ", DamageType.Air);
        ReplaceKey(ref name, "Earth ", DamageType.Earth);
        ReplaceKey(ref name, "Fire ", DamageType.Fire);
        ReplaceKey(ref name, "Water ", DamageType.Water);
        ReplaceKey(ref name, "Dark ", DamageType.Dark);
        ReplaceKey(ref name, "Light ", DamageType.Light);
    }

    public bool ReplaceKey(ref String name, String key, DamageType damageType)
    {
        if (name.StartsWith(key))
        {
            name = name.Remove(0, key.Length);
            DamageType = damageType;
        }
        Name = name;

        return true;
    }

    public String InternalName
    {
        get;
        set;
    }

    public String Name
    { get; set; }

    //public int Cost
    //{
    //    get;
    //    set;
    //}

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

    public bool Available
    {
        get; set;
    }

    public bool Affordable
    { get; set; }

    public bool Usable
    { get; set; }

}

