namespace TagCloud.WordPreprocessor;

public interface IBoringWordProvider
{
    public bool IsBoring(string word);
    public void LoadBoringWordsFile(string filePath);
}