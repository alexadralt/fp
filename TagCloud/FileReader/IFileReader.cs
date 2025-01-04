using System.Collections;
using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public interface IFileReader
{
    public IEnumerable<Result<string>> ReadAllLines(string filePath);
    public string FileExtension { get; }
}