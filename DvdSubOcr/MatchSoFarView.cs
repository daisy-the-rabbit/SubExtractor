using System.ComponentModel;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;

namespace DvdSubOcr;
public partial class MatchSoFarView : UserControl
{
    SolidBrush backgroundBrush = new SolidBrush(SystemColors.Control);
    SolidBrush foregroundBrush = new SolidBrush(SystemColors.ControlText);
    List<BlockEncode> blocks = [];
    List<EncodeMatch> matches = [];
    Font normal;
    Font italic;
    Image backgroundImage;
    Point origin;
    float xFactor;
    float yFactor;

    public MatchSoFarView()
    {
        InitializeComponent();
    
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.Opaque, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.Selectable, false);
        UpdateStyles();

        CreateFonts();
    }

    void CreateFonts()
    {
        float dpi = this.DeviceDpi;
        normal?.Dispose();
        italic?.Dispose();

        normal = DpiHelper.CreateScaledFont("Tahoma", 11.0f, FontStyle.Regular, dpi);
        italic = DpiHelper.CreateScaledFont("Tahoma", 11.0f, FontStyle.Italic, dpi);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        CreateFonts();
        Invalidate();
    }

    public void Update(IEnumerable<BlockEncode> blocks, IEnumerable<EncodeMatch> matches)
    {
        this.blocks = new List<BlockEncode>(blocks);
        this.matches = new List<EncodeMatch>(matches);
        Invalidate();
    }

    public void UpdateBackground(Image image, Point origin, Size videoSize)
    {
        if(this.backgroundImage != null)
        {
            this.backgroundImage.Dispose();
            this.backgroundImage = null;
        }

        ColorPalette palette = image.Palette;
        List<Color> savedColors = new List<Color>(palette.Entries);
        for(int index = 0; index < palette.Entries.Length; index++)
        {
            Color newColor = Color.FromArgb(palette.Entries[index].A / 2, palette.Entries[index]);
            palette.Entries[index] = newColor;
        }
        image.Palette = palette;

        this.origin = origin;
        this.backgroundImage = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
        this.xFactor = (float)this.ClientRectangle.Width / videoSize.Width;
        this.yFactor = (float)this.ClientRectangle.Height / videoSize.Height;
        float yOffset = 0.0f;
        if(this.yFactor > this.xFactor)
        {
            yOffset = this.ClientRectangle.Height - (videoSize.Height * this.xFactor);
            this.yFactor = this.xFactor;
        }
        using(Graphics g = Graphics.FromImage(this.backgroundImage))
        {
            RectangleF rect = new RectangleF(
                origin.X * this.xFactor, yOffset + origin.Y * this.yFactor,
                image.Width * this.xFactor, image.Height * this.yFactor);
            g.DrawImage(image, rect);
        }

        for(int index = 0; index < palette.Entries.Length; index++)
        {
            palette.Entries[index] = savedColors[index];
        }
        image.Palette = palette;
        Invalidate();
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        this.foregroundBrush.Dispose();
        this.foregroundBrush = new SolidBrush(this.ForeColor);
        Invalidate();
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        this.backgroundBrush.Dispose();
        this.backgroundBrush = new SolidBrush(this.BackColor);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.FillRectangle(backgroundBrush, e.ClipRectangle);
        if(this.backgroundImage != null)
        {
            e.Graphics.DrawImage(this.backgroundImage, this.ClientRectangle);
        }

        Rectangle rect = this.ClientRectangle;
        rect.Width--;
        rect.Height--;
        e.Graphics.DrawRectangle(SystemPens.ControlDark, rect);

        for(int index = 0; index < this.blocks.Count; index++)
        {
            EncodeMatch match = this.matches[index];
            if((match != null) && (match.OcrEntry.OcrCharacter != OcrCharacter.Unmatched))
            {
                BlockEncode block = this.blocks[index];
                Point p = Point.Round(new PointF(block.Origin.X * this.xFactor * 1.4f, block.Origin.Y * this.yFactor * 1.4f));
                Font f = this.normal;
                if(match.OcrEntry.OcrCharacter.Italic)
                {
                    f = this.italic;
                }
                e.Graphics.DrawString(new string(match.OcrEntry.OcrCharacter.Value, 1),
                    f, this.foregroundBrush, p);
            }
        }
    }
}

