using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mola
{
    public class UtilsFace
    {
        public static BoundingBox FaceBoundingBox(Vec3[] face_vertices)
        {
            return new BoundingBox(face_vertices);
        }
        public static float FacePerimeter(Vec3[] face_vertices)
        {
            Vec3 v0 = face_vertices[face_vertices.Length - 1];
            float perimeter = 0;
            for (int i = 0; i < face_vertices.Length;i++)
            {
                Vec3 v1 = face_vertices[i];
                perimeter += (v1 - v0).magnitude;
                v0 = v1;
            }
            return perimeter;
        }
        public static float FaceEdgeLength(Vec3[] face_vertices, int direction)
        {
            return (face_vertices[(direction + 1) % face_vertices.Length] - face_vertices[direction % face_vertices.Length]).magnitude;
        }
        public static float AreaTriangle(Vec3 a,Vec3 b,Vec3 c)
        {
            Vec3 ab = b - a;
            Vec3 ac = c - a;
            return Vec3.Cross(ab, ac).magnitude / 2f;
        }
        public static float FaceAreaTriOrQuad(Vec3[] face_vertices)
        {
            float area = 0;
            Vec3 v0 = face_vertices[face_vertices.Length - 2];
            Vec3 v1 = face_vertices[face_vertices.Length - 1];
            for (int i = 0; i < face_vertices.Length-2; i++)
            {
                Vec3 v2 = face_vertices[i];
                area+=AreaTriangle(v0, v1, v2);
                v0 = v1;
                v1 = v2;
            }
            return area;
        }
        public static Vec3 FaceNormal(Vec3[] face_vertices)
        {
            //"""
            //Returns the normal of a face, a vector of length 1 perpendicular to the plane of the triangle.

            //Arguments:
            //----------
            //face : mola.Face
            //    the face to get the normal from
            //"""
            //return utils_vertex.TriangleNormal(face.vertices[0], face.vertices[1], face.vertices[2])

            return TriangleNormal(face_vertices[0], face_vertices[1], face_vertices[2]);
        }

        public static Vec3 QuadNormal(Vec3 v1, Vec3 v2, Vec3 v3,Vec4 v4)
        {

            Vec3 n1 = TriangleNormal(v1, v2, v3);
            Vec3 n2 = TriangleNormal(v3, v4, v1);
            Vec3 normal = (n1 + n2) * 0.5f;
            return normal.Normalize(); 
        }
        public static Vec3 TriangleNormal(Vec3 v1, Vec3 v2, Vec3 v3)
        {
            //"""
            //Returns the normal of a triangle defined by 3 vertices.
            //The normal is a vector of length 1 perpendicular to the plane of the triangle.

            //Arguments:
            //----------
            //v1, v2, v3: mola.Vertex
            //   the vertices get the normal from
            //"""

            Vec3 v = v2 - v1;
            Vec3 u = v3 - v1;
            Vec3 crossProduct = Vec3.Cross(v, u);
            crossProduct.Normalize();

            return crossProduct;
        }
        public static Vec3 FaceCenter(Vec3[] face_vertices)
        {
            List<Vec3> vertices_list = new List<Vec3>(face_vertices);
            return UtilsVertex.vertices_list_center(vertices_list);
        }
        public static float FaceCenterY(Vec3[] face_verties)
        {
            return FaceCenter(face_verties).y;
        }
        public static Vec3 FaceCenter(MolaMesh molaMesh, int[] face)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            return FaceCenter(face_vertices);
        }
        /// <summary>
        /// Returns the altitude, 0 if the face is vertical, -Pi/2 if it faces downwards, +Pi/2 if it faces upwards.
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <returns></returns>
        public static float FaceAngleVertical(Vec3[] face_vertices)
        {
            Vec3 n = FaceNormal(face_vertices);
            return (float)Math.Asin(n.z);
        }
        /// <summary>
        /// Returns the altitude, 0 if the face is vertical, -Pi/2 if it faces downwards, +Pi/2 if it faces upwards.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public static float FaceAngleVertical(MolaMesh molaMesh, int[] face)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            Vec3 n = FaceNormal(face_vertices);
            return (float)Math.Asin(n.z);
        }
        /// <summary>
         /// Returns the azimuth, the orientation of the face around the y-axis in the XZ-plane
         /// </summary>
         /// <param name="face_vertices"></param>
         /// <returns></returns>
        public static float FaceAngleHorizontal(Vec3[] face_vertices)
        {
            //Vec3 n = FaceNormal(face_vertices);
            //return (float)Math.Atan2(n.y, n.x);
            Vec3 v = face_vertices[1] - face_vertices[0];
            return Vec3.Angle(new Vec3(1, 0, 0), new Vec3(v.x, v.y, 0));
        }
        /// <summary>
        /// Returns the azimuth, the orientation of the face around the y-axis in the XZ-plane
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public static float FaceAngleHorizontal(MolaMesh molaMesh, int[] face)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            return FaceAngleHorizontal(face_vertices);
        }
        /// <summary>
        /// Returns the compactness of a face as the ratio between area and perimeter.
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <returns></returns>
        public static float FaceCompactness(Vec3[] face_vertices)
        {
            return FaceAreaTriOrQuad(face_vertices) / FacePerimeter(face_vertices);
        }
        public static float FaceProportion(Vec3[] face_vertices)
        {
            Vec3 v0 = face_vertices[face_vertices.Length - 1];
            Vec3 v00 = face_vertices[0];
            float dmax = (v00 - v0).magnitude;
            float dmin = (v00 - v0).magnitude;
            for (int i = 0; i < face_vertices.Length; i++)
            {
                Vec3 v1 = face_vertices[i];
                float dtemp = (v1 - v0).magnitude;
                if (dmax < dtemp) dmax = dtemp;
                if (dmin > dtemp) dmin = dtemp; 
                v0 = v1;
            }
            return dmax/dmin;
        
        }
        /// <summary>
        /// Assigns a color to all the faces by values,
        /// from smallest(red) to biggest(purple).
        /// </summary>
        public static void ColorFaceByValue(MolaMesh mesh, List<int[]> faces, List<float> values, bool doGrayScale=false)
        {
            if (faces.Count != values.Count)
            {
                throw new ArgumentException("face count and value count doesnt match!");
            }
            float valueMin = values.Min();
            float valueMax = values.Max();

            for (int i = 0; i < faces.Count; i++)
            {
                float value = Mathf.Map(values[i], valueMin, valueMax, 0f, 1);
                foreach (int v in faces[i])
                {
                    mesh.Colors[v] = Color.HSVToRGB(value, 1, 1);
                }
            }
        }
        /// <summary>
        /// Assigns a color to all the faces by values,
        /// from smallest(red) to biggest(purple).
        /// </summary>
        public static void ColorFaceByValue(MolaMesh mesh, List<float> values, bool doGrayScale = false)
        {
            ColorFaceByValue(mesh, mesh.Faces, values, doGrayScale);
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
                center += p;
            }
            center /= (1f * nodes.Count);
            return center;
        }
        public static List<Vec3> offset(IList<Vec3> pts, float offset)
        {
            float[] offsets = new float[pts.Count];
            offsets = Enumerable.Repeat(offset, pts.Count).ToArray();
            return UtilsFace.offset(pts, offsets);
        }
        public static List<Vec3> offset(IList<Vec3> pts, float[] offset)
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
                Vec3 is1= UtilsFace.offset(a, b, c, offset[iPrev], offset[i], plane);
                
                offsetVec3s.Add(UtilsFace.offset(a, b, c, offset[iPrev], offset[i], plane));
            }
            return offsetVec3s;
        }
        public static Vec3 offset(Vec3 a, Vec3 b, Vec3 c, float offsetAB, float offsetBC, Plane planeABC)
        {

            Vec3 planeNormal = planeABC.normal;

            Vec3 abNorm = Vec3.Cross(b - a,planeNormal).normalized;
            Vec3 bcNorm = Vec3.Cross(c - b,planeNormal).normalized;

            Plane pAB = new Plane(abNorm, a + abNorm * offsetAB);
            Plane pBC = new Plane(bcNorm, b + bcNorm * offsetBC);
            
            Vec3[] line = Intersection.PlanePlaneIntersection(pAB, pBC);
            planeABC.LinePlaneIntersection(line[0], line[1], out Vec3 intersection);
            return intersection;
        }
    }
}