namespace SecurePasswordGenerator.Domain.Models;

/// <summary>
/// Represents a generated password with its metadata
/// Single Responsibility: Only holds generated password information
/// </summary>
public class GeneratedPassword
{
    /// <summary>
    /// Unique identifier for the password
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The generated password value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Strength analysis of the password
    /// </summary>
    public PasswordStrength Strength { get; set; } = new();

    /// <summary>
    /// Options used to generate this password
    /// </summary>
    public PasswordOptions Options { get; set; } = new();

    /// <summary>
    /// When the password was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional label/note for the password
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Whether this password is marked as favorite
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Type of password (Standard, Passphrase, Pattern)
    /// </summary>
    public PasswordType Type { get; set; } = PasswordType.Standard;

    /// <summary>
    /// Creates a copy without the actual password value (for safe logging)
    /// </summary>
    public GeneratedPassword CreateSafeClone()
    {
        return new GeneratedPassword
        {
            Id = Id,
            Value = "***REDACTED***",
            Strength = Strength,
            Options = Options.Clone(),
            GeneratedAt = GeneratedAt,
            Label = Label,
            IsFavorite = IsFavorite,
            Type = Type
        };
    }
}

/// <summary>
/// Types of passwords that can be generated
/// </summary>
public enum PasswordType
{
    /// <summary>
    /// Standard random password
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Passphrase (word-based password)
    /// </summary>
    Passphrase = 1,

    /// <summary>
    /// Pattern-based password (e.g., XXX-999-xxx)
    /// </summary>
    Pattern = 2,

    /// <summary>
    /// PIN (numbers only)
    /// </summary>
    Pin = 3
}
