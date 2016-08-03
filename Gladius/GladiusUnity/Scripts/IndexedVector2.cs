using UnityEngine;
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

    // User-defined conversion from IndexedVector3 to Vector3
    public static implicit operator UnityEngine.Vector2(IndexedVector2 v)
    {
        return new UnityEngine.Vector2(v.X, v.Y);
    }

    // User-defined conversion from IndexedVector3 to Vector3
    public static implicit operator IndexedVector2(UnityEngine.Vector2 v)
    {
        return new IndexedVector2(v.x, v.y);
    }



    public float X;
    public float Y;
}