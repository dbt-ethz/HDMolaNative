using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Mola
{
    /// <summary>
    /// A collection of methods to analyze MolaMesh Face
    /// </summary>
    public class MeshAnalysis
    {
        /// <summary>
        /// Get the area value of each face of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh to be analyzed</param>
        /// <returns>A list of float values</returns>
        public static List<float> FaceArea(MolaMesh molaMesh)
        {
            List<float> valueList = new List<float>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceArea(i));
            }
            return valueList;
        }
        /// <summary>
        /// Get the compactness value of each face of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh to be analyzed</param>
        /// <returns>A list of float values</returns>
        public static List<float> FaceCompactness(MolaMesh molaMesh)
        {
            List<float> valueList = new List<float>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceCompactness(i));
            }
            return valueList;
        }
        /// <summary>
        /// Get the index of each face of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh to be analyzed</param>
        /// <returns>A list of int values</returns>
        public static List<int> FaceIndex(MolaMesh molaMesh)
        {
            int fcount = molaMesh.FacesCount();
            List<int> valueList = Enumerable.Range(0, fcount).ToList();
 
            return valueList;
        }
        /// <summary>
        /// Get the center position of each face of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh to be analyzed</param>
        /// <returns>A list of Mola Vec3</returns>
        public static List<Vec3> FaceLocation(MolaMesh molaMesh)
        {
            List<Vec3> valueList = new List<Vec3>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceCenter(i));
            }
            return valueList;
        }
        /// <summary>
        /// Get a boolean value for each face according to modulo
        /// </summary>
        /// <param name="molaMesh">A MolaMesh to be analyzed</param>
        /// <param name="modulo">The modulo int number</param>
        /// <param name="n">the desired index number within the modulo</param>
        /// <returns>A list of boolean values</returns>
        /// ### Example
        /// ~~~~~~~~~~~~~~.cs
        /// MolaMesh mesh = MeshFactory.CreateBox();
        /// List<bool> values = MeshAnalysis.FaceModulo(mesh, 6, 0);
        /// ~~~~~~~~~~~~~~
        public static List<bool> FaceModulo(MolaMesh molaMesh, int modulo=5, int n=4)
        {
            List<bool> valueList = Enumerable.Repeat(false, molaMesh.FacesCount()).ToList();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                if (i % modulo == n)
                {
                    valueList[i] = true;
                }
            }
            return valueList;
        }
        /// <summary>
        /// Get the normal vector of each face of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh to be analyzed</param>
        /// <returns>A list of Mola Vec3</returns>
        public static List<Vec3> FaceNormal(MolaMesh molaMesh)
        {
            List<Vec3> valueList = new List<Vec3>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceNormal(i));
            }
            return valueList;
        }
        /// <summary>
        /// Get the perimeter value of each face of a MolaMesh
        /// </summary>
        /// <param name="molaMesh">A MolaMesh to be analyzed</param>
        /// <returns>A list of float values</returns>
        public static List<float> FacePerimeter(MolaMesh molaMesh)
        {
            List<float> valueList = new List<float>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FacePerimeter(i));
            }
            return valueList;
        }
    }
}
