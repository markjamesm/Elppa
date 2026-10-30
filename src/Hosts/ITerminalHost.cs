namespace Elppa.Hosts;

public interface ITerminalHost
{
    Action<char>? KeyPressed { get; set; }
    void WriteCharacter(byte ascii);
    void Run(Action runEmulation);
}