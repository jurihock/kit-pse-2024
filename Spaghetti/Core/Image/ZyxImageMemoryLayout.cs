using System.Runtime.CompilerServices;

namespace Spaghetti.Core.Image;

public sealed class ZyxImageMemoryLayout : ImageMemoryLayout
{
  private readonly long Y;
  private readonly long Z;

  public ZyxImageMemoryLayout(ImageShape shape) : base(shape)
  {
    Y = 1L * Shape.Width;
    Z = 1L * Shape.Height * Shape.Width;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override long Flatten(int x, int y, int z) => x + y * Y + z * Z;
}
