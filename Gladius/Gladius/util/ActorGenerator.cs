using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;
using GameStateManagement;

namespace Gladius.util
{
    public static class ActorGenerator
    {
        public static BaseActor GenerateActor(int level, ActorClass actorClass, GameScreen gameScreen)
        {
            // quick and dirty way of generating characters.
            int accuracy = 10 * level;
            int defense = 12 * level;
            int power = 8 * level;
            int cons = 10 * level;
            BaseActor baseActor = new BaseActor(gameScreen);
            baseActor.AttributeDictionary[xexuxjy.Gladius.util.GameObjectAttributeType.Accuracy] = new xexuxjy.Gladius.util.BoundedAttribute(accuracy);
            baseActor.AttributeDictionary[xexuxjy.Gladius.util.GameObjectAttributeType.Defense] = new xexuxjy.Gladius.util.BoundedAttribute(defense);
            baseActor.AttributeDictionary[xexuxjy.Gladius.util.GameObjectAttributeType.Power] = new xexuxjy.Gladius.util.BoundedAttribute(power);
            baseActor.AttributeDictionary[xexuxjy.Gladius.util.GameObjectAttributeType.Constitution] = new xexuxjy.Gladius.util.BoundedAttribute(cons);
            return baseActor;
        }


        public static void LoadActors(String filename, List<BaseActor> results, GameScreen gameScreen)
        {
            using (StreamReader sr = new StreamReader(TitleContainer.OpenStream(filename)))
            {
                String result = sr.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                XmlNodeList nodes = doc.SelectNodes("//Character");
                foreach (XmlNode node in nodes)
                {
                    BaseActor baseActor = new BaseActor(gameScreen);
                    baseActor.SetupCharacterData(node as XmlElement);
                    results.Add(baseActor);
                }
            }
        }




    }
}
