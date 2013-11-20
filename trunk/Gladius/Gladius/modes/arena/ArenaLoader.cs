using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Xml;
using Gladius.gamestatemanagement.screens;
using Gladius.util;
using Gladius.renderer;

namespace Gladius.modes.arena
{
    public static class ArenaLoader
    {
        public static Arena BuildArena(ArenaScreen arenaScreen,String arenaDataName,ThreadSafeContentManager contentManager)
        {
            Arena arena = new Arena(arenaScreen);
            List<String> lines = new List<String>();
            using (StreamReader sr = new StreamReader(TitleContainer.OpenStream(arenaDataName)))
            {
                String result = sr.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);

                arena.ModelName = doc.SelectSingleNode("Arena/Model").Value;
                arena.TextureName = doc.SelectSingleNode("Arena/Texture").Value;


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

                // fixme man - arena model, texture, skybox, and static models specified in file.

                // models.
                XmlNodeList modelNodes = doc.SelectNodes("Arena/Models//Model");
                foreach (XmlNode node in modelNodes)
                {
                    String modelName = node.Attributes["meshName"].Value;
                    String textureName = node.Attributes["textureName"].Value;

                    ModelData modelData = new ModelData(modelName,textureName,contentManager);

                    XmlNodeList instanceNodes = node.SelectNodes("//ModelInstance");
                    foreach(XmlNode instanceNode in instanceNodes)
                    {
                        XmlElement element = instanceNode as XmlElement;
                        Vector3 position = GraphicsHelper.StringToVector3(element.Attributes["position"].Value);
                        Vector3 rotation = Vector3.Zero;
                        if (element.HasAttribute("rotation"))
                        {
                            rotation = GraphicsHelper.StringToVector3(element.Attributes["rotation"].Value);
                        }

                        Vector3 scale = Vector3.One;
                        if (element.HasAttribute("scale"))
                        {
                            rotation = GraphicsHelper.StringToVector3(element.Attributes["scale"].Value);
                        }

                        modelData.AddInstance(position, rotation, scale);
                    }
                    arena.ModelData.Add(modelData);
                }


            }
            return arena;
        }


    }
}
