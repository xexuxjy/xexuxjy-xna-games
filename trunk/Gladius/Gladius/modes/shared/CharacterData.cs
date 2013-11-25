using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using System.IO;
using System.Xml;
using Gladius.actors;
using Gladius.util;

namespace Gladius.modes.shared
{
    // simple data form of character for saving, management etc.
    public class CharacterData
    {
        
 //<Character Name="Urlan">
 //   <Attributes Accuracy ="40" Defense= "60" Power= "60" Consitution= "80" Experience="1000" Level ="1"/>
 //   <Skills>

 //   </Skills>
 //   <Equipment Head ="" Arm1="" Arm2="" Body="" Special=""/>
 // </Character>        
        
        public void InitValues()
        {
            m_attributeDictionary[GameObjectAttributeType.Accuracy] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Power] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Defense] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Constitution] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.CharacterSkillPoints] = new BoundedAttribute(10);
            m_attributeDictionary[GameObjectAttributeType.Experience] = new BoundedAttribute(10);

        }

        public void Load(XmlElement xmlElement)
        {
            SetupCharacterData(xmlElement);
        }

        public void Save(StreamWriter streamWriter )
        {
            StringBuilder data = new StringBuilder();
            data.AppendFormat("<Character Name=\"{0}\">", Name);
            data.Append("<Attributes ");
            foreach (GameObjectAttributeType attrType in m_attributeDictionary.Keys)
            {
                data.AppendFormat("{0}=\"{1}\" ", attrType, m_attributeDictionary[attrType].CurrentValue);
            }
            data.Append(" />");

            data.AppendFormat("<Equipment Head=\"{0}\" Arm1=\"{1}\" Arm2=\"{2}\" Body=\"{3}\" Special=\"{4}\" />\n", 
                m_items[(int)ItemLocation.Head], m_items[(int)ItemLocation.LHand], m_items[(int)ItemLocation.RHand], 
                m_items[(int)ItemLocation.Body], m_items[(int)ItemLocation.Special]);

            data.Append("</Character>");
            streamWriter.WriteLine(data);
        }

        public int XPToNextLevel
        {
            get
            {
                if (Level < Globals.MaxLevel-2)
                {
                    int nextlevel = ActorGenerator.ActorXPLevels[ActorClass][Level + 1];
                    return nextlevel - m_attributeDictionary[GameObjectAttributeType.Experience].CurrentValue;
                }
                return -1;
            }
        }


        public String Name
        {
            get;set;
        }

        public int Level
        {
            get;
            set;
        }

        public ActorClass ActorClass
        {
            get;
            set;
        }

        public Item GetItemAtLocation(ItemLocation location)
        {
            return m_items[(int)location];
        }
        
        
        /*
         *   <Character Name="" Accuracy ="" Defense= "" Power= "" Consitution= "" Experience="" Level ="">
    <Skills>
    
    </Skills>
    <Equipment Head ="" Arm1="" Arm2="" Body="" Special=""/>
  </Character>
*/
        public void SetupCharacterData(XmlElement element)
        {
            Name = element.Attributes["Name"].Value;
            XmlElement attributes = (XmlElement)element.SelectSingleNode("Attributes");
            foreach (XmlAttribute attr in attributes.Attributes)
            {
                try
                {
                    GameObjectAttributeType attrType = (GameObjectAttributeType)Enum.Parse(typeof(GameObjectAttributeType), attr.Name);
                    int val = int.Parse(attr.Value);
                    m_attributeDictionary[attrType] = new BoundedAttribute(val);
                }
                catch (System.Exception ex)
                {

                }

            }

        }

        public void AddItem(int itemKey)
        {
            Item item;
            if(Globals.ItemManager.TryGetValue(itemKey,out item))
            {
                m_items[(int)item.Location] = item;
                UpdateStats();
            }
        }

        public void RemoveItem(int itemKey)
        {
            Item item;
            if(Globals.ItemManager.TryGetValue(itemKey,out item))
            {
                m_items[(int)item.Location] = null;
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
        }

        public Dictionary<GameObjectAttributeType, BoundedAttribute> Attributes
        {
            get
            {
                return m_attributeDictionary;
            }
        }

        private Item[] m_items = new Item[(int)ItemLocation.NumItems];
        private Dictionary<GameObjectAttributeType, BoundedAttribute> m_attributeDictionary = new Dictionary<GameObjectAttributeType, BoundedAttribute>();

    }

}


