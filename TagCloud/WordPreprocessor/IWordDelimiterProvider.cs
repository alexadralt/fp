namespace TagCloud.WordPreprocessor;

public interface IWordDelimiterProvider
{
    public string[] GetDelimiters();
    public void LoadDelimitersFile(string path);
}