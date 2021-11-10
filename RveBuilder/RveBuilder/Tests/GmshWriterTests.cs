using System;
using System.Collections.Generic;
using System.Text;
using RveBuilder.Geometry;
using RveBuilder.Gmsh;
using Xunit;

namespace RveBuilder.Tests
{
	public static class GmshWriterTests
	{
		[Fact]
		public static void WriteFile()
		{
			string path = @"C:\Users\Serafeim\Desktop\gmsh inclusions\results\test1.geo";
			double[] minCoords = { -1, -1, -1 };
			double[] maxCoords = { +1, +1, +1 };
			var inclusions = new List<Sphere3D>();
			inclusions.Add(new Sphere3D(new double[] { -0.8, -0.8, -0.8 }, 0.35));
			inclusions.Add(new Sphere3D(new double[] { +0.5, -0.9, -0.8 }, 0.30));
			inclusions.Add(new Sphere3D(new double[] { +0.8, +0.5, -0.5 }, 0.45));
			var writer = new GmshCompositeWriter(minCoords, maxCoords, inclusions);
			writer.WriteFile(path);
		}
	}
}
