using System.Runtime.InteropServices;

namespace Spaghetti.Core.Bitmap;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BGR
{
  public byte B;
  public byte G;
  public byte R;
}
