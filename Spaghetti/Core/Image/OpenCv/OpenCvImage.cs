using OpenCvSharp;
using System;
using System.IO;

namespace Spaghetti.Core.Image.OpenCv;

public sealed class OpenCvImage<T> : IImage<T>, IDisposable
{
  private Mat? Data { get; set; }
  private Func<long, T> GetValue { get; init; }

  public T this[int x, int y, int z] => GetValue(Layout.Flatten(x, y, z));
  public T this[long i] => GetValue(i);

  public ImageShape Shape { get; init; }
  public ImageMemoryLayout Layout { get; init; }

  public OpenCvImage(string filepath)
  {
    var mat = Cv2.ImRead(filepath, ImreadModes.Unchanged);

    if (mat.Dims != 2)
    {
      throw new FileLoadException(
        "The OpenCV matrix is not 2D!");
    }

    if (!mat.IsContinuous())
    {
      throw new FileLoadException(
        "The OpenCV matrix is not continuous!");
    }

    var width = mat.Width;
    var height = mat.Height;
    var channels = mat.Channels();

    Data = mat;
    GetValue = mat.CreateValueGetter<T>();

    Shape = new ImageShape(width, height, channels);
    Layout = new YxzImageMemoryLayout(Shape);
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void Dispose(bool disposing)
  {
    if (!disposing)
    {
      return;
    }

    if (Data != null)
    {
      Data.Dispose();
      Data = null;
    }
  }
}
