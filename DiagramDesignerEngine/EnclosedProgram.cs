using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class EnclosedProgram
    {
        internal UndividableDiagramFragment Geometry { get; set; }
        public List<Point> Perimeter { get { return this.Geometry.GetPerimeterInPoints(); } }
        public List<List<Point>> InnerPerimeters { get { return this.Geometry.GetInnerPerimetersInPoints(); } }

        internal EnclosedProgram(UndividableDiagramFragment geometry)
		{
            this.Geometry = geometry;
		}

        public double CalculateEnclosedArea()
        {
            throw new NotImplementedException();
        }

        public double CalculatePerimeterLength()
        {
            throw new NotImplementedException();
        }
    }
}
