using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesigner
{
    abstract class Entity
    {
        public Geometry geometry { get; protected set; }

        public void Draw()
        {
            // TODO: stub
            this.geometry.Draw();
        }
    }
}
