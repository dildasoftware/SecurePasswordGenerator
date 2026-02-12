namespace SecurePasswordGenerator.Domain.Models;

/// <summary>
/// Represents options for passphrase generation
/// Single Responsibility: Only holds passphrase configuration
/// </summary>
public class PassphraseOptions
{
    /// <summary>
    /// Number of words in the passphrase (3-10)
    /// </summary>
    public int WordCount { get; set; } = 4;

    /// <summary>
    /// Separator between words (space, dash, underscore, etc.)
    /// </summary>
    public string Separator { get; set; } = "-";

    /// <summary>
    /// Capitalize first letter of each word
    /// </summary>
    public bool Capitalize { get; set; } = true;

    /// <summary>
    /// Include a number at the end
    /// </summary>
    public bool IncludeNumber { get; set; } = true;

    /// <summary>
    /// Include a special character at the end
    /// </summary>
    public bool IncludeSymbol { get; set; } = false;

    /// <summary>
    /// Minimum word length
    /// </summary>
    public int MinWordLength { get; set; } = 4;

    /// <summary>
    /// Maximum word length
    /// </summary>
    public int MaxWordLength { get; set; } = 8;
    
    /// <summary>
    /// Language for word list
    /// </summary>
    public Language Language { get; set; } = Language.English;

    /// <summary>
    /// Validates the passphrase options
    /// </summary>
    public bool IsValid()
    {
        return WordCount >= 3 && WordCount <= 10 &&
               MinWordLength >= 3 && MinWordLength <= MaxWordLength &&
               MaxWordLength <= 12;
    }
}

/// <summary>
/// Represents a generated passphrase
/// </summary>
public class Passphrase
{
    /// <summary>
    /// The generated passphrase value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Individual words in the passphrase
    /// </summary>
    public List<string> Words { get; set; } = new();

    /// <summary>
    /// Options used to generate this passphrase
    /// </summary>
    public PassphraseOptions Options { get; set; } = new();

    /// <summary>
    /// Strength analysis
    /// </summary>
    public PasswordStrength Strength { get; set; } = new();

    /// <summary>
    /// When the passphrase was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
