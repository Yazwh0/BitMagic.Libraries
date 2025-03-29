using BitMagic.X16Emulator;

namespace BitMagic.Common.Address;

public static class AddressFunctions
{
    public static int GetDebuggerAddress(int address, int ramBank, int romBank) =>
        (address, ramBank, romBank) switch
        {
            ( >= 0xc000, _, _) => ((romBank & 0xff) << 16) + (address & 0xffff),
            ( >= 0xa000, _, _) => ((ramBank & 0xff) << 16) + (address & 0xffff),
            _ => address
        };
    public static int GetDebuggerAddress(int address, Emulator emulator) =>
        GetDebuggerAddress(address, (int)emulator.RamBankAct, (int)emulator.RomBankAct);

    public static (int Address, int RamBank, int RomBank) GetAddress(int debuggerAddress) =>
        (debuggerAddress) switch
        {
            >= 0xc000 => (debuggerAddress & 0xffff, 0, (debuggerAddress & 0xff0000) >> 16),
            >= 0xa000 => (debuggerAddress & 0xffff, (debuggerAddress & 0xff0000) >> 16, 0),
            _ => (debuggerAddress & 0xffff, 0, 0)
        };

    public static (int Address, int Bank) GetAddressBank(int debuggerAddress) =>
        (debuggerAddress) switch
        {
            >= 0xc000 => (debuggerAddress & 0xffff, (debuggerAddress & 0xff0000) >> 16),
            >= 0xa000 => (debuggerAddress & 0xffff, (debuggerAddress & 0xff0000) >> 16),
            _ => (debuggerAddress & 0xffff, 0)
        };

    public static string GetDebuggerAddressString(int address, int ramBank, int romBank) =>
        (address, ramBank, romBank) switch
        {
            ( >= 0xc000, _, _) => $"0x{((romBank & 0xff) << 16) + (address & 0xffff):X6}",
            ( >= 0xa000, _, _) => $"0x{((ramBank & 0xff) << 16) + (address & 0xffff):X6}",
            _ => $"0x{address:X6}"
        };

    public static string GetDebuggerAddressDisplayString(int address, int ramBank, int romBank) =>
        (address, ramBank, romBank) switch
        {
            ( >= 0xc000, _, _) => $"{(romBank & 0xff):X2}:{address & 0xffff:X4}",
            ( >= 0xa000, _, _) => $"{(ramBank & 0xff):X2}:{address & 0xffff:X4}",
            _ => $"00:{address:X4}"
        };

    public static string GetDebuggerAddressDisplayString(int debuggerAddress) =>
        debuggerAddress >= 0xa000 ?
        $"{(debuggerAddress & 0xff0000) >> 16:X2}:{debuggerAddress & 0xffff:X4}" :
        $"00:{debuggerAddress:X4}";

    public static (int Address, int RamBank, int RomBank) GetMachineAddress(int debuggerAddress) =>
         (debuggerAddress & 0xffff) switch
         {
             >= 0xc000 => (debuggerAddress & 0xffff, 0, (debuggerAddress & 0xff0000) >> 16),
             >= 0xa000 => (debuggerAddress & 0xffff, (debuggerAddress & 0xff0000) >> 16, 0),
             _ => (debuggerAddress & 0xffff, 0, 0)
         };

    // returns the location in the break point array for a given bank\address
    // second value is returned if the address is currently the active bank
    // breakpoint array:
    // Start      End (-1)     0x:-
    //       0 =>   10,000   : active memory
    //  10,000 =>  210,000   : ram banks
    // 210,000 =>  610,000   : rom banks
    public static (int address, int secondAddress) GetMemoryLocations(int bank, int address) =>
        (bank, address) switch
        {
            (_, < 0xa000) => (address, 0),
            (_, < 0xc000) => (address, 0x10000 + bank * 0x2000 + address - 0xa000),
            _ => (address, 0x210000 + (bank * 0x4000) + address - 0xc000)
        };

    public static (int address, int secondAddress) GetMemoryLocations(int debuggerAddress) =>
        (debuggerAddress & 0xffff) switch
        {
            < 0xa000 => (debuggerAddress, 0),
            < 0xc000 => (debuggerAddress & 0xffff, 0x10000 + (debuggerAddress & 0xffff) - 0xa000 + ((debuggerAddress & 0xff0000) >> 16) * 0x2000), // Ram
            _ => (debuggerAddress & 0xffff, 0x210000 + (debuggerAddress & 0xffff) - 0xc000 + ((debuggerAddress & 0xff0000) >> 16) * 0x4000), // Rom
        };

    public static (int address, int secondAddress) GetMemoryLocations(int address, Emulator emulator) =>
        GetMemoryLocations(GetDebuggerAddress(address, emulator));

    public static int IncrementDebuggerAddress(int debuggerAddress) =>
        (debuggerAddress & 0xffff) switch
        {
            < 0xa000 => debuggerAddress + 1, // normal ram
            < 0xbfff => debuggerAddress + 1, // banked ram
            < 0xc000 => (debuggerAddress & 0xff0000) + 0x01a000, // roll over
            < 0xffff => debuggerAddress + 1, // banked rom
            <= 0xffff => (debuggerAddress & 0xff0000) + 0x01c000, // roll over
            _ => debuggerAddress + 1
        };

    public static int AddDebuggerAddress(int debuggerAddress, int count)
    {
        var address = debuggerAddress & 0xffff;

        if (address < 0xa000)
            return address + count;

        var bank = (debuggerAddress & 0xff0000) >> 16;
        int banksToAdd = 0;

        if (address < 0xc000)
        {
            address -= 0xa000;
            banksToAdd = (count & (0xffffff - 0x1fff)) >> 13;
            bank += banksToAdd;
            address += count & 0x1fff;

            if (address > 0x1fff)
            {
                bank++;
                address &= 0x1fff;
            }
            address += 0xa000;

            return bank * 0x10000 + address;
        }

        address -= 0xc000;
        banksToAdd = (count & (0xffffff - 0x3fff)) >> 13;
        bank += banksToAdd;
        address += count & 0x3fff;

        if (address > 0x3fff)
        {
            bank++;
            address &= 0x3fff;
        }
        address += 0xc000;

        return bank * 0x10000 + address;
    }

    public static int GetOffsetFromDebuggerAddress(int baseDebuggerAddress, int debuggerAddress)
    {
        // normal ram
        if (baseDebuggerAddress < 0xa000)
            return debuggerAddress - baseDebuggerAddress;

        var baseBank = (baseDebuggerAddress & 0xff0000) >> 16;
        var addressBank = (debuggerAddress & 0xff0000) >> 16;
        var baseAddress = (baseDebuggerAddress & 0xffff);
        var address = (debuggerAddress & 0xffff);

        // banked ram
        if (address < 0xc000)
        {
            baseAddress -= 0xa000;
            baseAddress += baseBank * 0x2000;

            address -= 0xa000;
            address += addressBank * 0x2000;

            return address - baseAddress;
        }

        // banked rom
        baseAddress -= 0xc000;
        baseAddress += baseBank * 0x4000;

        address -= 0xc000;
        address += addressBank * 0x4000;

        return address - baseAddress;
    }
}
