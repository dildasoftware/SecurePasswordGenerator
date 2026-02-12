namespace SecurePasswordGenerator.Domain.Models;

/// <summary>
/// Represents the strength analysis of a password
/// Single Responsibility: Only holds password strength information
/// </summary>
public class PasswordStrength
{
    /// <summary>
    /// The password being analyzed
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Strength level of the password
    /// </summary>
    public PasswordStrengthLevel Level { get; set; }

    /// <summary>
    /// Entropy in bits (measure of randomness)
    /// </summary>
    public double Entropy { get; set; }

    /// <summary>
    /// Score from 0-100
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Estimated time to crack (human-readable)
    /// </summary>
    public string TimeToCrack { get; set; } = string.Empty;

    /// <summary>
    /// Whether the password contains uppercase letters
    /// </summary>
    public bool HasUppercase { get; set; }

    /// <summary>
    /// Whether the password contains lowercase letters
    /// </summary>
    public bool HasLowercase { get; set; }

    /// <summary>
    /// Whether the password contains numbers
    /// </summary>
    public bool HasNumbers { get; set; }

    /// <summary>
    /// Whether the password contains symbols
    /// </summary>
    public bool HasSymbols { get; set; }

    /// <summary>
    /// Number of unique characters in the password
    /// </summary>
    public int UniqueCharacters { get; set; }

    /// <summary>
    /// Feedback messages for improving password strength
    /// </summary>
    public List<string> Feedback { get; set; } = new();

    /// <summary>
    /// Warnings about password weaknesses
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Gets a color representation for the strength level
    /// </summary>
    public string GetColorCode()
    {
        return Level switch
        {
            PasswordStrengthLevel.VeryWeak => "#ef4444", // Red
            PasswordStrengthLevel.Weak => "#f59e0b", // Orange
            PasswordStrengthLevel.Moderate => "#eab308", // Yellow
            PasswordStrengthLevel.Strong => "#22c55e", // Green
            PasswordStrengthLevel.VeryStrong => "#10b981", // Emerald
            _ => "#6b7280" // Gray
        };
    }

    /// <summary>
    /// Gets a text description of the strength level
    /// </summary>
    public string GetStrengthText()
    {
        return Level switch
        {
            PasswordStrengthLevel.VeryWeak => "Very Weak",
            PasswordStrengthLevel.Weak => "Weak",
            PasswordStrengthLevel.Moderate => "Moderate",
            PasswordStrengthLevel.Strong => "Strong",
            PasswordStrengthLevel.VeryStrong => "Very Strong",
            _ => "Unknown"
        };
    }
}
