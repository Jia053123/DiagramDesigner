using BasicGeometries;
using DiagramDesignerEngine;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerModel
{
    /// <summary>
    /// An thing in the diagram that is not an annotation for such diagram. 
    /// </summary>
    public abstract class Entity {
        public bool Isfixed;
        internal Shape UnderlyingShape { get { return this.Geometry.UnderlyingShape; } }
		public abstract PolylineGeometry Geometry { get; protected set; }

		internal void AddPointToGeometry(Point p)
		{
            this.Geometry.PathsDefinedByPoints.Add(p);
		}
    }
}
