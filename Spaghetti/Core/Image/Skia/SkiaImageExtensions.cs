using SkiaSharp;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spaghetti.Core.Image.Skia;

public static class SkiaImageExtensions
{
  public static Type TypeOf(this SKColorType type)
  {
    return type switch
    {
      SKColorType.Gray8 => typeof(byte),
      SKColorType.Bgra8888 => typeof(byte),
      _ => throw new NotSupportedException(
        $"Unsupported Skia data type \"{type}\"!")
    };
  }

  public static int SizeOf(this SKColorType type)
  {
    return Marshal.SizeOf(type.TypeOf());
  }

  public static unsafe T UnsafeRead<T>(this SKBitmap bmp, long offset)
  {
    return Unsafe.Read<T>((byte*)bmp.GetPixels() + offset);
  }

  public static Func<long, T> CreateValueGetter<T>(this SKBitmap bmp)
  {
    var bitmap = Expression.Constant(bmp);
    var method = typeof(SkiaImageExtensions).GetMethod(nameof(UnsafeRead))!
                                            .MakeGenericMethod(bmp.ColorType.TypeOf());

    var index = Expression.Parameter(typeof(long));
    var bytes = Expression.Constant((long)bmp.ColorType.SizeOf());
    var offset = Expression.Multiply(index, bytes);

    var input = Expression.Call(method, bitmap, offset);
    var output = Expression.Convert(input, typeof(T));

    return Expression.Lambda<Func<long, T>>(output, index).Compile();
  }
}
