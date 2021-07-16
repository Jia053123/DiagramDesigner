using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesigner
{
    readonly struct MouseEventParameters
    {
        public readonly int LocationX;
        public readonly int LocationY;
        public readonly bool IsButtonPressed;
        public MouseEventParameters(int locationX, int locationY, bool isButtonPressed)
        {
            this.LocationX = locationX;
            this.LocationY = locationY;
            this.IsButtonPressed = isButtonPressed;
        }
    }
}
