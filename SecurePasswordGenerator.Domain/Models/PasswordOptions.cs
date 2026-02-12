namespace SecurePasswordGenerator.Domain.Models;

/// <summary>
/// Represents options for password generation
/// Single Responsibility: Only holds password generation configuration
/// </summary>
public class PasswordOptions
{
    /// <summary>
    /// Length of the password to generate (4-128 characters)
    /// </summary>
    public int Length { get; set; } = 16;

    /// <summary>
    /// Include uppercase letters (A-Z)
    /// </summary>
    public bool IncludeUppercase { get; set; } = true;

    /// <summary>
    /// Include lowercase letters (a-z)
    /// </summary>
    public bool IncludeLowercase { get; set; } = true;

    /// <summary>
    /// Include numbers (0-9)
    /// </summary>
    public bool IncludeNumbers { get; set; } = true;

    /// <summary>
    /// Include special characters (!@#$%^&*...)
    /// </summary>
    public bool IncludeSymbols { get; set; } = true;

    /// <summary>
    /// Exclude similar characters (i, l, 1, L, o, 0, O)
    /// </summary>
    public bool ExcludeSimilarCharacters { get; set; } = false;

    /// <summary>
    /// Exclude ambiguous characters ({, }, [, ], (, ), /, \, ', ", `, ~, ,, ;, :, ., <, >)
    /// </summary>
    public bool ExcludeAmbiguousCharacters { get; set; } = false;

    /// <summary>
    /// Custom character set (overrides other options if provided)
    /// </summary>
    public string? CustomCharacterSet { get; set; }

    /// <summary>
    /// Minimum number of uppercase letters required
    /// </summary>
    public int MinUppercase { get; set; } = 0;

    /// <summary>
    /// Minimum number of lowercase letters required
    /// </summary>
    public int MinLowercase { get; set; } = 0;

    /// <summary>
    /// Minimum number of numbers required
    /// </summary>
    public int MinNumbers { get; set; } = 0;

    /// <summary>
    /// Minimum number of symbols required
    /// </summary>
    public int MinSymbols { get; set; } = 0;

    /// <summary>
    /// Validates that at least one character type is selected
    /// </summary>
    public bool IsValid()
    {
        if (!string.IsNullOrEmpty(CustomCharacterSet))
        {
            return CustomCharacterSet.Length > 0;
        }

        return IncludeUppercase || IncludeLowercase || IncludeNumbers || IncludeSymbols;
    }

    /// <summary>
    /// Creates a copy of the current options
    /// </summary>
    public PasswordOptions Clone()
    {
        return new PasswordOptions
        {
            Length = Length,
            IncludeUppercase = IncludeUppercase,
            IncludeLowercase = IncludeLowercase,
            IncludeNumbers = IncludeNumbers,
            IncludeSymbols = IncludeSymbols,
            ExcludeSimilarCharacters = ExcludeSimilarCharacters,
            ExcludeAmbiguousCharacters = ExcludeAmbiguousCharacters,
            CustomCharacterSet = CustomCharacterSet,
            MinUppercase = MinUppercase,
            MinLowercase = MinLowercase,
            MinNumbers = MinNumbers,
            MinSymbols = MinSymbols
        };
    }
}
