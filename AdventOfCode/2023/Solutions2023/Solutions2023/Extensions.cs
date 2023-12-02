using System.Numerics;

public static class Extensions
{

    public static float ManhattanDistance(this Vector2 v,Vector2 v2)
    {
        float distanceX = Math.Abs(v.X - v2.X);
        float distanceY = Math.Abs(v.Y - v2.Y);

        return distanceX + distanceY;
    }

}