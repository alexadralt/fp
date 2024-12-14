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

    public bool TryGetImageFileWriter(string extension, out IImageFileWriter imageFileWriter)
    {
        return _imageFileWriters.TryGetValue(extension, out imageFileWriter);
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