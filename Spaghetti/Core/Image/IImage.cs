using System;

namespace Spaghetti.Core.Image;

public interface IImage<T>
{
  ImageShape Shape { get; }

  T this[int x, int y, int z] { get; }
}
