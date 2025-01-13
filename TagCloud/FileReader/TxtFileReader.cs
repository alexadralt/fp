using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public class TxtFileReader : BaseFileReader
{
    public override string FileExtension => ".txt";

    public override Result<string[]> ReadAllLines(string filePath)
    {
        return Result.FromValue(filePath)
            .Then(CheckFile)
            .Try(File.ReadAllLines);
    }
}