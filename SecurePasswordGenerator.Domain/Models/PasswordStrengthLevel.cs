namespace SecurePasswordGenerator.Domain.Models;

/// <summary>
/// Represents the strength level of a password
/// </summary>
public enum PasswordStrengthLevel
{
    /// <summary>
    /// Very weak password (entropy < 28 bits)
    /// </summary>
    VeryWeak = 0,

    /// <summary>
    /// Weak password (entropy 28-35 bits)
    /// </summary>
    Weak = 1,

    /// <summary>
    /// Moderate password (entropy 36-59 bits)
    /// </summary>
    Moderate = 2,

    /// <summary>
    /// Strong password (entropy 60-127 bits)
    /// </summary>
    Strong = 3,

    /// <summary>
    /// Very strong password (entropy >= 128 bits)
    /// </summary>
    VeryStrong = 4
}
