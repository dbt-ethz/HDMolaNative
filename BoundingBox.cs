using System;
using System.Collections.Generic;

namespace Mola
{
    public class BoundingBox
    {

        float x1 = float.MaxValue;
        float y1 = float.MaxValue;
        float z1 = float.MaxValue;
        float x2 = float.MinValue;
        float y2 = float.MinValue;
        float z2 = float.MinValue;


        public BoundingBox()
        {

        }

        public void SetPoint1(float x1, float y1, float z1)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.z1 = z1;
        }
        public void SetPoint2(float x2, float y2, float z2)
        {
            this.x2 = x2;
            this.y2 = y2;
            this.z2 = z2;
        }
        public BoundingBox(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            SetPoint1(x1, y1, z1);
            SetPoint2(x2, y2, z2);
        }
        public BoundingBox(IEnumerable<Vec3> vertices)
        {
            foreach (Vec3 v in vertices)
            {
                this.AddPoint(v);
            }
        }

        public BoundingBox(Vec3 x1y1z1, Vec3 x2y2z2)
        {
            SetPoint1(x1y1z1.x, x1y1z1.y, x1y1z1.z);
            SetPoint2(x2y2z2.x, x2y2z2.y, x2y2z2.z);
        }


        public BoundingBox(float x1, float y1, float x2, float y2)
        {
            SetPoint1(x1, y1, 0);
            SetPoint2(x2, y2, 0);
        }




        public BoundingBox(BoundingBox b)
        {

            this.X1(b.X1());
            this.Y1(b.Y1());
            this.Z1(b.Z1());
            this.X2(b.X2());
            this.Y2(b.Y2());
            this.Z2(b.Z2());
        }

        public void ReOrient()
        {
            if (X1() > X2())
            {
                float tmp = X1();
                X1(X2());
                X2(tmp);
            }
            if (Y1() > Y2())
            {
                float tmp = Y1();
                Y1(Y2());
                Y2(tmp);
            }
            if (Z1() > Z2())
            {
                float tmp = Z1();
                Z1(Z2());
                Z2(tmp);
            }
        }

        public void Add(BoundingBox box)
        {
            X1(Math.Min(box.X1(), X1()));
            Y1(Math.Min(box.Y1(), Y1()));
            Z1(Math.Min(box.Z1(), Z1()));
            X2(Math.Max(box.X2(), X2()));
            Y2(Math.Max(box.Y2(), Y2()));
            Z2(Math.Max(box.Z2(), Z2()));
        }

        public void Intersect(BoundingBox box)
        {
            X1(Math.Max(box.X1(), X1()));
            Y1(Math.Max(box.Y1(), Y1()));
            Z1(Math.Max(box.Z1(), Z1()));
            X1(Math.Min(box.X2(), X2()));
            Y1(Math.Min(box.Y2(), Y2()));
            Z1(Math.Min(box.Z2(), Z2()));
        }

        public void AddPoint(float x, float y, float z)
        {
            if (x < X1()) X1(x);
            if (y < Y1()) Y1(y);
            if (z < Z1()) Z1(z);
            if (x > X2()) X2(x);
            if (y > Y2()) Y2(y);
            if (z > Z2()) Z2(z);
        }


        public void AddPoint(Vec3 v)
        {
            AddPoint(v.x, v.y, v.z);
        }
        public void AddPoints(IEnumerable<Vec3> vertices)
        {
            foreach (Vec3 v in vertices)
            {
                this.AddPoint(v);
            }
        }

        public void ScaleAroundCenter(float s)
        {
            float cX = this.GetCenterX();
            float cY = this.GetCenterY();
            float cZ = this.GetCenterZ();
            float dX = this.GetDimX() * s * 0.5f;
            float dY = this.GetDimY() * s * 0.5f;
            float dZ = this.GetDimZ() * s * 0.5f;
            X1(cX - dX);
            X2(cX + dX);
            Y1(cY - dY);
            Y2(cY + dY);
            Z1(cZ - dZ);
            Z2(cZ + dZ);
        }

        public float GetMaxDimension()
        {
            return Math.Max(Math.Max(GetDimX(), GetDimY()), GetDimZ());
        }
        public float GetMinDimension()
        {
            return Math.Min(Math.Min(GetDimX(), GetDimY()), GetDimZ());
        }

        public float GetInnerDistance(float x, float y, float z)
        {
            float MinX = Math.Min(Math.Abs(x - X1()), Math.Abs(x - X2()));
            float MinY = Math.Min(Math.Abs(y - Y1()), Math.Abs(y - Y2()));
            float MinZ = Math.Min(Math.Abs(z - Z1()), Math.Abs(z - Z2()));
            float MinD = Math.Min(MinX, MinY);
            MinD = Math.Min(MinD, MinZ);
            return MinD;
        }

        public void Offset(float offset)
        {
            X1(this.X1() - offset);
            Y1(this.Y1() - offset);
            Z1(this.Z1() - offset);

            X2(this.X2() + offset);
            Y2(this.Y2() + offset);
            Z2(this.Z2() + offset);

        }

        public BoundingBox GetOffsetBox(float offset)
        {
            return new BoundingBox(X1() - offset, Y1() - offset, Z1() - offset, X2() + offset, Y2() + offset, Z2() + offset);
        }

        public static float GetOverlap(double a1, double a2, double b1, double b2)
        {
            double v1 = Math.Max(a1, b1);
            double v2 = Math.Min(a2, b2);
            return (float)(v2 - v1);//negative, 0, or positive
                                    //return (float)(v2-v1);
        }

        public float GetOverlapVolume(BoundingBox r)
        {
            float oY = GetOverlap(r.Y1(), r.Y2(), Y1(), Y2());
            if (oY <= 0) return 0;
            float oX = GetOverlap(r.X1(), r.X2(), X1(), X2());
            if (oX <= 0) return 0;
            float oZ = GetOverlap(r.X1(), r.Z2(), Z1(), Z2());
            if (oZ <= 0) return 0;
            return oX * oY * oZ;
        }

        public float GetVolume()
        {
            return Math.Abs(GetDimX() * GetDimY() * GetDimZ());
        }

        public float GetAreaXY()
        {
            return Math.Abs(GetDimX() * GetDimY());
        }

        public bool IsOverlap(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            float oY = GetOverlap(y1, y2, this.Y1(), this.Y2());
            if (oY <= 0) return false;
            float oX = GetOverlap(x1, x2, this.X1(), this.X2());
            if (oX <= 0) return false;
            float oZ = GetOverlap(z1, z2, this.Z1(), this.Z2());
            if (oZ <= 0) return false;
            return true;
        }

        public bool IsOverlap(BoundingBox r)
        {
            float oY = GetOverlap(r.Y1(), r.Y2(), Y1(), Y2());
            if (oY <= 0) return false;
            float oX = GetOverlap(r.X1(), r.X2(), X1(), X2());
            if (oX <= 0) return false;
            float oZ = GetOverlap(r.Z1(), r.Z2(), Z1(), Z2());
            if (oZ <= 0) return false;
            return true;
        }





        public float Distance(float x, float y, float z)
        {
            Vec3 cen = this.GetCenter();
            float a = this.GetDimX();
            float b = this.GetDimY();
            float c = this.GetDimZ();

            float dx = Math.Abs(x - cen.x) - (a / 2f);
            float dy = Math.Abs(y - cen.y) - (b / 2f);
            float dz = Math.Abs(z - cen.z) - (c / 2f);
            float inside = Math.Max(dx, Math.Max(dy, dz));
            dx = Math.Max(dx, 0);
            dy = Math.Max(dy, 0);
            dz = Math.Max(dz, 0);
            float corner = (float)(Math.Sqrt(dx * dx + dy * dy + dz * dz));
            return (inside > 0) ? corner : inside;
        }



        public bool GetInside(float x, float y)
        {
            if (x < X2() && x >= X1() && y < Y2() && y >= Y1()) return true;
            return false;
        }

        public bool Contains(float x, float y, float z)
        {
            if (x < X2() && x >= X1() && y < Y2() && y >= Y1() && z < Z2() && z >= Z1()) return true;
            return false;
        }



        public float X1()
        {
            // TODO Auto-generated method stub
            return x1;
        }


        public float Y1()
        {
            // TODO Auto-generated method stub
            return y1;
        }


        public float Z1()
        {
            // TODO Auto-generated method stub
            return z1;
        }


        public float X2()
        {
            // TODO Auto-generated method stub
            return x2;
        }


        public float Y2()
        {
            // TODO Auto-generated method stub
            return y2;
        }


        public float Z2()
        {
            // TODO Auto-generated method stub
            return z2;
        }


        public void X1(float x)
        {
            // TODO Auto-generated method stub
            this.x1 = x;

        }


        public void Y1(float y)
        {
            // TODO Auto-generated method stub
            this.y1 = y;
        }


        public void Z1(float z)
        {
            // TODO Auto-generated method stub
            this.z1 = z;
        }


        public void X2(float x)
        {
            // TODO Auto-generated method stub
            this.x2 = x;
        }


        public void Y2(float y)
        {
            // TODO Auto-generated method stub
            this.y2 = y;
        }


        public void Z2(float z)
        {
            // TODO Auto-generated method stub
            this.z2 = z;
        }

        public float GetDimX()
        {
            return X2() - X1();
        }
        public float GetDimY()
        {
            return Y2() - Y1();
        }
        public float GetDimZ()
        {
            return Z2() - Z1();
        }
        public float GetCenterX()
        {
            // TODO Auto-generated method stub
            return (X1() + X2()) / 2f;
        }
        public float GetCenterY()
        {
            // TODO Auto-generated method stub
            return (Y1() + Y2()) / 2f;
        }
        public float GetCenterZ()
        {
            // TODO Auto-generated method stub
            return (Z1() + Z2()) / 2f;
        }
        public Vec3 GetCenter()
        {
            // TODO Auto-generated method stub
            return new Vec3(GetCenterX(), GetCenterY(), GetCenterZ());
        }
    }
}