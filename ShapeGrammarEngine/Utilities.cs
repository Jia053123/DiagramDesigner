using ListOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using Svg;
using Svg.Pathing;
using System.Drawing;
using MyPoint = BasicGeometries.Point;

namespace ShapeGrammarEngine
{
	static internal class Utilities
	{
		/// <summary>
		/// Generate all unique permutations of a list
		/// </summary>
		/// <returns> All unique permutations </returns>
		static internal List<List<T>> GenerateAllPermutations<T>(List<T> listToPermutate) where T : IEquatable<T>
		{
			if (listToPermutate.Count == 0)
			{
				throw new ArgumentException("listToPermutate must have at least one entry");
			}

			List < List < T >> helper(int beginningIndex, int endIndex)
			{
				// terminal case: if there is only one member, output that member
				if (beginningIndex == endIndex)
				{
					return new List<List<T>> { new List<T> { listToPermutate[beginningIndex] } };
				}

				// otherwise, get all permutations of all members other than the first one, 
				// then for each permutation in the result insert the first member at all possible locations, creating the new set of permutations
				var result = helper(beginningIndex + 1, endIndex);
				var output = new List<List<T>>();
				foreach (List<T> permutation in result)
				{
					for (int i = 0; i < permutation.Count + 1; i++)
					{
						var newPermutation = new List<T>(permutation);
						newPermutation.Insert(i, listToPermutate[beginningIndex]);
						output.Add(newPermutation);
					}
				}
				return output;
			}

			var permutations = helper(0, listToPermutate.Count - 1);
			var duplicateIndexes = new List<int>();
			for (int i = 0; i < permutations.Count; i++)
			{
				for (int j = i+1; j < permutations.Count; j++)
				{
					if (ListUtilities.AreContentsEqualInOrder(permutations[i], permutations[j]))
					{
						duplicateIndexes.Add(j);
					}
				}
			}

			duplicateIndexes.Sort();
			duplicateIndexes.Reverse();
			foreach (int indexToRemove in duplicateIndexes)
			{
				permutations.RemoveAt(indexToRemove);
			}

			return permutations;
		}

		static internal double CalculateVariance(List<double> data)
		{
			// calculate average
			var average = data.Sum() / data.Count;
			// calculate variance
			double sumOfSquaredDiff = 0;
			foreach (double entry in data)
			{
				sumOfSquaredDiff += Math.Pow(entry - average, 2);
			}
			return sumOfSquaredDiff / data.Count;
		}

		/// <summary>
		/// Render a PolylinesGeometry object as svg. The polylines will be squeezed within the specified dimension
		/// </summary>
		/// <param name="polyGeo"> The PolylineGeometry to render </param>
		/// <param name="width"> Width of the resulting svg document </param>
		/// <param name="height"> Height of the resulting svg document </param>
		/// <param name="strokeWidth"> Width of the stroke used the render the geometry; Recommend even number </param>
		/// <returns> A SVG document with the specified dimensions </returns>
		static internal SvgDocument PolylinesGeometryToSvg(PolylinesGeometry polyGeo, int width, int height, int strokeWidth)
		{
			SvgDocument shapeDoc = new SvgDocument();
			shapeDoc.Width = width;
			shapeDoc.Height = height;
			shapeDoc.ViewBox = new SvgViewBox(0, 0, width, height);

			var sp = new SvgPath();
			var spsl = new SvgPathSegmentList();

			var boundingBox = polyGeo.GetBoundingBox();
			double xDelta = -1 * boundingBox.xMin;
			double yDelta = -1 * boundingBox.yMax;
			double xFactor = width / (boundingBox.xMax - boundingBox.xMin);
			double yFactor = height / (boundingBox.yMax - boundingBox.yMin);
			foreach (List<MyPoint> polyline in polyGeo.PolylinesCopy)
			{
				for (int i = 0; i < polyline.Count; i++)
				{
					MyPoint p = polyline[i];
					int mappedX = Convert.ToInt32((p.coordinateX + xDelta) * xFactor);
					int mappedY = -1 * Convert.ToInt32((p.coordinateY + yDelta) * yFactor);
					if (i == 0)
					{
						spsl.Add(new SvgMoveToSegment(false, new PointF(mappedX, mappedY)));
					}
					else
					{
						spsl.Add(new SvgLineSegment(false, new PointF(mappedX, mappedY)));
					}
				}
			}

			sp.PathData = spsl;
			sp.StrokeWidth = strokeWidth;
			sp.Stroke = new SvgColourServer(Color.Black);
			sp.Fill = null;

			shapeDoc.Children.Add(sp);

			return shapeDoc;
		}

		/// <summary>
		/// Render a PolylinesGeometry object as svg. The polylines will be squeezed within the specified dimension with padding to accommodate the stroke width
		/// </summary>
		/// <param name="polyGeo"> The PolylineGeometry to render </param>
		/// <param name="width"> Width of the resulting svg document </param>
		/// <param name="height"> Height of the resulting svg document </param>
		/// <param name="strokeWidth"> Width of the stroke used the render the geometry; Recommend even number </param>
		/// <returns> A SVG document with the specified dimensions </returns>
		static internal SvgDocument PolylinesGeometryToSvgPadded(PolylinesGeometry polyGeo, int width, int height, int strokeWidth)
        {
			SvgDocument shapeDoc = new SvgDocument();
			shapeDoc.Width = width;
			shapeDoc.Height = height;
			shapeDoc.ViewBox = new SvgViewBox(0, 0, width, height);

			var sp = new SvgPath();
			var spsl = new SvgPathSegmentList();

			var paddedWidth = width - strokeWidth;
			var paddedHeight = height - strokeWidth;
			var boundingBox = polyGeo.GetBoundingBox();
			double xDelta = -1 * boundingBox.xMin;
			double yDelta = -1 * boundingBox.yMax;
			double xFactor = paddedWidth / (boundingBox.xMax - boundingBox.xMin);
			double yFactor = paddedHeight / (boundingBox.yMax - boundingBox.yMin);
			foreach (List<MyPoint> polyline in polyGeo.PolylinesCopy)
			{
				for (int i = 0; i < polyline.Count; i++)
				{
					MyPoint p = polyline[i];
					int mappedX = Convert.ToInt32(strokeWidth/2 + (p.coordinateX + xDelta) * xFactor);
					int mappedY = Convert.ToInt32(strokeWidth/2 - (p.coordinateY + yDelta) * yFactor);
					if (i == 0)
                    {
						spsl.Add(new SvgMoveToSegment(false, new PointF(mappedX, mappedY)));
                    }
					else
                    {
						spsl.Add(new SvgLineSegment(false, new PointF(mappedX, mappedY)));
                    }
				}
            }
	
			sp.PathData = spsl;
			sp.StrokeWidth = strokeWidth;
			sp.Stroke = new SvgColourServer(Color.Black);
			sp.Fill = null;

			shapeDoc.Children.Add(sp);

			return shapeDoc;
        }
	}
}
