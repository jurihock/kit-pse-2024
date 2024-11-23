using System;
using OxyPlot;

namespace Spaghetti.Plot.Manipulators;

public sealed class SyncPanEventArgs(ScreenVector delta) : EventArgs
{
  public ScreenVector Delta { get; private init; } = delta;
}

public sealed class SyncPanManipulator : MouseManipulator
{
  private ScreenPoint PreviousPosition;

  public event EventHandler<SyncPanEventArgs>? PanChanged;

  public SyncPanManipulator(IPlotView view) : base(view)
  {
  }

  public override void Started(OxyMouseEventArgs args)
  {
    base.Started(args);

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

    var delta = y - x;

    PreviousPosition = y;
    args.Handled = true;

    PanChanged?.Invoke(this, new SyncPanEventArgs(delta));
  }
}
