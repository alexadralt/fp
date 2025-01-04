using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public abstract class BaseFileReader : IFileReader
{
    public abstract string FileExtension { get; }
    public abstract IEnumerable<Result<string>> ReadAllLines(string filePath);
    
    protected Result<Nothing> CheckFile(string filePath)
    {
        if (!Path.IsPathFullyQualified(filePath))
            return Result.Failure("path must be absolute");
        if (!Path.HasExtension(filePath) || !Path.GetExtension(filePath).Equals(FileExtension))
            return Result.Failure($"given path does not refer to a {FileExtension} file");
        if (!File.Exists(filePath))
            return Result.Failure($"file does not exist: {filePath}");
        
        return Result.Success();
    }
}