namespace FileApiService.Application.ItemExtensions;

public struct ExtensionWeights
{
    private static readonly Dictionary<ItemExtensions, double> _weights = new()
    {
        // Folders
        { ItemExtensions.Folder, 1.0 },

        // Documents
        { ItemExtensions.Pdf,  0.9 },
        { ItemExtensions.Docx, 0.9 },
        { ItemExtensions.Txt,  0.9 },

        // Media
        { ItemExtensions.Jpg,  0.8 },
        { ItemExtensions.Png,  0.8 },
        { ItemExtensions.Mp4,  0.8 },

        // Music and archive
        { ItemExtensions.Zip,  0.6 },
        { ItemExtensions.Mp3,  0.6 },

        // Trash
        { ItemExtensions.Exe,  0.1 },
        { ItemExtensions.Bat,  0.1 },
        { ItemExtensions.Dll,  0.0 }
    };
    public static double Get(ItemExtensions ext)
    {
        return _weights.GetValueOrDefault(ext, 0.5);
    }
}