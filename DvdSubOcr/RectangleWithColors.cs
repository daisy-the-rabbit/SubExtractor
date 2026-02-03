namespace DvdSubOcr;
public class RectangleWithColors
{
    SortedSet<int> colors = new SortedSet<int>();

    public RectangleWithColors(Rectangle rect, int colorIndex)
    {
        this.Rectangle = rect;
        this.ColorIndexes = colors;
        this.colors.Add(colorIndex);
    }

    public Rectangle Rectangle { get; private set; }
    public ICollection<int> ColorIndexes { get; private set; }

    public void Add(Rectangle rect, int colorIndex)
    {
        this.Rectangle = Rectangle.Union(this.Rectangle, rect);
        this.colors.Add(colorIndex);
    }

    public void TrimRectangle(int xTrim, int yTrim)
    {
        this.Rectangle = Rectangle.Inflate(this.Rectangle, -xTrim, -yTrim);
    }

    public void MergeWith(RectangleWithColors other)
    {
        this.Rectangle = Rectangle.Union(this.Rectangle, other.Rectangle);
        this.colors.UnionWith(other.ColorIndexes);
    }

    public override string ToString()
    {
        return $"{this.Rectangle} Colors {this.ColorIndexes.Aggregate("", (concat, current) => (concat.Length == 0) ? current.ToString() : concat + ", " + current.ToString())}";
    }
}
