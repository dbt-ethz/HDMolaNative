// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Mola
{

    // Representation of rays.
    public partial struct Ray : IFormattable
    {
        private Vec3 m_Origin;
        private Vec3 m_Direction;

        // Creates a ray starting at /origin/ along /direction/.
        public Ray(Vec3 origin, Vec3 direction)
        {
            m_Origin = origin;
            m_Direction = direction.normalized;
        }

        // The origin point of the ray.
        public Vec3 origin
        {
            get { return m_Origin; }
            set { m_Origin = value; }
        }

        // The direction of the ray.
        public Vec3 direction
        {
            get { return m_Direction; }
            set { m_Direction = value.normalized; }
        }

        // Returns a point at /distance/ units along the ray.
        public Vec3 GetPoint(float distance)
        {
            return m_Origin + m_Direction * distance;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return  m_Origin.ToString(format, formatProvider)+" "+ m_Direction.ToString(format, formatProvider);
        }
    }
}