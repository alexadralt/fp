using System.Drawing;

namespace TagCloud.TagsCloudVisualization;

public interface ICircularCloudLayouter
{
    public Rectangle PutNextRectangle(Size rectangleSize);
    public Point CloudCenter { get; set; }
    public IEnumerable<Rectangle> Layout { get; }
}