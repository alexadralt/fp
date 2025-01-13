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
    public Result<string[]> ReadAllLines(string filePath)
    {
        return Result.FromValue(filePath)
            .Validate(path => !string.IsNullOrWhiteSpace(path), "Input file was not specified.")
            .Then(Path.GetExtension)
            .Validate(extension => !string.IsNullOrWhiteSpace(extension),
                $"Missing input file extension: {filePath} <---")
            .Then(extension => readerRegistry.GetFileReader(extension!))
            .Then(fr => fr.ReadAllLines(Path.GetFullPath(filePath)));
    }

    public Result<Nothing> SaveImage(Bitmap image, string filePath)
    {
        return writerRegistry.GetImageFileWriter(Path.GetExtension(filePath))
            .Then(fw => fw.SaveImage(image, filePath));
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