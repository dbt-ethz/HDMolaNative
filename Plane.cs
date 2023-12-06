using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Mola
{
    // Representation of planes. Uses the formula Ax + By + Cz + D = 0.
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct Plane : IFormattable
    {
        // sizeof(Plane) is not const in C# and so cannot be used in fixed arrays, so we define it here
        internal const int size = 16;

        Vec3 m_Normal;
        float m_Distance;

        // Normal vector of the plane.
        public Vec3 normal
        {
            get { return m_Normal; }
            set { m_Normal = value; }
        }
        // Distance from the origin to the plane.
        public float distance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }

        // Creates a plane.
        public Plane(Vec3 inNormal, Vec3 inPoint)
        {
            m_Normal = Vec3.Normalize(inNormal);
            m_Distance = -Vec3.Dot(m_Normal, inPoint);
        }

        // Creates a plane.
        public Plane(Vec3 inNormal, float d)
        {
            m_Normal = Vec3.Normalize(inNormal);
            m_Distance = d;
        }

        // Creates a plane.
        public Plane(Vec3 a, Vec3 b, Vec3 c)
        {
            m_Normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
            m_Distance = -Vec3.Dot(m_Normal, a);
        }

        // Sets a plane using a point that lies within it plus a normal to orient it
        public void SetNormalAndPosition(Vec3 inNormal, Vec3 inPoint)
        {
            m_Normal = Vec3.Normalize(inNormal);
            m_Distance = -Vec3.Dot(m_Normal, inPoint);
        }



        // doc: returns parametric line point and direction...
        // http://www.songho.ca/math/plane/plane.html#intersect_2planes
        public static Vec3[] PlanePlaneIntersection(Plane plane1, Plane plane2)
        {
            Vec3 n1 = plane1.normal;
            Vec3 n2 = plane2.normal;
            float d1 = plane1.m_Distance;
            float d2 = plane2.m_Distance;

            // find direction vector of the intersection line
            Vec3 v = n1.Cross(n2);                   // cross product

            // if |direction| = 0, 2 planes are parallel (no intersect)
            // return a line with NaN
            if (v.x == 0 && v.y == 0 && v.z == 0)
                return null;

            // find a point on the line, which is also on both planes
            // choose simplest plane where d=0: ax + by + cz = 0
            float dot = v.Dot(v);                       // V dot V
            Vec3 u1 = n1* d2;                      // d2 * N1
            Vec3 u2 = n2* -d1;                      //-d1 * N2
            Vec3 p = (u1+ u2).Cross(v) / dot;       // (d2*N1-d1*N2) X V / V dot V
            return new Vec3[] { p, v };
        }

        // Sets a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.
        public void Set3Points(Vec3 a, Vec3 b, Vec3 c)
        {
            m_Normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
            m_Distance = -Vec3.Dot(m_Normal, a);
        }

        public Vec3 PointInPlane()
        {
            return normal * distance;
        }

        // returns a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.
        public static Plane From3Points(Vec3 a, Vec3 b, Vec3 c)
        {
            Vec3 m_Normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
            float m_Distance = -Vec3.Dot(m_Normal, a);
            return new Plane(m_Normal, m_Distance);
        }

        // Make the plane face the opposite direction
        public void Flip() { m_Normal = -m_Normal; m_Distance = -m_Distance; }

        // Return a version of the plane that faces the opposite direction
        public Plane flipped
        {
            get { return new Plane(-m_Normal, -m_Distance); }
        }

        public Vec3 Mirror(Vec3 pt)
        {
            return  normal*(this.GetDistanceToPoint(pt) * -2f)+pt;
           
        }
        // Translates the plane into a given direction
        public void Translate(Vec3 translation) {
            m_Distance += Vec3.Dot(m_Normal, translation);
        }
        // Translates the plane into a given direction
        public void TranslateAlongNormal(float translation)
        {
            Translate(this.m_Normal * translation);
        }

        // Creates a plane that's translated into a given direction
        public static Plane Translate(Plane plane, Vec3 translation) {
            return new Plane(plane.m_Normal, plane.m_Distance += Vec3.Dot(plane.m_Normal, translation));
        }

        // Calculates the closest point on the plane.
        public Vec3 ClosestPointOnPlane(Vec3 point)
        {
            var pointToPlaneDistance = Vec3.Dot(m_Normal, point) + m_Distance;
            return point - (m_Normal * pointToPlaneDistance);
        }

        // Returns a signed distance from plane to point.
        public float GetDistanceToPoint(Vec3 point) { return Vec3.Dot(m_Normal, point) + m_Distance; }

        // Is a point on the positive side of the plane?
        public bool GetSide(Vec3 point) { return Vec3.Dot(m_Normal, point) + m_Distance > 0.0F; }

        // Are two points on the same side of the plane?
        public bool SameSide(Vec3 inPt0, Vec3 inPt1)
        {
            float d0 = GetDistanceToPoint(inPt0);
            float d1 = GetDistanceToPoint(inPt1);
            return (d0 > 0.0f && d1 > 0.0f) ||
                (d0 <= 0.0f && d1 <= 0.0f);
        }

        public bool SegmentIntersection(Vec3 v1,Vec3 v2, out Vec3 intersection)
        {
            Vec3 direction = v2 - v1;
            float mag = direction.magnitude;
            Ray ray = new Ray(v1, direction);
            float parameter = 0;
            bool cast=Raycast(ray, out parameter);
            if (parameter>0 && parameter < mag)
            {
                intersection = ray.GetPoint(parameter);
                return true;
            }
            intersection = new Vec3(0, 0, 0);
            return false;
        }

        // Intersects a ray with the plane.
        public bool LinePlaneIntersection(Vec3 a, Vec3 b, out Vec3 intersection)
        {
            Vec3 dir = b - a;
            float vdot = Vec3.Dot(dir, m_Normal);
            float ndot = -Vec3.Dot(a, m_Normal) - m_Distance;

            if (Mathf.Approximately(vdot, 0.0f))
            {
                //enter = 0.0F;
                intersection = new Vec3();
                return false;
            }

            float enter = ndot / vdot;
            intersection = enter * dir + a;

            return true;
        }
        // Intersects a ray with the plane.
        public bool Raycast(Ray ray, out float enter)
        {
            float vdot = Vec3.Dot(ray.direction, m_Normal);
            float ndot = -Vec3.Dot(ray.origin, m_Normal) - m_Distance;

            if (Mathf.Approximately(vdot, 0.0f))
            {
                enter = 0.0F;
                return false;
            }

            enter = ndot / vdot;

            return enter > 0.0F;
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
            return m_Normal.ToString(format, formatProvider) + " " + m_Distance.ToString(format, formatProvider);
        }
    }
}