using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SecurePasswordGenerator.Domain.Interfaces;

namespace SecurePasswordGenerator.Web.Services;

public class ClientLocalizationService : ILocalizationService
{
    private readonly HttpClient _httpClient;
    private Dictionary<string, string> _translations = new();
    
    public event Action? OnChange;
    public string CurrentCulture { get; private set; } = "tr"; // Varsayılan Türkçe

    public ClientLocalizationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string this[string key]
    {
        get
        {
            if (_translations.TryGetValue(key, out var value))
                return value;
            return key; // Bulunamazsa key döner
        }
    }
    
    public CultureInfo CurrentCultureInfo => new CultureInfo(CurrentCulture);

    public async Task InitializeAsync()
    {
        await LoadLanguageAsync(CurrentCulture);
    }
    
    public async Task SetLanguageAsync(string culture)
    {
        if (CurrentCulture != culture)
        {
            CurrentCulture = culture;
            await LoadLanguageAsync(culture);
            OnChange?.Invoke(); // UI güncellemesini tetikle
        }
    }
    
    private async Task LoadLanguageAsync(string culture)
    {
        try
        {
            var fileName = culture == "tr" ? "locales/tr.json" : "locales/en.json";
            _translations = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>(fileName) ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dil dosyası yüklenirken hata: {culture} - {ex.Message}");
        }
    }
}
