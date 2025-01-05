using System.Drawing;
using System.Drawing.Imaging;

namespace TagCloud.ImageFileWriter;

public class IconImageFileWriter : BitmapImageFileWriter
{
    public override string Extension => ".ico";
    protected override ImageFormat Format => ImageFormat.Icon;
}