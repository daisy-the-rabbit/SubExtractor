using System.IO;
using DvdNavigatorCrm;
using DvdSubOcr;

namespace DvdSubExtractor;
public class ExtractData
{
    const float PlaybackTimeDifferential = 0.8f;

    List<DvdTrackItem> programs = [];
    bool isCurrentStepComplete;
    bool isPreviousStepComplete;
    string helpText = "";
    IEnumerable<Type> jumpToSteps = [];

    public ExtractData()
    {
    }

    public IList<DvdTrackItem> Programs { get { return this.programs; } }
    public string DvdFolder { get; private set; }
    public string DvdName { get; private set; }
    public IList<string> SelectedSubtitleFiles { get; set; }

    public string SelectedSubtitleBinaryFile { get; set; }
    public int SelectedSubtitleStreamId { get; set; }
    public string SelectedSubtitleDescription { get; set; }
    public OcrWorkingData WorkingData { get; set; }

    public bool IsHighDef
    {
        get
        {
            if(!String.IsNullOrEmpty(this.SelectedSubtitleBinaryFile))
            {
                string binaryFileExt = Path.GetExtension(this.SelectedSubtitleBinaryFile).ToLowerInvariant();
                if(binaryFileExt == ".sup")
                {
                    return true;
                }
            }
            if((this.WorkingData != null) && (this.WorkingData.VideoAttributes.HorizontalResolution > 1400))
            {
                return true;
            }
            return false;
        }
    }

    public event EventHandler OptionsUpdated;

    public void OnOptionsUpdated(object sender)
    {
        OptionsUpdated?.Invoke(sender, EventArgs.Empty);
    }

    public void NewStepInitialize(bool currentComplete, bool previousComplete, string stepHelpText, IEnumerable<Type> jumpableSteps)
    {
        this.IsCurrentStepComplete = currentComplete;
        this.IsPreviousStepComplete = previousComplete;
        this.HelpText = stepHelpText;
        this.JumpToSteps = jumpableSteps;
    }

    public event EventHandler IsCurrentStepCompleteUpdated;

    public bool IsCurrentStepComplete 
    {
        get { return this.isCurrentStepComplete; }
        set
        {
            this.isCurrentStepComplete = value;
            IsCurrentStepCompleteUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler IsPreviousStepCompleteUpdated;

    public bool IsPreviousStepComplete
    {
        get { return this.isPreviousStepComplete; }
        set
        {
            this.isPreviousStepComplete = value;
            IsPreviousStepCompleteUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler HelpTextUpdated;

    public string HelpText 
    {
        get { return this.helpText; }
        set
        {
            this.helpText = value;
            HelpTextUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler JumpToStepsUpdated;

    public IEnumerable<Type> JumpToSteps
    {
        get { return this.jumpToSteps; }
        set
        {
            this.jumpToSteps = value;
            JumpToStepsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler<TypeEventArgs> JumpTo;

    public void OnJumpTo(object sender, Type typeOfWizard)
    {
        this.JumpTo?.Invoke(sender, new TypeEventArgs(typeOfWizard));
    }

    public void LoadDvdPrograms(string dvdFolder)
    {
        this.DvdFolder = Path.GetFullPath(dvdFolder);
        if(dvdFolder.Length <= 3)
        {
            DriveInfo drive = new DriveInfo(dvdFolder.Substring(0, 1));
            this.DvdName = drive.VolumeLabel;
        }
        else
        {
            this.DvdName = Path.GetFileName(this.DvdFolder);
        }
        this.programs.Clear();

        string dvdPath = this.DvdFolder;
        if(!Directory.Exists(dvdPath))
        {
            return;
        }

        try
        {
            string[] trackIfos = Directory.GetFiles(dvdPath, "*_0.ifo");
            if((trackIfos.Length == 0) && Directory.Exists(Path.Combine(dvdPath, "VIDEO_TS")))
            {
                dvdPath = Path.Combine(dvdPath, "VIDEO_TS");
                trackIfos = Directory.GetFiles(dvdPath, "*_0.ifo");
            }

            foreach(string ifoPath in trackIfos)
            {
                DvdTitleSet titleSet = new DvdTitleSet(ifoPath);
                if(titleSet.IsValidTitleSet)
                {
                    titleSet.Parse();

                    for(int titleIndex = 0; titleIndex < titleSet.Titles.Count; titleIndex++)
                    {
                        DvdTitle title = titleSet.Titles[titleIndex];
                        if(title.PlaybackTime >= Properties.Settings.Default.MinimumDvdTrackLength)
                        {
                            this.programs.Add(new DvdTrackItem(titleSet, titleIndex));
                        }
                    }
                }
            }
        }
        catch(IOException ex)
        {
            MessageBox.Show("IOException: " + ex.Message);
        }
        finally
        {
            if(this.programs.Count != 0)
            {
                this.programs.Sort();
                this.programs.Reverse();
                // sort groups of tracks that are within 20% of each other's playback time
                // by track order instead of time
                for(int index = 0; index < programs.Count - 1; index++)
                {
                    int endNameSort = index;
                    while((endNameSort + 1 < programs.Count) &&
                        (this.programs[index].PlaybackTime * PlaybackTimeDifferential <
                            this.programs[endNameSort + 1].PlaybackTime))
                    {
                        endNameSort++;
                    }
                    if(endNameSort != index)
                    {
                        this.programs.Sort(index, endNameSort - index + 1, new DvdTrackItem.SortByName());
                        index = endNameSort;
                    }
                }
                for(int index = 0; index < programs.Count; index++)
                {
                    this.programs[index].ProgramNumber = index + 1;
                }
            }
        }
    }

    public string ComputeMpegFileName(DvdTrackItem item)
    {
        if(item.Angle == 0)
        {
            return $"{this.DvdName} Track {item.ProgramNumber}.mpg";
        }
        else
        {
            return $"{this.DvdName} Track {item.ProgramNumber} Angle {item.Angle}.mpg";
        }
    }

    public string ComputeSubtitleDataFileName(DvdTrackItem item)
    {
        if(item.Angle == 0)
        {
            return $"{this.DvdName} Track {item.ProgramNumber}.bin";
        }
        else
        {
            return $"{this.DvdName} Track {item.ProgramNumber} Angle {item.Angle}.bin";
        }
    }

    public string ComputeD2vFileName(DvdTrackItem item)
    {
        if(item.Angle == 0)
        {
            return $"{this.DvdName} Track {item.ProgramNumber}.d2v";
        }
        else
        {
            return $"{this.DvdName} Track {item.ProgramNumber} Angle {item.Angle}.d2v";
        }
    }
}

