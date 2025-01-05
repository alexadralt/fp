using TagCloud.ResultUtils;

namespace TagCloud.ImageFileWriter;

public class ImageFileWriterRegistry
{
    private readonly Dictionary<string, IImageFileWriter> _imageFileWriters;

    public ImageFileWriterRegistry(IImageFileWriter[] imageFileWriters)
    {
        _imageFileWriters = new Dictionary<string, IImageFileWriter>();
        foreach (var writer in imageFileWriters)
            _imageFileWriters.TryAdd(writer.Extension, writer);
    }

    public Result<IImageFileWriter> TryGetImageFileWriter(string extension)
    {
        if (_imageFileWriters.TryGetValue(extension, out var imageFileWriter))
            return Result.FromValue(imageFileWriter);
        
        return Result.FromError<IImageFileWriter>($"Unsupported image file extension: {extension}");
    }

    public bool IsSupportedExtension(string extension)
    {
        return _imageFileWriters.ContainsKey(extension);
    }

    public IEnumerable<string> GetSupportedImageFileExtensions()
    {
        return _imageFileWriters.Keys;
    }
}