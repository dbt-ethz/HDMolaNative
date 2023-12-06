using System;
using System.Collections.Generic;
using Mola;
using System.Linq;

public class Polygon
{
	public Polygon()
	{
	}

    public static float Area2D(IList<Vec3> Vec3s)
    {
        float area = 0;
        Vec3 p1 = Vec3s[Vec3s.Count - 1];
        foreach (Vec3 p2 in Vec3s)
        {
            area += (p1.x * p2.y) - (p2.x * p1.y);
            p1 = p2;
        }
        return area / 2f;
    }

    public static Vec3 AverageCenter(IList<Vec3> nodes)
    {
        Vec3 center = new Vec3();
        foreach (Vec3 p in nodes)
        {
            center+=p;
        }
        center/=(1f * nodes.Count);
        return center;
    }

    public static List<Vec3> offsetFace3D(IList<Vec3> pts, float offset)
    {
        float[] offsets = new float[pts.Count];
        offsets = Enumerable.Repeat(offset, pts.Count).ToArray();
        return offsetFace3D(pts, offsets);
    }

    public static List<Vec3> offsetFace3D(IList<Vec3> pts, float[] offset)
    {
        List<Vec3> offsetVec3s = new List<Vec3>();
        Plane plane;
        for (int i = 0; i < pts.Count; i++)
        {
            int iPrev = i - 1;
            if (iPrev < 0) iPrev = pts.Count - 1;
            int iNext = i + 1;
            if (iNext >= pts.Count) iNext = 0;
            Vec3 a = pts[iPrev];
            Vec3 b = pts[i];
            Vec3 c = pts[iNext];
            plane = new Plane(a, b, c);
            offsetVec3s.Add(offsetFace3D(a, b, c, offset[iPrev], offset[i], plane));
        }
        return offsetVec3s;
    }

    public static Vec3 offsetFace3D(Vec3 a, Vec3 b, Vec3 c, float offsetAB, float offsetBC, Plane planeABC)
    {
        Vec3 planeNormal = planeABC.normal;
        Plane pAB = Plane.From3Points(a, b, a+ planeNormal* -1*offsetAB);
      
        Plane pBC = new Plane(b, c, b+ planeNormal*-1* offsetBC);
       // pBC.origin.add(VecMath.setMag(pBC.normal, -1 * offsetBC));
      // pBC.
        Vec3[] line = Plane.PlanePlaneIntersection(pAB, pBC);
        Vec3 intersection = new Vec3();
        bool test= planeABC.LinePlaneIntersection(line[0], line[1],out  intersection);
        return intersection;
    }
}

