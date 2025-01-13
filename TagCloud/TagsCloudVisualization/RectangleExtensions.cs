using System.Drawing;

namespace TagCloud.TagsCloudVisualization;

public static class RectangleExtensions
{
    public static bool ContainsFloat(this Rectangle rectangle, PointF point)
    {
        return rectangle.X <= point.X
               && point.X < rectangle.Right
               && rectangle.Y <= point.Y
               && point.Y < rectangle.Bottom;
    }
}