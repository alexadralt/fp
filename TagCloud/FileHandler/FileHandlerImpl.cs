using System.Drawing;
using TagCloud.FileReader;
using TagCloud.ImageFileWriter;
using TagCloud.ResultUtils;

namespace TagCloud.FileHandler;

public class FileHandlerImpl(
    FileReaderRegistry readerRegistry,
    ImageFileWriterRegistry writerRegistry
    ) : IFileHandler
{
    public IEnumerable<Result<string>> ReadAllLines(string filePath)
    {
        var check = CheckInputFile(filePath);
        if (!check.Success)
            return Result.FromErrorEnumerable<string>(check.Error!);
        
        var extension = Path.GetExtension(filePath);
        var fileReaderResult = readerRegistry.GetFileReader(extension);
        if (fileReaderResult.Success)
        {
            var fileReader = fileReaderResult.Value!;
            return fileReader.ReadAllLines(Path.GetFullPath(filePath));
        }

        return Result.FromErrorEnumerable<string>(fileReaderResult.Error!);
    }

    private Result<Nothing> CheckInputFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result.Failure("Input file was not specified.");
        }
        
        if (string.IsNullOrEmpty(Path.GetExtension(filePath)))
        {
            return Result.Failure($"Missing input file extension: {filePath} <---");
        }
        
        return Result.Success();
    }

    public Result<Nothing> SaveImage(Bitmap image, string filePath)
    {
        var fileWriterResult = writerRegistry.TryGetImageFileWriter(Path.GetExtension(filePath)); 
        if (fileWriterResult.Success)
            return fileWriterResult.Value!.SaveImage(image, filePath);

        return Result.Failure(fileWriterResult.Error!);
    }

    public bool IsSupportedOutputFileExtension(string extension)
    {
        return writerRegistry.IsSupportedExtension(extension);
    }

    public IEnumerable<string> GetSupportedOutputFileExtensions()
    {
        return writerRegistry.GetSupportedImageFileExtensions();
    }
}