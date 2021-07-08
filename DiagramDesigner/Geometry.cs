using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesigner
{
    abstract class Geometry
    {
        public double TranslationX = 0;
        public double TranslationY = 0;
        public double Rotation = 0;
        abstract public void Draw();
        abstract public bool IsClosed();
    }
}
