using ReactiveUI;
using Spaghetti.Core.Bitmap;
using Spaghetti.Core.Image;
using Spaghetti.Core.Image.Envi;
using Spaghetti.Core.Image.Skia;
using Spaghetti.Plot.Controllers;
using Spaghetti.Plot.Models;
using System.IO;
using System.Linq;

namespace Spaghetti.Face.ViewModels;

public sealed class OxyPlotViewModel : ReactiveObject
{
  public BitmapPlotModel PlotModel1 { get; } = new();
  public BitmapPlotModel PlotModel2 { get; } = new();
  public BitmapPlotModel PlotModel3 { get; } = new();
  public BitmapPlotModel PlotModel4 { get; } = new();

  public SyncPlotController PlotController { get; set; }

  public OxyPlotViewModel()
  {
    PlotController = new SyncPlotController(
      PlotModel1, PlotModel2, PlotModel3, PlotModel4);

    var files = new[] { "test.png", "test-bip.hdr", "test-bil.hdr", "test-bsq.hdr" };
    var indices = Enumerable.Range(0, files.Length);

    foreach (var (i, file) in indices.Zip(files))
    {
      var model = (BitmapPlotModel)PlotController.SyncPlotModels[i];

      IImage<double>? image = null;

      if (File.Exists(file))
      {
        if (file.EndsWith(".hdr"))
        {
          image = new EnviImage<double>(file);
        }
        else
        {
          image = new SkiaImage<double>(file);
        }
      }

      if (image != null)
      {
        var (w, h) = (image.Shape.Width, image.Shape.Height);

        model.Bitmap = new BgrBitmap(w, h);

        for (var y = 0; y < h; y++)
        {
          for (var x = 0; x < w; x++)
          {
            model.Bitmap[x, y][0] = (byte)image.Flip()[x, y, 0];
            model.Bitmap[x, y][1] = (byte)image.Flip()[x, y, 1];
            model.Bitmap[x, y][2] = (byte)image.Flip()[x, y, 2];
          }
        }
      }
      else
      {
        var (w, h) = (1 * 1000, 1 * 1000);

        var b = new byte[] { 100, 000, 000, 100 };
        var g = new byte[] { 000, 100, 000, 100 };
        var r = new byte[] { 000, 000, 100, 100 };

        model.Bitmap = new BgrBitmap(w, h).Fill(b: b[i], g: g[i], r: r[i]);
      }
    }
  }
}
