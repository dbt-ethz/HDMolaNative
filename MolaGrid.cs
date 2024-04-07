using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Mola
{
    /// <summary>
    /// A `MolaGrid` is taking care of getting and setting values
    /// and retrieving neighbors in an orthogonal grid of either 2 or 3 dimension.
    /// </summary>
    public class MolaGrid<T> : IEnumerable
    {
        /// <summary>
        /// Create a MolaGrid
        /// </summary>
        /// <param name="nX">X dimention</param>
        /// <param name="nY">Y dimention</param>
        /// <param name="nZ">Z dimention</param>
        /// ### Example
        /// ~~~~~~~~~~~~~~~~~~~~~~.cs
        /// MolaGrid<bool> grid = new MolaGrid<bool>(10, 10, 10);
        /// ~~~~~~~~~~~~~~~~~~~~~~
        public MolaGrid(int nX, int nY, int nZ)
        {
            this.NX = nX;
            this.NY = nY;
            this.NZ = nZ;
            this.nYZ = nY * nZ;
            this.Values = new T[nX * nY * nZ];
        }
        public T this[int index]
        {
            get { return values[index]; }
            set { values[index] = value; }
        }
        public T this[int x, int y, int z]
        {
            get { return GetValue(x, y, z); }
            set { SetValue(x, y, z, value); }
        }
        private int nX;
        private int nY;
        private int nZ;
        public int nYZ;
        IList<T> values;
        public int Count => Values.Count;
        public int NX { get => nX; set => nX = value; }
        public int NY { get => nY; set => nY = value; }
        public int NZ { get => nZ; set => nZ = value; }
        public IList<T> Values { get => values; set => values = value; }
        public T GetValue(int x, int y, int z)
        {
            return Values[GetIndex(x, y, z)];
        }
        public int GetIndex(int x, int y, int z)
        {
            return x * nYZ + y * NZ + z;
        }
        public void SetValue(int x, int y, int z, T value)
        {
            Values[GetIndex(x, y, z)] = value;
        }
        public int getX(int index)
        {
            return index / nYZ;
        }
        public int getY(int index)
        {
            return (index / NZ) % NY;
        }
        public int getZ(int index)
        {
            return index % NZ;
        }
        public IEnumerator GetEnumerator()
        {
            return Values.GetEnumerator();
        }
        public void CopyTo(Array array, int index)
        {
            Values.CopyTo((T[])array, index);
        }
        public int[][] GetNbs(int[][] kernel)
        {
            int[][] nbs = new int[Count][];
            for (int x = 0; x < nX; x++)
            {
                for (int y = 0; y < nY; y++)
                {
                    for (int z = 0; z < nZ; z++)
                    {
                        int[] cellNbs = new int[kernel.Length];
                        cellNbs = Enumerable.Repeat(-1, kernel.Length).ToArray();
                        nbs[GetIndex(x, y, z)] = cellNbs;
                        for (int i = 0; i < kernel.Length; i++)
                        {
                            int[] coords = kernel[i];
                            int cx = coords[0] + x;
                            int cy = coords[1] + y;
                            int cz = coords[2] + z;
                            if (cx >= 0 && cy >= 0 && cz >= 0 && cx < nX && cy < nY && cz < nZ)
                            {
                                cellNbs[i] = GetIndex(cx, cy, cz);
                            }
                        }
                    }
                }
            }
            return nbs;
        }
        public int[][] GetXZNbs4()
        {
            int[][] kernel = new int[4][];
            kernel[0] = new int[] { -1, 0, 0 };
            kernel[1] = new int[] { 0, 0, 1 };
            kernel[2] = new int[] { 1, 0, 0 };
            kernel[3] = new int[] { 0, 0, -1 };
            return GetNbs(kernel);
        }
        public int[][] GetXYZNbs6()
        {
            int[][] kernel = new int[6][];
            kernel[0] = new int[] { -1, 0, 0 };
            kernel[1] = new int[] { 0, 0, 1 };
            kernel[2] = new int[] { 1, 0, 0 };
            kernel[3] = new int[] { 0, 0, -1 };
            kernel[4] = new int[] { 0, 1, 0 };
            kernel[5] = new int[] { 0, -1, 0 };
            return GetNbs(kernel);
        }
        public MolaGrid<T> Copy()
        {
            MolaGrid<T> copyGrid = new MolaGrid<T>(nX, nY, nZ);
            for (int i = 0; i < this.values.Count; i++)
            {
                copyGrid.values[i] = this.values[i];
            }

            return copyGrid;
        }
    }
}
