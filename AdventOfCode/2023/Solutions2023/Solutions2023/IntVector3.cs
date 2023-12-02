using System.Collections;

public struct IntVector3
{
    public static readonly IntVector3 Zero = new IntVector3();
    
    public IntVector3(int x, int y,int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public IntVector3(int x)
    {
        X = x;
        Y = x;
        Z = x;
    }

    public IntVector3(IntVector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }

    public IntVector3(ref IntVector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }

    public static IntVector3 operator +(IntVector3 value1, IntVector3 value2)
    {
        IntVector3 vector;
        vector.X = value1.X + value2.X;
        vector.Y = value1.Y + value2.Y;
        vector.Z= value1.Z + value2.Z;
        return vector;
    }



    public static IntVector3 operator -(IntVector3 value1, IntVector3 value2)
    {
        IntVector3 vector;
        vector.X = value1.X - value2.X;
        vector.Y = value1.Y - value2.Y;
        vector.Z= value1.Z - value2.Z;
        return vector;
    }

    public static IntVector3 operator /(IntVector3 value1, int value)
    {
        IntVector3 vector;
        vector.X = value1.X /value;
        vector.Y = value1.Y /value;
        vector.Z = value1.Z /value;
        return vector;
    }

    public static IntVector3 operator *(IntVector3 value1, int value)
    {
        IntVector3 vector;
        vector.X = value1.X *value;
        vector.Y = value1.Y *value;
        vector.Z = value1.Z *value;
        return vector;
    }

    public static bool operator <(IntVector3 value1, IntVector3 value2)
    {
        return value1.X < value2.X || value1.Y < value2.Y || value1.Z < value2.Z;
    }

    public static bool operator <=(IntVector3 value1, IntVector3 value2)
    {
        return value1.X <= value2.X || value1.Y <= value2.Y || value1.Z <= value2.Z;
    }


    public static bool operator >(IntVector3 value1, IntVector3 value2)
    {
        return value1.X > value2.X || value1.Y > value2.Y || value1.Z > value2.Z;
    }

    public static bool operator >=(IntVector3 value1, IntVector3 value2)
    {
        return value1.X >= value2.X || value1.Y >= value2.Y || value1.Z >= value2.Z;
    }


    public int ManhattanDistance(IntVector3 v)
    {
        int distanceX = Math.Abs(X - v.X);
        int distanceY = Math.Abs(Y - v.Y);
        int distanceZ = Math.Abs(Z - v.Z);
        return distanceX + distanceY + distanceZ;

    }

    public override string ToString()
    {
        return "("+X+","+Y+","+Z+")";
    }

    public int X;
    public int Y;
    public int Z;
}