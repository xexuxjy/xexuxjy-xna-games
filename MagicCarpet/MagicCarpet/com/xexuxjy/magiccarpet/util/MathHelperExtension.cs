using System;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.util
{
    public static class MathHelperExtension
    {
        public static int Clamp(int low, int val,int high)
        {
            return val < low ? low : (val > high )? high : val;
        }

        public static bool CompareFloat(float val1, float val2)
        {
            return Math.Abs(val1 - val2) < Epsilon;
        }


        public static Point Add(this Point value1, Point value2)
        {
            return new Point(value1.X+value2.X,value1.Y+value2.Y);
        }

        public const float Epsilon = 0.0000000001f;
    }
}
