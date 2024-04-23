using System;
using System.Collections.Generic;
using System.Linq;

namespace Mola
{
    /// <summary>
    /// A collection of methods to applay subdivision rules to a MolaMesh
    /// </summary>
    public class MeshSubdivision
    {
        /// <summary>
        /// Apply CatmullClark algorithm to a MolaMesh
        /// </summary>
        /// <param name="mesh">A MolaMesh</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](catmullclark.png)
        public static MolaMesh CatmullClark(MolaMesh mesh)
        {
            return SubdivisionCatmull.Subdivide(mesh);
        }
        /// <summary>
        /// Extrudes the all faces in a MolaMesh straight by a single distance height.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="height">Extruding height</param>
        /// <param name="capTop">Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](extrude.png)
        public static MolaMesh Extrude(MolaMesh molaMesh, float height, bool capTop=true)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(molaMesh.FaceVertices(i), height, capTop);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Extrudes the all faces in a MolaMesh straight by a list distance height. The list length must much the face count.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="height">A list Extruding height</param>
        /// <param name="capTop">A list of bool to decide Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](extrude.png)
        public static MolaMesh Extrude(MolaMesh molaMesh, List<float> heights, List<bool> capTops)
        {
            if(heights.Count != molaMesh.FacesCount() || capTops.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list count doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(molaMesh.FaceVertices(i), heights[i], capTops[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Extrudes the all faces in a MolaMesh along a direction by a single distance height.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="direction">A Vec3 for direction</param>
        /// <param name="height">Extruding height</param>
        /// <param name="capTop">Wether to cp the top or not</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh ExtrudeAlongVec(MolaMesh molaMesh, Vec3 direction, float height, bool capTop = true)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(molaMesh.FaceVertices(i), direction, height, capTop);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Extrudes the all faces in a MolaMesh along a list of directions and by a list distance height. The list length must much the face count.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="directions">A list of Vec3</param>
        /// <param name="heights">A list Extruding height</param>
        /// <param name="capTops">A list of bool to decide Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh ExtrudeAlongVec(MolaMesh molaMesh, List<Vec3> directions, List<float> heights, List<bool> capTops)
        {
            if (heights.Count != molaMesh.FacesCount() || capTops.Count != molaMesh.FacesCount()
                || directions.Count != molaMesh.FacesCount()
                )
            {
                throw new ArgumentException("list count doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Extrude(molaMesh.FaceVertices(i), directions[i], heights[i], capTops[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Splits all triangle or quad faces in a MolaMesh into regular grids
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="nU">Division count on U direction</param>
        /// <param name="nV">Devision count on V direction</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](grid.png)
        public static MolaMesh Grid(MolaMesh molaMesh, int nU, int nV)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Grid(molaMesh.FaceVertices(i), nU, nV);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// splits all triangle or quad faces in a MolaMesh into regular grids
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="nU">A list of int U</param>
        /// <param name="nV">A list of int V</param>/// <param name="nU">Division count on U direction</param>
        /// <param name="nV">Devision count on V direction</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](grid.png)
        public static MolaMesh Grid(MolaMesh molaMesh, List<int> nUList, List<int> nVList)
        {
            if (nUList.Count != molaMesh.FacesCount() || nVList.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list count doesn't match face count!");
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
        /// Split all faces in a MolaMesh based on relative parameters 
        /// </summary>
        /// <param name="mesh">A MolaMesh</param>
        /// <param name="startSplit">Choose U or V as starting direction</param>
        /// <param name="minSplit1">min relative parameter on the first direction</param>
        /// <param name="maxSplit1">max relative parameter on the first direction</param>
        /// <param name="minSplit2">min relative parameter on the second direction</param>
        /// <param name="maxSplit2">max relative parameter on the second direction</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](relative.png)
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
        //public static MolaMesh ExtrudeTapered(MolaMesh molaMesh, float height = 0f, float fraction = 0.5f, bool capTop = true)
        //{
        //    MolaMesh newMesh = new();
        //    for (int i = 0; i < molaMesh.Faces.Count; i++)
        //    {
        //        List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeTapered(molaMesh.FaceVertices(i), height, fraction, capTop);
        //        foreach (var face_vertices in new_faces_vertices)
        //        {
        //            newMesh.AddFace(face_vertices);
        //        }
        //    }
        //    return newMesh;
        //}
        ///// <summary>
        ///// Split each face in a quad MolaMesh into three quads in one direction 
        ///// by specifying the range to generate random widths of the first two segments.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="borderWidth1">An absolute distance from one side of border</param>
        /// <param name="borderWidth2">An absolute distance from one side of border</param>
        /// <param name="dir"></param>
        /// <returns>The result MolaMesh</returns>
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
        /// ![](linearsplitborder.png)
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
        /// ![](linearsplitborder.png)
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
        /// <param name="height">Extruding height</param>
        /// <param name="fraction">A relative value</param>
        /// <param name="capTop">A bool to decide Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](extrudetapered.png)
        public static MolaMesh ExtrudeTapered(MolaMesh molaMesh, float height, float fraction, bool capTop)
        {
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeTapered(molaMesh.FaceVertices(i), height, fraction, capTop);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Extrudes all face in a MolaMesh tapered like a window by
        /// creating an offset face and quads between every original 
        /// edge and the corresponding new edge.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="heights">A list of extruding height</param>
        /// <param name="fractions">A list of relative values</param>
        /// <param name="capTops">A list of bool to decide Wether to cap the top or not</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](extrudetapered.png)
        public static MolaMesh ExtrudeTapered(MolaMesh molaMesh, List<float> heights, List<float> fractions, List<bool> capTops)
        {
            if (heights.Count != molaMesh.FacesCount() || capTops.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list count doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeTapered(molaMesh.FaceVertices(i), heights[i], fractions[i], capTops[i]);
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
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="height">Extruding height</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](roof.png)
        public static MolaMesh SplitRoof(MolaMesh molaMesh, float height = 0f)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Roof(molaMesh.FaceVertices(i), height);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Extrude each face in a MolaMesh into pitched rooves 
        /// by specifying individual extrusion heights for each face.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="heightList">The list of extruding height</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](roof.png)
        public static MolaMesh SplitRoof(MolaMesh molaMesh, List<float> heightList)
        {
            if (heightList.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list count doesn't match face count!");
            }
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Roof(molaMesh.FaceVertices(i), heightList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Extrude each face in a MolaMesh to a new point offset from its center 
        /// by a distance along the normal vector of the face and 
        /// create triangular faces from each edge to the point.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="height">Extruding height</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](extrudecenter.png)
        public static MolaMesh ExtrudeToPointCenter(MolaMesh molaMesh, float height = 0f)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPointCenter(molaMesh.FaceVertices(i), height);
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
        /// from each edge to the point.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="heightList">A list of extruding height</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](extrudecenter.png)
        public static MolaMesh ExtrudeToPointCenter(MolaMesh molaMesh, List<float> heightList)
        {
            if (heightList.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list count doesn't match face count!");
            }
            MolaMesh newMesh = new MolaMesh();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.ExtrudeToPointCenter(molaMesh.FaceVertices(i), heightList[i]);
                newMesh.AddFaces(new_faces_vertices);
            }
            return newMesh;
        }
        /// <summary>
        /// Create an offset frame with quad corners from each face in a Molamesh. 
        /// Only work for convex shapes.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="w">The relative value</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](splitframe.png)
        public static MolaMesh SplitFrame(MolaMesh molaMesh, float w)
        {
            MolaMesh newMesh = new();
            for (int i = 0; i < molaMesh.Faces.Count; i++)
            {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Frame(molaMesh.FaceVertices(i), w);
                foreach (var face_vertices in new_faces_vertices)
                {
                    newMesh.AddFace(face_vertices);
                }
            }
            return newMesh;
        }
        /// <summary>
        /// Create an offset frame with quad corners from each face in a Molamesh by specifying individual offset distances. 
        /// Only work for convex shapes.
        /// </summary>
        /// <param name="molaMesh">The MolaMesh</param>
        /// <param name="wList">A list of relative value</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](splitframe.png)
        public static MolaMesh SplitFrame(MolaMesh molaMesh, List<float> wList)
        {
            if (wList.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("list count doesn't match face count!");
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
        /// ![](offset.png)
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
        /// ![](offset.png)
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
        /// <summary>
        /// Subidivide each face in a MolaMesh into cells with absolute size
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="x">Size on U direction</param>
        /// <param name="y">Size on V direction</param>
        /// <returns>The result MolaMesh</returns>
        /// ![](gridabs.png)
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
        /// ![](gridabs.png)
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