using System;
using System.Collections.Generic;
using System.Text;
using Svg;
using Svg.Pathing;
using System.Drawing;
using MyPoint = BasicGeometries.Point;

namespace ShapeGrammarEngine
{
    class MachineLearningUtilities
    {
		static internal SvgDocument PolylinesGeometryToSvgOnCanvas(PolylinesGeometry polyGeo, int canvasWidth, int canvasHeight, int outWidth, int outHeight, int strokeWidth)
        {
			SvgDocument renderedDoc = new SvgDocument();
			renderedDoc.Width = outWidth;
			renderedDoc.Height = outHeight;
			renderedDoc.ViewBox = new SvgViewBox(0, 0, outWidth, outHeight);

			var sp = new SvgPath();
			var spsl = new SvgPathSegmentList();

			double xFactor = outWidth / (double)canvasWidth;
			double yFactor = outHeight / (double)canvasHeight;

			foreach (List<MyPoint> polyline in polyGeo.PolylinesCopy)
			{
				for (int i = 0; i < polyline.Count; i++)
				{
					MyPoint p = polyline[i];
					int mappedX = Convert.ToInt32(p.coordinateX * xFactor);
					int mappedY = Convert.ToInt32(p.coordinateY * yFactor);
					if (i == 0)
					{
						spsl.Add(new SvgMoveToSegment(false, new PointF(mappedX, mappedY)));
					}
					else
					{
						spsl.Add(new SvgLineSegment(false, new PointF(mappedX, mappedY)));
					}
				}
			}

			sp.PathData = spsl;
			sp.StrokeWidth = strokeWidth;
			sp.Stroke = new SvgColourServer(Color.Black);
			sp.Fill = null;

			renderedDoc.Children.Add(sp);
			return renderedDoc;
		}

		/// <summary>
		/// Render a PolylinesGeometry object as svg. The polylines will be squeezed within the specified dimension
		/// </summary>
		/// <param name="polyGeo"> The PolylineGeometry to render </param>
		/// <param name="width"> Width of the resulting svg document </param>
		/// <param name="height"> Height of the resulting svg document </param>
		/// <param name="strokeWidth"> Width of the stroke used the render the geometry; Recommend even number </param>
		/// <returns> A SVG document with the specified dimensions </returns>
		static internal SvgDocument PolylinesGeometryToSvg(PolylinesGeometry polyGeo, int width, int height, int strokeWidth)
		{
			SvgDocument shapeDoc = new SvgDocument();
			shapeDoc.Width = width;
			shapeDoc.Height = height;
			shapeDoc.ViewBox = new SvgViewBox(0, 0, width, height);

			var sp = new SvgPath();
			var spsl = new SvgPathSegmentList();

			var boundingBox = polyGeo.GetBoundingBox();
			double xDelta = -1 * boundingBox.xMin;
			double yDelta = -1 * boundingBox.yMax;
			double xFactor = width / (boundingBox.xMax - boundingBox.xMin);
			double yFactor = height / (boundingBox.yMax - boundingBox.yMin);
			foreach (List<MyPoint> polyline in polyGeo.PolylinesCopy)
			{
				for (int i = 0; i < polyline.Count; i++)
				{
					MyPoint p = polyline[i];
					int mappedX = Convert.ToInt32((p.coordinateX + xDelta) * xFactor);
					int mappedY = -1 * Convert.ToInt32((p.coordinateY + yDelta) * yFactor);
					if (i == 0)
					{
						spsl.Add(new SvgMoveToSegment(false, new PointF(mappedX, mappedY)));
					}
					else
					{
						spsl.Add(new SvgLineSegment(false, new PointF(mappedX, mappedY)));
					}
				}
			}

			sp.PathData = spsl;
			sp.StrokeWidth = strokeWidth;
			sp.Stroke = new SvgColourServer(Color.Black);
			sp.Fill = null;

			shapeDoc.Children.Add(sp);

			return shapeDoc;
		}

		/// <summary>
		/// Render a PolylinesGeometry object as svg. The polylines will be squeezed within the specified dimension with padding to accommodate the stroke width
		/// </summary>
		/// <param name="polyGeo"> The PolylineGeometry to render </param>
		/// <param name="width"> Width of the resulting svg document </param>
		/// <param name="height"> Height of the resulting svg document </param>
		/// <param name="strokeWidth"> Width of the stroke used the render the geometry; Recommend even number </param>
		/// <returns> A SVG document with the specified dimensions </returns>
		static internal SvgDocument PolylinesGeometryToSvgPadded(PolylinesGeometry polyGeo, int width, int height, int strokeWidth)
		{
			SvgDocument shapeDoc = new SvgDocument();
			shapeDoc.Width = width;
			shapeDoc.Height = height;
			shapeDoc.ViewBox = new SvgViewBox(0, 0, width, height);

			var sp = new SvgPath();
			var spsl = new SvgPathSegmentList();

			var paddedWidth = width - strokeWidth;
			var paddedHeight = height - strokeWidth;
			var boundingBox = polyGeo.GetBoundingBox();
			double xDelta = -1 * boundingBox.xMin;
			double yDelta = -1 * boundingBox.yMax;
			double xFactor = paddedWidth / (boundingBox.xMax - boundingBox.xMin);
			double yFactor = paddedHeight / (boundingBox.yMax - boundingBox.yMin);
			foreach (List<MyPoint> polyline in polyGeo.PolylinesCopy)
			{
				for (int i = 0; i < polyline.Count; i++)
				{
					MyPoint p = polyline[i];
					int mappedX = Convert.ToInt32(strokeWidth / 2 + (p.coordinateX + xDelta) * xFactor);
					int mappedY = Convert.ToInt32(strokeWidth / 2 - (p.coordinateY + yDelta) * yFactor);
					if (i == 0)
					{
						spsl.Add(new SvgMoveToSegment(false, new PointF(mappedX, mappedY)));
					}
					else
					{
						spsl.Add(new SvgLineSegment(false, new PointF(mappedX, mappedY)));
					}
				}
			}

			sp.PathData = spsl;
			sp.StrokeWidth = strokeWidth;
			sp.Stroke = new SvgColourServer(Color.Black);
			sp.Fill = null;

			shapeDoc.Children.Add(sp);

			return shapeDoc;
		}
	}
}
