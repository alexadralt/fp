using TagCloud.FileReader;
using TagCloud.Logger;
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
        if (!Path.Exists(path))
            return Result.Failure($"Could not find delimiters file at: {Path.GetFullPath(path)}");
        
        var extension = Path.GetExtension(path);
        var fileReaderResult = _fileReaderRegistry.GetFileReader(extension);
        if (fileReaderResult.Success)
        {
            var fileReader = fileReaderResult.Value!;
            
            var openFileResult = fileReader.OpenFile(Path.GetFullPath(path));
            if (!openFileResult.Success)
                return Result.Failure($"Failed to open file: {openFileResult.Error}");
            
            var result = fileReader.GetNextLine();
            while (result.Success)
            {
                _delimiters.Add(result.Value!);
                result = fileReader.GetNextLine();
            }
            
            _fileReaderRegistry.ReturnFileReader(fileReader);
        }
        else
        {
            return Result.Failure(fileReaderResult.Error!);
        }

        return Result.Success();
    }
}