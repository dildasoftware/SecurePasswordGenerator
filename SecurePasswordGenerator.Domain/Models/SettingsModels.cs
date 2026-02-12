namespace SecurePasswordGenerator.Domain.Models;

public enum ExportFormat
{
    JSON,
    CSV,
    TXT
}

public class ImportResult
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

public enum ThemeMode
{
    Dark,
    Light,
    Auto
}

public enum Language
{
    English,
    Turkish
}
