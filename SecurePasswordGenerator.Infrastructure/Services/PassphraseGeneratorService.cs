using SecurePasswordGenerator.Domain.Interfaces;
using SecurePasswordGenerator.Domain.Models;
using System.Net.Http.Json;

namespace SecurePasswordGenerator.Infrastructure.Services;

public class PassphraseGeneratorService : IPassphraseGenerator
{
    private readonly HttpClient _httpClient;
    private readonly ICryptoRandomService _randomService;
    private static readonly Dictionary<Language, List<string>> _wordCache = new();

    public PassphraseGeneratorService(HttpClient httpClient, ICryptoRandomService randomService)
    {
        _httpClient = httpClient;
        _randomService = randomService;
    }

    public async Task<Passphrase> GenerateAsync(PassphraseOptions options)
    {
        // 1. Load word list based on language
        if (!_wordCache.ContainsKey(options.Language))
        {
            await LoadWordListAsync(options.Language);
        }

        var wordList = _wordCache[options.Language];
        if (wordList == null || !wordList.Any())
        {
            // Fallback if list is empty
            wordList = new List<string> { "correct", "horse", "battery", "staple" };
        }

        // 2. Select random words
        var selectedWords = new List<string>();
        
        // Simple filter
        var validWords = wordList.Where(w => w.Length >= options.MinWordLength && w.Length <= options.MaxWordLength).ToList();
        
        if (!validWords.Any())
        {
             validWords = wordList;
        }

        for (int i = 0; i < options.WordCount; i++)
        {
            var index = _randomService.GetRandomInt(0, validWords.Count);
            var word = validWords[index];
            
            if (options.Capitalize)
            {
                // Capitalize first letter
                if (!string.IsNullOrEmpty(word))
                    word = char.ToUpper(word[0]) + word.Substring(1);
            }
            else
            {
                word = word.ToLowerInvariant();
            }
            
            selectedWords.Add(word);
        }

        // 3. Assemble passphrase
        var value = string.Join(options.Separator, selectedWords);

        // 4. Add extras
        if (options.IncludeNumber)
        {
            value += _randomService.GetRandomInt(0, 10);
        }

        if (options.IncludeSymbol)
        {
            string[] symbols = { "!", "@", "#", "$", "%", "&", "*" };
            value += symbols[_randomService.GetRandomInt(0, symbols.Length)];
        }

        // 5. Calculate strength
        double entropy = Math.Log2(Math.Pow(validWords.Count, options.WordCount));
        if (options.IncludeNumber) entropy += Math.Log2(10);
        if (options.IncludeSymbol) entropy += Math.Log2(7);
        
        // Calculate score
        int score = 0;
        if (entropy < 28) score = 20; // Very Weak
        else if (entropy < 36) score = 40; // Weak
        else if (entropy < 60) score = 60; // Moderate
        else if (entropy < 128) score = 80; // Strong
        else score = 100; // Very Strong

        PasswordStrengthLevel level;
        if (score < 40) level = PasswordStrengthLevel.VeryWeak;
        else if (score < 60) level = PasswordStrengthLevel.Weak;
        else if (score < 80) level = PasswordStrengthLevel.Medium;
        else if (score < 100) level = PasswordStrengthLevel.Strong;
        else level = PasswordStrengthLevel.VeryStrong;

        // Crack time (rough estimate: 10^9 guesses/sec)
        // 2^entropy guesses
        double seconds = Math.Pow(2, entropy) / 1e9;
        
        string timeToCrack;
        if (seconds < 1) timeToCrack = "Anında";
        else if (seconds < 60) timeToCrack = $"{(int)seconds} saniye";
        else if (seconds < 3600) timeToCrack = $"{(int)(seconds/60)} dakika";
        else if (seconds < 86400) timeToCrack = $"{(int)(seconds/3600)} saat";
        else if (seconds < 31536000) timeToCrack = $"{(int)(seconds/86400)} gün";
        else timeToCrack = $"{(int)(seconds/31536000)} yıl";
        
        // Override for very high entropy
        if (entropy > 100) timeToCrack = "Yüzyıllar";


        return new Passphrase
        {
            Value = value,
            Words = selectedWords,
            Options = options,
            GeneratedAt = DateTime.UtcNow,
            Strength = new PasswordStrength 
            { 
               Entropy = entropy,
               Score = score,
               Level = level,
               TimeToCrack = timeToCrack
            } 
        };
    }

    private async Task LoadWordListAsync(Language language)
    {
        string fileName = language == Language.Turkish ? "turkish.json" : "english.json";
        try 
        {
            var words = await _httpClient.GetFromJsonAsync<List<string>>($"data/{fileName}");
            if (words != null)
            {
                _wordCache[language] = words;
            }
            else 
            {
                 _wordCache[language] = new List<string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading word list: {ex.Message}");
            // Fallback minimal list to prevent crash
            _wordCache[language] = new List<string> { 
                "apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew",
                "elma", "armut", "kiraz", "hurma", "incir", "uzum", "kavun", "karpuz"
            };
        }
    }
}
