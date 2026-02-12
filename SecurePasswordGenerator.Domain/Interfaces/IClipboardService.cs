namespace SecurePasswordGenerator.Domain.Interfaces;

/// <summary>
/// Interface for clipboard operations
/// Interface Segregation Principle: Focused on clipboard only
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Copies text to clipboard
    /// </summary>
    /// <param name="text">Text to copy</param>
    /// <returns>Task representing the async operation</returns>
    Task CopyToClipboardAsync(string text);

    /// <summary>
    /// Clears the clipboard
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task ClearClipboardAsync();

    /// <summary>
    /// Copies text and clears after a delay
    /// </summary>
    /// <param name="text">Text to copy</param>
    /// <param name="clearAfterSeconds">Seconds to wait before clearing</param>
    /// <returns>Task representing the async operation</returns>
    Task CopyWithAutoClearAsync(string text, int clearAfterSeconds = 30);
}
