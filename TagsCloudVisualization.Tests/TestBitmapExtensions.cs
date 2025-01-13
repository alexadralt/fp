using System.Drawing;
using TagCloud.TagsCloudVisualization;

namespace TagsCloudVisualization.Tests;

internal static class TestBitmapExtensions
{
#pragma warning disable CA1416
    private static Bitmap DrawRectangles(this Bitmap bitmap, Rectangle[] rectangles, Pen pen)
    {
        var graphics = Graphics.FromImage(bitmap);
        graphics.DrawRectangles(pen, rectangles);
        graphics.Dispose();
        return bitmap;
    }

    private static Bitmap DrawEllipse(this Bitmap bitmap, Rectangle boundingRect, Pen pen)
    {
        var graphics = Graphics.FromImage(bitmap);
        graphics.DrawEllipse(pen, boundingRect);
        graphics.Dispose();
        return bitmap;
    }
    
    public static Bitmap DrawFailedTestImage(
        this Bitmap bitmap,
        Rectangle[] rectangles,
        Point centerPoint,
        Point barycenterPoint,
        int maxDistanceFromBarycenter,
        TestType testType)
    {
        bitmap.DrawRectangles(rectangles, new Pen(Color.Blue));

        if (testType != TestType.BarycenterTest)
            return bitmap;
        
        bitmap.DrawEllipse(
            new Rectangle(centerPoint.X, centerPoint.Y, maxDistanceFromBarycenter, maxDistanceFromBarycenter),
            new Pen(Color.Lime));
        bitmap.DrawEllipse(
            new Rectangle(barycenterPoint.X, barycenterPoint.Y, 1, 1),
            new Pen(Color.Red));
        return bitmap;
    }
#pragma warning restore CA1416
}