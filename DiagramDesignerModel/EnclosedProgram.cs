using BasicGeometries;
using DiagramDesignerGeometryParser;
using System;
using System.Collections.Generic;

namespace DiagramDesignerModel
{
	public class EnclosedProgram
    {
        public String Name { get; private set; }
        public double Area { get { return this.CalculateEnclosedArea(); } }
        internal UndividableDiagramFragment Geometry { get; set; }
        public List<Point> Perimeter { get { return this.Geometry.GetPerimeterInPoints(); } }
        public List<List<Point>> InnerPerimeters { get { return this.Geometry.GetInnerPerimetersInPoints(); } }

        internal EnclosedProgram(String name, UndividableDiagramFragment geometry)
		{
            this.Name = name;
            this.Geometry = geometry;
		}

        public double CalculateEnclosedArea()
        {
            return this.Geometry.CalculateFragmentArea();
		}

		public double CalculatePerimeterLength()
        {
            throw new NotImplementedException();
        }
    }
}

