using System.Drawing;

namespace TagCloud.ImageFileWriter;

public interface IImageFileWriter
{
    public string Extension { get; }
    public void SaveImage(Bitmap image, string filePath);
}