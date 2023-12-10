using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mola
{
    public class MeshSubdivision
    {
        
        /*private static List<Vec3> VerticesBetween(Vec3 v1, Vec3 v2, int n)
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
        private static List<Vec3> VerticesFrame(Vec3 v1, Vec3 v2, float w1, float w2)
        {
            Vec3 p1 = UtilsVertex.vertex_between_abs(v1, v2, w1);
            Vec3 p2 = UtilsVertex.vertex_between_abs(v2, v1, w2);
            return new List<Vec3>() { v1, p1, p2, v2 };
        }
        */

        /// <summary>
        /// Apply Catmull�Clark algorithm to a MolaMesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static MolaMesh CatmullClark(MolaMesh mesh)
        {
            return SubdivisionCatmull.Subdivide(mesh);
        }
        
        /// <summary>
        /// Extrudes the face straight by distance height.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="height"></param>
        /// <param name="capTop"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceExtrude(MolaMesh molaMesh, int[] face, float height, bool capTop = true)
        {

            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(face_vertices, height, capTop);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceExtrude(Vec3[] face_vertices, float height, bool capTop = true) // to be removed in next year
        {
            return FaceSubdivision.Extrude(face_vertices, height, capTop);
        }
        /// <summary>
        /// Extrudes the all faces in a MolaMesh straight by distance height.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="extrudeHeight"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshExtrude(MolaMesh molaMesh, float extrudeHeight, bool capTop=true)

        {
            return Extrude(molaMesh, extrudeHeight);
        }
        public static MolaMesh Extrude(MolaMesh molaMesh, float extrudeHeight, bool capTop=true)
        {
            MolaMesh newMesh = new MolaMesh();
            foreach (var face in molaMesh.Faces)
            {
                List<Vec3[]> new_faces_vertices = MeshSubdivision.SubdivideFaceExtrude(molaMesh, face, extrudeHeight, capTop);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshExtrude(MolaMesh molaMesh, List<float> extrudeHeights, List<bool> capTops)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(molaMesh.FaceVertices(i), extrudeHeights[i], capTops[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }

        /// <summary>
        /// splits a triangle, quad or a rectangle into a regular grid
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="nU"></param>
        /// <param name="nV"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceGrid(MolaMesh molaMesh, int[] face, int nU, int nV)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Grid(face_vertices, nU, nV);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceGrid(Vec3[] face_vertices, int nU, int nV)
        {
            return FaceSubdivision.Grid(face_vertices, nU, nV);
        }
        
        /// <summary>
        /// splits all triangle or quad faces in a MolaMesh into a regular grid
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="nU"></param>
        /// <param name="nV"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshGrid(MolaMesh molaMesh, int nU, int nV)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Grid(molaMesh.FaceVertices(i), nU, nV);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static MolaMesh SubdivideMeshGrid(MolaMesh molaMesh, List<int> nUList, List<int> nVList)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Grid(molaMesh.FaceVertices(i), nUList[i], nVList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        public static MolaMesh SplitRelative(MolaMesh mesh, int startSplit, float minSplit1, float maxSplit1, float minSplit2, float maxSplit2)
        {
            MolaMesh newMesh = mesh.CopyVertices();
            System.Random rnd = new System.Random();
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

                List<int> face2 = new List<int>();
                face2.Add(s1);
                face2.Add(s2);

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
        /// <summary>
        /// splits all triangle or quad faces in a MolaMesh into regular grids
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="nU"></param>
        /// <param name="nV"></param>
        /// <returns></returns>
        public static MolaMesh Grid(MolaMesh molaMesh, int nU, int nV)
        {
            MolaMesh newMesh = new MolaMesh();
            foreach (var face in molaMesh.Faces) //list of index
            {
                List<Vec3[]> new_faces_vertices = SubdivideFaceGrid(molaMesh, face, nU, nV);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        
        /// <summary>
        /// Extrudes the face tapered like a window by creating an
        /// offset face and quads between every original edge and the
        /// corresponding new edge.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="height"></param>
        /// <param name="fraction"></param>
        /// <param name="capTop"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceExtrudeTapered(MolaMesh molaMesh, int[] face, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeTapered(face_vertices, height, fraction, capTop);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceExtrudeTapered(Vec3[] face_vertices, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            return FaceSubdivision.ExtrudeTapered(face_vertices, height, fraction, capTop);
        }
        /// <summary>
        /// Extrudes all face in a MolaMesh tapered like a window by 
        /// creating an offset face and quads between every original 
        /// edge and the corresponding new edge.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="height"></param>
        /// <param name="fraction"></param>
        /// <param name="capTop"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshExtrudeTapered(MolaMesh molaMesh, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            return ExtrudeTapered(molaMesh, height = 0f, fraction = 0.5f, capTop = true);
        }
        public static MolaMesh ExtrudeTapered(MolaMesh molaMesh, float height = 0f, float fraction = 0.5f, bool capTop = true)
        {
            MolaMesh newMesh = new MolaMesh();
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

        public static MolaMesh SubdivideMeshLinearSplitBorder(MolaMesh molaMesh, float borderWidth1 = 1f, float borderWidth2 = 1, int dir = 0)
        {
            MolaMesh newMesh = new MolaMesh();
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

        public static MolaMesh SubdivideMeshLinearSplitQuad(MolaMesh molaMesh, float minSplitWidth = 0f, float maxSplitWidth = 0.5f,int dir=0)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i=0;i<molaMesh.FacesCount();i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.LinearSplitQuad(molaMesh.FaceVertices(i), minSplitWidth, maxSplitWidth,dir);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }

        public static MolaMesh SubdivideMeshLinearSplitQuad(MolaMesh molaMesh,float minSplitWidth = 1f, int dir = 0)
        {
            //if (meshCenter = new MolaMesh();
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.LinearSplitQuad(molaMesh.FaceVertices(i), minSplitWidth, dir);
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
        /// <param name="molaMesh"></param>
        /// <param name="heights"></param>
        /// <param name="fractions"></param>
        /// <param name="capTops"></param>
        /// <returns></returns>
        public static MolaMesh ExtrudeTapered(MolaMesh molaMesh, List<float> heights, List<float> fractions, List<bool> capTops)
        {
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
        
        /// <summary>
        /// Extrudes a pitched roof
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceSplitRoof(MolaMesh molaMesh, int[] face, float height = 0f)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Roof(face_vertices, height);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceSplitRoof(Vec3[] face_vertices, float height = 0f)
        {
            return FaceSubdivision.Roof(face_vertices, height);
        }
        /// <summary>
        /// Extrudes all faces in a MolaMesh into pitched rooves
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshSplitRoof(MolaMesh molaMesh, float height = 0f)
        {
            return SplitRoof(molaMesh,  height = 0f);
        }
        public static MolaMesh SplitRoof(MolaMesh molaMesh, float height = 0f)
        {
            MolaMesh newMesh = new MolaMesh();
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
        /// <summary>
        /// Extrudes all faces in a MolaMesh into pitched rooves
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="heightList"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshSplitRoof(MolaMesh molaMesh, List<float> heightList)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Roof(molaMesh.FaceVertices(i), heightList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }

        /// <summary>
        /// Extrudes the face to a point by creating a
        /// triangular face from each edge to the point.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceExtrudeToPoint(MolaMesh molaMesh, int[] face, Vec3 point)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPoint(face_vertices, point);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceExtrudeToPoint(Vec3[] face_vertices, Vec3 point)
        {
            return FaceSubdivision.ExtrudeToPoint(face_vertices, point);
        }

        /// <summary>
        /// Extrudes the face to the center point moved by height
        /// normal to the face and creating a triangular face from
        /// each edge to the point.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceExtrudeToPointCenter(MolaMesh molaMesh, int[] face, float height = 0f)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPointCenter(face_vertices, height);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceExtrudeToPointCenter(Vec3[] face_vertices, float height = 0f)
        {
            return FaceSubdivision.ExtrudeToPointCenter(face_vertices, height);
        }
        /// <summary>
        /// Extrudes each face in a MolaMesh to the center point moved 
        /// by height normal to the face and creating a triangular face
        /// from each edge to the point.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static MolaMesh ExtrudeToPointCenter(MolaMesh molaMesh, float height = 0f)
        {
            MolaMesh newMesh = new MolaMesh();
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
        /// <summary>
        /// Extrudes each face in a MolaMesh to the center point moved 
        /// by height normal to the face and creating a triangular face
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="heightList"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshExtrudeToPointCenter(MolaMesh molaMesh, List<float> heightList)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPointCenter(molaMesh.FaceVertices(i), heightList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }

        /// <summary>
        /// Creates an offset frame with quad corners. Works only with convex shapes.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static List<Vec3[]> SubdivideFaceSplitFrame(MolaMesh molaMesh, int[] face, float w)
        {
            Vec3[] face_vertices = UtilsVertex.face_vertices(molaMesh, face);
            List<Vec3[]> new_faces_vertices = FaceSubdivision.Frame(face_vertices, w);

            return new_faces_vertices;
        }
        public static List<Vec3[]> SubdivideFaceSplitFrame(Vec3[] face_vertices, float w)
        {
            return FaceSubdivision.Frame(face_vertices, w);
        }
        /// <summary>
        /// For each face in a MolaMesh, creates an offset frame with quad corners. 
        /// Works only with convex shapes.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static MolaMesh SplitFrame(MolaMesh molaMesh, float w)
        {
            MolaMesh newMesh = new MolaMesh();
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
        /// <summary>
        /// For each face in a MolaMesh, creates an offset frame with quad corners. 
        /// Works only with convex shapes.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshSplitFrame(MolaMesh molaMesh, List<float> wList)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Frame(molaMesh.FaceVertices(i), wList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// For each face in a MolaMesh, creates an absolute offset frame. 
        /// Works only with convex shapes.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshOffset(MolaMesh molaMesh, float offset)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Offset(molaMesh.FaceVertices(i), offset);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// For each face in a MolaMesh, creates an absolute offset frame. 
        /// Works only with convex shapes.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshOffset(MolaMesh molaMesh, IList<float> offset)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Offset(molaMesh.FaceVertices(i), offset[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// For each face in a MolaMesh, creates an absolute offset frame. 
        /// Works only with convex shapes.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
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

        /// <summary>
        /// For each face in a MolaMesh, creates an absolute offset frame. 
        /// Works only with convex shapes.
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshOffsetPerEdge(MolaMesh molaMesh, float[]offset)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Offset(molaMesh.FaceVertices(i), offset);
                newMesh.AddFaces(new_faces_vertices);

            }
            return newMesh;
        }
        /// <summary>
        /// Subidivide face into cells with absolute size
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="face"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
        /// Subidivide all faces in a MolaMesh into cells with absolute size
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static MolaMesh SplitGridAbs(MolaMesh molaMesh, float x, float y)
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
        /// Subidivide all faces in a MolaMesh into cells with absolute size
        /// </summary>
        /// <param name="molaMesh"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static MolaMesh SubdivideMeshSplitGridAbs(MolaMesh molaMesh, List<float> xList, List<float> yList)
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