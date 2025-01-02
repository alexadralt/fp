using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public class TxtFileReader : IFileReader
{
    private StreamReader? _streamReader;

    public string FileExtension { get => ".txt"; }
    
    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _streamReader?.Dispose();
            _streamReader = null;
        }
    }

    public Result<Nothing> OpenFile(string filePath)
    {
        if (_streamReader != null)
            return Result.Failure("File is already open");

        if (!Path.IsPathFullyQualified(filePath))
            return Result.Failure("path must be absolute");
        if (!Path.HasExtension(filePath) || !Path.GetExtension(filePath).Equals(FileExtension))
            return Result.Failure($"given path does not refer to a {FileExtension} file");

        try
        {
            _streamReader = new StreamReader(filePath);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
        
        return Result.Success();
    }

    public Result<string> GetNextLine()
    {
        if (_streamReader == null)
            return Result.FromError<string>("File is not open");
        if (_streamReader.EndOfStream)
            return Result.FromError<string>("End of file reached");

        string? line;
        try
        {
            line = _streamReader.ReadLine();
        }
        catch (Exception ex)
        {
            return Result.FromError<string>(ex.Message);
        }

        return line != null ? Result.FromValue(line) : Result.FromError<string>("Could not read from a file");
    }
}