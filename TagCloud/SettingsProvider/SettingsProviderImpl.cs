using System.Drawing;
using System.Text.Json;
using TagCloud.json;
using TagCloud.Logger;
using TagCloud.ResultUtils;

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

        var settingsResult = LoadSettings();
        if (settingsResult.Success)
            _settings = settingsResult.Value!;
        else
        {
            _settings = Settings.DefaultSettings;
            
            logger.Warning($"Failed to load settings file: {settingsResult.Error}");
            logger.Warning("Using default settings.");
            
            if (!Path.Exists(_settingsFile))
            {
                var saveSettingsResult = SaveSettings();
                if (saveSettingsResult.Success)
                    logger.Info($"Created settings.json file at {Path.GetFullPath(_settingsFile)}");
                else
                    logger.Error($"Could not create settings.json file at {Path.GetFullPath(_settingsFile)}, " +
                                 $"because: {saveSettingsResult.Error}");
            }
        }
        
        return _settings;
    }

    public Result<Nothing> UpdateSettings(Settings settings)
    {
        if (_settings == null)
            return Result.Failure("Settings object is null");
        if (settings != _settings)
        {
            _settings = settings;
            return SaveSettings();
        }
        
        return Result.Success();
    }

    private Result<Settings> LoadSettings()
    {
        Settings? settings;
        try
        {
            var json = File.ReadAllText(_settingsFile);
            settings = JsonSerializer.Deserialize<Settings>(json, _options);
        }
        catch (Exception e)
        {
            return Result.FromError<Settings>(e.Message);
        }
        
        if (settings == null)
            return Result.FromError<Settings>("Could not parse settings file.");
        if (!settings.TextColor.IsKnownColor)
            return Result.FromError<Settings>("Unknown TextColor value");
        if (!settings.BackgroundColor.IsKnownColor)
            return Result.FromError<Settings>("Unknown BackgroundColor value");
        if (settings.Font == null)
        {
            var names = FontFamily.Families.Select(family => family.Name).ToArray();
            return Result.FromError<Settings>(
                $"Unknown Font value. Available options are:\n{string.Join(", ", names)}");
        }
        if (settings.MinFontSize <= 0)
            return Result.FromError<Settings>("MinFontSize must be greater than zero");
        if (settings.MaxFontSize <= 0)
            return Result.FromError<Settings>("MaxFontSize must be greater than zero");
        if (settings.ImageSize == Size.Empty)
            return Result.FromError<Settings>("ImageSize contains incorrect values");
        if (settings.CloudCenter == Point.Empty)
            return Result.FromError<Settings>("CloudCenter contains incorrect values");
        if (settings.AngleStep <= 0)
            return Result.FromError<Settings>("AngleStep must be greater than zero");
        if (settings.TracingStep <= 0)
            return Result.FromError<Settings>("TracingStep must be greater than zero");

        return Result.FromValue(settings);
    }

    private Result<Nothing> SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, _options);
            using var writer = new StreamWriter(_settingsFile);
            writer.Write(json);
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
        
        return Result.Success();
    }
}