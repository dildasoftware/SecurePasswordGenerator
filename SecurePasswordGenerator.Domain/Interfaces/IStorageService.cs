using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Domain.Interfaces;

/// <summary>
/// Interface for password storage operations
/// Interface Segregation Principle: Focused on storage only
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Saves a password to history
    /// </summary>
    /// <param name="password">Password to save</param>
    /// <returns>Task representing the async operation</returns>
    Task SavePasswordAsync(PasswordHistory password);

    /// <summary>
    /// Gets all passwords from history
    /// </summary>
    /// <returns>List of password history entries</returns>
    Task<List<PasswordHistory>> GetAllPasswordsAsync();

    /// <summary>
    /// Gets a password by ID
    /// </summary>
    /// <param name="id">Password ID</param>
    /// <returns>Password history entry or null</returns>
    Task<PasswordHistory?> GetPasswordByIdAsync(Guid id);

    /// <summary>
    /// Deletes a password from history
    /// </summary>
    /// <param name="id">Password ID</param>
    /// <returns>Task representing the async operation</returns>
    Task DeletePasswordAsync(Guid id);

    /// <summary>
    /// Clears all password history
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task ClearAllAsync();

    /// <summary>
    /// Updates a password in history
    /// </summary>
    /// <param name="password">Password to update</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdatePasswordAsync(PasswordHistory password);

    /// <summary>
    /// Exports password history
    /// </summary>
    /// <returns>JSON string of password history</returns>
    Task<string> ExportAsync();

    /// <summary>
    /// Imports password history
    /// </summary>
    /// <param name="json">JSON string to import</param>
    /// <returns>Task representing the async operation</returns>
    Task ImportAsync(string json);
}
