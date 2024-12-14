using System.Drawing;
using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class JpegImageFileWriter : IImageFileWriter
{
    public string Extension => ".jpeg";
    public void SaveImage(Bitmap image, string filePath)
    {
        image.Save(filePath, ImageFormat.Jpeg);
    }
}