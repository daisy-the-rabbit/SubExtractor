namespace DvdSubOcr;

public static class DpiHelper
{
    private const float BaseDpi = 96.0f;

    public static int Scale(int value, float deviceDpi)
    {
        return (int)Math.Round(value * deviceDpi / BaseDpi);
    }

    public static float Scale(float value, float deviceDpi)
    {
        return value * deviceDpi / BaseDpi;
    }

    public static Size Scale(Size size, float deviceDpi)
    {
        return new Size(Scale(size.Width, deviceDpi), Scale(size.Height, deviceDpi));
    }

    public static Point Scale(Point point, float deviceDpi)
    {
        return new Point(Scale(point.X, deviceDpi), Scale(point.Y, deviceDpi));
    }

    public static float GetScaleFactor(float deviceDpi)
    {
        return deviceDpi / BaseDpi;
    }

    public static Font CreateScaledFont(string familyName, float pointSize, FontStyle style, float deviceDpi)
    {
        // Point-based fonts are inherently DPI-independent — GDI+ renders them
        // at the correct physical size for the current DPI automatically.
        // No additional scaling is needed; the deviceDpi parameter is accepted
        // for API consistency but not used for point-based fonts.
        return new Font(familyName, pointSize, style, GraphicsUnit.Point);
    }
}

