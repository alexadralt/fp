using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TagCloud;
using TagCloud.FileHandler;
using TagCloud.FileReader;
using TagCloud.ImageFileWriter;
using TagCloud.Logger;
using TagCloud.ResultUtils;
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
        
        A.CallTo(() => _settingsProvider.GetImageSettings())
            .Returns(Result.FromValue(Settings.TestSettings.Image));
        A.CallTo(() => _settingsProvider.GetAlgorithmSettings())
            .Returns(Result.FromValue(Settings.TestSettings.Algorithm));
        A.CallTo(() => _settingsProvider.GetFontSettings())
            .Returns(Result.FromValue(Settings.TestSettings.Font));
        
        var fileReaderRegistry = new FileReaderRegistry([new TxtFileReader()]);
        var fileWriterRegistry = new ImageFileWriterRegistry([new PngImageFileWriter()]);
        var realFileHandler = new FileHandlerImpl(fileReaderRegistry, fileWriterRegistry);
        
        _defaultInputFile = "./../../../HarryPotterText_mod.txt";
        A.CallTo(() => _fileHandler.ReadAllLines(_defaultInputFile))
            .Returns(realFileHandler.ReadAllLines(_defaultInputFile));
#pragma warning disable CA1416
        A.CallTo(() => _fileHandler.SaveImage(A<Bitmap>.Ignored, A<string>.Ignored))
            .Invokes((Bitmap bitmap, string path) => realFileHandler.SaveImage(bitmap, path));
#pragma warning restore CA1416
        
        _defaultDelimitersFile = "./../../../delimiters.txt";
        _defaultBoringWordsFile = "./../../../boring.txt";

        var wordDelimiterProvider = new WordDelimiterProviderImpl(fileReaderRegistry);
        var boringWordProvider = new BoringWordProviderImpl(fileReaderRegistry);
        var tagPreprocessor = new TagPreprocessor(boringWordProvider, wordDelimiterProvider);
        var cloudLayouter = new CircularCloudLayouterImpl(_settingsProvider);
        var wordStatistics = new WordStatisticsImpl();
        var wordCloudLayouter = new WordCloudLayouterImpl(cloudLayouter, wordStatistics, _logger, _settingsProvider);
        var wordRenderer = new TagCloudWordRenderer(wordCloudLayouter, _settingsProvider);
        
        _imageGenerator = new WordCloudImageGeneratorImpl(
            _fileHandler, tagPreprocessor, wordRenderer);
    }

    [Test]
    [TestCase("ab cd ef")]
    [TestCase("a b c d e f g h i j k l m n o p q r s")]
    [TestCase("There Are Five Words")]
    public Task TryGenerateImage_GeneratesImage(string words)
    {
        A.CallTo(() => _settingsProvider.GetFontSettings())
            .Returns(Result.FromValue(Settings.TestSettings.Font with { MaxFontSize = 20 }));
        A.CallTo(() => _fileHandler.ReadAllLines(A<string>.Ignored))
            .Returns(Result.FromValue(words.Split(' ')));

        var outputFile = "GeneratesImageAndReturnsTrue.png";
        
#pragma warning disable CA1416
        _imageGenerator.GenerateImageFromFile(String.Empty)
            .Then(image => _imageGenerator.SaveImageToFile(image, outputFile));
#pragma warning restore CA1416
        return Verifier.VerifyFile(outputFile);
    }

    [Test]
    public Task TryGenerateImage_BigFileTest()
    {
        var outputFile = "HarryPotter.png";
        
#pragma warning disable CA1416
        _imageGenerator.GenerateImageFromFile(_defaultInputFile)
            .Then(image => _imageGenerator.SaveImageToFile(image, outputFile));
#pragma warning restore CA1416

        return Verifier.VerifyFile(outputFile);
    }

    [Test]
    public Task TryGenerateImage_BigFileWithDelimitersSpecified()
    {
        var outputFile = "HarryPotter_WithDelimiters.png";
        _imageGenerator.LoadWordDelimitersFile(_defaultDelimitersFile);
        
#pragma warning disable CA1416
        _imageGenerator.GenerateImageFromFile(_defaultInputFile)
            .Then(image => _imageGenerator.SaveImageToFile(image, outputFile));
#pragma warning restore CA1416
        
        return Verifier.VerifyFile(outputFile);
    }

    [Test]
    public Task TryGenerateImage_BigFileWithDelimitersAndBoringWordsSpecified()
    {
        var outputFile = "HarryPotter_WithDelimitersAndBoringWords.png";
        _imageGenerator.LoadWordDelimitersFile(_defaultDelimitersFile);
        _imageGenerator.LoadBoringWordsFile(_defaultBoringWordsFile);
        
#pragma warning disable CA1416
        _imageGenerator.GenerateImageFromFile(_defaultInputFile)
            .Then(image => _imageGenerator.SaveImageToFile(image, outputFile));
#pragma warning restore CA1416
        
        return Verifier.VerifyFile(outputFile);
    }
}