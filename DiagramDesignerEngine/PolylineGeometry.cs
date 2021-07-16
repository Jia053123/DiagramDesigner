using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class PolylineGeometry : Geometry
    {
        public Point[] PathsDefinedByPoints { get; set; }

        public override PolylineGeometry GetPolylineApproximation()
        {
            return this;
        }

        public override bool IsClosed()
        {
            // TODO: stub
            return false;
        }
    }
}
