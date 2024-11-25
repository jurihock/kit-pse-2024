using System;

namespace Spaghetti.Core.Image;

/// <summary>
/// Indicates an image stored in a contiguous memory block.
/// </summary>
/// <typeparam name="T">
/// Type of the image value, e.g. byte, int, float, or double.
/// </typeparam>
public interface IContiguousImage<T> : IImage<T>
{
  /// <summary>
  /// Arrangement of the image values in memory.
  /// </summary>
  ImageMemoryLayout Layout { get; }

  /// <summary>
  /// Gets image value of type T at position I.
  /// </summary>
  /// <param name="i">Flat image value index in range [0..W*H*C).</param>
  T this[long i] { get; }
}
