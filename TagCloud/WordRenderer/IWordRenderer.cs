using System.Drawing;
using TagCloud.ResultUtils;
using TagCloud.WordStatistics;

namespace TagCloud.WordRenderer;

public interface IWordRenderer
{
    public Result<Bitmap> Render();
    public IWordStatistics WordStatistics { get; }
}