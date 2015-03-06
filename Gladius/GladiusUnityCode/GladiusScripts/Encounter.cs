using UnityEngine;
using System.Collections.Generic;

    public class Encounter 
    {
        public static Encounter Load(string name)
        {
            string filename = GladiusGlobals.EncountersPath + name;
            TextAsset textAsset = (TextAsset)Resources.Load(filename);
            Encounter encounter = new Encounter();
            encounter.Parse(textAsset.text);
            return encounter;
        }

        public void Parse(string data)
        {
            string[] lines = data.Split('\n');

            EncounterSide side = null;
            for (int counter = 0; counter < lines.Length; counter++)
            {
                string line = lines[counter];
                if (line.StartsWith("//"))
                {
                    continue;
                }

                string[] lineTokens = GladiusGlobals.SplitAndTidyString(line,new char[] { ',', ':' });
				if(lineTokens.Length > 0)
				{
					if (lineTokens[0] == "SCHOOL")
					{
						side = new EncounterSide();
						Sides.Add(side);
						side.School = new GladiatorSchool();
						side.School.Load(lineTokens[1]);
					}
					else if (lineTokens[0] == "TEAM")
					{
						side.TeamName = lineTokens[1];
						//heroName = lineTokens[1];
					}
					else if (lineTokens[0] == "GLADIATOR")
					{
						side.ChosenGladiators.Add(lineTokens[1]);
					}
            	}
        	}
		}


        public List<EncounterSide> Sides = new List<EncounterSide>();



    }
    public class EncounterSide
    {
        public EncounterSide()
        {
            ChosenGladiators = new List<string>();
        }

        public GladiatorSchool School
        {
            get;
            set;

        }

        public string TeamName
        {
            get;
            set;
        }

        public List<string> ChosenGladiators
        {
            get;
            set;
        }

    }
