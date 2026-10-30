using Elppa.Core;

namespace Elppa.Cli.Hosts;

// Hosts the emulator directly inside a console app.
public class ConsoleTerminal: ITerminal
{
    public void WriteCharacter(byte asciiCode)
    {
        switch (asciiCode)
        {
            case 0x0D:                      // carriage return '\r'
                Console.Write('\n');
                break;
            case 0x5F:                      // '_' = rubout: back, erase, back
                Console.Write("\b \b");
                break;
            case >= 0x20 and < 0x7F:        // printable ASCII range
                Console.Write(Convert.ToChar(asciiCode));
                break;
            // ignore anything else (NUL, DEL, tab, other control codes)
        }
    }

    public static void Run(Emulator emulator)
    {
        new Thread(() =>
        {
            while (true)
            {
                var ch = Console.ReadKey(intercept: true).KeyChar;
                emulator.SendKey(ch);
            }
        }) { IsBackground = true }.Start();

        while (true)
        {
            emulator.RunFrame();
            Thread.Sleep(16);
        }
    }
}