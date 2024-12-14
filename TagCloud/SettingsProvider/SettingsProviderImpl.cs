using System.Drawing;
using System.Text.Json;
using TagCloud.json;
using TagCloud.Logger;

namespace TagCloud.SettingsProvider;

public class SettingsProviderImpl(ILogger logger) : ISettingsProvider
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        Converters =
        {
            new JsonColorConverter(),
            new JsonFontFamilyConverter(),
            new JsonSizeConverter(),
            new JsonPointConverter()
        },
        WriteIndented = true
    };
    private static readonly string _settingsFile = "settings.json";

    private Settings? _settings;

    public Settings GetSettings()
    {
        if (_settings != null)
            return _settings;

        try
        {
            LoadSettings();
        }
        catch (Exception e)
        {
            _settings = Settings.DefaultSettings;
            
            logger.Warning($"Failed to load settings file: {e.Message}");
            logger.Warning("Using default settings.");
            
            if (!Path.Exists(_settingsFile))
            {
                SaveSettings();
                logger.Warning($"Created settings.json file at {Path.GetFullPath(_settingsFile)}");
            }
        }
        
        return _settings;
    }

    public void UpdateSettings(Settings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (settings != _settings)
        {
            _settings = settings;
            SaveSettings();
        }
    }

    private void LoadSettings()
    {
        var json = File.ReadAllText(_settingsFile);
        _settings = JsonSerializer.Deserialize<Settings>(json, _options);
        
        if (_settings == null)
            throw new JsonException("Could not parse settings file.");
        if (!_settings.TextColor.IsKnownColor)
            throw new JsonException("Unknown TextColor value");
        if (!_settings.BackgroundColor.IsKnownColor)
            throw new JsonException("Unknown BackgroundColor value");
        if (_settings.Font == null)
        {
            var names = FontFamily.Families.Select(family => family.Name).ToArray();
            throw new JsonException($"Unknown Font value. Available options are:\n {string.Join(", ", names)}");
        }
        if (_settings.MinFontSize <= 0)
            throw new JsonException("MinFontSize must be greater than zero");
        if (_settings.MaxFontSize <= 0)
            throw new JsonException("MaxFontSize must be greater than zero");
        if (_settings.ImageSize == Size.Empty)
            throw new JsonException("ImageSize contains incorrect values");
        if (_settings.CloudCenter == Point.Empty)
            throw new JsonException("CloudCenter contains incorrect values");
        if (_settings.AngleStep <= 0)
            throw new JsonException("AngleStep must be greater than zero");
        if (_settings.TracingStep <= 0)
            throw new JsonException("TracingStep must be greater than zero");
    }

    private void SaveSettings()
    {
        var json = JsonSerializer.Serialize(_settings, _options);
        using var writer = new StreamWriter(_settingsFile);
        writer.Write(json);
    }
}