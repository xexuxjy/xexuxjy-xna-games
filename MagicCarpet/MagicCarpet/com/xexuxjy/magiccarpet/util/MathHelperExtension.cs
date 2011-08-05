using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        public const float Epsilon = 0.0000000001f;
    }
}
