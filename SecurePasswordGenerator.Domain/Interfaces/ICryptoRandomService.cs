namespace SecurePasswordGenerator.Domain.Interfaces;

/// <summary>
/// Interface for cryptographically secure random number generation
/// Interface Segregation Principle: Focused on random generation only
/// </summary>
public interface ICryptoRandomService
{
    /// <summary>
    /// Gets a cryptographically secure random integer
    /// </summary>
    /// <param name="minValue">Minimum value (inclusive)</param>
    /// <param name="maxValue">Maximum value (exclusive)</param>
    /// <returns>Random integer</returns>
    int GetRandomInt(int minValue, int maxValue);

    /// <summary>
    /// Gets a cryptographically secure random byte array
    /// </summary>
    /// <param name="length">Length of the array</param>
    /// <returns>Random byte array</returns>
    byte[] GetRandomBytes(int length);

    /// <summary>
    /// Shuffles an array using cryptographically secure random
    /// </summary>
    /// <typeparam name="T">Type of array elements</typeparam>
    /// <param name="array">Array to shuffle</param>
    void Shuffle<T>(T[] array);
}
