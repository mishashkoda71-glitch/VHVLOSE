using System.Text.Json;

namespace VHVLOSE.Client;

internal sealed class ClientSettings
{
    private static string? gameRoot;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public string ProfileName { get; set; } = "Default";
    public int FormatVersion { get; set; } = 1;

    public static void ConfigureGameRoot(string? path)
    {
        if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            gameRoot = Path.GetFullPath(path);
    }

    public static string? GetConfigDirectory()
    {
        return gameRoot is null ? null : Path.Combine(gameRoot, "VHVLOSE", "configs");
    }

    public static IReadOnlyList<string> ListProfiles()
    {
        string? directory = GetConfigDirectory();
        if (directory is null || !Directory.Exists(directory))
            return [];

        try
        {
            return Directory.GetFiles(directory, "*.vhv")
                .Select(Path.GetFileNameWithoutExtension)
                .OfType<string>()
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    public static ClientSettings Load(string profileName = "Default")
    {
        string? directory = GetConfigDirectory();
        if (directory is null)
            return new ClientSettings { ProfileName = profileName };

        string path = GetProfilePath(directory, profileName);
        try
        {
            if (!File.Exists(path))
                return new ClientSettings { ProfileName = profileName };

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ClientSettings>(json) ??
                   new ClientSettings { ProfileName = profileName };
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return new ClientSettings { ProfileName = profileName };
        }
    }

    public string Save()
    {
        string? directory = GetConfigDirectory();
        if (directory is null)
            throw new InvalidOperationException("CS2 installation directory was not provided by the launcher.");

        Directory.CreateDirectory(directory);
        string path = GetProfilePath(directory, ProfileName);
        string json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(path, json);
        return path;
    }

    private static string GetProfilePath(string directory, string profileName)
    {
        string safeName = string.Concat(profileName
            .Where(character => !Path.GetInvalidFileNameChars().Contains(character)))
            .Trim();

        if (safeName.Length == 0 || safeName.Length > 32)
            throw new ArgumentException("Profile name must contain 1–32 valid filename characters.");

        return Path.Combine(directory, safeName + ".vhv");
    }
}
