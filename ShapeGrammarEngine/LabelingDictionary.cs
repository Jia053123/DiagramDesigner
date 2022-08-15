using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	/// <summary>
	/// Stores the one to one mapping between the points in a geometry and their corresponding shape labels. 
	/// The labels and points are guaranteed to be unique
	/// </summary>
	public class LabelingDictionary
	{
		private Dictionary<Point, int> dictionary;
		private Dictionary<int, Point> reversedDictionary;
		private HashSet<Point> allPoints;
		private HashSet<int> allLabels;
		public int Count { get { return this.dictionary.Count; } }

		public LabelingDictionary()
		{
			this.dictionary = new Dictionary<Point, int>();
			this.reversedDictionary = new Dictionary<int, Point>();
			this.allPoints = new HashSet<Point>();
			this.allLabels = new HashSet<int>();
		}

		private LabelingDictionary(Dictionary<Point, int> dic, Dictionary<int, Point> revDic, HashSet<Point> allP, HashSet<int> allL)
		{
			this.dictionary = dic;
			this.reversedDictionary = revDic;
			this.allPoints = allP;
			this.allLabels = allL;
		}

		public HashSet<Point> GetAllPoints()
		{
			return new HashSet<Point>(this.allPoints);
		}

		public HashSet<int> GetAllLabels()
		{
			return new HashSet<int>(this.allLabels);
		}

		public bool DoesContainPair(Point point, int label)
		{
			if (this.GetAllPoints().Contains(point))
			{
				if (this.GetLabelByPoint(point) == label)
				{
					return true;
				}
			}
			return false;
		}

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

		public bool Remove(Point point)
		{
			if (this.allPoints.Contains(point))
			{
				int label;
				this.dictionary.TryGetValue(point, out label);

				this.dictionary.Remove(point);
				this.reversedDictionary.Remove(label);
				this.allPoints.Remove(point);
				this.allLabels.Remove(label);

				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(int label)
		{
			if (this.allLabels.Contains(label))
			{
				Point point;
				this.reversedDictionary.TryGetValue(label, out point);

				this.dictionary.Remove(point);
				this.reversedDictionary.Remove(label);
				this.allPoints.Remove(point);
				this.allLabels.Remove(label);

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

		/// <summary>
		/// Return a deep-copied duplicate instance
		/// </summary>
		public LabelingDictionary Copy()
		{
			return new LabelingDictionary ( 
				new Dictionary<Point, int>(this.dictionary),
				new Dictionary<int, Point>(this.reversedDictionary),
				new HashSet<Point>(this.allPoints),
				new HashSet<int>(this.allLabels));
		}
	}
}
