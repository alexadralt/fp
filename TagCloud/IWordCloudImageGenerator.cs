namespace TagCloud;

public interface IWordCloudImageGenerator
{
    public bool TryGenerateImageFromFile(string filePath);
    public void SaveImageToFile(string filePath);
    public bool IsValidInputFile(string filePath, out string? errorMessage);
    public bool IsSupportedOutputFileExtension(string? filePath, out string? errorMessage);
    public bool DoesOutputFileExist(string filePath);
    public void LoadWordDelimitersFile(string filePath);
    public void LoadBoringWordsFile(string filePath);
}