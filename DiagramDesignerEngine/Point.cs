using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class Point
    {
        public double coordinateX { get; set; }
        public double coordinateY { get; set; }

        public Point(double x, double y)
        {
            this.coordinateX = x;
            this.coordinateY = y;
        }
    }
}
