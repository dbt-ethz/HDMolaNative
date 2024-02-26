using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Mola
{
    public class MeshAnalysis
    {
        public static List<float> FaceArea(MolaMesh molaMesh)
        {
            List<float> valueList = new List<float>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceArea(i));
            }
            return valueList;
        }
        public static List<float> FaceCompactness(MolaMesh molaMesh)
        {
            List<float> valueList = new List<float>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceCompactness(i));
            }
            return valueList;
        }
        public static List<int> FaceIndex(MolaMesh molaMesh)
        {
            int fcount = molaMesh.FacesCount();
            List<int> valueList = Enumerable.Range(0, fcount).ToList();
 
            return valueList;
        }
        public static List<Vec3> FaceLocation(MolaMesh molaMesh)
        {
            List<Vec3> valueList = new List<Vec3>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceCenter(i));
            }
            return valueList;
        }
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
        public static List<Vec3> FaceNormal(MolaMesh molaMesh)
        {
            List<Vec3> valueList = new List<Vec3>();
            for (int i = 0; i < molaMesh.FacesCount(); i++)
            {
                valueList.Add(molaMesh.FaceNormal(i));
            }
            return valueList;
        }
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
