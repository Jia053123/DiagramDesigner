using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("DiagramDesignerEngine.UnitTests")]
namespace DiagramDesignerEngine
{
    public readonly struct Point: IEquatable<Point>
    {
        //public const int DIGITS_FOR_ROUNDING = 3;

        // setting these to internal in order to make them readonly to the ViewModel
        public double coordinateX { get; }
        public double coordinateY { get; }

        public Point(double x, double y)
        {
			this.coordinateX = x; //(Math.Round(x, DIGITS_FOR_ROUNDING));
			this.coordinateY = y; //(Math.Round(y, DIGITS_FOR_ROUNDING));
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
			return (this.coordinateX == p.coordinateX && this.coordinateY == p.coordinateY);
		}

		public override string ToString()
		{
			return String.Concat("(", this.coordinateX.ToString(), " ", this.coordinateY.ToString(), ")");
		}
	}
}
