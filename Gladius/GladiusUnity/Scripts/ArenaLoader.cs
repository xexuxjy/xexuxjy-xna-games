using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;


public static class ArenaLoader
{
    public static void SetupArena(String arenaDataName,Arena arena)
    {
        //Arena arena = new Arena();
        List<String> lines = new List<String>();

        TextAsset textAsset = (TextAsset)Resources.Load(arenaDataName);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        arena.ModelName = doc.SelectSingleNode("Arena/Model").InnerText;

        arena.ModelPosition = GladiusGlobals.ReadVector3("Arena/Position",doc);
        arena.ModelScale = GladiusGlobals.ReadVector3("Arena/Scale", doc);
        arena.ModelRotation = GladiusGlobals.ReadVector3("Arena/Rotation", doc);
            
        arena.SkyBoxMaterialName = doc.SelectSingleNode("Arena/SkyBoxMaterial").InnerText;

        String gridFilename = doc.SelectSingleNode("Arena/GridFile").InnerText;
        GridFile gridFile = GridFileReader.ReadFile(gridFilename);

        arena.AddGridFile(gridFile);
        arena.LoadAllGridFiles();

        arena.SetupData();
        arena.SetupScenery();

        PopulateList(doc, "Arena/PlacementPoints/Player//Point", arena.PlayerPointList);
        PopulateList(doc, "Arena/PlacementPoints/Team1//Point", arena.Team1PointList);
        PopulateList(doc, "Arena/PlacementPoints/Team2//Point", arena.Team2PointList);
        PopulateList(doc, "Arena/PlacementPoints/Team3//Point", arena.Team3PointList);
    }

    private static void PopulateList(XmlDocument doc, String xpath, List<Point> points)
    {
        XmlNodeList nodes = doc.SelectNodes(xpath);
        foreach (XmlNode node in nodes)
        {
            int x = Int32.Parse(node.Attributes["x"].Value);
            int y = Int32.Parse(node.Attributes["y"].Value);
            points.Add(new Point(x, y));
        }
    }

    public static void SetupArena(ArenaEncounter arenaEncounter,Arena arena)
    {
        // setup the arena based on the arenaEncounter data
        //arena.PrefabName = arenaEncounter.Encounter.
        //arena.TextureName = doc.SelectSingleNode("Arena/Texture").InnerText;




    }
}

public static class GridFileReader
{
    public static GridFile ReadFile(String gridFileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(gridFileName);
        return ReadFile(gridFileName,textAsset);
    }

    public static GridFile ReadFile(String gridFileName,TextAsset textAsset)
    {
        int rowLength = 32;
        uint[] arenaGrid = new uint[rowLength*rowLength];
        List<string> entryList = new List<string>();

        using (BinaryReader binReader = new BinaryReader(new MemoryStream(textAsset.bytes)))
        {
            int header = binReader.ReadInt32();
            int numEntries = binReader.ReadInt32();
            int totalEntries = 30;
            for (int i = 0; i < totalEntries; ++i)
            {
                String s = System.Text.Encoding.Default.GetString(binReader.ReadBytes(32));
                s = s.TrimEnd('\0');
                //s = s.Remove('\0');
                entryList.Add(s);
            }


            for (int i = 0; i < rowLength * rowLength; ++i)
            {
                arenaGrid[i] = binReader.ReadUInt32();
            }
        }

        GridFile gridFile = new GridFile(gridFileName, arenaGrid,entryList);

        return gridFile;
    }

}


public class GridFile
{
    static String noMoveKey = "FNoMove";
    static String noMoveNoCursorKey = "FNoMoveNoCursor";
    static String mapCenterKey = "FMapCenter";

    static List<String> teamStartKeys = new string[] {"FStart1", "FStart2", "FStart3", "FStart4",
                                                "FStart5", "FStart6", "FStart7", "FStart8", }.ToList<string>();


    List<string> entrySlots = new List<string>();
    

    public GridFile(String fileName, uint[] gridData,List<string> entryList)
    {
        FileName = fileName;
        GridData = gridData;
        EntryList = entryList;
        MapCenter = new Point(16, 16);
        SetupGrid();
    }


    private bool TestPointKey(uint val, string key)
    {
        int index = entrySlots.IndexOf(key);
        if (index >= 0)
        {
            uint mask = (uint)(2 ^ index);
            if ((val & mask) == mask)
            {
                return true;
            }
        }
        return false;
    }

    private void SetupGrid()
    {
        bool foundCenter = false;

        for (int i = 0; i < Arena.ArenaSize; ++i)
        {
            for (int j = 0; j < Arena.ArenaSize; ++j)
            {
                int index = (Arena.ArenaSize * i) + j;
                uint v = GridData[index];

                if (TestPointKey(v, mapCenterKey))
                {
                    if (!foundCenter)
                    {
                        MapCenter = new Point(i, j);
                        foundCenter = true;
                    }
                    else
                    {
                        int ibreak = 0; // error
                    }
                }

                foreach (String teamKey in teamStartKeys)
                {
                    if (TestPointKey(v, teamKey))
                    {
                        List<Point> startPoints;
                        if (!TeamStartPoints.TryGetValue(teamKey, out startPoints))
                        {
                            startPoints = new List<Point>();
                            TeamStartPoints[teamKey] = startPoints;
                        }
                        startPoints.Add(new Point(i, j));
                    }
                }
            }
        }
    }

    private int GetIndex(int x, int y)
    {
        return ((Arena.ArenaSize - 1 - y) * Arena.ArenaSize) + (Arena.ArenaSize - 1 - x);
    }
    public bool IsBlocked(int x, int y)
    {
        //int index = (x * Arena.ArenaSize) + y;
        //int index = (y * Arena.ArenaSize) + x;
        // flip x&y here so it aligns with grid
        int index = GetIndex(x, y);
        return (GridData[index] & 0x01) == 0x01;
    }

    public int GetHeightAtPoint(int x, int y)
    {
        return 0;
    }

    public String FileName;
    public uint[] GridData;
    public List<string> EntryList;
    public Point MapCenter;
    public Dictionary<string, List<Point>> TeamStartPoints = new Dictionary<string, List<Point>>();
}



