using System;
using System.Drawing;
using System.Linq;
using System.Text;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TagCloud.Logger;
using TagCloud.SettingsProvider;
using TagCloud.TagsCloudVisualization;
using TagCloud.WordCloudLayouter;
using TagCloud.WordStatistics;

namespace WordCloudLayouter.Tests.WordCloudLayouter;

[TestFixture]
[TestOf(typeof(WordCloudLayouterImpl))]
public class WordCloudLayouterImplTests
{
    private IWordCloudLayouter _wordCloudLayouter;
    private ICircularCloudLayouter _circularCloudLayouter;
    private IWordStatistics _wordStatistics;
    private ILogger _logger;
    private ISettingsProvider _settingsProvider;
    private Func<string, Font, SizeF> _defaultStringMeasurer;
    
    [SetUp]
    public void SetUp()
    {
        _circularCloudLayouter = A.Fake<ICircularCloudLayouter>();
        _wordStatistics = A.Fake<IWordStatistics>();
        _logger = A.Fake<ILogger>();
        _settingsProvider = A.Fake<ISettingsProvider>();
        _defaultStringMeasurer = (_, _) => SizeF.Empty;

        A.CallTo(() => _settingsProvider.GetSettings())
            .Returns(Settings.TestSettings);
        
        _wordCloudLayouter = new WordCloudLayouterImpl(
            _circularCloudLayouter, _wordStatistics, _logger, _settingsProvider);
    }

    [Test]
    public void GetWordCloudLayouter_CallsGetSettingsOnce()
    {
        A.CallTo(() => _wordStatistics.GetWords())
            .Returns(Enumerable.Repeat("abc", 10));

        _wordCloudLayouter.GetWordCloudLayout(_defaultStringMeasurer).ToArray();

        A.CallTo(() => _settingsProvider.GetSettings())
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public void GetWordCloudLayouter_CallsGetWordsOnce()
    {
        A.CallTo(() => _wordStatistics.GetWords())
            .Returns(Enumerable.Repeat("abc", 10));

        _wordCloudLayouter.GetWordCloudLayout(_defaultStringMeasurer).ToArray();
        
        A.CallTo(() => _wordStatistics.GetWords())
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    [TestCase(5)]
    [TestCase(8)]
    [TestCase(13)]
    public void GetWordCloudLayouter_ReportsProgress(int times)
    {
        A.CallTo(() => _wordStatistics.GetWords())
            .Returns(Enumerable.Repeat("abc", times));

        _wordCloudLayouter.GetWordCloudLayout(_defaultStringMeasurer).ToArray();
        
        A.CallTo(() => _logger.ReportProgress(A<string>.Ignored, A<double>.Ignored))
            .MustHaveHappened(times, Times.Exactly);
    }

    [Test]
    [TestCase("ab cd ef")]
    [TestCase("abc ef")]
    [TestCase("dg ab cd ef")]
    [TestCase("")]
    public void GetWordCloudLayout_ReturnsWordsInOrderProvidedByStatistics(string words)
    {
        var wordsArr = words.Split(" ").ToArray();
        A.CallTo(() => _wordStatistics.GetWords())
            .Returns(wordsArr);

        _wordCloudLayouter.GetWordCloudLayout(_defaultStringMeasurer)
            .Select(info => info.Word)
            .Should()
            .Equal(wordsArr);
    }

    [Test]
    [TestCase("Arial")]
    [TestCase("Courier New")]
    [TestCase("Times New Roman")]
    public void GetWordsCloudLayout_ReturnsFontSpecifiedInSettings(string fontName)
    {
#pragma warning disable CA1416
        var font = new Font(fontName, 8, FontStyle.Regular);
        A.CallTo(() => _settingsProvider.GetSettings())
            .Returns(Settings.TestSettings with { Font = font.FontFamily });
        A.CallTo(() => _wordStatistics.GetWords())
            .Returns(Enumerable.Repeat("abc", 10));

        _wordCloudLayouter.GetWordCloudLayout(_defaultStringMeasurer)
            .Select(info => info.Font)
            .Should()
            .OnlyContain(f => f.Equals(font));
#pragma warning restore CA1416
    }

    [Test]
    [TestCase("a b c d", 8, 10, 1, 1, 1, 1)]
    [TestCase("ba ab", 12, 20, 0.5f, 0.3f)]
    [TestCase("q w e r t", 8, 17, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f)]
    public void GetWordsCloudLayout_ReturnsCorrectFontSize(
        string words, int minFontSize, int maxFontSize, params float[] frequencies)
    {
        var wordsArr = words.Split(" ").ToArray();
        foreach (var (word, i) in wordsArr.Zip(Enumerable.Range(0, wordsArr.Length)))
            A.CallTo(() => _wordStatistics.GetWordFrequency(word))
                .Returns(frequencies[i]);
        
        A.CallTo(() => _wordStatistics.GetWords())
            .Returns(wordsArr);
        
        A.CallTo(() => _settingsProvider.GetSettings())
            .Returns(Settings.TestSettings with {MinFontSize = minFontSize, MaxFontSize = maxFontSize});
        
#pragma warning disable CA1416
        _wordCloudLayouter.GetWordCloudLayout(_defaultStringMeasurer)
            .Select(info => info.Font)
            .Zip(Enumerable.Range(0, wordsArr.Length))
            .Should()
            .OnlyContain(tuple => Math.Abs(
                tuple.Item1.Size - (minFontSize + (maxFontSize - minFontSize) * frequencies[tuple.Item2])) < 1e-7);
#pragma warning restore CA1416
    }

    [Test]
    [TestCase(5)]
    [TestCase(8.1f)]
    [TestCase(7.9f)]
    [TestCase(10.5f)]
    public void GetWordCloudLayout_ReturnsRectanglesWithCorrectSizes(float fontSize)
    {
        A.CallTo(() => _wordStatistics.GetWords())
            .Returns(Enumerable.Range(1, 10).Select(i => new string('a', i)));
        A.CallTo(() => _circularCloudLayouter.PutNextRectangle(A<Size>.Ignored))
            .ReturnsLazily((Size size) => new Rectangle(new Point(0, 0), size));
        var stringMeasurer = (string str, Font _) => new SizeF(str.Length * fontSize, fontSize);

        _wordCloudLayouter.GetWordCloudLayout(stringMeasurer)
            .Should()
            .OnlyContain(info => IsGeneratedRectangleInBounds(info, stringMeasurer));
    }

    private bool IsGeneratedRectangleInBounds(WordLayoutInfo info, Func<string, Font, SizeF> stringMeasurer)
    {
        var rect = new RectangleF(new Point(0, 0), stringMeasurer(info.Word, null));
        var expandedRect = rect with { Size = rect.Size + new SizeF(1, 1) };
        return expandedRect.Contains(info.Rectangle)
               && new RectangleF(info.Rectangle.Location, info.Rectangle.Size).Contains(rect);
    }
}