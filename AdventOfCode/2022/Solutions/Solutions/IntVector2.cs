using System.Collections;
using System.Diagnostics.CodeAnalysis;

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


    public static IntVector2 operator -(IntVector2 value1)
    {
        return new IntVector2(-value1.X, -value1.Y);
    }

    public static IntVector2 operator -(IntVector2 value1, IntVector2 value2)
    {
        IntVector2 vector;
        vector.X = value1.X - value2.X;
        vector.Y = value1.Y - value2.Y;
        return vector;
    }

    public static IntVector2 operator /(IntVector2 value1, int value)
    {
        IntVector2 vector;
        vector.X = value1.X / value;
        vector.Y = value1.Y / value;
        return vector;
    }

    public static IntVector2 operator *(IntVector2 value1, int value)
    {
        IntVector2 vector;
        vector.X = value1.X * value;
        vector.Y = value1.Y * value;
        return vector;
    }

    public static bool operator ==(IntVector2 value1, IntVector2 value2)
    {
        return value1.X == value2.X && value1.Y == value2.Y;
    }

    public static bool operator !=(IntVector2 value1, IntVector2 value2)
    {
        return value1.X != value2.X || value1.Y != value2.Y;
    }



    public int ManhattanDistance(IntVector2 v)
    {
        int distanceX = Math.Abs(X - v.X);
        int distanceY = Math.Abs(Y - v.Y);

        return distanceX + distanceY;

    }

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        return base.Equals(obj);
    }
    public override string ToString()
    {
        return "" + X + "," + Y;
    }

    public int X;
    public int Y;

    public static IntVector2 Left = new IntVector2(-1,0);
    public static IntVector2 Right = new IntVector2(1,0);
    public static IntVector2 Up = new IntVector2(0,-1);
    public static IntVector2 Down = new IntVector2(0,1);

    public static IntVector2[] Directions = new IntVector2[]{Left, Right, Up,Down };

}