using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;
using System.Xml;

public class GladiatorSchool : MonoBehaviour 
{

        public void Hire(CharacterData gladiator)
        {
            System.Diagnostics.Debug.Assert(!m_recruits.Contains(gladiator));
        }

        public void Fire(CharacterData gladiator)
        {
            System.Diagnostics.Debug.Assert(m_recruits.Contains(gladiator));

        }

        public SchoolRank SchoolRank
        {
            get;
            set;
        }

        public void Load(String filename)
        {
            TextAsset textAsset = (TextAsset)Resources.Load(filename);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(textAsset.text);

            XmlNodeList nodes = doc.SelectNodes("//Character");
            foreach (XmlNode node in nodes)
            {
                CharacterData characterData = new CharacterData();
                characterData.Load(node as XmlElement);
                m_recruits.Add(characterData);
            }
        }

        //public void Save(StreamWriter streamWriter)
        //{
        //    streamWriter.Write("<School>");
        //    foreach (CharacterData character in m_recruits)
        //    {
        //        character.Save(streamWriter);
        //    }
        //    streamWriter.Write("</School>");

        //}

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
            System.Diagnostics.Debug.Assert(false);
            return -1;
        }

        public List<CharacterData> Gladiators
        {
            get { return m_recruits; }
        }

        public List<CharacterData> CurrentParty
        {
            get { return m_currentParty; }
        }


        SchoolRank m_currentRank = SchoolRank.Bronze;
        List<CharacterData> m_recruits = new List<CharacterData>();
        List<CharacterData> m_currentParty = new List<CharacterData>();

    }

