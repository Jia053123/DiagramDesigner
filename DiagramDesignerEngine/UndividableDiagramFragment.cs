using BasicGeometries;
using System;
using System.Collections.Generic;

namespace DiagramDesignerGeometryParser
{
	public class UndividableDiagramFragment: DiagramFragment
	{
		/// <param name="perimeter"> the perimeter of this fragment </param>
		/// <param name="innerPerimeters"> the inner perimeters. They must be inside the perimeter with no overlap, 
		/// and they must not interset or overlap with each other </param>
		internal UndividableDiagramFragment(CycleOfLineSegments perimeter, List<CycleOfLineSegments> innerPerimeters)
		{
			// inner perimeters check against the perimeter
			foreach (CycleOfLineSegments ip in innerPerimeters)
			{
				foreach (LineSegment l in ip.GetPerimeter())
				{
					// check each segment of the inner perimeters
					if (!perimeter.IsLineSegmentInCycle(l))
					{
						throw new ArgumentException("an inner perimeter segment is outside the perimeter");
					}
					foreach (LineSegment pl in perimeter.GetPerimeter())
					{
						if (LineSegment.DoOverlap(pl, l))
						{
							throw new ArgumentException("an inner perimeter segment overlap with part of the perimeter");
						}
					}
				}
			}
			// inner perimeters check against each other
			for (int i = 0; i < innerPerimeters.Count; i++)
			{
				for (int j = i + 1; j < innerPerimeters.Count; j++)
				{
					var innerPeri1 = innerPerimeters[i];
					var innerPeri1List = innerPeri1.GetPerimeter();
					var innerPeri2 = innerPerimeters[j];
					var innerPeri2List = innerPeri2.GetPerimeter();

					foreach (LineSegment ls in innerPeri2List)
					{
						if (innerPeri1.IsLineSegmentInCycle(ls))
						{
							throw new ArgumentException("an inner perimeter segment is in another inner perimeter");
						}
					}
					foreach (LineSegment ls in innerPeri1List)
					{
						if (innerPeri2.IsLineSegmentInCycle(ls))
						{
							throw new ArgumentException("an inner perimeter segment is in another inner perimeter");
						}
					}

					for (int a = 0; a < innerPeri1List.Count; a++)
					{
						for (int b = 0; b < innerPeri2List.Count; b++)
						{
							var ls1 = innerPeri1List[a];
							var ls2 = innerPeri2List[b];
							if (LineSegment.DoOverlap(ls1, ls2))
							{
								throw new ArgumentException("two inner perimeter segments overlap");
							}
						}
					}
				}
			}

			this.Perimeter = perimeter;
			//this.SegmentsWithin = segmentsWithinPerimeter;
			this.InnerPerimeters = innerPerimeters;
		}

		public double CalculateFragmentArea()
		{
			double perimeterArea = this.ShoelaceArea(this.GetPerimeterInPoints());
			double innerPerimetersArea = 0;
			foreach (List<Point> ip in this.GetInnerPerimetersInPoints())
			{
				innerPerimetersArea += this.ShoelaceArea(ip);
			}

			return perimeterArea - innerPerimetersArea;
		}


		/// <summary>
		/// Source: https://rosettacode.org/wiki/Shoelace_formula_for_polygonal_area#C.23
		/// </summary>
		private double ShoelaceArea(List<Point> v)
		{
			int n = v.Count;
			double a = 0.0;
			for (int i = 0; i < n - 1; i++)
			{
				a += v[i].coordinateX * v[i + 1].coordinateY - v[i + 1].coordinateX * v[i].coordinateY;
			}
			return Math.Abs(a + v[n - 1].coordinateX * v[0].coordinateY - v[0].coordinateX * v[n - 1].coordinateY) / 2.0;
		}
	}
}
