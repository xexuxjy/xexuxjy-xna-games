#define XNA
using System;
using System.Diagnostics;
public struct IndexedQuaternion
{

    public IndexedQuaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public IndexedQuaternion(IndexedVector4 iv4)
    {
        X = iv4.X;
        Y = iv4.Y;
        Z = iv4.Z;
        W = iv4.W;
    }


    public float this[int i]
    {
        get
        {
            switch (i)
            {
                case (0): return X;
                case (1): return Y;
                case (2): return Z;
                case (3): return W;
                default:
                    {
                        Debug.Assert(false);
                        return 0.0f;
                    }
            }
        }
        set
        {
            switch (i)
            {
                case (0): X = value; break;
                case (1): Y = value; break;
                case (2): Z = value; break;
                case (3): W = value; break;
                default:
                    {
                        Debug.Assert(false);
                        break;
                    }
            }
        }
    }


    public IndexedQuaternion(IndexedVector3 axis, float angle)
    {
        //SetRotation(ref axis, angle);

        float d = axis.Length();
        Debug.Assert(d != 0.0f);
        float halfAngle = angle * 0.5f;
        float s = (float)Math.Sin(halfAngle) / d;
        X = axis.X * s;
        Y = axis.Y * s;
        Z = axis.Z * s;
        W = (float)Math.Cos(halfAngle);
    }

    public void SetRotation(ref IndexedVector3 axis, float angle)
    {
        float d = axis.Length();
        Debug.Assert(d != 0.0f);
        float halfAngle = angle * 0.5f;
        float s = (float)Math.Sin(halfAngle) / d;
        X = axis.X * s;
        Y = axis.Y * s;
        Z = axis.Z * s;
        W = (float)Math.Cos(halfAngle);
    }

    public void SetValue(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }


    public static IndexedQuaternion operator +(IndexedQuaternion quaternion1, IndexedQuaternion quaternion2)
    {
        IndexedQuaternion quaternion;
        quaternion.X = quaternion1.X + quaternion2.X;
        quaternion.Y = quaternion1.Y + quaternion2.Y;
        quaternion.Z = quaternion1.Z + quaternion2.Z;
        quaternion.W = quaternion1.W + quaternion2.W;
        return quaternion;
    }

    /// <summary>
    /// Subtracts a quaternion from another quaternion.
    /// </summary>
    /// <param name="quaternion1">Source quaternion.</param><param name="quaternion2">Source quaternion.</param>
    public static IndexedQuaternion operator -(IndexedQuaternion quaternion1, IndexedQuaternion quaternion2)
    {
        IndexedQuaternion quaternion;
        quaternion.X = quaternion1.X - quaternion2.X;
        quaternion.Y = quaternion1.Y - quaternion2.Y;
        quaternion.Z = quaternion1.Z - quaternion2.Z;
        quaternion.W = quaternion1.W - quaternion2.W;
        return quaternion;
    }

    public static IndexedQuaternion operator *(IndexedQuaternion q1, IndexedQuaternion q2)
    {
        return new IndexedQuaternion(q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,
            q1.W * q2.Y + q1.Y * q2.W + q1.Z * q2.X - q1.X * q2.Z,
            q1.W * q2.Z + q1.Z * q2.W + q1.X * q2.Y - q1.Y * q2.X,
            q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z);
    }


    public static IndexedQuaternion operator *(IndexedQuaternion q1, IndexedVector3 v1)
    {
        return new IndexedQuaternion(q1.W * v1.X + q1.Y * v1.Z - q1.Z * v1.Y,
            q1.W * v1.Y + q1.Z * v1.X - q1.X * v1.Z,
            q1.W * v1.Z + q1.X * v1.Y - q1.Y * v1.X,
            -q1.X * v1.X - q1.Y * v1.Y - q1.Z * v1.Z);
    }


    public static IndexedQuaternion operator *(IndexedVector3 v1, IndexedQuaternion q1)
    {
        return new IndexedQuaternion(v1.X * q1.W + v1.Y * q1.Z - v1.Z * q1.Y,
            v1.Y * q1.W + v1.Z * q1.X - v1.X * q1.Z,
            v1.Z * q1.W + v1.X * q1.Y - v1.Y * q1.X,
            -v1.X * q1.X - v1.Y * q1.Y - v1.Z * q1.Z);
    }


    public static IndexedQuaternion operator -(IndexedQuaternion value)
    {
        IndexedQuaternion q;
        q.X = -value.X;
        q.Y = -value.Y;
        q.Z = -value.Z;
        q.W = -value.W;
        return q;
    }

    /// <summary>
    /// Calculates the length squared of a Quaternion.
    /// </summary>
    public float LengthSquared()
    {
        return (float)(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
    }

    /// <summary>
    /// Calculates the length of a Quaternion.
    /// </summary>
    public float Length()
    {
        return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
    }

    /// <summary>
    /// Divides each component of the quaternion by the length of the quaternion.
    /// </summary>
    public void Normalize()
    {
        float num = 1f / (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
        this.X *= num;
        this.Y *= num;
        this.Z *= num;
        this.W *= num;
    }

    public static IndexedQuaternion Inverse(IndexedQuaternion q)
    {
        return new IndexedQuaternion(-q.X, -q.Y, -q.Z, q.W);
    }

    public IndexedQuaternion Inverse()
    {
        return new IndexedQuaternion(-X, -Y, -Z, W);
    }


    public float Dot(IndexedQuaternion q)
    {
        return X * q.X + Y * q.Y + Z * q.Z + W * q.W;
    }

    public static float Dot(IndexedQuaternion q, IndexedQuaternion q2)
    {
        return q.X * q2.X + q.Y * q2.Y + q.Z * q2.Z + q.W * q2.W;
    }

    public static bool operator ==(IndexedQuaternion value1, IndexedQuaternion value2)
    {
        if (value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z)
            return value1.W == value2.W;
        else
            return false;
    }

    public static bool operator !=(IndexedQuaternion value1, IndexedQuaternion value2)
    {
        if (value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z)
            return value1.W != value2.W;
        else
            return true;
    }

    public IndexedVector3 QuatRotate(IndexedQuaternion rotation, IndexedVector3 v)
    {
        IndexedQuaternion q = rotation * v;
        q *= rotation.Inverse();
        return new IndexedVector3(q.X, q.Y, q.Z);
    }

    public static IndexedQuaternion Lerp(IndexedQuaternion quaternion1, IndexedQuaternion quaternion2, float amount)
    {
        IndexedQuaternion result;
        float f2 = 1.0F - amount;
        if (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y) + (quaternion1.Z * quaternion2.Z) + (quaternion1.W * quaternion2.W)) >= 0.0F)
        {
            result.X = (f2 * quaternion1.X) + (amount * quaternion2.X);
            result.Y = (f2 * quaternion1.Y) + (amount * quaternion2.Y);
            result.Z = (f2 * quaternion1.Z) + (amount * quaternion2.Z);
            result.W = (f2 * quaternion1.W) + (amount * quaternion2.W);
        }
        else
        {
            result.X = (f2 * quaternion1.X) - (amount * quaternion2.X);
            result.Y = (f2 * quaternion1.Y) - (amount * quaternion2.Y);
            result.Z = (f2 * quaternion1.Z) - (amount * quaternion2.Z);
            result.W = (f2 * quaternion1.W) - (amount * quaternion2.W);
        }
        float f4 = (result.X * result.X) + (result.Y * result.Y) + (result.Z * result.Z) + (result.W * result.W);
        float f3 = 1.0F / (float)System.Math.Sqrt((double)f4);
        result.X *= f3;
        result.Y *= f3;
        result.Z *= f3;
        result.W *= f3;
        return result;
    }

    public static IndexedQuaternion operator *(IndexedQuaternion quaternion1, float scaleFactor)
    {
        quaternion1.X *= scaleFactor;
        quaternion1.Y *= scaleFactor;
        quaternion1.Z *= scaleFactor;
        quaternion1.W *= scaleFactor;
        return quaternion1;
    }


    public override string ToString()
    {
        return "X : " + X + " Y " + Y + " Z " + Z + "W " + W;
    }


    public float X;
    public float Y;
    public float Z;
    public float W;

    public static IndexedQuaternion Identity
    {
        get { return _identity; }
    }

    private static IndexedQuaternion _identity = new IndexedQuaternion(0, 0, 0, 1);
}
