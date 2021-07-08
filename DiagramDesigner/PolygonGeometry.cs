using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesigner
{
    class PolygonGeometry : Geometry
    {
        public Point[] PathsDefinedByPoints { get; set; }
        public override void Draw()
        {
            // TODO: stub
        }

        public override bool isClosed()
        {
            // TODO: stub
            return false;
        }
    }
}
