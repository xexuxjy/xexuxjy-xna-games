using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;


    public static class ArenaLoader
    {
        public static Vector3 ReadVector3(String path, XmlDocument doc)
        {
            Vector3 result = new Vector3();
            result.x = float.Parse(doc.SelectSingleNode(path+"/@x").InnerText);
            result.y = float.Parse(doc.SelectSingleNode(path + "/@y").InnerText);
            result.z = float.Parse(doc.SelectSingleNode(path + "/@z").InnerText);
            return result;
        }
        



        public static void SetupArena(String arenaDataName,Arena arena)
        {
            //Arena arena = new Arena();
            List<String> lines = new List<String>();

            TextAsset textAsset = (TextAsset)Resources.Load(arenaDataName);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(textAsset.text);
            arena.PrefabName = doc.SelectSingleNode("Arena/Prefab").InnerText;
            arena.TextureName = doc.SelectSingleNode("Arena/Texture").InnerText;

            arena.ModelPosition = ReadVector3("Arena/Position",doc);
            arena.ModelScale = ReadVector3("Arena/Scale", doc);
            arena.ModelRotation = ReadVector3("Arena/Rotation", doc);
            
            arena.SkyBoxMaterialName = doc.SelectSingleNode("Arena/SkyBoxMaterial").InnerText;

            XmlNodeList nodes = doc.SelectNodes("Arena/Layout//row");
            foreach (XmlNode node in nodes)
            {
                lines.Add(node.InnerText);
            }

            arena.Width = lines[0].Length;
            arena.Breadth = lines.Count;

            arena.ArenaGrid = new SquareType[arena.Width, arena.Breadth];

            for (int i = 0; i < arena.Width; ++i)
            {
                for (int j = 0; j < arena.Breadth; ++j)
                {
                    if (lines[j][i] == '#')
                    {
                        arena.ArenaGrid[j, i] = SquareType.Wall;
                    }
                    else if (lines[j][i] == 'P')
                    {
                        arena.ArenaGrid[j, i] = SquareType.Pillar;
                    }
                    else if (lines[j][i] == '1')
                    {
                        arena.ArenaGrid[j, i] = SquareType.Level1;
                    }
                    else if (lines[j][i] == '2')
                    {
                        arena.ArenaGrid[j, i] = SquareType.Level2;
                    }
                    else if (lines[j][i] == '3')
                    {
                        arena.ArenaGrid[j, i] = SquareType.Level3;
                    }
                }
            }


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

    }

