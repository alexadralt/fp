using DocumentFormat.OpenXml.Packaging;
using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public class DocxFileReader : BaseFileReader
{
    public override string FileExtension => ".docx";

    public override IEnumerable<Result<string>> ReadAllLines(string filePath)
    {
        var check = CheckFile(filePath);
        if (!check.Success)
            return Result.FromErrorEnumerable<string>(check.Error!);

        WordprocessingDocument? document = null;
        try
        {
            document = WordprocessingDocument.Open(filePath, false);
            var childElements = document.MainDocumentPart?.Document.Body?.ChildElements;
            if (childElements == null)
                return Result.FromErrorEnumerable<string>("Word document is empty?");
            return childElements.Value.Select(e => Result.FromValue(e.InnerText));
        }
        catch (Exception ex)
        {
            return Result.FromErrorEnumerable<string>(ex.Message);
        }
        finally
        {
            document?.Dispose();
        }
    }
}