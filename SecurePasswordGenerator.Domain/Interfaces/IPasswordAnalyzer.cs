using SecurePasswordGenerator.Domain.Models;

namespace SecurePasswordGenerator.Domain.Interfaces;

/// <summary>
/// Interface for password strength analysis
/// Interface Segregation Principle: Focused on analysis only
/// </summary>
public interface IPasswordAnalyzer
{
    /// <summary>
    /// Analyzes the strength of a password
    /// </summary>
    /// <param name="password">Password to analyze</param>
    /// <returns>Password strength analysis</returns>
    PasswordStrength Analyze(string password);

    /// <summary>
    /// Calculates the entropy of a password
    /// </summary>
    /// <param name="password">Password to analyze</param>
    /// <returns>Entropy in bits</returns>
    double CalculateEntropy(string password);

    /// <summary>
    /// Estimates time to crack the password
    /// </summary>
    /// <param name="entropy">Password entropy</param>
    /// <returns>Human-readable time estimate</returns>
    string EstimateTimeToCrack(double entropy);
}
