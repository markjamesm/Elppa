namespace Elppa;

public interface ITerminalHost
{
    Action<char>? KeyPressed { get; set; }
    void WriteCharacter(byte ascii);
    void Run(Action runEmulation);
}