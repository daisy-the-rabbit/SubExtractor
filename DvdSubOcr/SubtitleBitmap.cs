using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DvdSubOcr;

public class SubtitleBitmap : SubtitleInformation, IDisposable
{
    // Guards the unmanaged allocation below against integer overflow: width/height derive
    // from untrusted subtitle data (a Blu-ray PGS object union can reach ~131070), and
    // Stride * height must stay within int. Real DVD (<=720) and Blu-ray (<=3840) subtitles
    // are far below this ceiling, so a valid subtitle is never rejected.
    private const int MaxDimension = 16384;

    bool isDisposed;

    public SubtitleBitmap(int left, int top, int width, int height, double pts, double duration,
        Color[] paletteEntries, bool isForced) : base(left, top, width, height,
        pts, duration, paletteEntries, isForced)
    {
        if (width < 0 || height < 0 || width > MaxDimension || height > MaxDimension)
        {
            throw new ArgumentOutOfRangeException(nameof(width),
                $"Subtitle bitmap dimensions {width}x{height} exceed the supported {MaxDimension} limit.");
        }
        this.Stride = (width + 3) / 4 * 4;
        this.Data = Marshal.AllocCoTaskMem(checked(this.Stride * height));
        this.Bitmap = new Bitmap(width, height, this.Stride, PixelFormat.Format8bppIndexed, this.Data);
        this.Bitmap.SetResolution(96, 96);
        ColorPalette palette = this.Bitmap.Palette;
        for (int index = 0; index < paletteEntries.Length; index++)
        {
            if (paletteEntries[index].A > 0)
            {
                // you can get some funny blending if you let partially transparent colors into a windows bitmap
                palette.Entries[index] = Color.FromArgb(255, paletteEntries[index]);
            }
            else
            {
                palette.Entries[index] = paletteEntries[index];
            }
        }
        this.Bitmap.Palette = palette;
    }

    public void UpdatePalette(Color[] paletteEntries)
    {
        base.RgbPalette = new List<Color>(paletteEntries).AsReadOnly();
        ColorPalette palette = this.Bitmap.Palette;
        for (int index = 0; index < paletteEntries.Length; index++)
        {
            palette.Entries[index] = paletteEntries[index];
        }
        this.Bitmap.Palette = palette;
    }

    public int Stride { get; private set; }
    public Bitmap Bitmap { get; private set; }
    public IntPtr Data { get; private set; }

    public void Dispose()
    {
        if (!this.isDisposed)
        {
            this.isDisposed = true;
            this.Bitmap.Dispose();
            Marshal.FreeCoTaskMem(this.Data);
        }
    }
}

