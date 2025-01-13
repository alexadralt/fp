using System.Drawing;
using TagCloud.FileHandler;
using TagCloud.ResultUtils;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;

namespace TagCloud;

public class WordCloudImageGeneratorImpl(
    IFileHandler fileHandler,
    IWordPreprocessor wordPreprocessor,
    IWordRenderer wordRenderer
    ): IWordCloudImageGenerator
{
#pragma warning disable CA1416
    public Result<Bitmap> GenerateImageFromFile(string filePath)
    {
        return fileHandler.ReadAllLines(filePath)
            .ForEach(line =>
            {
                var words = wordPreprocessor.ExtractWords(line);
                wordRenderer.WordStatistics.Populate(words);
            })
            .ChangeError(err => $"Couldn't read input file:\n{err}")
            .Then(_ => wordRenderer.Render());
    }

    public Result<Nothing> SaveImageToFile(Bitmap image, string filePath)
    {
        return Result.FromValue(image)
            .Then(bitmap => fileHandler.SaveImage(bitmap, filePath));
    }
#pragma warning restore CA1416

    public Result<Nothing> ValidateOutputFile(string? filePath)
    {
        return Result.FromValue(filePath)
            .Validate(path => !string.IsNullOrWhiteSpace(path), "Output file was not specified.")
            .Then(Path.GetExtension)
            .Validate(ext => !string.IsNullOrEmpty(ext), $"Missing output file extension: {filePath} <---")
            .Validate(ext => fileHandler.IsSupportedOutputFileExtension(ext!),
                $"Unsupported output file extension: {Path.GetExtension(filePath)}\n"
                + $"Supported extensions are: {string.Join(", ",
                    fileHandler.GetSupportedOutputFileExtensions())}")
            .Then(_ => Result.Success());
    }

    public bool DoesOutputFileExist(string filePath)
    {
        return File.Exists(filePath);
    }

    public Result<Nothing> LoadWordDelimitersFile(string filePath)
    {
        return wordPreprocessor.LoadWordDelimitersFile(filePath);
    }

    public Result<Nothing> LoadBoringWordsFile(string filePath)
    {
        return wordPreprocessor.LoadBoringWordsFile(filePath);
    }
}