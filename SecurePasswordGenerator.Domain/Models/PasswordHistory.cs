namespace SecurePasswordGenerator.Domain.Models;

/// <summary>
/// Represents a password history entry for storage
/// Single Responsibility: Only holds password history data
/// </summary>
public class PasswordHistory
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The password value (should be encrypted in storage)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// When the password was generated
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional label/description
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Password length
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Password strength level
    /// </summary>
    public PasswordStrengthLevel StrengthLevel { get; set; }

    /// <summary>
    /// Entropy value
    /// </summary>
    public double Entropy { get; set; }

    /// <summary>
    /// Password type
    /// </summary>
    public PasswordType Type { get; set; }

    /// <summary>
    /// Whether marked as favorite
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Number of times this password was copied
    /// </summary>
    public int CopyCount { get; set; }

    /// <summary>
    /// Last time this password was accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// Tags for categorization and search
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Creates a GeneratedPassword from this history entry
    /// </summary>
    public GeneratedPassword ToGeneratedPassword()
    {
        return new GeneratedPassword
        {
            Id = Id,
            Value = Password,
            GeneratedAt = CreatedAt,
            Label = Label,
            IsFavorite = IsFavorite,
            Type = Type,
            Strength = new PasswordStrength
            {
                Password = Password,
                Level = StrengthLevel,
                Entropy = Entropy,
                Score = CalculateScore()
            }
        };
    }

    /// <summary>
    /// Calculates score from strength level
    /// </summary>
    private int CalculateScore()
    {
        return StrengthLevel switch
        {
            PasswordStrengthLevel.VeryWeak => 20,
            PasswordStrengthLevel.Weak => 40,
            PasswordStrengthLevel.Moderate => 60,
            PasswordStrengthLevel.Strong => 80,
            PasswordStrengthLevel.VeryStrong => 100,
            _ => 0
        };
    }
}
