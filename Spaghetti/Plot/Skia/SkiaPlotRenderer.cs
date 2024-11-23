using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using Spaghetti.Core.Bitmap;
using System.Runtime.InteropServices;

namespace Spaghetti.Plot.Skia;

public sealed class SkiaPlotRenderer : ICustomDrawOperation
{
  private SKBitmap Bitmap;
  private SKImage Image;
  private byte Counter;

  public Rect Bounds { get; }

  public SkiaPlotRenderer(Rect bounds, SKBitmap bitmap, SKImage image, byte counter)
  {
    Bounds = bounds;
    Bitmap = bitmap;
    Image = image;
    Counter = counter;
  }

  public void Dispose() { }
  public bool Equals(ICustomDrawOperation? other) => false;
  public bool HitTest(Point point) => false;

  public void Render(ImmediateDrawingContext context)
  {
    var feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();

    if (feature == null)
    {
      return; // TODO: not skia context
    }

    using var lease = feature.Lease();
    var canvas = lease.SkCanvas;

    if (canvas == null)
    {
      return; // TODO: wtf
    }

    lease.GrContext?.SetResourceCacheLimit(1L * 1024 * 1024 * 1024); // depending on bitmap size

    // draw something

    var src = new SKRect(0, 0, Image.Width, Image.Height);
    var roi = new SKRect((float)Bounds.X, (float)Bounds.Y, (float)Bounds.Width, (float)Bounds.Height);

    var opt = new SKSamplingOptions(SKFilterMode.Nearest);

    canvas.Save();
    //canvas.Clear();

    var bytes = Bitmap.GetPixelSpan();
    var pixels = MemoryMarshal.CreateSpan(
      ref MemoryMarshal.AsRef<BGRA>(bytes),
      bytes.Length / Marshal.SizeOf<BGRA>());
    pixels.Fill(new BGRA() { B = 100, G = Counter, R = 100, A = 255 });

    if (true)
    {
      canvas.DrawBitmap(Bitmap, roi); // ok
    }
    else
    {
      canvas.DrawImage(Image, src, roi, opt); // nok
    }

    canvas.Restore();
    //canvas.Flush();
  }
}
