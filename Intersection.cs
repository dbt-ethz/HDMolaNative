using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;

namespace Mola
{
    public class Intersection
    {

        // Infinite Line Intersection (line1 is p1-p2 and line2 is p3-p4)
        internal static bool LineIntersection(Vec3 p1, Vec3 p2, Vec3 p3, Vec3 p4, ref Vec3 result)
        {
            float bx = p2.x - p1.x;
            float by = p2.y - p1.y;
            float dx = p4.x - p3.x;
            float dy = p4.y - p3.y;
            float bDotDPerp = bx * dy - by * dx;
            if (bDotDPerp == 0)
            {
                return false;
            }
            float cx = p3.x - p1.x;
            float cy = p3.y - p1.y;
            float t = (cx * dy - cy * dx) / bDotDPerp;

            result.x = p1.x + t * bx;
            result.y = p1.y + t * by;
            return true;
        }

        // Line Segment Intersection (line1 is p1-p2 and line2 is p3-p4)
        internal static bool LineSegmentIntersection(Vec3 p1, Vec3 p2, Vec3 p3, Vec3 p4, ref Vec3 result)
        {
            float bx = p2.x - p1.x;
            float by = p2.y - p1.y;
            float dx = p4.x - p3.x;
            float dy = p4.y - p3.y;
            float bDotDPerp = bx * dy - by * dx;
            if (bDotDPerp == 0)
            {
                return false;
            }
            float cx = p3.x - p1.x;
            float cy = p3.y - p1.y;
            float t = (cx * dy - cy * dx) / bDotDPerp;
            if (t < 0 || t > 1)
            {
                return false;
            }
            float u = (cx * by - cy * bx) / bDotDPerp;
            if (u < 0 || u > 1)
            {
                return false;
            }

            result.x = p1.x + t * bx;
            result.y = p1.y + t * by;
            return true;
        }
        public static Nullable<Vec3> LineLineIntersection(Vec3 a1, Vec3 a2, Vec3 b1, Vec3 b2)
        {
            return LineLineIntersection(a1.x, a1.y, a2.x, a2.y, b1.x, b1.y, b2.x, b2.y);
        }

        public static Nullable<Vec3> LineLineIntersection(float aX, float aY, float bX,
                float bY, float cX, float cY, float dX, float dY)
        {
            double denominator = ((bX - aX) * (dY - cY)) - ((bY - aY) * (dX - cX));
            if (denominator == 0)
                return null;// parallel
            double numerator = ((aY - cY) * (dX - cX)) - (aX - cX) * (dY - cY);
            double r = numerator / denominator;
            double x = aX + r * (bX - aX);
            double y = aY + r * (bY - aY);
            return new Vec3((float)x, (float)y);
        }

        public static Nullable<Vec3> LineLineIntersectionDir(Vec3 org1, Vec3 dir1,
                Vec3 org2, Vec3 dir2)
        {
            float denominator = dir1.x * dir2.y - dir1.y * dir2.x;
            if (denominator == 0)
                return null;// parallel
            float numerator = (org1.y - org2.y) * dir2.x - (org1.x - org2.x) * dir2.y;
            float r = numerator / denominator;
            return org1 + r * dir1;

        }

        public static Vec3? RaySegment(Vec3 org, Vec3 dir,
            Vec3 c, Vec3 d)
        {
            float denominator = dir.x * (d.y - c.y) - (dir.y) * (d.x - c.x);
            if (denominator == 0)
            {
                //Nullable<Vec3> result = null;
                return null;
            }
            float numerator = (org.y - c.y) * (d.x - c.x) - (org.x - c.x) * (d.y - c.y);
            float numerator2 = (org.y - c.y) * dir.x - (org.x - c.x) * dir.y;
            float r = numerator / denominator;
            float s = numerator2 / denominator;
            if (s < 0 || s > 1 || r <= 0)
            {
                //Nullable<Vec3> result = null;
                return null;
            }
            //return null;// colinear
            Vec3 intersection = org + r * dir;
            return intersection;
        }

        // intersect two planes
        // Intersection of 2-planes: a variation based on the 3-plane version.
        // see: Graphics Gems 1 pg 305
        //
        // Note that the 'normal' components of the planes need not be unit length
        public static Vec3[] PlanePlaneIntersection(Plane p1, Plane p2)
        {
            // logically the 3rd plane, but we only use the normal component.
            Vec3 p3_normal = p1.normal.Cross(p2.normal);
            float det = p3_normal.sqrMagnitude;

            // If the determinant is 0, that means parallel planes, no intersection.
            // note: you may want to check against an epsilon value here.
            if (det != 0.0)
            {
                // calculate the final (point, normal)
                Vec3 r_point = ((p3_normal.Cross(p2.normal) * p1.distance) +
                           (p1.normal.Cross(p3_normal) * p2.distance)) / det;
                Vec3 r_normal = p3_normal;
                Vec3[] line = new Vec3[2];
                line[0] = r_point;
                line[1] = r_point + r_normal;
                return line;
            }
            else
            {
                return null;
            }
        }
        

    }
}
