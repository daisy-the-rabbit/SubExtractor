using DvdNavigatorCrm;

namespace DvdSubExtractor;

public class SubtitleTrackItem
{
    public SubtitleTrackItem(int track, SubpictureAttributes subtitle)
    {
        this.Attributes = subtitle;
        this.Track = track;
    }

    public SubpictureAttributes Attributes { get; private set; }
    public int Track { get; private set; }

    public override string ToString()
    {
        string text = $"{this.Track} {DvdLanguageCodes.GetLanguageText(this.Attributes.Language)}";
        switch (this.Attributes.CodeExtension)
        {
            case SubpictureCodeExtension.Captions:
                text += " Captions";
                break;
            case SubpictureCodeExtension.Directors:
            case SubpictureCodeExtension.DirectorsForChildren:
            case SubpictureCodeExtension.LargeDirectors:
                text += " Directors";
                break;
            case SubpictureCodeExtension.Forced:
                text += " Forced";
                break;
        }
        return text;
    }
}

