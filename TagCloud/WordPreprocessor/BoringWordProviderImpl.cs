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
        if (!Path.Exists(filePath))
        {
            return Result.Failure($"Could not find boring words file at: {Path.GetFullPath(filePath)}");
        }
        
        var extension = Path.GetExtension(filePath);
        var fileReaderResult = fileReaderRegistry.GetFileReader(extension);
        if (fileReaderResult.Success)
        {
            var fileReader = fileReaderResult.Value!;
            var openFileResult = fileReader.OpenFile(Path.GetFullPath(filePath));
            if (!openFileResult.Success)
                return Result.Failure($"Failed to open file: {openFileResult.Error}");
            
            var result = fileReader.GetNextLine();
            while (result.Success)
            {
                _boringWords.Add(result.Value!);
                result = fileReader.GetNextLine();
            }
            
            fileReaderRegistry.ReturnFileReader(fileReader);
        }
        else
        {
            return Result.Failure(fileReaderResult.Error!);
        }
        
        return Result.Success();
    }
}