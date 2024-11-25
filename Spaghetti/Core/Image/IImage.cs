using System;

namespace Spaghetti.Core.Image;

/// <summary>
/// Generic image data access interface.
/// </summary>
/// <typeparam name="T">
/// Type of the image value, e.g. byte, int, float, or double.
/// </typeparam>
public interface IImage<T>
{
  /// <summary>
  /// Image width, height and number of color channels.
  /// </summary>
  ImageShape Shape { get; }

  /// <summary>
  /// Gets image value of type T at position X, Y, and Z.
  /// </summary>
  /// <param name="x">Image pixel index in range [0..W).</param>
  /// <param name="y">Image line index in range [0..H).</param>
  /// <param name="z">Image channel index in range [0..C).</param>
  T this[int x, int y, int z] { get; }
}
