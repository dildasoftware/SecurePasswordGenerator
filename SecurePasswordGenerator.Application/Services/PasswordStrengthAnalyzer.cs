using SecurePasswordGenerator.Domain.Interfaces;
using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Application.Services;

/// <summary>
/// Şifre gücü analiz servisi
/// 
/// SOLID Prensipleri:
/// - Single Responsibility: Sadece şifre gücü analizinden sorumlu
/// - Open/Closed: Yeni analiz kriterleri eklenebilir
/// 
/// Entropi hesaplama formülü: E = log2(N^L)
/// N = Karakter seti boyutu, L = Şifre uzunluğu
/// </summary>
public class PasswordStrengthAnalyzer : IPasswordAnalyzer
{
    // Saniyede deneme sayısı (modern GPU ile brute force)
    private const long AttemptsPerSecond = 100_000_000_000; // 100 milyar/saniye

    /// <summary>
    /// Şifrenin gücünü analiz eder
    /// </summary>
    public PasswordStrength Analyze(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return new PasswordStrength
            {
                Password = string.Empty,
                Level = PasswordStrengthLevel.VeryWeak,
                Entropy = 0,
                Score = 0,
                TimeToCrack = "Anında",
                Feedback = new List<string> { "Şifre boş olamaz" }
            };
        }

        var strength = new PasswordStrength
        {
            Password = password
        };

        // 1. Karakter tiplerini kontrol et
        AnalyzeCharacterTypes(password, strength);

        // 2. Entropi hesapla
        strength.Entropy = CalculateEntropy(password);

        // 3. Güç seviyesini belirle
        strength.Level = DetermineStrengthLevel(strength.Entropy);

        // 4. Skor hesapla (0-100)
        strength.Score = CalculateScore(strength);

        // 5. Kırılma süresini tahmin et
        strength.TimeToCrack = EstimateTimeToCrack(strength.Entropy);

        // 6. Geri bildirim ve uyarılar oluştur
        GenerateFeedback(password, strength);

        return strength;
    }

    /// <summary>
    /// Şifrenin entropisini hesaplar
    /// Entropi = log2(karakter_seti_boyutu ^ şifre_uzunluğu)
    /// </summary>
    public double CalculateEntropy(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return 0;
        }

        // Karakter seti boyutunu belirle
        int charsetSize = DetermineCharsetSize(password);

        // Entropi formülü: E = L * log2(N)
        // L = uzunluk, N = karakter seti boyutu
        double entropy = password.Length * Math.Log2(charsetSize);

        return Math.Round(entropy, 2);
    }

    /// <summary>
    /// Kırılma süresini tahmin eder
    /// </summary>
    public string EstimateTimeToCrack(double entropy)
    {
        if (entropy <= 0)
        {
            return "Anında";
        }

        // Toplam kombinasyon sayısı = 2^entropi
        double combinations = Math.Pow(2, entropy);

        // Ortalama deneme sayısı (kombinasyonların yarısı)
        double averageAttempts = combinations / 2;

        // Saniye cinsinden süre
        double seconds = averageAttempts / AttemptsPerSecond;

        return FormatTime(seconds);
    }

    #region Private Helper Methods

    /// <summary>
    /// Şifredeki karakter tiplerini analiz eder
    /// </summary>
    private void AnalyzeCharacterTypes(string password, PasswordStrength strength)
    {
        strength.HasUppercase = password.Any(char.IsUpper);
        strength.HasLowercase = password.Any(char.IsLower);
        strength.HasNumbers = password.Any(char.IsDigit);
        strength.HasSymbols = password.Any(c => !char.IsLetterOrDigit(c));
        strength.UniqueCharacters = password.Distinct().Count();
    }

    /// <summary>
    /// Şifredeki karakter seti boyutunu belirler
    /// </summary>
    private int DetermineCharsetSize(string password)
    {
        int size = 0;

        if (password.Any(char.IsUpper)) size += 26; // A-Z
        if (password.Any(char.IsLower)) size += 26; // a-z
        if (password.Any(char.IsDigit)) size += 10; // 0-9
        if (password.Any(c => !char.IsLetterOrDigit(c))) size += 32; // Semboller (yaklaşık)

        return size > 0 ? size : 1; // Sıfır bölme hatası önleme
    }

    /// <summary>
    /// Entropiye göre güç seviyesini belirler
    /// </summary>
    private PasswordStrengthLevel DetermineStrengthLevel(double entropy)
    {
        return entropy switch
        {
            < 28 => PasswordStrengthLevel.VeryWeak,    // Saniyeler
            < 36 => PasswordStrengthLevel.Weak,        // Dakikalar-Saatler
            < 60 => PasswordStrengthLevel.Moderate,    // Günler-Aylar
            < 128 => PasswordStrengthLevel.Strong,     // Yıllar-Yüzyıllar
            _ => PasswordStrengthLevel.VeryStrong      // Evren yaşından uzun
        };
    }

    /// <summary>
    /// Genel skoru hesaplar (0-100)
    /// </summary>
    private int CalculateScore(PasswordStrength strength)
    {
        int score = 0;

        // Uzunluk bonusu (max 30 puan)
        score += Math.Min(strength.Password.Length * 2, 30);

        // Karakter çeşitliliği bonusu (max 40 puan)
        if (strength.HasUppercase) score += 10;
        if (strength.HasLowercase) score += 10;
        if (strength.HasNumbers) score += 10;
        if (strength.HasSymbols) score += 10;

        // Benzersiz karakter bonusu (max 20 puan)
        double uniqueRatio = (double)strength.UniqueCharacters / strength.Password.Length;
        score += (int)(uniqueRatio * 20);

        // Entropi bonusu (max 10 puan)
        if (strength.Entropy >= 128) score += 10;
        else if (strength.Entropy >= 60) score += 7;
        else if (strength.Entropy >= 36) score += 4;

        return Math.Min(score, 100);
    }

    /// <summary>
    /// Kullanıcıya geri bildirim ve uyarılar oluşturur
    /// </summary>
    private void GenerateFeedback(string password, PasswordStrength strength)
    {
        strength.Feedback = new List<string>();
        strength.Warnings = new List<string>();

        // Uzunluk kontrolleri
        if (password.Length < 8)
        {
            strength.Warnings.Add("Şifre çok kısa. En az 8 karakter kullanın.");
        }
        else if (password.Length >= 16)
        {
            strength.Feedback.Add("Mükemmel uzunluk!");
        }

        // Karakter çeşitliliği kontrolleri
        if (!strength.HasUppercase)
        {
            strength.Warnings.Add("Büyük harf eklemeyi düşünün.");
        }

        if (!strength.HasLowercase)
        {
            strength.Warnings.Add("Küçük harf eklemeyi düşünün.");
        }

        if (!strength.HasNumbers)
        {
            strength.Warnings.Add("Rakam eklemeyi düşünün.");
        }

        if (!strength.HasSymbols)
        {
            strength.Warnings.Add("Özel karakter eklemeyi düşünün.");
        }

        // Tekrar eden karakterler
        if (HasRepeatingCharacters(password))
        {
            strength.Warnings.Add("Tekrar eden karakterler var (örn: 'aaa', '111').");
        }

        // Ardışık karakterler
        if (HasSequentialCharacters(password))
        {
            strength.Warnings.Add("Ardışık karakterler var (örn: 'abc', '123').");
        }

        // Pozitif geri bildirim
        if (strength.Level >= PasswordStrengthLevel.Strong)
        {
            strength.Feedback.Add("Harika! Çok güçlü bir şifre.");
        }
        else if (strength.Level == PasswordStrengthLevel.Moderate)
        {
            strength.Feedback.Add("İyi bir şifre, ama daha da güçlendirilebilir.");
        }

        // Benzersiz karakter oranı
        double uniqueRatio = (double)strength.UniqueCharacters / password.Length;
        if (uniqueRatio < 0.5)
        {
            strength.Warnings.Add("Çok fazla tekrar eden karakter var.");
        }
    }

    /// <summary>
    /// Tekrar eden karakterleri kontrol eder (örn: aaa, 111)
    /// </summary>
    private bool HasRepeatingCharacters(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] == password[i + 1] && password[i] == password[i + 2])
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Ardışık karakterleri kontrol eder (örn: abc, 123)
    /// </summary>
    private bool HasSequentialCharacters(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            // Artan sıra (abc, 123)
            if (password[i] + 1 == password[i + 1] && password[i] + 2 == password[i + 2])
            {
                return true;
            }

            // Azalan sıra (cba, 321)
            if (password[i] - 1 == password[i + 1] && password[i] - 2 == password[i + 2])
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Saniyeyi insan okunabilir formata çevirir
    /// </summary>
    private string FormatTime(double seconds)
    {
        if (seconds < 1)
            return "Anında";

        if (seconds < 60)
            return $"{Math.Ceiling(seconds)} saniye";

        if (seconds < 3600)
            return $"{Math.Ceiling(seconds / 60)} dakika";

        if (seconds < 86400)
            return $"{Math.Ceiling(seconds / 3600)} saat";

        if (seconds < 2592000) // 30 gün
            return $"{Math.Ceiling(seconds / 86400)} gün";

        if (seconds < 31536000) // 365 gün
            return $"{Math.Ceiling(seconds / 2592000)} ay";

        double years = seconds / 31536000;
        
        if (years < 1000)
            return $"{Math.Ceiling(years)} yıl";

        if (years < 1000000)
            return $"{Math.Ceiling(years / 1000)} bin yıl";

        if (years < 1000000000)
            return $"{Math.Ceiling(years / 1000000)} milyon yıl";

        return "Evren yaşından uzun";
    }

    #endregion
}
