using SecurePasswordGenerator.Domain.Interfaces;
using SecurePasswordGenerator.Domain.Models;
using Microsoft.JSInterop;

namespace SecurePasswordGenerator.Infrastructure.Services;

/// <summary>
/// Implementation of password history service using IndexedDB
/// Single Responsibility: Manages password history operations
/// </summary>
public class PasswordHistoryService : IPasswordHistoryService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IStorageService _storageService;

    public PasswordHistoryService(IJSRuntime jsRuntime, IStorageService storageService)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    }

    public async Task<List<PasswordHistory>> GetAllPasswordsAsync()
    {
        try
        {
            return await _storageService.GetAllPasswordsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting all passwords: {ex.Message}");
            return new List<PasswordHistory>();
        }
    }

    public async Task<List<PasswordHistory>> SearchPasswordsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllPasswordsAsync();
        }

        try
        {
            var allPasswords = await GetAllPasswordsAsync();
            var lowerQuery = query.ToLowerInvariant();

            return allPasswords
                .Where(p =>
                    (p.Label?.ToLowerInvariant().Contains(lowerQuery) ?? false) ||
                    p.Tags.Any(t => t.ToLowerInvariant().Contains(lowerQuery)) ||
                    p.Password.Contains(query)) // Exact match for password
                .ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching passwords: {ex.Message}");
            return new List<PasswordHistory>();
        }
    }

    public async Task<List<PasswordHistory>> FilterByDateAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var allPasswords = await GetAllPasswordsAsync();

            return allPasswords
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error filtering by date: {ex.Message}");
            return new List<PasswordHistory>();
        }
    }

    public async Task<List<PasswordHistory>> FilterByStrengthAsync(PasswordStrengthLevel level)
    {
        try
        {
            var allPasswords = await GetAllPasswordsAsync();

            return allPasswords
                .Where(p => p.StrengthLevel == level)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error filtering by strength: {ex.Message}");
            return new List<PasswordHistory>();
        }
    }

    public async Task<bool> ToggleFavoriteAsync(Guid id)
    {
        try
        {
            var allPasswords = await GetAllPasswordsAsync();
            var password = allPasswords.FirstOrDefault(p => p.Id == id);

            if (password == null)
            {
                return false;
            }

            password.IsFavorite = !password.IsFavorite;

            // Update in storage
            await _jsRuntime.InvokeVoidAsync("updatePasswordInDB", id.ToString(), password);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error toggling favorite: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeletePasswordAsync(Guid id)
    {
        try
        {
            await _storageService.DeletePasswordAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting password: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteAllPasswordsAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("clearAllPasswordsFromDB");
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting all passwords: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateTagsAsync(Guid id, List<string> tags)
    {
        try
        {
            var allPasswords = await GetAllPasswordsAsync();
            var password = allPasswords.FirstOrDefault(p => p.Id == id);

            if (password == null)
            {
                return false;
            }

            password.Tags = tags ?? new List<string>();

            await _jsRuntime.InvokeVoidAsync("updatePasswordInDB", id.ToString(), password);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating tags: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateLabelAsync(Guid id, string label)
    {
        try
        {
            var allPasswords = await GetAllPasswordsAsync();
            var password = allPasswords.FirstOrDefault(p => p.Id == id);

            if (password == null)
            {
                return false;
            }

            password.Label = label;

            await _jsRuntime.InvokeVoidAsync("updatePasswordInDB", id.ToString(), password);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating label: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RecordAccessAsync(Guid id)
    {
        try
        {
            var allPasswords = await GetAllPasswordsAsync();
            var password = allPasswords.FirstOrDefault(p => p.Id == id);

            if (password == null)
            {
                return false;
            }

            password.CopyCount++;
            password.LastAccessedAt = DateTime.UtcNow;

            await _jsRuntime.InvokeVoidAsync("updatePasswordInDB", id.ToString(), password);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error recording access: {ex.Message}");
            return false;
        }
    }

    public async Task<PasswordHistoryStatistics> GetStatisticsAsync()
    {
        try
        {
            var allPasswords = await GetAllPasswordsAsync();

            var stats = new PasswordHistoryStatistics
            {
                TotalPasswords = allPasswords.Count,
                FavoriteCount = allPasswords.Count(p => p.IsFavorite),
                StrengthDistribution = allPasswords
                    .GroupBy(p => p.StrengthLevel)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TypeDistribution = allPasswords
                    .GroupBy(p => p.Type)
                    .ToDictionary(g => g.Key, g => g.Count()),
                OldestPasswordDate = allPasswords.Any() ? allPasswords.Min(p => p.CreatedAt) : null,
                NewestPasswordDate = allPasswords.Any() ? allPasswords.Max(p => p.CreatedAt) : null,
                TotalCopyCount = allPasswords.Sum(p => p.CopyCount)
            };

            // Estimate storage size (rough calculation)
            stats.StorageSizeBytes = allPasswords.Sum(p =>
                p.Password.Length * 2 + // UTF-16 encoding
                (p.Label?.Length ?? 0) * 2 +
                p.Tags.Sum(t => t.Length * 2) +
                100 // Metadata overhead
            );

            return stats;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting statistics: {ex.Message}");
            return new PasswordHistoryStatistics();
        }
    }
}
