namespace DvdSubOcr;

public record EncodeMatch(OcrEntry OcrEntry, IList<int> ExtraBlocks)
{
    public IList<int> ExtraBlocks { get; init; } = new List<int>(ExtraBlocks);
}

