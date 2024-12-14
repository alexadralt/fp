namespace TagCloud.WordStatistics;

public interface IWordStatistics
{
    public float GetWordFrequency(string word);
    public IEnumerable<string> GetWords();
    public void Populate(IEnumerable<string> words);
}