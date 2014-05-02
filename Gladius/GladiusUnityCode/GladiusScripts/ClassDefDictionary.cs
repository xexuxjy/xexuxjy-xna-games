using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gladius
{
    public class ClassDef
    {
        public String ClassName
        { get; set; }

        public int DisplayNameId
        { get; set; }

        public int DescriptionId
        {
            get;
            set;
        }

        public String UnitSetName
        { get; set; }

        public String SkillUseName
        { get; set; }

        public String VoiceInlinePrefix
        { get; set; }

        public String SoundTableName
        { get; set; }

        public String MeshName
        { get; set; }

        public String HeadIconName
        { get; set; }

        public String ClassIconName
        { get; set; }

        public int GridSize
        { get; set; }

        public float XPAward1
        { get; set; }

        public float XPAward2
        { get; set; }

        public float ItemSizes1
        { get; set; }

        public float ItemSizes2
        { get; set; }

        public float ItemSizes3
        { get; set; }

        public List<String> Attributes
        { get; set; }

        //public List<String> ItemCat
        //{ get; set; }


        public AffinityType AffinityType
        { get; set; }



        public bool HasAttribute(String name)
        {
            return Attributes.IndexOf(name) >= 0;
        }

        public void AddItemCat(String location, String type, String restriction)
        {

        }



    }


	public class ClassDefDictionary : Dictionary<String,ClassDef>
	{

        public void LoadExtractedData(String path)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("ExtractedClassDefs");
            ParseExtractedData(textAsset.text);
        }

        /*
         * CREATECLASS: BeastAirGreater

DISPLAYNAMEID: 7879
DESCRIPTIONID: 0

	UNITSETNAME: "AffinityBeastGreater"
	SKILLUSENAME: "AffinityBeastGreater"
	SOUNDTABLENAME: "BeastAirGreater"
	MESH: "affinityAirGreater"
	HEADICON: "beastAir.tga"

	GRIDSIZE: "3"
	XPAWARD: "75", "1.52"
	ITEMSIZES: 1, 1, 1, 

	AFFINITY: "Air"
	ATTRIBUTE: "Beast"
	ATTRIBUTE: "NoKnockdown"
	ATTRIBUTE: "NoKnockback"
	ATTRIBUTE: "alwaysfaceondefend"
	ATTRIBUTE: "deathinstantdisappear"
	ATTRIBUTE: "simpleanims"

	////  ROCKSCISSORSPAPER
	ROCKSCISSORSPAPER: "Plain"

	////  WEAPONS

	////  ARMORS

	////  HELMETS

	////  SHIELDS

	////  ACCESSORIES
	FX: "death disappear", "affinityAirGreater_teleport"
	FX: "continuous alive", "affinityAirGreater_ambientEffects"

	LEVELUPXPNEEDED: 850, 1981, 3484, 5484, 1.52
	LEVELUPJPGIVEN: 5, 10, 10, 15, 15

	LEVELSTATAWARDS: 7, 7, 0, 0, 0, 0
	LEVELSTATAWARDS: 7, 7, 0, 0, 0, 0
	LEVELSTATAWARDS: 4, 4, 0, 1, 0, 1
	LEVELSTATAWARDS: 4, 4, 0, 1, 0, 1
	LEVELSTATAWARDS: 6, 6, 0, 1, 0, 0
	LEVELSTATAWARDS: 0, 0, 0, 0, 0, 0
	LEVELZEROSTATS: 10, 10, 20, 10, 10, 20
*/
        public void ParseExtractedData(String data)
        {
            Item currentItem = null;
            String[] lines = data.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            ClassDef currentClassDef = null;
            for (int counter = 0; counter < lines.Length; counter++)
            {
                String line = lines[counter];
                line = line.Trim();
                if (line.Length == 0 || line.StartsWith("//"))
                {
                    continue;
                }

                String[] lineTokens = line.Split(',');

                try
                {
                if (line.StartsWith("CREATECLASS:"))
                {
                    currentClassDef = new ClassDef();
                    currentClassDef.ClassName = lineTokens[1];
                    this[currentClassDef.ClassName] = currentClassDef;
                }
                else if (line.StartsWith("DISPLAYNAMEID:"))
                {
                    currentClassDef.DisplayNameId = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith("DESCRIPTIONID:"))
                {
                    currentClassDef.DescriptionId = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith("UNITSETNAME:"))
                {
                    currentClassDef.UnitSetName = lineTokens[1];
                }
                else if (line.StartsWith("VOICELINEPREFIX:"))
                {
                    //currentClassDef.voi
                }
                else if (line.StartsWith("SKILLUSENAME:"))
                {
                    currentClassDef.SkillUseName = lineTokens[1];
                }
                else if (line.StartsWith("SOUNDTABLENAME:"))
                {
                    currentClassDef.SoundTableName = lineTokens[1];
                }
                else if (line.StartsWith("MESH:"))
                {
                    currentClassDef.MeshName = lineTokens[1];
                }
                else if (line.StartsWith("HEADICON:"))
                {
                    currentClassDef.HeadIconName = lineTokens[1];
                }
                else if (line.StartsWith("CLASSICON:"))
                {
                    currentClassDef.ClassIconName = lineTokens[1];
                }
                else if (line.StartsWith("GRIDSIZE:"))
                {
                    currentClassDef.DescriptionId = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith("XPAWARD:"))
                {
                    currentClassDef.XPAward1= float.Parse(lineTokens[1]);
                    currentClassDef.XPAward2 = float.Parse(lineTokens[2]);
                }
                else if (line.StartsWith("ITEMSIZES:"))
                {
                    currentClassDef.ItemSizes1= float.Parse(lineTokens[1]);
                    currentClassDef.ItemSizes2 = float.Parse(lineTokens[2]);
                    currentClassDef.ItemSizes3 = float.Parse(lineTokens[3]);
                }
                else if (line.StartsWith("AFFINITY:"))
                {
                    currentClassDef.AffinityType = (AffinityType)Enum.Parse(typeof(AffinityType), lineTokens[1]);
                }
                else if (line.StartsWith("ATTRIBUTE:"))
                {
                    currentClassDef.Attributes.Add(lineTokens[1]);
                }
                else if (line.StartsWith("ITEMCAT:"))
                {
                    currentClassDef.AddItemCat(lineTokens[1], lineTokens[2], lineTokens[3]);
                }
                else if (line.StartsWith("ROCKSCISSORSPAPER:"))
                {
                }
                else if (line.StartsWith("FX:"))
                {
                }
                else if (line.StartsWith("LEVELUPXPNEEDED:"))
                {
                }
                else if (line.StartsWith("LEVELUPJPGIVEN:"))
                {
                }
                else if (line.StartsWith("LEVELSTATAWARDS:"))
                {
                }
                else if (line.StartsWith("LEVELZEROSTATS:"))
                {
                }
                else
                {
                    // unknown...
                    int ibreak = 0;
                }
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;
                }



            }

            int finished = 0;
        }
	}
}
