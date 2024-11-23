using System;

namespace Spaghetti.Core.Image;

public interface IImage<T>
{
  ImageShape Shape { get; }
  ImageMemoryLayout Layout { get; }

  T this[int x, int y, int z] { get; }
  T this[long i] { get; }
}
