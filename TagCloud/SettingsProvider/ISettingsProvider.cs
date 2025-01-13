using TagCloud.ResultUtils;

namespace TagCloud.SettingsProvider;

public interface ISettingsProvider
{
    public Result<FontSettings> GetFontSettings();
    public Result<ImageSettings> GetImageSettings();
    public Result<AlgorithmSettings> GetAlgorithmSettings();
}