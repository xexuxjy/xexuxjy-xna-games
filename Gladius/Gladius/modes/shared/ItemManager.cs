using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Gladius.actors;

namespace Gladius.modes.shared
{
    public class ItemManager : Dictionary<int,Item>
    {

        public void LoadItems(String filename)
        {
            using (StreamReader sr = new StreamReader(TitleContainer.OpenStream(filename)))
            {
                String result = null;
                bool firstSkipped = false;
                while (!sr.EndOfStream)
                {
                    result = sr.ReadLine();
                    if (!firstSkipped)
                    {
                        firstSkipped = true;
                        continue;
                    }
                    Item item = new Item(result);
                    Debug.Assert(!ContainsKey(item.ItemId));
                    this[item.ItemId] = item;
                }
                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(result);
                //XmlNodeList nodes = doc.SelectNodes("//Item");
                //foreach (XmlNode node in nodes)
                //{
                //    Item item = new Item(node as XmlElement);
                //    Debug.Assert(!ContainsKey(item.ItemId));
                //}
            }
            int ibreak = 0;
        }


    }

    public class Item
    {
        public Item(int id, String name, ItemLocation location, List<GameObjectAttributeModifier> modifiers,String modelName,String textureName)
        {
            m_itemId = id;
            m_itemName = name;
            m_itemLocation = location;
            m_modifiers.AddRange(modifiers);
            m_modelName = modelName;
            m_textureName = textureName;
        }

        public Item(XmlElement element)
        {
            m_itemId = int.Parse(element.Attributes["id"].Value);
            m_itemName = element.Attributes["name"].Value;
            m_itemLocation = (ItemLocation)Enum.Parse(typeof(ItemLocation), element.Attributes["location"].Value);
            m_modelName = element.Attributes["model"].Value;
            m_textureName = element.Attributes["texture"].Value;
        }

        public Item(String csvLine)
        {
            String[] fields = csvLine.Split(',');
            m_itemId = int.Parse(fields[(int)ItemColumn.ID]) ;
            m_itemName = fields[(int)ItemColumn.NAME];
            m_itemLocation = ToLocation(fields[(int)ItemColumn.NAME]);


        }

        public ItemLocation ToLocation(String val)
        {
            if (val == "Armor") return ItemLocation.Body;
            if (val == "Weapon") return ItemLocation.RHand;
            if (val == "Shield") return ItemLocation.LHand;
            if (val == "Helmet") return ItemLocation.Head;
            if (val == "Accessory") return ItemLocation.Special;
            return ItemLocation.Special;
        }


        public enum ItemColumn
        {
            ID=0,	
            NAME,	
            TYPE,	
            SUBTYPE,	
            CLASSTYPE,	
            PWR,	
            ACC,
            DEF,	
            INI,	
            AFFTYPE,	
            AFFVALUE,	
            SPECIAL,	
            SKILL,	
            MINLEVEL,	
            RARITY,	
            COST,
            REGION,	
            REGION2,	
            REGION3,	
            REGION4,	
            ATTRIBUTE1,	
            ATTRIBUTE2,	
            ATTRIBUTE3,
            ATTRIBUTE4, 
            DESC
        }


        public String ItemName
        {
            get
            {
                return m_itemName;
            }
        }

        public int ItemId
        {
            get
            {
                return m_itemId;
            }
        }

        public ItemLocation Location
        {
            get
            {
                return m_itemLocation;
            }
        }

        public List<GameObjectAttributeModifier> Modifiers
        {
            get
            {
                return m_modifiers;
            }
        }

        private String m_itemName;
        private String m_modelName;
        private String m_textureName;
        private int m_itemId;
        private List<ActorClass> RequiredClasses = new List<ActorClass>();
        private List<ActorClass> ProhibitedClasses = new List<ActorClass>();
        private ItemLocation m_itemLocation;
        private List<GameObjectAttributeModifier> m_modifiers;
    }


    public enum ItemLocation
    {
        Head,
        LHand,
        RHand,
        Body,
        Special
    }


}
