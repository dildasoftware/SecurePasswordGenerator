using Microsoft.JSInterop;
using SecurePasswordGenerator.Domain.Interfaces;

namespace SecurePasswordGenerator.Infrastructure.Services;

/// <summary>
/// Clipboard (Pano) işlemleri servisi
/// 
/// SOLID Prensipleri:
/// - Single Responsibility: Sadece pano işlemlerinden sorumlu
/// - Dependency Inversion: IClipboardService interface'ini implement eder
/// 
/// JavaScript Clipboard API kullanarak panoya erişir
/// </summary>
public class ClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Constructor - Dependency Injection ile JSRuntime alır
    /// </summary>
    public ClipboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Metni panoya kopyalar
    /// </summary>
    /// <param name="text">Kopyalanacak metin</param>
    public async Task CopyToClipboardAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        try
        {
            var success = await _jsRuntime.InvokeAsync<bool>("copyToClipboard", text);
            
            if (!success)
            {
                throw new InvalidOperationException("Failed to copy to clipboard");
            }
        }
        catch (JSException ex)
        {
            Console.WriteLine($"JavaScript error copying to clipboard: {ex.Message}");
            throw new InvalidOperationException("Clipboard operation failed", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying to clipboard: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Panoyu temizler
    /// </summary>
    public async Task ClearClipboardAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("clearClipboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing clipboard: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Metni kopyalar ve belirtilen süre sonra otomatik olarak temizler
    /// </summary>
    /// <param name="text">Kopyalanacak metin</param>
    /// <param name="clearAfterSeconds">Temizleme süresi (saniye)</param>
    public async Task CopyWithAutoClearAsync(string text, int clearAfterSeconds = 30)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        if (clearAfterSeconds <= 0 || clearAfterSeconds > 300)
        {
            throw new ArgumentException("Clear time must be between 1 and 300 seconds", nameof(clearAfterSeconds));
        }

        try
        {
            var success = await _jsRuntime.InvokeAsync<bool>("copyWithAutoClear", text, clearAfterSeconds);
            
            if (!success)
            {
                throw new InvalidOperationException("Failed to copy with auto-clear");
            }

            Console.WriteLine($"Text copied. Will auto-clear in {clearAfterSeconds} seconds");
        }
        catch (JSException ex)
        {
            Console.WriteLine($"JavaScript error in copyWithAutoClear: {ex.Message}");
            throw new InvalidOperationException("Clipboard operation failed", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in copyWithAutoClear: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Otomatik temizleme işlemini iptal eder
    /// </summary>
    public async Task CancelAutoClearAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("cancelAutoClear");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling auto-clear: {ex.Message}");
            // Bu hata kritik değil, sessizce yut
        }
    }

    /// <summary>
    /// Pano iznini kontrol eder
    /// </summary>
    public async Task<bool> CheckPermissionAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("checkClipboardPermission");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking clipboard permission: {ex.Message}");
            // İzin kontrolü başarısız olursa, varsayılan olarak true döndür
            return true;
        }
    }

    /// <summary>
    /// Panodan okur (şifre gücü test etmek için)
    /// </summary>
    public async Task<string?> ReadFromClipboardAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("readFromClipboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from clipboard: {ex.Message}");
            return null;
        }
    }
}
