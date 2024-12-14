namespace TagCloud.FileReader;

public interface IFileReader : IDisposable
{
    public void OpenFile(string filePath);
    public bool TryGetNextLine(out string line);
    public string FileExtension { get; }
}