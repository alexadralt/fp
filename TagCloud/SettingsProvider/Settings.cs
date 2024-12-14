using System.Drawing;

namespace TagCloud.SettingsProvider;

public record Settings
{
    public Color TextColor { get; init; }
    public Color BackgroundColor { get; init; }
    public FontFamily Font { get; init; }
    public int MinFontSize { get; init; }
    public int MaxFontSize { get; init; }
    public Size ImageSize { get; init; }
    public float TracingStep { get; init; }
    public double AngleStep { get; init; }
    public float Density { get; init; }
    public Point CloudCenter { get; init; }
    
    public static readonly Settings DefaultSettings = new Settings
    {
        BackgroundColor = Color.White,
#pragma warning disable CA1416
        Font = FontFamily.GenericMonospace,
#pragma warning restore CA1416
        ImageSize = new Size(700, 700),
        MaxFontSize = 300,
        MinFontSize = 8,
        TextColor = Color.Black,
        TracingStep = 0.001f,
        AngleStep = Math.PI / 32,
        Density = 0.1f,
        CloudCenter = new Point(350, 350)
    };

    public static readonly Settings TestSettings = new Settings
    {
        BackgroundColor = Color.White,
#pragma warning disable CA1416
        Font = FontFamily.GenericMonospace,
#pragma warning restore CA1416
        ImageSize = new Size(1000, 1000),
        MaxFontSize = 300,
        MinFontSize = 8,
        TextColor = Color.Black,
        TracingStep = 0.001f,
        AngleStep = Math.PI / 32,
        Density = 0.1f,
        CloudCenter = new Point(500, 500)
    };
}