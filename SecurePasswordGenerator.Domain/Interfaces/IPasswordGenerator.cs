using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Domain.Interfaces;

/// <summary>
/// Interface for password generation
/// Interface Segregation Principle: Focused on password generation only
/// Dependency Inversion Principle: High-level modules depend on this abstraction
/// </summary>
public interface IPasswordGenerator
{
    /// <summary>
    /// Generates a password based on the provided options
    /// </summary>
    /// <param name="options">Password generation options</param>
    /// <returns>Generated password</returns>
    GeneratedPassword Generate(PasswordOptions options);

    /// <summary>
    /// Generates multiple passwords
    /// </summary>
    /// <param name="options">Password generation options</param>
    /// <param name="count">Number of passwords to generate</param>
    /// <returns>List of generated passwords</returns>
    List<GeneratedPassword> GenerateBulk(PasswordOptions options, int count);

    /// <summary>
    /// Generates a password from a pattern (e.g., "XXX-999-xxx")
    /// </summary>
    /// <param name="pattern">Pattern string</param>
    /// <returns>Generated password</returns>
    GeneratedPassword GenerateFromPattern(string pattern);
}
