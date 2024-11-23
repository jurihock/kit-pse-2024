using System.Runtime.CompilerServices;

namespace Spaghetti.Core.Image;

public sealed class YzxImageMemoryLayout : ImageMemoryLayout
{
  private readonly long Y;
  private readonly long Z;

  public YzxImageMemoryLayout(ImageShape shape) : base(shape)
  {
    Y = 1L * Shape.Channels * Shape.Width;
    Z = 1L * Shape.Width;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override long Flatten(int x, int y, int z) => x + y * Y + z * Z;
}
