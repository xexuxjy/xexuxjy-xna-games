using System;


public class IndexedMatrix : IEquatable<IndexedMatrix>
{
    private static IndexedMatrix _identity = new IndexedMatrix(1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f);
    public static IndexedMatrix Identity
    {
        get
        {
            return IndexedMatrix._identity;
        }
    }
    
    public IndexedMatrix()
    {
    }

    public IndexedMatrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33, float m41, float m42, float m43)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = 0.0f;

        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = 0.0f;

        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = 0.0f;

        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = 0.0f;

        _basis = new IndexedBasisMatrix(m11, m12, m13, m21, m22, m23, m31, m32, m33);
        _origin = new IndexedVector3(m41, m42, m43);

    }

    public IndexedMatrix(float m11, float m12, float m13,float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34,float m41, float m42, float m43,float m44)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;

        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;

        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;

        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;

        _basis = new IndexedBasisMatrix(m11, m12, m13, m21, m22, m23, m31, m32, m33);
        _origin = new IndexedVector3(m41, m42, m43);

    }



    public IndexedMatrix(IndexedBasisMatrix basis, IndexedVector3 origin)
    {
        _basis = basis;
        _origin = origin;
    }


    public static IndexedMatrix CreateLookAt(IndexedVector3 cameraPosition, IndexedVector3 cameraTarget, IndexedVector3 cameraUpVector)
    {
        IndexedVector3 vector3_1 = IndexedVector3.Normalize(cameraPosition - cameraTarget);
        IndexedVector3 vector3_2 = IndexedVector3.Normalize(IndexedVector3.Cross(cameraUpVector, vector3_1));
        IndexedVector3 vector1 = IndexedVector3.Cross(vector3_1, vector3_2);
        IndexedMatrix matrix = IndexedMatrix.Identity;
        //matrix._basis = new IndexedBasisMatrix(vector3_2.X, vector1.X, vector3_1.X, vector3_2.Y, vector1.Y, vector3_1.Y, vector3_2.Z, vector1.Z, vector3_1.Z).Transpose();
        matrix._basis = new IndexedBasisMatrix(ref vector3_2, ref vector1, ref vector3_1);

        matrix._origin = new IndexedVector3(-IndexedVector3.Dot(vector3_2, cameraPosition),
        -IndexedVector3.Dot(vector1, cameraPosition),
        -IndexedVector3.Dot(vector3_1, cameraPosition));
        return matrix;
    }

    public static IndexedMatrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
    {
        //    if ((double)fieldOfView <= 0.0 || (double)fieldOfView >= 3.14159274101257)
        //        throw new ArgumentOutOfRangeException("fieldOfView", string.Format((IFormatProvider)CultureInfo.CurrentCulture, FrameworkResources.OutRangeFieldOfView, new object[1]
        //{
        //  (object) "fieldOfView"
        //}));
        //    else if ((double)nearPlaneDistance <= 0.0)
        //        throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format((IFormatProvider)CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[1]
        //{
        //  (object) "nearPlaneDistance"
        //}));
        //    else if ((double)farPlaneDistance <= 0.0)
        //    {
        //        throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format((IFormatProvider)CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[1]
        //{
        //  (object) "farPlaneDistance"
        //}));
        //    }
        //    else
        {
            //if ((double)nearPlaneDistance >= (double)farPlaneDistance)
            //    throw new ArgumentOutOfRangeException("nearPlaneDistance", FrameworkResources.OppositePlanes);
            float num1 = 1f / (float)Math.Tan((double)fieldOfView * 0.5);
            float num2 = num1 / aspectRatio;
            IndexedMatrix matrix = IndexedMatrix.Identity;
            matrix._basis = new IndexedBasisMatrix(num2, 0, 0, 0, num1, 0, 0, 0, farPlaneDistance / (nearPlaneDistance - farPlaneDistance));
            matrix._origin = new IndexedVector3(0, 0, (float)((double)nearPlaneDistance * (double)farPlaneDistance / ((double)nearPlaneDistance - (double)farPlaneDistance)));

            return matrix;
        }
    }


    public static bool operator ==(IndexedMatrix matrix1, IndexedMatrix matrix2)
    {
        return matrix1._basis == matrix2._basis &&
                matrix1._origin == matrix2._origin;
    }

    public static bool operator !=(IndexedMatrix matrix1, IndexedMatrix matrix2)
    {
        return matrix1._basis != matrix2._basis ||
                matrix1._origin != matrix2._origin;
    }

    public static IndexedVector3 operator *(IndexedMatrix matrix1, IndexedVector3 v)
    {
        //return new IndexedVector3(matrix1._basis[0].Dot(ref v) + matrix1._origin.X, 
        //                           matrix1._basis[1].Dot(ref v) + matrix1._origin.Y,
        //                            matrix1._basis[2].Dot(ref v) + matrix1._origin.Z);
        return new IndexedVector3(matrix1._basis._el0.Dot(ref v) + matrix1._origin.X,
                                               matrix1._basis._el1.Dot(ref v) + matrix1._origin.Y,
                                                matrix1._basis._el2.Dot(ref v) + matrix1._origin.Z);
    }


    public static void Multiply(out IndexedVector3 vout, ref IndexedMatrix matrix1, ref IndexedVector3 vin)
    {
        vout = new IndexedVector3(matrix1._basis._el0.Dot(ref vin) + matrix1._origin.X,
                                               matrix1._basis._el1.Dot(ref vin) + matrix1._origin.Y,
                                                matrix1._basis._el2.Dot(ref vin) + matrix1._origin.Z);
    }



    //public static IndexedVector3 operator *(IndexedVector3 v,IndexedMatrix matrix1)
    //{
    //    return new IndexedVector3(matrix1._basis[0].Dot(ref v) + matrix1._origin.X,
    //                               matrix1._basis[1].Dot(ref v) + matrix1._origin.Y,
    //                                matrix1._basis[2].Dot(ref v) + matrix1._origin.Z);
    //}

    public static IndexedMatrix operator *(IndexedMatrix matrix1, IndexedMatrix matrix2)
    {
        IndexedMatrix IndexedMatrix = _identity ;
        IndexedMatrix._basis = matrix1._basis * matrix2._basis;
        IndexedMatrix._origin = matrix1 * matrix2._origin;

        return IndexedMatrix;
    }

    //public static IndexedMatrix operator *(IndexedMatrix matrix1, float scaleFactor)
    //{
    //    float num = scaleFactor;
    //    IndexedMatrix IndexedMatrix;
    //    IndexedMatrix._basis._Row0 = matrix1._Row0  * scaleFactor;
    //    IndexedMatrix._basis._Row1 = matrix1._Row1 * scaleFactor;
    //    IndexedMatrix._basis._Row3 = matrix1._Row3 * scaleFactor;
    //    IndexedMatrix._basis.Row3 = matrix1.Row3 * scaleFactor;
    //    return IndexedMatrix;
    //}

    //public static IndexedMatrix operator *(float scaleFactor, IndexedMatrix matrix1)
    //{
    //    IndexedMatrix IndexedMatrix;
    //    IndexedMatrix._basis._Row0 = matrix1._basis._Row0 * scaleFactor;
    //    IndexedMatrix._basis._Row1 = matrix1._basis._Row1 * scaleFactor;
    //    IndexedMatrix._basis._Row2 = matrix1._basis._Row2 * scaleFactor;
    //    IndexedMatrix._origin = matrix1._origin * scaleFactor;
    //    return IndexedMatrix;
    //}

    //public static IndexedMatrix operator /(IndexedMatrix matrix1, IndexedMatrix matrix2)
    //{
    //    IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
    //    IndexedMatrix._basis = matrix1._basis / matrix2._basis;
    //    IndexedMatrix._origin = matrix1._origin / matrix2._origin;
    //    return IndexedMatrix;
    //}

    public static IndexedMatrix operator /(IndexedMatrix matrix1, float divider)
    {
        float num = 1f / divider;
        IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
        IndexedMatrix._basis = matrix1._basis * num;
        IndexedMatrix._origin = matrix1._origin * num;
        return IndexedMatrix;
    }


    public static IndexedMatrix CreateTranslation(IndexedVector3 position)
    {
        IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
        IndexedMatrix._origin = position;
        return IndexedMatrix;
    }

    public static void CreateTranslation(ref IndexedVector3 position, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;
        result._origin = position;
    }

    public static IndexedMatrix CreateTranslation(float xPosition, float yPosition, float zPosition)
    {
        IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
        IndexedMatrix._origin = new IndexedVector3(xPosition, yPosition, zPosition);
        return IndexedMatrix;
    }

    public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;
        result._origin = new IndexedVector3(xPosition, yPosition, zPosition);
    }

    public static IndexedMatrix CreateScale(float x, float y, float z)
    {
        IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
        IndexedMatrix._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(x, y, z));
        return IndexedMatrix;
    }

    public static void CreateScale(float xScale, float yScale, float zScale, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;
        result._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(xScale, yScale, zScale));
    }

    public static IndexedMatrix CreateScale(IndexedVector3 scales)
    {
        IndexedMatrix result = IndexedMatrix.Identity;
        result._basis = IndexedBasisMatrix.CreateScale(scales);
        return result;
    }

    public static void CreateScale(ref IndexedVector3 scales, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;
        result._basis = IndexedBasisMatrix.CreateScale(scales);
    }

    public static IndexedMatrix CreateScale(float scale)
    {
        IndexedMatrix result = IndexedMatrix.Identity;
        result._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(scale));
        return result;
    }

    public static void CreateScale(float scale, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;
        result._basis = IndexedBasisMatrix.CreateScale(new IndexedVector3(scale));
    }

    public static IndexedMatrix CreateRotationX(float radians)
    {
        IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
        IndexedMatrix._basis = IndexedBasisMatrix.CreateRotationX(radians);
        IndexedMatrix._origin = new IndexedVector3(0, 0, 0);

        return IndexedMatrix;
    }

    public static void CreateRotationX(float radians, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;
        result._basis = IndexedBasisMatrix.CreateRotationX(radians);
        result._origin = new IndexedVector3(0, 0, 0);
    }

    public static IndexedMatrix CreateRotationY(float radians)
    {
        IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
        IndexedMatrix._basis = IndexedBasisMatrix.CreateRotationY(radians);
        IndexedMatrix._origin = new IndexedVector3(0, 0, 0);

        return IndexedMatrix;
    }

    public static void CreateRotationY(float radians, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;
        result._basis = IndexedBasisMatrix.CreateRotationY(radians);
        result._origin = new IndexedVector3(0, 0, 0);
    }

    public static IndexedMatrix CreateRotationZ(float radians)
    {
        IndexedMatrix IndexedMatrix = IndexedMatrix.Identity;
        IndexedMatrix._basis = IndexedBasisMatrix.CreateRotationZ(radians);
        IndexedMatrix._origin = new IndexedVector3(0, 0, 0);

        return IndexedMatrix;
    }

    public static void CreateRotationZ(float radians, out IndexedMatrix result)
    {
        result = IndexedMatrix.Identity;

        result._basis = IndexedBasisMatrix.CreateRotationZ(radians);
        result._origin = new IndexedVector3(0, 0, 0);
    }

    public bool Equals(IndexedMatrix other)
    {
        return _basis.Equals(other._basis) && _origin.Equals(other._origin);
    }

    public override bool Equals(object obj)
    {
        bool flag = false;
        if (obj is IndexedMatrix)
            flag = this.Equals((IndexedMatrix)obj);
        return flag;
    }

    public override int GetHashCode()
    {
        return this._basis.GetHashCode() + this._origin.GetHashCode();
    }

    public IndexedMatrix Inverse()
    {
        IndexedBasisMatrix inv = _basis.Transpose();
        return new IndexedMatrix(inv, inv * -_origin);
    }

    public IndexedVector3 InvXform(IndexedVector3 inVec)
    {
        IndexedVector3 v = inVec - _origin;
        return (_basis.Transpose() * v);
    }

    public IndexedVector3 InvXform(ref IndexedVector3 inVec)
    {
        IndexedVector3 v = inVec - _origin;
        return (_basis.Transpose() * v);
    }

    public IndexedMatrix InverseTimes(ref IndexedMatrix t)
    {
        IndexedVector3 v = new IndexedVector3(t._origin.X - _origin.X, t._origin.Y - _origin.Y, t._origin.Z - _origin.Z);
        IndexedVector3 v2 = new IndexedVector3();
        IndexedBasisMatrix.Multiply(ref v2, ref _basis, ref v);
        return new IndexedMatrix(_basis.TransposeTimes(ref t._basis),
                v * _basis);
    }

    public void SetRotation(IndexedQuaternion q)
    {
        _basis.SetRotation(ref q);
    }

    public void SetRotation(ref IndexedQuaternion q)
    {
        _basis.SetRotation(ref q);
    }


    public IndexedQuaternion GetRotation()
    {
        return _basis.GetRotation();
    }

    public static IndexedMatrix CreateFromQuaternion(IndexedQuaternion q)
    {
        IndexedMatrix i = new IndexedMatrix();
        i._basis.SetRotation(ref q);
        return i;
    }

    public static IndexedMatrix CreateFromQuaternion(ref IndexedQuaternion q)
    {
        IndexedMatrix i = new IndexedMatrix();
        i._basis.SetRotation(ref q);
        return i;
    }

    public IndexedVector3 Left
    {
        get { return _basis.Left; }
    }

    public IndexedVector3 Right
    {
        get { return _basis.Right; }
    }

    public IndexedVector3 Up
    {
        get
        {
            return _basis.Up;
        }
    }

    public IndexedVector3 Down
    {
        get
        {
            return _basis.Down;
        }
    }

    public IndexedVector3 Forward
    {
        get
        {
            return _basis.Forward;
        }
    }

    public IndexedVector3 Backward
    {
        get
        {
            return _basis.Backward;
        }
    }


    public float M11= 1.0f;
    public float M12 = 0.0f;
    public float M13 = 0.0f;
    public float M14 = 0.0f;

    public float M21 = 0.0f;
    public float M22 = 1.0f;
    public float M23 = 0.0f;
    public float M24 = 0.0f;

    public float M31 = 0.0f;
    public float M32 = 0.0f;
    public float M33 = 1.0f;
    public float M34 = 0.0f;

    public float M41 = 0.0f;
    public float M42 = 0.0f;
    public float M43 = 0.0f;
    public float M44 = 1.0f;

    public IndexedBasisMatrix _basis;
    public IndexedVector3 _origin;

}
