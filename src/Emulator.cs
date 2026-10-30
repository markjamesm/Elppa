using Velutia.Processor;

namespace Elppa;

public static class Emulator
{
    public static void Start(ITerminalHost host)
    {
        var ram = InitializeRam();
        var pia = new Pia6820(host.WriteCharacter);
        var bus = new Bus(ram, pia);
        var cpu = new Cpu(bus);

        host.KeyPressed = ch => pia.KeyPress(NormalizeInput(ch));

        host.Run(() => RunFrame(cpu));
    }
    
    private static void RunFrame(Cpu cpu)
    {
        for (var i = 0; i < 17045; i++)
            cpu.RunInstruction();
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
    
    private static char NormalizeInput(char ch) => ch switch
    {
        '\n'           => '\r',
        '\b' or '\x7F' => '_',
        _              => char.ToUpperInvariant(ch) // Wozmon uses upper-case characters
    };
}