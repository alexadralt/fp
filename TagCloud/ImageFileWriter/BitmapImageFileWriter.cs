using System.Drawing;
using System.Drawing.Imaging;
using TagCloud.ResultUtils;

namespace TagCloud.ImageFileWriter;

public abstract class BitmapImageFileWriter : IImageFileWriter
{
    public abstract string Extension { get; }
    protected abstract ImageFormat Format { get; }
    public Result<Nothing> SaveImage(Bitmap image, string filePath)
    {
        return Result.Success()
            .Try(() => image.Save(Path.GetFullPath(filePath), Format));
    }
}