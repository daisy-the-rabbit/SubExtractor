namespace DvdSubExtractor;

public record CreateSubOptions
{
    public string FileName { get; init; }
    public string OutputDirectory { get; init; }
    public Point Crop { get; init; }
    public double OverallPtsAdjustment { get; init; }
    public bool Adjust25to24 { get; init; }
    public bool Is1080p { get; init; }
    public LineBreaksAndPositions PositionAllSubs { get; init; }
    public RemoveSDH RemoveSDH { get; init; }
}

