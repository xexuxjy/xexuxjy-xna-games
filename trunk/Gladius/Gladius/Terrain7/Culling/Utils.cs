// Sample taken from http://www.dustinhorne.com/page/XNA-Terrain-Tutorial-Table-of-Contents.aspx - many thanks

using System;
using Microsoft.Xna.Framework;

namespace Gladius.Terrain7.Culling
{
	public static class Utils
	{
		public static Vector3 GetDirectionVector(Vector3 point1, Vector3 point2)
		{
			return new Vector3(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
		}

		public static Vector3 LinePointFromKnownY(Vector3 point1, Vector3 point2, float y)
		{
			var direction = GetDirectionVector(point1, point2);

			//parameteric equation
			//Point1.Y - (direction.Y * t) = knownY
			//solve for T
			//so -->  Point1.Y - knownY = direction.Y * t
			//t = (Point1.Y - knownY) / direction.Y
			//
			//unknown X: Point1.X - (direction.X * t)
			//unknown Z: Point1.Z - (direction.Z * t)

			var t = (point1.Y - y) / direction.Y;
			var x = point1.X - (direction.X * t);
			var z = point1.Z - (direction.Z * t);

			return new Vector3(x, y, z);
		}

		public static Vector3 GetLineCenter(Vector3 point1, Vector3 point2)
		{
			var direction = GetDirectionVector(point1, point2);
			var y = point1.Y - (direction.Y * 0.5f);
			var x = point1.X - (direction.X * 0.5f);
			var z = point1.Z - (direction.Z * 0.5f);

			return new Vector3(x, y, z);
		}

		public static Vector2 GetEdgeNormal(Vector2 point1, Vector2 point2)
		{
			var edge = point1 - point2;
			edge.Normalize();

			return new Vector2(edge.Y, -edge.X);
		}

		public static float LineAngle(Vector2 v1, Vector2 v2)
		{
			return MathHelper.ToDegrees((float)Math.Abs(Math.Atan2(v2.Y - v1.Y, v2.X - v1.X)));
		}

	}

}
