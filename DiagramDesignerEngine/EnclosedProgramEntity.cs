using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    class EnclosedProgramEntity : Entity
    {
        public override Geometry Geometry => throw new NotImplementedException();

        public double CalculateEnclosedArea()
        {
            throw new NotImplementedException();
        }

        public double CalculatePerimeterLength()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
