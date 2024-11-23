using System.Runtime.CompilerServices;

namespace Spaghetti.Core.Image;

public sealed class XyzImageMemoryLayout : ImageMemoryLayout
{
  private readonly long X;
  private readonly long Y;

  public XyzImageMemoryLayout(ImageShape shape) : base(shape)
  {
    X = 1L * Shape.Height * Shape.Channels;
    Y = 1L * Shape.Channels;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override long Flatten(int x, int y, int z) => x * X + y * Y + z;
}
