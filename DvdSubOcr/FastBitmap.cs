//Downloaded from Visual C# Kicks - http://www.vcskicks.com/
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DvdSubOcr;

public class FastBitmap
{
    private const int BytesPerPixel = 4; // Format32bppArgb

    public int Width { get; set; }
    public int Height { get; set; }

    private readonly Bitmap _workingBitmap;
    private int _stride;
    private BitmapData _bitmapData;
    private IntPtr _scan0;
    private int _currentPixelOffset;

    public FastBitmap(Bitmap inputBitmap)
    {
        _workingBitmap = inputBitmap;

        Width = inputBitmap.Width;
        Height = inputBitmap.Height;
    }

    public void LockImage()
    {
        var bounds = new Rectangle(Point.Empty, _workingBitmap.Size);

        //Lock Image
        _bitmapData = _workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        _stride = _bitmapData.Stride;
        _scan0 = _bitmapData.Scan0;
    }

    public Color GetPixel(int x, int y)
    {
        _currentPixelOffset = y * _stride + x * BytesPerPixel;
        return Color.FromArgb(
            Marshal.ReadByte(_scan0, _currentPixelOffset + 3),
            Marshal.ReadByte(_scan0, _currentPixelOffset + 2),
            Marshal.ReadByte(_scan0, _currentPixelOffset + 1),
            Marshal.ReadByte(_scan0, _currentPixelOffset));
    }

    public Color GetPixelNext()
    {
        _currentPixelOffset += BytesPerPixel;
        return Color.FromArgb(
            Marshal.ReadByte(_scan0, _currentPixelOffset + 3),
            Marshal.ReadByte(_scan0, _currentPixelOffset + 2),
            Marshal.ReadByte(_scan0, _currentPixelOffset + 1),
            Marshal.ReadByte(_scan0, _currentPixelOffset));
    }

    public void SetPixel(int x, int y, Color color)
    {
        int offset = y * _stride + x * BytesPerPixel;
        Marshal.WriteByte(_scan0, offset, color.B);
        Marshal.WriteByte(_scan0, offset + 1, color.G);
        Marshal.WriteByte(_scan0, offset + 2, color.R);
        Marshal.WriteByte(_scan0, offset + 3, color.A);
    }

    public Bitmap GetBitmap()
    {
        return _workingBitmap;
    }

    public void UnlockImage()
    {
        _workingBitmap.UnlockBits(_bitmapData);
        _bitmapData = null;
        _scan0 = IntPtr.Zero;
    }
}

