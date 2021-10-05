using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	/// <summary>
	/// Works just like a built-in dictionary except each Point and corresponding label must be unique
	/// </summary>
	class LabelingDictionary
	{
		private Dictionary<Point, int> dictionary = new Dictionary<Point, int>();
		private Dictionary<int, Point> reversedDictionary = new Dictionary<int, Point>();
		private HashSet<Point> allPoints = new HashSet<Point>();
		private HashSet<int> allLabels = new HashSet<int>();

		public bool Add(Point point, int label)
		{
			if ((!this.allPoints.Contains(point)) && (!this.allLabels.Contains(label))) 
			{
				this.dictionary.Add(point, label);
				this.reversedDictionary.Add(label, point);
				this.allPoints.Add(point);
				this.allLabels.Add(label);
				return true;
			}
			else
			{
				return false;
			}
		}

		public Point GetPointByLabel(int label)
		{
			var s = this.reversedDictionary.TryGetValue(label, out var p);
			if (s)
			{
				return p;
			}
			else
			{
				throw new ArgumentException("label not found");
			}
		}

		public int GetLabelByPoint(Point point)
		{
			var s = this.dictionary.TryGetValue(point, out var l);
			if (s)
			{
				return l;
			}
			else
			{
				throw new ArgumentException("point not found");
			}
		}
	}
}
