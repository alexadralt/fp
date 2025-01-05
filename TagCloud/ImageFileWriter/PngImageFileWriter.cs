using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class PngImageFileWriter : BitmapImageFileWriter
{
    public override string Extension => ".png";
    protected override ImageFormat Format => ImageFormat.Png;
}