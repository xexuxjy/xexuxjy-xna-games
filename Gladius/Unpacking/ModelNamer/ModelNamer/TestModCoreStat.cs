using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class TestModCoreStat
    {

        List<ModeCOREStat> modeCoreStats = new List<ModeCOREStat>();

        public void LoadMODCOREStats(String[] fileData)
        {
            if (fileData.Length > 0)
            {
                String className = "unk";
                if (fileData[0].StartsWith("// STATSET"))
                {
                    className = fileData[0].Substring(fileData[0].LastIndexOf(":")+1);
                    className = className.Trim();

                    if (!m_classVariantStatData.ContainsKey(className))
                    {
                        m_classVariantStatData[className] = new Dictionary<String, List<ModeCOREStat>>();
                    }


                    List<String> shortList = new List<String>();
                    char[] splitTokens = new char[] { ' ', ',' };
                    for (int i = 1; i < fileData.Length; ++i)
                    {
                        if (fileData[i].StartsWith("MODCORESTATSCOMP:"))
                        {
                            String[] tokens = fileData[i].Split(splitTokens);
                            shortList.Clear();
                            foreach(String token in tokens)
                            {
                                if(!String.IsNullOrEmpty(token))
                                {
                                    shortList.Add(token);
                                }
                            }
                            int counter = 0;

                            String pad = shortList[counter++];
                            String variant = shortList[counter++];
                            String level = shortList[counter++];
                            String con = shortList[counter++];
                            String pow = shortList[counter++];
                            String acc = shortList[counter++];
                            String def = shortList[counter++];
                            String ini = shortList[counter++];
                            String mov = shortList[counter++];

                            ModeCOREStat modCOREStat = new ModeCOREStat();
                            modCOREStat.className = className;
                            modCOREStat.variantName = variant;
                            modCOREStat.level = int.Parse(level);
                            modCOREStat.CON = int.Parse(con);
                            modCOREStat.POW = int.Parse(pow);
                            modCOREStat.ACC = int.Parse(acc);
                            modCOREStat.DEF = int.Parse(def);
                            modCOREStat.INI = int.Parse(ini);
                            modCOREStat.MOV = float.Parse(mov);

                            List<ModeCOREStat> statList;
                            if (!m_classVariantStatData[className].ContainsKey(variant))
                            {
                                m_classVariantStatData[className][variant] = new List<ModeCOREStat>();
                            }

                            statList = m_classVariantStatData[className][variant];

                            statList.Add(modCOREStat);


                            int ibreak = 0;
                        }
                    }


                }
            }


        }

        Dictionary<String, Dictionary<String, List<ModeCOREStat>>> m_classVariantStatData = new Dictionary<String, Dictionary<String, List<ModeCOREStat>>>();

        public static void Main()
        {
            TestModCoreStat test = new TestModCoreStat();
            String[] files = Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\ModCoreStatFiles", "*");


            foreach (String file in files)
            {
                FileInfo sourceFile = new FileInfo(file);
                String[] lines = File.ReadAllLines(sourceFile.FullName);
                test.LoadMODCOREStats(lines);
            }
            int ibreak = 0;
        }

    }

    public class ModeCOREStat
    {
        public String className;
        public String variantName;
        public int level;
        public int CON;
        public int POW;
        public int ACC;
        public int DEF;
        public int INI;
        public float MOV;

    }


}
