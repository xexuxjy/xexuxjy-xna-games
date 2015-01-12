using Gladius;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

    public class GameLoadSave
    {
        public void Save(GladiatorSchool school)
        {
            // CharacterName,
            // SchoolName,
            // Roster,
            // Items,
            // TownData,
            // Flags
            StringBuilder sb = new StringBuilder();
            school.SaveSchoolXml(sb);


        }


        public GladiatorSchool Load()
        {
            XmlDocument doc = null;
            GladiatorSchool school = GladiatorSchool.LoadSchoolXml(doc);
            return school;

        }



    }
