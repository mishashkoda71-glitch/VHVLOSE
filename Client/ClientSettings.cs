using System.Text.Json;

namespace VHVLOSE.Client;

internal sealed class ClientSettings
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VHVLOSE");
    private static readonly string SettingsPath = Path.Combine(SettingsDirectory, "settings.json");

    public string ProfileName { get; set; } = "Default";

    public static ClientSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return new ClientSettings();

            string json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<ClientSettings>(json) ?? new ClientSettings();
        }
        catch (IOException)
        {
            return new ClientSettings();
        }
        catch (UnauthorizedAccessException)
        {
            return new ClientSettings();
        }
        catch (JsonException)
        {
            return new ClientSettings();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(SettingsDirectory);
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(SettingsPath, json);
    }
}
