namespace DiagramDesignerModel
{
	public class WallEntity : Entity
    {
        public double WallThickness { get; internal set; }
        public override EntityPolylineGeometry Geometry { get; protected set; } = new EntityPolylineGeometry();

        /// <summary>
        /// Create a wall
        /// </summary>
        /// <param name="thickness"> The thickness of the wall in real world unit </param>
        public WallEntity(double thickness)
		{
            this.WallThickness = thickness;
		}
    }
}
