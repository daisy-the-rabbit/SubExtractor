using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using DvdSubOcr;

namespace DvdSubExtractor;
static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Application.ThreadException += Application_ThreadException;
        HighDpiMode dpiMode = Properties.Settings.Default.DpiScale switch
        {
            "System" => HighDpiMode.SystemAware,
            "Unaware" => HighDpiMode.DpiUnaware,
            _ => HighDpiMode.PerMonitorV2,
        };
        Application.SetHighDpiMode(dpiMode);
        ThemeManager.ApplyTheme(null, Properties.Settings.Default.Theme);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try
        {
            Properties.Settings.Default.LargeMode = false;
            Properties.Settings.Default.Save();

            if(!Directory.Exists(Properties.Settings.Default.OutputDirectory))
            {
                Properties.Settings.Default.OutputDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            }

            OcrMap.UseProgramExeForStorage = Properties.Settings.Default.DataFileInExeDirectory;
            OcrMap.UseSpanishSpecialChars = Properties.Settings.Default.SpanishSpecialCharacters;
            SubtitleLine.CharactersNeverAfterASpace = Properties.Settings.Default.FrenchSpecialCharacters ?
                SubConstants.CharactersNeverAfterASpaceFrench : SubConstants.CharactersNeverAfterASpace;
            if(!string.IsNullOrWhiteSpace(Properties.Settings.Default.SavedKerningValues))
            {
                FontKerning.KerningDiffList = Properties.Settings.Default.SavedKerningValues;
            }

            if(!File.Exists(OcrMap.StorageFile))
            {
                string trainingDbName = OcrMap.DatabaseOriginalName + OcrMap.DatabaseExtension;
                string trainingOcrMap = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath),
                    trainingDbName);
                if(File.Exists(trainingOcrMap))
                {
                    try
                    {
                        File.Copy(trainingOcrMap, OcrMap.StorageFile);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Unable to copy " + trainingDbName + " data file to data directory");
                    }
                }
            }

            SubWizard subWiz;
            if((args.Length != 0) && File.Exists(args[0]))
            {
                int streamId;
                if((args.Length > 1) && Int32.TryParse(args[1], System.Globalization.NumberStyles.HexNumber, null, out streamId))
                {
                    subWiz = new SubWizard(args[0], streamId);
                }
                else
                {
                    subWiz = new SubWizard(args[0]);
                }
            }
            else
            {
                subWiz = new SubWizard();
            }

            Application.Run(subWiz);
        }
        catch(Exception ex)
        {
            LogException(ex);
            throw;
        }
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if(e.ExceptionObject is Exception ex)
        {
            LogException(ex);
        }
    }

    static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        LogException(e.Exception);
    }

    static Exception lastException;

    static void LogException(Exception ex)
    {
        if(object.ReferenceEquals(lastException, ex))
        {
            return;
        }
        lastException = ex;

        try
        {
            string directory = Properties.Settings.Default.OutputDirectory;
            if(!Directory.Exists(directory))
            {
                directory = System.Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);
            }
            string logFilePath = Path.Combine(directory, "ExceptionLog.txt");
            using(StreamWriter writer = new StreamWriter(logFilePath, true, Encoding.UTF8))
            {
                writer.WriteLine($"Exception thrown at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
                writer.WriteLine(ex.Message);
                writer.WriteLine(ex.Source);
                writer.Write(ex.StackTrace);
                writer.WriteLine();
                if(ex.InnerException != null)
                {
                    writer.WriteLine("Inner Exception");
                    writer.WriteLine("\t" + ex.InnerException.Message);
                    writer.WriteLine("\t" + ex.InnerException.Source);
                    writer.Write("\t" + ex.InnerException.StackTrace);
                    writer.WriteLine();
                }
                writer.Close();
            }
        }
        catch(Exception)
        {
        }
    }
}

