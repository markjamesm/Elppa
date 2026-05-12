using Velutia.Processor;

namespace Elppa;

public static class Machine
{
    private static readonly Lock ConsoleLock = new();
    
    public static void Start()
    {
        var ram = InitializeRam();
        var pia = new Pia6820(writeToDisplay: (value) =>
        {
            lock (ConsoleLock)
            {
                var c = (char)value;
        
                if (c == '\r')
                {
                    Console.Write('\n');
                }
                else if (c == '_')
                {
                    // Move back, erase the character, move back again
                    Console.Write("\b \b");
                }
                else
                {
                    Console.Write(c);
                }
            }
        } );
        
        var bus = new Bus(ram, pia);
        var cpu = new Cpu(bus);

        Task.Run(() =>
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                
                lock (ConsoleLock)
                {
                    var ch = key.KeyChar;
            
                    // Normalize Enter
                    if (ch == '\n') ch = '\r';
            
                    // Translate backspace/delete to Apple 1 underscore
                    if (ch is '\b' or '\x7F') ch = '_';
            
                    pia.KeyPress(ch);
                }
            }
        });

        EmulationLoop(cpu);
    }
    
    private static void EmulationLoop(Cpu cpu)
    {
        while (true)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(16.67));
            cpu.RunInstruction();
        }
    }
    
    private static Ram InitializeRam()
    {
        const ushort romStart = 0xFF00;
        const ushort basicStart = 0xE000;

        var memory = new Ram(64 * 1024); // 8kb of RAM for basic, need to setup memory bank
        var wozmon = RomLoader.LoadRom("monitor.rom");
        var appleBasic = RomLoader.LoadRom("basic.rom");
        
        memory.Load(wozmon, romStart);
        memory.Load(appleBasic, basicStart);
        
        return memory;
    }
}