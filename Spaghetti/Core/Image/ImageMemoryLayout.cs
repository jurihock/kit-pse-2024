using System;

namespace Spaghetti.Core.Image;

public abstract class ImageMemoryLayout
{
  public ImageShape Shape { get; init; }

  protected ImageMemoryLayout(ImageShape shape)
  {
    Shape = shape;
  }

  public abstract long Flatten(int x, int y, int z);
}
