using System;
using System.Collections.Generic;
using Mola;

namespace Mola
{
    public class MeshPiping
    {
        public static Vec3 GetCenterAverage(List<Vec3> profile)
        {
            Vec3 center = new Vec3();
            foreach (Vec3 p in profile)
            {
                center = center + p;
            }
            return center / profile.Count;
        }
        public static void AddTriangleFan(MolaMesh mesh, int center, int iStart, int iEnd, bool inverse = false)
        {
            for (int i = iStart; i < iEnd; i++)
            {
                int i2 = i + 1;
                if (i2 >= iEnd) i2 = iStart;
                if (inverse)
                {
                    mesh.AddTriangle(center, i2, i);
                }
                else
                {
                    mesh.AddTriangle(center, i, i2);
                }
            }
        }
        public static MolaMesh PipeLineWithConvexProfile(Vec3 a, Vec3 b, List<Vec3> profile, Vec3 up, bool closeStart, bool closeEnd)
        {
            MolaMesh pipe = new MolaMesh();
            Matrix4x4 m;

            Vec3 from = a;
            Vec3 to = b;
            Vec3 aToB = b - a;
            m = Matrix4x4.FromFrame(from, to, up);
            List<Vec3> ring = new List<Vec3>();
            foreach (Vec3 p in profile)
            {
                Vec3 cP = m.MultiplyPoint(p);
                ring.Add(cP);
                pipe.AddVertex(cP.x, cP.y, cP.z);
            }
            foreach (Vec3 cP in ring)
            {
                Vec3 p = cP + aToB;
                pipe.AddVertex(p.x, p.y, p.z);
            }

            int nSegs = profile.Count;
            for (int i = 0; i < pipe.Vertices.Count - nSegs; i += nSegs)
            {
                for (int j = 0; j < profile.Count; j++)
                {
                    int j2 = (j + 1) % profile.Count;
                    pipe.AddQuad(i + j, i + j + nSegs, i + j2 + nSegs, i + j2);
                }
            }

            if (closeStart)
            {
                List<Vec3> startCap = new List<Vec3>();
                for (int i = 0; i < profile.Count; i++)
                {
                    startCap.Add(pipe.Vertices[i]);

                }
                Vec3 center = GetCenterAverage(startCap);
                int iCenter = pipe.AddVertex(center.x, center.y, center.z);
                AddTriangleFan(pipe, iCenter, 0, profile.Count);

            }
            if (closeEnd)
            {
                List<Vec3> endCap = new List<Vec3>();
                int startI = 0;
                for (int i = 0; i < profile.Count; i++)
                {
                    endCap.Add(pipe.Vertices[i + startI]);
                }
                Vec3 center = GetCenterAverage(endCap);
                int iCenter = pipe.AddVertex(center.x, center.y, center.z);
                AddTriangleFan(pipe, iCenter, startI, startI + profile.Count);
            }

            return pipe;
        }
        public static Matrix4x4 TransformMatrix(Vec3 from1,Vec3 to1,Vec3 up1, Vec3 from2, Vec3 to2, Vec3 up2)
        {
            Matrix4x4 ma = Matrix4x4.FromFrame(from1, to1, up1);
            Matrix4x4 mb = Matrix4x4.FromFrame(from2, to2, up2);
            
            return mb* ma;
        }
        public static MolaMesh PipePolyLineWithConvexProfile(List<Vec3> nodes, List<Vec3> profile, Vec3 up, bool closeStart, bool closeEnd)
        {
            MolaMesh pipe = new MolaMesh();
            Matrix4x4 m;
            Matrix4x4 mb = Matrix4x4.FromFrame(Vec3.zero, new Vec3(0, 0, 1), new Vec3(1, 0, 0));

            // last direction taken from previous direction
            // todo: check closed ring
            for (int i = 0; i < nodes.Count; i++)
            {
                Vec3 from = nodes[i];
                Vec3 to;
                if (i < nodes.Count - 1)
                {
                    to = nodes[i + 1];
                }
                else
                {
                    to = from + from - nodes[i - 1];
                }

                m = Matrix4x4.FromFrame(from, to, up);
                // m.
                Matrix4x4 resultm = mb * m;
                foreach (Vec3 p in profile)
                {
                    Vec3 cP = resultm.MultiplyPoint(p);
                    pipe.AddVertex(cP.x, cP.y, cP.z);
                }
            }
            int nSegs = profile.Count;
            for (int i = 0; i < pipe.Vertices.Count - nSegs; i += nSegs)
            {
                for (int j = 0; j < profile.Count; j++)
                {
                    int j2 = (j + 1) % profile.Count;
                    pipe.AddQuad(i + j, i + j + nSegs, i + j2 + nSegs, i + j2);
                }
            }

            if (closeStart)
            {
                List<Vec3> startCap = new List<Vec3>();
                for (int i = 0; i < profile.Count; i++)
                {
                    startCap.Add(pipe.Vertices[i]);

                }
                Vec3 center = GetCenterAverage(startCap);
                int iCenter = pipe.AddVertex(center.x, center.y, center.z);
                AddTriangleFan(pipe, iCenter, 0, profile.Count);

            }
            if (closeEnd)
            {
                List<Vec3> endCap = new List<Vec3>();
                int startI = (nodes.Count - 2) * profile.Count;
                for (int i = 0; i < profile.Count; i++)
                {
                    endCap.Add(pipe.Vertices[i + startI]);
                }
                Vec3 center = GetCenterAverage(endCap);
                int iCenter = pipe.AddVertex(center.x, center.y, center.z);
                AddTriangleFan(pipe, iCenter, startI, startI + profile.Count);
            }

            return pipe;
        }
    }
}



