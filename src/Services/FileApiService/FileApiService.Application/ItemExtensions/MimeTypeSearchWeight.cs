namespace FileApiService.Application.ItemExtensions;

public class MimeTypeSearchWeight
{
    private static readonly Dictionary<string, double> _weights = new(StringComparer.OrdinalIgnoreCase)
    {
        // Documents
        { "application/pdf", 0.9 },
        { "text/plain", 0.9 },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 0.9 },
        { "application/msword", 0.9 },
        
        // Media
        { "image/jpeg", 0.8 },
        { "image/png", 0.8 },
        { "image/webp", 0.8 },
        { "image/gif", 0.8 },
        { "video/mp4", 0.8 },
        { "video/mpeg", 0.8 },
        { "video/quicktime", 0.8 }, 

        // Tables and presentations
        // Excel 
        { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 0.8 },
        // Excel 
        { "application/vnd.ms-excel", 0.8 },
        // PowerPoint
        { "application/vnd.openxmlformats-officedocument.presentationml.presentation", 0.8 },

        // Archives and audio
        { "application/zip", 0.6 },
        { "application/x-zip-compressed", 0.6 },
        { "application/x-rar-compressed", 0.6 },
        { "application/x-7z-compressed", 0.6 },
        { "audio/mpeg", 0.6 },
        { "audio/wav", 0.6 },
        { "audio/x-wav", 0.6 },

        // trash
        { "application/x-msdownload", 0.1 }, // exe, dll, msi
        { "application/x-dosexec", 0.1 },
        { "application/octet-stream", 0.1 } 
    };
    public static double Get(string? mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType)) 
            return 1; //Folder

        var cleanMime = mimeType.Split(';')[0].Trim();

        return _weights.GetValueOrDefault(cleanMime, 0.5);
    }
}