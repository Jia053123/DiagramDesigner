using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public abstract class Geometry
    {
        abstract public bool IsClosed();

        abstract public PolylineGeometry GetPolylineApproximation();
    }
}
