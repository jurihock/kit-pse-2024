using OxyPlot;
using OxyPlot.Axes;
using Spaghetti.Core.Bitmap;
using Spaghetti.Plot.Series;

namespace Spaghetti.Plot.Models;

public sealed class BitmapPlotModel : PlotModel
{
  private static readonly OxyColor DefaultBackgroundColor = OxyColors.Black;
  private static readonly OxyColor DefaultForegroundColor = OxyColors.White;

  public LinearAxis AxisX { get; init; }
  public LinearAxis AxisY { get; init; }

  public BitmapSeries BitmapSeries { get; init; }

  public IBitmap<BGR>? Bitmap
  {
    get => BitmapSeries.Bitmap;
    set => BitmapSeries.Bitmap = value;
  }

  public BitmapPlotModel()
  {
    Background = DefaultBackgroundColor;
    TextColor = DefaultForegroundColor;
    PlotAreaBorderColor = DefaultForegroundColor;
    PlotType = PlotType.Cartesian;

    AxisX = new LinearAxis()
    {
      Position = AxisPosition.Bottom,
      TicklineColor = DefaultForegroundColor,
      IsPanEnabled = true,
      IsZoomEnabled = true,
      IsAxisVisible = true
    };
    Axes.Add(AxisX);

    AxisY = new LinearAxis()
    {
      Position = AxisPosition.Left,
      TicklineColor = DefaultForegroundColor,
      IsPanEnabled = true,
      IsZoomEnabled = true,
      IsAxisVisible = true
    };
    Axes.Add(AxisY);

    BitmapSeries = new BitmapSeries();
    Series.Add(BitmapSeries);
  }
}
