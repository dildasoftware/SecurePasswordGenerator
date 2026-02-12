using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Domain.Interfaces;

public interface IImportExportService
{
    Task<string> ExportToJsonAsync();
    Task<string> ExportToCsvAsync();
    Task<int> ImportFromJsonAsync(string jsonContent);
    Task DownloadFileAsync(string fileName, string content, string contentType);
}
