namespace DiagramDesignerModel
{
	public class WallEntity : Entity
    {
        public double WallThickness { get; internal set; }
        public override PolylineGeometry Geometry { get; protected set; } = new PolylineGeometry();

        public WallEntity(double thickness)
		{
            this.WallThickness = thickness;
		}
    }
}
