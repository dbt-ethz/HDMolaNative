using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Mola
{
    /// <summary>
    /// Mola Vector
    /// </summary>
    public partial struct Vec3 : IEquatable<Vec3>, IFormattable
    {
        // *Undocumented*
        public const float kEpsilon = 0.00001F;
        // *Undocumented*
        public const float kEpsilonNormalSqrt = 1e-15F;

        /// <summary>
        /// X component of the vector.
        /// </summary>
        public float x;
        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public float y;
        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public float z;


        // Linearly interpolates between two vectors.
        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vec3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            return new Vec3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        // Moves a point /current/ in a straight line towards a /target/ point.
        public static Vec3 MoveTowards(Vec3 current, Vec3 target, float maxDistanceDelta)
        {
            // avoid vector ops because current scripting backends are terrible at inlining
            float toVector_x = target.x - current.x;
            float toVector_y = target.y - current.y;
            float toVector_z = target.z - current.z;

            float sqdist = toVector_x * toVector_x + toVector_y * toVector_y + toVector_z * toVector_z;

            if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
                return target;
            var dist = (float)Math.Sqrt(sqdist);

            return new Vec3(current.x + toVector_x / dist * maxDistanceDelta,
                current.y + toVector_y / dist * maxDistanceDelta,
                current.z + toVector_z / dist * maxDistanceDelta);
        }


        // Access the x, y, z components using [0], [1], [2] respectively.
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        public  Vec3 Rotate(  float degrees)
        {

            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = x;
            float ty = y;
            float vx = (cos * tx) - (sin * ty);
            float vy = (sin * tx) + (cos * ty);
            return new Vec3(vx, vy);
        }
        public static Vec3 FromPolar(float radius, float radians)
        {
            float x = radius * Mathf.Cos(radians);
            float y = radius * Mathf.Sin(radians);
            return new Vec3(x, y,0);
        }
        public static Vec3 SetMag( Vec3 v, float targetLength)
        {
            return v * targetLength / v.magnitude;
        }

        public Vec3 GetWithMag(float targetLength)
        {
            Vec3 v= Vec3.SetMag(this, targetLength);
            /*this.x = v.x;
            this.y = v.y;
            this.z = v.z;*/
            return v;
        }

        // Creates a new vector with given x, y, z components.
        public Vec3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        // Creates a new vector with given x, y components and sets /z/ to zero.
        public Vec3(float x, float y) { this.x = x; this.y = y; z = 0F; }

        // Set x, y and z components of an existing Vector3.
        public void Set(float newX, float newY, float newZ) { x = newX; y = newY; z = newZ; }

        // Multiplies two vectors component-wise.
        public static Vec3 Scale(Vec3 a, Vec3 b) { return new Vec3(a.x * b.x, a.y * b.y, a.z * b.z); }

        // Multiplies every component of this vector by the same component of /scale/.
        public void Scale(Vec3 scale) { x *= scale.x; y *= scale.y; z *= scale.z; }

        // Cross Product of two vectors.
        public static Vec3 Cross(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public Vec3 Cross(Vec3 rhs)
        {
            return Vec3.Cross(this, rhs);
        }

        // used to allow Vector3s to be used as keys in hash tables
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        // also required for being able to use Vector3s as keys in hash tables
        public override bool Equals(object other)
        {
            if (!(other is Vec3)) return false;

            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        // Reflects a vector off the plane defined by a normal.
        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal)
        {
            float factor = -2F * Dot(inNormal, inDirection);
            return new Vec3(factor * inNormal.x + inDirection.x,
                factor * inNormal.y + inDirection.y,
                factor * inNormal.z + inDirection.z);
        }

        // *undoc* --- we have normalized property now
        public static Vec3 Normalize(Vec3 value)
        {
            float mag = Magnitude(value);
            if (mag > kEpsilon)
                return value / mag;
            else
                return zero;
        }

        // Makes this vector have a ::ref::magnitude of 1.
        public Vec3 Normalize()
        {
            float mag = Magnitude(this);
            if (mag > kEpsilon)
                this = this / mag;
            else
                this = zero;
            return this;
        }

        // Returns this vector with a ::ref::magnitude of 1 (RO).
        public Vec3 normalized
        {
            get { return Vec3.Normalize(this); }
        }

        // Dot Product of two vectors.
        public static float Dot(Vec3 lhs, Vec3 rhs) { return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z; }


        // Dot Product of two vectors.
        public float Dot(Vec3 rhs) {
            return Vec3.Dot(this,rhs);
        }

        // Projects a vector onto another vector.
        public static Vec3 Project(Vec3 vector, Vec3 onNormal)
        {
            float sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < Mathf.Epsilon)
                return zero;
            else
            {
                var dot = Dot(vector, onNormal);
                return new Vec3(onNormal.x * dot / sqrMag,
                    onNormal.y * dot / sqrMag,
                    onNormal.z * dot / sqrMag);
            }
        }

        // Projects a vector onto a plane defined by a normal orthogonal to the plane.
        public static Vec3 ProjectOnPlane(Vec3 vector, Vec3 planeNormal)
        {
            float sqrMag = Dot(planeNormal, planeNormal);
            if (sqrMag < Mathf.Epsilon)
                return vector;
            else
            {
                var dot = Dot(vector, planeNormal);
                return new Vec3(vector.x - planeNormal.x * dot / sqrMag,
                    vector.y - planeNormal.y * dot / sqrMag,
                    vector.z - planeNormal.z * dot / sqrMag);
            }
        }

        // Returns the angle in degrees between /from/ and /to/. This is always the smallest
        public static float Angle(Vec3 from, Vec3 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = Mathf.Clamp(Dot(from, to) / denominator, -1F, 1F);
            return ((float)Math.Acos(dot)) * Mathf.Rad2Deg;
        }

        // The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
        // If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
        // The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
        public static float SignedAngle(Vec3 from, Vec3 to, Vec3 axis)
        {
            float unsignedAngle = Angle(from, to);

            float cross_x = from.y * to.z - from.z * to.y;
            float cross_y = from.z * to.x - from.x * to.z;
            float cross_z = from.x * to.y - from.y * to.x;
            float sign = Mathf.Sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
            return unsignedAngle * sign;
        }

        // Returns the distance between /a/ and /b/.
        public static float Distance(Vec3 a, Vec3 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            float diff_z = a.z - b.z;
            return (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
        }

        // Returns a copy of /vector/ with its magnitude clamped to /maxLength/.
        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength)
        {
            float sqrmag = vector.sqrMagnitude;
            if (sqrmag > maxLength * maxLength)
            {
                float mag = (float)Math.Sqrt(sqrmag);
                //these intermediate variables force the intermediate result to be
                //of float precision. without this, the intermediate result can be of higher
                //precision, which changes behavior.
                float normalized_x = vector.x / mag;
                float normalized_y = vector.y / mag;
                float normalized_z = vector.z / mag;
                return new Vec3(normalized_x * maxLength,
                    normalized_y * maxLength,
                    normalized_z * maxLength);
            }
            return vector;
        }

        // *undoc* --- there's a property now
        public static float Magnitude(Vec3 vector) { return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

        // Returns the length of this vector (RO).
        public float magnitude
        {
            get { return (float)Math.Sqrt(x * x + y * y + z * z); }
        }

        // *undoc* --- there's a property now
        public static float SqrMagnitude(Vec3 vector) { return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z; }

        // Returns the squared length of this vector (RO).
        public float sqrMagnitude { get { return x * x + y * y + z * z; } }

        // Returns a vector that is made from the smallest components of two vectors.
        public static Vec3 Min(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
        }

        // Returns a vector that is made from the largest components of two vectors.
        public static Vec3 Max(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
        }

        static readonly Vec3 zeroVector = new Vec3(0F, 0F, 0F);
        static readonly Vec3 oneVector = new Vec3(1F, 1F, 1F);
        static readonly Vec3 upVector = new Vec3(0F, 1F, 0F);
        static readonly Vec3 downVector = new Vec3(0F, -1F, 0F);
        static readonly Vec3 leftVector = new Vec3(-1F, 0F, 0F);
        static readonly Vec3 rightVector = new Vec3(1F, 0F, 0F);
        static readonly Vec3 forwardVector = new Vec3(0F, 0F, 1F);
        static readonly Vec3 backVector = new Vec3(0F, 0F, -1F);
        static readonly Vec3 positiveInfinityVector = new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vec3 negativeInfinityVector = new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        // Shorthand for writing @@Vector3(0, 0, 0)@@
        public static Vec3 zero { get { return zeroVector; } }
        // Shorthand for writing @@Vector3(1, 1, 1)@@
        public static Vec3 one { get { return oneVector; } }
        // Shorthand for writing @@Vector3(0, 0, 1)@@
        public static Vec3 forward { get { return forwardVector; } }
        public static Vec3 back { get { return backVector; } }
        // Shorthand for writing @@Vector3(0, 1, 0)@@
        public static Vec3 up { get { return upVector; } }
        public static Vec3 down { get { return downVector; } }
        public static Vec3 left { get { return leftVector; } }
        // Shorthand for writing @@Vector3(1, 0, 0)@@
        public static Vec3 right { get { return rightVector; } }
        // Shorthand for writing @@Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)@@
        public static Vec3 positiveInfinity { get { return positiveInfinityVector; } }
        // Shorthand for writing @@Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity)@@
        public static Vec3 negativeInfinity { get { return negativeInfinityVector; } }

        // Adds two vectors.
        public static Vec3 operator +(Vec3 a, Vec3 b) { return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z); }
        // Subtracts one vector from another.
        public static Vec3 operator -(Vec3 a, Vec3 b) { return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z); }
        // Negates a vector.
        public static Vec3 operator -(Vec3 a) { return new Vec3(-a.x, -a.y, -a.z); }
        // Multiplies a vector by a number.
        public static Vec3 operator *(Vec3 a, float d) { return new Vec3(a.x * d, a.y * d, a.z * d); }
        // Multiplies a vector by a number.
        public static Vec3 operator *(float d, Vec3 a) { return new Vec3(a.x * d, a.y * d, a.z * d); }
        // Divides a vector by a number.
        public static Vec3 operator /(Vec3 a, float d) { return new Vec3(a.x / d, a.y / d, a.z / d); }

        // Returns true if the vectors are equal.
        public static bool operator ==(Vec3 lhs, Vec3 rhs)
        {
            // Returns false in the presence of NaN values.
            float diff_x = lhs.x - rhs.x;
            float diff_y = lhs.y - rhs.y;
            float diff_z = lhs.z - rhs.z;
            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z;
            return sqrmag < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        public static bool operator !=(Vec3 lhs, Vec3 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }
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
                format = "F2";
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return "" + x.ToString(format, formatProvider) + " " + y.ToString(format, formatProvider) + " " + z.ToString(format, formatProvider);
        }

    }
}


