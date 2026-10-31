namespace Elppa.Core;

public interface ITerminal
{
    /// <summary>
    /// Called when the machine writes a character to its display.
    /// </summary>
    /// <param name="asciiCode">
    /// A 7-bit ASCII code. 0x0D is carriage return and 0x5F (_) is the
    /// Apple I's rubout. Other control codes can be ignored.
    /// </param>
    void WriteCharacter(byte asciiCode);
}