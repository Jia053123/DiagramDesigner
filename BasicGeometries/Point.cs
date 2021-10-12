using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DiagramDesignerEngine.UnitTests")]
namespace BasicGeometries
{
	public readonly struct Point: IEquatable<Point>
    {
        // setting these to internal in order to make them readonly to the ViewModel
        public double coordinateX { get; }
        public double coordinateY { get; }

        public Point(double x, double y)
        {
			this.coordinateX = x; 
			this.coordinateY = y; 
        }

		/// <summary>
		/// Assume there is a vector from this point towards the input point, 
		/// calculate the angle from the (1, 0) vector to this vector, counter-clockwise. 
		/// This value is always between -Pi and Pi
		/// </summary>
		/// <param name="point"> the point towards which the angle is measured. Cannot be identical to this </param>
		public double AngleTowardsPoint(Point point)
		{
			if (this == point)
			{
				throw new ArgumentException("the other point cannot be the same as this point");
			}
			var result = Math.Atan2(point.coordinateY - this.coordinateY, point.coordinateX - this.coordinateX);
			return result;
		}

		public override int GetHashCode() => (this.coordinateX, this.coordinateY).GetHashCode();

		public static bool operator == (Point lhs, Point rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator != (Point lhs, Point rhs) => !(lhs == rhs);

		public override bool Equals(object obj) => obj is Point other && this.Equals(other);
		bool IEquatable<Point>.Equals(Point other) => this.Equals(other);

		private bool Equals(Point p)
		{
			return (Math.Abs(this.coordinateX - p.coordinateX) <= double.Epsilon &&
				Math.Abs(this.coordinateY - p.coordinateY) <= double.Epsilon);
		}

		public override string ToString()
		{
			return String.Concat("(", this.coordinateX.ToString(), " ", this.coordinateY.ToString(), ")");
		}

		public static double DistanceBetweenPoints(Point p1, Point p2)
		{
			var diffX = p1.coordinateX - p2.coordinateX;
			var diffY = p1.coordinateY - p2.coordinateY;
			return Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
		}
	}
}
