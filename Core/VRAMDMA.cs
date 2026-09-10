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

    public byte HDMA1
    {
        get => (byte)(_sourceAddress >> 8);
        set
        {
            _sourceAddress = (ushort)(_sourceAddress & 0x00FF);
            _sourceAddress = (ushort)(_sourceAddress | (value << 8));
        }
    }
    public byte HDMA2
    {
        get => (byte)(_sourceAddress & 0x00FF);
        set
        {
            _sourceAddress = (ushort)(_sourceAddress & 0xFF00);
            _sourceAddress = (ushort)(_sourceAddress | (value & 0xF0));
        }
    }
    public byte HDMA3
    {
        get => (byte)(_destinationAddress >> 8);
        set
        {
            _destinationAddress = (ushort)(_destinationAddress & 0x00FF);
            _destinationAddress = (ushort)(_destinationAddress | ((value & 0x1F) << 8));
        }
    }
    public byte HDMA4
    {
        get => (byte)(_destinationAddress & 0x00FF);
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
                _hdma5 = value;
                _active = true;
                _hBlankDMA = (_hdma5 & 0x80) != 0;
                _dmaLength = ((_hdma5 & 0x7F) + 1) * 0x10;
            }
        }
    }

    public int Tick(MMU mmu)
    {
        if (!_active)
        {
            return 0;
        }

        if (_hBlankDMA)
        {
            for (int i = _transfers; i < _transfers + 0x10; i++)
            {
                byte b = mmu.ReadByte((ushort)(_sourceAddress + i));
                mmu.WriteByte((ushort)(_destinationAddress + i), b);
            }

            _transfers += 0x10;
            if (_transfers >= _dmaLength)
            {
                _active = false;
            }

            return CyclesPerTransfer * 0x10;
        }

        _active = false;
        for (int i = 0; i < _dmaLength; i++)
        {
            byte b = mmu.ReadByte((ushort)(_sourceAddress + i));
            mmu.WriteByte((ushort)(_destinationAddress + i), b);
        }

        return CyclesPerTransfer * _dmaLength;
    }
}
