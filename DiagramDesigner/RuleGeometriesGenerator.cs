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
		public double displayUnitOverRealUnit = 1;

		internal RuleGeometriesGenerator(double displayUnitOverRealUnit)
		{
			this.displayUnitOverRealUnit = displayUnitOverRealUnit;
		}

		internal Tuple<PolylinesGeometry, PolylinesGeometry> GenerateGeometriesFromContextAndAdditions(List<List<WinPoint>> AllWalls, List<Tuple<int, int, int>> ContextWalls, List<Tuple<int, int, int>> AdditionWalls)
		{
			
		}
	}
}
