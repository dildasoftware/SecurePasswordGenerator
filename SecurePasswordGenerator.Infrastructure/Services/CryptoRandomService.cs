using System.Security.Cryptography;
using SecurePasswordGenerator.Domain.Interfaces;

namespace SecurePasswordGenerator.Infrastructure.Services;

/// <summary>
/// Kriptografik olarak güvenli rastgele sayı üretim servisi
/// 
/// SOLID Prensipleri:
/// - Single Responsibility: Sadece güvenli rastgele sayı üretir
/// - Dependency Inversion: ICryptoRandomService interface'ini implement eder
/// 
/// Güvenlik: System.Random yerine RandomNumberGenerator kullanır (tahmin edilemez)
/// </summary>
public class CryptoRandomService : ICryptoRandomService
{
    /// <summary>
    /// Belirtilen aralıkta kriptografik güvenli rastgele bir tam sayı döndürür
    /// </summary>
    /// <param name="minValue">Minimum değer (dahil)</param>
    /// <param name="maxValue">Maksimum değer (hariç)</param>
    /// <returns>Rastgele tam sayı</returns>
    /// <exception cref="ArgumentException">minValue >= maxValue ise</exception>
    public int GetRandomInt(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
        {
            throw new ArgumentException("minValue must be less than maxValue");
        }

        // Aralık hesaplama
        uint range = (uint)(maxValue - minValue);

        // Bias (yanlılık) olmadan rastgele sayı üret
        // Örnek: 0-255 arasında 0-9 istiyorsak, 250+ değerleri atla (bias önleme)
        uint limit = uint.MaxValue - (uint.MaxValue % range);
        
        uint randomValue;
        do
        {
            randomValue = GetRandomUInt32();
        } while (randomValue >= limit);

        return (int)(minValue + (randomValue % range));
    }

    /// <summary>
    /// Belirtilen uzunlukta kriptografik güvenli rastgele byte dizisi döndürür
    /// </summary>
    /// <param name="length">Dizi uzunluğu</param>
    /// <returns>Rastgele byte dizisi</returns>
    public byte[] GetRandomBytes(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than 0", nameof(length));
        }

        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes); // .NET 6+ güvenli dolgu
        return bytes;
    }

    /// <summary>
    /// Diziyi kriptografik güvenli şekilde karıştırır (Fisher-Yates shuffle)
    /// </summary>
    /// <typeparam name="T">Dizi eleman tipi</typeparam>
    /// <param name="array">Karıştırılacak dizi</param>
    public void Shuffle<T>(T[] array)
    {
        if (array == null || array.Length <= 1)
        {
            return;
        }

        // Fisher-Yates shuffle algoritması
        // Sondan başa doğru her elemanı rastgele bir önceki elemanla değiştir
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = GetRandomInt(0, i + 1);
            
            // Swap (değiş-tokuş)
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    /// <summary>
    /// Kriptografik güvenli 32-bit unsigned integer üretir
    /// </summary>
    /// <returns>Rastgele uint32</returns>
    private uint GetRandomUInt32()
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }
}
