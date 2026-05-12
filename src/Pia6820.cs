namespace Elppa;

public class Pia6820
{
    // $D010
    // Keyboard input register. This register holds valid data when
    // bit 7 of KBDCR is "1". Reading KBD will automatically clear
    // bit 7 of KBDCR.
    private byte _keyboardInputRegister;
    
    // $D011
    // The only bit which we are interested in this register is the
    // read-only bit b7. It will be set by hardware whenever a key is
    // pressed on the keyboard. It is cleared again when the KBD
    // location is read.
    private byte _keyboardControlRegister;
    
    // $D012
    // Bits b6…b0 are the character outputs for the terminal display.
    // Writing to this register will set b7 of DSP, which is the only
    // input bit of this register. The terminal hardware will clear
    // bit b7 as soon as the character is accepted.
    private byte _displayRegister;
    
    // $D013
    // This register is better left untouched, it contains no useful
    // data for a user program. 
    // private byte _displayControlRegister;
    
    private readonly Action<byte> _writeToDisplay;

    public Pia6820(Action<byte> writeToDisplay)
    {
        _writeToDisplay = writeToDisplay;
    }

    public byte Read(ushort address)
    {
        switch (address)
        {
            case 0xD010:
               _keyboardControlRegister = (byte)(_keyboardControlRegister & ~0x80);
               return _keyboardInputRegister;
            
            case 0xD011:
                return (byte)(_keyboardControlRegister & 0x80) != 0 ? (byte)0x80 : (byte)0x00;
            
            case 0xD012:
            case 0xD013:
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(address));
        }

        return 0x00;
    }

    public void Write(ushort address, byte value)
    {
        switch (address)
        {
            case 0xD011:
                break;
            
            case 0xD012:
                _writeToDisplay.Invoke((byte)(value & 0x7F));
                _displayRegister |= 0x80;
                break;
            
            case 0xD013:
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(address));
        }
    }

    public void KeyPress(char ch)
    {
        _keyboardInputRegister = (byte)(ch | 0x80);
        _keyboardControlRegister |= 0x80;
    }
}