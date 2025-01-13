using TagCloud.FileReader;
using TagCloud.ResultUtils;

namespace TagCloud.WordPreprocessor;

public class WordDelimiterProviderImpl : IWordDelimiterProvider
{
    private readonly HashSet<string> _delimiters;
    private readonly FileReaderRegistry _fileReaderRegistry;

    public WordDelimiterProviderImpl(FileReaderRegistry fileReaderRegistry)
    {
        _fileReaderRegistry = fileReaderRegistry;
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

    public Result<Nothing> LoadDelimitersFile(string filePath)
    {
        return Result.FromValue(filePath)
            .Validate(path => !string.IsNullOrWhiteSpace(path), "File was not specified.")
            .Then(Path.GetExtension)
            .Validate(extension => !string.IsNullOrWhiteSpace(extension),
                $"Missing file extension: {filePath} <---")
            .Then(extension => _fileReaderRegistry.GetFileReader(extension!))
            .Then(fr => fr.ReadAllLines(Path.GetFullPath(filePath)))
            .ForEach(line => _delimiters.Add(line));
    }
}