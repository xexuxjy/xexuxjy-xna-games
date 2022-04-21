using System;
using System.Collections.Generic;
using UnityEngine;

public static class LocalisationData
{
    public static void Load()
    {
        String data = GladiusGlobals.ReadTextAsset(GladiusGlobals.DataRoot + "localisationdata");
        if (data != null)
        {
            String[] lines = data.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            char[] splitTokens = { '^' };
            for (int counter = 0; counter < lines.Length; counter++)
            {
                String line = lines[counter];
                if (line.StartsWith("//") || line.StartsWith("#"))
                {
                    continue;
                }

                String[] lineTokens = GladiusGlobals.SplitAndTidyString(line, splitTokens);
                Mappings[int.Parse(lineTokens[0])] = lineTokens[3];


            }
            int ibreak = 0;
        }
    }

    public static String GetValue(int id)
    {
        String result = "";
        Mappings.TryGetValue(id, out result);
        return result;
    }

    private static Dictionary<int, String> Mappings = new Dictionary<int, string>();
}
