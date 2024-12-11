using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace Mola
{
    /// <summary>
    /// A mesh describes a 3D surface made of Vertices connected by Faces.
    /// </summary>
    public class MolaMesh
    {
        List<Vec3> vertices;
        List<int[]> faces;
        private List<Color> vertexColors;
        private List<Vec3> uvs;

        private List<int[]> topoEdges;
        private List<int[]> topoVertexEdges;

        public static int VERTEX1 = 0;
        public static int VERTEX2 = 1;
        public static int FACE1 = 2;
        public static int FACE2 = 3;
        //static int NONE = -1;
        public List<Vec3> UVs { get => uvs; set => uvs = value; }
        /// <summary>
        /// A list of faces. Each face is represented by an array of vertex index. 
        /// </summary>
        public List<int[]> Faces { get => faces; set => faces = value; }
        /// <summary>
        /// A list of Mola Vec3.
        /// </summary>
        public List<Vec3> Vertices { get => vertices; set => vertices = value; }
        /// <summary>
        /// A list of Mola Colors.
        /// </summary>
        public List<Color> Colors { get => vertexColors; set => vertexColors = value; }
        /// <summary>
        /// Create a MolaMesh
        /// </summary>
        /// ### Example
        /// ~~~~~~~~~~~~~~.cs
        /// MolaMesh mesh = New MolaMesh();
        /// ~~~~~~~~~~~~~~
        public MolaMesh()
        {
            vertices = new List<Vec3>();
            faces = new List<int[]>();
            Colors = new List<Color>();
        }
        public string ToJson()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Prevent self-referencing errors
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(this, settings);
        }
        public static MolaMesh FromJson(string json)
        {
            return JsonConvert.DeserializeObject<MolaMesh>(json);
        }
        public Vec3[] CalculateFaceCenters()
        {
            Vec3[] centers = new Vec3[Faces.Count];
            for (int i = 0; i < Faces.Count; i++)
            {
                int[] faceVertices = Faces[i];
                Vec3 center = new Vec3();
                foreach (int j in faceVertices)
                {
                    center += vertices[j];
                }
                centers[i] = center / faceVertices.Count();
            }
            return centers;
        }
        public Vec3[] CalculateNormals()
        {
            Vec3[] normals = new Vec3[Faces.Count];
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            int i4 = 0;
            for (int i = 0; i < Faces.Count; i++)
            {
                int[] faceVertices = Faces[i];
                i1 = faceVertices[0];
                i2 = faceVertices[1];
                i3 = faceVertices[2];
                if (faceVertices.Length == 4)
                {
                    i4 = faceVertices[3];
                    normals[i] = UtilsFace.QuadNormal(vertices[i1], vertices[i2], vertices[i3], vertices[i4]);
                    // normals[i] = UtilsFace.TriangleNormal(vertices[i1], vertices[i2], vertices[i3]);

                }
                else
                {
                    normals[i] = UtilsFace.TriangleNormal(vertices[i1], vertices[i2], vertices[i3]);

                }

            }
            return normals;
        }
        public MolaMesh CopyVertices()
        {
            MolaMesh copyMesh = new MolaMesh();
            foreach (Vec3 vertex in vertices)
            {
                copyMesh.AddVertex(vertex.x, vertex.y, vertex.z);
            }
            copyMesh.vertexColors = this.vertexColors;
            return copyMesh;
        }
        /// <summary>
        /// Make a copy of this MolaMesh
        /// </summary>
        /// <returns>The copyed MolaMesh</returns>
        public MolaMesh Copy()
        {
            MolaMesh copyMesh = new MolaMesh();
            foreach (Vec3 vertex in vertices)
            {
                copyMesh.AddVertex(vertex.x, vertex.y, vertex.z);
            }

            foreach (int[] face in faces)
            {
                copyMesh.AddFace((int[])face.Clone());
            }
            foreach (Color color in vertexColors)
            {
                copyMesh.AddColor(color);
            }
            return copyMesh;

        }
        public void SetVertexColors(Color color)
        {
            for (int i = 0; i < vertexColors.Count; i++)
            {
                vertexColors[i] = color;
            }
        }
        public void SetColorToAllVertices(Color? c = null)
        {
            vertexColors.Clear();
            for (int i = 0; i < vertices.Count; i++)
            {
                vertexColors.Add(c ?? Color.white);
            }
        }
        public void Transform(Matrix4x4 matrix)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
            }
        }
        public void Translate(float x, float y, float z)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                Vec3 v = vertices[i];
                vertices[i] = new Vec3(v.x + x, v.y + y, v.z + z);

            }
        }
        public void Scale(float x, float y, float z)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                Vec3 v = vertices[i];
                vertices[i] = new Vec3(v.x * x, v.y * y, v.z * z);

            }
        }
        public void RotateZ(float radians)
        {
            Rotate(radians * Mathf.Rad2Deg, new Vec3(0, 0, 1));
        }
        public void Rotate(float degrees, Vec3 axis)
        {
            Quaternion quat = Quaternion.AngleAxis(degrees, new Vec3(0, 0, 1));
            //Quaternion quat=Quaternion.Euler(0, 0, degrees);

            for (int i = 0; i < vertices.Count; i++)
            {
                Vec3 v = vertices[i];

                vertices[i] = Rotated(v, quat);


            }
        }
        public MolaMesh MirroredCopy(Plane plane)
        {
            MolaMesh copy = this.Copy();
            Mirror(copy, plane);
            return copy;
        }
        public void Mirror(Plane plane)
        {
            Mirror(this, plane);
        }
        public static void Mirror(MolaMesh mesh, Plane plane)
        {
            for (int i = 0; i < mesh.vertices.Count; i++)
            {
                mesh.vertices[i] = plane.Mirror(mesh.vertices[i]);
            }
            mesh.FlipFaces();
        }
        public static Vec3 Rotated(Vec3 vector, Quaternion rotation, Vec3 pivot = default(Vec3))
        {
            return rotation * (vector - pivot) + pivot;
        }
        public static Vec3 Rotated(Vec3 vector, Vec3 rotation, Vec3 pivot = default(Vec3))
        {
            return Rotated(vector, Quaternion.Euler(rotation), pivot);
        }
        public static Vec3 Rotated(Vec3 vector, float x, float y, float z, Vec3 pivot = default(Vec3))
        {
            return Rotated(vector, Quaternion.Euler(x, y, z), pivot);
        }
        public void RotateRadians(float radians, Vec3 axis)
        {
            Rotate(radians * Mathf.Rad2Deg, axis);
        }
        public void AddMesh(MolaMesh mesh)
        {
            int nV = this.VertexCount();
            for (int i = 0; i < mesh.vertices.Count; i++)
            {
                vertices.Add(mesh.vertices[i]);
                if (i < mesh.Colors.Count)
                {
                    Colors.Add(mesh.Colors[i]);
                }
                else
                {
                    Colors.Add(Color.white);
                }
            }
            if (uvs != null && mesh.UVs != null)
            {
                for (int i = 0; i < mesh.vertices.Count; i++)
                {
                    uvs.Add(mesh.UVs[i]);

                }
            }

            for (int i = 0; i < mesh.faces.Count; i++)
            {
                int[] face = mesh.faces[i];
                int[] newFace = new int[face.Length];
                for (int j = 0; j < face.Length; j++)
                {
                    newFace[j] = face[j] + nV;

                }
                faces.Add(newFace);
            }
        }
        public int AddVertex(float x, float y, float z, Color? c = null)
        {
            vertices.Add(new Vec3(x, y, z));
            //Colors.Add(c ?? Color.white);
            return vertices.Count - 1;
        }
        public int AddVertex(Vec3 v, Color color)
        {
            vertices.Add(v);
            Colors.Add(color);
            return vertices.Count - 1;
        }
        public int AddVertex(float x, float y, float z, Color color)
        {
            vertices.Add(new Vec3(x, y, z));
            Colors.Add(color);
            return vertices.Count - 1;
        }
        public void AddFace(int[] face)
        {
            faces.Add(face);
        }
        public int AddColor(Color color)
        {
            this.Colors.Add(color);
            return Colors.Count - 1;
        }
        public int[] AddVertices(IList<Vec3> vertices, IList<Color> colors)
        {
            int[] vs = new int[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                Vec3 vector = vertices[i];
                vs[i] = this.AddVertex(vector.x, vector.y, vector.z);
                this.AddColor(colors[i]);
            }
            return vs;
        }
        public int[] AddVertices(IList<Vec3> vertices, Color color)
        {
            int[] vs = new int[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                Vec3 vector = vertices[i];
                vs[i] = this.AddVertex(vector.x, vector.y, vector.z);
                this.AddColor(color);
            }
            return vs;
        }
        public int[] AddVertices(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3)
        {
            int[] vs = new int[3];
            vs[0] = this.AddVertex(x1, y1, z1);
            vs[1] = this.AddVertex(x2, y2, z2);
            vs[2] = this.AddVertex(x3, y3, z3);
            return vs;
        }
        public Vec3[] FaceVertices(int faceIndex)
        {
            int[] faceIds = faces[faceIndex];
            Vec3[] vs = new Vec3[faceIds.Length];
            for (int i = 0; i < faceIds.Length; i++)
            {
                vs[i] = vertices[faceIds[i]];

            }
            return vs;
        }
        public int[] AddVertices(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4)
        {
            int[] vs = new int[4];
            vs[0] = this.AddVertex(x1, y1, z1);
            vs[1] = this.AddVertex(x2, y2, z2);
            vs[2] = this.AddVertex(x3, y3, z3);
            vs[3] = this.AddVertex(x4, y4, z4);
            return vs;
        }
        /// <summary>
        /// Get the vertex count of this MolaMesh
        /// </summary>
        /// <returns>The vertex count</returns>
        public int VertexCount()
        {
            return vertices.Count;
        }
        /// <summary>
        /// Get the face count of this MolaMesh
        /// </summary>
        /// <returns>The face count</returns>
        public int FacesCount()
        {
            return faces.Count;
        }
        public void AddTriangle(int index1, int index2, int index3)
        {
            faces.Add(new int[] { index1, index2, index3 });
        }
        public void AddQuad(int index1, int index2, int index3, int index4)
        {
            faces.Add(new int[] { index1, index2, index3, index4 });
        }
        public void FlipFaces()
        {
            for (int i = 0; i < faces.Count; i++)

            {
                int[] face = faces[i];
                int[] face2 = new int[face.Length];

                for (int j = 0; j < face.Length; j++)
                {
                    face2[j] = face[face.Length - 1 - j];

                }
                faces[i] = face2;
            }

        }
        public void FlipYZ()
        {
            for (int i = 0; i < vertices.Count; i++)

            {
                Vec3 v = vertices[i];
                vertices[i] = new Vec3(v.x, v.z, v.y);
            }

        }
        public void AddQuad(Vec3 v1, Vec3 v2, Vec3 v3, Vec3 v4, Color color)
        {
            int[] vs = new int[4];
            vs[0] = this.AddVertex(v1, color);
            vs[1] = this.AddVertex(v2, color);
            vs[2] = this.AddVertex(v3, color);
            vs[3] = this.AddVertex(v4, color);
            faces.Add(vs);
        }
        public void AddTriangle(Vec3 v1, Vec3 v2, Vec3 v3, Color color)
        {
            int[] vs = new int[3];
            vs[0] = this.AddVertex(v1, color);
            vs[1] = this.AddVertex(v2, color);
            vs[2] = this.AddVertex(v3, color);
            faces.Add(vs);
        }
        public void AddFace(Vec3[] vertices, Color? c = null)
        {
            Color color = c ?? Color.white;

            if (vertices.Length == 3) AddTriangle(vertices[0], vertices[1], vertices[2], color);
            else if (vertices.Length == 4) AddQuad(vertices[0], vertices[1], vertices[2], vertices[3], color);
            else return;
        }
        public void AddFaces(List<Vec3[]> faces_vertices, Color? c = null)
        {
            Color color = c ?? Color.white;
            foreach (var face_vertices in faces_vertices)
            {
                try
                {
                    AddFace(face_vertices);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public void AddTri2D(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            AddTri2D(x1, y1, x2, y2, x3, y3, 0, Color.white);
        }
        public void AddTri2D(float x1, float y1, float x2, float y2, float x3, float y3, float z)
        {
            AddTri2D(x1, y1, x2, y2, x3, y3, z, Color.white);

        }
        public void AddTri2D(float x1, float y1, float x2, float y2, float x3, float y3, float z, Color color)
        {
            int[] vs = new int[3];
            vs[0] = this.AddVertex(x1, y1, z, color);
            vs[1] = this.AddVertex(x2, y2, z, color);
            vs[2] = this.AddVertex(x3, y3, z, color);
            faces.Add(vs);
        }
        public void AddTriangle(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, Color color1, Color color2, Color color3)
        {
            int[] vs = new int[3];
            vs[0] = this.AddVertex(x1, y1, z1, color1);
            vs[1] = this.AddVertex(x2, y2, z2, color2);
            vs[2] = this.AddVertex(x3, y3, z3, color3);
            faces.Add(vs);
        }
        public void AddTri2D(float x1, float y1, float x2, float y2, float x3, float y3, float z, Color color1, Color color2, Color color3)
        {
            int[] vs = new int[3];
            vs[0] = this.AddVertex(x1, y1, z, color1);
            vs[1] = this.AddVertex(x2, y2, z, color2);
            vs[2] = this.AddVertex(x3, y3, z, color3);
            faces.Add(vs);
        }
        public void AddQuad2D(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float z, Color color)
        {
            int[] vs = new int[4];
            vs[0] = this.AddVertex(x1, y1, z, color);
            vs[1] = this.AddVertex(x2, y2, z, color);
            vs[2] = this.AddVertex(x3, y3, z, color);
            vs[3] = this.AddVertex(x4, y4, z, color);
            faces.Add(vs);
        }
        public void SeparateVerticesWithUVs()
        {
            List<Vec3> oldVertices = new List<Vec3>(vertices);
            List<Vec3> oldUVS = new List<Vec3>(uvs);
            List<Color> listColors = new List<Color>(Colors);
            this.vertices.Clear();
            this.Colors.Clear();
            this.uvs.Clear();
            int vertexIndex = 0;
            foreach (int[] face in faces)
            {
                for (int i = 0; i < face.Length; i++)
                {
                    vertexIndex = face[i];
                    this.vertices.Add(oldVertices[vertexIndex]);
                    this.Colors.Add(listColors[vertexIndex]);
                    this.uvs.Add(oldUVS[vertexIndex]);
                    face[i] = this.vertices.Count - 1;
                }
            }
        }
        public void SeparateVertices()
        {
            List<Vec3> oldVertices = new List<Vec3>(vertices);
            List<Color> listColors = new List<Color>(Colors);
            this.vertices.Clear();
            this.Colors.Clear();
            foreach (int[] face in faces)
            {
                for (int i = 0; i < face.Length; i++)
                {

                    Vec3 v = oldVertices[face[i]];

                    this.vertices.Add(v);
                    if (listColors.Count > face[i])
                    {
                        this.Colors.Add(listColors[face[i]]);
                    }

                    face[i] = this.vertices.Count - 1;
                }
            }

        }
        public void TriangulateQuads()
        {
            List<int[]> triangles = new List<int[]>();
            foreach (int[] face in faces)
            {
                if (face.Length == 3)
                {
                    triangles.Add(face);
                }
                if (face.Length == 4)
                {
                    triangles.Add(new int[] { face[0], face[1], face[2] });
                    triangles.Add(new int[] { face[2], face[3], face[0] });
                }
            }
            this.faces = triangles;
        }
        public Vec3[] VertexArray()
        {
            return vertices.ToArray();
        }
        public int[] FlattenedTriangles()
        {
            TriangulateQuads();
            int[] indices = new int[faces.Count * 3];
            int index = 0;
            foreach (int[] face in faces)
            {
                indices[index] = face[0];
                index++;
                indices[index] = face[1];
                index++;
                indices[index] = face[2];
                index++;
            }
            return indices;
        }
        public void UpdateTopology()
        {
            // TODO Auto-generated method stub
            topoEdges = new List<int[]>();
            topoVertexEdges = new List<int[]>(vertices.Count);
            for (int i = 0; i < vertices.Count; i++)
            {
                topoVertexEdges.Add(new int[] { });
            }
            for (int faceIndex = 0; faceIndex < faces.Count; faceIndex++)
            {
                int[] face = faces[faceIndex];
                int nPts = face.Length;
                for (int j = 0; j < nPts; j++)
                {
                    int v1Index = face[j];
                    int v2Index = face[(j + 1) % nPts];
                    int edgeIndex = AddEdge(v1Index, v2Index);
                    // this only works for clean meshes
                    AttachFaceToEdge(edgeIndex, v1Index, faceIndex);
                }
            }
        }
        public void WeldVertices()
        {
            List<Vec3> welded_vertices = new List<Vec3>();
            Dictionary<Vec3, int> vertex_lookup = new Dictionary<Vec3, int>();
            foreach (int[] face in Faces)
            {
                for (int i = 0; i < face.Length; i++)
                {
                    Vec3 vertex = Vertices[face[i]];
                    int value;
                    if (vertex_lookup.TryGetValue(vertex, out value))
                    {
                        face[i] = value;
                    }
                    else
                    {
                        value = welded_vertices.Count;
                        welded_vertices.Add(vertex);
                        face[i] = value;
                        vertex_lookup.Add(vertex, value);
                    }
                }
            }
            Vertices = welded_vertices;
            Colors = Enumerable.Repeat(Color.white, Vertices.Count).ToList();
        }
        private void AttachEdgeToVertex(int vertexIndex, int edgeIndex)
        {
            int[] edges = topoVertexEdges[vertexIndex];
            int[] newEdges = new int[edges.Length + 1];
            for (int i = 0; i < edges.Length; i++)
            {
                newEdges[i] = edges[i];
            }
            newEdges[edges.Length] = edgeIndex;
            topoVertexEdges[vertexIndex] = newEdges;
        }
        private void AttachFaceToEdge(int edgeIndex, int vertexIndex, int faceIndex)
        {
            int[] edge = topoEdges[edgeIndex];
            if (edge[0] == vertexIndex)
            {
                edge[FACE1] = faceIndex;
            }
            else if (edge[1] == vertexIndex)
            {
                edge[FACE2] = faceIndex;
            }
        }
        private int AddEdge(int v1, int v2)
        {
            int existingEdge = AdjacentEdgeToVertices(v1, v2);
            if (existingEdge >= 0)
            {
                return existingEdge;
            }
            int[] edgeVs = new int[] { v1, v2, -1, -1 };
            topoEdges.Add(edgeVs);
            int edgeIndex = topoEdges.Count - 1;
            AttachEdgeToVertex(v1, edgeIndex);
            AttachEdgeToVertex(v2, edgeIndex);
            return edgeIndex;
        }
        public int AdjacentEdgeToVertices(int v1, int v2)
        {
            int[] vEdges = topoVertexEdges[v1];
            for (int i = 0; i < vEdges.Length; i++)
            {
                int edgeIndex = vEdges[i];
                int[] edge = topoEdges[edgeIndex];
                if (edge[VERTEX1] == v2 || edge[VERTEX2] == v2)
                {
                    return edgeIndex;
                }
            }
            return -1;
        }
        public int AdjacentFacetoVertices(int v1, int v2)
        {
            int edgeIndex = AdjacentEdgeToVertices(v1, v2);
            int[] edge = topoEdges[edgeIndex];
            if (edge[0] == v1) return topoEdges[edgeIndex][FACE1];
            if (edge[1] == v1) return topoEdges[edgeIndex][FACE2];
            return -1;
        }
        public int[] AdjacentFacesToFace(int faceIndex)
        {
            int[] face = faces[faceIndex];
            int nPts = face.Length;
            int[] faceNbs = new int[nPts];
            for (int j = 0; j < nPts; j++)
            {
                int v1Index = face[j];
                int v2Index = face[(j + 1) % nPts];
                faceNbs[j] = AdjacentFacetoVertices(v1Index, v2Index);
            }
            return faceNbs;
        }
        public int[] AdjacentVerticesToVertex(int vertexIndex)
        {
            int[] vEdges = topoVertexEdges[vertexIndex];
            int[] nbVertices = new int[vEdges.Length];
            for (int i = 0; i < vEdges.Length; i++)
            {
                int[] eVertices = topoEdges[vEdges[i]];
                if (eVertices[VERTEX1] == vertexIndex)
                {
                    nbVertices[i] = eVertices[1];
                }
                else if (eVertices[VERTEX2] == vertexIndex)
                {
                    nbVertices[i] = eVertices[0];
                }
            }
            return nbVertices;
        }
        public int[] AdjacentFacesToVertex(int vertexIndex)
        {
            int[] vEdges = topoVertexEdges[vertexIndex];
            int[] allFaceIds = new int[vEdges.Length];
            int nFaces = 0;
            for (int i = 0; i < vEdges.Length; i++)
            {
                int edgeIndex = vEdges[i];
                int faceIndex = -1;

                if (topoEdges[edgeIndex][0] == vertexIndex)
                {
                    faceIndex = this.AdjacentFace1ToEdge(edgeIndex);
                }
                else
                {
                    faceIndex = this.AdjacentFace2ToEdge(edgeIndex);
                }
                allFaceIds[i] = faceIndex;
                if (faceIndex >= 0)
                {
                    nFaces++;
                }
            }
            int[] faceIds = new int[nFaces];
            int index = 0;
            for (int i = 0; i < allFaceIds.Length; i++)
            {
                if (allFaceIds[i] >= 0)
                {
                    faceIds[index] = allFaceIds[i];
                    index++;
                }
            }
            return faceIds;
        }
        public int[] AdjacentEdgesToEdge(int edgeIndex)
        {
            int v1 = AdjacentVertex1ToEdge(edgeIndex);
            int v2 = AdjacentVertex2ToEdge(edgeIndex);
            int[] vEdges1 = topoVertexEdges[v1];
            int[] vEdges2 = topoVertexEdges[v2];
            int[] nbEdges = new int[vEdges1.Length + vEdges2.Length - 2];
            int index = 0;
            for (int i = 0; i < vEdges1.Length; i++)
            {
                nbEdges[index] = vEdges1[i];
                index++;
            }
            for (int i = 0; i < vEdges2.Length; i++)
            {
                nbEdges[index] = vEdges2[i];
                index++;
            }

            return nbEdges;
        }
        public int AdjacentVertex1ToEdge(int edgeIndex)
        {
            return topoEdges[edgeIndex][VERTEX1];
        }
        public int AdjacentVertex2ToEdge(int edgeIndex)
        {
            return topoEdges[edgeIndex][VERTEX2];
        }
        public int AdjacentFace1ToEdge(int edgeIndex)
        {
            return topoEdges[edgeIndex][FACE1];
        }
        public int AdjacentFace2ToEdge(int edgeIndex)
        {
            return topoEdges[edgeIndex][FACE2];
        }
        public ReadOnlyCollection<int[]> GetTopoEdges()
        {
            return new ReadOnlyCollection<int[]>(topoEdges);
        }
        public ReadOnlyCollection<int[]> GetTopoVertexEdges()
        {
            return new ReadOnlyCollection<int[]>(topoVertexEdges);
        }
        public void RemoveUnusedVertices()
        {
            int[] newIndices = new int[this.VertexCount()];
            newIndices = Enumerable.Repeat(-1, this.VertexCount()).ToArray();
            List<Vec3> newVertices = new List<Vec3>();
            List<Color> newVertexColors = new List<Color>();
            foreach (int[] face in faces)
            {
                for (int i = 0; i < face.Count(); i++)
                {
                    int vertexIndex = face[i];
                    int newVertexIndex = newIndices[vertexIndex];
                    if (newVertexIndex == -1)
                    {
                        newVertexIndex = newVertices.Count;
                        newVertices.Add(vertices[vertexIndex]);
                        newVertexColors.Add(this.vertexColors[vertexIndex]);
                        newIndices[vertexIndex] = newVertexIndex;
                    }
                    face[i] = newVertexIndex;
                }
            }
            this.vertices = newVertices;
            this.Colors = newVertexColors;
        }
        public MolaMesh CopySubMesh(List<int> faceIds, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            if (invert == false)
            {
                for (int i = 0; i < faceIds.Count; i++)
                {
                    newMesh.AddFace(CopyFace(faceIds[i]));
                }
                newMesh.RemoveUnusedVertices();
                return newMesh;
            }
            else
            {
                bool[] mask = new bool[faces.Count];
                mask = Enumerable.Repeat(true, faces.Count).ToArray();
                for (int i = 0; i < faceIds.Count; i++)
                {
                    mask[faceIds[i]] = false;

                }
                return CopySubMesh(mask);

            }

        }
        public MolaMesh CopySubMeshByModulo(int result, int modulo, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                if (!invert)
                {
                    if (i % modulo == result)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }
                else
                {
                    if (i % modulo != result)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }


            }
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMeshByEdgeLength(float min, float max, int edgeIndex = 0, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                int[] face = faces[i];
                int e1 = edgeIndex % face.Length;
                int e2 = (edgeIndex + 1) % face.Length;
                Vec3 v1 = vertices[face[e1]];
                Vec3 v2 = vertices[face[e2]];
                float mag = (v2 - v1).magnitude;

                if (mag >= min && mag <= max)
                {
                    if (!invert)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }
                else
                {
                    if (invert)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }

            }
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMeshByBoundingBox(float x1, float y1, float z1, float x2, float y2, float z2, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                BoundingBox bounds = FaceBoundingBox(i);

                if (bounds.X1() >= x1 && bounds.X2() <= x2 && bounds.Y1() >= y1 && bounds.Y2() <= y2 && bounds.Z1() >= z1 && bounds.Z2() <= z2)
                {
                    if (!invert)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }
                else
                {
                    if (invert)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }

            }
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMeshByDimension(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                BoundingBox bounds = FaceBoundingBox(i);
                float dX = bounds.GetDimX();
                float dY = bounds.GetDimY();
                float dZ = bounds.GetDimZ();
                //Debug.Log(dX);

                if (dX >= minX && dX <= maxX && dY >= minY && dY <= maxY && dZ >= minZ && dZ <= maxZ)
                {
                    if (!invert)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }
                else
                {
                    if (invert)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }

            }
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMeshByNormalZ(float minZ, float maxZ, bool abs = false, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                Vec3 normal = FaceNormal(i);
                float z = normal.z;
                if (abs)
                {
                    z = Mathf.Abs(z);
                }

                if (z >= minZ && z <= maxZ)
                {
                    if (!invert)
                    {
                        newMesh.AddFace(this.CopyFace(i));
                    }

                }
                else
                {
                    if (invert)
                    {
                        newMesh.AddFace(this.CopyFace(i));
                    }
                }

            }

            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMeshByNormalX(float minX, float maxX, bool abs = false, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                Vec3 normal = FaceNormal(i);
                float x = normal.x;
                if (abs)
                {
                    x = Mathf.Abs(x);
                }
                if (!invert)
                {
                    if (x >= minX && x <= maxX)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }
                else
                {
                    if (x < minX || x > maxX)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }

            }

            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMeshByNormalY(float minY, float maxY, bool abs = false, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                Vec3 normal = FaceNormal(i);
                float y = normal.y;
                if (abs)
                {
                    y = Mathf.Abs(y);
                }
                if (!invert)
                {
                    if (y >= minY && y <= maxY)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }
                else
                {
                    if (y < minY || y > maxY)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }

            }

            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMeshByFaceVertexCount(int vertexC = 3)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < faces.Count; i++)
            {
                if (this.faces[i].Length == vertexC)
                {
                    newMesh.AddFace(CopyFace(i));
                }
            }
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMesh(bool[] mask)
        {
            MolaMesh newMesh = this.CopyVertices();
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i])
                {
                    newMesh.AddFace(CopyFace(i));
                }
            }
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public MolaMesh CopySubMesh(Predicate<int[]> faceFilter)
        {
            MolaMesh newMesh = this.CopyVertices();
            newMesh.faces = faces.FindAll(faceFilter);
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public int[] CopyFace(int i)
        {
            int l = faces[i].Length;
            int[] copyFace = new int[l];

            Array.Copy(faces[i], copyFace, l);
            return copyFace;
        }
        public MolaMesh CopySubMesh(int faceId, bool invert = false)
        {
            MolaMesh newMesh = this.CopyVertices();
            if (invert)
            {
                for (int i = 0; i < faces.Count; i++)
                {
                    if (i != faceId)
                    {
                        newMesh.AddFace(CopyFace(i));
                    }
                }
            }
            else
            {
                newMesh.AddFace(CopyFace(faceId));
            }
            newMesh.RemoveUnusedVertices();
            return newMesh;
        }
        public List<float> FaceProperties(Func<Vec3[], float> analyse)
        {
            List<float> values = new List<float>();
            foreach (int[] face in Faces)
            {
                values.Add(analyse(UtilsVertex.face_vertices(this, face)));
            }
            return values;
        }
        public void Clear()
        {
            this.vertices = new List<Vec3>();
            this.faces = new List<int[]>();
            this.Colors = new List<Color>();
            this.uvs = new List<Vec3>();
        }
        public Vec3 FaceNormal(int faceIndex)
        {
            return UtilsFace.FaceNormal(FaceVertices(faceIndex));
        }
        public BoundingBox FaceBoundingBox(int faceIndex)
        {
            return UtilsFace.FaceBoundingBox(FaceVertices(faceIndex));
        }
        public float FacePerimeter(int faceIndex)
        {
            return UtilsFace.FacePerimeter(FaceVertices(faceIndex));
        }
        public float FaceEdgeLength(int faceIndex, int direction)
        {
            return UtilsFace.FaceEdgeLength(FaceVertices(faceIndex), direction);
        }
        public float FaceArea(int faceIndex)
        {
            return UtilsFace.FaceAreaTriOrQuad(FaceVertices(faceIndex));
        }
        public Vec3 FaceCenter(int faceIndex)
        {
            return UtilsFace.FaceCenter(FaceVertices(faceIndex));
        }
        public float FaceAngleVertical(int faceIndex)
        {
            return UtilsFace.FaceAngleVertical(FaceVertices(faceIndex));
        }
        public float FaceAngleHorizontal(int faceIndex)
        {
            return UtilsFace.FaceAngleHorizontal(FaceVertices(faceIndex));
        }
        public float FaceCompactness(int faceIndex)
        {
            return UtilsFace.FaceCompactness(FaceVertices(faceIndex));
        }
        public float FaceProportion(int faceIndex)
        {
            return UtilsFace.FaceProportion(FaceVertices(faceIndex));
        }
    }
}