using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.util
{
    public static class GameUtil
    {
        public static bool InRange(GameObject obj1, GameObject obj2, float dist)
        {
            return (obj1.Position - obj2.Position).Length() <= dist;
        }

        public static Vector3 DirectionToTarget(GameObject source, GameObject target)
        {
            return Vector3.Normalize(target.Position - source.Position);
        }

        public static Vector3 DirectionToTarget(Vector3 source, GameObject target)
        {
            return Vector3.Normalize(target.Position - source);
        }

        public static Vector3 DirectionToTarget(GameObject source, Vector3 target)
        {
            return Vector3.Normalize(target - source.Position);
        }


    }
}
