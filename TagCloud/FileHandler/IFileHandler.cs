using System.Drawing;

namespace TagCloud.FileHandler;

public interface IFileHandler
{
    public IEnumerable<string> ReadAllLines(string filePath);
    public void SaveImage(Bitmap image, string filePath);
    public bool IsValidInputFile(string filePath, out string? errorMessage);
    
    public bool IsSupportedOutputFileExtension(string extension);
    public IEnumerable<string> GetSupportedOutputFileExtensions();
}