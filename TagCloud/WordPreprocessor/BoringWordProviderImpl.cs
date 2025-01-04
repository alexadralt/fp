using TagCloud.FileReader;
using TagCloud.ResultUtils;

namespace TagCloud.WordPreprocessor;

public class BoringWordProviderImpl(FileReaderRegistry fileReaderRegistry) : IBoringWordProvider
{
    private readonly HashSet<string> _boringWords = new();
    
    public bool IsBoring(string word)
    {
        return _boringWords.Contains(word);
    }

    public Result<Nothing> LoadBoringWordsFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        var fileReaderResult = fileReaderRegistry.GetFileReader(extension);
        if (fileReaderResult.Success)
        {
            var fileReader = fileReaderResult.Value!;
            foreach (var line in fileReader.ReadAllLines(Path.GetFullPath(filePath)))
            {
                if (line.Success)
                    _boringWords.Add(line.Value!);
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