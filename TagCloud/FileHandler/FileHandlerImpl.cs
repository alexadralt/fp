using System.Drawing;
using TagCloud.FileReader;
using TagCloud.ImageFileWriter;
using TagCloud.Logger;

namespace TagCloud.FileHandler;

public class FileHandlerImpl(
    FileReaderRegistry readerRegistry,
    ImageFileWriterRegistry writerRegistry,
    ILogger logger
    ) : IFileHandler
{
    public IEnumerable<string> ReadAllLines(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (readerRegistry.TryGetFileReader(extension, out var fileReader))
        {
            fileReader.OpenFile(Path.GetFullPath(filePath));
            while (fileReader.TryGetNextLine(out var line))
                yield return line;
            readerRegistry.ReturnFileReader(fileReader);
        }
        else
        {
            throw new ArgumentException($"Unsupported input file format: {extension}");
        }
    }

    public void SaveImage(Bitmap image, string filePath)
    {
        if (writerRegistry.TryGetImageFileWriter(Path.GetExtension(filePath), out var imageWriter))
        {
            imageWriter.SaveImage(image, filePath);
        }
        else
        {
            throw new ArgumentException($"Unsupported image format: {Path.GetExtension(filePath)}");
        }
        
        logger.Info($"Output file is saved to {Path.GetFullPath(filePath)}");
    }

    public bool IsValidInputFile(string filePath, out string? errorMessage)
    {
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            errorMessage = "Input file was not specified.";
            return false;
        }
        
        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(extension))
        {
            errorMessage = $"Missing input file extension: {filePath} <---";
            return false;
        }
        
        if (!readerRegistry.IsSupportedFileExtension(extension))
        {
            errorMessage = $"Unsupported input file extension: {extension}\n" +
                           $"Supported extensions are: {string.Join(", ",
                               readerRegistry.GetSupportedFileExtensions())}";
            return false;
        }
        
        if (!Path.Exists(filePath))
        {
            errorMessage = $"Could not find input file: {Path.GetFullPath(filePath)}";
            return false;
        }
        
        errorMessage = null;
        return true;
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