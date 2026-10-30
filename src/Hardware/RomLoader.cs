namespace Elppa.Hardware;

public static class RomLoader
{
    public static byte[] LoadRom(string name)
    {
        using var stream = typeof(RomLoader).Assembly
                               .GetManifestResourceStream($"roms/{name}")
                           ?? throw new FileNotFoundException($"Embedded ROM not found: {name}");

        var data = new byte[stream.Length];
        stream.ReadExactly(data);
        return data;
    }
}