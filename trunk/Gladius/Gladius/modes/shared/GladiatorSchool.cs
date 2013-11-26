using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Gladius.modes.shared
{
    public class GladiatorSchool
    {
        public void Hire(CharacterData gladiator)
        {
            Debug.Assert(!m_recruits.Contains(gladiator));
        }

        public void Fire(CharacterData gladiator)
        {
            Debug.Assert(m_recruits.Contains(gladiator));

        }

        public SchoolRank SchoolRank
        {
            get;
            set;
        }

        public void Load(String filename)
        {
            using (StreamReader sr = new StreamReader(TitleContainer.OpenStream(filename)))
            {
                String result = sr.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                XmlNodeList nodes = doc.SelectNodes("//Character");
                foreach (XmlNode node in nodes)
                {
                    CharacterData characterData = new CharacterData();
                    characterData.Load(node as XmlElement);
                    m_recruits.Add(characterData);
                }
            }
        }

        public void Save(StreamWriter streamWriter)
        {
            streamWriter.Write("<School>");
            foreach (CharacterData character in m_recruits)
            {
                character.Save(streamWriter);
            }
            streamWriter.Write("</School>");

        }

        public int CurrentSize
        {
            get;
            set;
        }

        public int GetMaxSize()
        {
            switch (m_currentRank)
            {
                case SchoolRank.Bronze: return 8;
                case SchoolRank.Silver: return 16;
                case SchoolRank.Gold: return 24;
            }
            Debug.Assert(false);
            return -1;
        }

        public List<CharacterData> Gladiators
        {
            get { return m_recruits; }
        }


        SchoolRank m_currentRank = SchoolRank.Bronze;
        List<CharacterData> m_recruits = new List<CharacterData>();
    }

    public enum SchoolRank
    {
        Bronze,
        Silver,
        Gold
    }
}
