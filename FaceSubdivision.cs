using System;
using Mola;
using System.Collections.Generic;
using System.Linq;

	public class FaceSubdivision
	{
		public FaceSubdivision()
		{
		}

    private static List<Vec3> VerticesBetween(Vec3 v1, Vec3 v2, int n)
    {
        List<Vec3> rowList = new List<Vec3>();
        Vec3 deltaV = (v2 - v1) / n;
        for (int i = 0; i < n; i++)
        {
            Vec3 addV = deltaV * i + v1;
            rowList.Add(addV);
        }
        rowList.Add(v2);

        return rowList;
    }

    public static List<Vec3[]> LinearSplitQuad(IList<Vec3> vertices, float minSegmentWidth = 1, float maxSegmentWidth = 2, int dir = 0)
    {
        List<Vec3[]> faces = new List<Vec3[]>();
        List<Vec3> list0 = new List<Vec3>();
        List<Vec3> list1 = new List<Vec3>();
        Vec3 v0 = vertices[0];
        Vec3 v1 = vertices[1];
        Vec3 v2 = vertices[2];
        Vec3 v3 = vertices[3];

        if (dir == 0)
        {
            Vec3 v01 = v1 - v0;
            Vec3 v32 = v2 - v3;
            float totalLength = v01.magnitude;

            float startLength = 0;
            float cLength = startLength;

            while (cLength < totalLength - minSegmentWidth)
            {
                float fac = cLength / totalLength;
                list0.Add(v0 + v01 * fac);
                list1.Add(v3 + v32 * fac);
                Random rand = new Random();
                float d = (float)rand.NextDouble() * (maxSegmentWidth - minSegmentWidth) + minSegmentWidth;
                cLength += d;
            }
            list0.Add(v1);
            list1.Add(v2);
            for (int i = 0; i < list0.Count - 1; i++)
            {
                int iNext = i + 1;
                Vec3[] face = new Vec3[] { list0[i], list0[iNext], list1[iNext], list1[i] };
                faces.Add(face);
            }
        }
        else
        {
            Vec3 v03 = v3 - v0;
            Vec3 v12 = v2 - v1;
            float totalLength = v03.magnitude;

            float startLength = 0;
            float cLength = startLength;

            while (cLength < totalLength - minSegmentWidth)
            {
                float fac = cLength / totalLength;
                list0.Add(v0 + v03 * fac);
                list1.Add(v1 + v12 * fac);
                Random rand = new Random();
                float d = (float)rand.NextDouble() * (maxSegmentWidth - minSegmentWidth) + minSegmentWidth;
                cLength += d;
            }
            list0.Add(v3);
            list1.Add(v2);
            for (int i = 0; i < list0.Count - 1; i++)
            {
                int iNext = i + 1;
                Vec3[] face = new Vec3[] { list0[i], list1[i], list1[iNext], list0[iNext] };
                faces.Add(face);
            }
        }
        return faces;
    }

    public static List<Vec3[]> LinearSplitQuadBorder(IList<Vec3> vertices, float borderWidth1 = 1, float borderWidth2 = 1,int dir = 0)
    {
        List<Vec3[]> faces = new List<Vec3[]>();
        List<Vec3> list0 = new List<Vec3>();
        List<Vec3> list1 = new List<Vec3>();
        Vec3 v0 = vertices[0];
        Vec3 v1 = vertices[1];
        Vec3 v2 = vertices[2];
        Vec3 v3 = vertices[3];

        if (dir == 0)
        {
            Vec3 v01 = v1 - v0;
            Vec3 v32 = v2 - v3;
            float totalLength = v01.magnitude;
            float startLength = 0;
            float cLength = startLength;

           
            float fac1 = borderWidth1 / totalLength;
            float fac2 = borderWidth2 / totalLength;
            list0.Add(v0 );
            list0.Add(v0 + v01 * fac1);
            list0.Add(v1 - v01 * fac2);
            list0.Add(v1);
            list1.Add(v3 );
            list1.Add(v3 + v32 * fac1);
            list1.Add(v2 - v32 * fac2);
            list1.Add(v2);


            
            list1.Add(v2);
            for (int i = 0; i < list0.Count - 1; i++)
            {
                int iNext = i + 1;
                Vec3[] face = new Vec3[] { list0[i], list0[iNext], list1[iNext], list1[i] };
                faces.Add(face);
            }
        }
        else
        {
            Vec3 v03 = v3 - v0;
            Vec3 v12 = v2 - v1;
            float totalLength = v03.magnitude;

            float startLength = 0;
            float cLength = startLength;

            float fac1 = borderWidth1 / totalLength;
            float fac2 = borderWidth2 / totalLength;
            list0.Add(v0);
            list0.Add(v0 + v03 * fac1);
            list0.Add(v3 - v03 * fac2);
            list0.Add(v3);

            list1.Add(v1);
            list1.Add(v1 + v12 * fac1);
            list1.Add(v2 - v12 * fac2);
            list1.Add(v2);

            for (int i = 0; i < list0.Count - 1; i++)
            {
                int iNext = i + 1;
                Vec3[] face = new Vec3[] { list0[i], list1[i], list1[iNext], list0[iNext] };
                faces.Add(face);
            }
        }
        return faces;
    }

    public static List<Vec3[]> LinearSplitQuad(IList<Vec3> vertices, float maxWidth = 1, int dir = 0)
    {
        List<Vec3[]> faces = new List<Vec3[]>();
        List<Vec3> list0 = new List<Vec3>();
        List<Vec3> list1 = new List<Vec3>();
        Vec3 v0 = vertices[0];
        Vec3 v1 = vertices[1];
        Vec3 v2 = vertices[2];
        Vec3 v3 = vertices[3];

        if (dir == 0)
        {
            Vec3 v01 = v1 - v0;
            Vec3 v32 = v2 - v3;
            float totalLength = v01.magnitude;
            int nSplits = (int)(totalLength / maxWidth)+1;
            float realwidth = totalLength / nSplits;
            float startLength = 0;
            float cLength = startLength;

            while (cLength < totalLength )
            {
                float fac = cLength / totalLength;
                list0.Add(v0 + v01 * fac);
                list1.Add(v3 + v32 * fac);
                cLength += realwidth;
            }
            list0.Add(v1);
            list1.Add(v2);
            for (int i = 0; i < list0.Count - 1; i++)
            {
                int iNext = i + 1;
                Vec3[] face = new Vec3[] { list0[i], list0[iNext], list1[iNext], list1[i] };
                faces.Add(face);
            }
        }
        else
        {
            Vec3 v03 = v3 - v0;
            Vec3 v12 = v2 - v1;
            float totalLength = v03.magnitude;

            float startLength = 0;
            float cLength = startLength;

            int nSplits = (int)(totalLength / maxWidth) + 1;
            float realwidth = totalLength / nSplits;

            while (cLength < totalLength )
            {
                float fac = cLength / totalLength;
                list0.Add(v0 + v03 * fac);
                list1.Add(v1 + v12 * fac);
                cLength += realwidth;
            }
            list0.Add(v3);
            list1.Add(v2);
            for (int i = 0; i < list0.Count - 1; i++)
            {
                int iNext = i + 1;
                Vec3[] face = new Vec3[] { list0[i], list1[i], list1[iNext], list0[iNext] };
                faces.Add(face);
            }
        }
        return faces;
    }
    private static List<Vec3> VerticesFrame(Vec3 v1, Vec3 v2, float w1, float w2)
    {
        Vec3 p1 = UtilsVertex.vertex_between_abs(v1, v2, w1);
        Vec3 p2 = UtilsVertex.vertex_between_abs(v2, v1, w2);
        return new List<Vec3>() { v1, p1, p2, v2 };
    }


    /// <summary>
    /// Extrudes the face straight by distance height.
    /// </summary>
    /// <param name="face_vertices">An array of Vec3 representing a MolaMesh face</param>
    /// <param name="height">The extrusion distance, default 0</param>
    /// <param name="capTop">Toggle if top face (extrusion face) should be created, default True</param>
    /// <returns></returns>
    public static List<Vec3[]> Extrude(Vec3[] face_vertices, float height, bool capTop = true)
    {
        Vec3 normal = UtilsFace.FaceNormal(face_vertices);
        normal *= height;

        List<Vec3> new_vertices = new List<Vec3>();
        foreach (Vec3 v in face_vertices)
        {
            new_vertices.Add(v + normal);
        }

        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();

        for (int i = 0; i < face_vertices.Length; i++)
        {
            Vec3 v0 = face_vertices[i];
            Vec3 v1 = face_vertices[(i + 1) % face_vertices.Length];
            Vec3 v2 = new_vertices[(i + 1) % face_vertices.Length];
            Vec3 v3 = new_vertices[i];

            new_faces_vertices.Add(new Vec3[] { v0, v1, v2, v3 });
        }

        if (capTop)
        {
            new_faces_vertices.Add(new_vertices.ToArray());
        }

        return new_faces_vertices;
    }
    /// <summary>
    /// Extrudes the face to the center point moved by height
    /// normal to the face and creating a triangular face from
    /// each edge to the point.
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static List<Vec3[]> ExtrudeToPointCenter(Vec3[] face_vertices, float height = 0f)
    {
        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();

        Vec3 normal = UtilsFace.FaceNormal(face_vertices);
        normal *= height;

        Vec3 center = UtilsFace.FaceCenter(face_vertices);
        center += normal;

        return FaceSubdivision.ExtrudeToPoint(face_vertices, center);
    }
    /// <summary>
    /// Creates an offset frame with quad corners. Works only with convex shapes.
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static List<Vec3[]> Offset(Vec3[] face_vertices,float offset)
    {
        float[] offsetArray = new float[face_vertices.Length];
        offsetArray = Enumerable.Repeat(offset, face_vertices.Length).ToArray();
        return Offset(face_vertices, offsetArray);
    }
    /// <summary>
    /// Creates an offset frame with quad corners. Works only with convex shapes.
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static List<Vec3[]> Offset(Vec3[] face_vertices, float[] offset)
    {
        List<Vec3> innerVertices = UtilsFace.offset(face_vertices, offset);
        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();
        for (int i = 0; i < face_vertices.Length; i++)
        {
            int iNext = i + 1;
            if (iNext >= face_vertices.Length)
            {
                iNext = 0;
            }
           
            Vec3 v1=face_vertices[i];
            Vec3 v2 = face_vertices[iNext];
            Vec3 v3 = innerVertices[iNext];
            Vec3 v4 = innerVertices[i];
            new_faces_vertices.Add(new Vec3[] {v1,v2,v3,v4 });
        }

        Vec3[] fInner = innerVertices.ToArray();
        new_faces_vertices.Add(fInner);

        return new_faces_vertices;
    }
    /// <summary>
    /// Creates an offset frame with quad corners. Works only with convex shapes.
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static List<Vec3[]> Frame(Vec3[] face_vertices, float w)
    {
        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();
        List<Vec3> innerVertices = new List<Vec3>();
        for (int i = 0; i < face_vertices.Length; i++)
        {
            Vec3 vp;
            if (i == 0)
            {
                vp = face_vertices[face_vertices.Length - 1];
            }
            else
            {
                vp = face_vertices[i - 1];
            }
            Vec3 v = face_vertices[i];
            Vec3 vn = face_vertices[(i + 1) % face_vertices.Length];
            Vec3 vnn = face_vertices[(i + 2) % face_vertices.Length];

            float th1 = UtilsVertex.vertex_angle_triangle(vp, v, vn);
            float th2 = UtilsVertex.vertex_angle_triangle(v, vn, vnn);

            float w1 = w / (float)Math.Sin(th1);
            float w2 = w / (float)Math.Sin(th2);

            List<Vec3> vs1 = VerticesFrame(v, vn, w1, w2);
            List<Vec3> vs2 = VerticesFrame(VerticesFrame(vp, v, w1, w1)[2], VerticesFrame(vn, vnn, w2, w2)[1], w1, w2);

            innerVertices.Add(vs2[1]);

            Vec3[] f1 = new Vec3[] { vs1[0], vs2[0], vs2[1], vs1[1] };
            Vec3[] f2 = new Vec3[] { vs1[1], vs2[1], vs2[2], vs1[2] };
            new_faces_vertices.Add(f1);
            new_faces_vertices.Add(f2);
        }

        Vec3[] fInner = innerVertices.ToArray();
        new_faces_vertices.Add(fInner);

        return new_faces_vertices;
    }
    /// <summary>
    /// Subidivide face into cells with absolute size
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static List<Vec3[]> GridAbs(Vec3[] face_vertices, float x, float y)
    {
        int u = (int)(Vec3.Distance(face_vertices[0], face_vertices[1]) / x);
        int v = (int)(Vec3.Distance(face_vertices[1], face_vertices[2]) / y);
        if (u == 0) u = 1;
        if (v == 0) v = 1;

        return FaceSubdivision.Grid(face_vertices, u, v);
    }
    /// <summary>
    /// splits a triangle, quad or a rectangle into a regular grid
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="nU"></param>
    /// <param name="nV"></param>
    /// <returns></returns>
    public static List<Vec3[]> Grid(Vec3[] face_vertices, int nU, int nV)
    {
        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();
        if (face_vertices.Length == 4)
        {
            List<Vec3> vsU1 = VerticesBetween(face_vertices[0], face_vertices[1], nU);
            List<Vec3> vsU2 = VerticesBetween(face_vertices[3], face_vertices[2], nU);

            List<List<Vec3>> gridVertices = new List<List<Vec3>>();
            for (int i = 0; i < vsU1.Count; i++)
            {
                gridVertices.Add(VerticesBetween(vsU1[i], vsU2[i], nV));
            }

            for (int u = 0; u < vsU1.Count - 1; u++)
            {
                List<Vec3> vs1 = gridVertices[u];
                List<Vec3> vs2 = gridVertices[u + 1];
                for (int v = 0; v < vs1.Count - 1; v++)
                {
                    Vec3[] face = new Vec3[] { vs1[v], vs2[v], vs2[v + 1], vs1[v + 1] };
                    new_faces_vertices.Add(face);
                }
            }
        }

        else if (face_vertices.Length == 3)
        {
            List<Vec3> vsU1 = VerticesBetween(face_vertices[0], face_vertices[1], nU);
            List<Vec3> vsU2 = VerticesBetween(face_vertices[0], face_vertices[2], nU);

            List<List<Vec3>> gridVertices = new List<List<Vec3>>();
            for (int u = 1; u < vsU1.Count; u++)
            {
                gridVertices.Add(VerticesBetween(vsU1[u], vsU2[u], nV));
            }

            Vec3 v0 = face_vertices[0];
            List<Vec3> vs1 = gridVertices[0];

            for (int v = 0; v < vs1.Count - 1; v++)
            {
                Vec3[] face = new Vec3[] { v0, vs1[v], vs1[v + 1] };
                new_faces_vertices.Add(face);
            }
            for (int u = 0; u < gridVertices.Count - 1; u++)
            {
                vs1 = gridVertices[u];
                List<Vec3> vs2 = gridVertices[u + 1];
                for (int v = 0; v < vs1.Count - 1; v++)
                {
                    Vec3[] face = new Vec3[] { vs1[v], vs2[v] ,vs2[v + 1], vs1[v + 1] };
                    new_faces_vertices.Add(face);
                }
            }
        }
        return new_faces_vertices;

    }

    /// <summary>
    /// Extrudes the face to a point by creating a
    /// triangular face from each edge to the point.
    /// </summary>
    /// <param name="face_vertices">The face to be extruded</param>
    /// <param name="point">The point to extrude to</param>
    /// <returns></returns>
    public static List<Vec3[]> ExtrudeToPoint(Vec3[] face_vertices, Vec3 point)
    {
        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();

        int numV = face_vertices.Length;
        for (int i = 0; i < numV; i++)
        {
            Vec3 v1 = face_vertices[i];
            Vec3 v2 = face_vertices[(i + 1) % numV];
            new_faces_vertices.Add(new Vec3[] { v1, v2, point });
        }

        return new_faces_vertices;
    }
    /// <summary>
    /// Extrudes the face tapered like a window by creating an
    /// offset face and quads between every original edge and the
    /// corresponding new edge.
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="height"></param>
    /// <param name="fraction"></param>
    /// <param name="capTop"></param>
    /// <returns></returns>
    public static List<Vec3[]> ExtrudeTapered(Vec3[] face_vertices, float height = 0f, float fraction = 0.5f, bool capTop = true)
    {
        Vec3 center_vertex = UtilsFace.FaceCenter(face_vertices);
        Vec3 normal = UtilsFace.FaceNormal(face_vertices);
        Vec3 scaled_normal = normal * height;

        //# calculate new vertex positions
        List<Vec3> new_vertices = new List<Vec3>();
        for (int i = 0; i < face_vertices.Length; i++)
        {
            Vec3 n1 = face_vertices[i];
            Vec3 betw = center_vertex - n1;
            betw *= fraction;
            Vec3 nn = n1 + betw;
            nn += scaled_normal;
            new_vertices.Add(nn);
        }

        //# create the quads along the edges
        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();
        int num = face_vertices.Length;
        for (int i = 0; i < num; i++)
        {
            Vec3 n1 = face_vertices[i];
            Vec3 n2 = face_vertices[(i + 1) % num];
            Vec3 n3 = new_vertices[(i + 1) % num];
            Vec3 n4 = new_vertices[i];
            Vec3[] new_face_vertices = new Vec3[] { n1, n2, n3, n4 };
            new_faces_vertices.Add(new_face_vertices);
        }

        //# create the closing cap face
        if (capTop)
        {
            Vec3[] cap_face_vertices = new_vertices.ToArray();
            new_faces_vertices.Add(cap_face_vertices);
        }

        return new_faces_vertices;
    }
    /// <summary>
    /// Extrudes a pitched roof
    /// </summary>
    /// <param name="face_vertices"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static List<Vec3[]> Roof(Vec3[] face_vertices, float height = 0f,float gableInsetRelative=0)
    {
        List<Vec3[]> new_faces_vertices = new List<Vec3[]>();

        Vec3 normal = UtilsFace.FaceNormal(face_vertices);
        normal *= height;

        if (face_vertices.Length == 4)
        {
            Vec3 ev1 = UtilsVertex.vertex_center(face_vertices[0], face_vertices[1]);
            ev1 += normal;
            Vec3 ev2 = UtilsVertex.vertex_center(face_vertices[2], face_vertices[3]);
            ev2 += normal;

            Vec3 v12 = (ev2 - ev1)* gableInsetRelative;
            ev1 += v12;
            ev2 -= v12;
            new_faces_vertices.Add(new Vec3[] { face_vertices[0], face_vertices[1], ev1 });
            new_faces_vertices.Add(new Vec3[] { face_vertices[1], face_vertices[2], ev2, ev1 });
            new_faces_vertices.Add(new Vec3[] { face_vertices[2], face_vertices[3], ev2 });
            new_faces_vertices.Add(new Vec3[] { face_vertices[3], face_vertices[0], ev1, ev2 });

            return new_faces_vertices;

        }
        else if (face_vertices.Length == 3)
        {
            Vec3 ev1 = UtilsVertex.vertex_center(face_vertices[0], face_vertices[1]);
            ev1 += normal;
            Vec3 ev2 = UtilsVertex.vertex_center(face_vertices[1], face_vertices[2]);
            ev2 += normal;

            new_faces_vertices.Add(new Vec3[] { face_vertices[0], face_vertices[1], ev1 });
            new_faces_vertices.Add(new Vec3[] { face_vertices[1], ev2, ev1 });
            new_faces_vertices.Add(new Vec3[] { face_vertices[1], face_vertices[2], ev2 });
            new_faces_vertices.Add(new Vec3[] { face_vertices[2], face_vertices[0], ev1, ev2 });

            return new_faces_vertices;
        }
        else
        {
            throw new ArgumentException("face has to be quad or triangle");
        }
    }
}


