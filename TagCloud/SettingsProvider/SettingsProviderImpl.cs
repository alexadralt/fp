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

    public Result<FontSettings> GetFontSettings()
    {
        return GetSettings()
            .Then(settings => settings.Font);
    }

    public Result<ImageSettings> GetImageSettings()
    {
        return GetSettings()
            .Then(settings => settings.Image);
    }

    public Result<AlgorithmSettings> GetAlgorithmSettings()
    {
        return GetSettings()
            .Then(settings => settings.Algorithm);
    }

    private Result<Settings> GetSettings()
    {
        if (_settings != null)
            return Result.FromValue(_settings);

        if (!File.Exists(_settingsFile))
        {
            _settings = Settings.DefaultSettings;
            return SaveSettings()
                .ChangeError(err => $"Could not create settings.json file at {Path.GetFullPath(_settingsFile)}, " +
                                    $"because: {err}")
                .Then(_ => logger.Info($"Created settings.json file at {Path.GetFullPath(_settingsFile)}"))
                .Then(_ => _settings);
        }

        return LoadSettings()
            .ChangeError(err => $"Failed to load settings file:\n{err}")
            .Then(settings => _settings = settings);
    }

    private Result<Settings> LoadSettings()
    {
        return Result.FromValue(_settingsFile)
            .Try(file =>
            {
                var json = File.ReadAllText(file);
                return JsonSerializer.Deserialize<Settings>(json, _options)!;
            })
            .Validate(settings => settings != null, "Could not parse settings file.")
            .Validate(settings => settings!.Image.TextColor.IsKnownColor,
                "Unknown TextColor value. Known values are:\n"
                + string.Join(", ", GetKnownColors()))
            .Validate(settings => settings!.Image.BackgroundColor.IsKnownColor,
                "Unknown BackgroundColor value. Known values are:\n"
                + string.Join(", ", GetKnownColors()))
            .Validate(settings => settings!.Font.Font != null,
                "Unknown Font value. Available options are:\n" +
                string.Join(", ", FontFamily.Families.Select(family => family.Name).ToArray()))
            .Validate(settings => settings!.Font.MinFontSize > 0, "MinFontSize must be greater than zero")
            .Validate(settings => settings!.Font.MaxFontSize > 0, "MaxFontSize must be greater than zero")
            .Validate(settings => settings!.Image.ImageSize.Width > 0, "Image width must be greater than zero")
            .Validate(settings => settings!.Image.ImageSize.Height > 0, "Image height must be greater than zero")
            .Validate(settings => settings!.Algorithm.CloudCenter.Y > 0,
                "CloudCenter Y must be greater than zero")
            .Validate(settings => settings!.Algorithm.CloudCenter.X > 0,
                "CloudCenter X must be greater than zero")
            .Validate(settings => settings!.Algorithm.AngleStep > 0, "AngleStep must be greater than zero")
            .Validate(settings => settings!.Algorithm.TracingStep > 0, "TracingStep must be greater than zero")
            .Validate(settings => settings!.Algorithm.TracingStep < 1, "TracingStep must be lees than 1")
            .Validate(settings => settings!.Algorithm.Density > 0, "Density must be greater than zero")
            .Validate(settings => settings!.Algorithm.Density < 1, "Density must be less than one");
    }

    private IEnumerable<string> GetKnownColors()
    {
        return Enum.GetValuesAsUnderlyingType<KnownColor>()
            .Cast<KnownColor>().Select(color => Color.FromKnownColor(color).Name);
    }

    private Result<Nothing> SaveSettings()
    {
        return Result.Success()
            .Try(() =>
            {
                var json = JsonSerializer.Serialize(_settings, _options);
                using var writer = new StreamWriter(_settingsFile);
                writer.Write(json);
            });
    }
}