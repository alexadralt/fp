using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

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

    public void OpenFile(string filePath)
    {
        if (_document != null)
            throw new InvalidOperationException("File is already open");
        ArgumentNullException.ThrowIfNull(filePath);
        
        if (!Path.IsPathFullyQualified(filePath))
            throw new ArgumentException("path must be absolute");
        if (!Path.HasExtension(filePath) || !Path.GetExtension(filePath).Equals(FileExtension))
            throw new ArgumentException($"given path does not refer to a {FileExtension} file");
        if (!Path.Exists(filePath))
            throw new FileNotFoundException($"file not found {filePath}");
        
        _document = WordprocessingDocument.Open(filePath, false);
    }

    public bool TryGetNextLine(out string line)
    {
        line = String.Empty;
        if (_document == null)
            throw new InvalidOperationException("File is not open");
        _elementList ??= _document.MainDocumentPart?.Document.Body?.ChildElements;
        if (_elementList == null)
            return false;
        if (_elementIndex < _elementList.Value.Count)
        {
            line = _elementList.Value[_elementIndex].InnerText;
            _elementIndex++;
            return true;
        }

        return false;
    }
}