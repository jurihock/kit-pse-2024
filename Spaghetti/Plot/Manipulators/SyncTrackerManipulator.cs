using System;
using System.Linq;
using OxyPlot;
using Spaghetti.Plot.Series;

namespace Spaghetti.Plot.Manipulators;

public sealed class SyncTrackerEventArgs(ScreenPoint? point) : EventArgs
{
  public ScreenPoint? ScreenPoint { get; private init; } = point;
}

public sealed class SyncTrackerManipulator : MouseManipulator
{
  private readonly OxyPlot.Series.Series? TrackableSeries;
  private readonly bool IsTrackerEnabled;

  public event EventHandler<SyncTrackerEventArgs>? TrackerChanged;

  public SyncTrackerManipulator(IPlotView view) : base(view)
  {
    TrackableSeries = PlotView.ActualModel.Series.FirstOrDefault(series => series is ISyncSeries);
    IsTrackerEnabled = TrackableSeries is not null;
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);

    if (!IsTrackerEnabled)
    {
      return;
    }

    Delta(args);

    View.SetCursorType(CursorType.Pan);
    args.Handled = true;
  }

  public override void Completed(OxyMouseEventArgs args)
  {
    base.Completed(args);

    if (!IsTrackerEnabled)
    {
      return;
    }

    View.SetCursorType(CursorType.Default);
    args.Handled = true;

    TrackerChanged?.Invoke(this, new SyncTrackerEventArgs(null));
  }

  public override void Delta(OxyMouseEventArgs args)
  {
    base.Delta(args);

    if (!IsTrackerEnabled)
    {
      return;
    }

    if (!PlotView.ActualModel.PlotArea.Contains(args.Position))
    {
      return;
    }

    args.Handled = true;

    TrackerChanged?.Invoke(this, new SyncTrackerEventArgs(args.Position));
  }
}
