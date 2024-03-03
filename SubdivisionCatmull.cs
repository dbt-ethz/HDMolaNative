using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mola
{
    public class SubdivisionCatmull
    {
        public SubdivisionCatmull()
        {
        }

        // avoiding instance creation through rhino vector methods, as this slows down the code
        public static MolaMesh Subdivide(MolaMesh meshInput, List<float> faceExtrusions=null)
        {
            MolaMesh newMesh = new MolaMesh();
            // For each face, add a face point
            // Set each face point to be the average of all original points for the respective face.
            meshInput.UpdateTopology();

            Vec3[] centers = meshInput.CalculateFaceCenters();

            if (faceExtrusions == null)
            {
                for (int i = 0; i < meshInput.Faces.Count; i++)
                {
                    Vec3 p = centers[i];
                    newMesh.AddVertex(p.x, p.y, p.z);
                    //newMeshData.vertLocked.Add(false);
                    // newMeshData.vertGenerations.Add(Utils.GetMaxVertGeneration(meshInput.Faces[i], inMeshData.vertGenerations) + 1);
                }
            }
            else if (faceExtrusions.Count == meshInput.Faces.Count)
            {
                Vec3[] normals = meshInput.CalculateNormals();
                for (int i = 0; i < meshInput.Faces.Count; i++)
                {
                    Vec3 p = centers[i];
                    Vec3 n = normals[i];
                    float factor = faceExtrusions[i];
                    newMesh.AddVertex(p.x + n.x * factor, p.y + n.y * factor, p.z + n.z * factor);
                    //newMeshData.vertLocked.Add(false);
                    //newMeshData.vertGenerations.Add(Utils.GetMaxVertGeneration(meshInput.Faces[i], inMeshData.vertGenerations) + 1);
                }
            }
            else if (faceExtrusions.Count == 1)
            {
                Vec3[] normals = meshInput.CalculateNormals();
                float factor = faceExtrusions[0];
                for (int i = 0; i < meshInput.Faces.Count; i++)
                {
                    Vec3 p = centers[i];
                    Vec3 n = normals[i];
                    newMesh.AddVertex(p.x + n.x * factor, p.y + n.y * factor, p.z + n.z * factor);
                    //newMeshData.vertLocked.Add(false);
                   // newMeshData.vertGenerations.Add(Utils.GetMaxVertGeneration(meshInput.Faces[i], inMeshData.vertGenerations) + 1);
                }
            }
           

            // For each edge, add an edge point.
            // Set each edge point to be the average of the two neighbouring face points and its two original endpoints.
            ReadOnlyCollection<int[]>edges = meshInput.GetTopoEdges();
            for (int i = 0; i < edges.Count; i++)
            {
                int[] edge = edges[i];
                int i1 = edge[MolaMesh.VERTEX1];
                int i2 = edge[MolaMesh.VERTEX2];
                int face1 = edge[MolaMesh.FACE1];
                int face2 = edge[MolaMesh.FACE2];
                if (face1>=0&&face2>=0)
                {
                    Vec3 n1 = newMesh.Vertices[face1];
                    Vec3 n2 = newMesh.Vertices[face2];
                    Vec3 a = meshInput.Vertices[i1];
                    Vec3 b = meshInput.Vertices[i2];
                    float x = (n1.x + n2.x + a.x + b.x) / 4f;
                    float y = (n1.y + n2.y + a.y + b.y) / 4f;
                    float z = (n1.z + n2.z + a.z + b.z) / 4f;
                    newMesh.AddVertex(x, y, z);

                }
                else if (face1 >= 0 || face2 >= 0)
                {
                    // border
                    Vec3 a = meshInput.Vertices[i1];
                    Vec3 b = meshInput.Vertices[i2];
                    float x = (a.x + b.x) / 2f;
                    float y = (a.y + b.y) / 2f;
                    float z = (a.z + b.z) / 2f;
                    newMesh.AddVertex(x, y, z);
                }

                //newMeshData.vertLocked.Add(false);
                //newMeshData.vertGenerations.Add(System.Math.Max(inMeshData.vertGenerations[i1], inMeshData.vertGenerations[i2]));

            }

            // For each original point P, take the average F of all n(recently created) face points for faces touching P, 
            // and take the average R of all n edge midpoints for (original) edges touching P, 
            // where each edge midpoint is the average of its two endpoint vertices(not to be confused with new "edge points" above).
            // Move each original point to the point (F+2*R+(n-3)*P)/n
            for (int i = 0; i < meshInput.Vertices.Count; i++)
            {
                //int vertexIndex = meshInput.TopologyVertices.MeshVertexIndices(i)[0];
                Vec3 p = meshInput.Vertices[i];
                if (false /*inMeshData.vertLocked[i]*/)
                {
                    //newMeshData.mesh.Vertices.Add(p.x, p.y, p.z);
                    //newMeshData.vertLocked.Add(true);
                }
                else
                {

                   // meshInput.
                    int[] nbFaces = meshInput.AdjacentFacesToVertex(i);
                    int[] nbVertices = meshInput.AdjacentVerticesToVertex(i);



                    //hole only boundary edges
                    if (nbFaces.Length != nbVertices.Length)
                    {
                        // average boundary edges
                        float rX = 0;
                        float rY = 0;
                        float rZ = 0;
                        int nBoundaryEdges = 0;
                        for (int j = 0; j < nbVertices.Length; j++)
                        {
                            int nbndex = nbVertices[j];
                            int edgeIndex = meshInput.AdjacentEdgeToVertices(i, nbndex);
                            //int face1= meshInput.
                            int face1 = meshInput.AdjacentFace1ToEdge(edgeIndex);
                            int face2 = meshInput.AdjacentFace2ToEdge(edgeIndex);
                            if (face1<0|| face2<0)
                            {
                                Vec3 nb = meshInput.Vertices[nbndex];
                                nBoundaryEdges++;
                                rX += (p.x + nb.x) / 2;
                                rY += (p.y + nb.y) / 2;
                                rZ += (p.z + nb.z) / 2;
                            }
                        }
                        rX /= nBoundaryEdges;
                        rY /= nBoundaryEdges;
                        rZ /= nBoundaryEdges;
                        float resX = (rX + p.x) / (2);
                        float resY = (rY + p.y) / (2);
                        float resZ = (rZ + p.z) / (2);
                        newMesh.AddVertex(resX, resY, resZ);
                    }
                    else
                    {
                        // average face point
                        float fX = 0;
                        float fY = 0;
                        float fZ = 0;
                        for (int j = 0; j < nbFaces.Length; j++)
                        {
                            Vec3 center = newMesh.Vertices[nbFaces[j]];
                            fX += center.x;
                            fY += center.y;
                            fZ += center.z;
                        }
                        fX /= nbFaces.Length;
                        fY /= nbFaces.Length;
                        fZ /= nbFaces.Length;

                        // average mid edges point 
                        float rX = 0;
                        float rY = 0;
                        float rZ = 0;
                        for (int j = 0; j < nbVertices.Length; j++)
                        {
                            int nbndex = nbVertices[j];
                            Vec3 nb = meshInput.Vertices[nbndex];
                            rX += (p.x + nb.x) / 2;
                            rY += (p.y + nb.y) / 2;
                            rZ += (p.z + nb.z) / 2;
                        }
                        float fac = 2f / nbVertices.Length;
                        rX *= fac;
                        rY *= fac;
                        rZ *= fac;


                        // original point
                        fac = nbFaces.Length - 3;
                        p.x *= fac;
                        p.y *= fac;
                        p.z *= fac;


                        float resX = (fX + rX + p.x) / nbFaces.Length;
                        float resY = (fY + rY + p.y) / nbFaces.Length;
                        float resZ = (fZ + rZ + p.z) / nbFaces.Length;
                        newMesh.AddVertex(resX, resY, resZ);
                    }
                    //newMeshData.vertLocked.Add(false);
                }

                //newMeshData.vertGenerations.Add(inMeshData.vertGenerations[vertexIndex]);

            }

            // create faces
            //Utils.CollectNewFaces(meshInput, inMeshData.faceGroups, newMeshData.mesh, newMeshData.faceGroups);
            CollectNewFaces(meshInput, newMesh);
            newMesh.UpdateTopology();
            return newMesh;
        }

        public static void CollectNewFaces(MolaMesh meshInput, MolaMesh meshOut)
        {
            // create faces
            int vertexIndexStart = meshInput.Faces.Count + meshInput.GetTopoEdges().Count;
            for (int i = 0; i < meshInput.Faces.Count; i++)
            {
                int[] topoV = meshInput.Faces[i];
                int faceVertexIndex = i;
                int j0 = topoV.Length - 2;
                int j1 = topoV.Length - 1;
                int edgeVertexIndexPrev = meshInput.AdjacentEdgeToVertices(topoV[j0], topoV[j1]) + meshInput.Faces.Count;
                for (int j2 = 0; j2 < topoV.Length; j2++)
                {
                    int edgeVertexIndex = meshInput.AdjacentEdgeToVertices(topoV[j1], topoV[j2]) + meshInput.Faces.Count;
                    meshOut.AddQuad(edgeVertexIndexPrev, topoV[j1] + vertexIndexStart, edgeVertexIndex, faceVertexIndex);
                    //faceGroupsOut.Add(faceGroups[i]);
                    j1 = j2;
                    edgeVertexIndexPrev = edgeVertexIndex;
                }
            }
            
        }
    }
   


}
