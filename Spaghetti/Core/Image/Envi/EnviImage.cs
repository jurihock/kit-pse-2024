using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Spaghetti.Core.Image.Envi;

public sealed class EnviImage<T> : IImage<T>, IDisposable
{
  private string FileID { get; } = Guid.NewGuid().ToString();
  private MemoryMappedFile? FileMapping { get; set; }
  private MemoryMappedViewAccessor? FileAccessor { get; set; }
  private Func<long, T> GetValue { get; init; }

  public ImageShape Shape { get; init; }
  public ImageMemoryLayout Layout { get; init; }

  public T this[int x, int y, int z] => GetValue(Layout.Flatten(x, y, z));
  public T this[long i] => GetValue(i);

  public EnviImage(string filepath) : this(filepath, (hdr: ".hdr", raw: ".raw"))
  {
  }

  public EnviImage(string filepath, (string hdr, string raw) extensions)
  {
    var filepaths = (
      hdr: Path.ChangeExtension(filepath, extensions.hdr),
      raw: Path.ChangeExtension(filepath, extensions.raw));

    if (!File.Exists(filepaths.hdr))
    {
      throw new FileNotFoundException(
        $"Missing ENVI header file \"{filepaths.hdr}\"!");
    }

    if (!File.Exists(filepaths.raw))
    {
      throw new FileNotFoundException(
        $"Missing ENVI raw file \"{filepaths.raw}\"!");
    }

    var header = EnviExtensions.ParseHeaderFile(filepaths.hdr);

    var filetype = EnviExtensions.ParseHeaderValue<string>(header, "file type", "ENVI");
    var offset = EnviExtensions.ParseHeaderValue<int>(header, "header offset", 0);

    var byteorder = EnviExtensions.ParseHeaderValue<EnviByteOrder>(header, "byte order", EnviExtensions.GetNativeByteOrder());
    var datatype = EnviExtensions.ParseHeaderValue<EnviDataType>(header, "data type");
    var interleave = EnviExtensions.ParseHeaderValue<EnviInterleave>(header, "interleave");

    var width = EnviExtensions.ParseHeaderValue<int>(header, "samples");
    var height = EnviExtensions.ParseHeaderValue<int>(header, "lines");
    var channels = EnviExtensions.ParseHeaderValue<int>(header, "bands");

    if (!filetype.Contains("ENVI", StringComparison.OrdinalIgnoreCase))
    {
      throw new NotSupportedException(
        $"Invalid ENVI file type \"{filetype}\"!");
    }

    if (!byteorder.IsNative())
    {
      throw new NotSupportedException(
        $"Incompatible ENVI byte order \"{byteorder}\"!");
    }

    if (datatype.IsComplex())
    {
      throw new NotSupportedException(
        $"Unsupported ENVI data type \"{datatype}\"!");
    }

    var size = 1L * width * height * channels * datatype.SizeOf();

    FileMapping = MemoryMappedFile.CreateFromFile(
      filepaths.raw,
      FileMode.OpenOrCreate,
      FileID,
      size + offset,
      MemoryMappedFileAccess.Read);

    FileAccessor = FileMapping.CreateViewAccessor(
      offset,
      size,
      MemoryMappedFileAccess.Read);

    GetValue = FileAccessor.CreateValueGetter<T>(
      datatype);

    Shape = new ImageShape(
      width,
      height,
      channels);

    Layout = interleave switch
    {
      EnviInterleave.BIP => new YxzImageMemoryLayout(Shape),
      EnviInterleave.BIL => new YzxImageMemoryLayout(Shape),
      EnviInterleave.BSQ => new ZyxImageMemoryLayout(Shape),
      _ => throw new NotSupportedException(
        $"Unsupported ENVI interleave \"{interleave}\"!")
    };
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

    if (FileAccessor != null)
    {
      FileAccessor.Dispose();
      FileAccessor = null;
    }

    if (FileMapping != null)
    {
      FileMapping.Dispose();
      FileMapping = null;
    }
  }
}