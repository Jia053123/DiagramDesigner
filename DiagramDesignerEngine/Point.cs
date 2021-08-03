using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class Point
    {
        // setting these to internal in order to make them readonly to the ViewModel
        public double coordinateX { get; internal set; }
        public double coordinateY { get; internal set; }

        public Point(double x, double y)
        {
            this.coordinateX = x;
            this.coordinateY = y;
        }
    }
}
