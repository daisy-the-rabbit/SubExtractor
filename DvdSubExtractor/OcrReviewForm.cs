using System.ComponentModel;
using System.Data;
using DvdSubOcr;
using DvdNavigatorCrm;

namespace DvdSubExtractor;
public partial class OcrReviewForm : Form
{
    OcrMap ocrMap;
    int movieId;
    Brush highlightBrush;
    Brush backgroundBrush;

    public OcrReviewForm()
    {
        InitializeComponent();

        bool isDark = Application.ColorMode == SystemColorMode.Dark ||
            (Application.ColorMode == SystemColorMode.System &&
             Application.SystemColorMode == SystemColorMode.Dark);
        this.highlightBrush = new SolidBrush(isDark ? Color.DimGray : Color.LightGray);
        this.backgroundBrush = new SolidBrush(isDark ? Color.FromArgb(43, 43, 43) : Color.White);
    }

    public bool ChangesMade { get; private set; }

    public void LoadMap(OcrMap ocrMap, int movieId)
    {
        this.ocrMap = ocrMap;
        this.movieId = movieId;

        float dpi = this.DeviceDpi;
        bool isDark = Application.ColorMode == SystemColorMode.Dark ||
            (Application.ColorMode == SystemColorMode.System &&
             Application.SystemColorMode == SystemColorMode.Dark);
        Brush accentBrush = new SolidBrush(isDark ? Color.LightSkyBlue : Color.DodgerBlue);
        Color glyphColor = isDark ? Color.White : Color.Black;

        this.splitListBox.Items.Clear();
        foreach(KeyValuePair<string, OcrMap.SplitMapEntry> split in ocrMap.Splits)
        {
            if(split.Value.MovieIds.Contains(movieId))
            {
                SplitEntry entry = new SplitEntry(split.Key,
                    split.Value.Split1, split.Value.Split2, dpi, glyphColor, isDark);
                this.splitListBox.Items.Add(entry);
            }
        }
        if(this.splitListBox.Items.Count != 0)
        {
            this.splitListBox.SelectedIndex = 0;
        }
        else
        {
            this.splitListBox.Enabled = false;
            this.removeSplitButton.Enabled = false;
        }

        SortedDictionary<OcrCharacter, List<OcrEntry>> matchEntries =
            new SortedDictionary<OcrCharacter, List<OcrEntry>>();

        foreach(OcrEntry entry in this.ocrMap.GetMatchesForMovie(this.movieId, true))
        {
            List<OcrEntry> entries;
            if(!matchEntries.TryGetValue(entry.OcrCharacter, out entries))
            {
                entries = [];
                matchEntries[entry.OcrCharacter] = entries;
            }
            entries.Add(entry);
        }

        /*HashSet<string> highDefs = new HashSet<string>(this.ocrMap.HighDefMatches);
        foreach(var v in this.ocrMap.Matches)
        {
            if(highDefs.Contains(v.Key) && (v.Value.Count != 0))
            {
                foreach(OcrEntry ent in v.Value)
                {
                    List<OcrEntry> oldEntries;
                    if(!matchEntries.TryGetValue(ent.OcrCharacter, out oldEntries))
                    {
                        oldEntries = [];
                        matchEntries[ent.OcrCharacter] = oldEntries;
                    }
                    oldEntries.Add(ent);
                }
            }
        }*/

        foreach(KeyValuePair<OcrCharacter, List<OcrEntry>> ocr in matchEntries)
        {
            ocr.Value.Sort((ocr1, ocr2) => ocr1.CalculateBounds().Height.CompareTo(ocr2.CalculateBounds().Height));
            foreach(OcrEntry entry in ocr.Value)
            {
                this.ocrListBox.Items.Add(new OcrMatchEntry(entry, dpi, accentBrush, glyphColor));
            }
        }
        if(this.ocrListBox.Items.Count != 0)
        {
            this.ocrListBox.SelectedIndex = 0;
        }
        else
        {
            this.ocrListBox.Enabled = false;
            this.removeOcrButton.Enabled = false;
        }
    }

    void DataDispose()
    {
        this.split1PictureBox.Image = null;
        this.split2PictureBox.Image = null;
        foreach(SplitEntry split in this.splitListBox.Items)
        {
            split.Dispose();
        }
        this.splitListBox.Items.Clear();

        foreach(OcrMatchEntry entry in this.ocrListBox.Items)
        {
            entry.Dispose();
        }
        this.ocrListBox.Items.Clear();
    }

    class OcrMatchEntry : IDisposable
    {
        static readonly StringFormat CharFormat = CreateCharFormat();

        static StringFormat CreateCharFormat()
        {
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Far;
            format.LineAlignment = StringAlignment.Center;
            return format;
        }

        public OcrMatchEntry(OcrEntry entry, float dpi, Brush accentBrush, Color glyphColor)
        {
            this.Entry = entry;
            float scale = DpiHelper.GetScaleFactor(dpi);
            Color backColor = Color.Transparent;
            using(Bitmap original = entry.CreateBlockBitmap(glyphColor, backColor, 100, 20))
            {
                int scaledWidth = (int)Math.Round(original.Width * scale);
                int scaledHeight = (int)Math.Round(original.Height * scale);
                this.Image = new Bitmap(scaledWidth, scaledHeight);
                this.Image.SetResolution(dpi, dpi);
                using(Graphics g = Graphics.FromImage(this.Image))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    g.DrawImage(original, 0, 0, scaledWidth, scaledHeight);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    string fontFamily = "♪♥".IndexOf(entry.OcrCharacter.Value) >= 0 ?
                        "Segoe UI Symbol" : "Tahoma";
                    using(Font font = new Font(fontFamily, 18.0f,
                        entry.OcrCharacter.Italic ? FontStyle.Italic : FontStyle.Regular))
                    {
                        g.DrawString(new string(entry.OcrCharacter.Value, 1),
                            font, accentBrush,
                            new RectangleF(0, 0, scaledWidth, scaledHeight),
                            CharFormat);
                    }
                }
            }
        }

        public OcrEntry Entry { get; private set; }
        public Bitmap Image { get; private set; }

        public void Dispose()
        {
            if(this.Image != null)
            {
                this.Image.Dispose();
                this.Image = null;
            }
        }
    }

    class SplitEntry : IDisposable
    {
        static Bitmap ScaleBitmap(Bitmap original, float scale)
        {
            int scaledWidth = (int)Math.Round(original.Width * scale);
            int scaledHeight = (int)Math.Round(original.Height * scale);
            Bitmap scaled = new Bitmap(scaledWidth, scaledHeight);
            using(Graphics g = Graphics.FromImage(scaled))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(original, 0, 0, scaledWidth, scaledHeight);
            }
            return scaled;
        }

        public SplitEntry(string fullEncode, OcrSplit split1, OcrSplit split2, float dpi, Color glyphColor, bool isDark)
        {
            this.FullEncode = fullEncode;
            float scale = DpiHelper.GetScaleFactor(dpi);
            BlockEncode block = new BlockEncode(fullEncode);
            using(Bitmap orig = block.CreateBlockBitmap(glyphColor, Color.Transparent, 20, 20))
            {
                this.OriginalImage = ScaleBitmap(orig, scale);
            }
            BlockEncode block1 = new BlockEncode(split1.FullEncode);
            Color split1Color = isDark ? Color.LightSkyBlue : Color.DodgerBlue;
            using(Bitmap orig1 = block1.CreateBlockBitmap(split1Color, Color.Transparent, 20, 20))
            {
                this.Split1Image = ScaleBitmap(orig1, scale);
            }
            BlockEncode block2 = new BlockEncode(split2.FullEncode);
            Color split2Color = isDark ? Color.LightGreen : Color.DarkGreen;
            using(Bitmap orig2 = block2.CreateBlockBitmap(split2Color, Color.Transparent, 20, 20))
            {
                this.Split2Image = ScaleBitmap(orig2, scale);
            }
        }

        public string FullEncode { get; private set; }
        public Bitmap OriginalImage { get; private set; }
        public Bitmap Split1Image { get; private set; }
        public Bitmap Split2Image { get; private set; }

        public void Dispose()
        {
            if(this.OriginalImage != null)
            {
                this.OriginalImage.Dispose();
                this.OriginalImage = null;
            }
            if(this.Split1Image != null)
            {
                this.Split1Image.Dispose();
                this.Split1Image = null;
            }
            if(this.Split2Image != null)
            {
                this.Split2Image.Dispose();
                this.Split2Image = null;
            }
        }
    }

    private void splitListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if((this.splitListBox.Items.Count != 0) && (this.splitListBox.SelectedIndex >= 0))
        {
            SplitEntry entry = this.splitListBox.Items[this.splitListBox.SelectedIndex] as SplitEntry;
            this.split1PictureBox.Image = entry.Split1Image;
            this.split2PictureBox.Image = entry.Split2Image;
        }
        else
        {
            this.split1PictureBox.Image = null;
            this.split2PictureBox.Image = null;
        }
    }

    private void splitListBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
        if((this.splitListBox.Items.Count != 0) && (e.Index >= 0))
        {
            SplitEntry entry = this.splitListBox.Items[e.Index] as SplitEntry;
            e.ItemWidth = entry.OriginalImage.Width;
            e.ItemHeight = entry.OriginalImage.Height;
        }
    }

    private void splitListBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        if((this.splitListBox.Items.Count != 0) && (e.Index >= 0))
        {
            if((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(this.highlightBrush, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(this.backgroundBrush, e.Bounds);
            }
            SplitEntry entry = this.splitListBox.Items[e.Index] as SplitEntry;
            e.Graphics.DrawImage(entry.OriginalImage, e.Bounds.Location);
        }
    }

    private void removeSplitBbutton_Click(object sender, EventArgs e)
    {
        if((this.splitListBox.Items.Count != 0) && (this.splitListBox.SelectedIndex >= 0))
        {
            SplitEntry entry = this.splitListBox.Items[this.splitListBox.SelectedIndex] as SplitEntry;
            this.ocrMap.RemoveSplit(entry.FullEncode);
            this.splitListBox.Items.Remove(entry);
            entry.Dispose();
            this.ChangesMade = true;
            if(this.splitListBox.Items.Count != 0)
            {
                //this.splitListBox.SelectedIndex = 0;
            }
            else
            {
                this.split1PictureBox.Image = null;
                this.split2PictureBox.Image = null;
                this.removeSplitButton.Enabled = false;
            }
        }
    }

    private void doneButton_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        Close();
    }

    private void removeOcrButton_Click(object sender, EventArgs e)
    {
        if((this.ocrListBox.Items.Count != 0) && (this.ocrListBox.SelectedIndex >= 0))
        {
            OcrMatchEntry entry = this.ocrListBox.SelectedItem as OcrMatchEntry;
            this.ocrMap.RemoveMatch(entry.Entry);
            this.ocrListBox.Items.Remove(entry);
            entry.Dispose();
            this.ChangesMade = true;
            if(this.ocrListBox.Items.Count != 0)
            {
                //this.ocrListBox.SelectedIndex = 0;
            }
            else
            {
                this.removeOcrButton.Enabled = false;
            }
        }
    }

    private void removeCharacterButton_Click(object sender, EventArgs e)
    {
        if((this.ocrListBox.Items.Count != 0) && (this.ocrListBox.SelectedIndex >= 0))
        {
            OcrMatchEntry selectedEntry = this.ocrListBox.SelectedItem as OcrMatchEntry;
            List<OcrMatchEntry> charEntries = [];
            foreach(OcrMatchEntry charEntry in this.ocrListBox.Items)
            {
                if(charEntry.Entry.OcrCharacter == selectedEntry.Entry.OcrCharacter)
                {
                    charEntries.Add(charEntry);
                }
            }

            foreach(OcrMatchEntry charEntry in charEntries)
            {
                this.ocrMap.RemoveMatch(charEntry.Entry);
                this.ocrListBox.Items.Remove(charEntry);
                charEntry.Dispose();
            }

            this.ChangesMade = true;
            if(this.ocrListBox.Items.Count != 0)
            {
                //this.ocrListBox.SelectedIndex = 0;
            }
            else
            {
                this.removeOcrButton.Enabled = false;
            }
        }
    }

    private void ocrListBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
        if((this.ocrListBox.Items.Count != 0) && (e.Index >= 0))
        {
            OcrMatchEntry entry = this.ocrListBox.Items[e.Index] as OcrMatchEntry;
            e.ItemWidth = entry.Image.Width;
            e.ItemHeight = entry.Image.Height;
        }
    }

    private void ocrListBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        if((this.ocrListBox.Items.Count != 0) && (e.Index >= 0))
        {
            if((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(this.highlightBrush, e.Bounds);
            }
            else if((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                e.Graphics.FillRectangle(this.highlightBrush, e.Bounds);
                Rectangle rect = e.Bounds;
                rect.Width--;
                rect.Height--;
                e.Graphics.DrawRectangle(Pens.Gray, rect);
            }
            else
            {
                e.Graphics.FillRectangle(this.backgroundBrush, e.Bounds);
            }
            OcrMatchEntry entry = this.ocrListBox.Items[e.Index] as OcrMatchEntry;
            e.Graphics.DrawImage(entry.Image, e.Bounds.Location);
        }
    }

    private void removeAllTrainingsButton_Click(object sender, EventArgs e)
    {
        if(MessageBox.Show("Do you really want to remove ALL trainings used in OCRing this movie?", "Really?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        {
            List<OcrEntry> entriesMovie = new List<OcrEntry>(
                this.ocrMap.GetMatchesForMovie(this.movieId, true));
            foreach(OcrEntry oldEntry in entriesMovie)
            {
                this.ocrMap.RemoveMatch(oldEntry, this.movieId);
            }
            this.ocrListBox.Items.Clear();
            this.removeOcrButton.Enabled = false;
            this.removeCharacterButton.Enabled = false;
            this.removeAllTrainingsButton.Enabled = false;
            this.ChangesMade = true;
        }
    }

    private void removeAllSplitsButton_Click(object sender, EventArgs e)
    {
        if(MessageBox.Show("Do you really want to remove ALL splits used in OCRing this movie?", "Really?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        {
            List<string> splitsMovie = [];
            foreach(KeyValuePair<string, OcrMap.SplitMapEntry> split in this.ocrMap.Splits)
            {
                if(split.Value.MovieIds.Contains(this.movieId))
                {
                    splitsMovie.Add(split.Key);
                }
            }
            foreach(string fullEncode in splitsMovie)
            {
                this.ocrMap.RemoveSplit(fullEncode, this.movieId);
            }

            this.splitListBox.Items.Clear();
            this.removeSplitButton.Enabled = false;
            this.removeAllSplitsButton.Enabled = false;
            this.ChangesMade = true;
        }
    }
}

