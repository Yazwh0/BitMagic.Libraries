namespace BitMagic.Common;

public interface IEmulatorLogger
{
    public void Log(string message);
    public void LogLine(string message);
    public void LogError(string message);
    public void LogError(string message, ISourceFile source, int lineNumber);
}
