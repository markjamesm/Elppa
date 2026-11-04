using System.Text.RegularExpressions;
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
    private readonly ITerminal _terminal;

    public Emulator(ITerminal terminal)
    {
        var ram = InitializeRam();
        _pia = new Pia6820(terminal.WriteCharacter);
        _cpu = new Cpu(new Bus(ram, _pia));
        _terminal = terminal;
    }

    public void SendKey(char ch)
    {
        if (KeyboardMapper.TryMap(ch, out var key))
        {
            _pia.Strobe((byte)key);
        }
    }

    public void ExecuteProgramFromText(string sourceCode)
    {
        Reset();

        if (IsBasicSource(sourceCode))
        {
            _pia.TypeText("E000R\r"); // cold start BASIC, clears any old program
            _pia.TypeText(sourceCode.TrimEnd() + "\r");
        }

        else
        {
            _pia.TypeText(sourceCode); // Assembly program; typically ends with xxxxR.
        }
    }

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

        var memory = new Ram(64 * 1024); // 8kb of RAM for basic, need to setup memory bank.
        var wozmon = RomLoader.LoadRom("monitor.rom");
        var appleBasic = RomLoader.LoadRom("basic.rom");

        memory.Load(wozmon, romStart);
        memory.Load(appleBasic, basicStart);

        return memory;
    }

    private void Reset()
    {
        _terminal.Clear();
        _pia.Reset(); // clears the key queue and PIA registers
        _cpu.Reset(); // jumps through the reset vector at $FFFC into Wozmon
    }

    // Wozmon listings start with an address and a colon ("0300: A9 00 ...");
    // anything else (line numbers, LOMEM = 768, ...) is treated as BASIC.
    private static bool IsBasicSource(string text) =>
        !Regex.IsMatch(text.ToUpperInvariant(), @"^[0-9A-F:.R\s]*$");
}