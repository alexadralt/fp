using System.Drawing;
using TagCloud.ResultUtils;

namespace TagCloud.ImageFileWriter;

public interface IImageFileWriter
{
    public string Extension { get; }
    public Result<Nothing> SaveImage(Bitmap image, string filePath);
}