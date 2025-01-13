using DocumentFormat.OpenXml.Packaging;
using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public class DocxFileReader : BaseFileReader
{
    public override string FileExtension => ".docx";

    public override Result<string[]> ReadAllLines(string filePath)
    {
        return Result.FromValue(filePath)
            .Then(CheckFile)
            .Try(file =>
            {
                using var document = WordprocessingDocument.Open(file, false);
                return Result.FromValue(document)
                    .Then(doc => doc.MainDocumentPart?.Document.Body?.ChildElements)
                    .Validate(list => list != null, "Word document is empty?")
                    .Then(list => list!.Value.Select(e => e.InnerText).ToArray());
            });
    }
}