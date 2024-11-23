using SkiaSharp;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Spaghetti.Core.Bitmap;

public sealed class BgraBitmap : SKBitmap, IBitmap<BGRA>
{
  public ref BGRA this[int x, int y]
  {
    get
    {
      var bytes = GetPixelSpan().Slice(
        y * RowBytes +
        x * BytesPerPixel,
        BytesPerPixel);

      return ref MemoryMarshal.AsRef<BGRA>(bytes);
    }
  }

  public ref BGRA this[long i]
  {
    get
    {
      var x = (int)(i % Width);
      var y = (int)(i / Width);

      var bytes = GetPixelSpan().Slice(
        y * RowBytes +
        x * BytesPerPixel,
        BytesPerPixel);

      return ref MemoryMarshal.AsRef<BGRA>(bytes);
    }
  }

  public BgraBitmap(int width, int height) :
    base(width, height, SKColorType.Bgra8888, SKAlphaType.Premul)
  {
    Debug.Assert(Marshal.SizeOf<BGRA>() == BytesPerPixel);
  }

  public void Read(string path)
  {
    throw new NotImplementedException("TODO");
  }

  public void Write(string path)
  {
    throw new NotImplementedException("TODO");
  }
}
