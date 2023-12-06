using System;
using System.Collections;
using System.Collections.Generic;
using static MolaDirectedGraph;

namespace Mola
{
    public class Geom2D

    {
        public static Vec3 RelFromAtoB(Vec3 a, Vec3 b, float parameter)
        {
            return (b - a) * parameter + a;
        }

        public static Vec3[] OffsetSegment(Vec3 v1, Vec3 v2, float offset)
        {
            Vec3 v12 = v2 - v1;
            Vec3 v = v12.Rotate(90).normalized * offset;
            return new Vec3[] { v1 + v, v2 + v };
        }

        static float AreaParallel(Vec3 a, Vec3 b, Vec3 c)
        {
            return (a.x - b.x) * (a.y - c.y) - (a.x - c.x) * (a.y - b.y);
        }

        public static bool IsLeft(Vec3 A, Vec3 B, Vec3 C)
        {

            return AreaParallel(A, B, C) > 0;
        }

    }
    
}