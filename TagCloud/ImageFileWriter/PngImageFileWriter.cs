using System.Drawing;
using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class PngImageFileWriter : IImageFileWriter
{
    public string Extension => ".png";
    public void SaveImage(Bitmap image, string filePath)
    {
        image.Save(Path.GetFullPath(filePath), ImageFormat.Png);
    }
}