using System;
using OxyPlot;
using OxyPlot.Series;
using Spaghetti.Core.Bitmap;
using Spaghetti.Plot.Extensions;
using Spaghetti.Plot.Transforms;

namespace Spaghetti.Plot.Series;

public sealed class BitmapSeries : XYAxisSeries, ISyncSeries
{
  private CartesianCoordinateTransformation CoordinateTransformation { get; set; }
  private (IBitmap<BGR> Bitmap, OxyImage Image)? Data {  get; set; }

  public IBitmap<BGR>? Bitmap
  {
    get => Data?.Bitmap;
    set
    {
      if (value is null)
      {
        Data = null;
        return;
      }

      // TODO: optionally transform image coordinates
      //CoordinateTransformation = new CartesianCoordinateTransformation(
      //  new LinearCoordinateTransformation(...),
      //  new LinearCoordinateTransformation(...));

      Data = (value, value.ToOxyImage());
    }
  }

  public BitmapSeries()
  {
    CoordinateTransformation = new CartesianCoordinateTransformation(
      new LinearCoordinateTransformation(),
      new LinearCoordinateTransformation());
  }

  public override void Render(IRenderContext rc) // TODO: OxyPlot.Avalonia.CanvasRenderContext
  {
    var data = Data;

    if (data is null)
    {
      return;
    }

    double left = 0;
    double right = data.Value.Bitmap.Width;
    double bottom = data.Value.Bitmap.Height;
    double top = 0;

    var dataPoint00 = new DataPoint(left, bottom);
    var dataPoint11 = new DataPoint(right, top);

    var worldPoint00 = CoordinateTransformation.Forward(dataPoint00);
    var worldPoint11 = CoordinateTransformation.Forward(dataPoint11);

    var screenPoint00 = Transform(worldPoint00);
    var screenPoint11 = Transform(worldPoint11);

    var screenRect = new OxyRect(screenPoint00, screenPoint11);

    left = screenRect.Left;
    top = screenRect.Top;
    right = screenRect.Width;
    bottom = screenRect.Height;

    rc.DrawImage(
      data.Value.Image,
      left, top, right, bottom,
      opacity: 1,
      interpolate: false);
  }

  protected override void UpdateMaxMin()
  {
    base.UpdateMaxMin();

    var bitmap = Bitmap;

    if (bitmap is null)
    {
      MinX = 0;
      MaxX = 0;
      MinY = 0;
      MaxY = 0;
      return;
    }

    double left = 0;
    double right = bitmap.Width;
    double bottom = bitmap.Height;
    double top = 0;

    var dataPoint00 = new DataPoint(left, bottom);
    var dataPoint11 = new DataPoint(right, top);

    var worldPoint00 = CoordinateTransformation.Forward(dataPoint00);
    var worldPoint11 = CoordinateTransformation.Forward(dataPoint11);

    MinX = Math.Min(worldPoint00.X, worldPoint11.X);
    MaxX = Math.Max(worldPoint00.X, worldPoint11.X);

    MinY = Math.Min(worldPoint00.Y, worldPoint11.Y);
    MaxY = Math.Max(worldPoint00.Y, worldPoint11.Y);
  }

  public override TrackerHitResult GetNearestPoint(ScreenPoint screenPoint, bool interpolate)
  {
    var bitmap = Bitmap;

    if (bitmap is null)
    {
      return base.GetNearestPoint(screenPoint, interpolate);
    }

    var worldPoint = InverseTransform(screenPoint);
    var dataPoint = CoordinateTransformation.Backward(worldPoint);

    double left = 0;
    double right = bitmap.Width;
    double bottom = bitmap.Height;
    double top = 0;

    int nearestX = (int)Math.Floor(dataPoint.X);
    int nearestY = (int)Math.Floor(dataPoint.Y);

    if (nearestX < left || nearestX >= right)
    {
      return base.GetNearestPoint(screenPoint, interpolate);
    }

    if (nearestY < top || nearestY >= bottom)
    {
      return base.GetNearestPoint(screenPoint, interpolate);
    }

    var nearestDataPoint = new DataPoint(nearestX, nearestY);
    var nearestWorldPoint = CoordinateTransformation.Forward(nearestDataPoint);

    var trackerDataPoint = new DataPoint(nearestX + 0.5, nearestY + 0.5);
    var trackerWorldPoint = CoordinateTransformation.Forward(trackerDataPoint);
    var trackerScreenPoint = Transform(trackerWorldPoint);

    var color = bitmap[nearestX, nearestY];
    var text = string.Join('\n',
      $"X: {nearestX}",
      $"Y: {nearestY}",
      $"B: {color.B}",
      $"G: {color.G}",
      $"R: {color.R}");

    return new TrackerHitResult
    {
      Series = this,
      DataPoint = nearestWorldPoint,
      Position = trackerScreenPoint,
      Text = text
    };
  }
}
