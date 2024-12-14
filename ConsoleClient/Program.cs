using CommandLine;
using ConsoleClient;
using Pure.DI;
using TagCloud;
using TagCloud.FileHandler;
using TagCloud.FileReader;
using TagCloud.ImageFileWriter;
using TagCloud.Logger;
using TagCloud.SettingsProvider;
using TagCloud.TagsCloudVisualization;
using TagCloud.WordCloudLayouter;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;
using TagCloud.WordStatistics;

DI.Setup("Composition")
    .Bind<IFileReader>().To<TxtFileReader>()
    .Bind<IFileReader>(2).To<DocxFileReader>()
    .Bind<IWordPreprocessor>().To<TagPreprocessor>()
    .Bind<IWordRenderer>().To<TagCloudWordRenderer>()
    .Bind<ICircularCloudLayouter>().To<CircularCloudLayouterImpl>()
    .Bind<IImageFileWriter>().To<PngImageFileWriter>()
    .Bind<IImageFileWriter>(2).To<JpegImageFileWriter>()
    .Bind<IImageFileWriter>(3).To<BmpImageFileWriter>()
    .Bind<IImageFileWriter>(4).To<IconImageFileWriter>()
    .Bind<IFileHandler>().To<FileHandlerImpl>()
    .Bind<IWordCloudImageGenerator>().To<WordCloudImageGeneratorImpl>()
    .Bind<IWordStatistics>().To<WordStatisticsImpl>()
    .Bind<IWordCloudLayouter>().To<WordCloudLayouterImpl>()
    .Bind<IBoringWordProvider>().To<BoringWordProviderImpl>()
    .Bind<IWordDelimiterProvider>().To<WordDelimiterProviderImpl>()
    .Hint(Hint.Resolve, "Off")
    .Bind().As(Lifetime.Singleton).To<ConsoleLogger>()
    .Bind().As(Lifetime.Singleton).To<SettingsProviderImpl>()
    .Root<CLIClient>("Client");

var composition = new Composition();
var client = composition.Client;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(client.RunOptions);