using TagCloud.ResultUtils;

namespace TagCloud;

public interface IWordCloudImageGenerator
{
    public Result<Nothing> GenerateImageFromFile(string filePath);
    public void SaveImageToFile(string filePath);
    public bool IsSupportedOutputFileExtension(string? filePath, out string? errorMessage);
    public bool DoesOutputFileExist(string filePath);
    public Result<Nothing> LoadWordDelimitersFile(string filePath);
    public Result<Nothing> LoadBoringWordsFile(string filePath);
}