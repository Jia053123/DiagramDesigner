using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesigner
{
    class PolygonGeometry : DDGeometry
    {
        public DDPoint[] PathsDefinedByPoints { get; set; }
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
