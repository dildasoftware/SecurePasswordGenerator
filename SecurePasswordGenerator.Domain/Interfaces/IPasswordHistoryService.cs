using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Domain.Interfaces;

/// <summary>
/// Service for managing password history
/// Interface Segregation Principle: Specific interface for history operations
/// </summary>
public interface IPasswordHistoryService
{
    /// <summary>
    /// Retrieves all saved passwords from history
    /// </summary>
    Task<List<PasswordHistory>> GetAllPasswordsAsync();

    /// <summary>
    /// Searches passwords by query (label, tags)
    /// </summary>
    Task<List<PasswordHistory>> SearchPasswordsAsync(string query);

    /// <summary>
    /// Filters passwords by date range
    /// </summary>
    Task<List<PasswordHistory>> FilterByDateAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Filters passwords by strength level
    /// </summary>
    Task<List<PasswordHistory>> FilterByStrengthAsync(PasswordStrengthLevel level);

    /// <summary>
    /// Toggles favorite status of a password
    /// </summary>
    Task<bool> ToggleFavoriteAsync(Guid id);

    /// <summary>
    /// Deletes a specific password from history
    /// </summary>
    Task<bool> DeletePasswordAsync(Guid id);

    /// <summary>
    /// Deletes all passwords from history
    /// </summary>
    Task<bool> DeleteAllPasswordsAsync();

    /// <summary>
    /// Updates tags for a password
    /// </summary>
    Task<bool> UpdateTagsAsync(Guid id, List<string> tags);

    /// <summary>
    /// Updates label for a password
    /// </summary>
    Task<bool> UpdateLabelAsync(Guid id, string label);

    /// <summary>
    /// Increments copy count and updates last accessed time
    /// </summary>
    Task<bool> RecordAccessAsync(Guid id);

    /// <summary>
    /// Gets statistics about password history
    /// </summary>
    Task<PasswordHistoryStatistics> GetStatisticsAsync();
}

/// <summary>
/// Statistics about password history
/// </summary>
public class PasswordHistoryStatistics
{
    public int TotalPasswords { get; set; }
    public int FavoriteCount { get; set; }
    public Dictionary<PasswordStrengthLevel, int> StrengthDistribution { get; set; } = new();
    public Dictionary<PasswordType, int> TypeDistribution { get; set; } = new();
    public long StorageSizeBytes { get; set; }
    public DateTime? OldestPasswordDate { get; set; }
    public DateTime? NewestPasswordDate { get; set; }
    public int TotalCopyCount { get; set; }
}
