using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gladius.util
{
    public static class QuaternionHelper
    {
        public static Quaternion LookRotation(Vector3 forward)
        {
            return LookRotation(forward, Vector3.Up);
        }

        public static Quaternion LookRotation(Vector3 forward, Vector3 up)
        {
            Vector3 right = Vector3.Cross(forward, up);
            Matrix m = Matrix.Identity;
            m.Forward = forward;
            m.Up = up;
            m.Right = right;
            return Quaternion.CreateFromRotationMatrix(m);
        }

        public static bool FuzzyEquals(Quaternion q1, Quaternion q2)
        {
            q1.Normalize();
            q2.Normalize();
            float diff = Math.Abs(1f - Quaternion.Dot(q1, q2));
            float closeEnough = 0.0001f;
            return diff < closeEnough;
        }

    }
}
