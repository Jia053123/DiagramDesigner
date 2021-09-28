namespace DiagramDesignerModel
{
	public class WallEntity : Entity
    {
        public double WallThickness { get; internal set; }
        public override PolylineEntityGeometry Geometry { get; protected set; } = new PolylineEntityGeometry();

        public WallEntity(double thickness)
		{
            this.WallThickness = thickness;
		}
    }
}
