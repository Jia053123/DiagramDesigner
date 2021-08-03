using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class PolylineGeometry
    {
        public List<Point> PathsDefinedByPoints { get; internal set; } = new List<Point>();

        public bool IsClosed()
        {
            // TODO: stub
            return false;
        }
    }
}
