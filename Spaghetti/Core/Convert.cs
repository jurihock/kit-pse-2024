using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Spaghetti.Core;

public static class Convert<Tin>
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Func<Tin, Tout> To<Tout>()
  {
    Debug.Assert(typeof(IConvertible).IsAssignableFrom(typeof(Tin)));
    Debug.Assert(typeof(IConvertible).IsAssignableFrom(typeof(Tout)));

    var input = Expression.Parameter(typeof(Tin));
    var output = Expression.Convert(input, typeof(Tout));

    return Expression.Lambda<Func<Tin, Tout>>(output, input).Compile();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Tout To<Tout>(Tin x)
  {
    Debug.Assert(typeof(IConvertible).IsAssignableFrom(typeof(Tin)));
    Debug.Assert(typeof(IConvertible).IsAssignableFrom(typeof(Tout)));

    if (Convert.ChangeType(x, typeof(Tout)) is Tout y)
    {
      return y;
    }

    throw new InvalidCastException();
  }
}
