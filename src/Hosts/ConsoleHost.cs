namespace Elppa;

// Hosts the emulator directly inside a console app.
public class ConsoleHost: ITerminalHost
{
    public Action<char>? KeyPressed { get; set; }
    
    public void WriteCharacter(byte ascii)
    {
        var asciiChar = Convert.ToChar(ascii);

        switch (asciiChar)
        {
            case '\r':
                Console.Write('\n');
                break;
            case '_':
                // Move back, erase the character, move back again
                Console.Write("\b \b");
                break;
            default:
                Console.Write(asciiChar);
                break;
        }
    }

    public void Run(Action runEmulation)
    {
        new Thread(() =>
        {
            while (true)
            {
                KeyPressed?.Invoke(Console.ReadKey(intercept: true).KeyChar);   
            }
        }) { IsBackground = true }.Start();

        while (true)
        {
            runEmulation();
            Thread.Sleep(16);
        }
    }
}