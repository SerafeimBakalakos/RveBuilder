using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RveBuilder.Geometry
{
	public class SphereBrickIntersection
	{
		public enum RelativePosition
		{
			SphereInsideBrick, SphereOutsideBrick, PartialIntersection
		}

		private readonly long numIntegrationPoints;
		private readonly Random rng;

		public SphereBrickIntersection(long numIntegrationPoints, int seed = int.MinValue)
		{
			this.numIntegrationPoints = numIntegrationPoints;
			if (seed == int.MinValue)
			{
				rng = new Random();
			}
			else
			{
				rng = new Random(seed);
			}
		}

		/// <summary>
		/// Checks if this sphere lies entirely inside a brick
		/// </summary>
		/// <param name="brick"></param>
		/// <returns></returns>
		public bool IsInside(Brick3D brick, Sphere3D sphere)
		{
			double[] faceDistances = brick.SignedDistancesFromFaces(sphere.Center);
			for (int i = 0; i < faceDistances.Length; ++i)
			{
				if (faceDistances[i] > 0)
				{
					// Center lies outside
					return false;
				}
				else if (Math.Abs(faceDistances[i]) < sphere.Radius)
				{
					// Center lies inside but not the whole sphere
					return false;
				}
			}
			return true;
		}

		public RelativePosition FindRelativePosition(Brick3D brick, Sphere3D sphere)
		{
			double[] faceDistances = brick.SignedDistancesFromFaces(sphere.Center);
			int numAxesInside = 0;
			int numAxesOutside = 0;
			int numAxesIntersected = 0;
			for (int d = 0; d < 3; ++d)
			{
				// For any axis: ------min-------max-------
				double distanceMinFace = faceDistances[2 * d];
				double distanceMaxFace = faceDistances[2 * d + 1];

				if (distanceMinFace > 0) // Center lies before min
				{
					if (sphere.Radius <= distanceMinFace) // Whole sphere lies before min
					{
						++numAxesOutside;
					}
					else
					{
						++numAxesIntersected;
					}
				}
				else if (distanceMaxFace > 0) // Center lies after max
				{
					if (sphere.Radius <= distanceMaxFace) // Whole sphere lies after max
					{
						++numAxesOutside;
					}
					else
					{
						++numAxesIntersected;
					}
				}
				else // Center lies between min and max
				{
					if ((sphere.Radius <= Math.Abs(distanceMinFace)) && (sphere.Radius <= Math.Abs(distanceMaxFace)))
					{ // Whole sphere lies between min and max
						++numAxesInside;
					}
					else
					{
						++numAxesIntersected;
					}
				}
			}
			if (numAxesOutside > 0)
			{
				return RelativePosition.SphereOutsideBrick;
			}
			else if (numAxesIntersected > 0)
			{
				return RelativePosition.PartialIntersection;
			}
			else
			{
				return RelativePosition.SphereInsideBrick;
			}
		}

		public double EstimateIntersectionVolume(Brick3D brick, Sphere3D sphere)
		{
			double diameter = 2 * sphere.Radius;
			double minX = sphere.Center[0] - sphere.Radius;
			double minY = sphere.Center[1] - sphere.Radius;
			double minZ = sphere.Center[2] - sphere.Radius;
			long numPointsInSphere = 0;
			long numPointsInIntersection = 0;
			for (long i = 0; i < numIntegrationPoints; ++i)
			{
				double[] point = 
				{ 
					minX + rng.NextDouble() * diameter, minY + rng.NextDouble() * diameter, minZ + rng.NextDouble() * diameter 
				};
				if (sphere.IsPointInside(point))
				{
					++numPointsInSphere;
					if (brick.IsPointInside(point))
					{
						++numPointsInIntersection;
					}
				}
			}

			if (numPointsInSphere == 0)
			{
				throw new Exception("Too few integration points");
			}
			return (numPointsInIntersection * sphere.Volume()) / numPointsInSphere;

		}
	}
}
