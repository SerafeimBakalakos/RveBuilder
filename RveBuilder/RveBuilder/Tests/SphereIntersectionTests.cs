using System;
using System.Collections.Generic;
using System.Text;
using RveBuilder.Geometry;
using Xunit;

namespace RveBuilder.Tests
{
	public static class SphereIntersectionTests
	{
		[Fact]
		public static void TestFullSphereVolume()
		{
			var sphere = new Sphere3D(new double[] { 0, 0, 0 }, 0.5);
			var brick = new Brick3D(new double[] { -1, -1, -1 }, new double[] { +1, +1, +1});
			int numIntegrationPoints = 100;
			int seed = 13;
			var intersection = new SphereBrickIntersection(numIntegrationPoints, seed);
			double volumeComputed = intersection.EstimateIntersectionVolume(brick, sphere);
			double volumeExpected = sphere.Volume();
			Assert.Equal(volumeExpected, volumeComputed, 4);
		}

		[Fact]
		public static void TestHalfSphereVolume()
		{
			var sphere = new Sphere3D(new double[] { 1, 0, 0 }, 0.5);
			var brick = new Brick3D(new double[] { -1, -1, -1 }, new double[] { +1, +1, +1 });
			int numIntegrationPoints = 100000000;
			int seed = 13;
			var intersection = new SphereBrickIntersection(numIntegrationPoints, seed);
			double volumeComputed = intersection.EstimateIntersectionVolume(brick, sphere);
			double volumeExpected = 0.5 * sphere.Volume();
			Assert.Equal(volumeExpected, volumeComputed, 4);
		}

		[Fact]
		public static void TestOneFourthSphereVolume()
		{
			var sphere = new Sphere3D(new double[] { 1, 1, 0 }, 0.5);
			var brick = new Brick3D(new double[] { -1, -1, -1 }, new double[] { +1, +1, +1 });
			int numIntegrationPoints = 100000000;
			int seed = 13;
			var intersection = new SphereBrickIntersection(numIntegrationPoints, seed);
			double volumeComputed = intersection.EstimateIntersectionVolume(brick, sphere);
			double volumeExpected = 0.25 * sphere.Volume();
			Assert.Equal(volumeExpected, volumeComputed, 4);
		}
	}
}
