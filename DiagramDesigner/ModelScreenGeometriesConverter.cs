using BasicGeometries;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	/// <summary>
	/// Handle conversions between display geometries and Model geometries
	/// </summary>
	class ModelScreenGeometriesConverter
	{
		private double displayUnitOverRealUnit;

		internal ModelScreenGeometriesConverter(double displayUnitOverRealUnit)
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
            var leftHandPoints = this.ConvertGeometriesOnScreenToPolylinesInPoints(allGeometries, contextGeometries); 
            PolylinesGeometry leftHandGeometry = new PolylinesGeometry(leftHandPoints);

            // create right hand geometry. Currently assume no point is erased from the left hand geometry so the right hand points = left hand points + added points, though this will not be the case once erasure comes into play
            var rightHandPoints = new List<List<Point>>();
            foreach (List<Point> ps in leftHandPoints)
			{
                rightHandPoints.Add(new List<Point>(ps));
			}
            rightHandPoints.AddRange(this.ConvertGeometriesOnScreenToPolylinesInPoints(allGeometries, additionGeometries));
            PolylinesGeometry rightHandGeometry = new PolylinesGeometry(rightHandPoints);

            return new Tuple<PolylinesGeometry, PolylinesGeometry>(leftHandGeometry, rightHandGeometry);
        }

        /// <summary>
        /// Generate PolylinesGeometry given the geometry on screen
        /// </summary>
        /// <param name="allGeometries"> all geometries on screen </param>
        /// <param name="contextGeometries"> the geometry used for generation; 
        ///  Each Tuple specifies the index of the geometry within allGeometries, and the two ascending consecutive indexes indicating the line segment within the geometry </param>
        /// <returns> the PolylinesGeometry generated </returns>
        internal PolylinesGeometry GenerateLeftHandGeometryFromContext(List<List<WinPoint>> allGeometries, List<Tuple<int, int, int>> contextGeometries)
		{
            var leftHandPoints = this.ConvertGeometriesOnScreenToPolylinesInPoints(allGeometries, contextGeometries);
			return new PolylinesGeometry(leftHandPoints);
		}

		private List<List<Point>> ConvertGeometriesOnScreenToPolylinesInPoints(List<List<WinPoint>> allGeometriesOnScreen, List<Tuple<int, int, int>> polylinesIndexes)
		{
            var polylinesInPoints = new List<List<Point>>();
            foreach (Tuple<int, int, int> t in polylinesIndexes)
            {
                if (t.Item3 != t.Item2 + 1)
				{
                    throw new ArgumentException("on polyline index {0}, the indexes for the two points forming the line is not consecutive in ascending order");
				}
                var wp1 = allGeometriesOnScreen[t.Item1][t.Item2];
                var p1 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp1, this.displayUnitOverRealUnit);
                var wp2 = allGeometriesOnScreen[t.Item1][t.Item3];
                var p2 = MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(wp2, this.displayUnitOverRealUnit);
                polylinesInPoints.Add(new List<Point> { p1, p2 });
            }
            return polylinesInPoints;
        }

        /// <summary>
        /// Convert single points and line segments represented by Points in real world unit to ones represented by WinPoints in screen unit
        /// </summary>
        /// <param name="polylinesInPoints"> line segments in real world unit to convert </param>
        /// <returns> the result of the conversion with list structures and orders intact </returns>
        internal List<List<WinPoint>> ConvertPolylinesInPointsToGeometriesOnScreen(List<List<Point>> polylinesInPoints)
		{
            if (polylinesInPoints is null)
			{
                throw new ArgumentNullException("polylinesInPoints is null");
			}
            var polylinesInWinPoints = new List<List<WinPoint>>();
            foreach (List<Point> polyline in polylinesInPoints)
			{
                polylinesInWinPoints.Add(new List<WinPoint>());
                foreach(Point p in polyline)
				{
                    var wp = MathUtilities.ConvertRealScaledPointToWindowsPointOnScreen(p, this.displayUnitOverRealUnit);
                    polylinesInWinPoints.Last().Add(wp);
				}
			}
            return polylinesInWinPoints;
		}
    }
}
