using System.Drawing;
using System.Drawing.Drawing2D;
using TagCloud.SettingsProvider;
using TagCloud.WordCloudLayouter;
using TagCloud.WordStatistics;

namespace TagCloud.WordRenderer;

public class TagCloudWordRenderer(
    IWordCloudLayouter wordCloudLayouter,
    ISettingsProvider settingsProvider
    ) : IWordRenderer
{
#pragma warning disable CA1416
    public Bitmap Render()
    {
        var settings = settingsProvider.GetSettings();
        
        var imageSize = settings.ImageSize;
        var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(settings.BackgroundColor);
        
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        var brush = new SolidBrush(settings.TextColor);

        foreach (var wordLayoutInfo in wordCloudLayouter.GetWordCloudLayout(
                     (word, font) => graphics.MeasureString(word, font)))
            graphics.DrawString(wordLayoutInfo.Word, wordLayoutInfo.Font,
                brush, wordLayoutInfo.Rectangle);
        
        return bitmap;
    }
#pragma warning restore CA1416

    public IWordStatistics WordStatistics => wordCloudLayouter.WordStatistics;
}