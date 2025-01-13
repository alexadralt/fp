using System.Drawing;
using TagCloud.ResultUtils;

namespace TagCloud;

public interface IWordCloudImageGenerator
{
    public Result<Bitmap> GenerateImageFromFile(string filePath);
    public Result<Nothing> SaveImageToFile(Bitmap image, string filePath);
    public Result<Nothing> ValidateOutputFile(string? filePath);
    public bool DoesOutputFileExist(string filePath);
    public Result<Nothing> LoadWordDelimitersFile(string filePath);
    public Result<Nothing> LoadBoringWordsFile(string filePath);
}