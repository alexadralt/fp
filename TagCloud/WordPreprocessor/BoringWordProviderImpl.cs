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
        return Result.FromValue(filePath)
            .Validate(path => !string.IsNullOrWhiteSpace(path), "File was not specified.")
            .Then(Path.GetExtension)
            .Validate(extension => !string.IsNullOrWhiteSpace(extension),
                $"Missing file extension: {filePath} <---")
            .Then(extension => fileReaderRegistry.GetFileReader(extension!))
            .Then(fr => fr.ReadAllLines(Path.GetFullPath(filePath)))
            .ForEach(line => _boringWords.Add(line));
    }
}