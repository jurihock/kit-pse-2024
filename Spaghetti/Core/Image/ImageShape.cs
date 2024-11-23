using System;

namespace Spaghetti.Core.Image;

public readonly struct ImageShape : IEquatable<ImageShape>
{
  public int Width { get; }
  public int Height { get; }
  public int Channels { get; }

  public long Area { get; }
  public long Volume { get; }

  public ImageShape(int width, int height, int channels)
  {
    Width = width;
    Height = height;
    Channels = channels;

    Area = (long)Width * (long)Height;
    Volume = (long)Width * (long)Height * (long)Channels;
  }

  public ImageShape(ImageShape other)
  {
    Width = other.Width;
    Height = other.Height;
    Channels = other.Channels;

    Area = other.Area;
    Volume = other.Volume;
  }

  public static bool operator ==(ImageShape left, ImageShape right) => left.Equals(right);
  public static bool operator !=(ImageShape left, ImageShape right) => !left.Equals(right);

  public bool Equals(ImageShape other)
  {
    if (this.Width != other.Width)
    {
      return false;
    }

    if (this.Height != other.Height)
    {
      return false;
    }

    if (this.Channels != other.Channels)
    {
      return false;
    }

    return true;
  }

  public override bool Equals(object? obj)
  {
    if (obj is ImageShape other)
    {
      return other.Equals(this);
    }

    return false;
  }

  public override string ToString()
  {
    return $"{Width}x{Height}x{Channels}";
  }

  public override int GetHashCode()
  {
    return $"{Width}x{Height}x{Channels}".GetHashCode();
  }
}
