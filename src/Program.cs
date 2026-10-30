using Elppa.Hosts;

namespace Elppa;

class Program
{
    static void Main(string[] args)
    {
        var host = new ConsoleHost();
        Emulator.Start(host);
    }
}