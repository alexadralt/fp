namespace TagCloud.Logger;

public interface ILogger
{
    public void Info(string message);
    public void Warning(string message);
    public void Error(string message);
    public void ReportProgress(string message, double progress);
}