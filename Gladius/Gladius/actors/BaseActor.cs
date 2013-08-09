using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using Microsoft.Xna.Framework;

namespace Gladius.actors
{
    public class BaseActor
    {
        public String Name
        {
            get;
            set;
        }

        public String DebugName
        {
            get;
            set;
        }

        public float GetAttributeValue(GameObjectAttributeType attributeType)
        {
            return m_attributeDictionary[attributeType].CurrentValue;
        }

        public void SetAttributeValue(GameObjectAttributeType attributeType,float val)
        {
            m_attributeDictionary[attributeType].CurrentValue = val;
        }


        public Point CurrentPoint
        {
            get;
            set;
        }


        public Vector3 Position
        {
            get;
            set;
        }

        public Matrix World
        {
            get;
            set;
        }

        public BoundingBox BoundingBox
        {
            get;
            set;
        }

        public String Team
        {
            get;
            set;
        }

        public ActorClass ActorClass
        {
            get;
            set;
        }



        public Dictionary<GameObjectAttributeType,BoundedAttribute> m_attributeDictionary;
    }

    public enum ActorClass
    {
        Light,
        Medium,
        Heavy
    }

}
