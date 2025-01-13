using System.Drawing;
using TagCloud.ResultUtils;

namespace TagCloud.TagsCloudVisualization;

public interface ICircularCloudLayouter
{
    public Result<Rectangle> PutNextRectangle(Size rectangleSize);
    public IEnumerable<Rectangle> Layout { get; }
}