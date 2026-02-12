using System;
using System.Threading.Tasks;

namespace SecurePasswordGenerator.Domain.Interfaces;

public interface ILocalizationService
{
    string this[string key] { get; }
    string CurrentCulture { get; }
    System.Globalization.CultureInfo CurrentCultureInfo { get; }
    Task InitializeAsync();
    Task SetLanguageAsync(string culture);
    event Action OnChange;
}
