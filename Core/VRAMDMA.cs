namespace GameBoyCEmulator.Core;

public class VRAMDMA
{
    public const int CyclesPerTransfer = 2;

    private int _dmaLength;
    private int _transfers;
    private ushort _sourceAddress;
    private ushort _destinationAddress;
    private byte _hdma5;

    private bool _active;
    private bool _hBlankDMA;
    private bool _inHBlank;
    private bool _halted;

    public byte HDMA1
    {
        set
        {
            _sourceAddress = (ushort)(_sourceAddress & 0x00FF);
            _sourceAddress = (ushort)(_sourceAddress | (value << 8));
        }
    }
    public byte HDMA2
    {
        set
        {
            _sourceAddress = (ushort)(_sourceAddress & 0xFF00);
            _sourceAddress = (ushort)(_sourceAddress | (value & 0xF0));
        }
    }
    public byte HDMA3
    {
        set
        {
            _destinationAddress = (ushort)(_destinationAddress & 0x00FF);
            _destinationAddress = (ushort)(_destinationAddress | ((value & 0x1F) << 8));
        }
    }
    public byte HDMA4
    {
        set
        {
            _destinationAddress = (ushort)(_destinationAddress & 0xFF00);
            _destinationAddress = (ushort)(_destinationAddress | (value & 0xF0));
        }
    }

    public byte HDMA5
    {
        get => _hdma5;
        set
        {
            if (!_active)
            {
                _hdma5 = (byte)(value & 0x7F);
                _active = true;
                _inHBlank = false;
                _transfers = 0;
                _hBlankDMA = (value & 0x80) != 0;
                _dmaLength = ((value & 0x7F) + 1) * 0x10;
            }
            else if ((value & 0x80) == 0)
            {
                _active = false;
                _hdma5 = (byte)(_hdma5 | 0x80);
            }
        }
    }

    public bool Active { get => _active; }
    public bool HBlankDMA { get => _hBlankDMA; }
    public bool InHBlank { get => _inHBlank; set => _inHBlank = value; }
    public bool Halted { get => _halted; set => _halted = value; }

    public int Tick(MMU mmu)
    {
        if (!_active || _halted)
        {
            return 0;
        }

        for (int i = _transfers; i < _transfers + 0x10; i++)
        {
            byte b = mmu.ReadByte((ushort)(_sourceAddress + i));
            mmu.WriteByte((ushort)(_destinationAddress + i), b);
        }

        _transfers += 0x10;
        _hdma5--;
        if (_transfers >= _dmaLength)
        {
            _active = false;
        }

        _inHBlank = false;
        return CyclesPerTransfer * 0x10;
    }
}
