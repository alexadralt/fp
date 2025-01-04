using TagCloud.ResultUtils;

namespace TagCloud.SettingsProvider;

public interface ISettingsProvider
{
    public Settings GetSettings();
    public Result<Nothing> UpdateSettings(Settings settings);
}