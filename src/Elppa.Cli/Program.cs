using Elppa.Cli.Hosts;
using Elppa.Core;

namespace Elppa.Cli;

class Program
{
    static void Main(string[] args)
    {
        var terminal = new ConsoleTerminal();
        var emulator = new Emulator(terminal);
        
        ConsoleTerminal.Run(emulator);
    }
}