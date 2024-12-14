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
    
    public bool TryGetFileReader(string fileExtension, out IFileReader fileReader)
    {
        return _fileReaders.Remove(fileExtension, out fileReader);
    }

    public void ReturnFileReader(IFileReader fileReader)
    {
        fileReader.Dispose();
        _fileReaders.Add(fileReader.FileExtension, fileReader);
    }

    public bool IsSupportedFileExtension(string fileExtension)
    {
        return _fileReaders.ContainsKey(fileExtension);
    }

    public IEnumerable<string> GetSupportedFileExtensions()
    {
        return _fileReaders.Keys;
    }
}