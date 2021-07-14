using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    class PolygonGeometry : Geometry
    {
        public Point[] PathsDefinedByPoints { get; set; }
        public override void Draw()
        {
            // TODO: stub
        }

        public override bool IsClosed()
        {
            // TODO: stub
            return false;
        }
    }
}
