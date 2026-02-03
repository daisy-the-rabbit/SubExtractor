namespace DvdSubOcr;

public class BlockEncode : IComparable<BlockEncode>, IEquatable<BlockEncode>
{
    int? pixelCount;

    public BlockEncode(string fullEncode)
        : this(Point.Empty, fullEncode, 0)
    {
    }

    public BlockEncode(Point origin, string fullEncode, int colorBitFlags)
    {
        this.FullEncode = fullEncode;
        this.Origin = origin;
        this.Width = Int32.Parse(fullEncode.Substring(0, 3));
        this.Encode = fullEncode.Substring(3);
        this.Height = this.Encode.Length / (this.Width / 4);
        this.ColorBitFlags = colorBitFlags;

        int charWidth = this.Width / 4;
        int lastColumnWidth = 0;
        for (int index = charWidth - 1; index < this.Encode.Length; index += charWidth)
        {
            int columnWidth = this.Encode[index] switch
            {
                '1' or '3' or '5' or '7' or '9' or 'b' or 'B' or 'd' or 'D' or 'f' or 'F' => 4,
                '2' or '6' or 'a' or 'A' or 'e' or 'E' => 3,
                '4' or 'c' or 'C' => 2,
                '8' => 1,
                _ => 0,
            };
            lastColumnWidth = Math.Max(lastColumnWidth, columnWidth);
        }
        this.TrueWidth = this.Width - 4 + lastColumnWidth;
    }

    public string FullEncode { get; private set; }
    public Point Origin { get; private set; }
    public string Encode { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Size Size { get { return new Size(TrueWidth, Height); } }
    public int ColorBitFlags { get; private set; }
    public int TrueWidth { get; private set; }

    public int PixelCount
    {
        get
        {
            if (!this.pixelCount.HasValue)
            {
                this.pixelCount = 0;
                foreach (Char c in this.Encode)
                {
                    pixelCount += CharacterCount(c);
                }
            }
            return this.pixelCount.Value;
        }
    }

    private int OriginOrder { get { return (this.Origin.X + this.TrueWidth / 2) * 10000 + this.Origin.Y; } }
    public int HeightOrder { get { return (this.Origin.Y + this.Height) * 10000 + this.Origin.X + this.TrueWidth / 2; } }

    public int CompareTo(BlockEncode other)
    {
        return this.OriginOrder.CompareTo(other.OriginOrder);
    }

    public bool IsMatch(OcrEntry entry, IList<BlockEncode> otherBlocks, HashSet<int> illegalBlocks,
        IList<int> matchIndexes, OcrMap ocrMap, bool isHighDef)
    {
        matchIndexes.Clear();
        if (!ocrMap.IsMatch(this.FullEncode, entry.FullEncode, isHighDef))
        {
            return false;
        }

        foreach (KeyValuePair<Point, string> piece in entry.ExtraPieces)
        {
            int oldMatchCount = matchIndexes.Count;
            for (int blockIndex = 0; blockIndex < otherBlocks.Count; blockIndex++)
            {
                if (!illegalBlocks.Contains(blockIndex))
                {
                    Point offset = otherBlocks[blockIndex].Origin - new Size(this.Origin);
                    if (ocrMap.IsMatch(offset, otherBlocks[blockIndex].FullEncode, piece.Key, piece.Value, isHighDef))
                    {
                        matchIndexes.Add(blockIndex);
                        break;
                    }
                }
            }
            if (matchIndexes.Count == oldMatchCount)
            {
                matchIndexes.Clear();
                return false;
            }
        }
        return true;
    }

    public Rectangle CalculateBounds(IList<int> extraBlocks, IList<BlockEncode> blocks)
    {
        Rectangle bounds = new Rectangle(this.Origin, new Size(this.TrueWidth, this.Height));
        foreach (int index in extraBlocks)
        {
            BlockEncode block = blocks[index];
            bounds = Rectangle.Union(bounds,
                new Rectangle(block.Origin, new Size(block.TrueWidth, block.Height)));
        }
        return bounds;
    }

    public Bitmap CreateBlockBitmap(Color foreColor, Color backColor,
        int minimumWidth, int minimumHeight)
    {
        Bitmap bmp = new Bitmap(Math.Max(this.Width + 4, minimumWidth),
            Math.Max(this.Height + 4, minimumHeight));
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(backColor);
        }

        IList<bool> decoded = DecodeToBoolArray();
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                if (decoded[y * this.Width + x])
                {
                    bmp.SetPixel(x + 2, y + 2, foreColor);
                }
            }
        }
        return bmp;
    }

    public IList<bool> DecodeToBoolArray()
    {
        bool[] decode = new bool[this.Width * this.Height];

        int decIndex = 0;
        foreach (char c in this.Encode)
        {
            int hex = HexCharToValue(c);
            decode[decIndex++] = ((hex & 8) != 0);
            decode[decIndex++] = ((hex & 4) != 0);
            decode[decIndex++] = ((hex & 2) != 0);
            decode[decIndex++] = ((hex & 1) != 0);
        }
        return decode;
    }

    public IList<bool> DecodeToBoolArray(int border)
    {
        int newWidth = this.Width + border * 2;
        int newHeight = this.Height + border * 2;
        bool[] decode = new bool[newWidth * newHeight];

        int decIndex = border * (1 + newWidth);
        int lineWidth = this.Width / 4;
        foreach (char c in this.Encode)
        {
            int hex = HexCharToValue(c);
            decode[decIndex++] = ((hex & 8) != 0);
            decode[decIndex++] = ((hex & 4) != 0);
            decode[decIndex++] = ((hex & 2) != 0);
            decode[decIndex++] = ((hex & 1) != 0);
            lineWidth--;
            if (lineWidth == 0)
            {
                decIndex += 2 * border;
                lineWidth = this.Width / 4;
            }
        }
        return decode;
    }

    static int CharacterCount(char c) => c switch
    {
        '0' => 0,
        '1' or '2' or '4' or '8' => 1,
        '3' or '5' or '6' or '9' or 'a' or 'A' or 'c' or 'C' => 2,
        '7' or 'b' or 'B' or 'd' or 'D' or 'e' or 'E' => 3,
        'f' or 'F' => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(c)),
    };

    public static int HexCharToValue(char c)
    {
        if ((c >= '0') && (c <= '9'))
        {
            return (int)(c - '0');
        }
        if ((c >= 'a') && (c <= 'f'))
        {
            return 10 + (int)(c - 'a');
        }
        if ((c >= 'A') && (c <= 'F'))
        {
            return 10 + (int)(c - 'A');
        }
        throw new ArgumentOutOfRangeException("c");
    }

    public bool Equals(BlockEncode other)
    {
        return (this.Origin == other.Origin) && (this.FullEncode == other.FullEncode);
    }
}

