using System.Runtime.InteropServices;

namespace Spaghetti.Core.Bitmap;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BGRA
{
  public byte B;
  public byte G;
  public byte R;
  public byte A;
}
