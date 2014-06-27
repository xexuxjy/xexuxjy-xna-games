using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace Gladius
{

    public class ItemManager : Dictionary<String, Item>
    {

        //public void LoadItems(String filename)
        //{
        //    //using (StreamReader sr = new StreamReader(filename))
        //    //{
        //    //    String result = null;
        //    //    bool firstSkipped = false;
        //    //    while (!sr.EndOfStream)
        //    //    {
        //    //        result = sr.ReadLine();
        //    //        if (!firstSkipped)
        //    //        {
        //    //            firstSkipped = true;
        //    //            continue;
        //    //        }
        //    //        Item item = new Item(result);
        //    //        Debug.Assert(!ContainsKey(item.ItemId));
        //    //        this[item.ItemId] = item;
        //    //    }
        //    //    //XmlDocument doc = new XmlDocument();
        //    //    //doc.LoadXml(result);
        //    //    //XmlNodeList nodes = doc.SelectNodes("//Item");
        //    //    //foreach (XmlNode node in nodes)
        //    //    //{
        //    //    //    Item item = new Item(node as XmlElement);
        //    //    //    Debug.Assert(!ContainsKey(item.ItemId));
        //    //    //}
        //    //}
        //    //int ibreak = 0;
        //}


        public void LoadExtractedData(String path)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("ExtractedData/ItemData");
            ParseExtractedData(textAsset.text);
        }

        public void ParseExtractedData(String data)
        {
            Item currentItem = null;
            String[] lines = data.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            char[] splitTokens = new char[] { ',', ':' };
            for (int counter = 0; counter < lines.Length; counter++)
            {
                String line = lines[counter];
                if (!line.Contains("ITEM"))
                {
                    continue;
                }

                String[] lineTokens = GladiusGlobals.SplitAndTidyString(line, splitTokens);

                if (line.StartsWith("ITEMCREATE:"))
                {
                    currentItem = new Item();
                    currentItem.Name = lineTokens[1];
                    currentItem.Location = Item.ToLocation(lineTokens[2]);
                    currentItem.ItemSubType = lineTokens[3];
                    currentItem.ClassInfo = lineTokens[4];
                    currentItem.Unknown1 = int.Parse(lineTokens[5]);
                    this[currentItem.Name] = currentItem;
                }
                else if (line.StartsWith(".ITEMDESCRIPTIONID:"))
                {
                    currentItem.DescriptionId = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith(".ITEMCOST:"))
                {
                    currentItem.Cost = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith(".ITEMRARITY:"))
                {
                    currentItem.Rarity = lineTokens[1];
                }
                else if (line.StartsWith(".ITEMREGION:"))
                {
                    currentItem.Region = lineTokens[1];
                }
                else if (line.StartsWith(".ITEMMESH:"))
                {
                    currentItem.MeshName = lineTokens[1];
                }
                else if (line.StartsWith(".ITEMMATERIAL:"))
                {
                    currentItem.MaterialName = lineTokens[1];
                }
                else if (line.StartsWith(".ITEMSKILL:"))
                {
                }
                else if (line.StartsWith(".ITEMAFFINITY:"))
                {
                    currentItem.BuildStat(lineTokens[2], Item.ToAttrType(lineTokens[1]));
                }
                else if (line.StartsWith(".ITEMMINLEVEL:"))
                {
                    currentItem.MinLevel = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith(".ITEMSTATMOD:"))
                {
                    currentItem.BuildStat(lineTokens[2],Item.ToAttrType(lineTokens[1]));
                }


            }
            int ibreak = 0;
        }



    }

    public class Item
    {

        public String Name
        {get;set;}

        public String ItemType
        {
            get;set;
        }

        public String ItemSubType
        {
            get;set;
        }

        public String ClassInfo
        {
            get;set;
        }

        public int Unknown1
        {
            get;set;
        }

        int _descriptionId;
        public int DescriptionId
        {
            get{return _descriptionId;}
            set
            {
                _descriptionId = value;
                Description = GladiusGlobals.LocalisationData[DescriptionId];
            }

        }

        public String Description
        {
            get;
            set;
        }


        public int DisplayNameId
        {
            get;set;
        }


        public int Cost
        {
            get;set;
        }

        public int MinLevel
        {
            get;set;
        }

        public String Rarity
        {
            get;set;
        }

        public String Region
        {
            get;set;
        }

        public String MeshName
        {
            get;set;
        }

        public String MaterialName
        {
            get;set;
        }

        public ItemLocation Location
        { get; set; }

        
        public void BuildStat(String val, GameObjectAttributeType attrType)
        {
            if (!String.IsNullOrEmpty(val))
            {
                int ival = int.Parse(val);
                GameObjectAttributeModifier modifier = new GameObjectAttributeModifier(attrType, GameObjectAttributeModifierDurationType.InstantPermanent, GameObjectAttributeModifierType.Add, ival, 0);
                m_modifiers.Add(modifier);
            }
        }

        public static ItemLocation ToLocation(String val)
        {
            return (ItemLocation)Enum.Parse(typeof(ItemLocation),val);
        }


        public static GameObjectAttributeType ToAttrType(String val)
        {
            if (val == "accuracy") return GameObjectAttributeType.Accuracy;
            if (val == "defense") return GameObjectAttributeType.Defense;
            if (val == "initiative") return GameObjectAttributeType.Initiative;
            if (val == "Water") return GameObjectAttributeType.WaterAffinity;
            if (val == "Fire") return GameObjectAttributeType.FireAffinity;
            if (val == "Earth") return GameObjectAttributeType.EarthAffinity;
            if (val == "Air") return GameObjectAttributeType.AirAffinity;
            if (val == "Light") return GameObjectAttributeType.LightAffinity;
            if (val == "Dark") return GameObjectAttributeType.DarkAffinity;
            return GameObjectAttributeType.NumTypes;
        }

        public List<GameObjectAttributeModifier> Modifiers
        {
            get
            {
                return m_modifiers;
            }
        }

    //    private String m_itemName;
    //    private S
    //    private String m_modelName;
    //    private String m_textureName;
    //    private int m_itemId;
    //    private List<ActorClass> RequiredClasses = new List<ActorClass>();
    //    private List<ActorClass> ProhibitedClasses = new List<ActorClass>();
    //    private ItemLocation m_itemLocation;
      private List<GameObjectAttributeModifier> m_modifiers = new List<GameObjectAttributeModifier>();
    }

}