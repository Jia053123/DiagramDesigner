using BasicGeometries;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	/// <summary>
	/// Generates left-hand and right-hand geometries based on highlighted walls
	/// </summary>
	class RuleGeometriesGenerator
	{
		private double displayUnitOverRealUnit;

		internal RuleGeometriesGenerator(double displayUnitOverRealUnit)
		{
			this.displayUnitOverRealUnit = displayUnitOverRealUnit;
		}

        internal void UpdateDisplayUnitOverRealUnit(double newValue)
		{
            this.displayUnitOverRealUnit = newValue;
		}

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
	}
}
