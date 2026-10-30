using Elppa.Cli.Hosts;
using Elppa.Core;

namespace Elppa.Cli;

class Program
{
    static void Main(string[] args)
    {
        var emulator = new Emulator(ConsoleHost.WriteCharacter);
        ConsoleHost.Run(emulator);
    }
}