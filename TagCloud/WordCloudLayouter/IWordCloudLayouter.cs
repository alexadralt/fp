using System.Drawing;
using TagCloud.WordStatistics;

namespace TagCloud.WordCloudLayouter;

public interface IWordCloudLayouter
{
    public IEnumerable<WordLayoutInfo> GetWordCloudLayout(Func<string, Font, SizeF> stringMeasure);
    public IWordStatistics WordStatistics { get; }
}