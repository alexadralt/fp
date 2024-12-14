using TagCloud.FileReader;
using TagCloud.Logger;

namespace TagCloud.WordPreprocessor;

public class BoringWordProviderImpl(FileReaderRegistry fileReaderRegistry, ILogger logger) : IBoringWordProvider
{
    private readonly HashSet<string> _boringWords = new();
    
    public bool IsBoring(string word)
    {
        return _boringWords.Contains(word);
    }

    public void LoadBoringWordsFile(string filePath)
    {
        if (!Path.Exists(filePath))
        {
            logger.Warning($"Could not find boring words file at: {Path.GetFullPath(filePath)}");
            return;
        }
        
        var extension = Path.GetExtension(filePath);
        if (fileReaderRegistry.TryGetFileReader(extension, out var fileReader))
        {
            logger.Info("Loading boring words file.");
            fileReader.OpenFile(Path.GetFullPath(filePath));
            while (fileReader.TryGetNextLine(out var line))
                _boringWords.Add(line);
            fileReaderRegistry.ReturnFileReader(fileReader);
        }
        else
        {
            logger.Error($"Unsupported file format \"{extension}\"");
        }
    }
}