using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SecurePasswordGenerator.Web;
using SecurePasswordGenerator.Domain.Interfaces;
using SecurePasswordGenerator.Application.Services;
using SecurePasswordGenerator.Infrastructure.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient servisi
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// ===== DEPENDENCY INJECTION CONFIGURATION =====
// SOLID Prensipleri: Dependency Inversion - Interface'lere bağımlılık

// Infrastructure Layer (Altyapı Servisleri)
builder.Services.AddScoped<ICryptoRandomService, CryptoRandomService>();
builder.Services.AddScoped<IStorageService, IndexedDbService>();
builder.Services.AddScoped<IClipboardService, ClipboardService>();
builder.Services.AddScoped<IPasswordHistoryService, PasswordHistoryService>();
builder.Services.AddScoped<IImportExportService, ImportExportService>();

// Application Layer (İş Mantığı Servisleri)
builder.Services.AddScoped<IPasswordGenerator, PasswordGeneratorService>();
builder.Services.AddScoped<IPasswordAnalyzer, PasswordStrengthAnalyzer>();
builder.Services.AddScoped<IPassphraseGenerator, PassphraseGeneratorService>();

// Logging (Konsol loglama)
builder.Logging.SetMinimumLevel(LogLevel.Information);

await builder.Build().RunAsync();
