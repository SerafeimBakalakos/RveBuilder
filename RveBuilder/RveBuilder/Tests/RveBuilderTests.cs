using System;
using System.Collections.Generic;
using System.Text;
using RveBuilder.Builders;
using RveBuilder.Geometry;
using RveBuilder.Gmsh;
using Xunit;

namespace RveBuilder.Tests
{
	public static class RveBuilderTests
	{
		[Fact]
		public static void CreateAndPrintRve()
		{
			string path = @"C:\Users\Serafeim\Desktop\gmsh inclusions\results\test2.geo";
			double[] minCoords = { -1, -1, -1 };
			double[] maxCoords = { +1, +1, +1 };
			double minRadius = 0.1;
			double maxRadius = 0.2;
			double targetVF = 0.5;
			var rveBuilder = new SphericalInclusionsRveBuilder(minCoords, maxCoords, minRadius, maxRadius, targetVF);
			rveBuilder.Seed = 1;
			(List<Sphere3D> inclusions, double actualVolumeFraction) = rveBuilder.GenerateInclusions();
			var writer = new GmshCompositeWriter(minCoords, maxCoords, inclusions, actualVolumeFraction);
			writer.NumCircleDiscretizationPoints = 12;
			writer.WriteFile(path);
		}
	}
}
