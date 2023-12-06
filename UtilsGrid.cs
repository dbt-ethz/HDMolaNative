using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mola
{
    public class UtilsGrid 
    {
        public static MolaMesh VoxelMesh(bool[,,] grid, Color? c = null)
        {
            int nX = grid.GetLength(0);
            int nY = grid.GetLength(1);
            int nZ = grid.GetLength(2);
            MolaGrid<bool> matrix = new MolaGrid<bool>(nX, nY, nZ);
            for (int x = 0; x < nX; x++)
            {
                for (int y = 0; y < nY; y++)
                {
                    for (int z = 0; z < nZ; z++)
                    {
                        matrix.SetValue(x, y, z, grid[x, y, z]);
                    }
                }
            }
            return VoxelMesh(matrix,  c );
        }
        public static MolaMesh VoxelMesh(MolaGrid<bool> grid, Color? c = null)
        {
            Color color = c ?? Color.white;
            MolaMesh molaMesh = new MolaMesh();

            for (int x = 0; x < grid.NX; x++)
            {
                for (int y = 0; y < grid.NY; y++)
                {
                    for (int z = 0; z < grid.NZ; z++)
                    {
                        if (grid[x, y, z])
                        {
                            if (x == grid.NX - 1 || !grid[x + 1, y, z])
                            {
                                Vec3 v1 = new Vec3(x + 1, y, z);
                                Vec3 v2 = new Vec3(x + 1, y + 1, z);
                                Vec3 v3 = new Vec3(x + 1, y + 1, z + 1);
                                Vec3 v4 = new Vec3(x + 1, y, z + 1);
                                molaMesh.AddFace(new Vec3[4] { v1, v2, v3, v4 });
                                //MolaMeshFactory.AddQuadX1(molaMesh, x, y, z);
                            }

                            if (x == 0 || !grid[x - 1, y, z])
                            {
                                Vec3 v1 = new Vec3(x, y + 1, z);
                                Vec3 v2 = new Vec3(x, y, z);
                                Vec3 v3 = new Vec3(x, y, z + 1);
                                Vec3 v4 = new Vec3(x, y + 1, z + 1);
                                molaMesh.AddFace(new Vec3[4] { v1, v2, v3, v4 });
                                //MolaMeshFactory.AddQuadX0(myMesh, x, y, z);
                            }

                            if (y == grid.NY - 1 || !grid[x, y + 1, z])
                            {
                                Vec3 v1 = new Vec3(x + 1, y + 1, z);
                                Vec3 v2 = new Vec3(x, y + 1, z);
                                Vec3 v3 = new Vec3(x, y + 1, z + 1);
                                Vec3 v4 = new Vec3(x + 1, y + 1, z + 1);
                                molaMesh.AddFace(new Vec3[4] { v1, v2, v3, v4 });
                                //MolaMeshFactory.AddQuadY1(myMesh, x, y, z);
                            }

                            if (y == 0 || !grid[x, y - 1, z])
                            {
                                Vec3 v1 = new Vec3(x, y, z);
                                Vec3 v2 = new Vec3(x + 1, y, z);
                                Vec3 v3 = new Vec3(x + 1, y, z + 1);
                                Vec3 v4 = new Vec3(x, y, z + 1);
                                molaMesh.AddFace(new Vec3[4] { v1, v2, v3, v4 });
                            }

                            if(z == grid.NZ - 1 || !grid[x, y, z + 1])
                            {
                                Vec3 v1 = new Vec3(x, y, z + 1);
                                Vec3 v2 = new Vec3(x + 1, y, z + 1);
                                Vec3 v3 = new Vec3(x + 1, y + 1, z + 1);
                                Vec3 v4 = new Vec3(x, y + 1, z + 1);
                                molaMesh.AddFace(new Vec3[4] { v1, v2, v3, v4 });
                                //MolaMeshFactory.AddQuadZ1(myMesh, x, y, z);
                            }

                            if (z == 0 || !grid[x, y, z- 1])
                            {
                                Vec3 v1 = new Vec3(x, y + 1, z);
                                Vec3 v2 = new Vec3(x + 1, y + 1, z);
                                Vec3 v3 = new Vec3(x + 1, y, z);
                                Vec3 v4 = new Vec3(x, y, z);
                                molaMesh.AddFace(new Vec3[4] { v1, v2, v3, v4 });
                                //MolaMeshFactory.AddQuadZ0(myMesh, x, y, z);
                            }

                        }
                    }
                }
            }
            molaMesh.SetVertexColors(color);
            return molaMesh;
        }
        public static MolaMesh VoxelMesh(MolaGrid<bool> grid, float scale, Color? c = null)
        {
            Color color = c ?? Color.white;
            MolaMesh molaMesh = VoxelMesh(grid, color);
            molaMesh.Scale(scale, scale, scale);
            return molaMesh;
        }
        public static MolaGrid<bool> GridBooleanUnion(MolaGrid<bool> grid1, MolaGrid<bool> grid2)
        {
            int[] dimention = GetGridDimention(grid1);
            if(dimention != GetGridDimention(grid2))
            {
                throw new Exception("two grids have different dimention!");
            }
            
            MolaGrid<bool> result = new MolaGrid<bool>(dimention[0], dimention[1], dimention[2]);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = grid1[i] || grid2[i];
            }
            return result;
        }
        public static MolaGrid<bool> GridBooleanUnionList(List<MolaGrid<bool>> grids)
        {
            int[] dimention = GetGridDimention(grids[0]);
            MolaGrid<bool> result = new MolaGrid<bool>(dimention[0], dimention[1], dimention[2]);

            var listsOfLists = new List<IEnumerable<bool>>();
            foreach (MolaGrid<bool> grid in grids)
            {
                listsOfLists.Add(grid.Values);
            }
            var combinedResults = listsOfLists.Aggregate((a, b) => a.Zip(b, (aElement, bElement) => aElement || bElement));
            result.Values = combinedResults.ToList();

            return result;
        }
        public static MolaGrid<bool> GridBooleanIntersection(MolaGrid<bool> grid1, MolaGrid<bool> grid2)
        {
            int[] dimention = GetGridDimention(grid1);
            MolaGrid<bool> result = new MolaGrid<bool>(dimention[0], dimention[1], dimention[2]);
            
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = grid1[i] && grid2[i];
            }
            return result;
        }
        public static MolaGrid<bool> GridBooleanDifference(MolaGrid<bool> grid1, MolaGrid<bool> grid2)
        {
            int[] dimention = GetGridDimention(grid1);
            MolaGrid<bool> result = new MolaGrid<bool>(dimention[0], dimention[1], dimention[2]);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = grid1[i] && !grid2[i];
            }
            return result;
        }
        public static int[] GetGridDimention(MolaGrid<bool> grid)
        {
            int nX = grid.NX;
            int nY = grid.NY;
            int nZ = grid.NZ;
            return new int[]{ nX, nY, nZ};
        }
    }
}