using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

namespace DocxFileReader.Tests.FileReader;

[TestFixture]
[TestOf(typeof(TagCloud.FileReader.DocxFileReader))]
public class DocxFileReaderTests
{
    private TagCloud.FileReader.DocxFileReader _docxFileReader;

    [OneTimeSetUp]
    public void Initialize()
    {
        _docxFileReader = new TagCloud.FileReader.DocxFileReader();
    }

    private Task VerifyResult(string inputFile)
    {
        var sb = _docxFileReader.ReadAllLines(Path.GetFullPath(inputFile))
            .Aggregate(new StringBuilder(), (sb, line) => sb.AppendLine(line.Value));
        
        return Verifier.Verify(sb.ToString());
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForBasicFile()
    {
        var inputFile = "./../../../BasicFile.docx";

        return VerifyResult(inputFile);
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForFileWithHeaders()
    {
        var inputFile = "./../../../FileWithHeaders.docx";
        
        return VerifyResult(inputFile);
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForFileWithBulletList()
    {
        var inputFile = "./../../../FileWithBulletList.docx";
        
        return VerifyResult(inputFile);
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForFileWithNumberedList()
    {
        var inputFile = "./../../../FileWithNumberedList.docx";
        
        return VerifyResult(inputFile);
    }

    [Test]
    public Task TryGetNextLine_ReturnsCorrectResult_ForBigFile()
    {
        var inputFile = "./../../../BigFile.docx";
        
        return VerifyResult(inputFile);
    }
}