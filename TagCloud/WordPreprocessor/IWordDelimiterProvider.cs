using TagCloud.ResultUtils;

namespace TagCloud.WordPreprocessor;

public interface IWordDelimiterProvider
{
    public string[] GetDelimiters();
    public Result<Nothing> LoadDelimitersFile(string path);
}