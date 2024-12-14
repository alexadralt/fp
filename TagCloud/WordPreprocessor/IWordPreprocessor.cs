namespace TagCloud.WordPreprocessor;

public interface IWordPreprocessor
{
    public IEnumerable<string> ExtractWords(string text);
    public void LoadWordDelimitersFile(string filePath);
    public void LoadBoringWordsFile(string filePath);
}