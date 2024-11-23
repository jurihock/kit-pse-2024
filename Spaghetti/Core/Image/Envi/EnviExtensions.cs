using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Spaghetti.Core.Image.Envi;

public static class EnviExtensions
{
  public static EnviByteOrder GetNativeByteOrder()
  {
    return BitConverter.IsLittleEndian
      ? EnviByteOrder.Host
      : EnviByteOrder.Network;
  }

  public static bool IsNative(this EnviByteOrder order)
  {
    return order == GetNativeByteOrder();
  }

  public static bool IsComplex(this EnviDataType type)
  {
    return type switch
    {
      EnviDataType.ComplexSingle => true,
      EnviDataType.ComplexDouble => true,
      _ => false
    };
  }

  public static Type TypeOf(this EnviDataType type)
  {
    return type switch
    {
      EnviDataType.Byte => typeof(byte),
      EnviDataType.UInt16 => typeof(ushort),
      EnviDataType.Int16 => typeof(short),
      EnviDataType.UInt32 => typeof(uint),
      EnviDataType.Int32 => typeof(int),
      EnviDataType.UInt64 => typeof(ulong),
      EnviDataType.Int64 => typeof(long),
      EnviDataType.Single => typeof(float),
      EnviDataType.Double => typeof(double),
      _ => throw new ArgumentException(
        $"Unsupported ENVI data type \"{type}\"!")
    };
  }

  public static int SizeOf(this EnviDataType type)
  {
    return Marshal.SizeOf(type.TypeOf());
  }

  public static Func<long, T> CreateValueGetter<T>(this MemoryMappedViewAccessor accessor, EnviDataType type)
  {
    var instance = Expression.Constant(accessor);
    var method = instance.Type.GetMethod($"Read{type.TypeOf().Name}")!;

    var index = Expression.Parameter(typeof(long));
    var bytes = Expression.Constant((long)type.SizeOf());
    var offset = Expression.Multiply(index, bytes);

    var input = Expression.Call(instance, method, offset);
    var output = Expression.Convert(input, typeof(T));

    return Expression.Lambda<Func<long, T>>(output, index).Compile();
  }

  public static Dictionary<string, string> ParseHeaderFile(string hdr)
  {
    return ParseHeaderString(File.ReadAllText(hdr));
  }

  public static Dictionary<string, string> ParseHeaderString(string hdr)
  {
    return new Dictionary<string, string>(hdr
      .Split(Environment.NewLine)
      .Select(line => line.Trim())
      .Where(line => !string.IsNullOrEmpty(line))
      .Select(line => line.Split(['='], 2))
      .Where(line => line.Length == 2)
      .Select(line => KeyValuePair.Create(
        line.First().Trim(),
        line.Last().Trim())),
      StringComparer.OrdinalIgnoreCase);
  }

  public static TValue ParseHeaderValue<TValue>(IReadOnlyDictionary<string, string> hdr, string key, object? def = null)
  {
    var value = hdr.GetValueOrDefault(key) ?? def?.ToString();

    if (value is null)
    {
      throw new ArgumentException(
        $"Missing ENVI header field \"{key}\"!");
    }
    else if (typeof(TValue).IsEnum)
    {
      return (TValue)Enum.Parse(
        typeof(TValue), value, ignoreCase: true);
    }
    else
    {
      return (TValue)Convert.ChangeType(
        value, typeof(TValue));
    }
  }
}
