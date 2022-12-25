using System.Collections;

public struct IntVector2
{
    public IntVector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public IntVector2(int x)
    {
        X = x;
        Y = x;
    }

    public IntVector2(IntVector2 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public IntVector2(ref IntVector2 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public static IntVector2 operator +(IntVector2 value1, IntVector2 value2)
    {
        IntVector2 vector;
        vector.X = value1.X + value2.X;
        vector.Y = value1.Y + value2.Y;
        return vector;
    }



    public static IntVector2 operator -(IntVector2 value1, IntVector2 value2)
    {
        IntVector2 vector;
        vector.X = value1.X - value2.X;
        vector.Y = value1.Y - value2.Y;
        return vector;
    }

    public int ManhattanDistance(IntVector2 v)
    {
        int distanceX = Math.Abs(X - v.X);
        int distanceY = Math.Abs(Y - v.Y);

        return distanceX + distanceY;

    }


    public int X;
    public int Y;
}