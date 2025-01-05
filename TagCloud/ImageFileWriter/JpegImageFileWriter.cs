using System.Drawing;
using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class JpegImageFileWriter : BitmapImageFileWriter
{
    public override string Extension => ".jpeg";
    protected override ImageFormat Format => ImageFormat.Jpeg;
}