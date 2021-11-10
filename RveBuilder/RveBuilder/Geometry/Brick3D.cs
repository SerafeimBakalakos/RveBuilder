using System;
using System.Collections.Generic;
using System.Text;

namespace RveBuilder.Geometry
{
	public class Brick3D
	{
		public Brick3D(double[] minCoords, double[] maxCoords)
		{
			this.MinCoords = new double[] { minCoords[0], minCoords[1], minCoords[2] };
			this.MaxCoords = new double[] { maxCoords[0], maxCoords[1], maxCoords[2] };
		}

		public double[] MinCoords { get; }
		public double[] MaxCoords { get; }

		public bool IsPointInside(double[] point)
		{
			return (point[0] >= MinCoords[0]) && (point[0] <= MaxCoords[0])
				&& (point[1] >= MinCoords[1]) && (point[1] <= MaxCoords[1])
				&& (point[2] >= MinCoords[2]) && (point[2] <= MaxCoords[2]);
		}

		/// <summary>
		/// Order of faces: x=minX, x=maxX, y=minY, y=maxY, z=minZ, z=maxZ
		/// Signed distance > 0: point is outside the brick
		/// Signed distance < 0: point is inside the brick
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public double[] SignedDistancesFromFaces(double[] point)
		{
			var result = new double[6];
			result[0] = MinCoords[0]- point[0];
			result[1] = point[0] - MaxCoords[0];
			result[2] = MinCoords[1] - point[1];
			result[3] = point[1] - MaxCoords[1];
			result[4] = MinCoords[2] - point[2];
			result[5] = point[2] - MaxCoords[2];
			return result;
		}
	}
}
