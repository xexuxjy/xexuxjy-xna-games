using System.Collections;

public struct IndexedVector2
{
    public IndexedVector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public IndexedVector2(float x)
    {
        X = x;
        Y = x;
    }

    public IndexedVector2(IndexedVector2 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public IndexedVector2(ref IndexedVector2 v)
    {
        X = v.X;
        Y = v.Y;
    }


    public override string ToString()
    {
        return "X : " + X + " Y " + Y ;
    }


    public float X;
    public float Y;
}