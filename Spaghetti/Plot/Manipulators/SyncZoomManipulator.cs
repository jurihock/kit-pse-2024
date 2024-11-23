using System;
using OxyPlot;

namespace Spaghetti.Plot.Manipulators;

public sealed class SyncZoomEventArgs(double factor, DataPoint center) : EventArgs
{
  public double Factor { get; private init; } = factor;
  public DataPoint Center { get; private init; } = center;
}

public sealed class SyncZoomManipulator : MouseManipulator
{
  private ScreenPoint InitialPosition;
  private ScreenPoint PreviousPosition;

  public double ZoomThreshold { get; init; } = double.Epsilon;
  public double ZoomDirection { get; init; } = -1;
  public double ZoomGamma { get; init; } = 1.5;
  public double ZoomSensitivity { get; init; } = 1e-2;

  public event EventHandler<SyncZoomEventArgs>? ZoomChanged;

  public SyncZoomManipulator(IPlotView view) : base(view)
  {
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);

    InitialPosition = args.Position;
    PreviousPosition = args.Position;

    View.SetCursorType(CursorType.Pan);
    args.Handled = true;
  }

  public override void Completed(OxyMouseEventArgs args)
  {
    base.Completed(args);

    View.SetCursorType(CursorType.Default);
    args.Handled = true;
  }

  public override void Delta(OxyMouseEventArgs args)
  {
    base.Delta(args);

    var x = PreviousPosition;
    var y = args.Position;

    var vector = y - x;
    var length = double.Hypot(vector.X, vector.Y);

    var zoom = (length > ZoomThreshold) && (Math.Abs(vector.Y) > Math.Abs(vector.X));

    if (!zoom)
    {
      return;
    }

    var factor = Math.Sign(vector.Y) * Math.Sign(ZoomDirection) *
                 Math.Pow(length, ZoomGamma) * ZoomSensitivity;

    if (factor > 0)
    {
      factor = 1.0 + factor;
    }
    else
    {
      factor = 1.0 / (1.0 - factor);
    }

    var center = InverseTransform(
      InitialPosition.X,
      InitialPosition.Y);

    PreviousPosition = y;
    args.Handled = true;

    ZoomChanged?.Invoke(this, new SyncZoomEventArgs(factor, center));
  }
}
