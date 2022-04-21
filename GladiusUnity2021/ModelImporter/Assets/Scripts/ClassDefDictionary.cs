using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public List<String> ItemCat
        { get; set; }




    }


	public class ClassDefDictionary : Dictionary<String,ClassDef>
	{
	}
