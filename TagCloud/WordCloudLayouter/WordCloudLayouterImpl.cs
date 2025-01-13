using System.Drawing;
using TagCloud.Logger;
using TagCloud.ResultUtils;
using TagCloud.SettingsProvider;
using TagCloud.TagsCloudVisualization;
using TagCloud.WordStatistics;

namespace TagCloud.WordCloudLayouter;

public class WordCloudLayouterImpl(
    ICircularCloudLayouter cloudLayouter,
    IWordStatistics statistics,
    ILogger logger,
    ISettingsProvider settingsProvider)
    : IWordCloudLayouter
{
#pragma warning disable CA1416
    public IEnumerable<Result<WordLayoutInfo>> GetWordCloudLayout(Func<string, Font, SizeF> stringMeasure)
    {
        var settingsResult = settingsProvider.GetFontSettings();
        if (!settingsResult.Success)
        {
            yield return Result.FromError<WordLayoutInfo>(settingsResult.Error!);
            yield break;
        }

        var settings = settingsResult.Value!;
        var words = statistics.GetWords().ToArray();
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            
            var frequency = statistics.GetWordFrequency(word);
            var fontSize = settings.MinFontSize + (settings.MaxFontSize - settings.MinFontSize) * frequency;
            var font = new Font(settings.Font!, fontSize);
            var stringSize = stringMeasure(word, font);
            var renderSize = new Size(1 + (int)stringSize.Width, 1 + (int)stringSize.Height);
            
            var rectangle = cloudLayouter.PutNextRectangle(renderSize);
            if (rectangle.Success)
            {
                logger.ReportProgress($"Put {i + 1}/{words.Length} words", (double)i / (words.Length - 1));
                yield return Result.FromValue(new WordLayoutInfo(word, font, rectangle.Value!));
            }
            else
            {
                yield return Result.FromError<WordLayoutInfo>(rectangle.Error!);
                break;
            }
        }
    }
#pragma warning restore CA1416

    public IWordStatistics WordStatistics => statistics;
}