using System.Drawing;
using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class IconImageFileWriter : IImageFileWriter
{
    public string Extension => ".ico";
    public void SaveImage(Bitmap image, string filePath)
    {
        image.Save(filePath, ImageFormat.Icon);
    }
}