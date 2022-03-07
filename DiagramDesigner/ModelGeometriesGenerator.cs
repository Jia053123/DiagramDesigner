using BasicGeometries;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	/// <summary>
	/// Handle conversions between display geometries and Model geometries
	/// </summary>
	class ModelGeometriesGenerator
	{
		private double displayUnitOverRealUnit;

		internal ModelGeometriesGenerator(double displayUnitOverRealUnit)
		{
			this.displayUnitOverRealUnit = displayUnitOverRealUnit;
		}

        internal void UpdateDisplayUnitOverRealUnit(double newValue)
		{
            this.displayUnitOverRealUnit = newValue;
		}

        /// <summary>
        /// Generate geometries for rule creation from highlighted context and addition geometries on screen
        /// </summary>
        /// <param name="allGeometries"> all geometries on screen </param>
        /// <param name="contextGeometries"> the geometries highlighted as context from all geometries on screen; 
        /// each Tuple specifies the index of the geometry within allGeometries, and the two ascending consecutive indexes indicating the line segment within the geometry </param>
        /// <param name="additionGeometries"> the geometries highlighted as addition from all geometries on screen;
        /// each Tuple specifies the index of the geometry within allGeometries, and the two ascending consecutive indexes indicating the line segment within the geometry </param>
        /// <returns> a Tuple containing the geometry for the left hand side of the rule and the one for the right hand side </returns>
        internal Tuple<PolylinesGeometry, PolylinesGeometry> GenerateGeometriesFromContextAndAdditions(List<List<WinPoint>> allGeometries, List<Tuple<int, int, int>> contextGeometries, List<Tuple<int, int, int>> additionGeometries)
		{
            // create left hand geometry
            var leftHandPoints = new List<List<Point>>();
            foreach (Tuple<int, int, int> t in contextGeometries)
            {
                var wp1 = allGeometries[t.Item1][t.Item2];
                var p1 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp1, this.displayUnitOverRealUnit);
                var wp2 = allGeometries[t.Item1][t.Item3];
                var p2 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp2, this.displayUnitOverRealUnit);
                leftHandPoints.Add(new List<Point> { p1, p2 });
            }
            PolylinesGeometry leftHandGeometry = new PolylinesGeometry(leftHandPoints);

            // create right hand geometry
            var rightHandPoints = new List<List<Point>>(leftHandPoints); // currently assume no point is erased from the left hand geometry though this will not be the case
            foreach (Tuple<int, int, int> t in additionGeometries)
            {
                var wp1 = allGeometries[t.Item1][t.Item2];
                var p1 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp1, this.displayUnitOverRealUnit);
                var wp2 = allGeometries[t.Item1][t.Item3];
                var p2 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp2, this.displayUnitOverRealUnit);
                rightHandPoints.Add(new List<Point> { p1, p2 });
            }
            PolylinesGeometry rightHandGeometry = new PolylinesGeometry(rightHandPoints);

            return new Tuple<PolylinesGeometry, PolylinesGeometry>(leftHandGeometry, rightHandGeometry);
        }

        /// <summary>
        /// Generate PolylinesGeometry given the geometry on screen
        /// </summary>
        /// <param name="allGeometries"> all geometries on screen </param>
        /// <param name="geometryIndexes"> the geometry used for generation
        ///  each Tuple specifies the index of the geometry within allGeometries, and the two ascending consecutive indexes indicating the line segment within the geometry </param>
        /// <returns> the PolylinesGeometry generated </returns>
        internal PolylinesGeometry MakePolylinesGeometry(List<List<WinPoint>> allGeometries, List<Tuple<int, int, int>> geometryIndexes)
		{
            var points = new List<List<Point>>();
            foreach (Tuple<int, int, int> t in geometryIndexes)
            {
                var wp1 = allGeometries[t.Item1][t.Item2];
                var p1 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp1, this.displayUnitOverRealUnit);
                var wp2 = allGeometries[t.Item1][t.Item3];
                var p2 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp2, this.displayUnitOverRealUnit);
                points.Add(new List<Point> { p1, p2 });
            }
            return new PolylinesGeometry(points);
        }
	}
}
