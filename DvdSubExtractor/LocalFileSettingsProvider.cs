using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace DvdSubExtractor;

/// <summary>
/// A custom settings provider that stores user settings in a fixed location
/// (%LocalAppData%\DvdSubExtractor\user.config) instead of the default
/// version-specific path.
/// </summary>
public class LocalFileSettingsProvider : SettingsProvider
{
    private const string SettingsRootElement = "Settings";
    private readonly string settingsFilePath;

    public LocalFileSettingsProvider()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string settingsFolder = Path.Combine(appDataPath, "DvdSubExtractor");
        this.settingsFilePath = Path.Combine(settingsFolder, "user.config");
    }

    public override string ApplicationName
    {
        get => "DvdSubExtractor";
        set { }
    }

    public override string Name => "LocalFileSettingsProvider";

    public override void Initialize(string name, NameValueCollection config)
    {
        base.Initialize(Name, config);
    }

    public override SettingsPropertyValueCollection GetPropertyValues(
        SettingsContext context, SettingsPropertyCollection properties)
    {
        var values = new SettingsPropertyValueCollection();
        var storedValues = LoadSettings();

        foreach (SettingsProperty property in properties)
        {
            var value = new SettingsPropertyValue(property);

            if (storedValues.TryGetValue(property.Name, out string storedValue))
            {
                value.SerializedValue = storedValue;
            }
            else
            {
                value.SerializedValue = property.DefaultValue;
            }

            value.IsDirty = false;
            values.Add(value);
        }

        return values;
    }

    public override void SetPropertyValues(
        SettingsContext context, SettingsPropertyValueCollection properties)
    {
        var storedValues = new Dictionary<string, string>();

        foreach (SettingsPropertyValue property in properties)
        {
            if (property.SerializedValue != null)
            {
                storedValues[property.Name] = property.SerializedValue.ToString();
            }
        }

        SaveSettings(storedValues);
    }

    private Dictionary<string, string> LoadSettings()
    {
        var settings = new Dictionary<string, string>();

        if (!File.Exists(this.settingsFilePath))
        {
            return settings;
        }

        try
        {
            var doc = XDocument.Load(this.settingsFilePath);
            var root = doc.Element(SettingsRootElement);
            if (root != null)
            {
                foreach (var element in root.Elements())
                {
                    settings[element.Name.LocalName] = element.Value;
                }
            }
        }
        catch (Exception)
        {
            // If the file is corrupt, return empty settings and let defaults apply
        }

        return settings;
    }

    private void SaveSettings(Dictionary<string, string> settings)
    {
        try
        {
            string directory = Path.GetDirectoryName(this.settingsFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var root = new XElement(SettingsRootElement);
            foreach (var kvp in settings)
            {
                root.Add(new XElement(kvp.Key, kvp.Value));
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            doc.Save(this.settingsFilePath);
        }
        catch (Exception)
        {
            // Silently fail if we can't save settings
        }
    }
}
