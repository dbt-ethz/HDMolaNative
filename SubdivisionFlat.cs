using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mola
{
    public class SubdivisionFlat
    {
        public SubdivisionFlat()
        {
        }

        // avoiding instance creation through rhino vector methods, as this slows down the code
        public static MolaMesh Subdivide(MolaMesh meshInput)
        {
            MolaMesh newMesh = new MolaMesh();
            // For each face, add a face point
            // Set each face point to be the average of all original points for the respective face.

           
            Vec3[] centers = meshInput.CalculateFaceCenters();

            
                for (int i = 0; i < meshInput.Faces.Count; i++)
                {
                    Vec3 p = centers[i];
                    newMesh.AddVertex(p.x, p.y, p.z);
                    //newMeshData.vertLocked.Add(false);
                    // newMeshData.vertGenerations.Add(Utils.GetMaxVertGeneration(meshInput.Faces[i], inMeshData.vertGenerations) + 1);
                }
            
           
            
           

            // For each edge, add an edge point.
            // Set each edge point to be the average of the two neighbouring face points and its two original endpoints.
            ReadOnlyCollection<int[]>edges = meshInput.GetTopoEdges();
            for (int i = 0; i < edges.Count; i++)
            {
                int[] edge = edges[i];
                int i1 = edge[MolaMesh.VERTEX1];
                int i2 = edge[MolaMesh.VERTEX2];
                
                
                    Vec3 a = meshInput.Vertices[i1];
                    Vec3 b = meshInput.Vertices[i2];
                    float x = (a.x + b.x) / 2f;
                    float y = (a.y + b.y) / 2f;
                    float z = (a.z + b.z) / 2f;
                    newMesh.AddVertex(x, y, z);
                

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
                newMesh.AddVertex(p.x,p.y,p.z);
                
                    //newMeshData.vertLocked.Add(false);
                

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
