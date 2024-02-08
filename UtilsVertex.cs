using System;
using System.Collections;
using System.Collections.Generic;

namespace Mola
{
    public class UtilsVertex
    {
        public static Vec3[] face_vertices(MolaMesh mesh, int[] face)
        {
            Vec3[] face_v = new Vec3[face.Length];
            for (int i = 0; i < face.Length; i++)
            {
                face_v[i] = mesh.Vertices[face[i]];
            }

            return face_v;
        }
        public static Vec3 vertices_list_center(List<Vec3> vertices)
        {
            Vec3 vSum = new Vec3(0, 0, 0);
            foreach (var vertex in vertices)
            {
                vSum += vertex;
            }

            return vSum / vertices.Count;
        }
        public static Vec3 vertex_center(Vec3 v1, Vec3 v2)
        {
            return (v1 + v2) / 2;
        }
        public static Vec3[] getVertexNormals(MolaMesh mesh)
        {
            Vec3[] normals = new Vec3[mesh.Vertices.Count];
            int[] nFaces = new int[mesh.Vertices.Count];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = new Vec3();
            }
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                int[] face = mesh.Faces[i];
                for (int j = 0; j < face.Length; j++)
                {
                    int j2 = (j + 1) % face.Length;
                    int j3 = (j2 + 1) % face.Length;
                    int v1 = face[j];
                    int v2 = face[j2];
                    int v3 = face[j3];
                    Vec3 u = mesh.Vertices[v2] - mesh.Vertices[v1];
                    Vec3 v = mesh.Vertices[v3] - mesh.Vertices[v1];
                    Vec3 normal = Vec3.Cross(u, v);
                    normal.Normalize();
                    nFaces[v1] += 1;
                    normals[v1] += normal;

                }
            }
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normals[i] / nFaces[i];
                normals[i].Normalize();
            }
            return normals;
        }
        public static Vec3 polarRad(float angle, float radius)
        {
            //float a=Mathf.Rad2Deg
            return new Vec3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
        }
        public static Vec3 polarRadWithZ(float angle, float radius, float z)
        {
            //float a=Mathf.Rad2Deg
            return new Vec3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), z);
        }
        public static Vec3 getBetweenRel(Vec3 p1, Vec3 p2, float mag)
        {
            Vec3 v = p2 - p1;
            return v * mag + p1;
        }
        /// <summary>
        /// finds a position vector between v1 and v2 by a factor (0.0 to 1.0 corresponds to v1 to v2)
        /// and returns the result as a new Vertex.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Vec3 vertex_between_rel(Vec3 v1, Vec3 v2, float factor)
        {
            return new Vec3((v2.x - v1.x) * factor + v1.x, (v2.y - v1.y) * factor + v1.y, (v2.z - v1.z) * factor + v1.z);
        }
        public static List<Vec3> getLine(Vec3 v1, Vec3 v2, int segments)
        {
            List<Vec3> profile = new List<Vec3>();
            Vec3 vec = (v2 - v1) / segments;
            for (int i = 0; i < segments; i++)
            {
                Vec3 v = vec * i + v1;
                profile.Add(v);
            }
            return profile;
        }
        public static List<Vec3> getArc(float angle1, float angle2, float radius, int segments)
        {
            List<Vec3> profile = new List<Vec3>();
            float deltaAngle = (float)((angle2 - angle1) / (segments));
            for (int i = 0; i < segments; i++)
            {
                float cAngle = deltaAngle * i + angle1;
                profile.Add(new Vec3((float)Mathf.Cos(cAngle) * radius, (float)Mathf.Sin(cAngle) * radius, 0));
            }
            return profile;
        }
        public static List<Vec3> getCircle(float cX, float cY, float radius, int segments, float z = 0)
        {
            List<Vec3> profile = new List<Vec3>();
            float deltaAngle = (float)(2 * Math.PI / segments);
            for (int i = 0; i < segments; i++)
            {
                float cAngle = deltaAngle * i;
                profile.Add(new Vec3((float)Mathf.Cos(cAngle) * radius + cX, (float)Mathf.Sin(cAngle) * radius + cY, z));
            }
            return profile;
        }
        public static void translate(List<Vec3> vectors, float tX, float tY, float tZ)
        {

            for (int i = 0; i < vectors.Count; i++)
            {
                Vec3 v = vectors[i];
                v.Set(v.x + tX, v.y + tY, v.z + tZ);
                vectors[i] = v;

            }
        }
        public static List<Vec3> Rotate(float degrees, Vec3 axis, List<Vec3> vertices)
        {
            List<Vec3> newVertices = new List<Vec3>();
            Quaternion quat = Quaternion.AngleAxis(degrees, axis);
            for (int i = 0; i < vertices.Count; i++)
            {
                newVertices.Add(Rotated(vertices[i], quat));
            }
            return newVertices;
        }
        public static Vec3 Rotated(Vec3 vector, Quaternion rotation, Vec3 pivot = default(Vec3))
        {
            return rotation * (vector - pivot) + pivot;
        }
        /// <summary>
        /// finds a position vector between v1 and v2 by an absolute distance value from v1
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        public static Vec3 vertex_between_abs(Vec3 v1, Vec3 v2, float dis)
        {
            float d = Vec3.Distance(v1, v2);
            return vertex_between_rel(v1, v2, dis / d);
        }
        /// <summary>
        /// law of cosines
        /// </summary>
        /// <param name="vPrevious"></param>
        /// <param name="v"></param>
        /// <param name="vNext"></param>
        /// <returns></returns>
        public static float vertex_angle_triangle(Vec3 vPrevious, Vec3 v, Vec3 vNext)
        {
            float vvn = Vec3.Distance(v, vNext);
            float vvp = Vec3.Distance(vPrevious, v);
            float vnvp = Vec3.Distance(vNext, vPrevious);
            return (float)Math.Acos((vvn * vvn + vvp * vvp - vnvp * vnvp) / (2 * vvn * vvp));
        }
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

