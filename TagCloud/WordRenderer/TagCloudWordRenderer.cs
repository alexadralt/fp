using System.Drawing;
using System.Drawing.Drawing2D;
using TagCloud.ResultUtils;
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
    public Result<Bitmap> Render()
    {
        return settingsProvider.GetImageSettings()
            .Then(Render);
    }

    private Result<Bitmap> Render(ImageSettings settings)
    {
        var imageSize = settings.ImageSize;
        var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
        
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(settings.BackgroundColor);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        
        var brush = new SolidBrush(settings.TextColor);

        var bitmapResult = Result.FromValue(bitmap);
        foreach (var wordLayoutInfoResult in wordCloudLayouter.GetWordCloudLayout(
                     (word, font) => graphics.MeasureString(word, font)))
        {
            bitmapResult = wordLayoutInfoResult.Then(info =>
            {
                graphics.DrawString(info.Word, info.Font, brush, info.Rectangle);
                return bitmap;
            });
        }

        return bitmapResult;
    }
#pragma warning restore CA1416

    public IWordStatistics WordStatistics => wordCloudLayouter.WordStatistics;
}