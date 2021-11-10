using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RveBuilder.Geometry;

namespace RveBuilder.Gmsh
{
	public class GmshCompositeWriter
	{
		private readonly double[] minCoords;
		private readonly double[] maxCoords;
		private readonly List<Sphere3D> inclusions;
		private readonly double volumeFraction;

		public GmshCompositeWriter(double[] minCoords, double[] maxCoords, List<Sphere3D> inclusions, double volumeFraction = -1)
		{
			this.minCoords = minCoords;
			this.maxCoords = maxCoords;
			this.inclusions = inclusions;
			this.volumeFraction = volumeFraction;
		}

		public int NumCircleDiscretizationPoints { get; set; } = 12;

		public void WriteFile(string path)
		{
			using (var writer = new StreamWriter(path))
			{
				if (volumeFraction > 0)
				{
					writer.WriteLine($"// Volume fraction = {volumeFraction}");
				}

				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("// PARAMETERS");
				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("// Domain Parameters");
				writer.WriteLine($"xmin = {{ {minCoords[0]}, {minCoords[1]}, {minCoords[2]} }};");
				writer.WriteLine($"xmax = {{ {maxCoords[0]}, {maxCoords[1]}, {maxCoords[2]} }};");
				writer.WriteLine();
				writer.WriteLine("// Inclusion Parameters");
				writer.WriteLine($"numInclusions = {inclusions.Count};");
				WriteInclusions(writer);
				writer.WriteLine();
				writer.WriteLine("// Mesh Parameters");
				writer.WriteLine($"numCircleDiscretizationPoints = {NumCircleDiscretizationPoints};");
				writer.WriteLine();
				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("// GENERAL");
				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("SetFactory(\"OpenCASCADE\");");
				writer.WriteLine("Mesh.MshFileVersion = 2.2;");
				writer.WriteLine();
				writer.WriteLine("// Physical Groups");
				writer.WriteLine("matrix = 1;");
				writer.WriteLine("inclusions = 2;");
				writer.WriteLine();
				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("// GEOMETRIC ENTITIES");
				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("// This will preserve the physical group tags of the spherical inclusions after Constructive Solid Geometry operations");
				writer.WriteLine("Geometry.OCCBooleanPreserveNumbering = 1;");
				writer.WriteLine();
				writer.WriteLine("// Domain");
				writer.WriteLine("Box(1) = {xmin[0], xmin[1], xmin[2], xmax[0]-xmin[0], xmax[1]-xmin[1], xmax[2]-xmin[2]};");
				writer.WriteLine();
				writer.WriteLine("//Inclusions");
				writer.WriteLine("For i In { 0 : numInclusions-1 }");
				writer.WriteLine("  id = 2 + i;");
				writer.WriteLine("  tempID = 10 * numInclusions + i;");
				writer.WriteLine("  Sphere(tempID) = {xIncl[i], yIncl[i], zIncl[i], rIncl[i]};");
				writer.WriteLine("  BooleanIntersection(id) = { Volume{tempID}; Delete; }{ Volume{1}; };");
				writer.WriteLine("EndFor");
				writer.WriteLine("Physical Volume(inclusions) = {2:numInclusions+1};");
				writer.WriteLine();
				writer.WriteLine("// BooleanFragments will intersect the box with the spherical inclusions, in a conformal manner (without creating duplicate interfaces). The original volumes must be deleted.");
				writer.WriteLine("volumes() = BooleanFragments{ Volume{1}; Delete; }{ Volume{2:numInclusions+1}; Delete; };");
				writer.WriteLine();
				writer.WriteLine("// The physical volume tags of the original spheres are retained during BooleanFragments.");
				writer.WriteLine("// However, the physical volume tag of the outside box must be set (actually I am not sure that it would not be retained as well, but tutorial16.geo did it this way).");
				writer.WriteLine("// This is the last entry in the volumes that BooleanFragments outputs.");
				writer.WriteLine("Physical Volume(matrix) = volumes(#volumes()-1); // The symbol # returns the length of a list");
				writer.WriteLine();
				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("// MESH CONFIG");
				writer.WriteLine("// *******************************************************************************************");
				writer.WriteLine("// Adapt mesh sizes with respect to curvature of geometric entities.");
				writer.WriteLine("Mesh.MeshSizeFromCurvature = numCircleDiscretizationPoints; // This is the number of line segments used to approximate 2*pi radians");
				writer.WriteLine();
				writer.WriteLine("// Use elements with quadratic shape functions");
				writer.WriteLine("//Mesh.ElementOrder = 2;");
				writer.WriteLine("//Mesh.HighOrderOptimize = 2;");
				writer.WriteLine();
				writer.WriteLine("// These next lines are used to eliminate duplicate nodes. They are probably not necessary for this model.");
				writer.WriteLine("//Mesh 3;");
				writer.WriteLine("//Coherence Mesh;");
				
			}
		}

		private void WriteInclusions(StreamWriter writer)
		{
			// E.g 
			// xIncl = { -0.8, +0.5, +0.8, -0.5, -0.5, +0.5, +0.5, -0.5 };
			// yIncl = { -0.8, -0.9, +0.5, +0.5, -0.5, -0.5, +0.5, +0.5 };
			// zIncl = { -0.8, -0.8, -0.5, -0.5, +0.5, +0.5, +0.5, +0.5 };
			// rIncl = { 0.35, 0.30, 0.45, 0.40, 0.25, 0.20, 0.35, 0.30 };
			writer.Write($"xIncl = {{ {inclusions[0].Center[0]}"); ;
			for (int i = 1; i < inclusions.Count; ++i)
			{
				writer.Write($", {inclusions[i].Center[0]}");
			}
			writer.WriteLine(" };");

			writer.Write($"yIncl = {{ {inclusions[0].Center[1]}"); ;
			for (int i = 1; i < inclusions.Count; ++i)
			{
				writer.Write($", {inclusions[i].Center[1]}");
			}
			writer.WriteLine(" };");

			writer.Write($"zIncl = {{ {inclusions[0].Center[2]}"); ;
			for (int i = 1; i < inclusions.Count; ++i)
			{
				writer.Write($", {inclusions[i].Center[2]}");
			}
			writer.WriteLine(" };");

			writer.Write($"rIncl = {{ {inclusions[0].Radius}"); ;
			for (int i = 1; i < inclusions.Count; ++i)
			{
				writer.Write($", {inclusions[i].Radius}");
			}
			writer.WriteLine(" };");
		}
	}
}
