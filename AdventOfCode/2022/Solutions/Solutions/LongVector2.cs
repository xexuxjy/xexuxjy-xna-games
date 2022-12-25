using System.Collections;

public struct LongVector2
{
    public LongVector2(long x, long y)
    {
        X = x;
        Y = y;
    }

    public LongVector2(long x)
    {
        X = x;
        Y = x;
    }

    public LongVector2(LongVector2 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public LongVector2(ref LongVector2 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public static LongVector2 operator +(LongVector2 value1, LongVector2 value2)
    {
        LongVector2 vector;
        vector.X = value1.X + value2.X;
        vector.Y = value1.Y + value2.Y;
        return vector;
    }



    public static LongVector2 operator -(LongVector2 value1, LongVector2 value2)
    {
        LongVector2 vector;
        vector.X = value1.X - value2.X;
        vector.Y = value1.Y - value2.Y;
        return vector;
    }

    public long ManhattanDistance(LongVector2 v)
    {
        long distanceX = Math.Abs(X - v.X);
        long distanceY = Math.Abs(Y - v.Y);

        //long test = distanceX + distanceY;

        //long distanceX2 = Math.Abs(v.X - X);
        //long distanceY2 = Math.Abs(v.Y - Y);

        //long test2 = distanceX + distanceY;

        //if(test2 != test)
        //{
        //    int ibreak =0;
        //}


        return distanceX + distanceY;

    }


    public long X;
    public long Y;
}