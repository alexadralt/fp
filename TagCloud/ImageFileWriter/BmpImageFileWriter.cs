using System.Drawing;
using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class BmpImageFileWriter : IImageFileWriter
{
    public string Extension => ".bmp";
    public void SaveImage(Bitmap image, string filePath)
    {
        image.Save(filePath, ImageFormat.Bmp);
    }
}