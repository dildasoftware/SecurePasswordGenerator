using SecurePasswordGenerator.Domain.Constants;
using SecurePasswordGenerator.Domain.Interfaces;
using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Application.Services;

/// <summary>
/// Şifre üretim servisi
/// 
/// SOLID Prensipleri:
/// - Single Responsibility: Sadece şifre üretiminden sorumlu
/// - Dependency Inversion: Interface'lere bağımlı (ICryptoRandomService, IPasswordAnalyzer)
/// - Open/Closed: Yeni özellikler için genişletilebilir
/// 
/// Güvenlik: Kriptografik rastgele sayı kullanır
/// </summary>
public class PasswordGeneratorService : IPasswordGenerator
{
    private readonly ICryptoRandomService _cryptoRandom;
    private readonly IPasswordAnalyzer _passwordAnalyzer;

    /// <summary>
    /// Constructor - Dependency Injection ile bağımlılıkları alır
    /// </summary>
    public PasswordGeneratorService(
        ICryptoRandomService cryptoRandom,
        IPasswordAnalyzer passwordAnalyzer)
    {
        _cryptoRandom = cryptoRandom ?? throw new ArgumentNullException(nameof(cryptoRandom));
        _passwordAnalyzer = passwordAnalyzer ?? throw new ArgumentNullException(nameof(passwordAnalyzer));
    }

    /// <summary>
    /// Verilen ayarlara göre şifre üretir
    /// </summary>
    public GeneratedPassword Generate(PasswordOptions options)
    {
        // 1. Validasyon (doğrulama)
        ValidateOptions(options);

        // 2. Karakter setini oluştur
        string characterSet = BuildCharacterSet(options);

        // 3. Şifreyi üret
        string password = GeneratePasswordFromCharset(characterSet, options);

        // 4. Minimum gereksinimleri kontrol et ve gerekirse düzelt
        password = EnsureMinimumRequirements(password, options);

        // 5. Güç analizi yap
        var strength = _passwordAnalyzer.Analyze(password);

        // 6. Sonuç nesnesini oluştur ve döndür
        return new GeneratedPassword
        {
            Value = password,
            Strength = strength,
            Options = options.Clone(),
            GeneratedAt = DateTime.UtcNow,
            Type = PasswordType.Standard
        };
    }

    /// <summary>
    /// Toplu şifre üretimi yapar
    /// </summary>
    public List<GeneratedPassword> GenerateBulk(PasswordOptions options, int count)
    {
        if (count <= 0 || count > 100)
        {
            throw new ArgumentException("Count must be between 1 and 100", nameof(count));
        }

        var passwords = new List<GeneratedPassword>(count);
        
        for (int i = 0; i < count; i++)
        {
            passwords.Add(Generate(options));
        }

        return passwords;
    }

    /// <summary>
    /// Şablon (pattern) kullanarak şifre üretir
    /// Örnek: "XXX-999-xxx" -> "ABC-123-def"
    /// X = Büyük harf, x = küçük harf, 9 = rakam, @ = sembol
    /// </summary>
    public GeneratedPassword GenerateFromPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentException("Pattern cannot be empty", nameof(pattern));
        }

        var passwordChars = new char[pattern.Length];

        for (int i = 0; i < pattern.Length; i++)
        {
            passwordChars[i] = pattern[i] switch
            {
                'X' => GetRandomChar(CharacterSets.Uppercase),
                'x' => GetRandomChar(CharacterSets.Lowercase),
                '9' => GetRandomChar(CharacterSets.Numbers),
                '@' => GetRandomChar(CharacterSets.Symbols),
                'A' => GetRandomChar(CharacterSets.Uppercase + CharacterSets.Lowercase),
                _ => pattern[i] // Diğer karakterler olduğu gibi kalır (-, _, vb.)
            };
        }

        string password = new string(passwordChars);
        var strength = _passwordAnalyzer.Analyze(password);

        return new GeneratedPassword
        {
            Value = password,
            Strength = strength,
            GeneratedAt = DateTime.UtcNow,
            Type = PasswordType.Pattern
        };
    }

    #region Private Helper Methods (Yardımcı Metodlar)

    /// <summary>
    /// Şifre ayarlarını doğrular
    /// </summary>
    private void ValidateOptions(PasswordOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options.Length < 4 || options.Length > 128)
        {
            throw new ArgumentException("Password length must be between 4 and 128", nameof(options.Length));
        }

        if (!options.IsValid())
        {
            throw new ArgumentException("At least one character type must be selected");
        }

        // Minimum gereksinimler toplamı, şifre uzunluğundan fazla olamaz
        int minTotal = options.MinUppercase + options.MinLowercase + 
                       options.MinNumbers + options.MinSymbols;
        
        if (minTotal > options.Length)
        {
            throw new ArgumentException("Sum of minimum requirements exceeds password length");
        }
    }

    /// <summary>
    /// Ayarlara göre karakter seti oluşturur
    /// </summary>
    private string BuildCharacterSet(PasswordOptions options)
    {
        // Özel karakter seti varsa onu kullan
        if (!string.IsNullOrEmpty(options.CustomCharacterSet))
        {
            return options.CustomCharacterSet;
        }

        // Yoksa ayarlara göre oluştur
        return CharacterSets.GetCharacterSet(
            options.IncludeUppercase,
            options.IncludeLowercase,
            options.IncludeNumbers,
            options.IncludeSymbols,
            options.ExcludeSimilarCharacters,
            options.ExcludeAmbiguousCharacters
        );
    }

    /// <summary>
    /// Karakter setinden rastgele şifre üretir
    /// </summary>
    private string GeneratePasswordFromCharset(string characterSet, PasswordOptions options)
    {
        if (string.IsNullOrEmpty(characterSet))
        {
            throw new InvalidOperationException("Character set is empty");
        }

        var passwordChars = new char[options.Length];

        // Her pozisyon için rastgele karakter seç
        for (int i = 0; i < options.Length; i++)
        {
            int randomIndex = _cryptoRandom.GetRandomInt(0, characterSet.Length);
            passwordChars[i] = characterSet[randomIndex];
        }

        return new string(passwordChars);
    }

    /// <summary>
    /// Minimum karakter gereksinimlerini sağlar
    /// </summary>
    private string EnsureMinimumRequirements(string password, PasswordOptions options)
    {
        var chars = password.ToCharArray();
        var positions = Enumerable.Range(0, chars.Length).ToList();

        // Pozisyonları karıştır (rastgele yerleştirme için)
        var positionsArray = positions.ToArray();
        _cryptoRandom.Shuffle(positionsArray);
        var shuffledPositions = new Queue<int>(positionsArray);

        // Minimum büyük harf gereksinimi
        EnsureMinimumCharType(chars, shuffledPositions, CharacterSets.Uppercase, 
            options.MinUppercase, char.IsUpper);

        // Minimum küçük harf gereksinimi
        EnsureMinimumCharType(chars, shuffledPositions, CharacterSets.Lowercase, 
            options.MinLowercase, char.IsLower);

        // Minimum rakam gereksinimi
        EnsureMinimumCharType(chars, shuffledPositions, CharacterSets.Numbers, 
            options.MinNumbers, char.IsDigit);

        // Minimum sembol gereksinimi
        EnsureMinimumCharType(chars, shuffledPositions, CharacterSets.Symbols, 
            options.MinSymbols, c => CharacterSets.Symbols.Contains(c));

        return new string(chars);
    }

    /// <summary>
    /// Belirli bir karakter tipinden minimum sayıda olmasını sağlar
    /// </summary>
    private void EnsureMinimumCharType(
        char[] chars, 
        Queue<int> availablePositions, 
        string characterSet, 
        int minCount,
        Func<char, bool> isOfType)
    {
        // Mevcut sayıyı say
        int currentCount = chars.Count(isOfType);

        // Eksik varsa ekle
        int needed = minCount - currentCount;
        for (int i = 0; i < needed && availablePositions.Count > 0; i++)
        {
            int position = availablePositions.Dequeue();
            chars[position] = GetRandomChar(characterSet);
        }
    }

    /// <summary>
    /// Karakter setinden rastgele bir karakter döndürür
    /// </summary>
    private char GetRandomChar(string characterSet)
    {
        int index = _cryptoRandom.GetRandomInt(0, characterSet.Length);
        return characterSet[index];
    }

    #endregion
}
