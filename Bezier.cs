using System.Collections.Generic;

namespace Mola
{
    public class Bezier

    {
        public List<Vec3> controlPoints = new();

        public void AddControlPoint(Vec3 point)
        {
            controlPoints.Add(point);
        }
        public List<Vec3> CalculateQuadtraticBezier(int SEGMENT_COUNT)
        {
            List<Vec3> polyLine = new();
            int curveCount = controlPoints.Count / 3;
            for (int j = 0; j < curveCount; j++)
            {
                for (int i = 0; i <= SEGMENT_COUNT; i++)
                {
                    float t = i / (float)SEGMENT_COUNT;
                    int nodeIndex = j * 3;

                    Vec3 p = CalculateCubicBezierPoint(t, controlPoints[nodeIndex], controlPoints[nodeIndex + 1], controlPoints[nodeIndex + 2], controlPoints[nodeIndex + 3]);
                    polyLine.Add(p);
                }
            }
            return polyLine;
        }

        public Vec3 GetPoint(float t)
        {
            List<Vec3> cPts = new(controlPoints);
            while (cPts.Count > 1)
            {
                List<Vec3> newPts = new();
                for (int i = 0; i < cPts.Count - 1; i++)
                {
                    Vec3 p1 = cPts[i];
                    Vec3 p2 = cPts[i + 1];
                    Vec3 v = p2 - p1;
                    v *= t;
                    v += p1;

                    newPts.Add(v);
                }
                cPts = newPts;
            }
            return cPts[0];
        }

        public List<Vec3> GetPolyLine(int i)
        {
            List<Vec3> poly = new();
            for (int j = 0; j < i + 1; j++)
            {
                Vec3 cPt = GetPoint(j * 1f / i);
                poly.Add(cPt);
            }
            return poly;
        }


        public List<Vec3> CalculateCubicBezier(int SEGMENT_COUNT)
        {
            List<Vec3> polyLine = new();
            int curveCount = (int)controlPoints.Count / 3;
            for (int j = 0; j < curveCount; j++)
            {
                for (int i = 1; i <= SEGMENT_COUNT; i++)
                {
                    float t = i / (float)SEGMENT_COUNT;
                    int nodeIndex = j * 3;

                    Vec3 p = CalculateCubicBezierPoint(t, controlPoints[nodeIndex], controlPoints[nodeIndex + 1], controlPoints[nodeIndex + 2], controlPoints[nodeIndex + 3]);
                    polyLine.Add(p);
                }
            }
            return polyLine;
        }

        public Vec3 CalculateCubicBezierPoint(float t)
        {
            int curveCount = (int)controlPoints.Count / 3;
            int cCurve = (int)(curveCount / t);
            float segmentDomain = 1f / curveCount;
            float localT = t - cCurve * segmentDomain;
            int nodeIndex = cCurve * 3;
            return CalculateCubicBezierPoint(localT, controlPoints[nodeIndex], controlPoints[nodeIndex + 1], controlPoints[nodeIndex + 2], controlPoints[nodeIndex + 3]);
        }

        public static Vec3 CalculateCubicBezierPoint(float t, Vec3 p0, Vec3 p1, Vec3 p2, Vec3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vec3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }


        public static Vec3 CalculateQuadtraticBezierPoint(float t, Vec3 p0, Vec3 p1, Vec3 p2)
        {
            float u = 1 - t;
            return (u * u) * p0 + (2 * u) * t * p1 + (t * t) * p2;
        }
    }
}