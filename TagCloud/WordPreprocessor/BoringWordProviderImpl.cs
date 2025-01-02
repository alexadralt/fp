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
            var openFileResult = fileReader.OpenFile(Path.GetFullPath(filePath)); 
            if (!openFileResult.Success)
            {
                logger.Error($"Failed to open file: {openFileResult.Error}");
                return;
            }
            
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
            logger.Error($"Unsupported file format \"{extension}\"");
        }
    }
}