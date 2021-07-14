using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesigner
{
    struct DDPoint
    {
        public double coordinateX { get; set; }
        public double coordinateY { get; set; }

        public DDPoint(double x, double y)
        {
            this.coordinateX = x;
            this.coordinateY = y;
        }
    }
}
