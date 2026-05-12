namespace Elppa;

public static class RomLoader
{
    private static string GetRomDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var romsDirectory = Path.Join(currentDirectory, "/roms/");

        return romsDirectory;
    }
    
    public static byte[] LoadRom(string romName)
    {
        var romDir = GetRomDirectory();
        var romFile = Path.Join(romDir, romName);

        if (!File.Exists(romFile))
        {
            throw new FileNotFoundException("Error: ROM not found", romFile);
        }
        
        var romStream = File.ReadAllBytes(romFile);
        
        return romStream;
    }
}