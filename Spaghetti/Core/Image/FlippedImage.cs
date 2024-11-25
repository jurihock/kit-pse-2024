using System;

namespace Spaghetti.Core.Image;

/// <summary>
/// Flips the image along the Y axis.
/// </summary>
/// <param name="image">Image instance to wrap.</param>
public sealed class FlippedImage<T>(IImage<T> image) : IImage<T>
{
  public IImage<T> Image { get; } = image;
  public ImageShape Shape => Image.Shape;

  public T this[int x, int y, int z] =>
    Image[x, Shape.Height - (y + 1), z];
}
