using System;

namespace Spaghetti.Core.Image;

public interface IContiguousImage<T> : IImage<T>
{
  ImageMemoryLayout Layout { get; }

  T this[long i] { get; }
}
