namespace SecurePasswordGenerator.Domain.Constants;

/// <summary>
/// Character sets used for password generation
/// Single Responsibility: Only defines character sets
/// </summary>
public static class CharacterSets
{
    /// <summary>
    /// Uppercase letters (A-Z)
    /// </summary>
    public const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Lowercase letters (a-z)
    /// </summary>
    public const string Lowercase = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Numbers (0-9)
    /// </summary>
    public const string Numbers = "0123456789";

    /// <summary>
    /// Common symbols
    /// </summary>
    public const string Symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

    /// <summary>
    /// Extended symbols (includes more special characters)
    /// </summary>
    public const string ExtendedSymbols = "!@#$%^&*()_+-=[]{}|;:,.<>?/\\~`'\"";

    /// <summary>
    /// Similar characters that might be confused
    /// </summary>
    public const string SimilarCharacters = "il1Lo0O";

    /// <summary>
    /// Ambiguous characters that might be hard to type
    /// </summary>
    public const string AmbiguousCharacters = "{}[]()/\\'\"`~,;:.<>";

    /// <summary>
    /// All alphanumeric characters
    /// </summary>
    public static string Alphanumeric => Uppercase + Lowercase + Numbers;

    /// <summary>
    /// All characters (alphanumeric + symbols)
    /// </summary>
    public static string All => Uppercase + Lowercase + Numbers + Symbols;

    /// <summary>
    /// Gets a character set based on options
    /// </summary>
    public static string GetCharacterSet(
        bool includeUppercase,
        bool includeLowercase,
        bool includeNumbers,
        bool includeSymbols,
        bool excludeSimilar = false,
        bool excludeAmbiguous = false)
    {
        var charset = string.Empty;

        if (includeUppercase) charset += Uppercase;
        if (includeLowercase) charset += Lowercase;
        if (includeNumbers) charset += Numbers;
        if (includeSymbols) charset += Symbols;

        if (excludeSimilar)
        {
            charset = RemoveCharacters(charset, SimilarCharacters);
        }

        if (excludeAmbiguous)
        {
            charset = RemoveCharacters(charset, AmbiguousCharacters);
        }

        return charset;
    }

    /// <summary>
    /// Removes specified characters from a string
    /// </summary>
    private static string RemoveCharacters(string source, string toRemove)
    {
        return new string(source.Where(c => !toRemove.Contains(c)).ToArray());
    }
}
