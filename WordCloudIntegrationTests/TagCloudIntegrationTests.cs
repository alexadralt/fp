using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TagCloud;
using TagCloud.FileHandler;
using TagCloud.FileReader;
using TagCloud.Logger;
using TagCloud.SettingsProvider;
using TagCloud.TagsCloudVisualization;
using TagCloud.WordCloudLayouter;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;
using TagCloud.WordStatistics;
using VerifyNUnit;
using VerifyTests;

namespace WordCloudIntegrationTests;

[TestFixture]
[TestOf(typeof(WordCloudImageGeneratorImpl))]
public class TagCloudIntegrationTests
{
    private WordCloudImageGeneratorImpl _imageGenerator;
    private ILogger _logger;
    private IFileHandler _fileHandler;
    private ISettingsProvider _settingsProvider;
    private string _defaultInputFile;
    private string _defaultDelimitersFile;
    private string _defaultBoringWordsFile;

    [ModuleInitializer]
    public static void Init() => VerifyImageSharp.Initialize();

    [SetUp]
    public void SetUp()
    {
        _logger = A.Fake<ILogger>();
        _fileHandler = A.Fake<IFileHandler>();
        _settingsProvider = A.Fake<ISettingsProvider>();
        
        A.CallTo(() => _settingsProvider.GetSettings())
            .Returns(Settings.TestSettings);
#pragma warning disable CA1416
        A.CallTo(() => _fileHandler.SaveImage(A<Bitmap>.Ignored, A<string>.Ignored))
            .Invokes((Bitmap bitmap, string filePath) => bitmap.Save(filePath, ImageFormat.Png));
#pragma warning restore CA1416
        _defaultInputFile = "./../../../HarryPotterText_mod.txt";
        var lines = File.ReadAllLines(_defaultInputFile);
        A.CallTo(() => _fileHandler.ReadAllLines(_defaultInputFile))
            .Returns(lines);
        _defaultDelimitersFile = "./../../../delimiters.txt";
        _defaultBoringWordsFile = "./../../../boring.txt";
        

        var fileReaderRegistry = new FileReaderRegistry([new TxtFileReader()]);
        var wordDelimiterProvider = new WordDelimiterProviderImpl(fileReaderRegistry);
        var boringWordProvider = new BoringWordProviderImpl(fileReaderRegistry, _logger);
        var tagPreprocessor = new TagPreprocessor(boringWordProvider, wordDelimiterProvider);
        var cloudLayouter = new CircularCloudLayouterImpl(_settingsProvider);
        var wordStatistics = new WordStatisticsImpl();
        var wordCloudLayouter = new WordCloudLayouterImpl(cloudLayouter, wordStatistics, _logger, _settingsProvider);
        var wordRenderer = new TagCloudWordRenderer(wordCloudLayouter, _settingsProvider);
        
        _imageGenerator = new WordCloudImageGeneratorImpl(
            _logger, _fileHandler, tagPreprocessor, wordRenderer);
    }

    [Test]
    [TestCase("ab cd ef")]
    [TestCase("a b c d e f g h i j k l m n o p q r s")]
    [TestCase("There Are Five Words")]
    public Task TryGenerateImage_GeneratesImage(string words)
    {
        A.CallTo(() => _settingsProvider.GetSettings())
            .Returns(Settings.TestSettings with { MaxFontSize = 20 });
        A.CallTo(() => _fileHandler.ReadAllLines(A<string>.Ignored))
            .Returns(words.Split(' '));

        var outputFile = "GeneratesImageAndReturnsTrue.png";
        
        _imageGenerator.TryGenerateImageFromFile(String.Empty);
        _imageGenerator.SaveImageToFile(outputFile);
        return Verifier.VerifyFile(outputFile);
    }

    [Test]
    public Task TryGenerateImage_BigFileTest()
    {
        var outputFile = "HarryPotter.png";
        
        _imageGenerator.TryGenerateImageFromFile(_defaultInputFile);
        _imageGenerator.SaveImageToFile(outputFile);

        return Verifier.VerifyFile(outputFile);
    }

    [Test]
    public Task TryGenerateImage_BigFileWithDelimitersSpecified()
    {
        var outputFile = "HarryPotter_WithDelimiters.png";
        _imageGenerator.LoadWordDelimitersFile(_defaultDelimitersFile);
        
        _imageGenerator.TryGenerateImageFromFile(_defaultInputFile);
        _imageGenerator.SaveImageToFile(outputFile);
        
        return Verifier.VerifyFile(outputFile);
    }

    [Test]
    public Task TryGenerateImage_BigFileWithDelimitersAndBoringWordsSpecified()
    {
        var outputFile = "HarryPotter_WithDelimitersAndBoringWords.png";
        _imageGenerator.LoadWordDelimitersFile(_defaultDelimitersFile);
        _imageGenerator.LoadBoringWordsFile(_defaultBoringWordsFile);
        
        _imageGenerator.TryGenerateImageFromFile(_defaultInputFile);
        _imageGenerator.SaveImageToFile(outputFile);
        
        return Verifier.VerifyFile(outputFile);
    }
}