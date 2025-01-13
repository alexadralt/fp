using System.Collections;
using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public abstract class BaseFileReader : IFileReader
{
    public abstract string FileExtension { get; }
    public abstract Result<string[]> ReadAllLines(string filePath);
    
    protected Result<string> CheckFile(string filePath)
    {
        return Result.FromValue(filePath)
            .Validate(path => Path.IsPathFullyQualified(path!), "Path must be absolute")
            .Validate(path => Path.HasExtension(path) && Path.GetExtension(path) == FileExtension,
                $"Given path does not refer to a {FileExtension} file")
            .Validate(File.Exists, $"File does not exist: {filePath}");
    }
}