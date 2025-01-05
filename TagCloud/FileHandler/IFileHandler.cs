using System.Collections;
using System.Drawing;
using TagCloud.ResultUtils;

namespace TagCloud.FileHandler;

public interface IFileHandler
{
    public IEnumerable<Result<string>> ReadAllLines(string filePath);
    public Result<Nothing> SaveImage(Bitmap image, string filePath);
    
    public bool IsSupportedOutputFileExtension(string extension);
    public IEnumerable<string> GetSupportedOutputFileExtensions();
}