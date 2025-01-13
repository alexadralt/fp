using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public class FileReaderRegistry
{
    private readonly Dictionary<string, IFileReader> _fileReaders;

    public FileReaderRegistry(IFileReader[] fileReaders)
    {
        _fileReaders = new Dictionary<string, IFileReader>();
        foreach (var reader in fileReaders)
            _fileReaders.TryAdd(reader.FileExtension, reader);
    }
    
    public Result<IFileReader> GetFileReader(string fileExtension)
    {
        if (_fileReaders.TryGetValue(fileExtension, out var fileReader))
            return Result.FromValue(fileReader);

        return Result.FromError<IFileReader>($"Extension \"{fileExtension}\" is not supported.\n" +
                                             $"Supported extensions are: " +
                                             $"{string.Join(", ", _fileReaders.Keys)}");
    }
}