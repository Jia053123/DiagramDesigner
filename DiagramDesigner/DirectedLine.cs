using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	class DirectedLine
    {
        public WinPoint StartPoint { get; set; }
        public WinPoint EndPoint { get; set; }

        public DirectedLine(WinPoint startP, WinPoint endP)
		{
            this.StartPoint = startP;
            this.EndPoint = endP;
		}
    }
}
