using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public class TxtFileReader : BaseFileReader
{
    public override string FileExtension => ".txt";

    public override IEnumerable<Result<string>> ReadAllLines(string filePath)
    {
        var check = CheckFile(filePath);
        if (!check.Success)
            return Result.FromErrorEnumerable<string>(check.Error!);
        
        try
        {
            return File.ReadAllLines(filePath).Select(line => new Result<string>(line));
        }
        catch (Exception ex)
        {
            return Result.FromErrorEnumerable<string>(ex.Message);
        }
    }
}