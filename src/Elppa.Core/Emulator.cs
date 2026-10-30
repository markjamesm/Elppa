using Elppa.Cli.Elppa.Core.Hardware;
using Elppa.Cli.Hardware;
using Velutia.Processor;

namespace Elppa.Core;

public sealed class Emulator
{
    // 1.022727 MHz / 60 fps
    private const int CyclesPerFrame = 17045;

    private readonly Pia6820 _pia;
    private readonly Cpu _cpu;

    public Emulator(Action<byte> writeCharacter)
    {
        var ram = InitializeRam();
        _pia = new Pia6820(writeCharacter);
        _cpu = new Cpu(new Bus(ram, _pia));
    }

    public void KeyPress(char ch) => _pia.KeyPress(NormalizeInput(ch));
    
    // Call once per frame (about 60 times a second) from the frontend.
    public void RunFrame()
    {
        var start = _cpu.Cycles;

        // Subtracting keeps this correct even if the int counter wraps.
        while (_cpu.Cycles - start < CyclesPerFrame)
            _cpu.RunInstruction();
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