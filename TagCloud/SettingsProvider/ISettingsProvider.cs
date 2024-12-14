namespace TagCloud.SettingsProvider;

public interface ISettingsProvider
{
    public Settings GetSettings();
    public void UpdateSettings(Settings settings);
}