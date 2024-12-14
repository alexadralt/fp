using TagCloud.FileReader;
using TagCloud.Logger;

namespace TagCloud.WordPreprocessor;

public class WordDelimiterProviderImpl : IWordDelimiterProvider
{
    private readonly HashSet<string> _delimiters;
    private readonly ILogger _logger;
    private readonly FileReaderRegistry _fileReaderRegistry;

    public WordDelimiterProviderImpl(FileReaderRegistry fileReaderRegistry, ILogger logger)
    {
        _fileReaderRegistry = fileReaderRegistry;
        _logger = logger;
        _delimiters = new HashSet<string>();
        foreach (var del in new []{ "\n", "\t", "\r", " " })
        {
            _delimiters.Add(del);
        }
    }
    
    public string[] GetDelimiters()
    {
        return _delimiters.ToArray();
    }

    public void LoadDelimitersFile(string path)
    {
        if (!Path.Exists(path))
        {
            _logger.Warning($"Could not find delimiters file at: {Path.GetFullPath(path)}");
            _logger.Warning("Using only default word delimiters.");
            return;
        }
        
        var extension = Path.GetExtension(path);
        if (_fileReaderRegistry.TryGetFileReader(extension, out var fileReader))
        {
            _logger.Info("Loading delimiters file.");
            fileReader.OpenFile(Path.GetFullPath(path));
            while (fileReader.TryGetNextLine(out var line))
                _delimiters.Add(line);
            _fileReaderRegistry.ReturnFileReader(fileReader);
        }
        else
        {
            _logger.Error($"Unsupported file format \"{extension}\"");
        }
    }
}