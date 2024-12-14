using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TagCloud.FileReader;
using VerifyNUnit;

namespace DocxFileReader.Tests.FileReader;

[TestFixture]
[TestOf(typeof(TagCloud.FileReader.DocxFileReader))]
public class DocxFileReaderTests
{
    private TagCloud.FileReader.DocxFileReader _docxFileReader;
    private StringBuilder _stringBuilder;

    [OneTimeSetUp]
    public void Initialize()
    {
        _docxFileReader = new TagCloud.FileReader.DocxFileReader();
    }

    [SetUp]
    public void SetUp()
    {
        _stringBuilder = new StringBuilder();
    }

    [TearDown]
    public void TearDown()
    {
        _docxFileReader.Dispose();
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForBasicFile()
    {
        var inputFile = "./../../../BasicFile.docx";
        _docxFileReader.OpenFile(Path.GetFullPath(inputFile));

        while (_docxFileReader.TryGetNextLine(out var line))
            _stringBuilder.AppendLine(line);
        
        return Verifier.Verify(_stringBuilder.ToString());
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForFileWithHeaders()
    {
        var inputFile = "./../../../FileWithHeaders.docx";
        _docxFileReader.OpenFile(Path.GetFullPath(inputFile));

        while (_docxFileReader.TryGetNextLine(out var line))
            _stringBuilder.AppendLine(line);
        
        return Verifier.Verify(_stringBuilder.ToString());
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForFileWithBulletList()
    {
        var inputFile = "./../../../FileWithBulletList.docx";
        _docxFileReader.OpenFile(Path.GetFullPath(inputFile));

        while (_docxFileReader.TryGetNextLine(out var line))
            _stringBuilder.AppendLine(line);
        
        return Verifier.Verify(_stringBuilder.ToString());
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForFileWithNumberedList()
    {
        var inputFile = "./../../../FileWithNumberedList.docx";
        _docxFileReader.OpenFile(Path.GetFullPath(inputFile));

        while (_docxFileReader.TryGetNextLine(out var line))
            _stringBuilder.AppendLine(line);
        
        return Verifier.Verify(_stringBuilder.ToString());
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForBigFile()
    {
        var inputFile = "./../../../BigFile.docx";
        _docxFileReader.OpenFile(Path.GetFullPath(inputFile));

        while (_docxFileReader.TryGetNextLine(out var line))
            _stringBuilder.AppendLine(line);
        
        return Verifier.Verify(_stringBuilder.ToString());
    }
}