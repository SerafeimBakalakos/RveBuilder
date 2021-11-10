using System;
using System.Collections.Generic;
using System.Text;

namespace RveBuilder.Geometry
{
    public class Sphere3D
    {
        public Sphere3D(double centerX, double centerY, double centerZ, double radius)
        {
            this.Center = new double[] { centerX, centerY, centerZ };
            this.Radius = radius;
        }

        public Sphere3D(double[] center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public double[] Center { get; }
        public double Radius { get; }

        public int Dimension => 3;

        public static double CalcRadiusFromVolume(double volume) => Math.Pow(3.0 / 4.0 * volume / Math.PI, 1.0 / 3.0);

        public bool CollidesWith(Sphere3D other)
        {
            double dx = other.Center[0] - this.Center[0];
            double dy = other.Center[1] - this.Center[1];
            double dz = other.Center[2] - this.Center[2];
            double centerDistance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return centerDistance <= this.Radius + other.Radius;
        }

        public bool IsPointInside(double[] point)
        {
            double dx = point[0] - Center[0];
            double dy = point[1] - Center[1];
            double dz = point[2] - Center[2];
            double centerDistance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return centerDistance <= Radius;
        }

        public double SignedDistanceOf(double[] point)
        {
            double dx = point[0] - Center[0];
            double dy = point[1] - Center[1];
            double dz = point[2] - Center[2];
            double centerDistance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return centerDistance - Radius;
        }

        public double Volume() => 4.0 * Math.PI * Radius * Radius * Radius / 3.0;

    }
}
