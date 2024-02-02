using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace Mola
{
    public class MeshOffset
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
        public static MolaMesh Offset(MolaMesh mesh, float offset, bool closeborders=true, bool constrainZ=false)
        {
            // calculate normals per vertex
            // create new vertices and duplicate faces
            // close borders
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
    }
}
