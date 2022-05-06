using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;



public static class GridFileManager
{
    public static GridFile GetGridFile(String gridFileName)
    {
        GridFile gridFile = null;
        if(!m_gidFileMap.TryGetValue(gridFileName,out gridFile))
        {
            TextAsset textAsset = GladiusGlobals.LoadTextAsset("GladiusData/" + gridFileName);
            if (textAsset != null)
            {
                gridFile = ReadFile(gridFileName, textAsset);
                m_gidFileMap[gridFileName] = gridFile;
            }
        }
        return gridFile;
    }

    public static GridFile ReadFile(String gridFileName,TextAsset textAsset)
    {
        int rowLength = Arena.ArenaSize;
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
                // first character is F for full or E for empty - don't need.
                s = s.Substring(1);
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

    private static Dictionary<string, GridFile> m_gidFileMap = new Dictionary<string, GridFile>();
}


public class GridFile 
{
    public static String noMoveKey = "NoMove";
    public static String noMoveNoCursorKey = "NoMoveNoCursor";
    public static String mapCenterKey = "MapCenter";

    public static List<String> teamStartKeys = new string[] {"Start1", "Start2", "Start3", "Start4",
                                                "Start5", "Start6", "Start7", "Start8", }.ToList<string>();


    public bool FlipX=true;
    public bool FlipY=true;
    public bool SwapXY;


    public GridFile() : this("Dummy", new uint[Arena.ArenaSize *  Arena.ArenaSize],new List<string>())
    {
        
    }

    public GridFile(String fileName, uint[] gridData,List<string> entryList)
    {
        FileName = fileName;
        GridData = gridData;
        EntryList = entryList;
        MapCenter = new Point(Arena.ArenaSize/2, Arena.ArenaSize/2);
        SetupGrid();
    }

    public void SetPoint(int x, int y, string key)
    {
        uint mask = 0;
        if (key != null)
        {
            int keyIndex = EntryList.IndexOf(key);
            if (keyIndex != -1)
            {
                mask = (uint)(2 ^ keyIndex);
            }
        }

        int gridIndex = GetIndex(x, y);
        GridData[gridIndex] = mask;
    }

    public bool TestPointKey(int x, int y, string key)
    {
        uint val = GridData[GetIndex(x, y)];
        return TestPointKey(val, key);
    }

    private bool TestPointKey(uint val, string key)
    {
        int index = EntryList.IndexOf(key);
        if (index >= 0)
        {
            uint mask = (uint)1 << index;
            uint mask2 = (uint)(2 ^ index);
            if ((val & (mask)) != 0)
            {
                return true;
            }
        }
        return false;
    }

    private void SetupGrid()
    {
        for(int x=0;x<Arena.ArenaSize;++x)
        {
            for (int y = 0; y < Arena.ArenaSize; ++y)
            {
                if(TestPointKey(x,y,mapCenterKey))
                {
                    MapCenter = new Point(x, y);
                }

                if (!IsBlocked(x, y))
                {
                    foreach (String teamKey in teamStartKeys)
                    {
                        if (TestPointKey(x, y, teamKey))
                        {
                            List<Point> startPoints;
                            if (!TeamStartPoints.TryGetValue(teamKey, out startPoints))
                            {
                                startPoints = new List<Point>();
                                TeamStartPoints[teamKey] = startPoints;
                            }
                            startPoints.Add(new Point(x, y));
                        }
                    }
                }
            }
        }
    }

    public List<Point> GetTeamStartPointa(int teamIndex)
    {
        List<Point> result = null;
        TeamStartPoints.TryGetValue("Start" + (teamIndex + 1),out result);
        return result;
    }

    private int GetIndex(int x, int y)
    {
        int yVal = Arena.ArenaSize - 1 - y;
        int xVal = Arena.ArenaSize - 1 - x;
        if (FlipX)
        {
            xVal = x;
        }
        if (FlipY)
        {
            yVal = y;
        }

        if (SwapXY)
        {
            int temp = xVal;
            xVal = yVal;
            yVal = temp;
        }

        return ((yVal) * Arena.ArenaSize) + (xVal);
    }
    public bool IsBlocked(int x, int y)
    {
        // flip x&y here so it aligns with grid
        int index = GetIndex(x, y);
        uint val = GridData[index];
        bool noMoveBlocked = TestPointKey(val, noMoveKey);
        bool noMoveNoCursorBlocked = TestPointKey(val, noMoveNoCursorKey);
        return (noMoveBlocked || noMoveNoCursorBlocked);//(GridData[index] & 0x01) == 0x01;
    }

    public String FileName;
    public uint[] GridData;
    //public List<Point> BlockedPoints = new List<Point>();

    public List<string> EntryList;
    public bool SpecifiesCenter = false;
    public bool SpecifiesNoMove = false;
    public bool SpecifiesNoMoveNoCursor = false;

    public Point MapCenter;
    private Dictionary<string, List<Point>> TeamStartPoints = new Dictionary<string, List<Point>>();
}



