using System.Drawing;
using TagCloud.WordStatistics;

namespace TagCloud.WordRenderer;

public interface IWordRenderer
{
    public Bitmap Render();
    public IWordStatistics WordStatistics { get; }
}