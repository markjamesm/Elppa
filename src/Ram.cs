namespace Elppa;

public sealed class Ram
{
    private readonly byte[] _ram;

    public Ram(int memorySize)
    {
        _ram = new byte[memorySize];
    }
    
    public byte this[ushort address]
    {
        get => _ram[address];
        set => _ram[address] = value;
    }

    public void Load(byte[] rom, int address)
    {
        rom.CopyTo(_ram, address);
    }
}