using System.Drawing;
using TagCloud.Logger;
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
    public IEnumerable<WordLayoutInfo> GetWordCloudLayout(Func<string, Font, SizeF> stringMeasure)
    {
        var settings = settingsProvider.GetSettings();
        
        var words = statistics.GetWords().ToArray();
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            
            var frequency = statistics.GetWordFrequency(word);
            var fontSize = settings.MinFontSize + (settings.MaxFontSize - settings.MinFontSize) * frequency;
            var font = new Font(settings.Font, fontSize);
            var stringSize = stringMeasure(word, font);
            var renderSize = new Size(1 + (int)stringSize.Width, 1 + (int)stringSize.Height);
            
            var rectangle = cloudLayouter.PutNextRectangle(renderSize);
            yield return new WordLayoutInfo(word, font, rectangle);
            logger.ReportProgress($"Put {i + 1}/{words.Length} words", (double)i / (words.Length - 1));
        }
    }
#pragma warning restore CA1416

    public IWordStatistics WordStatistics => statistics;
}