using Microsoft.JSInterop;
using SecurePasswordGenerator.Domain.Interfaces;
using SecurePasswordGenerator.Domain.Models;
using System.Text.Json;

namespace SecurePasswordGenerator.Infrastructure.Services;

/// <summary>
/// IndexedDB ile etkileşim servisi
/// 
/// SOLID Prensipleri:
/// - Single Responsibility: Sadece IndexedDB işlemlerinden sorumlu
/// - Dependency Inversion: IStorageService interface'ini implement eder
/// 
/// JavaScript Interop kullanarak tarayıcının IndexedDB'sine erişir
/// </summary>
public class IndexedDbService : IStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized = false;

    /// <summary>
    /// Constructor - Dependency Injection ile JSRuntime alır
    /// </summary>
    public IndexedDbService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Veritabanını başlatır (ilk kullanımda)
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("initializeDb");
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndexedDB initialization error: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Şifreyi geçmişe kaydeder
    /// </summary>
    public async Task SavePasswordAsync(PasswordHistory password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        await EnsureInitializedAsync();

        try
        {
            // C# nesnesini JavaScript'e gönder
            await _jsRuntime.InvokeVoidAsync("savePassword", password);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving password: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Tüm şifre geçmişini getirir
    /// </summary>
    public async Task<List<PasswordHistory>> GetAllPasswordsAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            // JavaScript'ten veri al ve C# nesnesine dönüştür
            var result = await _jsRuntime.InvokeAsync<List<PasswordHistory>>("getAllPasswords");
            return result ?? new List<PasswordHistory>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting passwords: {ex.Message}");
            return new List<PasswordHistory>();
        }
    }

    /// <summary>
    /// ID'ye göre şifre getirir
    /// </summary>
    public async Task<PasswordHistory?> GetPasswordByIdAsync(Guid id)
    {
        await EnsureInitializedAsync();

        try
        {
            var result = await _jsRuntime.InvokeAsync<PasswordHistory?>("getPasswordById", id.ToString());
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting password by ID: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Şifreyi günceller
    /// </summary>
    public async Task UpdatePasswordAsync(PasswordHistory password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        await EnsureInitializedAsync();

        try
        {
            await _jsRuntime.InvokeVoidAsync("updatePassword", password);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating password: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Şifreyi siler
    /// </summary>
    public async Task DeletePasswordAsync(Guid id)
    {
        await EnsureInitializedAsync();

        try
        {
            await _jsRuntime.InvokeVoidAsync("deletePassword", id.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting password: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Tüm şifre geçmişini temizler
    /// </summary>
    public async Task ClearAllAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            await _jsRuntime.InvokeVoidAsync("clearAllPasswords");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing passwords: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Şifre geçmişini JSON olarak dışa aktarır
    /// </summary>
    public async Task<string> ExportAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("exportPasswords");
            return json ?? "[]";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting passwords: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// JSON'dan şifre geçmişini içe aktarır
    /// </summary>
    public async Task ImportAsync(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON data cannot be empty", nameof(json));
        }

        await EnsureInitializedAsync();

        try
        {
            // JSON'u doğrula
            var passwords = JsonSerializer.Deserialize<List<PasswordHistory>>(json);
            
            if (passwords == null)
            {
                throw new InvalidOperationException("Invalid JSON format");
            }

            // JavaScript'e gönder
            await _jsRuntime.InvokeVoidAsync("importPasswords", json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing error: {ex.Message}");
            throw new InvalidOperationException("Invalid JSON format", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing passwords: {ex.Message}");
            throw;
        }
    }
}
