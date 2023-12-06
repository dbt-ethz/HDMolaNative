// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.InteropServices;
using scm = System.ComponentModel;

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace Mola
{
    // A standard 4x4 transformation matrix.

    public partial struct Matrix4x4 : IEquatable<Matrix4x4>, IFormattable
    {
        // memory layout:
        //
        //                row no (=vertical)
        //               |  0   1   2   3
        //            ---+----------------
        //            0  | m00 m10 m20 m30
        // column no  1  | m01 m11 m21 m31
        // (=horiz)   2  | m02 m12 m22 m32
        //            3  | m03 m13 m23 m33

        ///*undocumented*
        public float m00;
        ///*undocumented*
        public float m10;
        ///*undocumented*
        public float m20;
        ///*undocumented*
        public float m30;

        ///*undocumented*
        public float m01;
        ///*undocumented*
        public float m11;
        ///*undocumented*
        public float m21;
        ///*undocumented*
        public float m31;

        ///*undocumented*
        public float m02;
        ///*undocumented*
        public float m12;
        ///*undocumented*
        public float m22;
        ///*undocumented*
        public float m32;

        ///*undocumented*
        public float m03;
        ///*undocumented*
        public float m13;
        ///*undocumented*
        public float m23;
        ///*undocumented*
        public float m33;

        public Matrix4x4(Vec4 column0, Vec4 column1, Vec4 column2, Vec4 column3)
        {
            this.m00 = column0.x; this.m01 = column1.x; this.m02 = column2.x; this.m03 = column3.x;
            this.m10 = column0.y; this.m11 = column1.y; this.m12 = column2.y; this.m13 = column3.y;
            this.m20 = column0.z; this.m21 = column1.z; this.m22 = column2.z; this.m23 = column3.z;
            this.m30 = column0.w; this.m31 = column1.w; this.m32 = column2.w; this.m33 = column3.w;
        }

        // Access element at [row, column].
        public float this[int row, int column]
        {
            get
            {
                return this[row + column * 4];
            }

            set
            {
                this[row + column * 4] = value;
            }
        }

        // Access element at sequential index (0..15 inclusive).
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m00;
                    case 1: return m10;
                    case 2: return m20;
                    case 3: return m30;
                    case 4: return m01;
                    case 5: return m11;
                    case 6: return m21;
                    case 7: return m31;
                    case 8: return m02;
                    case 9: return m12;
                    case 10: return m22;
                    case 11: return m32;
                    case 12: return m03;
                    case 13: return m13;
                    case 14: return m23;
                    case 15: return m33;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: m00 = value; break;
                    case 1: m10 = value; break;
                    case 2: m20 = value; break;
                    case 3: m30 = value; break;
                    case 4: m01 = value; break;
                    case 5: m11 = value; break;
                    case 6: m21 = value; break;
                    case 7: m31 = value; break;
                    case 8: m02 = value; break;
                    case 9: m12 = value; break;
                    case 10: m22 = value; break;
                    case 11: m32 = value; break;
                    case 12: m03 = value; break;
                    case 13: m13 = value; break;
                    case 14: m23 = value; break;
                    case 15: m33 = value; break;

                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        // used to allow Matrix4x4s to be used as keys in hash tables
        public override int GetHashCode()
        {
            return GetColumn(0).GetHashCode() ^ (GetColumn(1).GetHashCode() << 2) ^ (GetColumn(2).GetHashCode() >> 2) ^ (GetColumn(3).GetHashCode() >> 1);
        }

        // also required for being able to use Matrix4x4s as keys in hash tables
        public override bool Equals(object other)
        {
            if (!(other is Matrix4x4)) return false;

            return Equals((Matrix4x4)other);
        }

        public bool Equals(Matrix4x4 other)
        {
            return GetColumn(0).Equals(other.GetColumn(0))
                && GetColumn(1).Equals(other.GetColumn(1))
                && GetColumn(2).Equals(other.GetColumn(2))
                && GetColumn(3).Equals(other.GetColumn(3));
        }

        // Multiplies two matrices.
        public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            Matrix4x4 res;
            res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

            res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

            res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

            res.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            res.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            res.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            res.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;

            return res;
        }

        // Transforms a [[Vec4]] by a matrix.
        public static Vec4 operator *(Matrix4x4 lhs, Vec4 vector)
        {
            Vec4 res;
            res.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
            res.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
            res.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
            res.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;
            return res;
        }

        //*undoc*
        public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            // Returns false in the presence of NaN values.
            return lhs.GetColumn(0) == rhs.GetColumn(0)
                && lhs.GetColumn(1) == rhs.GetColumn(1)
                && lhs.GetColumn(2) == rhs.GetColumn(2)
                && lhs.GetColumn(3) == rhs.GetColumn(3);
        }

        //*undoc*
        public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // Get a column of the matrix.
        public Vec4 GetColumn(int index)
        {
            switch (index)
            {
                case 0: return new Vec4(m00, m10, m20, m30);
                case 1: return new Vec4(m01, m11, m21, m31);
                case 2: return new Vec4(m02, m12, m22, m32);
                case 3: return new Vec4(m03, m13, m23, m33);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        // Returns a row of the matrix.
        public Vec4 GetRow(int index)
        {
            switch (index)
            {
                case 0: return new Vec4(m00, m01, m02, m03);
                case 1: return new Vec4(m10, m11, m12, m13);
                case 2: return new Vec4(m20, m21, m22, m23);
                case 3: return new Vec4(m30, m31, m32, m33);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        public Vec3 GetPosition()
        {
            return new Vec3(m03, m13, m23);
        }

        // Sets a column of the matrix.
        public void SetColumn(int index, Vec4 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
            this[3, index] = column.w;
        }

        // Sets a row of the matrix.
        public void SetRow(int index, Vec4 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
        }

        // Transforms a position by this matrix, with a perspective divide. (generic)
        public Vec3 MultiplyPoint(Vec3 point)
        {
            Vec3 res;
            float w;
            res.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
            res.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
            res.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
            w = this.m30 * point.x + this.m31 * point.y + this.m32 * point.z + this.m33;


            w = 1F / w;
            res.x *= w;
            res.y *= w;
            res.z *= w;
            return res;
        }

        // Transforms a position by this matrix, without a perspective divide. (fast)
        public Vec3 MultiplyPoint3x4(Vec3 point)
        {
            Vec3 res;
            res.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
            res.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
            res.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
            return res;
        }

        // Transforms a direction by this matrix.
        public Vec3 MultiplyVector(Vec3 vector)
        {
            Vec3 res;
            res.x = this.m00 * vector.x + this.m01 * vector.y + this.m02 * vector.z;
            res.y = this.m10 * vector.x + this.m11 * vector.y + this.m12 * vector.z;
            res.z = this.m20 * vector.x + this.m21 * vector.y + this.m22 * vector.z;
            return res;
        }

        // Transforms a plane by this matrix.
        public Plane TransformPlane(Plane plane)
        {
            var ittrans = this.inverse();

            float x = plane.normal.x, y = plane.normal.y, z = plane.normal.z, w = plane.distance;
            // note: a transpose is part of this transformation
            var a = ittrans.m00 * x + ittrans.m10 * y + ittrans.m20 * z + ittrans.m30 * w;
            var b = ittrans.m01 * x + ittrans.m11 * y + ittrans.m21 * z + ittrans.m31 * w;
            var c = ittrans.m02 * x + ittrans.m12 * y + ittrans.m22 * z + ittrans.m32 * w;
            var d = ittrans.m03 * x + ittrans.m13 * y + ittrans.m23 * z + ittrans.m33 * w;

            return new Plane(new Vec3(a, b, c), d);
        }

        // Creates a scaling matrix.
        public static Matrix4x4 Scale(Vec3 vector)
        {
            Matrix4x4 m;
            m.m00 = vector.x;
            m.m01 = 0F;
            m.m02 = 0F;
            m.m03 = 0F;

            m.m10 = 0F;
            m.m11 = vector.y;
            m.m12 = 0F;
            m.m13 = 0F;

            m.m20 = 0F;
            m.m21 = 0F;
            m.m22 = vector.z;
            m.m23 = 0F;

            m.m30 = 0F;
            m.m31 = 0F;
            m.m32 = 0F;
            m.m33 = 1F;
            return m;
        }

        // Creates a translation matrix.
        public static Matrix4x4 Translate(Vec3 vector)
        {
            Matrix4x4 m;
            m.m00 = 1F;
            m.m01 = 0F;
            m.m02 = 0F;
            m.m03 = vector.x;

            m.m10 = 0F;
            m.m11 = 1F;
            m.m12 = 0F;
            m.m13 = vector.y;

            m.m20 = 0F;
            m.m21 = 0F;
            m.m22 = 1F;
            m.m23 = vector.z;

            m.m30 = 0F;
            m.m31 = 0F;
            m.m32 = 0F;
            m.m33 = 1F;
            return m;
        }

        // source chatgpt
        public static Matrix4x4 FromFrame(Vec3 source, Vec3 target, Vec3 up)
        {
            Vec3 forward = (target - source).normalized;
            Vec3 right = Vec3.Cross(up, forward).normalized;
            Vec3 newUp = Vec3.Cross(forward, right);
            Matrix4x4 result = new Matrix4x4();
            result.SetColumn(0, new Vec4(right.x, right.y, right.z, 0));
            result.SetColumn(1, new Vec4(newUp.x, newUp.y, newUp.z, 0));
            result.SetColumn(2, new Vec4(forward.x, forward.y, forward.z, 0));
            result.SetColumn(3, new Vec4(source.x, source.y, source.z, 1));
            return result;
        }
        public static Matrix4x4 LookAt2(Vec3 pos, Vec3 target, Vec3 up)
        {

            Vec3 forward = (target - pos).Normalize();
            Vec3 right = forward.Cross(up).Normalize();
            Vec3 newUp = right.Cross(forward).Normalize();

            Matrix4x4 viewMatrix = new Matrix4x4();
            viewMatrix[0, 0] = right[0];
            viewMatrix[1, 0] = right[1];
            viewMatrix[2, 0] = right[2];
            viewMatrix[0, 1] = newUp[0];
            viewMatrix[1, 1] = newUp[1];
            viewMatrix[2, 1] = newUp[2];
            viewMatrix[0, 2] = -forward[0];
            viewMatrix[1, 2] = -forward[1];
            viewMatrix[2, 2] = -forward[2];
            viewMatrix[3, 0] = -right.Dot(pos);
            viewMatrix[3, 1] = -newUp.Dot(pos);
            viewMatrix[3, 2] = forward.Dot(pos);

            viewMatrix[3, 3] =1;

            return viewMatrix;

        }
        public static Matrix4x4 LookAt3(Vec3 pos,Vec3 target,Vec3 up)
        {

            up.Normalize();

            Vec3 zAxis = Vec3.Normalize(target - pos);
            Vec3 xAxis = Vec3.Normalize(Vec3.Cross( zAxis,up));
            Vec3 yAxis = Vec3.Cross( xAxis, zAxis);

            /*Vec3 zAxis = (target-pos).Normalize();
            Vec3 xAxis = zAxis.Cross(up).Normalize();
            Vec3 yAxis = xAxis.Cross(zAxis);*/

            Matrix4x4 viewMatrix = new Matrix4x4();
            
            viewMatrix.m00 = xAxis.x;
            viewMatrix.m10 = yAxis.x;
            viewMatrix.m20 = zAxis.x;
            viewMatrix.m30 = 0;

            viewMatrix.m01 = xAxis.y;
            viewMatrix.m11 = yAxis.y;
            viewMatrix.m21 = zAxis.y;
            viewMatrix.m31 = 0;

            viewMatrix.m02 = xAxis.z;
            viewMatrix.m12 = yAxis.z;
            viewMatrix.m22 = zAxis.z;
            viewMatrix.m32 = 0;

            viewMatrix.m03 = -1 * xAxis.Dot(pos);
            viewMatrix.m13 = -1 * yAxis.Dot(pos);
            viewMatrix.m23 = -1 * zAxis.Dot(pos);
            viewMatrix.m33 = 1;


            /*viewMatrix.m00 = xAxis.x;
            viewMatrix.m01 = yAxis.x;
            viewMatrix.m02 = zAxis.x;
            viewMatrix.m03 = 0;

            viewMatrix.m10 = xAxis.y;
            viewMatrix.m11 = yAxis.y;
            viewMatrix.m12 = zAxis.y;
            viewMatrix.m13 = 0;

            viewMatrix.m20 = xAxis.z;
            viewMatrix.m21 = yAxis.z;
            viewMatrix.m22 = zAxis.z;
            viewMatrix.m23 = 0;

            viewMatrix.m30 = -1 * xAxis.Dot(pos);
            viewMatrix.m31 = -1 * yAxis.Dot(pos);
            viewMatrix.m32 = -1 * zAxis.Dot(pos);
            viewMatrix.m33 = 1;*/

            return viewMatrix;
            
        }
        // Creates a rotation matrix. Note: Assumes unit quaternion
        public static Matrix4x4 Rotate(Quaternion q)
        {
            // Precalculate coordinate products
            float x = q.x * 2.0F;
            float y = q.y * 2.0F;
            float z = q.z * 2.0F;
            float xx = q.x * x;
            float yy = q.y * y;
            float zz = q.z * z;
            float xy = q.x * y;
            float xz = q.x * z;
            float yz = q.y * z;
            float wx = q.w * x;
            float wy = q.w * y;
            float wz = q.w * z;

            // Calculate 3x3 matrix from orthonormal basis
            Matrix4x4 m;
            m.m00 = 1.0f - (yy + zz); m.m10 = xy + wz; m.m20 = xz - wy; m.m30 = 0.0F;
            m.m01 = xy - wz; m.m11 = 1.0f - (xx + zz); m.m21 = yz + wx; m.m31 = 0.0F;
            m.m02 = xz + wy; m.m12 = yz - wx; m.m22 = 1.0f - (xx + yy); m.m32 = 0.0F;
            m.m03 = 0.0F; m.m13 = 0.0F; m.m23 = 0.0F; m.m33 = 1.0F;
            return m;
        }

        // Matrix4x4.zero is of questionable usefulness considering C# sets everything to 0 by default, however:
        //  1. it's consistent with other Math structs in Unity such as Vector2, Vec3 and Vec4,
        //  2. "Matrix4x4.zero" is arguably more readable than "new Matrix4x4()",
        //  3. it's already in the API ..
        static readonly Matrix4x4 zeroMatrix = new Matrix4x4(new Vec4(0, 0, 0, 0),
            new Vec4(0, 0, 0, 0),
            new Vec4(0, 0, 0, 0),
            new Vec4(0, 0, 0, 0));

        // Returns a matrix with all elements set to zero (RO).
        public static Matrix4x4 zero { get { return zeroMatrix; } }

        static readonly Matrix4x4 identityMatrix = new Matrix4x4(new Vec4(1, 0, 0, 0),
            new Vec4(0, 1, 0, 0),
            new Vec4(0, 0, 1, 0),
            new Vec4(0, 0, 0, 1));

        // Returns the identity matrix (RO).
        public static Matrix4x4 identity { get { return identityMatrix; } }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F5";
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return
                m00.ToString(format, formatProvider) + " " + m01.ToString(format, formatProvider) + " " + m02.ToString(format, formatProvider) + " " + m03.ToString(format, formatProvider) + " " +
                m10.ToString(format, formatProvider) + " " + m11.ToString(format, formatProvider) + " " + m12.ToString(format, formatProvider) + " " + m13.ToString(format, formatProvider) + " " +
                m20.ToString(format, formatProvider) + " " + m21.ToString(format, formatProvider) + " " + m22.ToString(format, formatProvider) + " " + m23.ToString(format, formatProvider) + " " +
                m30.ToString(format, formatProvider) + " " + m31.ToString(format, formatProvider) + " " + m32.ToString(format, formatProvider) + " " + m33.ToString(format, formatProvider);
        }


        const double EPSILON = 1e-10;

        // Function to calculate the determinant of a 3x3 submatrix
        static float Determinant3x3(float a1, float a2, float a3,
                                     float b1, float b2, float b3,
                                     float c1, float c2, float c3)
        {
            return a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2);
        }

        // Function to calculate the determinant of a 4x4 matrix
        static float Determinant4x4(Matrix4x4 matrix)
        {
            float a1 = matrix[0, 0], b1 = matrix[0, 1], c1 = matrix[0, 2], d1 = matrix[0, 3];
            float a2 = matrix[1, 0], b2 = matrix[1, 1], c2 = matrix[1, 2], d2 = matrix[1, 3];
            float a3 = matrix[2, 0], b3 = matrix[2, 1], c3 = matrix[2, 2], d3 = matrix[2, 3];
            float a4 = matrix[3, 0], b4 = matrix[3, 1], c4 = matrix[3, 2], d4 = matrix[3, 3];
            return a1 * Determinant3x3(b2, b3, b4, c2, c3, c4, d2, d3, d4)
                 - b1 * Determinant3x3(a2, a3, a4, c2, c3, c4, d2, d3, d4)
                 + c1 * Determinant3x3(a2, a3, a4, b2, b3, b4, d2, d3, d4)
                 - d1 * Determinant3x3(a2, a3, a4, b2, b3, b4, c2, c3, c4);
        }

        
        // chatGPT
        public Matrix4x4 inverse()
        {
            return Matrix4x4.inverse(this);

        }
        public static Matrix4x4 inverse(Matrix4x4 source)
        {
            float[, ] matrixArray = new float[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixArray[i, j] = source[i, j];
                }
            }

            float[,] invertedMatrixArray = Inverse(matrixArray);
            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = invertedMatrixArray[i, j];
                }
            }
            return result;

        }

        public static float[,] Inverse(float[,] matrix)
         {
                int n = matrix.GetLength(0);
            float[,] result = new float[n, n];

            float[,] temp = new float[n, 2 * n];

                // Copy input matrix into temp
                for (int i = 0; i < n; ++i)
                {
                    for (int j = 0; j < n; ++j)
                    {
                        temp[i, j] = matrix[i, j];
                    }
                }

                // Augment with identity matrix
                for (int i = 0; i < n; ++i)
                {
                    for (int j = n; j < 2 * n; ++j)
                    {
                        if (j - n == i)
                        {
                            temp[i, j] = 1;
                        }
                        else
                        {
                            temp[i, j] = 0;
                        }
                    }
                }

                // Perform row operations
                for (int i = 0; i < n; ++i)
                {
                    // Swap rows if necessary
                    if (temp[i, i] == 0)
                    {
                        int swapRow = i + 1;
                        while (swapRow < n && temp[swapRow, i] == 0)
                        {
                            ++swapRow;
                        }

                        if (swapRow == n)
                        {
                            throw new Exception("Matrix is singular and cannot be inverted.");
                        }

                        for (int j = 0; j < 2 * n; ++j)
                        {
                        float tempValue = temp[i, j];
                            temp[i, j] = temp[swapRow, j];
                            temp[swapRow, j] = tempValue;
                        }
                    }

                // Scale row so that diagonal element is 1
                float scaleValue = temp[i, i];
                    for (int j = 0; j < 2 * n; ++j)
                    {
                        temp[i, j] /= scaleValue;
                    }

                    // Zero out other elements in column
                    for (int k = 0; k < n; ++k)
                    {
                        if (k != i)
                        {
                        float multValue = temp[k, i];
                            for (int j = 0; j < 2 * n; ++j)
                            {
                                temp[k, j] -= multValue * temp[i, j];
                            }
                        }
                    }
                }

                // Copy result into output matrix
                for (int i = 0; i < n; ++i)
                {
                    for (int j = n; j < 2 * n; ++j)
                    {
                        result[i, j - n] = temp[i, j];
                    }
                }

                return result;
            }
        
        
    }
}//namespace