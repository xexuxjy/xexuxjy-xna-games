using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.util
{
    public static class MathUtil
    {
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static Vector3 Vector3InverseLerp(Vector3 start, Vector3 point, Vector3 end)
        {
            float x = MathHelper.Clamp(point.X,start.X,end.X);
            float y = MathHelper.Clamp(point.Y,start.Y,end.Y);
            float z = MathHelper.Clamp(point.Z,start.Z,end.Z);

            x = (x-start.X) /(end.X-start.X);
            y = (y-start.Y) /(end.Y-start.Y);
            y = (z-start.Z) /(end.Z-start.Z);

            return new Vector3(x,y,z);
        }

    }
}
