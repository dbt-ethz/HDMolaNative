using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;

namespace Mola
{
    /// <summary>
    /// Tools to edit MolaMesh
    /// </summary>
    public class MeshTools
    {
        /// <summary>
        /// Creates an offset of a mesh.
        /// If `doclose` is `true`, it will create quad faces
        /// along the naked edges of an open input mesh.
        /// </summary>
        /// <param name="mesh">A MolaMesh</param>
        /// <param name="offset">Offset distance</param>
        /// <param name="closeborders">Wether to close the borders or not</param>
        /// <param name="constrainZ"></param>
        /// <returns>The result MolaMesh</returns>
        /// ![](offsetface.png)
        public static MolaMesh Offset(MolaMesh mesh, float offset, bool closeborders = true, bool constrainZ = false)
        {
            // calculate normals per vertex
            // create new vertices and duplicate faces
            // close borders
            mesh = mesh.Copy(); // do not change original mesh
            if (closeborders) mesh.WeldVertices();

            int nFaces = mesh.Faces.Count;
            int nVertices = mesh.Vertices.Count;
            Vec3[] normals = UtilsVertex.getVertexNormals(mesh);
            if (constrainZ)
            {
                for (int i = 0; i < normals.Length; i++)
                {
                    Vec3 normal = normals[i];
                    normal.Set(normal.x, normal.y, 0);
                    normal.Normalize();
                    normals[i] = normal;
                }
            }
            for (int i = 0; i < normals.Length; i++)
            {
                Vec3 n = normals[i];
                n *= offset;
                n += mesh.Vertices[i];
                mesh.AddVertex(n.x, n.y, n.z);
            }

            for (int i = 0; i < nFaces; i++)
            {
                int[] face = mesh.Faces[i];
                int[] newFace = new int[face.Length];
                for (int j = 0; j < face.Length; j++)
                {
                    newFace[j] = face[face.Length - j - 1] + nVertices;
                }
                mesh.AddFace(newFace);
            }

            if (closeborders)
            {
                mesh.UpdateTopology();
                ReadOnlyCollection<int[]> edges = mesh.GetTopoEdges();
                foreach (int[] edge in edges)
                {
                    int f1 = edge[2];
                    int f2 = edge[3];
                    if (f1 < 0 || f2 < 0)
                    {
                        if (edge[0] < nVertices && edge[1] < nVertices)
                        {
                            int[] face = new int[4];
                            face[3] = edge[0];
                            face[2] = edge[1];
                            face[1] = edge[1] + nVertices;
                            face[0] = edge[0] + nVertices;
                            mesh.AddFace(face);
                        }

                    }
                }
            }
            for (int i = 0; i < mesh.FacesCount(); i++)
            {
                mesh.Faces[i] = Enumerable.Reverse(mesh.Faces[i]).ToArray();
            }
            return mesh;
        }
        /// <summary>
        /// Divide a MolaMesh into 2 based on the boolean mask
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="mask">A boolean array</param>
        /// <returns>A list of two MolaMesh</returns>
        public static List<MolaMesh> Split(MolaMesh molaMesh, bool[] mask)
        {
            if(mask.Length != molaMesh.FacesCount())
            {
                throw new ArgumentException("mask array count doesn't match face count!");
            }
            MolaMesh m1 = molaMesh.CopySubMesh(mask);
            mask = mask.Select(b => !b).ToArray(); 
            MolaMesh m2 = molaMesh.CopySubMesh(mask);

            return new List<MolaMesh> { m1, m2 };
        }
        /// <summary>
        /// Divide a MolaMesh into 2 based on the boolean mask
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="mask">A boolean array</param>
        /// <returns>A list of two molaMesh</returns>
        public static List<MolaMesh> Split(MolaMesh molaMesh, List<bool> mask)
        {
            bool[] maskArray = mask.ToArray();

            return Split(molaMesh, maskArray);
        }
        /// <summary>
        /// Merge a list of MolaMesh into one
        /// </summary>
        /// <param name="molaMeshes">A list of MolaMesh</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Merge(List<MolaMesh> molaMeshes)
        {
            MolaMesh molaMesh = new MolaMesh();
            for (int i = 0; i < molaMeshes.Count; i++)
            {
                molaMesh.AddMesh(molaMeshes[i]);
            }
            return molaMesh;
        }
        /// <summary>
        /// Color each face of a MolaMesh with a list of float values.
        /// The list length must match the face count.
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="values">A list of float values</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Color(MolaMesh molaMesh, List<float> values)
        {
            if (values.Count != molaMesh.FacesCount())
            {
                throw new ArgumentException("value list count doesn't match face count!");
            }
            UtilsFace.ColorFaceByValue(molaMesh, values);
            return molaMesh;
        }
        /// <summary>
        /// Color all faces of a MolaMesh 
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <param name="color">System.Drawing.Color</param>
        /// <returns>The result MolaMesh</returns>
        public static MolaMesh Color(MolaMesh molaMesh, System.Drawing.Color color)
        {
            Color mColor = new Color((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
            molaMesh.Colors = Enumerable.Repeat(mColor, molaMesh.VertexCount()).ToList();
            return molaMesh;
        }
        /// <summary>
        /// Get a boolean array from a float value list
        /// based on the input filter condition
        /// </summary>
        /// <param name="values">A float list</param>
        /// <param name="filter">A Predicate</param>
        /// <returns>A boolean Array</returns>
        /// ### Example
        /// ~~~~~~~~~~~~.cs
        /// MolaMesh molaMesh = MeshFactory.CreateSphere();
        /// List<float> faceArea = MeshAnalysis.FaceArea(molaMesh);
        /// Predicate<float> filter = a => a > 1;
        /// bool[] mask = MeshUtils.FaceMask(faceArea, filter);
        /// ~~~~~~~~~~~~
        public static bool[] FaceMask(List<float> values, Predicate<float> filter)
        {
            return values.Select(a => filter(a)).ToArray();
        }
        /// <summary>
        /// Update the topology of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <returns>A boolean Array</returns>
        public static MolaMesh UpdateTopology(MolaMesh molaMesh)
        {
            molaMesh.UpdateTopology();
            return molaMesh;
        }
        /// <summary>
        /// Weld overlapping vertices of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh</param>
        /// <returns>A boolean Array</returns>
        public static MolaMesh WeldVertices(MolaMesh molaMesh)
        {
            molaMesh.WeldVertices();
            return molaMesh;
        }
    }
}
