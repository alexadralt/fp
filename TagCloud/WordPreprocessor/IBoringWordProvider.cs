using TagCloud.ResultUtils;

namespace TagCloud.WordPreprocessor;

public interface IBoringWordProvider
{
    public bool IsBoring(string word);
    public Result<Nothing> LoadBoringWordsFile(string filePath);
}