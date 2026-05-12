using Velutia.Bus;

namespace Elppa;

public class Bus: IBus
{
    private readonly Ram _ram;
    private readonly Pia6820 _pia;
    
    public Bus(Ram ram, Pia6820 pia)
    {
        _ram = ram;
        _pia = pia;
    }

    public byte Read(ushort address)
    {
        if (address is >= 0xD010 and <= 0xD013)
        {
            return _pia.Read(address);
        }
        
        return _ram[address];
    }

    public void Write(ushort address, byte value)
    {
        if (address is >= 0xD010 and <= 0xD013)
        {
            _pia.Write(address, value);
        }
        
        else
        {
            _ram[address] = value;
        }
    }
}