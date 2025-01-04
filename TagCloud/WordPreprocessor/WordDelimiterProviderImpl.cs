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

    public Result<Nothing> LoadDelimitersFile(string path)
    {
        var extension = Path.GetExtension(path);
        var fileReaderResult = _fileReaderRegistry.GetFileReader(extension);
        if (fileReaderResult.Success)
        {
            var fileReader = fileReaderResult.Value!;
            foreach (var line in fileReader.ReadAllLines(Path.GetFullPath(path)))
            {
                if (line.Success)
                    _delimiters.Add(line.Value!);
                else
                    return Result.Failure(line.Error!);
            }
        }
        else
        {
            return Result.Failure(fileReaderResult.Error!);
        }

        return Result.Success();
    }
}