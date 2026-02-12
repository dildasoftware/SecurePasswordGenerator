namespace SecurePasswordGenerator.Domain.Models;

/// <summary>
/// Represents the strength level of a password
/// </summary>
public enum PasswordStrengthLevel
{
    TooShort = 0,
    VeryWeak = 1,
    Weak = 2,
    Medium = 3,
    Strong = 4,
    VeryStrong = 5
}
