using OxyPlot;
using Spaghetti.Core.Bitmap;
using System;

namespace Spaghetti.Plot.Extensions;

public static class BitmapExtensions
{
  public static OxyImage ToOxyImage(this IBitmap<BGR> bitmap)
  {
    return new OxyImage(bitmap.Bytes);
  }

  public static OxyImage ToOxyImage(this IBitmap<BGRA> bitmap)
  {
    throw new NotSupportedException("OxyPlot doesn't support BGRA bitmaps!");
  }
}
