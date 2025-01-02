using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using TagCloud.ResultUtils;

namespace TagCloud.FileReader;

public class DocxFileReader : IFileReader
{
    private WordprocessingDocument? _document;
    private OpenXmlElementList? _elementList;
    private int _elementIndex;

    public string FileExtension => ".docx";
    
    public void Dispose()
    {
        Dispose(true);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _document?.Dispose();
            _document = null;
            _elementList = null;
            _elementIndex = 0;
        }
    }

    public Result<Nothing> OpenFile(string filePath)
    {
        if (_document != null)
            return Result.Failure("File is already open");

        if (!Path.IsPathFullyQualified(filePath))
            return Result.Failure("path must be absolute");
        if (!Path.HasExtension(filePath) || !Path.GetExtension(filePath).Equals(FileExtension))
            return Result.Failure($"given path does not refer to a {FileExtension} file");

        try
        {
            _document = WordprocessingDocument.Open(filePath, false);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
        
        return Result.Success();
    }

    public Result<string> GetNextLine()
    {
        if (_document == null)
            return Result.FromError<string>("File is not open");
        _elementList ??= _document.MainDocumentPart?.Document.Body?.ChildElements;
        if (_elementList == null)
            return Result.FromError<string>("Document is empty");
        if (_elementIndex < _elementList.Value.Count)
        {
            var line = _elementList.Value[_elementIndex].InnerText;
            _elementIndex++;
            return Result.FromValue(line);
        }

        return Result.FromError<string>("Reached end of file");
    }
}