using System;
using System.Collections.Generic;
using System.Diagnostics;
using Svg;
using Svg.Pathing;
using System.Drawing;
using MyPoint = BasicGeometries.Point;
using System.Drawing.Drawing2D;

namespace ShapeGrammarEngine
{
    class MachineLearningUtilities
    {
		static internal (List<PolylinesGeometry> variationsForGeometryBefore, List<PolylinesGeometry> variationsForGeometryAfter) GenerateVariations(
			PolylinesGeometry polylinesGeometryBefore, 
			PolylinesGeometry polylinesGeometryAfter, 
			int canvasWidth, 
			int canvasHeight, 
			int numOfVariations)
        {
			const double scaleFactorMin = 0.8;
			const double scaleFactorMax = 0.12;
			const int noiseAbs = 5;

			var variationsGeoBefore = new List<PolylinesGeometry>();
			var variationsGeoAfter = new List<PolylinesGeometry>();

			var bbAfter = polylinesGeometryAfter.GetBoundingBox();

			var rand = new Random();
			for (int i = 0; i < numOfVariations; i++)
            {
				int translationX = rand.Next((int)(0 - bbAfter.xMin), (int)(canvasWidth - bbAfter.xMax));
				int translationY = rand.Next((int)(0 - bbAfter.yMin), (int)(canvasHeight - bbAfter.yMax));
				float scaleFactor = (float)(rand.NextDouble() * (scaleFactorMax - scaleFactorMin) + scaleFactorMin);
				float rotation = (float)(rand.NextDouble() * 2 * Math.PI);

				var transformation = new Matrix(); // identity matrix
				transformation.Scale(scaleFactor, scaleFactor);
				transformation.RotateAt(rotation, new PointF(0,0));
				transformation.Translate(translationX, translationY);

				void ApplyTransformations(ref List<List<MyPoint>> polylines)
                {
					foreach (List<MyPoint> polyline in polylines)
					{
						for (int j = 0; i < polyline.Count; j++)
						{
							MyPoint mp = polyline[j];
							PointF p = new PointF((float)mp.coordinateX, (float)mp.coordinateY);
							transformation.TransformPoints(new PointF[] { p });

							// add noise to each point to take "sketchiness" into account
							var noiseTranslation = new Matrix();
							noiseTranslation.Translate(rand.Next(-1 * noiseAbs, noiseAbs), rand.Next(-1 * noiseAbs, noiseAbs));
							noiseTranslation.TransformPoints(new PointF[] { p });

							polyline[j] = new MyPoint(p.X, p.Y);
						}
					}
				}
				var newVariationPolylinesBefore = polylinesGeometryBefore.PolylinesCopy;
				ApplyTransformations(ref newVariationPolylinesBefore);
				PolylinesGeometry newVariationBefore = new PolylinesGeometry(newVariationPolylinesBefore);
				variationsGeoBefore.Add(newVariationBefore);

				var newVariationPolylinesAfter = polylinesGeometryAfter.PolylinesCopy;
				ApplyTransformations(ref newVariationPolylinesAfter);
                PolylinesGeometry newVariationAfter = new PolylinesGeometry(newVariationPolylinesAfter);
				variationsGeoAfter.Add(newVariationAfter);
            }
			return (variationsGeoBefore, variationsGeoAfter);
        }

		

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
