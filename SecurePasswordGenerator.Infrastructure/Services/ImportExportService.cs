using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;
using SecurePasswordGenerator.Domain.Interfaces;
using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Infrastructure.Services;

public class ImportExportService : IImportExportService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IPasswordHistoryService _historyService;

    public ImportExportService(IJSRuntime jsRuntime, IPasswordHistoryService historyService)
    {
        _jsRuntime = jsRuntime;
        _historyService = historyService;
    }

    public async Task<string> ExportToJsonAsync()
    {
        // Use JS export for consistency with IndexedDB structure
        return await _jsRuntime.InvokeAsync<string>("exportPasswords");
    }

    public async Task<string> ExportToCsvAsync()
    {
        var passwords = await _historyService.GetAllPasswordsAsync();
        var sb = new StringBuilder();
        // BOM for Excel compatibility
        sb.Append("\uFEFF");
        sb.AppendLine("Label,Password,Strength,Tags,Created Date");

        foreach (var p in passwords)
        {
            var tags = string.Join(";", p.Tags).Replace("\"", "\"\"");
            var label = (p.Label ?? "").Replace("\"", "\"\"");
            // Password might contain comma or quotes
            var pass = p.Password.Replace("\"", "\"\"");
            
            sb.AppendLine($"\"{label}\",\"{pass}\",\"{p.StrengthLevel}\",\"{tags}\",\"{p.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
        }
        return sb.ToString();
    }

    public async Task<int> ImportFromJsonAsync(string jsonContent)
    {
        try
        {
            // Validate JSON first
            JsonSerializer.Deserialize<List<PasswordHistory>>(jsonContent);
            
            // Use JS import logic which handles transaction
            return await _jsRuntime.InvokeAsync<int>("importPasswords", jsonContent);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error importing JSON: {ex.Message}");
            throw;
        }
    }

    public async Task DownloadFileAsync(string fileName, string content, string contentType)
    {
        await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, content, contentType);
    }
}
