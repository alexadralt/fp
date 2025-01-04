using TagCloud.ResultUtils;

namespace TagCloud.WordPreprocessor;

public interface IWordPreprocessor
{
    public IEnumerable<string> ExtractWords(string text);
    public Result<Nothing> LoadWordDelimitersFile(string filePath);
    public Result<Nothing> LoadBoringWordsFile(string filePath);
}