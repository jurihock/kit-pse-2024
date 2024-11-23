using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using Spaghetti.Plot.Manipulators;
using Spaghetti.Plot.Series;

namespace Spaghetti.Plot.Controllers;

public sealed class SyncPlotController : PlotController
{
  public IReadOnlyList<PlotModel> SyncPlotModels { get; private init; }

  public SyncPlotController(params PlotModel[] syncPlotModels)
  {
    SyncPlotModels = syncPlotModels.ToList();

    UnbindAll();
    BindSync();
  }

  public void ResetAllAxes()
  {
    foreach (var model in SyncPlotModels)
    {
      model.ResetAllAxes();
    }

    OnResetAllAxes();
  }

  private void BindSync()
  {
    this.BindMouseDown(OxyMouseButton.Right, OxyModifierKeys.None, 2,
      new DelegatePlotCommand<OxyMouseEventArgs>(
        (IPlotView view, IController controller, OxyMouseEventArgs args) =>
        {
          ResetAllAxes();
          args.Handled = true;
        }));

    this.BindMouseDown(OxyMouseButton.Middle, OxyModifierKeys.None, 2,
      new DelegatePlotCommand<OxyMouseEventArgs>(
        (IPlotView view, IController controller, OxyMouseEventArgs args) =>
        {
          ResetAllAxes();
          args.Handled = true;
        }));

    this.BindMouseDown(OxyMouseButton.Right,
      new DelegatePlotCommand<OxyMouseDownEventArgs>(
        (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
        {
          var manipulator = new SyncPanManipulator(view);
          manipulator.PanChanged += OnMasterPanChanged;
          controller.AddMouseManipulator(view, manipulator, args);
        }));

    this.BindMouseDown(OxyMouseButton.Middle,
      new DelegatePlotCommand<OxyMouseDownEventArgs>(
        (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
        {
          var manipulator = new SyncZoomManipulator(view);
          manipulator.ZoomChanged += OnMasterZoomChanged;
          controller.AddMouseManipulator(view, manipulator, args);
        }));

    this.BindMouseDown(OxyMouseButton.Left,
      new DelegatePlotCommand<OxyMouseDownEventArgs>(
        (IPlotView view, IController controller, OxyMouseDownEventArgs args) =>
        {
          var manipulator = new SyncTrackerManipulator(view);
          manipulator.TrackerChanged += OnMasterTrackerChanged;
          controller.AddMouseManipulator(view, manipulator, args);
        }));
  }

  private void OnResetAllAxes()
  {
    var filter = (OxyPlot.Series.Series series) =>
      (series is ISyncSeries) &&
      (series is XYAxisSeries);

    var cast = (OxyPlot.Series.Series? series) =>
      (XYAxisSeries)(series ?? throw new InvalidOperationException());

    var models = SyncPlotModels
      .Select(model => (model, series: model.Series.FirstOrDefault(filter)))
      .Where(model => model.series is not null)
      .Select(model => (model.model, series: cast(model.series)))
      .ToList();

    var minX = models.Min(_ => _.series.XAxis.ActualMinimum);
    var maxX = models.Max(_ => _.series.XAxis.ActualMaximum);

    var minY = models.Min(_ => _.series.YAxis.ActualMinimum);
    var maxY = models.Max(_ => _.series.YAxis.ActualMaximum);

    foreach (var (model, series) in models)
    {
      var invalidate = false;

      if (series.XAxis.IsZoomEnabled)
      {
        series.XAxis.Zoom(
          minX,
          maxX);

        invalidate = true;
      }

      if (series.YAxis.IsZoomEnabled)
      {
        series.YAxis.Zoom(
          minY,
          maxY);

        invalidate = true;
      }

      if (invalidate)
      {
        Task.Factory.StartNew(
          _ => (_ as PlotModel)?.InvalidatePlot(false),
          model);
      }
    }
  }

  private void OnMasterZoomChanged(object? sender, SyncZoomEventArgs args)
  {
    var masterPlotManipulator = sender as PlotManipulator<OxyMouseEventArgs>;
    var masterPlotModel = masterPlotManipulator?.PlotView?.ActualModel;

    if (masterPlotModel is null)
    {
      return;
    }

    var filter = (OxyPlot.Series.Series series) =>
      (series is ISyncSeries) &&
      (series is XYAxisSeries);

    var cast = (OxyPlot.Series.Series? series) =>
      (XYAxisSeries)(series ?? throw new InvalidOperationException());

    var models = SyncPlotModels
      .Select(model => (model, series: model.Series.FirstOrDefault(filter)))
      .Where(model => model.series is not null)
      .Select(model => (model.model, series: cast(model.series)));

    foreach (var (model, series) in models)
    {
      var invalidate = false;

      if (series.XAxis.IsZoomEnabled)
      {
        series.XAxis.ZoomAt(
          args.Factor,
          args.Center.X);

        invalidate = true;
      }

      if (series.YAxis.IsZoomEnabled)
      {
        series.YAxis.ZoomAt(
          args.Factor,
          args.Center.Y);

        invalidate = true;
      }

      if (invalidate)
      {
        Task.Factory.StartNew(
          _ => (_ as PlotModel)?.InvalidatePlot(false),
          model);
      }
    }
  }

  private void OnMasterPanChanged(object? sender, SyncPanEventArgs args)
  {
    var masterPlotManipulator = sender as PlotManipulator<OxyMouseEventArgs>;
    var masterPlotModel = masterPlotManipulator?.PlotView?.ActualModel;

    if (masterPlotModel is null)
    {
      return;
    }

    var filter = (OxyPlot.Series.Series series) =>
      (series is ISyncSeries) &&
      (series is XYAxisSeries);

    var cast = (OxyPlot.Series.Series? series) =>
      (XYAxisSeries)(series ?? throw new InvalidOperationException());

    var models = SyncPlotModels
      .Select(model => (model, series: model.Series.FirstOrDefault(filter)))
      .Where(model => model.series is not null)
      .Select(model => (model.model, series: cast(model.series)));

    foreach (var (model, series) in models)
    {
      var invalidate = false;

      if (series.XAxis.IsPanEnabled)
      {
        series.XAxis.Pan(
          args.Delta.X);

        invalidate = true;
      }

      if (series.YAxis.IsPanEnabled)
      {
        series.YAxis.Pan(
          args.Delta.Y);

        invalidate = true;
      }

      if (invalidate)
      {
        Task.Factory.StartNew(
          _ => (_ as PlotModel)?.InvalidatePlot(false),
          model);
      }
    }
  }

  private void OnMasterTrackerChanged(object? sender, SyncTrackerEventArgs args)
  {
    var masterPlotManipulator = sender as PlotManipulator<OxyMouseEventArgs>;
    var masterPlotModel = masterPlotManipulator?.PlotView?.ActualModel;

    if (masterPlotModel is null)
    {
      return;
    }

    var filter = (OxyPlot.Series.Series series) =>
      (series is ISyncSeries) &&
      (series is XYAxisSeries);

    var cast = (OxyPlot.Series.Series? series) =>
      (XYAxisSeries)(series ?? throw new InvalidOperationException());

    var models = SyncPlotModels
      .Select(model => (model, series: model.Series.FirstOrDefault(filter)))
      .Where(model => model.series is not null)
      .Select(model => (model.model, series: cast(model.series)));

    foreach (var (model, series) in models)
    {
      var screenPoint = args.ScreenPoint;

      if (screenPoint is null)
      {
        model.PlotView?.HideTracker();
        continue;
      }

      var hit = series.GetNearestPoint(screenPoint.Value, interpolate: false);

      if (hit is null)
      {
        continue;
      }

      hit.PlotModel = model;
      model.PlotView?.ShowTracker(hit);
    }
  }
}
