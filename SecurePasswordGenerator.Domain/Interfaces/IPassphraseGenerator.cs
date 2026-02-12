using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Domain.Interfaces;

/// <summary>
/// Interface for passphrase generation
/// Interface Segregation Principle: Separate from password generation
/// </summary>
public interface IPassphraseGenerator
{
    /// <summary>
    /// Generates a passphrase based on the provided options
    /// </summary>
    /// <param name="options">Passphrase generation options</param>
    /// <returns>Generated passphrase</returns>
    Task<Passphrase> GenerateAsync(PassphraseOptions options);
}
