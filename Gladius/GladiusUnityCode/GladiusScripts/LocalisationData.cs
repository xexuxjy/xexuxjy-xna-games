using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gladius
{
	public class LocalisationData : Dictionary<int,String>
	{

        public void LoadExtractedData(String path)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("LocalisationData");
            String data = textAsset.text;
            String[] lines = textAsset.text.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int counter = 0; counter < lines.Length; counter++)
            {
                String line = lines[counter];
                String[] lineTokens = line.Split('^');
                this[int.Parse(lineTokens[0])] = lineTokens[1];


            }
            int ibreak = 0;
        }
	}
}
