using System.Xml;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEngine;

    public class CharacterData
    {
        public void InitValues()
        {
            ACC = 10;
            PWR = 10;
            DEF = 10;
            CON = 10;
            XP = 10;
        }

        public String Name
        {
            get;
            set;
        }

        public String ThumbNailName
        {
            get;
            set;
        }


        public int Level
        {
            get;
            set;
        }

        public int Experience
        {
            get;
            set;
        }

        public int JobPoints
        {
            get;
            set;
        }

        private ActorClass m_actorClass;
        public ActorClass ActorClass
        {
            get { return m_actorClass; }
            set
            {
                m_actorClass = value;
                if (!ActorGenerator.ClassDataMap.ContainsKey(m_actorClass))
                {
                    Debug.LogWarning("Can't find key : " + value.ToString());
                }

                m_actorClassData = ActorGenerator.ClassDataMap[m_actorClass];
            }
        }

        private ActorClassData m_actorClassData;
        public ActorClassData ActorClassData
        {
            get{return m_actorClassData;}
        }


        public Point? StartPosition
        { get; set; }


        public Item GetItemAtLocation(ItemLocation location)
        {
            String itemName = m_itemNames[(int)location];
            if (m_items[(int)location] == null && itemName != null)
            {
                Item item = null;
                if (GladiusGlobals.ItemManager.TryGetValue(itemName, out item))
                {
                    m_items[(int)location] = item;
                }
            }

            return m_items[(int)location];
        }

        //            	CON, PWR, ACC, DEF, INT, MOVE
        public int CON
        { get; set; }

        public int PWR
        { get; set; }

        public int ACC
        { get; set; }

        public int DEF
        { get; set; }

        public int INI
        { get; set; }

        public float MOV
        { get; set; }

        public String TeamName
        {get;set;}

        public GladiatorSchool School
        { get; set; }


        public void ReplaceItem(String itemKey)
        {
            Item item;
            int location = -1;
            if (GladiusGlobals.ItemManager.TryGetValue(itemKey, out item))
            {
                location = (int)item.Location;
            }
            if(m_items[location] != null)
            {
                // return current item to the school
                if (School != null)
                {
                    School.AddToInventory(m_items[location].Name);
                }
                AddItem(itemKey);
            }
        }


        public void AddItem(String itemKey)
        {
            Item item;
            if (GladiusGlobals.ItemManager.TryGetValue(itemKey, out item))
            {
                m_items[(int)item.Location] = item;
                UpdateStats();
            }
        }

        public void RemoveItem(String itemKey)
        {
            Item item;
            if (GladiusGlobals.ItemManager.TryGetValue(itemKey, out item))
            {
                m_items[(int)item.Location] = null;
                UpdateStats();
            }
        }




        private void UpdateStats()
        {
        }

        public int XP
        {
            get;
            set;
        }

        public String GetInfoString()
        {
            if (m_infoString == null)
            {
                StringBuilder sb = new StringBuilder();
                //sb.AppendFormat("*** {0} ***\n", Name);
                //sb.Append("\nClass : " + ActorClass);
                //sb.Append("\nLevel : " + Level);
                //sb.Append("\nPower : " + m_attributeDictionary[GameObjectAttributeType.Power].CurrentValue);
                //sb.Append("\nAccuracy : " + m_attributeDictionary[GameObjectAttributeType.Accuracy].CurrentValue);
                //sb.Append("\nDefense : " + m_attributeDictionary[GameObjectAttributeType.Defense].CurrentValue);
                //sb.Append("\nConstitution : " + m_attributeDictionary[GameObjectAttributeType.Constitution].CurrentValue);
                //sb.Append("\nXP : " + m_attributeDictionary[GameObjectAttributeType.Experience].CurrentValue);

                m_infoString = sb.ToString();
            }
            return m_infoString;
        }

        public void ToXml(StringBuilder sb)
        {
            //            	CON, PWR, ACC, DEF, INT, MOVE");
            sb.AppendFormat("<Unit name=\"{0}\" class=\"{1}\" level=\"{2}\" con=\"{3}\" pwr=\"{4}\" acc=\"{5}\" def=\"{6}\" ini=\"{7}\" move=\"{8}\" xp=\"{9}\" jp=\"{10}\" >",Name,ActorClassData.Name,Level,CON,PWR,ACC,DEF,INI,MOV,XP,JobPoints);
            //	weapon,	armor,	shield,	helmet,	accessory");
            sb.AppendFormat("<Equipment weapon=\"{0}\" armor=\"{1}\" shield=\"{2}\" helmet=\"{3}\" accessory=\"{4}\"/>", GetItemNameAtLoc(ItemLocation.Weapon), GetItemAtLocation(ItemLocation.Armor), GetItemAtLocation(ItemLocation.Shield), GetItemAtLocation(ItemLocation.Helmet), GetItemAtLocation(ItemLocation.Accessory));

                //skills
            sb.AppendFormat("<Skills>");
            foreach (string name in m_skillList)
            {
                sb.AppendFormat("<Skill id=\"{0}\">",name);
            }
            sb.AppendFormat("</Skills>");
            sb.AppendFormat("</Unit>");
        }

        public static CharacterData FromXml(XmlNode node)
        {
            CharacterData cd = new CharacterData();
            cd.Name = node.SelectSingleNode("@name").Value;
            cd.ActorClass = (ActorClass)Enum.Parse(typeof(ActorClass), node.SelectSingleNode("@class").Value);
            cd.Level= int.Parse(node.SelectSingleNode("@level").Value);
            cd.CON = int.Parse(node.SelectSingleNode("@con").Value);
            cd.PWR = int.Parse(node.SelectSingleNode("@pwr").Value);
            cd.ACC = int.Parse(node.SelectSingleNode("@acc").Value);
            cd.DEF = int.Parse(node.SelectSingleNode("@def").Value);
            cd.INI = int.Parse(node.SelectSingleNode("@ini").Value);
            cd.MOV = int.Parse(node.SelectSingleNode("@move").Value);
            cd.XP = int.Parse(node.SelectSingleNode("@xp").Value);
            cd.JobPoints = int.Parse(node.SelectSingleNode("@jp").Value);

            cd.AddItemByNameAndLoc(node.SelectSingleNode("Equipment/@weapon").Value,ItemLocation.Weapon);
            cd.AddItemByNameAndLoc(node.SelectSingleNode("Equipment/@armor").Value, ItemLocation.Armor);
            cd.AddItemByNameAndLoc(node.SelectSingleNode("Equipment/@shield").Value, ItemLocation.Shield);
            cd.AddItemByNameAndLoc(node.SelectSingleNode("Equipment/@helmet").Value, ItemLocation.Helmet);
            cd.AddItemByNameAndLoc(node.SelectSingleNode("Equipment/@accessory").Value, ItemLocation.Accessory);

            XmlNodeList skills = node.SelectNodes("//Skill");
            foreach (XmlNode skill in skills)
            {
                cd.m_skillList.Add(node.SelectSingleNode("@id").Value);
            }
            return cd;

        }


        public void AddSkill(String skillName)
        {
            m_skillList.Add(skillName);
        }

        public void AddItemByNameAndLoc(String itemName,ItemLocation loc)
        {
            m_itemNames[(int)loc] = itemName;
        }

        public String GetItemNameAtLoc(ItemLocation loc)
        {
            return m_itemNames[(int)loc];
        }

        public bool Selected
        {
            get;
            set;
        }
        private Item[] m_items = new Item[(int)ItemLocation.NumItems];
        private String[] m_itemNames = new String[(int)ItemLocation.NumItems];
        private String m_infoString;
        public List<String> m_skillList = new List<String>();
    }
