namespace TagCloud.WordPreprocessor;

public class TagPreprocessor(
    IBoringWordProvider boringWordProvider,
    IWordDelimiterProvider wordDelimiterProvider
    ) : IWordPreprocessor
{
    private string[] _delimiters = null!;
    
    public IEnumerable<string> ExtractWords(string text)
    {
        if (_delimiters == null!)
            _delimiters = wordDelimiterProvider.GetDelimiters();
        
        return text.Split(_delimiters,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(ProcessWord)
            .Where(IsGoodWord);
    }

    public void LoadWordDelimitersFile(string filePath)
    {
        wordDelimiterProvider.LoadDelimitersFile(filePath);
    }

    public void LoadBoringWordsFile(string filePath)
    {
        boringWordProvider.LoadBoringWordsFile(filePath);
    }

    private string ProcessWord(string word)
    {
        return word.ToLower();
    }

    private bool IsGoodWord(string word)
    {
        return !boringWordProvider.IsBoring(word);
    }
}