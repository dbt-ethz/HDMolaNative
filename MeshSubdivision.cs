using System;
using System.Collections.Generic;
using System.Linq;

namespace Mola
{
    public class MeshSubdivision
    {
        /// <summary>
        /// Apply CatmullClark algorithm to a MolaMesh
        /// </summary>
        /// <param name="mesh">A MolaMesh</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh CatmullClark(MolaMesh mesh)
        {
            return SubdivisionCatmull.Subdivide(mesh);
        }
        public static List<Vec3[]> SubdivideFaceExtrude(MolaMesh molaMesh, int[] face, float height, bool capTop = true)
        {

            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(face_vertices, height, capTop);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceExtrude(Vec3[] face_vertices, float height, bool capTop = true)
        {
            return FaceSubdivision.Extrude(face_vertices, height, capTop);
        }
        public static MolaMesh SubdivideMeshExtrude(MolaMesh molaMesh, float height, bool capTop=true)
        {
            return Extrude(molaMesh, height);
        }
        /// <summary>
        /// Extrudes the all faces in a MolaMesh straight by a single distance height.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="height">Extruding height</param>
        /// <param name="capTop">Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Extrude(MolaMesh molaMesh, float height, bool capTop=true)
        {
            MolaMesh newMesh = new();
            foreach (var face in molaMesh.Faces)
            {
                List<Vec3[]> new_faces_vertices = MeshSubdivision.SubdivideFaceExtrude(molaMesh, face, height, capTop);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshExtrude(MolaMesh molaMesh, List<float> heights, List<bool> capTops)
        {
            return Extrude(molaMesh, heights, capTops);
        }
        /// <summary>
        /// Extrudes the all faces in a MolaMesh straight by a list distance height. The list length must much the face count.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="height">A list Extruding height</param>
        /// <param name="capTop">A list of bool to decide Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Extrude(MolaMesh molaMesh, List<float> heights, List<bool> capTops)
        {
            if(heights.Count != molaMesh.FacesCount() || capTops.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list counts doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(molaMesh.FaceVertices(i), heights[i], capTops[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static List<Vec3[]> SubdivideFaceGrid(MolaMesh molaMesh, int[] face, int nU, int nV)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Grid(face_vertices, nU, nV);

            return new_faces_vertices;
        }
        /// <summary>
        /// Split a triangle or quad face into a regular grid by u and v resolutions. 
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <param name="nU"></param>
        /// <param name="nV"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceGrid(Vec3[] face_vertices, int nU, int nV)
        {
            return FaceSubdivision.Grid(face_vertices, nU, nV);
        }
        public static MolaMesh SubdivideMeshGrid(MolaMesh molaMesh, int nU, int nV)
        {
            return Grid(molaMesh, nU, nV);
        }
        /// <summary>
        /// Split each face in a MolaMesh into a regular grid 
        /// by specifying individual u and v resolutions for each face.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="nUList"></param>
        /// <param name="nVList"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshGrid(MolaMesh molaMesh, List<int> nUList, List<int> nVList)
        {
            return Grid(molaMesh, nUList, nVList);
        }
        /// <summary>
        /// splits all triangle or quad faces in a MolaMesh into regular grids
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="nU">Division count on U direction</param>
        /// <param name="nV">Devision count on V direction</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Grid(MolaMesh molaMesh, List<int> nUList, List<int> nVList)
        {
            if (nUList.Count != molaMesh.FacesCount() || nVList.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list counts doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Grid(molaMesh.FaceVertices(i), nUList[i], nVList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Splits all triangle or quad faces in a MolaMesh into regular grids
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="nU">A list of int U</param>
        /// <param name="nV">A list of int V</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Grid(MolaMesh molaMesh, int nU, int nV)
        {
            MolaMesh newMesh = new MolaMesh();
            foreach (var face in molaMesh.Faces) 
            {
                List<Vec3[]> new_faces_vertices = SubdivideFaceGrid(molaMesh, face, nU, nV);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        public static MolaMesh SplitRelative(MolaMesh mesh, int startSplit, float minSplit1, float maxSplit1, float minSplit2, float maxSplit2)
        {
            return Relative(mesh, startSplit, minSplit1, maxSplit1, minSplit2, maxSplit2);
        }
        /// <summary>
        /// Split all faces in a MolaMesh based on relative parameters 
        /// </summary>
        /// <param name="mesh">A MolaMesh</param>
        /// <param name="startSplit">Choose U or V as starting direction</param>
        /// <param name="minSplit1">min relative parameter on the first direction</param>
        /// <param name="maxSplit1">max relative parameter on the first direction</param>
        /// <param name="minSplit2">min relative parameter on the second direction</param>
        /// <param name="maxSplit2">max relative parameter on the second direction</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Relative(MolaMesh mesh, int startSplit, float minSplit1, float maxSplit1, float minSplit2, float maxSplit2)
        {
            MolaMesh newMesh = mesh.CopyVertices();
            Random rnd = new Random();
            foreach (var face in mesh.Faces) //list of index
            {
                int iv0 = face[startSplit % face.Length];
                int iv1 = face[(startSplit + 1) % face.Length];
                int iv2 = face[(startSplit + 2) % face.Length];
                int iv3 = face[(startSplit + 3) % face.Length];

                Vec3 v0 = newMesh.Vertices[iv0];
                Vec3 v1 = newMesh.Vertices[iv1];
                Vec3 v2 = newMesh.Vertices[iv2];
                Vec3 v3 = newMesh.Vertices[iv3];

                float split1 = (float)rnd.NextDouble() * (maxSplit1 - minSplit1) + minSplit1;
                float split2 = (float)rnd.NextDouble() * (maxSplit2 - minSplit2) + minSplit2;

                Vec3 splitPt1 = (v1 - v0) * split1 + v0;
                Vec3 splitPt2 = (v3 - v2) * split2 + v2;

                int s1 = newMesh.AddVertex(splitPt1.x, splitPt1.y, splitPt1.z);
                int s2 = newMesh.AddVertex(splitPt2.x, splitPt2.y, splitPt2.z);

                // add Quad
                int[] face1 = new int[] { s2, s1, iv1, iv2 };

                List<int> face2 = new()
                {
                    s1,
                    s2
                };

                for (int i = startSplit + 3; i < startSplit + 3 + face.Length - 2; i++)
                {
                    int index = i % face.Length;
                    face2.Add(face[index]);
                }

                newMesh.AddFace(face1);
                newMesh.AddFace(face2.ToArray());
            }
            newMesh.Colors = Enumerable.Repeat(Color.white, newMesh.VertexCount()).ToList();

            return newMesh;

        }
        public static List<Vec3[]> SubdivideFaceExtrudeTapered(MolaMesh molaMesh, int[] face, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeTapered(face_vertices, height, fraction, capTop);

            return new_faces_vertices;
        }
        /// <summary>
        /// Extrude a face tapered like a window by creating an
        /// offset face and quads between each pair of 
        /// the original edge and the corresponding offset edge.
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <param name="height"></param>
        /// <param name="fraction"></param>
        /// <param name="capTop"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceExtrudeTapered(Vec3[] face_vertices, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            return FaceSubdivision.ExtrudeTapered(face_vertices, height, fraction, capTop);
        }
        /// <summary>
        /// Extrude each face in a MolaMesh tapered like a window by 
        /// creating an offset face and quads between each pair of 
        /// the original edge and the corresponding offset edge.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="height"></param>
        /// <param name="fraction"></param>
        /// <param name="capTop"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshExtrudeTapered(MolaMesh molaMesh, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            return ExtrudeTapered(molaMesh, height, fraction, capTop);
        }
        /// <summary>
        /// Extrudes all face in a MolaMesh tapered like a window by
        /// creating an offset face and quads between every original 
        /// edge and the corresponding new edge.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="height">Extruding height</param>
        /// <param name="fraction">Relative value of how much the result is tapered</param>
        /// <param name="capTop">Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh ExtrudeTapered(MolaMesh molaMesh, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            MolaMesh newMesh = new();
            foreach (var face in molaMesh.Faces)
            {
                List<Vec3[]> new_faces_vertices = SubdivideFaceExtrudeTapered(molaMesh, face, height, fraction, capTop);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Split each face in a quad MolaMesh into three quads in one direction 
        /// by specifying the range to generate random widths of the first two segments.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="borderWidth1"></param>
        /// <param name="borderWidth2"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static MolaMesh LinearSplitBorder(MolaMesh molaMesh, float borderWidth1 = 1f, float borderWidth2 = 1, int dir = 0)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.LinearSplitQuadBorder(molaMesh.FaceVertices(i), borderWidth1, borderWidth2, dir);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Split each face in a quad MolaMesh into three quads in one direction 
        /// by specifying the range to generate random widths of the first two segments.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="minSplitWidth"></param>
        /// <param name="maxSplitWidth"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static MolaMesh LinearSplitQuad(MolaMesh molaMesh, float minSplitWidth = 0f, float maxSplitWidth = 0.5f, int dir = 0)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.LinearSplitQuad(molaMesh.FaceVertices(i), minSplitWidth, maxSplitWidth, dir);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Split each face in a quad MolaMesh into three quads in one direction 
        /// by specifying the max width of the segments.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="maxSplitWidth"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static MolaMesh LinearSplitQuad(MolaMesh molaMesh, float maxSplitWidth = 1f, int dir = 0)
        {
            //if (meshCenter = new MolaMesh();
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.LinearSplitQuad(molaMesh.FaceVertices(i), maxSplitWidth, dir);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
                /*if (meshCenter != null) {
                    meshCenter.AddFace(new_faces_vertices[1]);
                }*/
            }
            return newMesh;
        }
        /// <summary>
        /// Extrudes all face in a MolaMesh tapered like a window by
        /// creating an offset face and quads between every original 
        /// edge and the corresponding new edge.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="heights">A list Extruding height</param>
        /// <param name="fractions">A list of relative values</param>
        /// <param name="capTops">A list of bool to decide Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh ExtrudeTapered(MolaMesh molaMesh, List<float> heights, List<float> fractions, List<bool> capTops)
        {
            if (heights.Count != molaMesh.FacesCount() || capTops.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list counts doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = SubdivideFaceExtrudeTapered(molaMesh, molaMesh.Faces[i], heights[i], fractions[i], capTops[i]);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        public static List<Vec3[]> SubdivideFaceSplitRoof(MolaMesh molaMesh, int[] face, float height = 0f)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Roof(face_vertices, height);

            return new_faces_vertices;
        }
        /// <summary>
        /// Extrude a face into a pitched roof by an extrusion height.
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceSplitRoof(Vec3[] face_vertices, float height = 0f)
        {
            return FaceSubdivision.Roof(face_vertices, height);
        }
        public static MolaMesh SubdivideMeshSplitRoof(MolaMesh molaMesh, float height = 0f)
        {
            return SplitRoof(molaMesh, height);
        }
        /// <summary>
        /// Extrudes all faces in a MolaMesh into pitched rooves
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="height">Extruding height</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh SplitRoof(MolaMesh molaMesh, float height = 0f)
        {
            MolaMesh newMesh = new();
            foreach (var face in molaMesh.Faces)
            {
                List<Vec3[]> new_faces_vertices = SubdivideFaceSplitRoof(molaMesh, face, height);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshSplitRoof(MolaMesh molaMesh, List<float> heightList)
        {
            return SplitRoof(molaMesh, heightList);
        }
        /// <summary>
        /// Extrude each face in a MolaMesh into pitched rooves 
        /// by specifying individual extrusion heights for each face.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="heightList">The list of extruding height</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh SplitRoof(MolaMesh molaMesh, List<float> heightList)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Roof(molaMesh.FaceVertices(i), heightList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static List<Vec3[]> SubdivideFaceExtrudeToPoint(MolaMesh molaMesh, int[] face, Vec3 point)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPoint(face_vertices, point);

            return new_faces_vertices;
        }
        /// <summary>
        /// Extrude a face to a point by creating
        /// triangular faces from each edge to the point.
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceExtrudeToPoint(Vec3[] face_vertices, Vec3 point)
        {
            return FaceSubdivision.ExtrudeToPoint(face_vertices, point);
        }
        public static List<Vec3[]> SubdivideFaceExtrudeToPointCenter(MolaMesh molaMesh, int[] face, float height = 0f)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPointCenter(face_vertices, height);

            return new_faces_vertices;
        }
        /// <summary>
        /// Extrude a face to a new point offset from its center 
        /// by a distance along the normal vector of the face and 
        /// create triangular faces from each edge to the point.
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceExtrudeToPointCenter(Vec3[] face_vertices, float height = 0f)
        {
            return FaceSubdivision.ExtrudeToPointCenter(face_vertices, height);
        }
        /// <summary>
        /// Extrude each face in a MolaMesh to a new point offset from its center 
        /// by a distance along the normal vector of the face and 
        /// create triangular faces from each edge to the point.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="height">Extruding height</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh ExtrudeToPointCenter(MolaMesh molaMesh, float height = 0f)
        {
            MolaMesh newMesh = new();
            foreach (var face in molaMesh.Faces)
            {
                List<Vec3[]> new_faces_vertices = SubdivideFaceExtrudeToPointCenter(molaMesh, face, height);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshExtrudeToPointCenter(MolaMesh molaMesh, float height = 0f)
        {
            return ExtrudeToPointCenter(molaMesh, height);
        }
        /// <summary>
        /// Extrudes each face in a MolaMesh to the center point moved 
        /// by height normal to the face and creating a triangular face
        /// from each edge to the point.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="heightList">A list of extruding height</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh ExtrudeToPointCenter(MolaMesh molaMesh, List<float> heightList)
        {
            if (heightList.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list counts doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPointCenter(molaMesh.FaceVertices(i), heightList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshExtrudeToPointCenter(MolaMesh molaMesh, List<float> heightList)
        {
            return ExtrudeToPointCenter(molaMesh, heightList);
        }
        public static List<Vec3[]> SubdivideFaceSplitFrame(MolaMesh molaMesh, int[] face, float w)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Frame(face_vertices, w);

            return new_faces_vertices;
        }
        /// <summary>
        /// Create an offset frame with quad corners from a face. 
        /// Only work for convex shapes.
        /// </summary>
        /// <param name="face_vertices"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceSplitFrame(Vec3[] face_vertices, float w)
        {
            return FaceSubdivision.Frame(face_vertices, w);
        }
        /// <summary>
        /// Create an offset frame with quad corners from each face in a Molamesh. 
        /// Only work for convex shapes.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="w">The relative value</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh SplitFrame(MolaMesh molaMesh, float w)
        {
            MolaMesh newMesh = new();
            foreach (var face in molaMesh.Faces)
            {
                List<Vec3[]> new_faces_vertices = SubdivideFaceSplitFrame(molaMesh, face, w);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshSplitFrame(MolaMesh molaMesh, List<float> wList)
        {
            return SplitFrame(molaMesh, wList);
        }
        /// <summary>
        /// Create an offset frame with quad corners from each face in a Molamesh by specifying individual offset distances. 
        /// Only work for convex shapes.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="wList">A list of relative value</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh SplitFrame(MolaMesh molaMesh, List<float> wList)
        {
            if (wList.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list counts doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Frame(molaMesh.FaceVertices(i), wList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Offset each face in a MolaMesh by a distance. 
        /// Only work for convex shapes.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="offset">The offset distance</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh SplitOffset(MolaMesh molaMesh, float offset)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Offset(molaMesh.FaceVertices(i), offset);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Offset each face in a MolaMesh by specifying individual distances for each face. 
        /// Only work for convex shapes.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="offsetList">A list of offset distance</param>
        /// <returns></returns>
        public static MolaMesh SplitOffset(MolaMesh molaMesh, IList<float> offsetList)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Offset(molaMesh.FaceVertices(i), offsetList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshOffsetPerEdge(MolaMesh molaMesh, IList<float[]> offset)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Offset(molaMesh.FaceVertices(i), offset[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshOffsetPerEdge(MolaMesh molaMesh, float[]offset)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Offset(molaMesh.FaceVertices(i), offset[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static List<Vec3[]> SubdivideFaceSplitGridAbs(MolaMesh molaMesh, int[] face, float x, float y)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.GridAbs(face_vertices, x, y);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceSplitGridAbs(Vec3[] face_vertices, float x, float y)
        {
            return FaceSubdivision.GridAbs(face_vertices, x, y);
        }
        /// <summary>
        /// Subidivide each face in a MolaMesh into cells with absolute size
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="x">Size on U direction</param>
        /// <param name="y">Size on V direction</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh GridAbs(MolaMesh molaMesh, float x, float y)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.GridAbs(molaMesh.FaceVertices(i), x, y);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Subidivide each face in a MolaMesh into cells with absolute size
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="x">A list of size on U direction</param>
        /// <param name="y">A list of size on V direction</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh GridAbs(MolaMesh molaMesh, List<float> xList, List<float> yList)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.GridAbs(molaMesh.FaceVertices(i), xList[i], yList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
    } 
}