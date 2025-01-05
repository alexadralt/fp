using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class BmpImageFileWriter : BitmapImageFileWriter
{
    public override string Extension => ".bmp";
    protected override ImageFormat Format => ImageFormat.Bmp;
}