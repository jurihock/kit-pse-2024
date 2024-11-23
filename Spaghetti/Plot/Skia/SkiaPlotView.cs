using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using Spaghetti.Core.Bitmap;
using System.Runtime.InteropServices;

namespace Spaghetti.Plot.Skia;

// https://stackoverflow.com/questions/61627374/is-it-possible-to-create-a-skia-canvas-element-in-an-avalonia-application
// https://github.com/AvaloniaUI/Avalonia/blob/master/samples/RenderDemo/Pages/CustomSkiaPage.cs
// https://github.com/oxyplot/oxyplot/blob/develop/Source/OxyPlot.SkiaSharp.Wpf/PlotView.cs
// https://github.com/oxyplot/oxyplot-avalonia/tree/master/Source/OxyPlot.Avalonia
// https://github.com/oxyplot/oxyplot-avalonia/blob/master/Source/OxyPlot.Avalonia/PlotBase.cs

public sealed class SkiaPlotView : Control // TODO: check also SkiaSharp.Views.WPF
{
  private SKBitmap Bitmap;
  private SKImage Image;

  byte counter = 0;

  public SkiaPlotView()
  {
    Bitmap = new SKBitmap(1 * 1000, 1 * 1000, SKColorType.Bgra8888, SKAlphaType.Premul);

    var bytes = Bitmap.GetPixelSpan();
    var pixels = MemoryMarshal.CreateSpan(
      ref MemoryMarshal.AsRef<BGRA>(bytes),
      bytes.Length / Marshal.SizeOf<BGRA>());
    pixels.Fill(new BGRA() { B = 100, G = 0, R = 100, A = 255 });

    Image = SKImage.FromPixels(Bitmap.Info, Bitmap.GetPixels());
    //Image = SKImage.FromBitmap(Bitmap);
  }

  public override void Render(DrawingContext context)
  {
    context.Custom(new SkiaPlotRenderer(new Rect(0, 0, Bounds.Width, Bounds.Height), Bitmap, Image, counter++));
    Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
  }
}
