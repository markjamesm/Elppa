namespace Elppa.Core;

public static class KeyboardMapper
{
    /// <summary>
    /// Attempts to map modern keyboard keys into the format that the Apple I expects.
    /// </summary>
    /// <param name="input">The input character.</param>
    /// <param name="key">The mapped Apple I key.</param>
    /// <returns>Whether or not the mapping succeeded, and the corresponding output key.</returns>
    public static bool TryMap(char input, out char key)
    {
        key = '\0';

        if (input > '\x7F') // Reject characters outside of ASCII range.
        {
            return false;
        }

        var mapped = Normalize(input);

        if (!IsValidKey(mapped))
        {
            return false;
        }

        key = mapped;
        return true;
    }
    
    private static char Normalize(char ch) => ch switch
    {
        '\n'           => '\r', // Map newline to carriage return.
        '\b' or '\x7F' => '_', // Map backspace & delete to underscore.
        _              => char.ToUpperInvariant(ch) // Wozmon uses upper-case characters only.
    };
    
    private static bool IsValidKey(char ch)
    {
        // True if key pressed is Enter, Escape, Space, or any printable ASCII characters.
        return ch is '\r' or '\x1B' or >= '\x20' and < '\x7F';
    }
}