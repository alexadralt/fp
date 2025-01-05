using System.Diagnostics;
using System.Drawing;
using TagCloud.FileHandler;
using TagCloud.Logger;
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
    private Bitmap? _bitmap;
    
    public Result<Nothing> GenerateImageFromFile(string filePath)
    {
        foreach (var line in fileHandler.ReadAllLines(filePath))
        {
            if (line.Success)
            {
                var words = wordPreprocessor.ExtractWords(line.Value!);
                wordRenderer.WordStatistics.Populate(words);
            }
            else
            {
                return Result.Failure($"Couldn't read input file:\n" +
                                      $"{line.Error!}");
            }
        }

        try
        {
            _bitmap = wordRenderer.Render();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
        
        return Result.Success();
    }

    public Result<Nothing> SaveImageToFile(string filePath)
    {
        if (_bitmap == null)
            return Result.Failure("Image was not generated yet.");
        
        return fileHandler.SaveImage(_bitmap, filePath);
    }

    public bool IsSupportedOutputFileExtension(string? filePath, out string? errorMessage)
    {
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            errorMessage = "Output file was not specified.";
            return false;
        }
        
        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(extension))
        {
            errorMessage = $"Missing output file extension: {filePath} <---";
            return false;
        }

        var isSupported = fileHandler.IsSupportedOutputFileExtension(extension);
        errorMessage = isSupported
            ? null
            : $"Unsupported output file extension: {extension}\n"
              + $"Supported extensions are: {string.Join(", ",
                  fileHandler.GetSupportedOutputFileExtensions())}";
        return isSupported;
    }

    public bool DoesOutputFileExist(string filePath)
    {
        return Path.Exists(filePath);
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