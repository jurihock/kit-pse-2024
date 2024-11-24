using System;

namespace Spaghetti.Core.Image;

public static class ImageExtensions
{
  public static IImage<T> Flip<T>(this IImage<T> image)
  {
    return new FlippedImage<T>(image);
  }
}
