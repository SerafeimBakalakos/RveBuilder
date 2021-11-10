using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using RveBuilder.Geometry;

namespace RveBuilder.Builders
{
	public class SphericalInclusionsRveBuilder
	{
		public SphericalInclusionsRveBuilder(double[] minCoords, double[] maxCoords, double minRadius, double maxRadius, 
			double targetVolumeFraction)
		{
			this.CoordsMin = minCoords;
			this.CoordsMax = maxCoords;
			this.RadiusMin = minRadius;
			this.RadiusMax = maxRadius;
			this.TargetVolumeFraction = targetVolumeFraction;
		}

		public double[] CoordsMin { get; set; }
		public double[] CoordsMax { get; set; }

		public int NumIntegrationPointsMC { get; set; } = 1000000;

		public int NumTriesPerInclusion { get; set; } = 100;

		public double RadiusMin { get; set; }
		public double RadiusMax { get; set; }

		public int Seed { get; set; } = 13;

		public double TargetVolumeFraction { get; set; }

		public (List<Sphere3D> inclusions, double actualVolumeFraction) GenerateInclusions()
		{
			double actualVolumeFraction = 0;
			double rveVolume = (CoordsMax[0] - CoordsMin[0]) * (CoordsMax[1] - CoordsMin[1]) * (CoordsMax[2] - CoordsMin[2]);
			var inclusions = new List<Sphere3D>();
			var rng = new Random(Seed);
			var domain = new Brick3D(CoordsMin, CoordsMax);
			var intersection = new SphereBrickIntersection(NumIntegrationPointsMC, Seed);
			while (actualVolumeFraction < TargetVolumeFraction)
			{
				// Create a new trial inclusion
				double r = RadiusMin + rng.NextDouble() * (RadiusMax - RadiusMin);
				var sphere = new Sphere3D(new double[3], r);
				double extraSpace = 0.5 * r;
				double[] lb = { CoordsMin[0] - extraSpace, CoordsMin[1] - extraSpace, CoordsMin[2] - extraSpace };
				double[] ub = { CoordsMax[0] + extraSpace, CoordsMax[1] + extraSpace, CoordsMax[2] + extraSpace };
				PlaceInclusionRandomly(sphere, rng, lb, ub, domain, intersection);

				// Try to fit the inclusion with the rest
				int t;
				for (t = 0; t < NumTriesPerInclusion; ++t)
				{
					if (CollidesWithOtherInclusions(sphere, inclusions))
					{
						PlaceInclusionRandomly(sphere, rng, lb, ub, domain, intersection);
					}
					else
					{
						inclusions.Add(sphere);
						break;
					}
				}
				if (t == NumTriesPerInclusion)
				{
					throw new NotImplementedException($"Could not fit sphere with radius={sphere.Radius}." +
						$" Current volume fraction = {actualVolumeFraction}. Target colume fraction = {TargetVolumeFraction}");
				}

				// Update volume
				SphereBrickIntersection.RelativePosition pos = intersection.FindRelativePosition(domain, sphere);
				if (pos == SphereBrickIntersection.RelativePosition.SphereInsideBrick)
				{
					actualVolumeFraction += sphere.Volume() / rveVolume;
				}
				else
				{
					Debug.Assert(pos == SphereBrickIntersection.RelativePosition.PartialIntersection);
					actualVolumeFraction += intersection.EstimateIntersectionVolume(domain, sphere) / rveVolume;
				}
			}
			return (inclusions, actualVolumeFraction);
		}

		private void PlaceInclusionRandomly(Sphere3D sphere, Random rng, double[] lb, double[] ub, 
			Brick3D domain, SphereBrickIntersection intersection)
		{
			while (true)
			{
				sphere.Center[0] = lb[0] + rng.NextDouble() * (ub[0] - lb[0]);
				sphere.Center[1] = lb[1] + rng.NextDouble() * (ub[1] - lb[1]);
				sphere.Center[2] = lb[2] + rng.NextDouble() * (ub[2] - lb[2]);

				SphereBrickIntersection.RelativePosition pos = intersection.FindRelativePosition(domain, sphere);
				if (pos != SphereBrickIntersection.RelativePosition.SphereOutsideBrick)
				{
					return;
				}
			}
		}

		private bool CollidesWithOtherInclusions(Sphere3D newInclusion, List<Sphere3D> existingInclusions)
		{
			foreach (Sphere3D other in existingInclusions)
			{
				if (newInclusion.CollidesWith(other))
				{
					return true;
				}
			}
			return false;
		}
	}
}
