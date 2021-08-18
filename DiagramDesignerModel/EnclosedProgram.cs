using DiagramDesignerEngine;
using System;
using System.Collections.Generic;
using System.Text;

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
            double perimeterArea = this.ShoelaceArea(this.Perimeter);
            double innerPerimetersArea = 0;
            foreach (List<Point> ip in this.InnerPerimeters)
			{
                innerPerimetersArea += this.ShoelaceArea(ip);
            }

            return perimeterArea - innerPerimetersArea;
        }

        public double CalculatePerimeterLength()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Source: https://rosettacode.org/wiki/Shoelace_formula_for_polygonal_area#C.23
        /// </summary>
        private double ShoelaceArea(List<Point> v)
        {
            int n = v.Count;
            double a = 0.0;
            for (int i = 0; i < n - 1; i++)
            {
                a += v[i].coordinateX * v[i + 1].coordinateY - v[i + 1].coordinateX * v[i].coordinateY;
            }
            return Math.Abs(a + v[n - 1].coordinateX * v[0].coordinateY - v[0].coordinateX * v[n - 1].coordinateY) / 2.0;
        }
    }
}

