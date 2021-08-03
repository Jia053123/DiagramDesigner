using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class EnclosedProgram
    {
        public bool IsAlsoCirculation;
        public PolylineGeometry Geometry { get; internal set; }

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
