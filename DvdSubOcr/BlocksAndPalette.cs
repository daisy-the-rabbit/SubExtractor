namespace DvdSubOcr;
public class BlocksAndPalette
{
    public BlocksAndPalette(IEnumerable<BlockEncode> blocks,
        IEnumerable<int> indexes, IEnumerable<string> splitEncodes, 
        int averagePixels, int countedBlocks)
    {
        this.Blocks = new List<BlockEncode>(blocks);
        this.ColorIndexes = new List<int>(indexes);
        this.SplitEncodes = new List<string>(splitEncodes);
        this.AveragePixelCount = averagePixels;
        this.CountedBlocks = countedBlocks;
        this.WrapsAround = [];
        this.WrapsAroundGently = [];
        this.WrapsAndShares = [];
        this.InterestingMatchBlocks = [];
    }

    public IList<BlockEncode> Blocks { get; private set; }
    public IList<int> ColorIndexes { get; private set; }
    public IList<string> SplitEncodes { get; private set; }
    public int AveragePixelCount { get; private set; }
    public int CountedBlocks { get; private set; }
    public int InterestingMatches { get; set; }
    public IList<BlocksAndPalette> WrapsAround { get; private set; }
    public IList<BlocksAndPalette> WrapsAroundGently { get; private set; }
    public IList<BlocksAndPalette> WrapsAndShares { get; private set; }
    public IList<BlockEncode> InterestingMatchBlocks { get; private set; }
}

