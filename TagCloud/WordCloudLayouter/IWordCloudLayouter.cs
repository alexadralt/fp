using System.Drawing;
using TagCloud.ResultUtils;
using TagCloud.WordStatistics;

namespace TagCloud.WordCloudLayouter;

public interface IWordCloudLayouter
{
    public IEnumerable<Result<WordLayoutInfo>> GetWordCloudLayout(Func<string, Font, SizeF> stringMeasure);
    public IWordStatistics WordStatistics { get; }
}