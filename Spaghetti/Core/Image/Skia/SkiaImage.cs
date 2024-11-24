using SkiaSharp;
using System;

namespace Spaghetti.Core.Image.Skia;

public sealed class SkiaImage<T> : IContiguousImage<T>, IDisposable
{
  private SKBitmap? Data { get; set; }
  private Func<long, T> GetValue { get; init; }
  private Func<int, int> GetColor { get; init; }

  public T this[int x, int y, int z] => GetValue(Layout.Flatten(x, y, GetColor(z)));
  public T this[long i] => GetValue(i);

  public ImageShape Shape { get; init; }
  public ImageMemoryLayout Layout { get; init; }

  public SkiaImage(string filepath)
  {
    var bmp = SKBitmap.Decode(filepath);

    var width = bmp.Width;
    var height = bmp.Height;
    var channels = bmp.BytesPerPixel / bmp.ColorType.SizeOf();

    Data = bmp;
    GetValue = bmp.CreateValueGetter<T>();
    GetColor = bmp.CreateColorChannelDecoder();

    Shape = new ImageShape(width, height, channels);
    Layout = new YxzImageMemoryLayout(Shape);
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void Dispose(bool disposing)
  {
    if (!disposing)
    {
      return;
    }

    if (Data != null)
    {
      Data.Dispose();
      Data = null;
    }
  }
}
