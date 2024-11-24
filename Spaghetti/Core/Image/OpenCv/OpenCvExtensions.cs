using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spaghetti.Core.Image.OpenCv;

public static class OpenCvExtensions
{
  public static Type TypeOf(this MatType type)
  {
    return type.Depth switch
    {
      MatType.CV_8U => typeof(byte),
      MatType.CV_8S => typeof(sbyte),
      MatType.CV_16U => typeof(ushort),
      MatType.CV_16S => typeof(short),
      MatType.CV_32S => typeof(int),
      MatType.CV_32F => typeof(float),
      MatType.CV_64F => typeof(double),
      _ => throw new NotSupportedException(
        $"Unsupported OpenCv data type \"{type}\"!")
    };
  }

  public static int SizeOf(this MatType type)
  {
    return Marshal.SizeOf(type.TypeOf());
  }

  public static unsafe T UnsafeRead<T>(this Mat mat, long offset)
  {
    Debug.Assert(mat.IsContinuous());

    return Unsafe.Read<T>(mat.DataPointer + offset);
  }

  public static Func<long, T> CreateValueGetter<T>(this Mat mat)
  {
    var matrix = Expression.Constant(mat);
    var method = typeof(OpenCvExtensions).GetMethod(nameof(UnsafeRead))!
                                         .MakeGenericMethod(mat.Type().TypeOf());

    var index = Expression.Parameter(typeof(long));
    var bytes = Expression.Constant((long)mat.Type().SizeOf());
    var offset = Expression.Multiply(index, bytes);

    var input = Expression.Call(method, matrix, offset);
    var output = Expression.Convert(input, typeof(T));

    return Expression.Lambda<Func<long, T>>(output, index).Compile();
  }
}
