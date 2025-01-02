using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public interface IFileReader : IDisposable
{
    public Result<Nothing> OpenFile(string filePath);
    public Result<string> GetNextLine();
    public string FileExtension { get; }
}