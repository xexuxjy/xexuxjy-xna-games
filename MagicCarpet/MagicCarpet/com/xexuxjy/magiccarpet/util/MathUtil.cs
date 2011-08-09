using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xexuxjy.magiccarpet.util
{
    public static class MathUtil
    {
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
