using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.util
{
    public static class GameUtil
    {
        public static bool InRange(GameObject obj1, GameObject obj2, float dist)
        {
            //return (obj1.Position - obj2.Position).Length() <= dist;
            // can't do a simple point distance check so do it based on bounding boxes?
            BoundingBox obj1BB = obj1.BoundingBox;
            BoundingBox obj2BB = obj2.BoundingBox;

            // grow one of the boxes by dist and check for intersection?
            obj2BB.Min -= new Vector3(dist);
            obj2BB.Max += new Vector3(dist);

            return obj2BB.Contains(obj1BB) != ContainmentType.Disjoint;
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


        public static Matrix RebaseMatrixOnForward(Vector3 forward)
        {
            //Matrix m = Matrix.Identity;
            //Vector3 right = Vector3.Cross(forward, Vector3.Up);
            //Vector3 up = Vector3.Cross(right,forward);
            //m.Forward = forward;
            //m.Up = up;
            //m.Right = right;
            //return m;
            return Matrix.CreateWorld(Vector3.Zero, forward, Vector3.Up);
        }

    }
}
