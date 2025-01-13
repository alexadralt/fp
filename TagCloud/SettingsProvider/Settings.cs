using System.Drawing;

namespace TagCloud.SettingsProvider;

public record FontSettings(FontFamily? Font, int MinFontSize, int MaxFontSize);
public record ImageSettings(Size ImageSize, Color TextColor, Color BackgroundColor);
public record AlgorithmSettings(float TracingStep, double AngleStep, float Density, Point CloudCenter);

public record Settings(FontSettings Font, ImageSettings Image, AlgorithmSettings Algorithm)
{
    public static readonly Settings DefaultSettings = new Settings(
        new FontSettings(FontFamily.GenericMonospace, 8, 300),
        new ImageSettings(new Size(700, 700), Color.Black, Color.White),
        new AlgorithmSettings(0.001f, Math.PI / 32, 0.1f, new Point(350, 350)));

    public static readonly Settings TestSettings = new Settings(
        new FontSettings(FontFamily.GenericMonospace, 8, 300),
        new ImageSettings(new Size(1000, 1000), Color.Black, Color.White),
        new AlgorithmSettings(0.001f, Math.PI / 32, 0.1f, new Point(500, 500)));
}