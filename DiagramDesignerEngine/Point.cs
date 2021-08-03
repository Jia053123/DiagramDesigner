using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleToAttribute("DiagramDesignerEngine.UnitTests")]
namespace DiagramDesignerEngine
{
    public class Point
    {
        public const double EQUALITY_TOLERANCE = 0.001;

        // setting these to internal in order to make them readonly to the ViewModel
        public double coordinateX { get; internal set; }
        public double coordinateY { get; internal set; }

        public Point(double x, double y)
        {
            this.coordinateX = x;
            this.coordinateY = y;
        }

        public static bool operator == (Point lhs, Point rhs)
		{
            if (lhs is null)
			{
                return false;
			}
            return lhs.Equals(rhs);
		}

        public static bool operator != (Point lhs, Point rhs) => !(lhs == rhs);

        public override bool Equals(object obj) => this.Equals(obj as Point);

        public override int GetHashCode() => (this.coordinateX, this.coordinateY).GetHashCode();

        private bool Equals(Point p)
		{
            if (p is null)
			{
                return false;
			}
            return (Math.Abs(this.coordinateX - p.coordinateX) < EQUALITY_TOLERANCE) &&
                (Math.Abs(this.coordinateY - p.coordinateY) < EQUALITY_TOLERANCE);
        }
    }
}
